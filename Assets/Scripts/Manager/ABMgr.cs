using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;
using YooAsset;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

public class ABMgr : SingletonMono<ABMgr>
{
    public bool HasInit { get; private set; }
    private Action _initFinishCb;
    private ResourcePackage _package;
    private bool _hotUpdate = false;

    private Dictionary<string, byte[]> _luaDataMap;

    private Dictionary<string, ABRecord> _loadABRecords;

    public override void Awake()
    {
        base.Awake();
        HasInit = false;
    }

    public void Init(Action finishCb)
    {
        _initFinishCb = finishCb;
        _loadABRecords = new Dictionary<string, ABRecord>();
        _luaDataMap = new Dictionary<string, byte[]>();

        StartCoroutine(InitPackage());
    }

    private void InitFinish()
    {
        HasInit = true;
        if (_initFinishCb != null)
        {
            _initFinishCb();
        }
    }

    private IEnumerator InitPackage()
    {
        //资源部初始化
        YooAssets.Initialize();
        var package = YooAssets.TryGetPackage("DefaultPackage");
        if(package == null)
        {
            package = YooAssets.CreatePackage("DefaultPackage");
        }
        _package = package;

        InitializationOperation initOperation = null;

#if UNITY_EDITOR
        Debug.Log(" ===============>>   编辑器");
        var buildResult = EditorSimulateModeHelper.SimulateBuild("DefaultPackage");
        var initParameters = new EditorSimulateModeParameters();
        initParameters.EditorFileSystemParameters = FileSystemParameters.CreateDefaultEditorFileSystemParameters(buildResult.PackageRootDirectory);
        initOperation = package.InitializeAsync(initParameters);
#else
        Debug.Log(" ===============>>   模拟真机");
        if (_hotUpdate)
        {
            string defaultHostServer = "http://127.0.0.1/CDN/Android/v1.0";
            string fallbackHostServer = "http://127.0.0.1/CDN/Android/v1.0";
            IRemoteServices remoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);
            var initParameters = new HostPlayModeParameters();
            initParameters.BuildinFileSystemParameters = FileSystemParameters.CreateDefaultBuildinFileSystemParameters();
            initParameters.CacheFileSystemParameters = FileSystemParameters.CreateDefaultCacheFileSystemParameters(remoteServices);
            initOperation = package.InitializeAsync(initParameters);
        }
        else
        {
            var initParameters = new OfflinePlayModeParameters();
            initParameters.BuildinFileSystemParameters = FileSystemParameters.CreateDefaultBuildinFileSystemParameters();
            initOperation = package.InitializeAsync(initParameters);
    }
#endif
        yield return initOperation;

        if (initOperation.Status != EOperationStatus.Succeed)
        {
            Debug.LogError(initOperation.Error);
            yield break;
        }

        //资源包在初始化成功之后，需要获取包裹版本
        var requetVersionOp = package.RequestPackageVersionAsync(false);
        yield return requetVersionOp;
        if (requetVersionOp.Status != EOperationStatus.Succeed)
        {
            Debug.LogError(requetVersionOp.Error);
            yield break;
        }
        Debug.Log($"Request package Version : {requetVersionOp.PackageVersion}");

        //更新资源清单
        var updateManifestOp = package.UpdatePackageManifestAsync(requetVersionOp.PackageVersion);
        yield return updateManifestOp;
        if (updateManifestOp.Status != EOperationStatus.Succeed)
        {
            Debug.LogError(updateManifestOp.Error);
            yield break;
        }

        StartCoroutine(loadLuaAssetCo());
    }

    private class RemoteServices : IRemoteServices
    {
        private readonly string _defaultHostServer;
        private readonly string _fallbackHostServer;

        public RemoteServices(string defaultHostServer, string fallbackHostServer)
        {
            _defaultHostServer = defaultHostServer;
            _fallbackHostServer = fallbackHostServer;
        }
        string IRemoteServices.GetRemoteMainURL(string fileName)
        {
            return $"{_defaultHostServer}/{fileName}";
        }
        string IRemoteServices.GetRemoteFallbackURL(string fileName)
        {
            return $"{_fallbackHostServer}/{fileName}";
        }
    }

    IEnumerator loadLuaAssetCo()
    {
        if (GlobalLuaEnv.readFromStreaming)
        {
            yield return LoadAssetsCo("main.lua", typeof(TextAsset));
        }
    }

    public IEnumerator LoadAssetsCo(string assetPath, Type type)
    {
        ABRecord record;
        if(!_loadABRecords.TryGetValue(assetPath, out record))
        {
            AllAssetsHandle allAssetsHandle = _package.LoadAllAssetsAsync(assetPath, type);
            record = new ABRecord(assetPath, assetPath, allAssetsHandle);
            _loadABRecords[assetPath] = record;

            yield return allAssetsHandle;
        
            record.AddRef();
            if (!record.HandleBase.IsValid)
            {
                UnloadAsset(assetPath);
                yield break;
            }

            if (record.HandleBase.IsDone)
            {
                foreach (TextAsset assetObj in allAssetsHandle.AllAssetObjects)
                {
                    _luaDataMap.Add(assetObj.name, assetObj.bytes);
                }
                UnloadAsset(assetPath);
                InitFinish();
            }
            else
            {
                (record.HandleBase as AllAssetsHandle).Completed += (AllAssetsHandle handle) =>
                {
                    foreach (TextAsset assetObj in allAssetsHandle.AllAssetObjects)
                    {
                        _luaDataMap.Add(assetObj.name, assetObj.bytes);
                    }
                    UnloadAsset(assetPath);
                    InitFinish();
                };
            }
        }
    }

    public void UnloadAsset(string assetPath)
    {
        ABRecord record;
        if(_loadABRecords.TryGetValue(assetPath, out record))
        {
            record.Release();
            if(record.Ref <= 0)
            {
                _loadABRecords.Remove(assetPath);
            }
            if (YooAssets.Initialized)
            {
                _package.TryUnloadUnusedAsset(assetPath);
            }
        }
    }

    public byte[] loadLuaFile(string fileName)
    {
        fileName.Replace('\\', '/');
        string path = fileName + ".lua";
        byte[] data = null;
        if(!_luaDataMap.TryGetValue(fileName, out data))
        {
            Debug.LogError("====>> no lua file " + fileName);
        }

        return data;
    }


}

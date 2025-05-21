using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;

public class ABMgr : SingletonMono<ABMgr>
{
    public bool HasInit { get; private set; }
    private Action _initFinishCb;
    private ResourcePackage _package;
    private bool _hotUpdate = false;

    public override void Awake()
    {
        base.Awake();
        HasInit = false;
    }

    public void Init(Action finishCb)
    {
        _initFinishCb = finishCb;

        StartCoroutine(InitPackage());
    }
    
    private IEnumerator InitPackage()
    {
        YooAssets.Initialize();
        var package = YooAssets.TryGetPackage("DefaultPackage");
        if(package == null)
        {
            package = YooAssets.CreatePackage("DefaultPackage");
        }
        _package = package;

        InitializationOperation initOperation = null;

#if UNITY_EDITOR
        Debug.Log(" ===============>>   ±à¼­Æ÷");
        var buildResult = EditorSimulateModeHelper.SimulateBuild("DefaultPackage");
        var initParameters = new EditorSimulateModeParameters();
        initParameters.EditorFileSystemParameters = FileSystemParameters.CreateDefaultEditorFileSystemParameters(buildResult.PackageRootDirectory);
        initOperation = package.InitializeAsync(initParameters);
#else
        Debug.Log(" ===============>>   Ä£ÄâÕæ»ú");
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
        yield return null;
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
}

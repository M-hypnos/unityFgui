using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;
using UnityEngine.Windows;
using System.Text;
using System.Linq;


public class GlobalLuaEnv : SingletonMono<GlobalLuaEnv>
{
    public static bool readFromStreaming = true;
    public LuaEnv luaEnv;
    private IntPtr lPtr;

    private Action _luaInit;
    private Action _luaUpdate;
    private Action _luaExit;

    private bool _isDisposed = false;

    public static byte[] GetUTF8String(byte[] buffer)
    {
        if (buffer == null)
            return null;

        if (buffer.Length <= 3)
        {
            return buffer;
        }

        byte[] bomBuffer = new byte[] { 0xef, 0xbb, 0xbf };

        if (buffer[0] == bomBuffer[0]
            && buffer[1] == bomBuffer[1]
            && buffer[2] == bomBuffer[2])
        {
            return buffer.Skip(3).ToArray();
        }

        return buffer;
    }

    public LuaEnv.CustomLoader loader = (ref string filepath) =>{
        if(!string.IsNullOrEmpty(filepath))
        {

#if UNITY_EDITOR
            filepath = filepath.Replace("/", ".");
            if (GlobalLuaEnv.readFromStreaming)
            {
                return GetUTF8String(ABMgr.Instance.loadLuaFile(filepath + ".lua"));
            }
            else
            {
                filepath = filepath.Replace(".", "/");
                string path = Application.dataPath + "/ABResource/Lua/" + filepath + ".lua.txt";
                if (File.Exists(path))
                {
                    return GetUTF8String(File.ReadAllBytes(path));
                }
            }
#else
        return GetUTF8String(ABMgr.Instance.loadLuaFile(filepath + ".lua"));
#endif

        }
        return null;
    };
    public void Init()
    {
        luaEnv = new LuaEnv();
        luaEnv.AddLoader(loader);

        lPtr = luaEnv.L;

        var results = luaEnv.DoString("return require 'main'");
        LuaTable main = results[0] as LuaTable;
        _luaInit = main.Get<Action>("init");
        _luaUpdate = main.Get<Action>("update");
        _luaExit = main.Get<Action>("exit");


        _luaInit.Invoke();

        _isDisposed = false;
    }

    public void onTick()
    {
        if (_isDisposed) return;
        luaEnv.Tick();
        _luaUpdate.Invoke();
    }

    private void OnDisable()
    {
        if (_luaExit != null)
        {
            _luaExit.Invoke();
        }
    }

    private void OnDestroy()
    {
        _isDisposed = true;
        if(_luaInit != null) _luaInit = null;
        if(_luaUpdate != null) _luaUpdate = null;
        if (_luaExit != null) _luaExit = null;
        if (luaEnv != null)
        {
            luaEnv.Dispose();
            luaEnv = null;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

public class GlobalLuaEnv : SingletonMono<GlobalLuaEnv>
{
    public LuaEnv luaEnv;
    public void Init()
    {
        luaEnv = new LuaEnv();
    }
}

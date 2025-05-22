using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

public class GameEntry : MonoBehaviour
{
    public static GameEntry Instance;
    public GlobalLuaEnv luaEnv;
    private bool _inited = false;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;

        //if (ABMgr.Instance == null || !ABMgr.Instance.HasInit)
        //{
        //    ABMgr.Instance.Init(Init);
        //}
        //else
        //{
            Init();
        //}
    }

    private void Init()
    {

        Debug.Log("============>>  game init");
        StartCoroutine(initCo());
    }

    private IEnumerator initCo()
    {
        yield return null;
        luaEnv = GlobalLuaEnv.Instance;

        luaEnv.Init();


        _inited = true;
    }

    void Update()
    {
        if (!_inited)
            return;
        luaEnv.onTick();
    }
}

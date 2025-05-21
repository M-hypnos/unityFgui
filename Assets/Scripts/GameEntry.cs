using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEntry : MonoBehaviour
{
    public static GameEntry Instance;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;

        if (ABMgr.Instance == null || !ABMgr.Instance.HasInit) { 
            ABMgr.Instance.Init(InitCo);
        }
        else
        {
            InitCo();
        }
    }

    private void InitCo()
    {
        Debug.Log("============>>  game init");
    }
}

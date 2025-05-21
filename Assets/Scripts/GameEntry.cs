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

        if (ABMgr.Instance == null || !ABMgr.Instance.HasInit)
        {
            ABMgr.Instance.Init(Init);
        }
        else
        {
            Init();
        }
    }

    private void Init()
    {

        Debug.Log("============>>  game init");
        StartCoroutine(initCo());
    }

    private IEnumerator initCo()
    {
        yield return null;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;

public class ABRecord : MonoBehaviour
{
    private static int _id = 0;
    public int ID { get; private set; }
    public string Name { get; private set; }
    public string Path { get; private set; }

    public int Ref { get; private set; }

    public HandleBase HandleBase { get; private set; }

    public ABRecord(string name, string path, HandleBase handleBase)
    {
        ID = _id++;
        this.Name = name;
        this.Path = path;
        this.HandleBase = handleBase;
    }

    public void AddRef()
    {
        Ref++;
    }

    public void Release()
    {
        Ref--;
        if(Ref <= 0)
        {
            if(HandleBase != null)
            {
                if(HandleBase is AllAssetsHandle)
                {
                    (HandleBase as AllAssetsHandle).Release();
                }
                HandleBase = null;
            }
        }
    }
}

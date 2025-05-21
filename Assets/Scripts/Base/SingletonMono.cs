using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonMono<T> : MonoBehaviour where T : Component
{
    private static T _instance;
    
    public static T Instance
    {
        get
        {
            if(_instance == null)
            {
                GameObject obj = new GameObject(typeof(T).Name);
                _instance = obj.AddComponent<T>();
                DontDestroyOnLoad(obj);
            }
            return _instance;
        }
    }
    public virtual void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            _instance = null;
            return;
        }
        _instance = this as T;
        DontDestroyOnLoad(gameObject);
    }
}

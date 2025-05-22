using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ABRecord : MonoBehaviour
{
    private static int _id = 0;
    public int ID { get; private set; }
    public string Name { get; private set; }
    public string Path { get; private set; }

    public int Ref { get; private set; }

    public ABRecord(string name, string path)
    {
        ID = _id++;
        this.Name = name;
        this.Path = path;
    }
}

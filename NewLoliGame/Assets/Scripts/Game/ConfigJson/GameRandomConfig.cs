using System;
using UnityEngine;

/// <summary>
/// 姓名配置
/// </summary>
[Serializable]
public class GameRandomConfig
{
    public int type;
    public string content;
    //姓氏
    public static readonly int SURNNAME = 1;
    //名称
    public static readonly int NAME = 2;

    public void print()
    {
        Debug.Log(" [ type=" + type + "  content=" + content+ " ]");
    }
}


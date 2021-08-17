using System;
using UnityEngine;

/// <summary>
/// 属性配置表
/// </summary>

[Serializable]
public class PlayerAttrLevelConfig
{
    public int level_id;
    public int charm;
    public int evn;
    public int intell;
    public int mana;
    public string consume;

    public static readonly int MAX_LEVEL = 100;
    public void print()
    {
        Debug.Log("[ level_id=" + level_id + ", charm=" + charm + ", evn=" + evn + ", intell=" + intell + ", mana=" + mana + ", consume=" + consume + " ]");
    }
}

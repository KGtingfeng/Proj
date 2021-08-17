using System;
using UnityEngine;


/// <summary>
/// 玩家等级配置
/// </summary>

[Serializable]
public class PlayerLevelConfig
{
    public int level_id;
    public string name_cn;
    public string logo;
    public int reg;
    public string color;
    public int status;
    public int add_mid_num;
    public int out_mid_num;
    /// <summary>
    /// 智力
    /// </summary>
    public int charm;
    /// <summary>
    /// 环保
    /// </summary>
    public int evn;
    /// <summary>
    /// 智力
    /// </summary>
    public int intell;
    /// <summary>
    /// 魔法
    /// </summary>
    public int mana;



    public void print()
    {
        Debug.Log("[ level_id=" + level_id +  ", name_cn=" + name_cn + ", evn=" + evn + ", intell=" + intell + ", mana=" + mana + ", charm=" + charm + " ]");
    }
}


using System;
using UnityEngine;

[Serializable]
public class GameInitCardsConfig
{
    /// <summary>
    /// 拥有属性 玩家可以拥有
    /// </summary>
    public static readonly int TYPE_DOLL = 1;
    /// <summary>
    ///不用有属性 剧情使用
    /// </summary>
    public static readonly int TYPE_ROLE = 2;

    public int card_id;
    public int init_level;
    public int skin_id;
    public int prop_id;
    public string OwnSkins;
    public string name_cn;
    public string logo;
    /// <summary>
    /// //true 仙子 false 人类
    /// </summary>
    public bool status;
    public int charm;
    public int evn;
    public int intell;
    public int mana;
    public string story;
    public int type;
    public string price;
    /// <summary>
    /// 0女 1男
    /// </summary>
    public int gender;

    public string info;



    public void print()
    {
        Debug.Log(" [ cardid=" + card_id + "  init_level=" + init_level + " name_cn=" + name_cn + " logo=" + logo +
        " status=" + status + " charm=" + charm + " evn=" + evn + " intell=" + intell + " mana=" + mana + " story=" + story + " ]");

    }


}




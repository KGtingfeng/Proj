using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RankDataMgr
{
    static RankDataMgr ins;
    public static RankDataMgr Instance
    {
        get
        {
            if (ins == null)
            {
                ins = new RankDataMgr();
            }
            return ins;
        }
    }

    public readonly List<string> typeName = new List<string>
    {
        "好感",
        "时刻",
        "总属性",
        "魅力",
        "智慧",
        "环保",
        "魔法",
    };

    public RankType RankType { get; set; }

    public RankAll FavorRankAll { get; set; }
    public RankAll TimeRankAll { get; set; }
    public RankAll AttrRankAll { get; set; }

    public RankSingle RankSingle { get; set; }


}

public enum RankType
{
    //好感
    Favor,
    //时刻
    Time,
    //总属性
    Attr,
    //魅力
    Charm,
    //智慧
    Intell,
    //环保
    Eve,
    //魔法
    Mana,
}

using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
///  
/// </summary>
[Serializable]
public class GameMallConfig
{
    /// <summary>
    /// ID
    /// </summary>
    public int mall_id;

    /// <summary>
    /// 名字
    /// </summary>
    public string name;

    /// <summary>
    /// 类型 [0:RMB,1:钻石,2:星星,999:不显示]
    /// </summary>
    public int type;

    /// <summary>
    /// 消耗数量
    /// </summary>
    public string cost;

    /// <summary>
    /// 解锁消耗
    /// </summary>
    public string unlock_cost;

    /// <summary>
    /// 商城道具描述
    /// </summary>
    public string description;

    /// <summary>
    /// 折扣
    /// </summary>
    public int discount;

    /// <summary>
    /// 物品ID
    /// </summary>
    public int prop_id;

    /// <summary>
    /// 奖励
    /// </summary>
    public string award;

    /// <summary>
    /// 道具标签
    /// </summary>
    public int label_type;

    /// <summary>
    /// 限制类型,[0:普通,1:个人限购,2:全服限购,3:只vip购买]
    /// </summary>
    public int limit_type;

    /// <summary>
    /// 限制数量
    /// </summary>
    public int limit_num;

    /// <summary>
    /// 物品位置
    /// </summary>
    public int rank;

    /// <summary>
    /// 需要的VIP等级
    /// </summary>
    public int need_vip;

    /// <summary>
    /// 限时开关
    /// </summary>
    public int has_limit_time;

    /// <summary>
    /// 开始时间
    /// </summary>
    public string limit_start_time;

    /// <summary>
    /// 结束时间
    /// </summary>
    public string limit_end_time;

    /// <summary>
    /// 月卡表示每天发放的钻石
    /// </summary>
    public string extra;

}

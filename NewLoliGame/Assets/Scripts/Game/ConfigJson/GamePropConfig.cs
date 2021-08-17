
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Response 
/// </summary>
[Serializable]
public class GamePropConfig
{
    /// <summary>
    /// 不显示
    /// </summary>
    public static readonly int NO_SHOW_TYPE = -1;
    /// <summary>
    /// 可以使用
    /// </summary>
    public static readonly int CAN_USE_TYPE = 1;
    /// <summary>
    /// 碎片道具
    /// </summary>
    public static readonly int DEBRIS_PROPS_TYPE = 2;
    /// <summary>
    /// 好感度道具
    /// </summary>
    public static readonly int FAVOR_PROPS_TYPE = 3;

    /// <summary>
    /// 礼包道具
    /// </summary>
    public static readonly int PACKAGE_PROPS_TYPE = 6;
    /// <summary>
    /// 角色背景道具
    /// </summary>
    public static readonly int BACKGROUND_PROPS_TYPE = 7;

    public long prop_id;
	
	public string prop_name;
	
	public string description;
    // -1:不显示 1:可以使用的道具2：碎片道具3:好感度5：娃娃服装（暂定）6:礼包道具 7角色背景（暂定）
    public int prop_type;
    // 道具价格0为不可卖，大于0就可以卖
    public double sell_price;
    // 从1开始，越大越好品质
    public int quality;
    // 使用限制，0就没有
    public int limit;
    /// <summary>
	/// 如果是礼包，配置这里, prop_id,prop_type,quantity,gravity;
	/// </summary>
    public string pack_list;

    //1:商城
    public string from;

    public string used;

    public TinyItem Used
    {
        get
        {
            return ItemUtil.GetTinyItem(used);
        }
    }
}

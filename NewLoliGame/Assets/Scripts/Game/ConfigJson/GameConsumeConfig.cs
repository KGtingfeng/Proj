

using System;

/// <summary>
/// 消耗配置
/// </summary>
[Serializable]
public class GameConsumeConfig
{
    /// <summary>
    /// 剧情重置
    /// </summary>
    public static readonly int STORY_RESET_TYPE = 1;
    /// <summary>
    /// 跳过关卡
    /// </summary>
    public static readonly int STORY_SKIP_NODE_TYPE = 2;
    /// <summary>
    /// 直接过关
    /// </summary>
    public static readonly int STORY_PASS_NODE_TYPE = 3;
    /// <summary>
    /// 修改玩家姓名
    /// </summary>
    public static readonly int MODIFY_NAME = 4;
    /// <summary>
    /// 修改玩家昵称
    /// </summary>
    public static readonly int MODIFY_NICKNAME = 5;
    /// <summary>
    /// 通过钻石购买星星
    /// </summary>
    public static readonly int BUY_STAR_BY_DIAMOND = 6;

    public int id;
    public int type;
    /// <summary>
    /// 消耗数据
    /// </summary>
    public string pay;
    public string award;

    public TinyItem tinyItem
    {
        get
        {
            return ItemUtil.GetTinyItem(pay);
        }
    }

    public TinyItem Award
    {
        get
        {
            return ItemUtil.GetTinyItem(award);
        }
    }
}

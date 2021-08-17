using System;

[Serializable]
public class GameGuideConfig
{
    public const int GuideActor = 18;

    public const int TYPE_ROLE_GROUP = 4;
    public const int TYPE_ROLE_UPGRADE = 5;
    public const int TYPE_ATTRIBUTE_UPGRADE = 6;
    public const int TYPE_DOLL_UPGRADE = 7;
    public const int TYPE_APPEARANCE = 8;
    public const int TYPE_SEND_GIFT = 9;
    public const int TYPE_ALARM = 10;
    public const int TYPE_MONENT = 11; 
    public const int TYPE_ADDRESS = 12; 

    public int id;
    /// <summary>
    ///大步骤
    /// </summary>
    public int flow;
    /// <summary>
    ///小步骤
    /// </summary>
    public int step;
    public int type;
    public string actor;
    /// <summary>
    /// 角色坐标
    /// </summary>
    public string actor_axis;
    /// <summary>
    /// 角色声音
    /// </summary>
    public string actor_voice;

    /// <summary>
    ///说话内容
    /// </summary>
    public string contents;
    /// <summary>
    /// 对话框坐标
    /// </summary>
    public string contents_axis;
    /// <summary>
    /// 手指坐标
    /// </summary>
    public string cursor_axis;
    public string bgm;
    /// <summary>
    /// 需要保存
    /// </summary>
    public int need_save;
    /// <summary>
    /// 回到哪一步
    /// </summary>
    public string roll_to;

    /// <summary>
    ///下一步
    /// </summary>
    public string next_to;


}

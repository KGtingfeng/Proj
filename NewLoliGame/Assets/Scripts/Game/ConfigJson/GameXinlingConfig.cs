using System;

[Serializable]
public class GameXinlingConfig
{
    /// <summary>
    /// 制作娃娃
    /// </summary>
    public const int TYPE_MAKEDOLLS = 1;
    /// <summary>
    /// 打捞垃圾
    /// </summary>
    public const int TYPE_PICKUP = 2;
    /// <summary>
    /// 垃圾分类
    /// </summary>
    public const int TYPE_CLASSIFICATION = 3;
    /// <summary>
    /// 找茬
    /// </summary>
    public const int TYPE_FIND = 4;

    public int type;
    public string title;
    public string description;
    public string ckey;
    public string cval;
    public string award;

}

using System;

[Serializable]
public class GameXinlingLjflConfig
{
    public int id;
    /// <summary>
    /// 有害垃圾
    /// </summary>
    public const int TYPE_HARMFUL = 1;
    /// <summary>
    /// 可回收垃圾
    /// </summary>
    public const int TYPE_RECYCLABLE = 2;
    /// <summary>
    /// 厨余垃圾
    /// </summary>
    public const int TYPE_KITCHENWASTE = 3;
    /// <summary>
    /// 其他垃圾
    /// </summary>
    public const int TYPE_OTHER = 4;

    public int type;
    public string ckey;
    public string cval;
    public string description;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 送礼、制作、学习弹框数据
/// </summary>
public class GiftItem 
{
    /// <summary>
    /// 赠送
    /// </summary>
    public static readonly int SEND_GIFT = 0;
    /// <summary>
    /// 制作
    /// </summary>
    public static readonly int MAKE_GIFT = 1;
    /// <summary>
    /// 学习
    /// </summary>
    public static readonly int  LEARN_GIFT = 2;

    //区分赠送、制作、学习
    public int type;
    public int id;
}

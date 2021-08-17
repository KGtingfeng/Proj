using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpineConfig
{
    /// <summary>
    /// 空闲
    /// </summary>
    public static readonly int AC_TYPE_IDLE = 1;
    /// <summary>
    /// 小动作
    /// </summary>
    public static readonly int AC_TYPE_POSE = 2;
    /// <summary>
    /// 愤怒
    /// </summary>
    public static readonly int AC_TYPE_ANGRY = 3;
    /// <summary>
    /// 高兴
    /// </summary>
    public static readonly int AC_TYPE_HAPPY = 4;
    /// <summary>
    /// 忧伤
    /// </summary>
    public static readonly int AC_TYPE_SAD = 5;
    /// <summary>
    /// 腼腆
    /// </summary>
    public static readonly int AC_TYPE_SHY = 6;
    /// <summary>
    /// 说话
    /// </summary>
    public static readonly int AC_TYPE_TALK = 7;

    public static List<int> talkSpine = new List<int>() { 10, 11, 13, 15, 18 };

    /// <summary>
    /// spine通用动画名字
    /// </summary>
    static Dictionary<int, string> posKeyPairs = new Dictionary<int, string>()
    {
        {1,"idle"},
        {2,"pose"},
        {3,"angry"},
        {4,"happy"},
        {5,"sad"},
        {6,"shy"},
        {7,"talk"},
        {8,"dswx"},
        {9,"hy"},
        {10,"hy_talk"},
        {11,"idle_talk"},
        {12,"jt"},
        {13,"jt_talk"},
        {14,"ssjc"},
        {15,"ssjc_talk"},
        {16,"wq"},
        {17,"ys"},
        {18,"ys_talk"},
    };


    /// <summary>
    /// The spine orders. 动画Track index
    /// </summary>
    static Dictionary<int, int> spineOrders = new Dictionary<int, int>()
    {
        {1,0},
        {2,1},
        {3,1},
        {4,1},
        {5,1},
        {6,1},
        {7,2},
        {8,1},
        {9,1},
        {10,1},
        {11,1},
        {12,1},
        {13,1},
        {14,1},
        {15,1},
        {16,1},
        {17,1},
        {18,1},
    };


 
    /// <summary>
    /// 获取spine动画 名字和track index
    /// </summary>
    /// <param name="id">Identifier.</param>
    /// <param name="name">Name.</param>
    /// <param name="order">Order.</param>
    public static void GetSpineAnimationInfo(int id,out string name,out int order)
    {
        name = "idle";
        order = 0;
        if (posKeyPairs.ContainsKey(id))
            name = posKeyPairs[id];

        if (spineOrders.ContainsKey(id))
            order = spineOrders[id];
    }
 

}

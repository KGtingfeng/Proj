using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class GameTimelineNodeConfig
{
    public int id;
    /// <summary>
    /// 朋友圈id
    /// </summary>
    public int sms_id;
    /// <summary>
    /// 对话id
    /// </summary>
    public int point_id;
    /// <summary>
    /// 是否跳过
    /// </summary>
    public int isPass;
    public string awards;
}

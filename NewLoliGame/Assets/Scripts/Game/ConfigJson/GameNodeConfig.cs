using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameNodeConfig 
{
    public int id;
    /// <summary>
    /// 章节id
    /// </summary>
    public int chapter_id;
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

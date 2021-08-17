using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 玩家剧情信息
/// </summary>
///
[Serializable]
public class PlayerStoryInfo
{
    //故事状态，-1代表未开启，1 已重置 0通关中关，2代表完全通关

    /// <summary>
    /// 未开启
    /// </summary>
    public const int TYPE_NOT_OPEN = -1;
    /// <summary>
    /// 进行中
    /// </summary>
    public const int TYPE_PROCEING = 0;
    /// <summary>
    /// 已经重置
    /// </summary>
    public const int TYPE_RESTART = 1;
    /// <summary>
    /// 2完全通关
    /// </summary>
    public const int TYPE_OVER = 2;

    /// <summary>
    /// 节点开始位置
    /// </summary>
    public const int START_NODE = 1;

    public int story_status;
    /// <summary>
    /// 角色id
    /// </summary>
    public int actor_id;
    /// <summary>
    /// 章节id
    /// </summary>
    public int chapter_id;
    /// <summary>
    /// 节点id
    /// </summary>
    public int node_id;

    /// <summary>
    /// 节点数据
    /// </summary>
    public string pass_node;
    /// <summary>
    /// 奖励节点数据
    /// </summary>
    public string award_node;
    /// <summary>
    /// 奖励
    /// </summary>
    public string award;

    /// <summary>
    /// 升级信息
    /// </summary>
    public PlayerStoryInfoExtra extra;
    /// <summary>
    /// 结局
    /// </summary>
    public string end_node;

}

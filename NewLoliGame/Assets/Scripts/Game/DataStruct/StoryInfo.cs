
using System;
using System.Collections.Generic;
/// <summary>
/// Story info. 剧情信息
/// </summary>
public class StoryInfo
{
    /// <summary>
    /// 角色id
    /// </summary>
    public int actor_id;
    /// <summary>
    /// 章节id
    /// </summary>
    public int chapterId;
    /// <summary>
    /// 节点id
    /// </summary>
    public int node_id;
    /// <summary>
    /// 剧情连表
    /// </summary>
    public GameNodeConfig gameNodeConfig;
    /// <summary>
    /// 是否需要记录
    /// </summary>
    public bool needUpload;
    //public int uploadNodeId;
    public int story_status;
    /// <summary>
    /// 是否是重看章节
    /// </summary>
    public bool isReRead;
    /// <summary>
    /// 用于管理重看章节的节点
    /// </summary>
    public List<int> nodes;


    /// <summary>
    /// 移除已经播放的剧情节点
    /// </summary>
    public void RemoveNodes()
    {
        if (nodes != null && nodes.Count > 0)
            nodes.RemoveAt(0);
    }

}

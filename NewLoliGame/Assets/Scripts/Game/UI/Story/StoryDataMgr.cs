using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// 剧情数据管理
/// </summary>
public class StoryDataMgr
{

    public static StoryDataMgr _storyDataMgr;
    public static StoryDataMgr ins
    {
        get
        {
            if (_storyDataMgr == null)
                _storyDataMgr = new StoryDataMgr();
            return _storyDataMgr;

        }
    }

    private StoryDataMgr() { }


    /// <summary>
    /// 剧情节点信息
    /// </summary>
    StoryInfo _storyInfo;
    public StoryInfo StoryInfo
    {
        get { return _storyInfo; }
        set { _storyInfo = value; }
    }

    /// <summary>
    /// 添加重看章节节点
    /// </summary>
    /// <param name="nodes">Nodes.</param>
    public void AddStoryReReadNodes(List<int> nodes)
    {
        if (_storyInfo.nodes == null)
            _storyInfo.nodes = new List<int>();
        _storyInfo.nodes.Clear();
        _storyInfo.nodes.AddRange(nodes);
    }

    public int lastBgId = -1;

    /// <summary>
    /// 剧情信息
    /// </summary>
    public List<PlayerStoryInfo> storyInfos;


    /// <summary>
    /// 角色剧情信息 最外层【角色层】
    /// </summary>
    public PlayerStoryInfo roleStoryInfo;

    /// <summary>
    /// 具体章节信息 初始值从点击章节获取
    /// </summary>
    public PlayerChapterInfo playerChapterInfo;


    public List<int> saveNodes = new List<int>();
    public List<int> videoNodes = new List<int>();

    public TinyItem timeItem;

    /// <summary>
    /// 获取具体章节数据 用于节点推进
    /// </summary>
    /// <returns>The story info.</returns>
    /// <param name="chapterId">Chapter identifier.</param>
    public StoryInfo GetStoryInfo(GameChapterConfig chapterConfig)
    {
        List<GameChapterConfig> chapterConfigs = JsonConfig.GameChapterConfigs.FindAll(a => a.actor_id == chapterConfig.actor_id);
        int index = chapterConfigs.IndexOf(chapterConfig);

        PlayerStoryInfo storyInfo = storyInfos.Find(a => a.chapter_id == chapterConfigs[index].id && a.actor_id == chapterConfig.actor_id);
        //通关 或者未通关可重看章节
        _storyInfo = null;
        if (storyInfo.story_status == PlayerStoryInfo.TYPE_OVER)//|| storyInfo.chapter_id > chapterConfig.id)
        {
            _storyInfo = new StoryInfo();
            _storyInfo.actor_id = storyInfo.actor_id;
            _storyInfo.chapterId = chapterConfig.id;
            //这里就表示从第一个节点开始
            _storyInfo.node_id = chapterConfig.startPoint;
            _storyInfo.isReRead = true;
            _storyInfo.story_status = storyInfo.story_status;
        }
        else
        {
            _storyInfo = new StoryInfo();
            _storyInfo.actor_id = storyInfo.actor_id;
            _storyInfo.chapterId = storyInfo.chapter_id;
            _storyInfo.node_id = storyInfo.node_id;
            _storyInfo.story_status = storyInfo.story_status;
            _storyInfo.isReRead = false;
        }

        return _storyInfo;
    }

    /// <summary>
    /// 初始化具体角色剧情信息
    /// </summary>
    /// <param name="actorId">Actor identifier.</param>
    public void InitRoleStoryInfo(int actorId)
    {
        roleStoryInfo = QueryStoryInfo(actorId);
        if (roleStoryInfo == null)
        {
            Debug.LogError("Role actorId " + actorId + "  is null!!!!");
        }
    }


    public PlayerStoryInfo QueryStoryInfo(int actorId)
    {
        List<PlayerStoryInfo> infos = storyInfos.FindAll(a => a.actor_id == actorId);
        PlayerStoryInfo playerStory;
        if (infos.Count == 0)
            playerStory = null;
        else
            playerStory = infos[0];
        foreach (PlayerStoryInfo info in infos)
        {
            playerStory = info;
            if (info.story_status == 0 || info.story_status == 1)
                break;
        }
        return playerStory;
    }
    /// <summary>
    /// 查询真实章节号
    /// </summary>
    public int QueryStoryRealChapter(int chapter, int actorId)
    {
        List<PlayerStoryInfo> infos = storyInfos.FindAll(a => a.actor_id == actorId);
        PlayerStoryInfo playerStory = infos.Find(a => a.chapter_id == chapter);
        return infos.IndexOf(playerStory);
    }

    /// <summary>
    /// 刷新节点数据
    /// </summary>
    /// <param name="playerStoryInfo">Player story info.</param>
    public void RefreshRoleStoryInfo(PlayerStoryInfo playerStoryInfo)
    {
        //先同步外层 保持数据一致
        int index = storyInfos.FindIndex(a => a.actor_id == playerStoryInfo.actor_id && a.chapter_id == playerStoryInfo.chapter_id);

        if (index >= 0)
        {
            storyInfos[index].chapter_id = playerStoryInfo.chapter_id;
            storyInfos[index].node_id = playerStoryInfo.node_id;
            storyInfos[index].story_status = playerStoryInfo.story_status;
            storyInfos[index].end_node = playerStoryInfo.end_node;

            roleStoryInfo = storyInfos[index];

        }
        else
        {
            List<PlayerStoryInfo> infos = storyInfos.FindAll(a => a.actor_id == playerStoryInfo.actor_id);
            foreach (PlayerStoryInfo info in infos)
            {
                if (info.story_status != PlayerStoryInfo.TYPE_OVER)
                {
                    info.story_status = PlayerStoryInfo.TYPE_OVER;
                    break;
                }
            }
            PlayerStoryInfo storyInfo = new PlayerStoryInfo();
            storyInfo.chapter_id = playerStoryInfo.chapter_id;
            storyInfo.node_id = playerStoryInfo.node_id;
            storyInfo.actor_id = playerStoryInfo.actor_id;
            storyInfo.story_status = playerStoryInfo.story_status;
            storyInfo.extra = roleStoryInfo.extra;
            storyInfo.end_node = roleStoryInfo.end_node;
            roleStoryInfo = storyInfo;

            storyInfos.Add(storyInfo);
        }
        //同步具体章节信息
        playerChapterInfo.SychronizedPoints(playerStoryInfo.pass_node, playerStoryInfo.award_node);

        //弹出具体奖励
        if (!string.IsNullOrEmpty(playerStoryInfo.award) && playerStoryInfo.award != "0")
        {

            List<TinyItem> tinyItems = ItemUtil.GetTinyItmeList(playerStoryInfo.award);
            foreach (TinyItem item in tinyItems)
            {
                switch (item.type)
                {
                    case (int)TypeConfig.Consume.Time:
                        //EventMgr.Ins.DispachEvent(EventConfig.GET_DOLL, item);
                        timeItem = item;
                        break;
                    case (int)TypeConfig.Consume.EXP:
                    case (int)TypeConfig.Consume.Friendly:
                        EventMgr.Ins.DispachEvent(EventConfig.STORY_SHOW_GET_EFFECT, playerStoryInfo);
                        break;
                    case (int)TypeConfig.Consume.Item:
                    case (int)TypeConfig.Consume.AvatarFrame:
                    case (int)TypeConfig.Consume.Title:
                    case (int)TypeConfig.Consume.Diamond:
                    case (int)TypeConfig.Consume.Star:
                        EventMgr.Ins.DispachEvent(EventConfig.AWARD_GET_SINGLE, item);
                        break;
                }
            }
        }
    }

    /// <summary>
    /// 重置剧情
    /// </summary>
    /// <param name="playerStoryInfo">Player story info.</param>
    public void ResetRoleStoryInfo(List<PlayerStoryInfo> playerStoryInfo)
    {
        for (int i = playerStoryInfo.Count - 1; i >= 0; i--)
        {
            PlayerStoryInfo storyInfo = storyInfos.Find(a => a.chapter_id == playerStoryInfo[i].chapter_id);
            storyInfo.story_status = playerStoryInfo[i].story_status;
            storyInfo.node_id = JsonConfig.GameChapterConfigs.Find(a => a.id == storyInfo.chapter_id).startPoint;
        }
    }

    public void Dispose()
    {
        _storyInfo = null;
        roleStoryInfo = null;
        storyInfos = null;
        playerChapterInfo = null;
    }


}

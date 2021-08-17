using System;
using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

public class StorySelectChapterMoudle : BaseMoudle
{
    public static StorySelectChapterMoudle instance;
    GList loopList;
    NormalInfo normalInfo;
    PlayerStoryInfo roleStoryInfo
    {
        get { return StoryDataMgr.ins.roleStoryInfo; }
    }

    readonly List<string> itemUrl = new List<string>
    {
        "ui://6f05wxg5mjv5d",
        "ui://6f05wxg5w8ufq47",
    };

    public override void InitMoudle(GComponent gComponent, int controllerIndex)
    {
        base.InitMoudle(gComponent, controllerIndex);
        InitEvent();
        instance = this;
        loopList = SearchChild("n8").asList;
    }

    List<GameChapterConfig> gameChapterConfig;
    public override void InitData<D>(D data)
    {
        base.InitData(data);
        normalInfo = data as NormalInfo;
        if (normalInfo != null)
        {
            //获取章节列表
            gameChapterConfig = DataUtil.GetChapters(normalInfo.index);
            if (gameChapterConfig != null && gameChapterConfig.Count > 0)
            {
                InitList();
            }
        }
        //释放对话相关信息
        EventMgr.Ins.DispachEvent(EventConfig.STORY_DIALOG_DISPOSE);
       
    }

    public override void InitData()
    {
        base.InitData();
        if (normalInfo != null)
        {
            //获取章节列表
            gameChapterConfig = DataUtil.GetChapters(normalInfo.index);
            if (gameChapterConfig != null && gameChapterConfig.Count > 0)
            {
                InitList();
            }
        }
        //释放对话相关信息
        EventMgr.Ins.DispachEvent(EventConfig.STORY_DIALOG_DISPOSE);

        NewbieGotoChapter();
    }

    void InitList()
    {
        loopList.RemoveChildrenToPool();
        for (int i = 0; i < gameChapterConfig.Count; i++)
        {
            RenderListItem(i);
        }
        GameChapterConfig chapterConfig = gameChapterConfig.Find(a => a.id == roleStoryInfo.chapter_id);
        loopList.ScrollToView(gameChapterConfig.IndexOf(chapterConfig));
    }

    void RenderListItem(int index)
    {
        GButton item = loopList.AddItemFromPool(itemUrl[index % 2]).asButton;
        string title = gameChapterConfig[index].title.Replace('\n', ' ');
        item.title = title; //gameChapterConfig[index].title;

        int id = roleStoryInfo.chapter_id < gameChapterConfig[index].id ? 0 : index + 1;
        GLoader underLoader = item.GetChild("n3").asLoader;
        GLoader upLoader = item.GetChild("n4").asLoader;

        if (id == 0)//未开启章节
        {
            underLoader.url = UrlUtil.GetChapterHeadIconUrl(-1);//未开启底图
            upLoader.url = UrlUtil.GetChapterHeadIconUrl(0);
            item.onClick.Set(() =>
            {
                UIMgr.Ins.showErrorMsgWindow("该章节尚未解锁!");
            });
        }
        else
        {
            underLoader.url = UrlUtil.GetChapterHeadIconUrl(-2);//已开启底图
            upLoader.url = UrlUtil.GetChapterHeadIconUrl(id);
            item.onClick.Set(() =>
            {
                OpenNewChapter(gameChapterConfig[index], index);
            });
        }
    }

    public override void InitEvent()
    {
        SearchChild("n18").onClick.Add(() =>
        {
            NewbieReback();
        });
    }

    public void OpenNewChapter(GameChapterConfig config, int index)
    {

        RequestChapterInfo(config, () =>
        {
            baseView.StartCoroutine(GotoEffect(config));
            // GoToChapter(config);
        }, index);

    }

    public IEnumerator GotoEffect(GameChapterConfig config)
    {
        TouchScreenView.Ins.PlayTmpEffect();
        yield return new WaitForSeconds(0.8f);
        if (GameData.isGuider)
        {
            EventMgr.Ins.DispachEvent(EventConfig.SHANBAI_HIDE);
        }
        GoToChapter(config);
    }

    private void GoToChapter(GameChapterConfig config)
    {
        List<GameChapterConfig> chapterConfigs = JsonConfig.GameChapterConfigs.FindAll(a => a.actor_id == config.actor_id);
        int index = chapterConfigs.IndexOf(config) + 1;

        StoryInfo storyInfo = StoryDataMgr.ins.GetStoryInfo(config);
        if (storyInfo == null)
        {
            Debug.Log("not find chapter: " + config.id);
            return;
        }
        //1、该章节属于正在通关章节
        if (!storyInfo.isReRead && config.id == storyInfo.chapterId)
        {

            //章节开始
            if (roleStoryInfo.node_id == config.startPoint)
            {
                EventMgr.Ins.DispachEvent(EventConfig.STORY_CHAPTER_STARTING, config);
            }
            else
            {
                storyInfo.gameNodeConfig = DataUtil.GetNodeConfig(config.id, roleStoryInfo.node_id);
                storyInfo.node_id = roleStoryInfo.node_id;
                EventMgr.Ins.DispachEvent(EventConfig.STORY_TRIGGER_LOOP, storyInfo);
            }
            return;
        }

        //2、该章节属于已经过关章节 那就是需要重看的章节 重看章节特殊处理
        //重看章节其实就是播放所有的对话
        //章节重看，自动播放所有的节点即可

        if (storyInfo.isReRead)
        {
            //章节选择节点
            List<int> passNodePoint = new List<int>();
            passNodePoint.AddRange(StoryDataMgr.ins.playerChapterInfo.GetPointsQueue);

            //这个属于对话播放队列
            List<int> nodesQueue = DataUtil.GetChapterPassNodesForDialog(config.startPoint, passNodePoint);
            StoryDataMgr.ins.AddStoryReReadNodes(nodesQueue);
            EventMgr.Ins.DispachEvent(EventConfig.STORY_CHAPTER_STARTING, config);
            return;


        }
        //3、该章节属于为解锁章节

        if (index > storyInfo.chapterId)
        {
            UIMgr.Ins.showErrorMsgWindow(MsgException.STORY_CHAPTER_NOT_OPEN);
        }



    }


    /// <summary>
    /// 获取章节相关数据
    /// </summary>
    void RequestChapterInfo(GameChapterConfig config, Action callback, int index)
    {
        WWWForm wWForm = new WWWForm();
        wWForm.AddField("actorId", config.actor_id);
        wWForm.AddField("chapterId", config.id);

        GameMonoBehaviour.Ins.RequestInfoPost<PlayerChapterInfo>(NetHeaderConfig.STORY_CHAPTER_INFO, wWForm, callback);
    }

    public void NewbieGotoChapter()
    {
        GameChapterConfig config = JsonConfig.GameChapterConfigs.Find(a => a.id == 1);
        StoryDataMgr.ins.InitRoleStoryInfo(GameGuideConfig.GuideActor);

        OpenNewChapter(config, 0);
    }


    public void NewbieReback()
    {
        EventMgr.Ins.DispachEvent(EventConfig.STORY_GOTO_SELECT_STORY);
    }
}

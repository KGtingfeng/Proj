using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;


[ViewAttr("Game/UI/J_Story", "J_Story", "Story", true)]
public class StoryView : BaseView
{
    GGraph backgroundGraph;
    GComponent topCom;
    public static StoryView view;
    public enum MoudleType
    {
        SelectStory = 0,
        SelectChapter,
        Chapter_Starting,//章节序幕
        Story_Dialog,//剧情对话
        Story_Transition, //过场
        Story_MultSel, //剧情多选择 
        Story_Combine, //表盘
        Story_SelectWatch = 9,//选择表盘
        Story_Animation,//过渡动画
        Story_NotEnoughAttrihute = 7,//属性不足
        Story_Playr_Record = 8,//剧情回顾
    };




    Dictionary<MoudleType, string> moudleNamePairs = new Dictionary<MoudleType, string>()
    {
        {MoudleType.SelectStory,"n0"},
        {MoudleType.SelectChapter,"n1"},
        {MoudleType.Chapter_Starting,"n32"},//剧情序幕 开始的时候
        {MoudleType.Story_Dialog,"n35"},//剧情对话
        {MoudleType.Story_Transition,"n33"},//过场
        {MoudleType.Story_MultSel,"n36"},//多剧情选择
        {MoudleType.Story_Combine,"n37"},//表盘合成
        {MoudleType.Story_SelectWatch,"n42"},//选择表盘
        {MoudleType.Story_Animation,"n44"},//过渡动画
        {MoudleType.Story_NotEnoughAttrihute,"n37"},//属性不足
        {MoudleType.Story_Playr_Record,"n41"},//剧情回顾
    };


    StoryInfo storyInfo
    {
        get
        {
            return StoryDataMgr.ins.StoryInfo;
        }
    }

    int backGroundMusicId = -1;

    public override void InitUI()
    {
        view = this;
        base.InitUI();
        ui.opaque = false;
        controller = ui.GetController("c1");
        backgroundGraph = SearchChild("n35").asCom.GetChild("n17").asGraph;
        topCom = SearchChild("n44").asCom;
        InitEvent();
    }

    public override void InitData()
    {
        OnShowAnimation();
        backGroundMusicId = -1;
        StoryDataMgr.ins.saveNodes.Clear();
        GoToStorySelect();
        GetTopText();
        //if (GameData.isGuider)
        //{
        //    UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);
        //}
    }

    public override void InitData<D>(D data)
    {
        base.InitData(data);
        OnShowAnimation();
        backGroundMusicId = -1;
        StoryDataMgr.ins.saveNodes.Clear();
        GoToXinlingStory();
        GetTopText();
    }


    public override void InitEvent()
    {
        RegisterEvent();
        //click loveBtn星星
        topCom.GetChild("n13").onClick.Set(() =>
        {
            UIMgr.Ins.showNextPopupView<BuyStarsView>();
        });
        //click diamondBtn 钻石
        topCom.GetChild("n14").onClick.Set(() =>
        {
            UIMgr.Ins.showNextPopupView<ShopMallView>();
        });
        SearchChild("n43").onClick.Set(() =>
        {
            StoryDataMgr.ins.Dispose();
            UIMgr.Ins.showMainView<StoryView>();
        });

        EventMgr.Ins.RegisterEvent(EventConfig.REFRESH_STORY_TOP_INFO, GetTopText);

    }
    public void GetTopText()
    {
        topCom.GetChild("n15").asTextField.text = GameData.Player.love + "";
        topCom.GetChild("n16").asTextField.text = GameData.Player.diamond + "";
    }

    #region 注册事件


    void RegisterEvent()
    {
        EventMgr.Ins.RegisterEvent(EventConfig.STORY_BREAKE_MAIN, RebackToMain);

        EventMgr.Ins.RegisterEvent(EventConfig.STORY_BREACK_STORY, RebackStory);
        EventMgr.Ins.RegisterEvent<NormalInfo>(EventConfig.STORY_INTO_CHAPTER, GoToSelectChapter);
        //触发剧情
        EventMgr.Ins.RegisterEvent<StoryInfo>(EventConfig.STORY_TRIGGER_LOOP, TriggerStory);
        //开启新章节
        EventMgr.Ins.RegisterEvent<GameChapterConfig>(EventConfig.STORY_CHAPTER_STARTING, GoToChapterStarting);
        //过场
        EventMgr.Ins.RegisterEvent<GamePointConfig>(EventConfig.STORY_GOTO_TRANSITION, GoToTransitionMoudel);
        //触发下一个节点
        EventMgr.Ins.RegisterEvent<NormalInfo>(EventConfig.STORY_TRIGGER_NEXT_NODE, TriggetNextNode);

        //刷新存储记录点
        EventMgr.Ins.RegisterEvent<PlayerStoryInfo>(EventConfig.STORY_REFRESH_NODE, RefreshPlayerSotryInfo);

        //前往属性不足界面
        EventMgr.Ins.RegisterEvent<StoryAttributeWindowInfo>(EventConfig.STORY_GOTO_ATTRIBUTE, GoToAttributeNotEnoughWindow);

        //记录节点
        EventMgr.Ins.RegisterEvent<NormalInfo>(EventConfig.STORY_RECORD_SELECT_NODE, TriggetNextNodeForSelect);
        //播放剧情回顾
        EventMgr.Ins.RegisterEvent(EventConfig.STORY_PLAY_RECORD, PlayRecord);
        //跳过节点
        EventMgr.Ins.RegisterEvent<NormalInfo>(EventConfig.STORY_SKIP_NODE, CostItemForMoveNextNode);
        //前往剧情选择
        EventMgr.Ins.RegisterEvent(EventConfig.STORY_GOTO_SELECT_STORY, GoToStorySelect);

        //背景音乐注册事件
        EventMgr.Ins.RegisterEvent<GamePointConfig>(EventConfig.MUSIC_STORY_BG_MUSIC, PlayBgMusic);
        //剧情音效
        EventMgr.Ins.RegisterEvent<GamePointConfig>(EventConfig.MUSIC_STORY_EFFECT, PlayEffectMusic);
        //获得好感特效
        EventMgr.Ins.RegisterEvent<PlayerStoryInfo>(EventConfig.STORY_SHOW_GET_EFFECT, ShowGetEffect);
    }

    void GoToStorySelect()
    {
        GoToMoudle<StorySelectMoudle>((int)MoudleType.SelectStory);
    }

    public void GoToXinlingStory()
    {
        GoToMoudle<StorySelectMoudle, NormalInfo>((int)MoudleType.SelectStory, null);
    }

    /// <summary>
    /// 前往章节
    /// </summary>
    /// <param name="normalInfo">Normal info.</param>
    void GoToSelectChapter(NormalInfo normalInfo)
    {
        Debug.Log("basecount: " + baseMoudles.Count);
        GoToMoudle<StorySelectChapterMoudle, NormalInfo>((int)MoudleType.SelectChapter, normalInfo);
    }

    Extrand commonTipsInfo;
    void RebackStory()
    {
        if (commonTipsInfo == null)
        {
            commonTipsInfo = new Extrand();
            commonTipsInfo.key = "提示";
            commonTipsInfo.msg = "确认离开剧情，回到剧情选择界面？";
            commonTipsInfo.callBack = () =>
            {
                AudioClip audioClip = ResourceUtil.LoadBackGroundMusic(SoundConfig.CommonMusicId.BgId);
                GRoot.inst.PlayBgSound(audioClip);
                GoToSelectChapter();
            };
        }
        UIMgr.Ins.showNextPopupView<PopupDialogView, Extrand>(commonTipsInfo);

    }

    void RebackToMain()
    {
        GRoot.inst.StopDialogSound();
        backGroundMusicId = -1;
        EventMgr.Ins.DispachEvent(EventConfig.MUSIC_COMMON_BG_MUSIC);
        EventMgr.Ins.DispachEvent(EventConfig.REFRESH_PLAYER_PROPERTY_INFO);
        EventMgr.Ins.DispachEvent(EventConfig.STORY_GAME_QUIT);
        EventMgr.Ins.DispachEvent(EventConfig.HIDE_TIPS_PANEL);
        UIMgr.Ins.showMainView<StoryView>();
    }

    void GoToSelectChapter()
    {
        GRoot.inst.StopDialogSound();

        backGroundMusicId = -1;
        EventMgr.Ins.DispachEvent(EventConfig.MUSIC_COMMON_BG_MUSIC); 
        //EventMgr.Ins.DispachEvent(EventConfig.SYCHRONIZED_PLAYER_INFO);
        EventMgr.Ins.DispachEvent(EventConfig.REFRESH_PLAYER_PROPERTY_INFO);
        StartCoroutine(CreateTransitionEffect());
        Debug.LogError("story is over....");

    }

    IEnumerator CreateTransitionEffect()
    {
        TouchScreenView.Ins.PlayTmpEffect();
        yield return new WaitForSeconds(0.8f);
        EventMgr.Ins.DispachEvent(EventConfig.STORY_GAME_QUIT);
        EventMgr.Ins.DispachEvent(EventConfig.HIDE_TIPS_PANEL);
        NormalInfo normalInfo = new NormalInfo();
        normalInfo.index = storyInfo.actor_id;
        GoToSelectChapter(normalInfo);
    }

    /// <summary>
    /// 开始新章节
    /// </summary>
    /// <param name="gameChapterConfig">Game chapter config.</param>
    void GoToChapterStarting(GameChapterConfig gameChapterConfig)
    {

        GoToMoudle<StoryChapterStaringMoudle, GameChapterConfig>((int)MoudleType.Chapter_Starting, gameChapterConfig);
    }


    void GoToTransitionMoudel(GamePointConfig gamePointConfig)
    {
        GoToMoudle<StoryTransitionMoudle, GamePointConfig>((int)MoudleType.Story_Transition, gamePointConfig);
    }


    void GoToDialogMoudle(GamePointConfig gamePointConfig)
    {
        GoToMoudle<StoryDialogMoudle, GamePointConfig>((int)MoudleType.Story_Dialog, gamePointConfig);
    }


    void GoToMultSelectDialogsMoudle(GamePointConfig gamePointConfig)
    {

        //过度剧情的内容  平滑问题
        if (StoryDialogMoudle.Ins != null)
        {
            StoryDialogMoudle.Ins.ResetDialogPerson(() =>
            {
                GoToMoudle<StoryMultSelectMoudle, GamePointConfig>((int)MoudleType.Story_MultSel, gamePointConfig);

            });
        }
        else
        {
            GoToMoudle<StoryMultSelectMoudle, GamePointConfig>((int)MoudleType.Story_MultSel, gamePointConfig);

        }

    }


    void GoToCombineWatchMoudle(GamePointConfig gamePointConfig)
    {
        GoToMoudle<StoryCombienWatchMoudle, GamePointConfig>((int)MoudleType.Story_Combine, gamePointConfig);

    }

    void GoToSelectWatchMoudle(GamePointConfig gamePointConfig)
    {
        GoToMoudle<StorySelectWatchMoudel, GamePointConfig>((int)MoudleType.Story_SelectWatch, gamePointConfig);
    }



    void GoToAttributeNotEnoughWindow(StoryAttributeWindowInfo storyAttributeWindowInfo)
    {
        UIMgr.Ins.showNextPopupView<StoryNotEnoughAttributeWindow, StoryAttributeWindowInfo>(storyAttributeWindowInfo);
    }


    void GoToGameView<T>(GamePointConfig gamePointConfig)
    {
        GameDetailConfig gameDetailConfig = JsonConfig.GameDetailConfigs.Find(a => a.id == gamePointConfig.type);
        gameDetailConfig.callback = () =>
        {
            RequestRecordNode(() =>
            {
                RefreshStoryNode();

                StoryCacheMgr.storyCacheMgr.GetStoryGameSaves(gamePointConfig.id, (List<StoryGameSave> storyGameSave) =>
                {
                    StoryGameInfo storyGameInfo = new StoryGameInfo();
                    storyGameInfo.gamePointConfig = DataUtil.GetPointConfig(storyInfo.node_id);
                    storyGameInfo.gameSaves = storyGameSave;
                    UIMgr.Ins.showNextPopupView<T, StoryGameInfo>(storyGameInfo);
                });


                /*
                WWWForm wWWForm = new WWWForm();
                wWWForm.AddField("nodeId", gamePointConfig.id);



                GameMonoBehaviour.Ins.RequestInfoPost(NetHeaderConfig.STROY_LOAD_GAME, wWWForm,
                    (List<StoryGameSave> storyGameSave) =>
                    {
                        StoryGameInfo storyGameInfo = new StoryGameInfo();
                        storyGameInfo.gamePointConfig = DataUtil.GetPointConfig(storyInfo.node_id);
                        storyGameInfo.gameSaves = storyGameSave;
                        UIMgr.Ins.showNextPopupView<T, StoryGameInfo>(storyGameInfo);
                    });*/
            });
        };

        //TouchScreenView.Ins.PlayChangeEffect(null);
        UIMgr.Ins.showNextPopupView<GameDetailView, GameDetailConfig>(gameDetailConfig);

    }

    /// <summary>
    /// 播放视频
    /// </summary>
    void PlayVideo(GamePointConfig gamePointConfig)
    {
        bool isPlay = StoryDataMgr.ins.videoNodes.Contains(gamePointConfig.id);
        if (!isPlay)
        {
            StoryDataMgr.ins.videoNodes.Add(gamePointConfig.id);
            Debug.LogError("SPS***************   PlayVideo");
            CallBackList callBackList = new CallBackList();
            callBackList.callBack1 = () =>
            {
                UIMgr.Ins.showNextPopupView<StoryVideoView, GamePointConfig>(gamePointConfig);
            };


            if (StoryDialogMoudle.Ins != null)
            {
                StoryDialogMoudle.Ins.ResetDialogPerson(() =>
                {
                    UIMgr.Ins.showNextPopupView<StoryAnimationView, CallBackList>(callBackList);
                });
            }
            else { UIMgr.Ins.showNextPopupView<StoryAnimationView, CallBackList>(callBackList); }



        }
    }

    /// <summary>
    ///  剧情回顾
    /// </summary>
    /// <param name="normalInfo">Normal info.</param>
    void GoToPlayRecordMoudle(NormalInfo normalInfo)
    {
        //UIMgr.Ins.showWindow<StoryPlayRecordWindow, NormalInfo>(normalInfo);
        UIMgr.Ins.showNextPopupView<StoryPlayRecordView, NormalInfo>(normalInfo);
    }



    /// <summary>
    /// 显示过场动画
    /// </summary>
    /// <param name="callback">Callback.</param>
    void ShowStoryAnimationView(Action callback)
    {
        UIMgr.Ins.ShowViewWithoutHideBefore<StoryAnimationView, Action>(callback);
    }

    /// <summary>
    /// 隐藏过场动画
    /// </summary>
    void HideStoryAnimation()
    {
        UIMgr.Ins.HideView<StoryAnimationView>();
    }


    void RefreshPlayerSotryInfo(PlayerStoryInfo playerStory)
    {
        StoryDataMgr.ins.RefreshRoleStoryInfo(playerStory);
    }



    void PlayRecord()
    {
        NormalInfo normalInfo = new NormalInfo();
        normalInfo.index = storyInfo.node_id;
        GoToPlayRecordMoudle(normalInfo);
        //PlayerStoryInfo info = new PlayerStoryInfo();
        //info.extra = new PlayerStoryInfoExtra();
        //info.extra.favour = "100;600";
        //info.actor_id = 11;
        //ShowGetEffect(info);
    }




    #endregion




    #region 核心内容

    /**************剧情核心部分  开始*********************/



    void RefreshGameNodeConfig(StoryInfo storyInfo)
    {
        StoryDataMgr.ins.StoryInfo = storyInfo;
    }

    /// <summary>
    /// 触发剧情
    /// </summary>
    /// <param name="storyInfo">Story info.</param>
    void TriggerStory(StoryInfo storyInfo)
    {
        //不是相同的节点 需要标记上传
        storyInfo.needUpload = storyInfo.node_id != storyInfo.gameNodeConfig.point_id;
        GamePointConfig gamePointConfig = DataUtil.GetPointConfig(storyInfo.gameNodeConfig.point_id);
        if (gamePointConfig != null)
        {
            
            //避免漏洞点击
            if (gamePointConfig.type == (int)TypeConfig.StoyType.TYPE_ROLE ||
                gamePointConfig.type == (int)TypeConfig.StoyType.TYPE_SELF ||
                gamePointConfig.type == (int)TypeConfig.StoyType.TYPE_ASIDE ||
                gamePointConfig.type == (int)TypeConfig.StoyType.TYPE_TRANSITION)
            {
                RefreshStoryNode();
            }
            //单机测试用 实际数据这里要屏蔽调
            //RefreshStoryNode();
            SelectStoryStep(gamePointConfig);
            //处理背景音乐 
            PlayBgMusic(gamePointConfig);

        }




    }
    /// <summary>
    /// 更新节点信息
    /// </summary>
    void RefreshStoryNode()
    {
        storyInfo.node_id = storyInfo.gameNodeConfig.point_id;
        RefreshGameNodeConfig(storyInfo);
    }


    /// <summary>
    /// 普通触发下一个节点
    /// </summary>
    /// <param name="normalInfo">Normal info.</param>
    void TriggetNextNode(NormalInfo normalInfo)
    {
        Debug.Log("   TriggetNextNode node:  普通触发下一个节点" + storyInfo.chapterId + "  _ " + normalInfo.index);
        GameNodeConfig gameNodeConfig = DataUtil.GetNodeConfig(storyInfo.chapterId, normalInfo.index);
        EventMgr.Ins.DispachEvent(EventConfig.STORY_GAME_QUIT);
        if (gameNodeConfig != null)
        {
            //Debug.Log("TriggetNextNode:" + gameNodeConfig.id + "  " + gameNodeConfig.point_id);
            storyInfo.gameNodeConfig = gameNodeConfig;
            TriggerStory(storyInfo);

        }
        else
        {
            //剧情结束
            if (normalInfo.index == 0)
            {

                ResetChapterOver(() =>
                {
                    EventMgr.Ins.DispachEvent(EventConfig.REFRESH_PLAYER_PROPERTY_INFO);
                    normalInfo.index = storyInfo.actor_id;
                    int chapterNum = JsonConfig.GameStoryConfigs.Find(a => a.actor_id == storyInfo.actor_id).chapter_num;
                    GameChapterConfig gameChapter = JsonConfig.GameChapterConfigs.Find(a => a.actor_id == storyInfo.actor_id);
                    if (storyInfo.chapterId >= gameChapter.id + chapterNum - 1)//如果是最后一个章节结束，则回到章节选择界面。
                    {
                        AudioClip audioClip = ResourceUtil.LoadBackGroundMusic(SoundConfig.CommonMusicId.BgId);
                        GRoot.inst.PlayBgSound(audioClip);
                        StartCoroutine(StartGoToSelectChapter(normalInfo));
                    }
                    else//否则跳转到下一章节
                    {
                        StartCoroutine(OpenNewChapter(gameChapterConfigs[storyInfo.chapterId], storyInfo.chapterId));
                    }
                    //GoToSelectChapter(normalInfo);

                });

                PlayerStoryInfo playerStory = StoryDataMgr.ins.storyInfos.Find(a => a.chapter_id == storyInfo.chapterId);
                playerStory.story_status = PlayerStoryInfo.TYPE_OVER;
            }
            Debug.LogError("last point_id " + normalInfo.index);
        }
    }


    List<GameChapterConfig> gameChapterConfigs
    {
        get { return JsonConfig.GameChapterConfigs; }
    }

    #region 剧情结束，打开新的章节
    IEnumerator OpenNewChapter(GameChapterConfig config, int index)
    {
        TouchScreenView.Ins.PlayTmpEffect();
        yield return new WaitForSeconds(0.8f);//过场动画之后进入新的章节
        RequestChapterInfo(config, () =>
        {
            //StartCoroutine(GotoEffect(config));
            GoToChapter(config);
        }, index);
    }

    //IEnumerator GotoEffect(GameChapterConfig config)
    //{
    //    TouchScreenView.Ins.PlayTmpEffect();
    //    yield return new WaitForSeconds(0.8f);

    //    GoToChapter(config);
    //}

    PlayerStoryInfo roleStoryInfo
    {
        get { return StoryDataMgr.ins.roleStoryInfo; }
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

        //if (index > storyInfo.chapterId)
        //{
        //    Debug.Log("*********为解锁章节");
        //    UIMgr.Ins.showErrorMsgWindow(MsgException.STORY_CHAPTER_NOT_OPEN);
        //}



    }
    void RequestChapterInfo(GameChapterConfig config, Action callback, int index)
    {
        WWWForm wWForm = new WWWForm();
        wWForm.AddField("actorId", config.actor_id);
        wWForm.AddField("chapterId", config.id);

        GameMonoBehaviour.Ins.RequestInfoPost<PlayerChapterInfo>(NetHeaderConfig.STORY_CHAPTER_INFO, wWForm, callback);
    }
    #endregion  ************剧情结束，打开新的章节**********

    IEnumerator StartGoToSelectChapter(NormalInfo normalInfo)
    {
        TouchScreenView.Ins.PlayTmpEffect();
        yield return new WaitForSeconds(0.8f);
        GoToSelectChapter(normalInfo);
    }

    void ResetChapterOver(Action callBack)
    {
        //章节回放不需要记录
        if (StoryDataMgr.ins.StoryInfo.isReRead)
        {

            callBack();
            return;
        }
        Debug.LogError("reset over state");
        bool isContainNode = StoryDataMgr.ins.playerChapterInfo.GetPointsQueue.Contains(storyInfo.gameNodeConfig.point_id);
        bool isSaveNode = StoryDataMgr.ins.saveNodes.Contains(storyInfo.gameNodeConfig.point_id);

        if (!isSaveNode)
        {
            StoryDataMgr.ins.saveNodes.Add(storyInfo.gameNodeConfig.point_id);
            WWWForm wWForm = new WWWForm();
            wWForm.AddField("actorId", storyInfo.actor_id);
            wWForm.AddField("chapterId", storyInfo.chapterId);
            wWForm.AddField("nodeId", 0);
            GameMonoBehaviour.Ins.RequestInfoPost<PlayerStoryInfo>(NetHeaderConfig.STORY_RECORD_NODE, wWForm, callBack);

        }
    }

    /// <summary>
    /// 记录选择以后的更新节点
    /// </summary>
    /// <param name="normalInfoCallBack">Normal info call back.</param>
    void TriggetNextNodeForSelect(NormalInfo normalInfo)
    {

        storyInfo.needUpload = true;
        storyInfo.node_id = normalInfo.index;
        //这里是特殊处理节点
        storyInfo.gameNodeConfig.point_id = normalInfo.index;
        //正式版
        RequestRecordNode(() =>
        {
            TriggetNextNode(normalInfo);
        });

        //单机版
        //TriggetNextNode(normalInfo);

    }



    void SelectStoryStep(GamePointConfig gamePointConfig)
    {
        Debug.Log("<color=yellow>剧情具体步骤:</color> 类型= " + gamePointConfig.type + "  id=" + gamePointConfig.id);
        switch (gamePointConfig.type)
        {
            case (int)TypeConfig.StoyType.TYPE_ROLE:
                GoToDialogMoudle(gamePointConfig);
                break;
            case (int)TypeConfig.StoyType.TYPE_SELF:
                GoToDialogMoudle(gamePointConfig);
                break;
            case (int)TypeConfig.StoyType.TYPE_ASIDE:
                GoToDialogMoudle(gamePointConfig);
                break;
            case (int)TypeConfig.StoyType.TYPE_TRANSITION:
                //storyInfo.needUpload = true;
                //RequestRecordNode(() =>
                //{

                //    RefreshStoryNode();
                //    GoToTransitionMoudel(gamePointConfig);
                //});
                EventMgr.Ins.DispachEvent(EventConfig.STORY_GOTO_TRANSITION, gamePointConfig);
                break;
            case (int)TypeConfig.StoyType.TYPE_ROLE_BG_ENLARGE:
                GoToDialogMoudle(gamePointConfig);
                break;
            case (int)TypeConfig.StoyType.TYPE_SELF_BG_ENLARGE:
                GoToDialogMoudle(gamePointConfig);
                break;
            case (int)TypeConfig.StoyType.TYPE_ASIDE_BG_ENLARGE:
                GoToDialogMoudle(gamePointConfig);
                break;
            case (int)TypeConfig.StoyType.TYPE_TRANSITION_BG_ENLARGE:
                //storyInfo.needUpload = true;
                //RequestRecordNode(() =>
                //{

                //    RefreshStoryNode();
                //    GoToTransitionMoudel(gamePointConfig);
                //});
                EventMgr.Ins.DispachEvent(EventConfig.STORY_GOTO_TRANSITION, gamePointConfig);
                break;
            case (int)TypeConfig.StoyType.TYPE_PLOT:
                RequestRecordNode(() =>
                {
                    RefreshStoryNode();
                    GoToMultSelectDialogsMoudle(gamePointConfig);
                });

                break;
            case (int)TypeConfig.StoyType.TYPE_ATTRIBUTE:
                RequestRecordNode(() =>
                {
                    RefreshStoryNode();
                    GoToMultSelectDialogsMoudle(gamePointConfig);
                });
                break;
            case (int)TypeConfig.StoyType.TYPE_COMBINE:
                GameDetailConfig gameDetailConfig = JsonConfig.GameDetailConfigs.Find(a => a.id == gamePointConfig.type);
                gameDetailConfig.callback = () =>
                {
                    RequestRecordNode(() =>
                    {
                        RefreshStoryNode();
                        GoToCombineWatchMoudle(gamePointConfig);
                    });
                };
                UIMgr.Ins.showNextPopupView<GameDetailView, GameDetailConfig>(gameDetailConfig);
                break;
            case (int)TypeConfig.StoyType.TYPE_SEARCH:

                GameDetailConfig config = JsonConfig.GameDetailConfigs.Find(a => a.id == gamePointConfig.type);
                config.callback = () =>
                {
                    RequestRecordNode(() =>
                    {
                        RefreshStoryNode();
                        GoToSelectWatchMoudle(gamePointConfig);
                    });
                };
                UIMgr.Ins.showNextPopupView<GameDetailView, GameDetailConfig>(config);
                break;
            case (int)TypeConfig.StoyType.TYPE_FIND:

                GoToGameView<GameFindGemsView>(gamePointConfig);

                break;
            case (int)TypeConfig.StoyType.TYPE_PUZZLE:

                GoToGameView<GamePuzzleView>(gamePointConfig);

                break;
            case (int)TypeConfig.StoyType.TYPE_JUMP_SHIP:

                GoToGameView<GameJumpShipView>(gamePointConfig);

                break;
            case (int)TypeConfig.StoyType.TYPE_PUZZLE_WATCH:

                GoToGameView<GamePuzzleWatchView>(gamePointConfig);

                break;
            case (int)TypeConfig.StoyType.TYPE_VIDEO:
                RequestRecordNode(() =>
                {
                    RefreshStoryNode();
                    PlayVideo(gamePointConfig);
                });
                break;
            case (int)TypeConfig.StoyType.TYPE_SELECT_TIME:

                GoToGameView<GameSelectTimeView>(gamePointConfig);
                break;

            case (int)TypeConfig.StoyType.TYPE_PUZZLE_FOUR_CLOCK:

                GoToGameView<GamePuzzleFourClockView>(gamePointConfig);
                break;

            case (int)TypeConfig.StoyType.TYPE_WORD:

                GoToGameView<GameWordView>(gamePointConfig);

                break;
            case (int)TypeConfig.StoyType.TYPE_CONNECT_JINMAI:

                GoToGameView<GameConnectJinmaiView>(gamePointConfig);

                break;
            case (int)TypeConfig.StoyType.TYPE_ANSWER:

                GoToGameView<GameAnswerView>(gamePointConfig);

                break;
            case (int)TypeConfig.StoyType.TYPE_GEM_FILLING:

                GoToGameView<GameGemFillingView>(gamePointConfig);

                break;
            case (int)TypeConfig.StoyType.TYPE_IMAGE_SELECT:

                UIMgr.Ins.showNextPopupView<StoryImageSelectView, GamePointConfig>(gamePointConfig);

                break;
            case (int)TypeConfig.StoyType.TYPE_ClICK_CLOCK:

                GoToGameView<GameClickClockView>(gamePointConfig);

                break;
            case (int)TypeConfig.StoyType.TYPE_BELL_TOWER_FIND:

                GoToGameView<GameBellTowerFindView>(gamePointConfig);

                break;
            case (int)TypeConfig.StoyType.TYPE_BELL_TOWER_PUZZLE:

                GoToGameView<GamePuzzleBellTowerView>(gamePointConfig);

                break;
            case (int)TypeConfig.StoyType.TYPE_RUINS_FIND:

                GoToGameView<GameRuinsFindView>(gamePointConfig);

                break;
            case (int)TypeConfig.StoyType.TYPE_AVIOD_BLUE:

                GoToGameView<GameAviodView>(gamePointConfig);

                break;
            case (int)TypeConfig.StoyType.TYPE_ACROSS_BUILDING:

                GoToGameView<GameAcrossBuildingView>(gamePointConfig);

                break;
            case (int)TypeConfig.StoyType.TYPE_FIND_DOOR_SP:

                GoToGameView<FindDoorSPGameView>(gamePointConfig);

                break;
            case (int)TypeConfig.StoyType.TYPE_DEFEND_MDL:

                GoToGameView<GameClickDefendsView>(gamePointConfig);

                break;
            case (int)TypeConfig.StoyType.TYPE_ATTACT_MDL:

                GoToGameView<GameAttackMDLView>(gamePointConfig);

                break;
            case (int)TypeConfig.StoyType.TYPE_PUZZLE_DOOR:

                GoToGameView<GamePuzzleDoorView>(gamePointConfig);

                break;
            case (int)TypeConfig.StoyType.TYPE_SISI_PIANO:

                GoToGameView<GameMusicView>(gamePointConfig);

                break;
            case (int)TypeConfig.StoyType.TYPE_Thunder:
                GoToGameView<GameThunderView>(gamePointConfig);
                break;
            default:
                break;


        }

    }

    /// <summary>
    /// 更细剧情点
    /// </summary>
    /// <param name="callBack">Call back.</param>
    void RequestRecordNode(Action callBack)
    {
        //该节点是否已经记录过了
        bool isContainNode = StoryDataMgr.ins.playerChapterInfo.GetPointsQueue.Contains(storyInfo.gameNodeConfig.point_id);
        bool isSaveNode = StoryDataMgr.ins.saveNodes.Contains(storyInfo.gameNodeConfig.point_id);

        //需要更新该节点没有存储过 章节回放不需要记录
        if (storyInfo.needUpload && !isContainNode && !isSaveNode && !StoryDataMgr.ins.StoryInfo.isReRead)
        {
            StoryDataMgr.ins.saveNodes.Add(storyInfo.gameNodeConfig.point_id);
            WWWForm wWForm = new WWWForm();
            wWForm.AddField("actorId", storyInfo.actor_id);
            wWForm.AddField("chapterId", storyInfo.chapterId);
            wWForm.AddField("nodeId", storyInfo.gameNodeConfig.point_id);
            Debug.LogError("upload: actorId=" + storyInfo.actor_id + "  chapterId: " + storyInfo.chapterId + " nodeId: " + storyInfo.gameNodeConfig.point_id);
            GameMonoBehaviour.Ins.RequestInfoPost<PlayerStoryInfo>(NetHeaderConfig.STORY_RECORD_NODE, wWForm, callBack);
        }
        else
        {
            callBack();
        }

    }



    public void RequestNodePassResult(GamePointConfig gamePointConfig, Action callback)
    {
        WWWForm wWForm = new WWWForm();
        Debug.LogError(storyInfo.node_id + " _ " + gamePointConfig.id);
        wWForm.AddField("nodeId", storyInfo.node_id);
        wWForm.AddField("actorId", StoryDataMgr.ins.roleStoryInfo.actor_id);
        wWForm.AddField("chapterId", storyInfo.chapterId);
        GameMonoBehaviour.Ins.RequestInfoPost<PlayerStoryInfo>(NetHeaderConfig.STORY_JUDGE_NODE, wWForm, callback);
    }


    /// <summary>
    /// 消耗道具过关卡节点
    /// </summary>
    void CostItemForMoveNextNode(NormalInfo consume)
    {

        //当前的节点 成功以后触发下一个节点 
        int nodeId = storyInfo.node_id;
        int pointId = storyInfo.gameNodeConfig.id;
        int chosenId = DataUtil.GetPointConfig(pointId).point1;
        //传入的参数不一样 分开处理
        if (consume.index < 0)
            chosenId = consume.noIndex;

        Debug.LogError("skip param: nodeId=" + nodeId + "  _pointId= " + pointId + "  chosenId=" + chosenId);

        GameConsumeConfig gameConsumeConfig = DataUtil.GetConsumeConfig(consume.type);

        //花钱以后直接进行到当前的下一个节点
        System.Action callback = () =>
        {
            NormalInfo normalInfo = new NormalInfo();
            //这里直接到解锁关卡
            normalInfo.index = chosenId;
            EventMgr.Ins.DispachEvent(EventConfig.STORY_DELETE_GAME_INFO);
            EventMgr.Ins.DispachEvent(EventConfig.STORY_TRIGGER_NEXT_NODE, normalInfo);
        };

        System.Action questCallback = () =>
        {
            WWWForm wWForm = new WWWForm();
            wWForm.AddField("actorId", storyInfo.actor_id);
            wWForm.AddField("chapterId", storyInfo.chapterId);
            wWForm.AddField("chosenId", chosenId);
            wWForm.AddField("consumeId", gameConsumeConfig.id);
            GameMonoBehaviour.Ins.RequestInfoPost<PlayerStoryInfo>(NetHeaderConfig.STROY_SKIP_NODE, wWForm, callback);
        };

        Debug.Log(gameConsumeConfig.tinyItem.name + "  " + gameConsumeConfig.tinyItem.num);

        Extrand extrand = new Extrand();
        //extrand.key = "提示";
        //extrand.msg = "需要消耗" + gameConsumeConfig.tinyItem.num + gameConsumeConfig.tinyItem.name + "跳过当前关卡吗?";
        //extrand.callBack = questCallback;

        extrand = new Extrand();
        extrand.type = 1;
        extrand.key = "提示";
        extrand.item = gameConsumeConfig.tinyItem;
        extrand.msg = "跳过当前关卡";
        extrand.callBack = questCallback;
        UIMgr.Ins.showNextPopupView<PopupDialogView, Extrand>(extrand);

    }






    /**************剧情核心部分  结束*********************/
    #endregion



    #region 音乐部分


    void PlayBgMusic(GamePointConfig gamePointConfig)
    {
        // Debug.LogError(gamePointConfig.id + " PlayBgMusic: " + gamePointConfig.music_id);
        //backGroundMusicId 
        if (gamePointConfig.music_id != backGroundMusicId)
        {
            backGroundMusicId = gamePointConfig.music_id;
            AudioClip audioClip = ResourceUtil.LoadStoryBgMusic(backGroundMusicId);
            GRoot.inst.PlayBgSound(audioClip);
        }
        PlayEffectMusic(gamePointConfig);
    }

    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="gamePointConfig">Game point config.</param>
    void PlayEffectMusic(GamePointConfig gamePointConfig)
    {
        //Debug.LogError(gamePointConfig.id + " PlayEffectMusic: " + gamePointConfig.voice_id);
        if (gamePointConfig.voice_id != 0)
        {
            backGroundMusicId = gamePointConfig.voice_id;
            AudioClip audioClip = ResourceUtil.LoadStoryEffectSound(gamePointConfig.voice_id);
            GRoot.inst.PlayEffectSound(audioClip);
        }
    }


    #endregion





    public override void GoToMoudle<T, D>(int index, D data)
    {
        MoudleType moudleType = (MoudleType)index;
        BaseMoudle baseMoudle = FindMoudle<T>();
        // Debug.Log("Acount: " + baseMoudles.Count);
        if (baseMoudle == null)
        {
            baseMoudle = (BaseMoudle)Activator.CreateInstance(typeof(T));
            baseMoudles.Add(baseMoudle);
            GComponent gComponent = null;

            string componmentName;
            if (moudleNamePairs.TryGetValue(moudleType, out componmentName))
            {
                GObject gObject = ui.GetChild(componmentName);
                if (gObject == null)
                {
                    Debug.Log(componmentName + " not find, please check it");
                    return;
                }
                gComponent = gObject.asCom;
                //gComponent.opaque = true;
            }


            baseMoudle.baseView = this;
            baseMoudle.InitMoudle(gComponent, index);
        }

        baseMoudle.InitData<D>(data);
        SwitchController(index);
    }

    public override void GoToMoudle<T>(int index)
    {
        MoudleType moudleType = (MoudleType)index;
        BaseMoudle baseMoudle = FindMoudle<T>();
        Debug.Log("Bcount: " + baseMoudles.Count);
        if (baseMoudle == null)
        {
            baseMoudle = (BaseMoudle)Activator.CreateInstance(typeof(T));
            baseMoudles.Add(baseMoudle);
            GComponent gComponent = null;

            string componmentName;
            if (moudleNamePairs.TryGetValue(moudleType, out componmentName))
            {
                GObject gObject = ui.GetChild(componmentName);
                if (gObject == null)
                {
                    Debug.Log(componmentName + " not find, please check it");
                    return;
                }
                gComponent = gObject.asCom;
                gComponent.opaque = true;
            }


            baseMoudle.baseView = this;
            baseMoudle.InitMoudle(gComponent, index);
        }
        baseMoudle.InitData();
        SwitchController(index);
    }



    private void LateUpdate()
    {
        //监听对白说话是否结束
        if (controller.selectedIndex == (int)MoudleType.Story_Dialog)
        {
            EventMgr.Ins.DispachEvent(EventConfig.MUSIC_DIALOG_PLAY_OVER);
        }
    }


    void ShowGetEffect(PlayerStoryInfo item)
    {
        UpgradeInfo upgradeInfo = new UpgradeInfo();
        upgradeInfo.gGraph = backgroundGraph;
        upgradeInfo.extra = item.extra;
        upgradeInfo.actorId = item.actor_id;
        StartCoroutine(GameTool.ShowEffect(upgradeInfo));
    }
    public Vector2 GetButtonPos(int index)
    {
        return SearchChild("n" + index).asCom.position;
    }

}

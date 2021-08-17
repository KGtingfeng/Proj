using FairyGUI;
using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ViewAttr("Game/UI/Z_Main", "Z_Main", "Main")]
public class MainView : BaseView
{
    public static MainView ins;
    GComponent headUi;
    GGraph spineGraph;
    GoWrapper goWrapper;
    GLoader gLoader;

    GLoader frameLoader;
    GGraph frameGraph;

    GGroup leftGroup;
    GGroup rightGroup;
    GGroup bottomGroup;


    GComponent iconCom;
    GButton roleGroupBtn;
    GButton achievementBtn;
    GButton mailBtn;
    GButton taskBtn;
    GButton storyBtn;
    GButton interactiveBtn;
    public GButton phoneBtn;
    GButton welfareBtn;
    GButton friendBtn;


    GComponent highfiveCom;

    bool canShowTalk;
    public enum MoudleType
    {
        All = 0,
        Top,
        ShowBubble,
        PlayerInfor,//玩家信息
        ChangeHead, //更换头像
        ChangeName,//修改名称
        Highfive = 7,
    }

    Dictionary<MoudleType, string> moudleNamePairs = new Dictionary<MoudleType, string>()
    {
        {MoudleType.All,"n0"},
        {MoudleType.Top,"n1"},
        {MoudleType.PlayerInfor,"n36"},
        {MoudleType.ChangeHead,"n38"},
        { MoudleType.ChangeName,"n47"}
    };

    Player PlayerInfo
    {
        get { return GameData.Player; }
    }

    HomeActor HomeActorInfo
    {
        get { return GameData.Player.homeActor; }
    }

    SkeletonAnimation skeletonAnimation
    {
        get { return RoleMgr.roleMgr.GetSkeletonAnimation; }
    }
    GameRoleMainTipsConfig gameRoleMainTips;

    List<GComponent> btnList;

    GComponent talkSmall1;
    GComponent talkSmall2;


    public bool hasHightfive;
    public override void InitUI()
    {
        ins = this;
        controller = ui.GetController("c1");
        headUi = SearchChild("n40").asCom;
        iconCom = headUi.GetChild("n16").asCom;
        gLoader = SearchChild("n41").asLoader;
        roleGroupBtn = SearchChild("n29").asButton;
        achievementBtn = SearchChild("n22").asButton;
        mailBtn = SearchChild("n23").asButton;
        taskBtn = SearchChild("n54").asButton;
        storyBtn = SearchChild("n31").asButton;
        interactiveBtn = SearchChild("n28").asButton;
        phoneBtn = SearchChild("n27").asButton;
        welfareBtn = SearchChild("n19").asButton;
        friendBtn = SearchChild("n24").asButton;

        leftGroup = SearchChild("n49").asGroup;
        rightGroup = SearchChild("n50").asGroup;
        bottomGroup = SearchChild("n51").asGroup;
        highfiveCom = SearchChild("n52").asCom;

        frameLoader = headUi.GetChild("n16").asCom.GetChild("n18").asLoader;


        frameGraph = headUi.GetChild("n16").asCom.GetChild("n19").asGraph;
        talkSmall1 = SearchChild("n57").asCom;
        talkSmall2 = SearchChild("n58").asCom;
        talkSmall1.visible = false;
        talkSmall2.visible = false;
        GetBtn();

        InitEvent();
        currentSkin = -1;
        currentBg = -1;
        GetChannelSwitch();
        RefreshOpen();
        if (!GameData.isGuider)
            RefreshRedPoint();

    }

    bool isCheckNewbie;
    public override void InitData()
    {
        base.InitData();
        if (!isCheckNewbie)
        {
            isCheckNewbie = true;
            NewbieGuide();
        }
        SetActorSkin();
        InitTopInfo();
        RequestAlarm();

        RequestRedPoint();
        RequestMessage();
        LoadServerObject();
        RefreshAvatarFrame();
        RefreshOpen();
        ShowOpenEffect();
        canShowTalk = true;
    }


    public override void InitEvent()
    {
        InitBtnClick();

        //regist
        EventMgr.Ins.RegisterEvent(EventConfig.REFRESH_USER_TOP_INFO, InitTopInfo);
        EventMgr.Ins.RegisterEvent<TinyItem>(EventConfig.GET_DOLL, ShowGetDoll);
        //close tips window
        EventMgr.Ins.RegisterEvent(EventConfig.MUSIC_MAIN_TIPS, CloseTalkWindow);

        EventMgr.Ins.RegisterEvent<Vector2>(EventConfig.STAR_FLY_EFFECT, ShowStarEffect);
        EventMgr.Ins.RegisterEvent<Vector2>(EventConfig.DIAMOND_FLY_EFFECT, ShowDiamondEffect);

        EventMgr.Ins.RegisterEvent(EventConfig.MAIN_OPEN_EFFECT, ShowOpenEffect);
        EventMgr.Ins.RegisterEvent(EventConfig.GOTO_VIEW_STORY, RequestStoryBaseInfo);
        //获取奖励
        EventMgr.Ins.RegisterEvent<TinyItem>(EventConfig.AWARD_GET_SINGLE, ShowGetItemView);

        EventMgr.Ins.RegisterEvent(EventConfig.REFRESH_AVATAR_FRAME, RefreshAvatarFrame);
        EventMgr.Ins.RegisterEvent(EventConfig.GET_RED_POINT, RequestRedPoint);
        EventMgr.Ins.RegisterEvent(EventConfig.REFRESH_MAIN_RED_POINT, RefreshRedPoint);

        //过场
        EventMgr.Ins.RegisterEvent<CallBackList>(EventConfig.STORY_ANIMATION_VIEW, ShowStoryAnimationView);
         
        InitSpineEvent();
    }

    private void InitSpineEvent()
    {
        //head
        ui.GetChild("n44").displayObject.onClick.Set(() =>
        {
            if (GameData.isGuider)
                return;
            PlayRoleSounds(RoleBodyType.Head);
        });

        //body
        ui.GetChild("n45").displayObject.onClick.Set(() =>
        {
            if (GameData.isGuider)
                return;
            PlayRoleSounds(RoleBodyType.Body);
        });
    }

    private void NewbieGuide()
    {
        if (!GameData.isOpenGuider)
            return;
        
        StoryCacheMgr.storyCacheMgr.GetStoryGameSave("Newbie", 0, (storyGameSave) =>
          {
              if (!storyGameSave.IsDefault)
              {
                  string[] save = storyGameSave.value.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                  GameData.guiderCurrent = GuiderInfoLinked.GetCurrGuiderInfo(int.Parse(save[0]), int.Parse(save[1]));
                  if (GameData.guiderCurrent != null)
                  {
                      string[] roll_to = GameData.guiderCurrent.guiderInfo.roll_to.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                      if (roll_to.Length < 2 || !GameData.isOpenGuider)
                      {
                          GameData.isGuider = false;
                          return;
                      }
                      GameData.guiderCurrent = GuiderInfoLinked.GetCurrGuiderInfo(int.Parse(roll_to[0]), int.Parse(roll_to[1]));
                      if (GameData.guiderCurrent != null)
                      {
                          CloseTalkWindow();
                          switch (GameData.guiderCurrent.guiderInfo.flow)
                          {
                              case 1:
                                  JumpMgr.Ins.JumpView("23");
                                  GameData.isGuider = true;
                                  break;
                              case 2:
                                  
                                  UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);
                                  GameData.isGuider = true;
                                  break;
                              case 3:
                                  UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);
                                  GameData.isGuider = true;
                                  break;
                          }
                      }
                  }
              }

          });  

    }

    private void GetChannelSwitch()
    {
        ChannelSwitchConfig seven = GameData.Configs.Find(a => a.key == ChannelSwitchConfig.KEY_SEVEN);
        ChannelSwitchConfig ad = GameData.Configs.Find(a => a.key == ChannelSwitchConfig.KEY_AD);
        ChannelSwitchConfig daily = GameData.Configs.Find(a => a.key == ChannelSwitchConfig.KEY_DAILY);

        if (seven.value == 0 && ad.value == 0 && daily.value == 0)
        {
            SearchChild("n19").visible = false;
        }
        else
        {
            SearchChild("n19").visible = true;
        }
    }

    /// <summary>
    /// 动画播放结束 重置为idle
    /// </summary>
    /// <param name="trackEntry">Track entry.</param>
    void HandleComplete(TrackEntry trackEntry)
    {
        trackEntry.Complete -= HandleComplete;
        SpineCtr.RemoveAnimationTrack(skeletonAnimation, new int[] { SpineConfig.AC_TYPE_POSE });
        SpineCtr.SwitchAnimationState(skeletonAnimation, new int[] { SpineConfig.AC_TYPE_IDLE });
    }

    void ShowSpineTalk(string msg)
    {
        ErrorMsg errorMsg = new ErrorMsg(msg);
        UIMgr.Ins.showWindow<ShowRoleTalkWindow>(errorMsg);
    }


    void ShowGetDoll(TinyItem tinyItem)
    {
        UIMgr.Ins.showNextPopupView<GetRoleView, TinyItem>(tinyItem);
    }

    public void GotoRolegroup()
    {
        CloseTalkWindow();

        NormalInfo normalInfo = new NormalInfo();
        normalInfo.index = 0;
        //EventMgr.Ins.DispachEvent(EventConfig.GOTO_VIEW_ROLE_GROUP, normalInfo);

        Action action = () => { 
            UIMgr.Ins.showNextPopupView<RoleGropView, NormalInfo>(normalInfo); };
        StartCoroutine(GotoTmpEffectView(action));
    }

    public void GotoStory()
    {
        //ShowCloseEffect();
        CloseTalkWindow();
        RequestStoryBaseInfo();

    }

    public void GotoSms()
    {
        //ShowCloseEffect();
        CloseTalkWindow();
        WWWForm wWWForm = new WWWForm();
        GameMonoBehaviour.Ins.RequestInfoPost<List<SmsListIndex>>(NetHeaderConfig.CELL_LIST_INDEX, wWWForm, RequestPhoneInfo);

    }

    public void GotoTask()
    {
        CloseTalkWindow();
        //ShowCloseEffect();
        WWWForm wWWForm = new WWWForm();
        GameMonoBehaviour.Ins.RequestInfoPost<DailyTask>(NetHeaderConfig.MISSION_LIST, wWWForm, (DailyTask dailyTask) =>
        {
            Action action = () => { UIMgr.Ins.showNextPopupView<TaskView, DailyTask>(dailyTask); };
            StartCoroutine(GotoTmpEffectView(action));

        });
    }

    public void RequestRoleListInfo()
    {
        WWWForm wWWForm = new WWWForm();
        GameMonoBehaviour.Ins.RequestInfoPostList<Role>(NetHeaderConfig.ACTOR_LIST, wWWForm, () =>
        {
            Action action = () => { UIMgr.Ins.showNextView<InteractiveView>(); };
            StartCoroutine(GotoTmpEffectView(action));
        });
    }

    IEnumerator GotoTmpEffectView(Action action)
    {
        TouchScreenView.Ins.PlayTmpEffect();
        yield return new WaitForSeconds(0.4f);
        ShowCloseEffect();
        yield return new WaitForSeconds(0.4f);
        action();
    }

    void InitTopInfo()
    {
        //love
        headUi.GetChild("n12").text = PlayerInfo.love + "";
        //money
        headUi.GetChild("n13").text = PlayerInfo.diamond + "";
        //name
        headUi.GetChild("n14").text = PlayerInfo.name;
        //vip
        headUi.GetChild("n15").text = PlayerInfo.level.ToString();

        GComponent gCom = headUi.GetChild("n16").asCom;
        GLoader gLoader = gCom.GetChild("n16").asLoader;
        gLoader.url = UrlUtil.GetRoleHeadIconUrl(PlayerInfo.avatar);
    }
    /// <summary>
    /// 刷新头像框
    /// </summary>
    void RefreshAvatarFrame()
    {
        if (GameData.Player.avatar_frame.current != 0)
        {
            GameAvatarFrameConfig frameConfig = JsonConfig.GameAvatarFrameConfigs.Find(a => a.id == GameData.Player.avatar_frame.current);
            FXMgr.SetFrame(frameGraph, frameLoader, frameConfig.source_id, 100, new Vector3(135, 135, 1000));
        }
    }

    /// <summary>
    /// 刷新红点
    /// </summary>
    void RefreshRedPoint()
    {

        if (RedpointMgr.Ins.CanRefreshRedPoint)
        {
            iconCom.GetController("c1").selectedIndex = RedpointMgr.Ins.PlayerHeadHaveRedpoint() ? 1 : 0;
            roleGroupBtn.GetController("statue").selectedIndex = RedpointMgr.Ins.RoleGroupHaveRedpoint() ? 1 : 0;
            achievementBtn.GetController("statue").selectedIndex = RedpointMgr.Ins.AchievementHaveRedpoint();
            mailBtn.GetController("statue").selectedIndex = RedpointMgr.Ins.MailHaveRedpoint();
            taskBtn.GetController("statue").selectedIndex = RedpointMgr.Ins.TaskHaveRedpoint();
            storyBtn.GetController("statue").selectedIndex = RedpointMgr.Ins.StoryHaveRedpoint() ? 1 : 0;
            interactiveBtn.GetController("statue").selectedIndex = RedpointMgr.Ins.InteractiveHaveRedpoint() ? 1 : 0;
            phoneBtn.GetController("statue").selectedIndex = RedpointMgr.Ins.PhoneHaveRedpoint() ? 1 : 0;
            welfareBtn.GetController("statue").selectedIndex = RedpointMgr.Ins.WelfareHaveRedpoint();
            friendBtn.GetController("statue").selectedIndex = RedpointMgr.Ins.FriendHaveRedpoint();
        }

        if (!GameData.isGuider)
        {
            ShowDaily();
        }
    }

    const int dailyCountDown = 10;
    int countDown;
    int talkComIndex = 1;



    void CountDown(object param)
    {
        countDown--;
        if (countDown <= 0)
        {
            Timers.inst.Remove(CountDown);
            talkComIndex *= -1;
            ShowDaily();
        }
    }
    /// <summary>
    /// 日常引导
    /// </summary>
    private void ShowDaily()
    {
        if (RedpointMgr.Ins.XinlingHaveTip() && RedpointMgr.Ins.TaskHaveTip() && canUse.Contains(7) && canUse.Contains(10))
        {
            talkSmall2.visible = false;
            talkSmall1.visible = false;
            talkSmall1.GetTransition("t0").Stop();
            talkSmall2.GetTransition("t0").Stop();
            if (talkComIndex == 1)
            {
                talkSmall2.visible = true;
                talkSmall2.GetTransition("t0").Play(1, 0, 0, 0.5f, () =>
                {
                    talkSmall2.GetTransition("t0").Play(1, 1, 0.5f, 0.7f, () =>
                    {
                        talkSmall2.visible = false;
                    });
                });
            }
            else
            {
                talkSmall1.visible = true;
                talkSmall1.GetTransition("t0").Play(1, 0, 0, 0.5f, () =>
                {
                    talkSmall1.GetTransition("t0").Play(1, 1, 0.5f, 0.7f, () =>
                    {
                        talkSmall1.visible = false;
                    });
                });
            }

            countDown = dailyCountDown;
            Timers.inst.Add(1f, 0, CountDown);
        }
        else if (RedpointMgr.Ins.XinlingHaveTip() && (!RedpointMgr.Ins.TaskHaveTip() || canUse.Contains(7)) && canUse.Contains(10))
        {
            talkSmall2.visible = true;
            talkSmall1.visible = false;
            talkSmall2.GetTransition("t0").Stop();

            talkSmall2.GetTransition("t0").Play(1, 0, 0, 0.5f, () =>
             {
                 talkSmall2.GetTransition("t0").Play(1, 1, 0.5f, 0.7f, () =>
                 {
                     talkSmall2.visible = false;
                 });
             });
            countDown = dailyCountDown;
            Timers.inst.Add(1f, 0, CountDown);

        }
        else if ((!RedpointMgr.Ins.XinlingHaveTip() || !canUse.Contains(10)) && RedpointMgr.Ins.TaskHaveTip() && canUse.Contains(7))
        {
            talkSmall2.visible = false;
            talkSmall1.visible = true;
            talkSmall1.GetTransition("t0").Stop();
            talkSmall1.GetTransition("t0").Play(1, 0, 0, 0.5f, () =>
            {
                talkSmall1.GetTransition("t0").Play(1, 1, 0.5f, 0.7f, () =>
                {
                    talkSmall1.visible = false;
                });
            });
            countDown = dailyCountDown;
            Timers.inst.Add(1f, 0, CountDown);
        }

    }

    /// <summary>
    /// 获取主界面功能按钮
    /// </summary>
    private void GetBtn()
    {
        btnList = new List<GComponent>();
        //1玩家信息
        btnList.Add(headUi.GetChild("n16").asCom);
        //2商城
        btnList.Add(SearchChild("n17").asCom);
        //3成长
        btnList.Add(SearchChild("n29").asCom);
        //4收集
        btnList.Add(SearchChild("n26").asCom);
        //5成就
        btnList.Add(SearchChild("n22").asCom);
        //6剧情
        btnList.Add(SearchChild("n31").asCom);
        //7每日任务
        btnList.Add(SearchChild("n54").asCom);
        //8角色互动
        btnList.Add(SearchChild("n28").asCom);
        //9手机
        btnList.Add(SearchChild("n27").asCom);
        //10帮助辛灵
        btnList.Add(SearchChild("n55").asCom);
        //11福利
        btnList.Add(SearchChild("n19").asCom);
        //12活动
        btnList.Add(SearchChild("n20").asCom);
        //13邮件
        btnList.Add(SearchChild("n23").asCom);
        //14好友
        btnList.Add(SearchChild("n24").asCom);
        //15排行榜
        btnList.Add(SearchChild("n25").asCom);
        //16房间
        btnList.Add(SearchChild("n30").asCom);
    }

    Dictionary<int, List<int>> levelOpenDic;
    List<int> canUse = new List<int>();
    /// <summary>
    /// 刷新功能开启状态
    /// </summary>
    public void RefreshOpen()
    {
        if (levelOpenDic == null)
        {
            levelOpenDic = new Dictionary<int, List<int>>();
            List<GameFuncStepConfig> stepConfigs = JsonConfig.GameFuncStepConfigs;
            for (int i = 0; i < stepConfigs.Count; i++)
            {
                List<int> open = new List<int>();
                string[] step = stepConfigs[i].can_use.Split(new char[1] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < step.Length; j++)
                {
                    open.Add(int.Parse(step[j]));
                }
                levelOpenDic.Add(stepConfigs[i].level, open);
            }
        }
        canUse.Clear();
        foreach (var dic in levelOpenDic)
        {
            if (GameData.Player.level >= dic.Key)
            {
                canUse.AddRange(dic.Value);
            }

        }
        InitBtnClick();
        for (int i = 0; i < btnList.Count; i++)
        {
            if (!canUse.Contains(i + 1))
            {
                btnList[i].GetController("statue").selectedIndex = 0;
                btnList[i].GetController("open").selectedIndex = 1;
                int level = 0;
                foreach (var dic in levelOpenDic)
                {
                    if (dic.Value.Contains(i + 1))
                    {
                        level = dic.Key;
                    }

                }
                btnList[i].onClick.Set(() =>
                {
                    UIMgr.Ins.showErrorMsgWindow("该功能将在" + level + "等级开放!");
                });
            }
            else
            {
                Controller controller = btnList[i].GetController("open");
                if (controller != null)
                {
                    controller.selectedIndex = 0;
                }
            }

        }
    }

    private void InitBtnClick()
    {
        //剧情按钮
        storyBtn.onClick.Set(() =>
        {
            GotoStory();
        });
        //角色互动
        interactiveBtn.onClick.Set(() =>
        {
            //ShowCloseEffect();
            CloseTalkWindow();
            RequestRoleListInfo();
        });
        ////角色成长
        roleGroupBtn.onClick.Set(() =>
        {
            GotoRolegroup();
        });
        //商城
        SearchChild("n17").onClick.Set(() =>
        {
            //ShowCloseEffect();
            CloseTalkWindow();
            StartCoroutine(GotoTmpEffectView(() =>
            {
                UIMgr.Ins.showNextPopupView<ShopMallView>();
            }));
        });

        //首充
        SearchChild("n18").onClick.Set(() =>
        {
            CloseTalkWindow();
            //ShowCloseEffect();
            UIMgr.Ins.showNextPopupView<FirstBuyView>();
        });
        //福利按钮
        welfareBtn.onClick.Set(() =>
        {
            CloseTalkWindow();
            //ShowCloseEffect();
            Action action = () => { UIMgr.Ins.showNextView<WelfareView, List<ChannelSwitchConfig>>(GameData.Configs); };
            StartCoroutine(GotoTmpEffectView(action));

        });
        //好友系统
        friendBtn.onClick.Set(() =>
        {
            //ShowCloseEffect();
            CloseTalkWindow();
            FriendDataMgr.Ins.RequestFriendInfos(1, 12, info =>
            {
                Action action = () => { UIMgr.Ins.showNextView<FriendView, Friend>(info); };
                StartCoroutine(GotoTmpEffectView(action));

            });

        });

        ////头像按钮 
        iconCom.onClick.Set(() =>
        {
            //ShowCloseEffect();
            CloseTalkWindow();
            EventMgr.Ins.DispachEvent(EventConfig.SOUND_CLICK_BTN);
            //EventMgr.Ins.DispachEvent(EventConfig.GOTO_VIEW_PLAYER_HEAD);

            UIMgr.Ins.showNextPopupView<PlayerHeadView>();
        });

        //click loveBtn星星
        headUi.GetChild("n10").onClick.Set(() =>
        {
            CloseTalkWindow();
            UIMgr.Ins.showNextPopupView<BuyStarsView>();
        });
        //click diamondBtn 钻石
        headUi.GetChild("n11").onClick.Set(() =>
        {
            CloseTalkWindow();
            UIMgr.Ins.showNextPopupView<ShopMallView>();
        });
        //手机
        phoneBtn.onClick.Set(() =>
        {
            GotoSms();
        });

        //房间
        SearchChild("n30").onClick.Set(() =>
        {
            CloseTalkWindow();
            //ShowCloseEffect();
            StartCoroutine(GotoTmpEffectView(() =>
            {
                UIMgr.Ins.showNextPopupView<RoomView>();
            }));
        });

        //收集
        SearchChild("n26").onClick.Set(() =>
        {
            CloseTalkWindow();
            //ShowCloseEffect();
            StartCoroutine(GotoTmpEffectView(() =>
            {
                HolderData holder = new HolderData();
                UIMgr.Ins.showNextPopupView<RoomView, HolderData>(holder);
            }));
        });

        //邮件
        mailBtn.onClick.Set(() =>
        {
            CloseTalkWindow();
            //ShowCloseEffect();
            WWWForm wWWForm = new WWWForm();
            wWWForm.AddField("page", 1);
            GameMonoBehaviour.Ins.RequestInfoPost<Mail>(NetHeaderConfig.MAILS_LIST, wWWForm, (Mail mail) =>
            {
                UIMgr.Ins.showNextPopupView<MailView, Mail>(mail);
            });
        });

        //任务
        taskBtn.onClick.Set(() =>
        {
            GotoTask();
        });

        //排行
        SearchChild("n25").onClick.Set(() =>
        {
            CloseTalkWindow();
            //ShowCloseEffect();
            Action action = () => { UIMgr.Ins.showNextPopupView<RankView>(); };
            StartCoroutine(GotoTmpEffectView(action));

        });

        //成就
        achievementBtn.onClick.Set(() =>
        {
            CloseTalkWindow();
            //ShowCloseEffect();
            WWWForm wWWForm = new WWWForm();
            GameMonoBehaviour.Ins.RequestInfoPost<DailyTask>(NetHeaderConfig.MISSION_LIST, wWWForm, (DailyTask dailyTask) =>
            {
                Action action = () => { UIMgr.Ins.showNextPopupView<AchievementView, DailyTask>(dailyTask); };
                StartCoroutine(GotoTmpEffectView(action));
            });

        });

        SearchChild("n55").onClick.Set(() =>
        {
            CloseTalkWindow();
            //ShowCloseEffect();
            WWWForm wWWForm = new WWWForm();
            GameMonoBehaviour.Ins.RequestInfoPost<string>(NetHeaderConfig.XINLING_LIST, wWWForm, (string taskInfo) =>
            {
                Action action = () => { UIMgr.Ins.showNextView<HelpTaskView, string>(taskInfo); };
                StartCoroutine(GotoTmpEffectView(action));
            });
        });
    }

    public override void GoToMoudle<T>(int index)
    {
        MoudleType moudleType = (MoudleType)index;
        BaseMoudle baseMoudle = FindMoudle<T>();
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
        SwitchController(index);
    }

    public override void SwitchController(int index)
    {
        base.SwitchController(index);
        bool isVisible = index == (int)MoudleType.All || index == (int)MoudleType.Highfive;
        spineGraph.visible = isVisible;

    }

    public override void onShow()
    {
        StopAllCoroutines();
        SetActorSkin();
        SwitchController((int)MoudleType.All);
        ShowOpenEffect();
        RequestAlarm();
        RequestRedPoint();
        RequestMessage();
        InitTopInfo();
        canShowTalk = true;

    }

    /****************Net****************/
    void RequestMessage()
    {
        if (!GameData.isGuider)
        {
            WWWForm wWWForm = new WWWForm();
            GameMonoBehaviour.Ins.RequestInfoPost<PushInfo>(NetHeaderConfig.CELL_CHECK_MESSAGE, wWWForm, null, false);
        }
    }

    void RequestAlarm()
    {
        if (GameData.isGuider)
            return;
        WWWForm wWWForm = new WWWForm();
        GameMonoBehaviour.Ins.RequestInfoPostHaveData<HomeActor>(NetHeaderConfig.HIGHFIVE_STATUS, wWWForm, GetAlarmInfo, false);
    }

    public void RequestRedPoint()
    {
        if (!GameData.isGuider)
        {
            WWWForm wWWForm = new WWWForm();
            GameMonoBehaviour.Ins.RequestInfoPost<PlayerRedpoint>(NetHeaderConfig.RED_POINT, wWWForm, () =>
            {
                RefreshOpen();
                RefreshRedPoint();
            }, false);
        }
        else
        {
            RefreshOpen();
        }
    }

    /// <summary>
    /// 早安击掌
    /// </summary>
    void GetAlarmInfo(HomeActor homeActor)
    {

        if (!string.IsNullOrEmpty(homeActor.skin))
        {
            if (PlayerInfo.homeActor.actor_id != homeActor.actor_id)
            {
                string[] skins = homeActor.skin.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                string[] backgrounds = homeActor.background.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                PlayerInfo.homeActor.skin = skins[0];
                PlayerInfo.homeActor.background = backgrounds[0];
                PlayerInfo.homeActor.actor_id = homeActor.actor_id;
                WWWForm wWWForm = new WWWForm();
                wWWForm.AddField("actorId", homeActor.actor_id);
                GameMonoBehaviour.Ins.RequestInfoPost<HomeActor>(NetHeaderConfig.SET_HOMEACTOR, wWWForm, null);
                SetActorSkin();
            }
            hasHightfive = true;
            ShowHighfiveEffect();
            highfiveCom.onClick.Set(() =>
            {
                StartCoroutine(Highfive(homeActor.actor_id));
            });
            SwitchController((int)MoudleType.Highfive);
        }
    }

    void RequestStoryBaseInfo()
    {
        WWWForm wWWForm = new WWWForm();
        GameMonoBehaviour.Ins.RequestInfoPostList<PlayerStoryInfo>(NetHeaderConfig.STORY_GET_SOTRY_INFO, wWWForm, RequestStoryBaseInfoSuccess);
    }

    void RequestStoryBaseInfoSuccess()
    {
        Action action = () =>
        {
            UIMgr.Ins.showNextPopupView<StoryView>();
        };
        StartCoroutine(GotoTmpEffectView(action));

    }

    void RequestPhoneInfo(List<SmsListIndex> smsLists)
    {
        StartCoroutine(GotoTmpEffectView(() =>
        {
            UIMgr.Ins.showNextPopupView<SMSView, List<SmsListIndex>>(smsLists);
        }));
    }

    /// <summary>
    /// 显示过场动画
    /// </summary>
    /// <param name="callback">Callback.</param>
    void ShowStoryAnimationView(CallBackList callback)
    {
        UIMgr.Ins.ShowViewWithoutHideBefore<StoryAnimationView, CallBackList>(callback);
    } 


    /**********获取物品奖励弹窗*********/

    void ShowGetItemView(TinyItem tinyItem)
    {
        UIMgr.Ins.showNextPopupView<CommonGetPropsView, TinyItem>(tinyItem);
    }


    /**********Role Music*********/
    bool isPlayRoleDialog;

    void PlayRoleSounds(RoleBodyType roleBodyType)
    {
        if (!canShowTalk)
            return;
        if (!GRoot.inst.GetDialogIsPlaying())
        {

            AudioClip audioClip = null;
            switch (roleBodyType)
            {
                case RoleBodyType.Head:
                    {
                        BodyInfo bodyInfo = gameRoleMainTips.GetHeadInfo();
                        SpineCtr.SwitchAnimationState(skeletonAnimation, new int[] { bodyInfo.faceId });

                        ShowSpineTalk(bodyInfo.context);
                        audioClip = ResourceUtil.LoadMainRoleTipsMusic(skinConfig.actor_id + "/" + bodyInfo.voiceId);
                    }
                    break;
                case RoleBodyType.Body:
                    {
                        BodyInfo bodyInfo = gameRoleMainTips.GetBodyInfo();
                        SpineCtr.SwitchAnimationState(skeletonAnimation, new int[] { bodyInfo.faceId });

                        ShowSpineTalk(bodyInfo.context);
                        audioClip = ResourceUtil.LoadMainRoleTipsMusic(skinConfig.actor_id + "/" + bodyInfo.voiceId);
                    }
                    break;
            }
            GRoot.inst.PlayDialogSound(audioClip);
            isPlayRoleDialog = true;
        }
    }


    public void CloseTalkWindow(bool canShow)
    {
        canShowTalk = canShow;
        if (BaseWindow.window is ShowRoleTalkWindow && BaseWindow.window.contentPane.visible)
        {
            GRoot.inst.HideWindowImmediately(BaseWindow.window);
            if (GRoot.inst.GetDialogIsPlaying())
            {
                GRoot.inst.StopDialogSound();
            }
            SpineCtr.ResetAniamtionToIdle(skeletonAnimation);
        }
        talkSmall1.visible = false;
        talkSmall2.visible = false;
        Timers.inst.Remove(CountDown);

    }

    public void CloseTalkWindow()
    {
        canShowTalk = false;
        if (BaseWindow.window is ShowRoleTalkWindow && BaseWindow.window.contentPane.visible)
        {
            GRoot.inst.HideWindowImmediately(BaseWindow.window);
            if (GRoot.inst.GetDialogIsPlaying())
            {
                GRoot.inst.StopDialogSound();
            }
            SpineCtr.ResetAniamtionToIdle(skeletonAnimation);
        }
        talkSmall1.visible = false;
        talkSmall2.visible = false;
        Timers.inst.Remove(CountDown);

    }


    private void Update()
    {
        if (isPlayRoleDialog)
        {
            if (!GRoot.inst.GetDialogIsPlaying())
            {
                isPlayRoleDialog = false;
                //播放结束
                CloseTalkWindow(true);
            }
        }
    }

    //请求服务器时间
    void LoadServerObject()
    {
        ServiceObject service = GameObject.FindObjectOfType<ServiceObject>();
        if (service != null)
            return;
        GameObject go = new GameObject();
        ServiceObject serviceObject = go.AddComponent<ServiceObject>();
        serviceObject.Init();
        go.name = "ServiceObject";
        DontDestroyOnLoad(go);
    }

    int currentSkin;
    GameActorSkinConfig skinConfig;
    string showRoleKey;
    RoleMappingCom roleMappingCom;
    IEnumerator CreateSpine()
    {
        yield return 0;
        currentSkin = int.Parse(HomeActorInfo.skin);
        skinConfig = DataUtil.GetGameActorSkinConfig(currentSkin);
        if (skinConfig != null)
        {
            bool isSkin = skinConfig.type != 1;
            string path = isSkin ? (skinConfig.actor_id + "_" + currentSkin) : skinConfig.actor_id.ToString();

            UnityEngine.Object prefab = ResourceUtil.LoadSpineByName(path);
            if (prefab == null)
                prefab = ResourceUtil.LoadSpineByName(skinConfig.actor_id.ToString());

            //GameObject go = (GameObject)Instantiate(prefab);
            showRoleKey = prefab.name;
            RoleMgr.roleMgr.CreateRole(prefab);
            //GameObject go = (GameObject)Instantiate(RoleMappingCom.roleMappingCom.gameObject);
            //GameObject go = (GameObject)Instantiate(RoleMgr.roleMgr.GetMapingCom());
            GameObject go = RoleMgr.roleMgr.GetMapingCom();
            //go.transform.localPosition = new Vector3(30, -980, 1000);
            go.transform.localPosition = new Vector3(17, -475, 1000);
            //go.transform.localScale = Vector3.one * 108f;//新模型较大
            go.transform.localScale = new Vector3(857.0313f, 1855.758f, 114.2708f);////新模型较大
            //Component[] components = go.GetComponentsInChildren(typeof(SkeletonAnimation));
            //if (components != null && components.Length > 0)
            //{
            //    skeletonAnimation = components[0] as SkeletonAnimation;
            //}    
            SpineCtr.SetIdleAnimation(skeletonAnimation);
            if (goWrapper == null)
            {
                goWrapper = new GoWrapper(go);
            }
            else
            {
                GameObject gobj = goWrapper.wrapTarget;
                goWrapper.wrapTarget = go;
                Destroy(gobj);
            }
            spineGraph = ui.GetChild("n43").asGraph;
            spineGraph.SetNativeObject(goWrapper);
            roleMappingCom = go.GetComponent<RoleMappingCom>();
            roleMappingCom.FadeIn();
            //goWrapper.gameObject.transform.GetComponentInChildren<RoleAlphaCom>(true).TriggerAlpha();  
            gameRoleMainTips = DataUtil.GetRoleMainTipsConfig(skinConfig.actor_id);
        }
    }

    int currentBg;
    void InitBg()
    {
        if (currentBg != int.Parse(PlayerInfo.homeActor.background))
        {
            gLoader.url = UrlUtil.GetSkinBgUrl(int.Parse(PlayerInfo.homeActor.background));
            currentBg = int.Parse(PlayerInfo.homeActor.background);
        }
    }

    void SetActorSkin()
    {
        if (currentSkin != int.Parse(PlayerInfo.homeActor.skin))
        {
            StartCoroutine(CreateSpine());
        }
        else
        {
            RoleMgr.roleMgr.RefreshShow(showRoleKey);
            roleMappingCom.FadeIn();
        }
        InitBg();
    }


    GGraph highfiveXin;
    void ShowHighfiveEffect()
    {
        if (highfiveXin == null)
        {
            highfiveXin = FXMgr.CreateEffectWithScale(highfiveCom, new Vector2(263, -11), "UI_jizhang0", 162, -1);
        }
        highfiveXin.displayObject.gameObject.SetActive(false);
        highfiveXin.displayObject.gameObject.SetActive(true);
    }

    IEnumerator Highfive(int actorId)
    {
        highfiveCom.onClick.Clear();
        AudioClip audioClip = ResourceUtil.LoadEffect(SoundConfig.CommonEffectId.Highfive);
        GRoot.inst.PlayEffectSound(audioClip);
        FXMgr.CreateEffectWithScale(highfiveCom, new Vector2(234, -21), "zhuanquan", 162, -1);
        FXMgr.CreateEffectWithScale(highfiveCom, new Vector2(617, 768), "UI_jizhang1", 3.5f, -1);
        yield return new WaitForSeconds(1.6f);
        FXMgr.CreateEffectWithScale(highfiveCom, new Vector2(272, -51), "UI_jizhang2", 162, -1);

        yield return new WaitForSeconds(1f);
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("actorId", actorId);
        GameMonoBehaviour.Ins.RequestInfoPost<PlayerStoryInfoExtra>(NetHeaderConfig.ACTOR_HIGHFIVE, wWWForm, (PlayerStoryInfoExtra extra) =>
        {
            UpgradeInfo upgradeInfo = new UpgradeInfo();
            upgradeInfo.actorId = actorId;
            upgradeInfo.extra = extra;
            TouchScreenView.Ins.StartCoroutine(GameTool.ShowEffect(upgradeInfo));
            SwitchController((int)MoudleType.All);
        });
        hasHightfive = false;

    }

    void ShowOpenEffect()
    {
        leftGroup.x = -820;
        rightGroup.x = 1000;
        bottomGroup.y = 1637;
        leftGroup.alpha = 0;
        rightGroup.alpha = 0;
        bottomGroup.alpha = 0;

        //  EaseType.    缓动函数   不是速度是距离
        leftGroup.TweenMoveX(-440, 1.2f).SetEase(EaseType.QuartOut);
        rightGroup.TweenMoveX(619, 1.2f).SetEase(EaseType.QuartOut);
        bottomGroup.TweenMoveY(1111, 1.2f).SetEase(EaseType.QuartOut);

        leftGroup.TweenFade(1, 1.2f).SetEase(EaseType.QuartOut);
        rightGroup.TweenFade(1, 1.2f).SetEase(EaseType.QuartOut);
        bottomGroup.TweenFade(1, 1.2f).SetEase(EaseType.QuartOut);
    }

    void ShowCloseEffect()
    {
        leftGroup.TweenMoveX(-820, 1.2f).SetEase(EaseType.QuartOut);
        rightGroup.TweenMoveX(1000, 1.2f).SetEase(EaseType.QuartOut);
        bottomGroup.TweenMoveY(1637, 1.2f).SetEase(EaseType.QuartOut);

        leftGroup.TweenFade(0, 1.2f).SetEase(EaseType.QuartOut);
        rightGroup.TweenFade(0, 1.2f).SetEase(EaseType.QuartOut);
        bottomGroup.TweenFade(0, 1.2f).SetEase(EaseType.QuartOut);
    }

    Vector2 startFinish;
    void ShowStarEffect(Vector2 pos)
    {
        if (startFinish == Vector2.zero)
        {
            GImage gImage = headUi.GetChild("n8").asImage;
            startFinish = new Vector2(gImage.x + gImage.width / 2, gImage.y + gImage.height / 2);
        }
        TouchScreenView.Ins.CreateEffect(pos, startFinish, true);
    }

    Vector2 diamondFinish;
    void ShowDiamondEffect(Vector2 pos)
    {
        if (diamondFinish == Vector2.zero)
        {
            GImage gImage = headUi.GetChild("n9").asImage;
            diamondFinish = new Vector2(gImage.x + gImage.width / 2, gImage.y + gImage.height / 2);
        }
        TouchScreenView.Ins.CreateEffect(pos, diamondFinish, false);
    }
    public Vector2 GetButtonPos(int index)
    {
        return SearchChild("n" + index).asCom.position;
    }



}


public enum RoleBodyType
{
    Head,
    Body,
}
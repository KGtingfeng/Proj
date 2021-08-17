using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 跳转
/// </summary>
public class JumpMgr
{

    private static JumpMgr instance;
    public static JumpMgr Ins
    {
        get
        {
            if (instance == null)
            {
                instance = new JumpMgr();
            }
            return instance;
        }
    }
    Dictionary<int, Action<string>> jumpDic = new Dictionary<int, Action<string>>()
    {
        {(int)JumpType.BUY_STAR_VIEW, ShowBuyStarsView},
        {(int)JumpType.SHOP_MALL_VIEW, JumpShopMallView},
        {(int)JumpType.MAKE_GIFTS_VIEW, JumpMakeGiftsView},
        {(int)JumpType.PLAYER_HEAD_VIEW, JumpPlayerHeadView},
        {(int)JumpType.ROLE_GROUP_VIEW, JumpRoleGroupView},
        {(int)JumpType.INTERACTIVE_ROLE_LIST, JumpInteractiveView},
        {(int)JumpType.INTERACTIVE_MAIN_MOUDLE, JumpRoleInteractive},
        {(int)JumpType.STORY_VIEW, JumpStoryView},
        {(int)JumpType.SHOP_MALL_MOUDLE, JumpShopMallMoudle},
        {(int)JumpType.PHONE_VIEW, JumpPhoneView},
        {(int)JumpType.SMS_ACTOR_MOUDLE, JumpSmsMoudle},
        {(int)JumpType.ROOM_VIEW, JumpRoomView},
        {(int)JumpType.ALARM_VIEW, JumpAlarmMoudle},
        {(int)JumpType.RANK_VIEW, JumpRankView},
        {(int)JumpType.FAVOR_RANK_VIEW, JumpRankFavorView},
        {(int)JumpType.ATTRIBUTE_RANK_VIEW, JumpRankAttrView},
        {(int)JumpType.TIME_RANK_VIEW, JumpRankTimeView},
        {(int)JumpType.RANK_MOUDLE, JumpRankMoudle},
        { (int)JumpType.ACHIEVEMENT_VIEW,JumpAchievementMoudle},
        { (int)JumpType.ACHIEVEMENT_ROLE_TASK_VIEW,JumpAchievementRoleTaskMoudle},
        { (int)JumpType.STORY_ROLE,JumpStory},
    };

    public void JumpView(string type)
    {
        string[] str = type.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        string from = "";

        if (str.Length > 1)
        {
            from = str[1];
        }
        if (jumpDic.ContainsKey(int.Parse(str[0])))
        {
            jumpDic[int.Parse(str[0])].Invoke(from);
        }
        else
        {
            Debug.LogError("SPS----------------- jump type error  " + type);
        }

    }

    static void ShowBuyStarsView(string from)
    {
        UIMgr.Ins.showNextPopupView<BuyStarsView>();
    }

    static void JumpShopMallView(string from)
    {
        UIMgr.Ins.showNextView<ShopMallView>();
    }

    static void JumpMakeGiftsView(string from)
    {
        if (InteractiveDataMgr.ins.CurrentPlayerActor == null)
        {
            WWWForm wWWForm = new WWWForm();
            wWWForm.AddField("actorId", 11);
            GameMonoBehaviour.Ins.RequestInfoPost<PlayerActor>(NetHeaderConfig.ACTOR_SKIN_LIST, wWWForm, (PlayerActor playerActor) =>
            {
                RequestMakeGifts(from);
            });

        }
        else
        {
            RequestMakeGifts(from);
        }

    }

    static void RequestMakeGifts(string from)
    {
        WWWForm wWWForm = new WWWForm();
        GameMonoBehaviour.Ins.RequestInfoPostList<PlayerProp>(NetHeaderConfig.ACTOR_PROP_LIST, wWWForm, () =>
        {
            Action action = () =>
            {
                NormalInfo normalInfo = new NormalInfo();
                normalInfo.index = int.Parse(from);
                UIMgr.Ins.showNextView<MakeGiftsView, NormalInfo>(normalInfo);
            };
            TouchScreenView.Ins.StartCoroutine(GotoTmpEffectView(action));

        });
    }

    static void JumpPlayerHeadView(string from)
    {
        UIMgr.Ins.showNextPopupView<PlayerHeadView>();
    }

    static void JumpRoleGroupView(string from)
    {

        Action action = () =>
        {
            NormalInfo normalInfo = new NormalInfo();
            normalInfo.index = int.Parse(from);
            UIMgr.Ins.showNextView<RoleGropView, NormalInfo>(normalInfo);
        };
        TouchScreenView.Ins.StartCoroutine(GotoTmpEffectView(action));

    }

    static void JumpInteractiveView(string from)
    {
        WWWForm wWWForm = new WWWForm();
        GameMonoBehaviour.Ins.RequestInfoPostList<Role>(NetHeaderConfig.ACTOR_LIST, wWWForm, () =>
        {
            Action action = () =>
            {
                UIMgr.Ins.showNextView<InteractiveView>();
            };
            TouchScreenView.Ins.StartCoroutine(GotoTmpEffectView(action));
        });
    }

    static void JumpRoleInteractive(string from)
    {
        WWWForm wWWForm = new WWWForm();
        GameMonoBehaviour.Ins.RequestInfoPostList<Role>(NetHeaderConfig.ACTOR_LIST, wWWForm, () =>
        {
            RequestRoleList(int.Parse(from));
        });
    }

    static void RequestRoleList(int roleId)
    {
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("actorId", roleId);
        GameMonoBehaviour.Ins.RequestInfoPost<PlayerActor>(NetHeaderConfig.ACTOR_SKIN_LIST, wWWForm, () =>
        {
            Action action = () =>
            {
                NormalInfo normalInfo = new NormalInfo();
                normalInfo.type = (int)InteractiveView.MoudleType.INTERACTIVE;
                UIMgr.Ins.showNextView<InteractiveView, NormalInfo>(normalInfo);
            };
            TouchScreenView.Ins.StartCoroutine(GotoTmpEffectView(action));
        });
    }

    static void JumpStoryView(string from)
    {
        WWWForm wWWForm = new WWWForm();
        GameMonoBehaviour.Ins.RequestInfoPostList<PlayerStoryInfo>(NetHeaderConfig.STORY_GET_SOTRY_INFO, wWWForm, () =>
        {
            Action action = () =>
            {
                UIMgr.Ins.showNextView<StoryView>();
            };
            TouchScreenView.Ins.StartCoroutine(GotoTmpEffectView(action));
        });
    }

    static void JumpStory(string from)
    {
        WWWForm wWWForm = new WWWForm();
        GameMonoBehaviour.Ins.RequestInfoPostList<PlayerStoryInfo>(NetHeaderConfig.STORY_GET_SOTRY_INFO, wWWForm, () =>
        { 
                UIMgr.Ins.showNextView<StoryView, NormalInfo>(null);
        });
    }

    static void JumpShopMallMoudle(string from)
    {
        Action action = () =>
        {
            NormalInfo normalInfo = new NormalInfo();
            normalInfo.type = int.Parse(from);
            UIMgr.Ins.showNextView<ShopMallView, NormalInfo>(normalInfo);
        };
        TouchScreenView.Ins.StartCoroutine(GotoTmpEffectView(action));

    }

    static void JumpPhoneView(string from)
    {
        WWWForm wWWForm = new WWWForm();
        GameMonoBehaviour.Ins.RequestInfoPostHaveData<List<SmsListIndex>>(NetHeaderConfig.CELL_LIST_INDEX, wWWForm, (List<SmsListIndex> smsLists) =>
        {
            Action action = () =>
            {
                UIMgr.Ins.showNextView<SMSView, List<SmsListIndex>>(smsLists);
            };
            TouchScreenView.Ins.StartCoroutine(GotoTmpEffectView(action));
        });
    }

    static void JumpSmsMoudle(string from)
    {
        WWWForm wWWForm = new WWWForm();
        GameMonoBehaviour.Ins.RequestInfoPost<List<SmsListIndex>>(NetHeaderConfig.CELL_LIST_INDEX, wWWForm, (List<SmsListIndex> smsLists) =>
        {
            RequestSms(smsLists, int.Parse(from));
        });
    }

    static void RequestSms(List<SmsListIndex> smsLists, int roleId)
    {
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("actorId", roleId);
        GameMonoBehaviour.Ins.RequestInfoPostHaveData<SmsListIndex>(NetHeaderConfig.CELL_QUERY_MESSAGE, wWWForm, (SmsListIndex smsList) =>
        {
            GotoSms(smsList, smsLists);
        }, false);
    }

    static void GotoSms(SmsListIndex smsList, List<SmsListIndex> smsLists)
    {
        Action action = () =>
        {
            SMSView view = UIMgr.Ins.showNextView<SMSView, List<SmsListIndex>>(smsLists) as SMSView;
            view.GoToMoudle<SMSMoudle, SmsListIndex>((int)SMSView.MoudleType.TYPE_SMS, smsList);
        };
        TouchScreenView.Ins.StartCoroutine(GotoTmpEffectView(action));
    }


    static void JumpRoomView(string from)
    {
        Action action = () =>
        {
            UIMgr.Ins.showNextView<RoomView>();
        };
        TouchScreenView.Ins.StartCoroutine(GotoTmpEffectView(action));
    }

    static void JumpAlarmMoudle(string from)
    {
        WWWForm wWWForm = new WWWForm();
        GameMonoBehaviour.Ins.RequestInfoPostHaveData(NetHeaderConfig.QUERY_ALARM, wWWForm, (AlarmClockInfo info) =>
        {
            Action action = () =>
            {
                UIMgr.Ins.showNextView<RoomView>();
                UIMgr.Ins.showNextPopupView<AlarmClockView, AlarmClockInfo>(info);
            };
            TouchScreenView.Ins.StartCoroutine(GotoTmpEffectView(action));
        });
    }

    static void JumpRankView(string from)
    {
        Action action = () =>
        {
            UIMgr.Ins.showNextView<RankView>();
        };
        TouchScreenView.Ins.StartCoroutine(GotoTmpEffectView(action));
    }

    static void JumpRankMoudle(string from)
    {
        int index = int.Parse(from);

        Action action = () =>
        {
            NormalInfo normalInfo = new NormalInfo
            {
                index = index,
            };
            UIMgr.Ins.showNextView<RankView, NormalInfo>(normalInfo);
        };
        TouchScreenView.Ins.StartCoroutine(GotoTmpEffectView(action));
    }

    static void JumpRankAttrView(string from)
    {
        WWWForm wWWForm = new WWWForm();
        int index = int.Parse(from);
        switch (index)
        {
            case 1:
                wWWForm.AddField("type", "Charm");
                break;
            case 2:
                wWWForm.AddField("type", "Intell");
                break;
            case 3:
                wWWForm.AddField("type", "Evn");
                break;
            case 4:
                wWWForm.AddField("type", "Mana");
                break;
            case 0:
                wWWForm.AddField("type", "Main");
                break;
        }

        GameMonoBehaviour.Ins.RequestInfoPost(NetHeaderConfig.RANK_ATTR_TYPE, wWWForm, (RankSingle rankSingle) =>
        {
            RankDataMgr.Instance.RankSingle = rankSingle;
            RankDataMgr.Instance.RankType = RankType.Attr;

            Action action = () =>
            {

                NormalInfo info = new NormalInfo
                {
                    index = index + 2,
                };
                RankView view = UIMgr.Ins.showNextView<RankView>() as RankView;
                view.GoToMoudle<RankMoudle, NormalInfo>((int)RankView.MoudleType.Rank, info);

            };
            TouchScreenView.Ins.StartCoroutine(GotoTmpEffectView(action));
        });
    }

    static void JumpRankFavorView(string from)
    {
        if (from == "0")
        {
            from = "Main";
        }
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("type", from);
        GameMonoBehaviour.Ins.RequestInfoPost(NetHeaderConfig.RANK_FAVOR_ID, wWWForm, (RankSingle rankSingle) =>
         {
             RankDataMgr.Instance.RankSingle = rankSingle;
             RankDataMgr.Instance.RankType = RankType.Favor;
             Action action = () =>
             {
                 RankView view = UIMgr.Ins.showNextView<RankView>() as RankView;

                 NormalInfo normalInfo = new NormalInfo
                 {
                     index = (int)RankType.Favor,
                     type = int.Parse(from),
                 };
                 view.GoToMoudle<RankMoudle, NormalInfo>((int)RankView.MoudleType.Rank, normalInfo);
             };
             TouchScreenView.Ins.StartCoroutine(GotoTmpEffectView(action));
         });

    }

    static void JumpRankTimeView(string from)
    {

        if (from == "0")
        {
            from = "Main";
        }
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("type", from);
        GameMonoBehaviour.Ins.RequestInfoPost(NetHeaderConfig.RANK_CARD_ID, wWWForm, (RankSingle rankSingle) =>
        {
            RankDataMgr.Instance.RankSingle = rankSingle;
            RankDataMgr.Instance.RankType = RankType.Time;
            Action action = () =>
            {
                RankView view = UIMgr.Ins.showNextView<RankView>() as RankView;

                NormalInfo normalInfo = new NormalInfo
                {
                    index = (int)RankType.Time,
                    type = int.Parse(from),
                };
                view.GoToMoudle<RankMoudle, NormalInfo>((int)RankView.MoudleType.Rank, normalInfo);
            };
            TouchScreenView.Ins.StartCoroutine(GotoTmpEffectView(action));
        });

    }

    static void JumpAchievementMoudle(string from)
    {

        int type = int.Parse(from);
        WWWForm wWWForm = new WWWForm();
        GameMonoBehaviour.Ins.RequestInfoPost<DailyTask>(NetHeaderConfig.MISSION_LIST, wWWForm, (DailyTask dailyTask) =>
        {
            Action action = () =>
            {
                AchievementView view = UIMgr.Ins.showNextView<AchievementView, DailyTask>(dailyTask) as AchievementView;
                NormalInfo normalInfo;
                switch (type)
                {
                    case (int)TaskType.GrowUp:
                        AchievementDataMgr.Ins.CurrentTaskType = TaskType.GrowUp;
                        normalInfo = new NormalInfo()
                        {
                            type = (int)TaskType.GrowUp,
                        };
                        view.GoToMoudle<AchievementTaskMoudle, NormalInfo>((int)AchievementView.MoudleType.Task, normalInfo);
                        break;
                    case (int)TaskType.First:
                        AchievementDataMgr.Ins.CurrentTaskType = TaskType.GrowUp;
                        normalInfo = new NormalInfo()
                        {
                            type = (int)TaskType.First,
                        };
                        view.GoToMoudle<AchievementTaskMoudle, NormalInfo>((int)AchievementView.MoudleType.Task, normalInfo);
                        break;
                    case (int)TaskType.Role:
                        view.GoToMoudle<AchievementRoleMoudle>((int)AchievementView.MoudleType.Role);
                        break;
                    case (int)TaskType.Collect:
                        AchievementDataMgr.Ins.CurrentTaskType = TaskType.GrowUp;
                        normalInfo = new NormalInfo()
                        {
                            type = (int)TaskType.Collect,
                        };
                        view.GoToMoudle<AchievementTaskMoudle, NormalInfo>((int)AchievementView.MoudleType.Task, normalInfo);
                        break;
                }

            };
            TouchScreenView.Ins.StartCoroutine(GotoTmpEffectView(action));

        });
    }

    static void JumpAchievementRoleTaskMoudle(string from)
    {
        WWWForm wWWForm = new WWWForm();
        GameMonoBehaviour.Ins.RequestInfoPost<DailyTask>(NetHeaderConfig.MISSION_LIST, wWWForm, (DailyTask dailyTask) =>
        {
            Action action = () =>
            {
                AchievementView view = UIMgr.Ins.showNextView<AchievementView, DailyTask>(dailyTask) as AchievementView;
                AchievementDataMgr.Ins.CurrentTaskType = TaskType.Role;
                NormalInfo normalInfo = new NormalInfo()
                {
                    type = (int)TaskType.Role,
                    index = int.Parse(from),
                };
                view.GoToMoudle<AchievementTaskMoudle, NormalInfo>((int)AchievementView.MoudleType.Task, normalInfo);
            };
            TouchScreenView.Ins.StartCoroutine(GotoTmpEffectView(action));

        });
    }


    static WaitForSeconds wait = new WaitForSeconds(0.8f);
    static IEnumerator GotoTmpEffectView(Action action)
    {
        TouchScreenView.Ins.PlayTmpEffect();
        yield return wait;
        action();
    }
}


public enum JumpType
{
    /// <summary>
    /// 购买星星界面
    /// </summary>
    BUY_STAR_VIEW = 0,
    /// <summary>
    /// 商店界面
    /// </summary>
    SHOP_MALL_VIEW = 1,
    /// <summary>
    /// 制作工坊界面
    /// </summary>
    MAKE_GIFTS_VIEW = 2,
    /// <summary>
    /// 玩家信息界面
    /// </summary>
    PLAYER_HEAD_VIEW = 3,
    /// <summary>
    /// 玩家成长界面   0=>玩家成长页面  9=>属性升级界面  4=>娃娃升级界面  5=>娃娃时装界面
    /// </summary>
    ROLE_GROUP_VIEW = 4,


    /// <summary>
    /// 角色互动列表界面
    /// </summary>
    INTERACTIVE_ROLE_LIST = 8,
    /// <summary>
    /// 对应角色互动界面 9,11
    /// </summary>
    INTERACTIVE_MAIN_MOUDLE = 9,
    /// <summary>
    /// 剧情界面
    /// </summary>
    STORY_VIEW = 10,
    /// <summary>
    /// 商店分页界面 11,1
    /// </summary>
    SHOP_MALL_MOUDLE = 11,
    /// <summary>
    /// 手机首页
    /// </summary>
    PHONE_VIEW = 12,
    /// <summary>
    /// 角色聊天界面 13,11
    /// </summary>
    SMS_ACTOR_MOUDLE = 13,
    /// <summary>
    /// 房间首页
    /// </summary>
    ROOM_VIEW = 14,
    /// <summary>
    /// 闹钟页面
    /// </summary>
    ALARM_VIEW = 15,
    /// <summary>
    /// 排行榜首页
    /// </summary>
    RANK_VIEW = 16,
    /// <summary>
    /// 对应角色好感排行榜 17,11；0=>总好感
    /// </summary>
    FAVOR_RANK_VIEW = 17,
    /// <summary>
    /// 对应属性排行榜,0=>总属性，1=>魅力,2=>智慧,3=>环保,4=>魔法,
    /// </summary>
    ATTRIBUTE_RANK_VIEW = 18,
    /// <summary>
    /// 对应角色时刻排行榜 19,11；0=>总时刻
    /// </summary>
    TIME_RANK_VIEW = 19,
    /// <summary>
    /// 排行榜分榜，0=>好感，1=>时刻，2=>属性
    /// </summary>
    RANK_MOUDLE = 20,
    /// <summary>
    /// 成就任务，2=>成长，3=>首次，4=>角色，5=>收集，
    /// </summary>
    ACHIEVEMENT_VIEW = 21,
    /// <summary>
    /// 成就角色任务，22,11  
    /// </summary>
    ACHIEVEMENT_ROLE_TASK_VIEW = 22,
    /// <summary>
    /// 剧情
    /// </summary>
    STORY_ROLE = 23,
}
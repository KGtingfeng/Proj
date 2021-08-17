using System.Collections.Generic;
using FairyGUI;
using UnityEngine;
using FairyGUI.Utils;
using System;

public class SelectServiceMoudle : BaseMoudle
{
    public static SelectServiceMoudle moudle;
    int NEW_USER = 0;
    //1: 新服 2: 流畅 3: 火爆 4: 测试 5: 维护
    public static Dictionary<int, string> serverTypeUrl = new Dictionary<int, string>() {
        {1,"ui://t0kqvihpizqh12"},
        {2,"ui://t0kqvihpizqh12"},
        {3,"ui://t0kqvihpizqh11"},
        {4,"ui://t0kqvihpizqh12"},
        {5,"ui://t0kqvihpizqh13"}
    };

    public static List<ServerList> serverList;

    private static ServerList _currentServer;
    public static ServerList currentServer
    {
        set { _currentServer = value; }
        get { return _currentServer; }
    }

    GLoader serverTypeLoader;
    GTextField zoneText;
    GTextField serverNameText;


    GButton chackBtn;
    public override void InitMoudle(GComponent gComponent, int controllerIndex)
    {
        base.InitMoudle(gComponent, controllerIndex);
        moudle = this;

        InitUI();
        InitEvent();

    }

    public override void InitUI()
    {
        base.InitUI();
        chackBtn = SearchChild("n20").asButton;
        serverTypeLoader = SearchChild("n14").asLoader;
        zoneText = SearchChild("n15").asTextField;
        serverNameText = SearchChild("n39").asTextField;
    }

    public override void InitData()
    {
        base.InitData();
        chackBtn.selected = GameData.playerId == NEW_USER ? false : true;
        initServerListInfos();
        SwitchConfig switchConfig = GameData.SwitchConfigs.Find(a => a.key == SwitchConfig.KEY_REALNAME);
        if (switchConfig != null && switchConfig.value == "1")
        {
            if (GameData.User.status == 0)
            {
                UIMgr.Ins.showNextPopupView<RealNameView>();
            }
            else if (GameData.User.age < 18)
            {
                Extrand extrand = new Extrand
                {
                    key = "知道了",
                    callBack = QueryAnnouncement,
                    type = 1
                };
                extrand.msg = "该账号已纳入防沉迷保护系统，每日在线总时长不超过1.5小时，每天22点至次日8点间限制登录游戏；未满8周岁的用户不可进行充值；8周岁至15周岁的用户单次充值不超过50元，月累计充值不超过200元；年满16至17周岁，单次充值不超过100元，月累计充值不超过400元。";
                UIMgr.Ins.showNextPopupView<RealNameView, Extrand>(extrand);
            }

        }
        else
            QueryAnnouncement();
    }

    public override void InitEvent()
    {
        SearchChild("n22").onClick.Set(OnClickEnter);
        //announce
        SearchChild("n3").onClick.Set(QueryAnnouncement);
        //res 
        SearchChild("n4").onClick.Set(() => { Debug.Log("res"); });
        //logout
        SearchChild("n10").onClick.Set(() =>
        {
            EventMgr.Ins.DispachEvent(EventConfig.LOGIN_GOTO_LOGIN);
        });

        SearchChild("n17").onClick.Set(() =>
        {
            UIMgr.Ins.showNextPopupView<ChooseServerView, List<ServerList>>(serverList);
        });

        SearchChild("n21").onClick.Set(QueryServiceAgreement);

        RegisterEvent();
    }

    void RegisterEvent()
    {
        //公告
        EventMgr.Ins.RegisterEvent<List<Announcement>>(EventConfig.LOGIN_ANNOUCEMENT, ShowAnnounceMent);
        EventMgr.Ins.RegisterEvent<List<Announcement>>(EventConfig.LOGIN_SERVICEAGREEMENT, ShowAnnounceMent);
        EventMgr.Ins.RegisterEvent<List<ServerList>>(EventConfig.LOGIN_SERVERLIST, initCurrentServerData);
    }

    public void initServerListInfos()
    {
        if (serverList == null || serverList.Count <= 0)
        {
            QueryServerList();
            return;
        }
        initCurrentServerData(serverList);
    }

    /// <summary>
    /// 请求公告
    /// </summary>
    public void QueryAnnouncement()
    {
        WWWForm wWWForm = new WWWForm();
        GameMonoBehaviour.Ins.RequestInfoPostList<Announcement>(NetHeaderConfig.QUERY_ANNOUNCEMENT, wWWForm, null);
    }

    /// <summary>
    /// 请求服务协议
    /// </summary>
    void QueryServiceAgreement()
    {
        WWWForm wWWForm = new WWWForm();
        GameMonoBehaviour.Ins.RequestInfoPostList<Announcement>(NetHeaderConfig.QUERY_SERVICEAGREEMENT, wWWForm, null);
    }

    //请求服务器列表
    void QueryServerList()
    {
        WWWForm wWWForm = new WWWForm();
        GameMonoBehaviour.Ins.RequestInfoPostList<ServerList>(NetHeaderConfig.QUERY_SERVICELIST, wWWForm, null);
    }

    void QueryPlayerInfo()
    {
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("playerId", GameData.playerId);
        wWWForm.AddField("userId", GameData.User.id);
        GameMonoBehaviour.Ins.RequestInfoPost<Player>(NetHeaderConfig.GET_PLAYR_INFO, wWWForm, RequestPlayerInfoCallBack);
    }

    void ShowAnnounceMent(List<Announcement> announcements)
    {
        UIMgr.Ins.showNextPopupView<AnnouncementView, List<Announcement>>(announcements);
    }

    //获取当前服务器列表数据
    void initCurrentServerData(List<ServerList> currentServerList)
    {
        serverList = new List<ServerList>();
        serverList = currentServerList;
        if (serverList != null && serverList.Count > 0)
        {
            int serverId;
            if (int.TryParse(PlayerPrefsUtil.GetServerId(GameData.User.id), out serverId))
            {
                currentServer = serverList.Find(a => a.id == serverId);
            }
            else
            {
                currentServer = serverList[0];
                serverId = currentServer.id;
            }
            SynchronizeCurrentServer();
        }
    }

    public void SynchronizeCurrentServer()
    {
        serverTypeLoader.url = serverTypeUrl[currentServer.status];
        zoneText.text = currentServer.zone;
        serverNameText.text = currentServer.server_name;
    }


    void RequestPlayerInfoCallBack()
    {
        if(!GameData.isOpenGuider)
        {
            EventMgr.Ins.DispachEvent(EventConfig.GOTO_VIEW_MAIN_EFFECT);
            return;
        }
        StoryCacheMgr.storyCacheMgr.GetStoryGameSave("Newbie", 0, (storyGameSave) =>
        {
            if (!storyGameSave.IsDefault)
            {
                string[] save = storyGameSave.value.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                GameData.guiderCurrent = GuiderInfoLinked.GetCurrGuiderInfo(int.Parse(save[0]), int.Parse(save[1]));
                if (GameData.guiderCurrent != null)
                {
                    string[] roll_to = GameData.guiderCurrent.guiderInfo.roll_to.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (roll_to.Length < 2 )
                    {
                EventMgr.Ins.DispachEvent(EventConfig.GOTO_VIEW_MAIN_EFFECT);
                        return;
                    }
                    GameData.guiderCurrent = GuiderInfoLinked.GetCurrGuiderInfo(int.Parse(roll_to[0]), int.Parse(roll_to[1]));
                    if (GameData.guiderCurrent != null&& GameData.guiderCurrent.guiderInfo.flow == 1)
                    {
                            CallBackList callBackList = new CallBackList();
                            callBackList.callBack1 = () => {
                                UIMgr.Ins.showViewWithReleaseOthers<MainView>();
                            };
                            UIMgr.Ins.showNextPopupView<ShanbaiWaittimeView, CallBackList>(callBackList);
                    }
                    else
                    {
                        EventMgr.Ins.DispachEvent(EventConfig.GOTO_VIEW_MAIN_EFFECT);
                    }
                }
            }
            else
            {
                EventMgr.Ins.DispachEvent(EventConfig.GOTO_VIEW_MAIN_EFFECT);
            }

        });

        //EventMgr.Ins.DispachEvent(EventConfig.GOTO_VIEW_MAIN_EFFECT);
        //WWWForm wWWForm = new WWWForm();
        //WWWForm wWForm = new WWWForm();  

        //if (GameData.isOpenGuider)
        //{
        //    WWWForm wForm = new WWWForm();
        //    wForm.AddField("nodeId", 0);
        //    wForm.AddField("key", "Newbie");
        //    //todo:小游戏家族方式
        //    GameMonoBehaviour.Ins.RequestInfoPostHaveData<List<StoryGameSave>>(NetHeaderConfig.STROY_LOAD_GAME, wForm, (List<StoryGameSave> storyGameSaves) =>
        //    {
        //        if (storyGameSaves.Count > 0)
        //        {
        //            string[] save = storyGameSaves[0].value.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        //            GameData.guiderCurrent = GuiderInfoLinked.GetCurrGuiderInfo(int.Parse(save[0]), int.Parse(save[1]));
        //            if (GameData.guiderCurrent != null)
        //            {
        //                string[] roll_to = GameData.guiderCurrent.guiderInfo.roll_to.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        //                if (roll_to.Length < 2)
        //                {
        //                    GameData.isGuider = false;
        //                    EventMgr.Ins.DispachEvent(EventConfig.GOTO_VIEW_MAIN_EFFECT);
        //                    return;
        //                }
        //                GameData.guiderCurrent = GuiderInfoLinked.GetCurrGuiderInfo(int.Parse(roll_to[0]), int.Parse(roll_to[1]));
        //                if (GameData.guiderCurrent != null)
        //                    GameData.isGuider = true;
        //            }
        //        }
        //        EventMgr.Ins.DispachEvent(EventConfig.GOTO_VIEW_MAIN_EFFECT);
        //    }, false);
        //}
        //else
        //{
        //    EventMgr.Ins.DispachEvent(EventConfig.GOTO_VIEW_MAIN_EFFECT);
        //}


        //todo:渠道开关 和登录信息无关
        //GameMonoBehaviour.Ins.RequestInfoPost(NetHeaderConfig.CONFIG_GAME_ALL, wWForm,
        //    (List<ChannelSwitchConfig> configs) =>
        //{
        //    GameData.Configs = configs;
        //    ChannelSwitchConfig guid = configs.Find(a => a.key == ChannelSwitchConfig.KEY_GUID);
        //    if (guid != null)
        //    {
        //        if (guid.value == 1)
        //            GameData.isOpenGuider = true;
        //        else
        //            GameData.isOpenGuider = false;
        //    }
        //    else
        //    {
        //        GameData.isOpenGuider = false;
        //    }
        //    if (GameData.isOpenGuider)
        //    {
        //        WWWForm wForm = new WWWForm();
        //        wForm.AddField("nodeId", 0);
        //        wForm.AddField("key", "Newbie");
        //        //todo:小游戏家族方式
        //        GameMonoBehaviour.Ins.RequestInfoPostHaveData<List<StoryGameSave>>(NetHeaderConfig.STROY_LOAD_GAME, wForm, (List<StoryGameSave> storyGameSaves) =>
        //        {
        //            if (storyGameSaves.Count > 0)
        //            {
        //                string[] save = storyGameSaves[0].value.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        //                GameData.guiderCurrent = GuiderInfoLinked.GetCurrGuiderInfo(int.Parse(save[0]), int.Parse(save[1]));
        //                if (GameData.guiderCurrent != null)
        //                {
        //                    string[] roll_to = GameData.guiderCurrent.guiderInfo.roll_to.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        //                    if (roll_to.Length < 2)
        //                    {
        //                        GameData.isGuider = false;
        //                        EventMgr.Ins.DispachEvent(EventConfig.GOTO_VIEW_MAIN_EFFECT);
        //                        return;
        //                    }
        //                    GameData.guiderCurrent = GuiderInfoLinked.GetCurrGuiderInfo(int.Parse(roll_to[0]), int.Parse(roll_to[1]));
        //                    if (GameData.guiderCurrent != null)
        //                        GameData.isGuider = true;
        //                }
        //            }
        //            EventMgr.Ins.DispachEvent(EventConfig.GOTO_VIEW_MAIN_EFFECT);
        //        }, false);
        //    }
        //    else
        //    {
        //        EventMgr.Ins.DispachEvent(EventConfig.GOTO_VIEW_MAIN_EFFECT);
        //    }

        //});
        //GameMonoBehaviour.Ins.RequestInfoPost<PlayerRedpoint>(NetHeaderConfig.RED_POINT, wWWForm, () =>
        //{

        //    WWWForm wWForm = new WWWForm();
        //    //todo:渠道开关 和登录信息无关
        //    GameMonoBehaviour.Ins.RequestInfoPost(NetHeaderConfig.CONFIG_GAME_ALL, wWForm, (List<ChannelSwitchConfig> configs) =>
        //    {
        //        GameData.Configs = configs;
        //        ChannelSwitchConfig guid = configs.Find(a => a.key == ChannelSwitchConfig.KEY_GUID);
        //        if (guid != null)
        //        {
        //            if (guid.value == 1) 
        //                GameData.isOpenGuider = true;
        //             else
        //                GameData.isOpenGuider = false;
        //        }
        //        else
        //        {
        //            GameData.isOpenGuider = false;
        //        }
        //        if (GameData.isOpenGuider)
        //        {
        //            WWWForm wForm = new WWWForm();
        //            wForm.AddField("nodeId", 0);
        //            wForm.AddField("key", "Newbie");
        //            //todo:小游戏家族方式
        //            GameMonoBehaviour.Ins.RequestInfoPostHaveData<List<StoryGameSave>>(NetHeaderConfig.STROY_LOAD_GAME, wForm, (List<StoryGameSave> storyGameSaves) =>
        //            {
        //                if (storyGameSaves.Count > 0)
        //                {
        //                    string[] save = storyGameSaves[0].value.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        //                    GameData.guiderCurrent = GuiderInfoLinked.GetCurrGuiderInfo(int.Parse(save[0]), int.Parse(save[1]));
        //                    if (GameData.guiderCurrent != null)
        //                    {
        //                        string[] roll_to = GameData.guiderCurrent.guiderInfo.roll_to.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        //                        if (roll_to.Length < 2)
        //                        {
        //                            GameData.isGuider = false;
        //                            EventMgr.Ins.DispachEvent(EventConfig.GOTO_VIEW_MAIN_EFFECT);
        //                            return;
        //                        }
        //                        GameData.guiderCurrent = GuiderInfoLinked.GetCurrGuiderInfo(int.Parse(roll_to[0]), int.Parse(roll_to[1]));
        //                        if (GameData.guiderCurrent != null)
        //                            GameData.isGuider = true;
        //                    }
        //                }
        //                EventMgr.Ins.DispachEvent(EventConfig.GOTO_VIEW_MAIN_EFFECT);
        //            }, false);
        //        }
        //        else
        //        {
        //            EventMgr.Ins.DispachEvent(EventConfig.GOTO_VIEW_MAIN_EFFECT);
        //        }

        //    });
        //}, true);
    }

    void OnClickEnter()
    {
        if (!chackBtn.selected)
        {
            UIMgr.Ins.showErrorMsgWindow(MsgException.NO_AGREE_SERVICE_AGREEMENT);
            return;
        }
        SwitchConfig realName = GameData.SwitchConfigs.Find(a => a.key == SwitchConfig.KEY_REALNAME);
        SwitchConfig time = GameData.SwitchConfigs.Find(a => a.key == SwitchConfig.KEY_TIME);


        if (realName != null && time != null &&
            realName.value == "1" && time.value == "1" &&
        GameData.User.status == 1 && GameData.User.type != 0)
        {
            Extrand extrand = new Extrand
            {
                key = "确定",
                callBack = GotoLogin,
                type = 1
            };
            if (GameData.User.type == 1)
            {
                extrand.msg = "根据健康系统限制，由于你是未成年玩家，每日22点 - 次日8点无法登陆，注意休息！明天再来吧！";
                UIMgr.Ins.showNextPopupView<RealNameView, Extrand>(extrand);
            }
            else if (GameData.User.type == 2)
            {
                float limitTime = float.Parse(GameData.User.limitHour);
                extrand.msg = "根据健康系统限制，由于你是未成年玩家，非节假日仅能游戏" + limitTime.ToString("0.00") + "小时。你今天已经进行游戏" + limitTime.ToString("0.00") + "小时，不能继续游戏，请注意休息";
                UIMgr.Ins.showNextPopupView<RealNameView, Extrand>(extrand);
            }
        }
        else
        {
            //玩家id为0 或者没有选择角色都需要
            if (GameData.playerId == NEW_USER)//|| GameData.Player.card.Count == 0)
            {
                //CallBackList callBackList = new CallBackList();
                //callBackList.callBack1 = () => {
                //    UIMgr.Ins.showNextView<ChooseRoleView>();
                //};
                //UIMgr.Ins.showNextPopupView<LoginAnimationView, CallBackList>(callBackList);

                CallBackList callBackList = new CallBackList();
                callBackList.callBack1 = () => {
                    UIMgr.Ins.showNextView<ChooseRoleView>();

                };
                UIMgr.Ins.showNextPopupView<LoginWaitimeView, CallBackList>(callBackList);
            }
            else
                QueryPlayerInfo();
            PlayerPrefsUtil.SerServerInfo(GameData.User.id, currentServer.id.ToString());
        }
    }



    void GotoLogin()
    {
        EventMgr.Ins.DispachEvent(EventConfig.LOGIN_GOTO_LOGIN);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using System;
using Spine.Unity;

[ViewAttr("Game/UI/H_Interactive", "H_Interactive", "Interactive")]
public class InteractiveView : BaseView
{

    public static InteractiveView ins;
    public bool isGuide;
    Player PlayerInfo
    {
        get { return GameData.Player; }
    }
    public enum MoudleType
    {
        ROLE_LIST = 0,
        INTERACTIVE,
        //个人信息
        ROLL_INFO,
        //今日愿望
        TODAY_WISH,
        //赠送礼物
        SEND_GIFTS,
        //外观
        APPEARANCE = 6,
    };

    readonly Dictionary<MoudleType, string> moudleNamePairs = new Dictionary<MoudleType, string>()
    {
        { MoudleType.TODAY_WISH,"n57"},
        { MoudleType.SEND_GIFTS,"n58"},
    };

    GTextField loveText;
    GTextField diamondText;
    GGraph spineGraph;

    GGroup leftGroup;
    GGroup rightGroup;
    GGroup upGroup;

    GGraph backgroundGraph;

    GButton showMainButton;
    public Vector2 showMainPos;

    public override void InitUI()
    {
        base.InitUI();
        controller = ui.GetController("c1");
        ui.opaque = false;

        loveText = SearchChild("n103").asCom.GetChild("n15").asTextField;
        diamondText = SearchChild("n103").asCom.GetChild("n16").asTextField;
        spineGraph = SearchChild("n91").asCom.GetChild("n91").asGraph;

        leftGroup = SearchChild("n97").asGroup;
        rightGroup = SearchChild("n98").asGroup;
        upGroup = SearchChild("n33").asGroup;

        rightGroupx = rightGroup.x;


        backgroundGraph = SearchChild("n100").asGraph;
        showMainButton = SearchChild("n31").asButton;
        showMainPos = showMainButton.position;
        InitEvent();
        ins = this;
    }
    public override void InitData()
    {
        base.InitData();
        InitTopInfo();
        GoToMoudle<InteractiveRoleListMoudle>((int)MoudleType.ROLE_LIST);
         
    }

    public override void InitData<D>(D data)
    {
        base.InitData(data);
        InitTopInfo();
        NormalInfo normalInfo = data as NormalInfo;
        InitMoudle(normalInfo);
    }

    void InitTopInfo()
    {
        //love
        loveText.text = PlayerInfo.love + "";
        //money
        diamondText.text = PlayerInfo.diamond + "";
    }


    public override void InitEvent()
    {
        SearchChild("n36").onClick.Set(ClickBack);
        SearchChild("n43").onClick.Set(ClickBlank);

        SearchChild("n103").asCom.GetChild("n13").onClick.Set(() =>
        {
            EventMgr.Ins.DispachEvent(EventConfig.CLOSE_INTERACTIVE_TALK);
            UIMgr.Ins.showNextPopupView<BuyStarsView>();
        });
        //click diamondBtn 
        SearchChild("n103").asCom.GetChild("n14").onClick.Set(() =>
        {
            EventMgr.Ins.DispachEvent(EventConfig.CLOSE_INTERACTIVE_TALK);
            UIMgr.Ins.showNextPopupView<ShopMallView>();
        });
        //故事
        SearchChild("n22").onClick.Set(() =>
        {
            GoToMoudle<DollInfoMoudle>((int)MoudleType.ROLL_INFO);
        });
        //外观
        SearchChild("n23").onClick.Set(() =>
        {
            GoToMoudle<ChangeAppearanceMoudle>((int)MoudleType.APPEARANCE);
        });
        //今日心愿
        SearchChild("n24").onClick.Set(() =>
        {
            GotoTodayWish();
        });
        //送礼
        SearchChild("n25").onClick.Set(() =>
        {
            GotoDollGifts();
        });
        //制作礼物
        SearchChild("n60").onClick.Set(() =>
        {
            GotoMakeGift();
        });
        //购买礼物
        SearchChild("n61").onClick.Set(() =>
        {
            CloseRoleTalk();
            UIMgr.Ins.showNextPopupView<ShopMallView>();
        });
        //展示在主页
        showMainButton.onClick.Set(ShowOnMain);
        SearchChild("n27").onClick.Set(ShowPhone);

        EventMgr.Ins.RegisterEvent(EventConfig.REFRESH_INTERACTIVE_TOP_INFO, InitTopInfo);
        EventMgr.Ins.RegisterEvent<FavorItem>(EventConfig.PRESENT_SUCCESS_EFFECT, ShowPrensent);
        EventMgr.Ins.RegisterEvent(EventConfig.CLOSE_PRESENT_TALK, CloseRoleTalk);
        EventMgr.Ins.RegisterEvent(EventConfig.INTERACTIVE_MAIN_EFFECT, ShowOpenEffect);
    }

    //点击空白处
    void ClickBlank()
    {
        switch (controller.selectedIndex)
        {
            case (int)MoudleType.TODAY_WISH:
                EventMgr.Ins.RemoveEvent(EventConfig.REFRSH_COUNTDOWN);
                EventMgr.Ins.RemoveEvent(EventConfig.REFRESH_TODAY_WISH);
                CloseRoleTalk();
                GoToMoudle<InteractiveMainMoudle>((int)MoudleType.INTERACTIVE);
                break;
            case (int)MoudleType.ROLL_INFO:
            case (int)MoudleType.SEND_GIFTS:
                CloseRoleTalk();
                EventMgr.Ins.RemoveEvent(EventConfig.REFRESH_PROPS);
                GoToMoudle<InteractiveMainMoudle>((int)MoudleType.INTERACTIVE);
                break;
            case (int)MoudleType.APPEARANCE:
                EventMgr.Ins.DispachEvent(EventConfig.BLANK_CLICK);
                break;
        }
    }

    //关闭模型展示及语音
    void CloseSpineInfo(int index)
    {
        EventMgr.Ins.DispachEvent(EventConfig.CLOSE_INTERACTIVE_TALK);
        if (index == (int)MoudleType.ROLE_LIST)
        {
            spineGraph.visible = false;
            return;
        }
        spineGraph.visible = true;
    }

    /// <summary>
    /// 从外部直接进入界面
    /// </summary>
    /// <param name="normalInfo">type ：MoudleType</param>
    void InitMoudle(NormalInfo normalInfo)
    {
        switch (normalInfo.type)
        {
            case (int)MoudleType.SEND_GIFTS:
                InteractiveDataMgr.ins.SelectRoleCardConfig = DataUtil.GetGameInitCard(normalInfo.index);
                //GotoDollGifts();
                RequestSkinList(normalInfo.index);
                break;
            case (int)MoudleType.INTERACTIVE:
                GoToMoudle<InteractiveMainMoudle>((int)MoudleType.INTERACTIVE);
                break;
            default:
                GoToMoudle<InteractiveRoleListMoudle>((int)MoudleType.ROLE_LIST);
                break;
        }
    }


    void ClickBack()
    {
        switch (controller.selectedIndex)
        {
            case (int)MoudleType.ROLE_LIST:
                UIMgr.Ins.showMainView<InteractiveView>();
                break;
            case (int)MoudleType.INTERACTIVE:
                GoToMoudle<InteractiveRoleListMoudle>((int)MoudleType.ROLE_LIST);
                break;
            case (int)MoudleType.TODAY_WISH:
                EventMgr.Ins.RemoveEvent(EventConfig.REFRSH_COUNTDOWN);
                EventMgr.Ins.RemoveEvent(EventConfig.REFRESH_TODAY_WISH);
                CloseRoleTalk();
                GoToMoudle<InteractiveMainMoudle>((int)MoudleType.INTERACTIVE);
                break;
            case (int)MoudleType.ROLL_INFO:
            case (int)MoudleType.SEND_GIFTS:
                CloseRoleTalk();
                EventMgr.Ins.RemoveEvent(EventConfig.REFRESH_PROPS);
                GoToMoudle<InteractiveMainMoudle>((int)MoudleType.INTERACTIVE);
                break;
            case (int)MoudleType.APPEARANCE:
                EventMgr.Ins.DispachEvent(EventConfig.APPEARANCE_REBACK);
                break;

        }
    }

    public void GotoDollGifts()
    {
        EventMgr.Ins.DispachEvent(EventConfig.CLOSE_INTERACTIVE_TALK);
        WWWForm wWWForm = new WWWForm();
        GameMonoBehaviour.Ins.RequestInfoPostList<PlayerProp>(NetHeaderConfig.ACTOR_PROP_LIST, wWWForm, RequestPropListSuccess);
    }
    void RequestPropListSuccess()
    {
        ShowCloseEffect();
        GoToMoudle<DollGiftsMoudle>((int)MoudleType.SEND_GIFTS);
    }
    /// <summary>
    /// 展示在主页
    /// </summary>
    public void ShowOnMain()
    {
        if (InteractiveDataMgr.ins.CurrentPlayerActor.actor_id == GameData.Player.homeActor.actor_id)
        {
            UIMgr.Ins.showErrorMsgWindow(MsgException.HAS_SHOWN);
            if (GameData.isGuider)
            {
                UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);
            }
            return;
        }
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("actorId", InteractiveDataMgr.ins.CurrentPlayerActor.actor_id);
        GameMonoBehaviour.Ins.RequestInfoPost<HomeActor>(NetHeaderConfig.SET_HOMEACTOR, wWWForm, RequestShowOnMain);
    }
    /// <summary>
    /// 手机界面
    /// </summary>
    void ShowPhone()
    {
        EventMgr.Ins.DispachEvent(EventConfig.CLOSE_INTERACTIVE_TALK);
        WWWForm wWWForm = new WWWForm();
        GameMonoBehaviour.Ins.RequestInfoPost<List<SmsListIndex>>(NetHeaderConfig.CELL_LIST_INDEX, wWWForm, RequestPhoneInfo);
    }

    void RequestPhoneInfo(List<SmsListIndex> smsLists)
    {
        StartCoroutine(GotoTmpEffectView(() =>
        {
            UIMgr.Ins.showNextView<SMSView, List<SmsListIndex>>(smsLists);
        }));
    }

    IEnumerator GotoTmpEffectView(Action action)
    {
        TouchScreenView.Ins.PlayTmpEffect();
        yield return new WaitForSeconds(0.8f);
        UIMgr.Ins.showMainView<InteractiveView>();
        action();
    }

    void RequestShowOnMain()
    {
        if (GameData.isGuider)
        {
            UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);
        }
        else
        {
            UIMgr.Ins.showErrorMsgWindow(MsgException.SHOW_SUCCESSFULLY);
        }
    }

    void GotoTodayWish()
    {
        EventMgr.Ins.DispachEvent(EventConfig.CLOSE_INTERACTIVE_TALK);
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("actorId", InteractiveDataMgr.ins.CurrentPlayerActor.actor_id);
        GameMonoBehaviour.Ins.RequestInfoPost<ActorTask>(NetHeaderConfig.TODAY_WISH, wWWForm, RequestGotoTodayWish);
    }

    void RequestGotoTodayWish(ActorTask actorTask)
    {
        ShowCloseEffect();
        GoToMoudle<DollTodayWishMoudle, ActorTask>((int)MoudleType.TODAY_WISH, actorTask);
    }

    public void GotoMakeGift()
    {
        CloseRoleTalk();
        EventMgr.Ins.DispachEvent(EventConfig.CLOSE_INTERACTIVE_TALK);
        UIMgr.Ins.showNextPopupView<MakeGiftsView>();
    }

    /// <summary>
    /// 互动主界面角色信息
    /// </summary>
    void RequestSkinList(int id)
    {
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("actorId", id);
        GameMonoBehaviour.Ins.RequestInfoPost<PlayerActor>(NetHeaderConfig.ACTOR_SKIN_LIST, wWWForm, () =>
        {
            GoToMoudle<InteractiveMainMoudle>((int)MoudleType.INTERACTIVE);
            GotoDollGifts();
        });
    }

    private void LateUpdate()
    {
        if (InteractiveMainMoudle.ins != null)
            InteractiveMainMoudle.ins.ListenSoundPlayOver();
    }

    #region 送礼后说话
    void ShowPrensent(FavorItem item)
    {
        StartCoroutine(ShowPrensentSuccess(item));
    }


    IEnumerator ShowPrensentSuccess(FavorItem item)
    {
        yield return 0;
        item.actorId = InteractiveDataMgr.ins.CurrentPlayerActor.actor_id;
        UIMgr.Ins.showNextPopupView<FavorView, FavorItem>(item);
        yield return new WaitForSeconds(2f);
        if (item.newFavor - item.oldFavor != 0)
        {
            FXMgr.CreateEffectWithGGraph(backgroundGraph, new Vector3(-18, -279), "UI_juesebeijingguang", 162, 2);

            UIMgr.Ins.showNextPopupView<FavorLevelUpView, FavorItem>(item);
            yield return new WaitForSeconds(5f);
        }

        isShowRoleTalk = true;
        ErrorMsg message = new ErrorMsg(InteractiveDataMgr.ins.GetTalkText());
        UIMgr.Ins.showWindow<ShowRoleTalkWindow>(message);

        yield return new WaitForSeconds(2f);
        CloseRoleTalk();
    }

    bool isShowRoleTalk;
    void CloseRoleTalk()
    {
        if (isShowRoleTalk)
        {
            if (BaseWindow.window is ShowRoleTalkWindow && BaseWindow.window.contentPane.visible)
            {
                GRoot.inst.HideWindowImmediately(BaseWindow.window);
            }
            isShowRoleTalk = false;
        }

    }

    #endregion

    public override void GoToMoudle<T, D>(int index, D data)
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
            }
            else
                gComponent = ui;

            baseMoudle.baseView = this;
            baseMoudle.InitMoudle<D>(gComponent, index, data);
        }
        baseMoudle.InitData<D>(data);
        CloseSpineInfo(index);
        SwitchController(index);
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
            }
            else
                gComponent = ui;

            baseMoudle.baseView = this;
            baseMoudle.InitMoudle(gComponent, index);
        }
        CloseSpineInfo(index);
        baseMoudle.InitData();
        SwitchController(index);
    }

    float rightGroupx;
    public void ShowOpenEffect()
    {
        leftGroup.x = -197;
        rightGroup.x = 725;
        leftGroup.alpha = 0;
        rightGroup.alpha = 0;

        leftGroup.TweenMoveX(4, 1.2f).SetEase(EaseType.QuartOut);
        rightGroup.TweenMoveX(rightGroupx, 1.2f).SetEase(EaseType.QuartOut);
        upGroup.TweenMoveY(162, 1.2f).SetEase(EaseType.QuartOut);

        leftGroup.TweenFade(1, 1.2f).SetEase(EaseType.QuartOut);
        rightGroup.TweenFade(1, 1.2f).SetEase(EaseType.QuartOut);
        upGroup.TweenFade(1, 1.2f).SetEase(EaseType.QuartOut);

    }

    public void ShowCloseEffect()
    {
        leftGroup.TweenMoveX(-197, 1.2f).SetEase(EaseType.QuartOut);
        rightGroup.TweenMoveX(725, 1.2f).SetEase(EaseType.QuartOut);

        leftGroup.TweenFade(0, 1.2f).SetEase(EaseType.QuartOut);
        rightGroup.TweenFade(0, 1.2f).SetEase(EaseType.QuartOut);
    }
    public Vector2 GetButtonPos(int index)
    {
        return SearchChild("n" + index).asCom.position;
    }

}

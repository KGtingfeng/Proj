using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using System;


[ViewAttr("Game/UI/P_Friend", "P_Friend", "Friend")]
public class FriendView : BaseView
{
    public static FriendView ins;
    public enum MoudleType
    {
        /// <summary>
        /// 好友列表
        /// </summary>
        FREINDS_LIST = 0,
        /// <summary>
        /// 查找好友
        /// </summary>
        SEARCH_FREIND,
        /// <summary>
        /// 好友申请
        /// </summary>
        FREINDS_APPLY
    }

    readonly Dictionary<MoudleType, string> moudleNamePairs = new Dictionary<MoudleType, string>()
    {
        { MoudleType.FREINDS_LIST,"n10"},
        { MoudleType.SEARCH_FREIND,"n5" },
        { MoudleType.FREINDS_APPLY,"n16"}
    };

    GList topBtnList;           //顶部选项按钮
    int selectType;
    GLoader bgLoader;
    GTextField love;
    GTextField diamond;
    GComponent topInfoCom;

    public override void InitUI()
    {
        base.InitUI();
        topBtnList = SearchChild("n6").asList;
        controller = ui.GetController("c1");

        bgLoader = SearchChild("n17").asLoader;
        topInfoCom = SearchChild("n26").asCom;
        love = topInfoCom.GetChild("n15").asTextField;
        diamond = topInfoCom.GetChild("n16").asTextField;

        InitEvent();
        ins = this;
    }

    public override void InitEvent()
    {
        base.InitEvent();
        //返回按钮
        SearchChild("n4").onClick.Set(() =>
        {
            UIMgr.Ins.showMainView<FriendView>();
        });
        SearchChild("n26").asCom.GetChild("n13").onClick.Set(() =>
        {
            UIMgr.Ins.showNextPopupView<BuyStarsView>();
        });
        //click diamondBtn 
        SearchChild("n26").asCom.GetChild("n14").onClick.Set(() =>
        {
            UIMgr.Ins.showNextPopupView<ShopMallView>();
        });

        //绑定选项卡按钮事件
        topBtnList.onClickItem.Set(() =>
        {
            if (selectType != topBtnList.selectedIndex)
            {
                selectType = topBtnList.selectedIndex;

                switch ((MoudleType)selectType)
                {
                    case MoudleType.FREINDS_LIST:
                        GotoFriendMainMoudle();
                        break;
                    case MoudleType.SEARCH_FREIND:
                        GotoSearchFriendMoudle();
                        break;
                    case MoudleType.FREINDS_APPLY:
                        GotoApllyFriendMoudle();
                        break;
                    default:
                        break;
                }
            }
        });

        //注册查看好友详情事件
        EventMgr.Ins.RegisterEvent<NormalInfo>(EventConfig.GET_FRIEND_DETAIL, OnClickDetail);
        //注册刷新顶部按钮事件
        EventMgr.Ins.RegisterEvent(EventConfig.REFRESH_FRIEND_TOP_INFO, RefreshTopInfo);
        //注册刷新顶部小红点事件
        EventMgr.Ins.RegisterEvent(EventConfig.FRIEND_TAB_RED_POINT, InitTopBtnList);
    }



    public override void InitData<D>(D data)
    {
        base.InitData();


        bgLoader.url = UrlUtil.GetShopBgUrl("Shop");
        RefreshTopInfo();

        Friend info = data as Friend;
        selectType = (int)MoudleType.FREINDS_LIST;
        topBtnList.selectedIndex = selectType;
        GoToMoudle<FriendMainMoudle, Friend>(selectType, info);
        InitTopBtnList();
    }

    void RefreshTopInfo()
    {
        love.text = GameData.Player.love + "";
        diamond.text = GameData.Player.diamond + "";
    }
    /// <summary>
    /// 去到好友列表模块
    /// </summary>
    private void GotoFriendMainMoudle()
    {
        FriendDataMgr.Ins.RequestFriendInfos(1, 12, info =>
        {
            GoToMoudle<FriendMainMoudle, Friend>(selectType, info);
        });

    }
    /// <summary>
    /// 去到查找好友模块
    /// </summary>
    private void GotoSearchFriendMoudle()
    {
        WWWForm wWWForm = new WWWForm();
        GameMonoBehaviour.Ins.RequestInfoPost<List<FriendInfo>>(NetHeaderConfig.FRIEND_RECOMMEND, wWWForm, (List<FriendInfo> friendInfos) =>
        {
            GoToMoudle<FriendSearchMoudle, List<FriendInfo>>(selectType, friendInfos);
        });
    }

    private void GotoApllyFriendMoudle()
    {
        WWWForm wWWForm = new WWWForm();
        GameMonoBehaviour.Ins.RequestInfoPost<Friend>(NetHeaderConfig.FRIEND_APLLY_LIST, wWWForm, (Friend friendInfos) =>
        {
            GoToMoudle<FriendApplyMoudle, Friend>(selectType, friendInfos);
        });
    }

    void OnClickDetail(NormalInfo info)        //显示玩家具体信息
    {

        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("type", info.index);
        GameMonoBehaviour.Ins.RequestInfoPost<PlayerInfo>(NetHeaderConfig.RANK_PLAYER_ID, wWWForm, (PlayerInfo friendInfo) =>
        {
            friendInfo.player.playerId = info.index;
            friendInfo.player.isApply = info.type;

            UIMgr.Ins.showNextPopupView<PlayerInfoView, PlayerInfo>(friendInfo);
        });
    }

    public void InitTopBtnList()
    {
        topBtnList.itemRenderer = TopBtnRenderer;
        topBtnList.numItems = 3;
    }

    void TopBtnRenderer(int index, GObject obj)
    {
        GComponent com = obj.asCom;
        com.GetController("c1").selectedIndex = RedpointMgr.Ins.friendRedPoint[index];
    }

    #region 两个GoToMoudle方法
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
        baseMoudle.InitData();
        SwitchController(index);
    }
    #endregion


    public void NewbieSearch()
    {
        topBtnList.selectedIndex = (int)MoudleType.SEARCH_FREIND;
        selectType = topBtnList.selectedIndex;
        GotoSearchFriendMoudle();

    }

    public void NewbieApply()
    {
        topBtnList.selectedIndex = (int)MoudleType.FREINDS_APPLY;
        selectType = topBtnList.selectedIndex;
        GotoApllyFriendMoudle();
    }
}

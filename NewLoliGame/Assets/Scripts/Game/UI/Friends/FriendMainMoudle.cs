using FairyGUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// 好友界面
/// </summary>
public class FriendMainMoudle : BaseMoudle
{
    enum GiveType
    {
        /// <summary>
        /// 已赠送
        /// </summary>
        GIVEN = 0,
        /// <summary>
        /// 未赠送
        /// </summary>
        NOT_GIVED
    }

    enum ReceiveType
    {
        /// <summary>
        /// 已领取
        /// </summary>
        GOT = 1,
        /// <summary>
        /// 未领取
        /// </summary>
        NOT_GOT = 0,
        /// <summary>
        /// 空
        /// </summary>
        NULL = 2
    }

    Friend friendMain;
    GList friendList;

    GButton sendAllGiftsBtn;
    GButton receiveAllGiftsBtn;

    GTextField friendCountText;
    Controller pageController;

    //好友较多的时候，一次性请求所有数据服务器压力大，因此需要分页加载数据
    /****分页加载后端数据***/
    /// <summary>
    /// 一次加载多少人
    /// </summary>
    const int PAGE_SIZE = 12;
    bool needLoad;  //当前是否需要加载数据
    int curPage;    //当前加载到哪一页数据
    int totalPage;  //一共有多少页数据

    List<FriendInfo> friendInfos
    {
        get
        {
            return FriendDataMgr.Ins.friendInfos;
        }
        set
        {
            FriendDataMgr.Ins.friendInfos = value;
        }
    }


    public override void InitMoudle(GComponent gComponent, int controllerIndex)
    {
        base.InitMoudle(gComponent, controllerIndex);
        InitUI();
        InitEvent();
    }

    public override void InitUI()
    {
        base.InitUI();
        friendCountText = SearchChild("n12").asTextField;
        friendList = SearchChild("n13").asCom.GetChild("n13").asList;
        receiveAllGiftsBtn = SearchChild("n14").asButton;
        sendAllGiftsBtn = SearchChild("n15").asButton;
        pageController = ui.GetController("c1");
    }

    public override void InitEvent()
    {
        base.InitEvent();

        sendAllGiftsBtn.onClick.Set(SendAllGifts);

        receiveAllGiftsBtn.onClick.Set(GetAllGifts);

        friendList.scrollPane.onScroll.Add(OnScroll);
    }

    public override void InitData<D>(D data)
    {
        base.InitData();
        friendMain = data as Friend;
        friendInfos = friendMain.list;

        friendList.SetVirtual();
        friendList.itemRenderer = RenderItem;

        ResetFriendList(friendMain);
       
    }

    void ResetFriendList(Friend friend)  //重置好友列表，并返回第一页数据
    {
        this.friendMain = friend;
        int limit = int.Parse(GameData.SwitchConfigs.Find(a => a.key == SwitchConfig.KEY_FRIEND_LIMIT).description);
        friendCountText.text = "好友数量:" + friendMain.totalFriends + "/" + limit;

        friendInfos = friendMain.list;
        curPage = 1;
        totalPage = friendMain.totalFriends / PAGE_SIZE;
        totalPage += friendMain.totalFriends % PAGE_SIZE == 0 ? 0 : 1;

        if (curPage < totalPage)
        {
            needLoad = true;
        }
        friendList.scrollPane.ScrollTop();
        RefreshFriendList();
    }

    void RefreshFriendList()    //刷新好友列表
    {
        if (friendInfos.Count == 0)
        {
            pageController.selectedIndex = 1;//好友列表为空
        }
        else
        {
            pageController.selectedIndex = 0;
        }
        friendList.numItems = friendInfos.Count;
        SetItemEffect();
    }


    void RenderItem(int index, GObject gObject)
    {
        GComponent gComponent = gObject as GComponent;

        InitItemBasicInfo(friendInfos[index], gComponent);

        gComponent.GetChild("n19").onClick.Set(() =>//查看好友信息
        {
            NormalInfo info = new NormalInfo()
            {
                index = friendInfos[index].id,
                type = 1,
            };
            EventMgr.Ins.DispachEvent(EventConfig.GET_FRIEND_DETAIL, info);

            //在查看信息界面删除好友后，要重置刷新好友列表(排行榜也可以查看删除好友，所以用Replace)
            EventMgr.Ins.ReplaceEvent<int>(EventConfig.REFRESH_AFTER_DELETE, (playerID) =>
            {
                friendMain.list.Remove(friendMain.list.Find(a => a.id == playerID));
                friendMain.totalFriends -= 1;
                ResetFriendList(friendMain);
            });
        });


        Controller sendController = gComponent.GetController("c1");
        sendController.selectedIndex = friendInfos[index].presented;    //0：已赠送；1：未赠送

        GButton sendBtn = gComponent.GetChild("n11").asButton;          //送礼
        sendBtn.onClick.Set(() =>
        {
            SendGift(index);
        });

        Controller receiveController = gComponent.GetController("c2");
        receiveController.selectedIndex = friendInfos[index].presentedMe;//0：未领取；1：已领取；2：空；

        GButton receiveBtn = gComponent.GetChild("n10").asButton;       //收礼
        receiveBtn.onClick.Set(() =>
        {
            GetGift(index);
        });
    }

    private void InitItemBasicInfo(FriendInfo info, GComponent gComponent)
    {
        GTextField levelText = gComponent.GetChild("n3").asTextField;   //等级
        levelText.text = info.level.ToString();

        GTextField nameText = gComponent.GetChild("n4").asTextField;    //昵称
        nameText.text = info.name;

        GTextField tittleText = gComponent.GetChild("n6").asTextField;  //头衔
        tittleText.text = info.title;

        GLoader headIconGLoader = gComponent.GetChild("n1").asLoader;   //头像
        headIconGLoader.url = UrlUtil.GetRoleHeadIconUrl(info.avatar);

        GTextField playerTitleText = gComponent.GetChild("n6").asTextField;
        GLoader titleLoader = gComponent.GetChild("n15").asCom.GetChild("n39").asLoader;
        GGraph titleGraph = gComponent.GetChild("n15").asCom.GetChild("n40").asGraph;
        GLoader frameLoader = gComponent.GetChild("n14").asLoader;
        GGraph frameGraph = gComponent.GetChild("n16").asGraph;
        if (info.title != "0")
        {
            GameTitleConfig titleConfig = JsonConfig.GameTitleConfigs.Find(a => a.id == int.Parse(info.title));
            FXMgr.SetTitle(titleGraph, titleLoader, titleConfig.level, 80, new Vector3(127, 20, 1000));
            playerTitleText.text = titleConfig.name_cn;
        }
        else
        {
            FXMgr.SetTitle(titleGraph, titleLoader, 1, 80, new Vector3(127, 20, 1000));
            playerTitleText.text = "暂无";
        }
        GameAvatarFrameConfig frameConfig = JsonConfig.GameAvatarFrameConfigs.Find(a => a.id == info.frame);
        FriendDataMgr.Ins.SetFrame(frameGraph, frameLoader, frameConfig.source_id);

    }



    private void SendGift(int index)           //给列表中第index个玩家送礼
    {
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("ids", friendInfos[index].id.ToString());
        GameMonoBehaviour.Ins.RequestInfoPost<HolderData>(NetHeaderConfig.FRIEND_GIFT_SEND, wWWForm, (HolderData data) =>
        {
            friendInfos[index].presented = (int)GiveType.GIVEN;
            RefreshFriendList();
        });
    }

    private void SendAllGifts()
    {
        WWWForm wWWForm = new WWWForm();
        StringBuilder text = new StringBuilder();

        for (int i = 0; i < friendInfos.Count; i++)
        {
            if (friendInfos[i].presented == (int)GiveType.NOT_GIVED)
            {
                text.Append(friendInfos[i].id + ",");
            }
        }
        if (text.ToString() == "")
        {
            UIMgr.Ins.showErrorMsgWindow("已赠送全部好友!");
            return;
        }
        wWWForm.AddField("ids", text.ToString());

        GameMonoBehaviour.Ins.RequestInfoPost<HolderData>(NetHeaderConfig.FRIEND_GIFT_SEND, wWWForm, (HolderData giftInfos) =>
        {
            for (int i = 0; i < friendInfos.Count; i++)
            {
                if (friendInfos[i].presented == (int)GiveType.NOT_GIVED)
                {
                    friendInfos[i].presented = (int)GiveType.GIVEN;
                }
            }
            RefreshFriendList();

        });
    }

    private void GetGift(int index)            //收列表中第index个玩家的礼物
    {
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("ids", friendInfos[index].giftId.ToString());
        GameMonoBehaviour.Ins.RequestInfoPost<PropMake>(NetHeaderConfig.FRIEND_GIFT_RECEIVE, wWWForm, (PropMake propMake) =>
        {
            if (propMake.playerProp != null)
            {
                TouchScreenView.Ins.ShowPropsTost(propMake.playerProp);
            }
            friendInfos[index].presentedMe = (int)ReceiveType.GOT;
            RefreshFriendList();
            CheckRedPoint();
        });

    }

    private void GetAllGifts()
    {
        WWWForm wWWForm = new WWWForm();
        StringBuilder text = new StringBuilder();

        for (int i = 0; i < friendInfos.Count; i++)
        {
            if (friendInfos[i].presentedMe == (int)ReceiveType.NOT_GOT)
            {
                text.Append(friendInfos[i].id + ",");
            }
        }
        if (text.ToString() == "")
        {
            UIMgr.Ins.showErrorMsgWindow("已领取全部礼物!");
            return;
        }
        wWWForm.AddField("ids", text.ToString());
        GameMonoBehaviour.Ins.RequestInfoPost<PropMake>(NetHeaderConfig.FRIEND_GIFT_RECEIVE, wWWForm, (PropMake propMake) =>
        {
            if (propMake.playerProp != null)
            {
                TouchScreenView.Ins.ShowPropsTost(propMake.playerProp);
            }
            for (int i = 0; i < friendInfos.Count; i++)
            {
                if (friendInfos[i].presentedMe == (int)ReceiveType.NOT_GOT)
                {
                    friendInfos[i].presentedMe = (int)ReceiveType.GOT;
                }
            }
            RefreshFriendList();

        });
    }

    private void OnScroll()
    {
        if (needLoad)
        {
            int itemIndex = friendList.ChildIndexToItemIndex(1);
            if (itemIndex > (curPage - 1) * PAGE_SIZE + 5)
            {
                LoadFriends();
            }
        }
    }

    void LoadFriends()
    {
        curPage++;
        if (curPage == totalPage)
        {
            needLoad = false;
        }

        FriendDataMgr.Ins.RequestFriendInfos(curPage, PAGE_SIZE, RequestFriendList);

    }

    void RequestFriendList(Friend friend)
    {
        if (friend.list.Count > 0)
        {
            friendInfos.AddRange(friend.list);
            RefreshFriendList();
            Debug.Log("总页数：" + totalPage + "///当前页：" + curPage + "///当前加载了多少人：" + friend.list.Count);
        }
    }

    void SetItemEffect()
    {
        Vector3 pos = new Vector3();
        for (int i = 0; i < friendList.numChildren; i++)
        {
            GObject item = friendList.GetChildAt(i);

            item.alpha = 0;

            pos = GetItemPos(i, item);
            item.SetPosition(pos.x, pos.y + 200, pos.z);
            float time = i * 0.1f;

            item.TweenMoveY(pos.y, (time + 0.1f)).SetEase(EaseType.CubicOut).OnStart(() =>
            {
                item.TweenFade(1, (time + 0.15f)).SetEase(EaseType.QuadOut);
            });
        }
    }

    Vector3 GetItemPos(int index, GObject gObject)
    {
        Vector3 pos = gObject.position;
        if (pos == Vector3.zero)
        {
            pos.y = index * 255;
        }
        return pos;
    }


    void CheckRedPoint()
    {
        bool isRedPoint = false;
        for (int i = 0; i < friendInfos.Count; i++)
        {
            if (friendInfos[i].presentedMe == (int)ReceiveType.NOT_GOT)
            {
                isRedPoint = true;
            }
        }
        RedpointMgr.Ins.friendRedPoint[0] = isRedPoint ? 1 : 0;
        EventMgr.Ins.DispachEvent(EventConfig.FRIEND_TAB_RED_POINT);
    }
}

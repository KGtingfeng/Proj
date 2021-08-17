using FairyGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 查找好友
/// </summary>
public class FriendSearchMoudle : BaseMoudle
{
    List<FriendInfo> friendInfos;   //推荐好友信息
    GList recommendList;            //好友推荐列表
    GComponent searchResultCom;     //搜索结果显示组件

    /// <summary>
    /// 0:查找；1:结果；2：结果为空
    /// </summary>
    Controller pageController;

    GButton searchBtn;
    GTextInput nameInput;

    public override void InitMoudle(GComponent gComponent, int controllerIndex)
    {
        base.InitMoudle(gComponent, controllerIndex);
        InitUI();
        InitEvent();
    }

    public override void InitUI()
    {
        base.InitUI();
        recommendList = SearchChild("n4").asCom.GetChild("n4").asList;
        searchBtn = SearchChild("n7").asButton;
        nameInput = SearchChild("n11").asTextInput;

        searchResultCom = SearchChild("n14").asCom;
        pageController = ui.GetController("c1");
    }

    public override void InitEvent()
    {
        base.InitEvent();

        searchBtn.onClick.Set(() =>     //按照昵称查找好友
        {
            Debug.Log("你查找的好友叫：" + nameInput.text);
            WWWForm wWWForm = new WWWForm();
            wWWForm.AddField("nickName", nameInput.text);
            GameMonoBehaviour.Ins.RequestInfoPost<FriendInfo>(NetHeaderConfig.FRIEND_SEARCH, wWWForm, RequestSearchInfo);
        });
    }
    private void RequestSearchInfo(FriendInfo friendInfos)
    {
        if (friendInfos == null)     //查找结果为空
        {
            pageController.selectedIndex = 2;
        }
        else                        //查找结果展示
        {
            pageController.selectedIndex = 1;

            InitItemBasicInfo(friendInfos, searchResultCom);

            InitAddFriend(searchResultCom, friendInfos);

            searchResultCom.GetChild("n19").onClick.Set(() =>        //好友详情
            {
                NormalInfo info = new NormalInfo()
                {
                    index = friendInfos.id,
                    type = friendInfos.applied ? 1 : 0,
                };
                EventMgr.Ins.DispachEvent(EventConfig.GET_FRIEND_DETAIL, info);
            });
        }
    }

    public override void InitData<D>(D data)
    {
        base.InitData(data);
        friendInfos = data as List<FriendInfo>;
        nameInput.text = "";
        recommendList.SetVirtual();
        recommendList.itemRenderer = RenderItem;
        recommendList.numItems = friendInfos.Count;
        SetItemEffect();
        pageController.selectedIndex = 0;   //默认展示推荐的玩家
    }

    void RenderItem(int index, GObject gObject)
    {
        GComponent gComponent = gObject as GComponent;

        InitItemBasicInfo(friendInfos[index], gComponent);

        InitAddFriend(gComponent, friendInfos[index]);

        gComponent.GetChild("n19").onClick.Set(() =>        //好友详情
        {
            NormalInfo info = new NormalInfo()
            {
                index = friendInfos[index].id,
                type = friendInfos[index].applied ? 1 : 0,
            };
            EventMgr.Ins.DispachEvent(EventConfig.GET_FRIEND_DETAIL, info);
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
        GLoader titleLoader = gComponent.GetChild("n23").asCom.GetChild("n39").asLoader;
        GGraph titleGraph = gComponent.GetChild("n23").asCom.GetChild("n40").asGraph;
        GLoader frameLoader = gComponent.GetChild("n21").asLoader;
        GGraph frameGraph = gComponent.GetChild("n22").asGraph;
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

    private void InitAddFriend(GComponent gComponent, FriendInfo info)
    {
        Controller addController = gComponent.GetController("c1");
        addController.selectedIndex = info.applied ? 1 : 0;
        GButton addBtn = gComponent.GetChild("n13").asButton;

        addBtn.visible = true;

        FriendInfo myFriend = FriendDataMgr.Ins.friendInfos.Find(a => a.id == info.id);

        if (myFriend != null || info.id == GameData.playerId)//如果是自己或者好友、隐藏添加按钮
        {
            addBtn.visible = false;
            return;
        }
        addBtn.onClick.Set(() =>    //申请好友
        {
            WWWForm wWWForm = new WWWForm();
            wWWForm.AddField("ids", info.id.ToString());
            GameMonoBehaviour.Ins.RequestInfoPost<HolderData>(NetHeaderConfig.FRIEND_APLLY, wWWForm, (HolderData data) =>
            {
                addController.selectedIndex = 1;   //已申请
                info.applied = true;
            });
        });
    }

    void SetItemEffect()
    {
        Vector3 pos = new Vector3();
        for (int i = 0; i < recommendList.numChildren; i++)
        {
            GObject item = recommendList.GetChildAt(i);

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


}

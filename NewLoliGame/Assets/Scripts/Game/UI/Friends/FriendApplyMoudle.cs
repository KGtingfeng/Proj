using FairyGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 好友申请
/// </summary>
public class FriendApplyMoudle : BaseMoudle
{
    Friend friendAplly;
    List<FriendInfo> friendInfos;
    GList apllyList;


    const int PAGE_SIZE = 12;
    bool needLoad;
    int curPage;
    int totalPage;


    public override void InitMoudle(GComponent gComponent, int controllerIndex)
    {
        base.InitMoudle(gComponent, controllerIndex);
        InitUI();
        InitEvent();
    }

    public override void InitUI()
    {
        base.InitUI();
        apllyList = SearchChild("n12").asCom.GetChild("n12").asList;
    }

    public override void InitEvent()
    {
        base.InitEvent();

        apllyList.scrollPane.onScroll.Add(() =>
        {
            OnScroll();
        });
    }

    public override void InitData<D>(D data)
    {
        base.InitData(data);
        friendAplly = data as Friend;
        friendInfos = friendAplly.list;

        apllyList.SetVirtual();
        apllyList.itemRenderer = RenderItem;

        ResetFriendList(friendAplly);
    }

    #region RenderItem

    void RenderItem(int index, GObject gObject)
    {
        GComponent gComponent = gObject as GComponent;

        InitItemBasicInfo(friendInfos[index], gComponent);

        gComponent.GetChild("n19").onClick.Set(() =>        //好友详情
        {
            NormalInfo info = new NormalInfo()
            {
                index = friendInfos[index].id,
                type = friendInfos[index].applied ? 1 : 0,

            };
            EventMgr.Ins.DispachEvent(EventConfig.GET_FRIEND_DETAIL, info);
        });

        Controller controller = gComponent.GetController("c1");
        //设置控制器为同意忽略
        controller.selectedIndex = 3;

        GButton agreeBtn = gComponent.GetChild("n16").asButton;
        agreeBtn.onClick.Set(() =>
        {
            WWWForm wWWForm = new WWWForm();
            wWWForm.AddField("ids", friendInfos[index].id.ToString());
            GameMonoBehaviour.Ins.RequestInfoPost<HolderData>(NetHeaderConfig.FRIEND_AGREE, wWWForm, (HolderData info) =>
            {
                friendInfos.Remove(friendInfos[index]);
                apllyList.numItems = friendInfos.Count;
                CheckRedPoint();
            });
        });

        GButton refuseBtn = gComponent.GetChild("n14").asButton;
        refuseBtn.onClick.Set(() =>
        {
            WWWForm wWWForm = new WWWForm();
            wWWForm.AddField("ids", friendInfos[index].id.ToString());
            GameMonoBehaviour.Ins.RequestInfoPost<HolderData>(NetHeaderConfig.FRIEND_IGNORE, wWWForm, (HolderData info) =>
            {
                Debug.Log("拒绝玩家" + friendInfos[index].id + "的申请");
                friendInfos.Remove(friendInfos[index]);
                apllyList.numItems = friendInfos.Count;
                CheckRedPoint();
            });
        });
    }

    private void InitItemBasicInfo(FriendInfo info, GComponent gComponent)
    {
        GTextField levelText = gComponent.GetChild("n3").asTextField;   //等级
        levelText.text = info.level.ToString();

        GTextField nameText = gComponent.GetChild("n24").asTextField;    //昵称
        nameText.text = info.name;

        GTextField tittleText = gComponent.GetChild("n6").asTextField;  //头衔
        tittleText.text = info.title;

        GLoader headIconGLoader = gComponent.GetChild("n1").asLoader;   //头像
        headIconGLoader.url = UrlUtil.GetRoleHeadIconUrl(info.avatar);

        GTextField playerTitleText = gComponent.GetChild("n26").asTextField;
        GLoader titleLoader = gComponent.GetChild("n25").asCom.GetChild("n39").asLoader;
        GGraph titleGraph = gComponent.GetChild("n25").asCom.GetChild("n40").asGraph;
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

    #endregion RenderItem

    #region 分页加载

    void ResetFriendList(Friend friend)
    {
        friendAplly = friend;
        friendInfos = friendAplly.list;
        curPage = 1;
        totalPage = friendAplly.totalFriends / PAGE_SIZE;
        totalPage += friendAplly.totalFriends % PAGE_SIZE == 0 ? 0 : 1;

        if (curPage < totalPage)
        {
            needLoad = true;
        }

        apllyList.scrollPane.ScrollTop();
        apllyList.numItems = friendInfos.Count;
        SetItemEffect();
    }

    private void OnScroll()
    {
        if (needLoad)
        {
            int itemIndex = apllyList.ChildIndexToItemIndex(1);
            if (itemIndex > (curPage - 1) * PAGE_SIZE + 5)
            {
                if (itemIndex > (curPage - 1) * PAGE_SIZE + 5)
                {
                    LoadFriends();
                }
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
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("page", curPage);
        wWWForm.AddField("pagesize", PAGE_SIZE);
        GameMonoBehaviour.Ins.RequestInfoPost<Friend>(NetHeaderConfig.FRIEND_APLLY_LIST, wWWForm, RequestFriendList);
    }

    void RequestFriendList(Friend friend)
    {
        if (friend.list.Count > 0)
        {
            friendInfos.AddRange(friend.list);
            apllyList.numItems = friendInfos.Count;
            Debug.Log("总页数：" + totalPage + "///当前页：" + curPage + "///当前加载了多少人：" + friend.list.Count);
        }
    }

    void SetItemEffect()
    {
        Vector3 pos = new Vector3();
        for (int i = 0; i < apllyList.numChildren; i++)
        {
            GObject item = apllyList.GetChildAt(i);

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
    #endregion 分页加载

    void CheckRedPoint()
    {
        if (friendInfos.Count == 0 || friendInfos == null)
        {
            RedpointMgr.Ins.friendRedPoint[2] = 0;
        }
        else
        {
            RedpointMgr.Ins.friendRedPoint[2] = 1;
        }
        EventMgr.Ins.DispachEvent(EventConfig.FRIEND_TAB_RED_POINT);
    }
}


using System.Collections.Generic;
using System.Linq;
using FairyGUI;
using UnityEngine;

[ViewAttr("Game/UI/Z_Main", "Z_Main", "Frame_changehead")]
public class ChangeHeadView : BaseView
{

    GComponent headGComponent;
    //头像
    GLoader currentIcon;
    GLoader frameLoader;
    GGraph frameGgraph;
    GTextField idNameText;
    GTextField idText;
    GTextField levelText;

    GList btnGList;
    GList roleHeadGList;
    GList headFrameGList;

    //列表下单个头像
    GLoader iconGLoader;
    GComponent iconHead;

    GTextField titleText;
    GLoader titleLoader;
    GGraph titleGraph;
    /// <summary>
    /// 当前已拥有的头像
    /// </summary>
    List<Avatar> ownAvatars
    {
        get { return GameData.Avatars; }
    }

    Player player
    {
        get { return GameData.Player; }
    }
    //点击头像Id
    int selectIconId = 0;
    int currentBtnIndex = 0;
    //头像框id
    int avatarFrameIndex;
    /// <summary>
    /// 临时使用 图片到齐后删除
    /// </summary>
    List<int> tmpHeadIconList = new List<int> { 1, 4, 6, 8, 9, 10, 11, 12, 13, 15, 16, 19, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34 };

    List<GameInitCardsConfig> gameInitCards;
    List<GameMomentConfig> gameMomentConfigs;
    List<int> redpoint;
    //全部头像
    List<Avatar> allAvatars = new List<Avatar>();
    //时刻头像
    List<Avatar> momentAvatars = new List<Avatar>();

    Avatar currentAvatar = null;
    List<Avatar> currentAvatars = new List<Avatar>();

    List<GameAvatarFrameConfig> frameConfigs;
    public override void InitUI()
    {
        base.InitUI();
        headGComponent = SearchChild("n5").asCom;
        currentIcon = headGComponent.GetChild("n16").asLoader;
        frameLoader = headGComponent.GetChild("n18").asLoader;
        frameGgraph = headGComponent.GetChild("n19").asGraph;

        btnGList = SearchChild("n13") as GList;
        roleHeadGList = SearchChild("n15") as GList;

        idText = SearchChild("n11").asTextField;
        levelText = SearchChild("n21").asTextField;
        idNameText = SearchChild("n10").asTextField;
        headFrameGList = SearchChild("n29").asCom.GetChild("n29").asList;
        controller = ui.GetController("c1");
        titleText = SearchChild("n22").asTextField;
        titleLoader = SearchChild("n18").asCom.GetChild("n39").asLoader;
        titleGraph = SearchChild("n18").asCom.GetChild("n40").asGraph;
        InitEvent();
    }


    public override void InitData()
    {
        OnShowAnimation();
        base.InitData();
        idText.text = player.id + "";
        levelText.text = player.level.ToString();
        idNameText.text = player.name;
        frameConfigs = new List<GameAvatarFrameConfig>(JsonConfig.GameAvatarFrameConfigs);
        GameAvatarFrameConfig frameConfig = JsonConfig.GameAvatarFrameConfigs.Find(a => a.id == player.avatar_frame.current);
        FXMgr.SetFrame(frameGgraph, frameLoader, frameConfig.source_id, 100, new Vector3(135.2f, 133.6f, 1000));

        if (player.title.current != 0)
        {
            GameTitleConfig titleConfig = JsonConfig.GameTitleConfigs.Find(a => a.id == player.title.current);
            titleText.text = titleConfig.name_cn;
            FXMgr.SetTitle(titleGraph, titleLoader, titleConfig.level, 80, new Vector3(127f, 20.5f, 1000));
        }
        else
        {
            titleText.text = "暂无";
            FXMgr.SetTitle(titleGraph, titleLoader, 1, 80, new Vector3(127, 40, 1000));
        }

        getAvatarsList();
        currentAvatars = allAvatars;
        initRoleHeadList();
        btnGList.selectedIndex = 0;
        currentBtnIndex = 0;

        btnGList.GetChildAt(1).asCom.GetController("c1").selectedIndex = RedpointMgr.Ins.FrameHaveRedpoint() ? 1 : 0;
        redpoint = RedpointMgr.Ins.GetFrameRedpoint();
    }

    public override void InitEvent()
    {
        //取消
        SearchChild("n30").onClick.Set(() =>
        {
            UIMgr.Ins.showBeforeView<PlayerHeadView, ChangeHeadView>();
            EventMgr.Ins.DispachEvent(EventConfig.GOTO_VIEW_PLAYER_HEAD);
        });
        //确定
        SearchChild("n16").onClick.Set(() =>
        {
            reqChangeAvatarBtn();
        });

        //头像列表顶部按钮切换
        btnGList.onClickItem.Set(() =>
        {
            OnClickBtn();
        });
    }

    void reqChangeAvatarBtn()
    {

        if (btnGList.selectedIndex == 0)
        {
            WWWForm wWWForm = new WWWForm();
            wWWForm.AddField("avatar", selectIconId);
            GameMonoBehaviour.Ins.RequestInfoPost<Player>(NetHeaderConfig.MODIFY_PLAYERINFO, wWWForm, () =>
            {
                initRoleHeadList();
                UIMgr.Ins.showErrorMsgWindow(MsgException.REPLACED_SUCCESSFULLY);
            });
        }
        else
        {
            WWWForm wWWForm = new WWWForm();
            wWWForm.AddField("frameId", frameConfigs[avatarFrameIndex].id);
            GameMonoBehaviour.Ins.RequestInfoPost<AvatarFrame>(NetHeaderConfig.SET_FRAME, wWWForm, (AvatarFrame frame) =>
            {
                player.avatar_frame = frame;
                EventMgr.Ins.DispachEvent(EventConfig.REFRESH_AVATAR_FRAME);
                Refresh();
            });
        }
    }

    void getAvatarsList()
    {
        if (gameInitCards == null || gameInitCards.Count <= 0)
            gameInitCards = JsonConfig.GameInitCardsConfigs;
        if (gameMomentConfigs == null || gameMomentConfigs.Count <= 0)
            gameMomentConfigs = JsonConfig.GameMomentConfigs;

        allAvatars.Clear();
        momentAvatars.Clear();
        Avatar tmpAvatar = null;

        foreach (var card in gameInitCards)
        {
            if (!tmpHeadIconList.Contains(card.card_id))
                continue;
            tmpAvatar = getAvatar(card.type, card.card_id);
            allAvatars.Add(tmpAvatar);
        }

    }

    void sortAvatarList()
    {
        allAvatars.Sort((Avatar a, Avatar b) => { return a.id.CompareTo(b.id); });
        int.TryParse(player.avatar, out int index);
        currentAvatar = allAvatars.Where(a => a.id == index).FirstOrDefault();
        //momentAvatars.Sort((Avatar a, Avatar b) => { return a.id.CompareTo(b.id); });
        if (currentAvatar == null)
            return;
        if (allAvatars.Contains(currentAvatar))
            allAvatars.Remove(currentAvatar);

        allAvatars.Insert(0, currentAvatar);
    }

    /// <summary>
    /// 将配置信息转换为Avatar
    /// </summary>
    /// <returns></returns>
    private Avatar getAvatar(int type, int cardId)
    {
        Avatar tmpAvatar = ownAvatars.Where(a => a.type == type && a.id == cardId).FirstOrDefault();
        if (tmpAvatar != null)
            return tmpAvatar;

        tmpAvatar = new Avatar();
        tmpAvatar.id = cardId;
        tmpAvatar.type = type;
        tmpAvatar.isOwn = 0;
        return tmpAvatar;
    }

    void OnClickBtn()
    {
        if (btnGList.selectedIndex == currentBtnIndex)
        {
            return;
        }
        currentBtnIndex = btnGList.selectedIndex;
        if (btnGList.selectedIndex == 0)
        {
            initRoleHeadList();
        }
        else
        {
            WWWForm wWWForm = new WWWForm();
            GameMonoBehaviour.Ins.RequestInfoPost<AvatarFrame>(NetHeaderConfig.PLAYER_FRAME, wWWForm, (AvatarFrame frame) =>
            {
                player.avatar_frame = frame;
                InitAvatarFrame();
                btnGList.GetChildAt(1).asCom.GetController("c1").selectedIndex = 0;
                RedpointMgr.Ins.ClearFrame();
            });
        }
    }

    /// <summary>
    /// 初始化头像
    /// </summary>
    /// <param name="selectIndex"></param>
    void initRoleHeadList()
    {
        controller.selectedIndex = 0;
        sortAvatarList();
        roleHeadGList.itemRenderer = RenderListItem;
        roleHeadGList.numItems = allAvatars.Count;
    }

    void RenderListItem(int index, GObject obj)
    {
        obj.alpha = 0;
        GButton item = obj.asButton;
        iconHead = item.GetChild("n0").asCom;
        iconGLoader = iconHead.GetChild("n16").asLoader;
        string selectId = currentAvatars[index].id + "";
        iconGLoader.url = UrlUtil.GetRoleHeadIconUrl(selectId);

        item.selected = false;
        if (selectId.Equals(player.avatar))
        {
            selectIconId = currentAvatars[index].id;
            currentIcon.url = UrlUtil.GetRoleHeadIconUrl(selectIconId + "");
            item.selected = true;
        }

        item.onClick.Set(() =>
        {
            selectIconId = currentAvatars[index].id;
            currentIcon.url = UrlUtil.GetRoleHeadIconUrl(selectIconId + "");
            roleHeadGList.selectedIndex = index;

        });
        SetRoleItemEffect(index, obj);
    }

    void SetRoleItemEffect(int index, GObject obj)
    {
        Vector3 pos = new Vector3();
        pos = obj.position;
        if (pos == Vector3.zero)
        {
            pos.x = (index % 4) * 137f;
            pos.y = (index / 4) * 136f;
        }
        obj.SetPosition(pos.x, pos.y + 50, pos.z);
        float time = (index / 4) * 0.2f;
        obj.TweenMoveY(pos.y, (time + 0.3f)).SetEase(EaseType.CubicOut).OnStart(() =>
         {
             obj.TweenFade(1, (time + 0.3f)).SetEase(EaseType.QuadOut);
         });
    }

    #region 头像框
    Dictionary<int, List<GameObject>> framePool = new Dictionary<int, List<GameObject>>();
    Vector3 framePos = new Vector3(59.5f, 60, 1000);

    void InitAvatarFrame()
    {
        SortFrameList();

        headFrameGList.SetVirtual();
        headFrameGList.itemRenderer = RenderFrameItem;
        headFrameGList.numItems = frameConfigs.Count;
        headFrameGList.onClickItem.Set(OnClickItem);
        controller.selectedIndex = 1;
        avatarFrameIndex = 0;
        headFrameGList.RefreshVirtualList();
        headFrameGList.selectedIndex = 0;
    }


    void Refresh()
    {
        SortFrameList();
        GameAvatarFrameConfig frameConfig = JsonConfig.GameAvatarFrameConfigs.Find(a => a.id == player.avatar_frame.current);
        FXMgr.SetFrame(frameGgraph, frameLoader, frameConfig.source_id, 100, new Vector3(135.2f, 133.6f, 1000));

        controller.selectedIndex = 1;
        avatarFrameIndex = 0;
        headFrameGList.RefreshVirtualList();
        headFrameGList.selectedIndex = 0;

    }

    void SortFrameList()
    {

        foreach (int i in player.avatar_frame.own)
        {
            GameAvatarFrameConfig ownFrame = frameConfigs.Find(a => a.id == i);
            if (ownFrame == null)
                continue;
            frameConfigs.Remove(ownFrame);
            frameConfigs.Insert(0, ownFrame);
        }
        if (player.avatar_frame.current != 0)
        {
            GameAvatarFrameConfig frameConfig = frameConfigs.Find(a => a.id == player.avatar_frame.current);
            frameConfigs.Remove(frameConfig);
            frameConfigs.Insert(0, frameConfig);
        }
    }

    void RenderFrameItem(int index, GObject gObject)
    {
        GComponent gCom = gObject.asCom;
        GLoader gLoader = gCom.GetChild("n2").asLoader;
        GGraph gGraph = gCom.GetChild("n4").asGraph;
        Controller frameController = gCom.GetController("c1");
        GLoader iconLoader = gCom.GetChild("n0").asCom.GetChild("n16").asLoader;
        Controller redPointController = gCom.GetController("c2");
        iconLoader.url = UrlUtil.GetRoleHeadIconUrl(player.avatar);

        GameAvatarFrameConfig frameConfig = frameConfigs[index];
        redPointController.selectedIndex = redpoint.Contains(frameConfig.id) ? 1 : 0;
        SetFrame(gGraph, gLoader, frameConfig.source_id);
        frameController.selectedIndex = player.avatar_frame.own.Contains(frameConfig.id) ? 1 : 0;
        //SetRoleItemEffect(index, gObject);

    }

    void SetFrame(GGraph gGraph, GLoader gLoader, int level)
    {
        if (level > 2000)
        {
            gGraph.visible = true;
            gLoader.visible = false;
            if (string.IsNullOrEmpty(gGraph.url))
            {
                gGraph.url = level.ToString();
                GameObject go = GetFxObject(level);
                GoWrapper goWrapper = new GoWrapper();
                goWrapper.SetWrapTarget(go, true);
                gGraph.SetNativeObject(goWrapper);
            }
            else if (gGraph.url != level.ToString())
            {
                GoWrapper wrapper = gGraph.displayObject as GoWrapper;
                RemoveToDic(wrapper.wrapTarget, int.Parse(gGraph.url));
                gGraph.url = level.ToString();
                GameObject go = GetFxObject(level);
                wrapper.SetWrapTarget(go, true);
            }
            gGraph.displayObject.gameObject.transform.localScale = (Vector3.one * 37);
            gGraph.position = framePos;
        }
        else
        {
            gGraph.visible = false;
            gLoader.visible = true;
            gLoader.url = UrlUtil.GetAvatarFrame(level);
        }
    }

    /// <summary>
    /// 获取称号背景，如果称号背景是特效需特殊获得
    /// </summary>
    GameObject GetFxObject(int id)
    {
        GameObject go;
        List<GameObject> wrappers;
        if (framePool.TryGetValue(id, out wrappers))
        {
            if (wrappers.Count > 0)
            {
                go = wrappers[0];
                wrappers.RemoveAt(0);
            }
            else
            {
                go = FXMgr.CreateObject(UrlUtil.GetAvatarFrame(id));
            }
        }
        else
        {
            wrappers = new List<GameObject>();
            framePool.Add(id, wrappers);
            go = FXMgr.CreateObject(UrlUtil.GetAvatarFrame(id));
        }
        go.SetActive(true);
        return go;
    }

    void RemoveToDic(GameObject go, int id)
    {
        go.SetActive(false);
        List<GameObject> gos;
        if (framePool.TryGetValue(id, out gos))
        {
            gos.Add(go);
        }
        else
        {
            gos = new List<GameObject>();
            gos.Add(go);
            framePool.Add(id, gos);
        }
    }


    void OnClickItem(EventContext context)
    {
        //当前可视item中的index
        int index = headFrameGList.GetChildIndex((GObject)context.data);
        //真实index
        int realIndex = headFrameGList.ChildIndexToItemIndex(index);

        if (player.avatar_frame.own.Contains(frameConfigs[realIndex].id))
        {
            FXMgr.SetFrame(frameGgraph, frameLoader, frameConfigs[realIndex].source_id, 100, new Vector3(135.2f, 133.6f, 1000));
            avatarFrameIndex = realIndex;
        }
        else
        {
            PlayerProp playerProp = new PlayerProp();
            playerProp.prop_id = frameConfigs[realIndex].id;
            playerProp.prop_count = 0;
            UIMgr.Ins.showNextPopupView<PropSourcesView, PlayerProp>(playerProp);
            headFrameGList.selectedIndex = avatarFrameIndex;
        }
    }

    #endregion
}

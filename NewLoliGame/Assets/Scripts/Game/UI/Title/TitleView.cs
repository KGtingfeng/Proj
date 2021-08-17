using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

[ViewAttr("Game/UI/T_Common", "T_Common", "Frame_changetitle")]
public class TitleView : BaseView
{
    Player player
    {
        get { return GameData.Player; }
    }

    GLoader iconLoader;
    GTextField nameText;
    GTextField titleText;
    GTextField levelText;
    GTextField idText;

    GList titleList;
    GLoader titleLoader;
    GGraph titleGraph;

    GLoader frameLoader;
    GGraph frameGraph;
    int currentId;

    Dictionary<int, List<GameObject>> titleDic = new Dictionary<int, List<GameObject>>();
    AvatarFrame title;
    List<GameTitleConfig> gameTitleConfigs;
    List<int> redPoints;
    public override void InitUI()
    {
        base.InitUI();
        nameText = SearchChild("n52").asTextField;
        titleText = SearchChild("n53").asTextField;
        levelText = SearchChild("n73").asTextField;
        idText = SearchChild("n75").asTextField;

        titleList = SearchChild("n67").asCom.GetChild("n67").asList;
        iconLoader = SearchChild("n49").asCom.GetChild("n16").asLoader;

        titleLoader = SearchChild("n71").asCom.GetChild("n39").asLoader;
        titleGraph = SearchChild("n71").asCom.GetChild("n40").asGraph;

        frameLoader = SearchChild("n49").asCom.GetChild("n18").asLoader;
        frameGraph = SearchChild("n49").asCom.GetChild("n19").asGraph;

        InitEvent();
    }

    public override void InitEvent()
    {
        base.InitEvent();

        SearchChild("n70").onClick.Set(() =>
        {
            UIMgr.Ins.showBeforeView<PlayerHeadView, TitleView>();
        });
        //卸下
        SearchChild("n90").onClick.Set(OnClickRemove);
        //佩戴
        SearchChild("n91").onClick.Set(OnClickWear);

    }

    public override void InitData<D>(D data)
    {
        OnShowAnimation();
        base.InitData(data);
        title = data as AvatarFrame;
        gameTitleConfigs = JsonConfig.GameTitleConfigs;
        InitInfos();
        redPoints = RedpointMgr.Ins.GetTitleRedpoint();
        RedpointMgr.Ins.ClearTitle();
        titleList.SetVirtual();
        titleList.itemRenderer = RenderItem;
        titleList.numItems = gameTitleConfigs.Count;
        titleList.onClickItem.Set(OnClickItem);
    }

    void InitInfos()
    {
        idText.text = player.id + "";
        levelText.text = player.level.ToString();
        nameText.text = player.name;
        iconLoader.url = UrlUtil.GetRoleHeadIconUrl(player.avatar);
        if (title != null)
        {
            currentId = title.current;
            if (title.current != 0)
            {
                GameTitleConfig titleConfig = gameTitleConfigs.Find(a => a.id == currentId);
                titleText.text = titleConfig.name_cn;
                FXMgr.SetTitle(titleGraph, titleLoader, titleConfig.level, 80, new Vector3(119, 19, 1000));
            }
            else
            {
                titleText.text = "暂无";
                FXMgr.SetTitle(titleGraph, titleLoader, 1, 80, new Vector3(123, 19, 1000));
            }
            SortTitle();
        }
        else
        {
            currentId = 0;
            titleText.text = "暂无";
            FXMgr.SetTitle(titleGraph, titleLoader, 1, 80, new Vector3(123, 19, 1000));
        }
        if (player.avatar_frame.current != 0)
        {
            GameAvatarFrameConfig frameConfig = JsonConfig.GameAvatarFrameConfigs.Find(a => a.id == player.avatar_frame.current);
            FXMgr.SetFrame(frameGraph, frameLoader, frameConfig.source_id, 100, new Vector3(135, 136, 1000));
        }
        else
        {
            frameGraph.visible = false;
            frameLoader.visible = false;
        }
    }

    void SortTitle()
    {
        foreach (int i in title.own)
        {
            GameTitleConfig ownTitle = gameTitleConfigs.Find(a => a.id == i);
            if (ownTitle == null)
                continue;
            gameTitleConfigs.Remove(ownTitle);
            gameTitleConfigs.Insert(0, ownTitle);
        }
        if (title.current != 0)
        {
            GameTitleConfig titleConfig = gameTitleConfigs.Find(a => a.id == title.current);
            gameTitleConfigs.Remove(titleConfig);
            gameTitleConfigs.Insert(0, titleConfig);
        }

    }

    void RenderItem(int index, GObject gObject)
    {
        GComponent gCom = gObject.asCom;
        Controller listController = gCom.GetController("c1");
        Controller redPointController = gCom.GetController("c2");
        GTextField nameText = gCom.GetChild("n91").asTextField;
        GTextField numText = gCom.GetChild("n85").asTextField;
        GLoader attrLoader = gCom.GetChild("n84").asLoader;
        GLoader bgLoader = gCom.GetChild("n89").asLoader;
        GGraph gGraph = gCom.GetChild("n92").asGraph;

        GameTitleConfig titleConfig = gameTitleConfigs[index];
        TinyItem tinyItem = titleConfig.GetAttr();
        nameText.text = titleConfig.name_cn;
        attrLoader.url = tinyItem.url;
        numText.text = "+" + tinyItem.num;
        SetTitle(gGraph, bgLoader, titleConfig.level);
        listController.selectedIndex = 0;
        if (currentId == titleConfig.id)
        {
            listController.selectedIndex = 1;
        }
        else if (title.own.Contains(titleConfig.id))
        {
            listController.selectedIndex = 2;
        }
        redPointController.selectedIndex = redPoints.Contains(titleConfig.id) ? 1 : 0;
        gCom.GetChild("n86").onClick.Set(() =>
        {
            OnClickLook(index);
        });

    }

    void SetTitle(GGraph gGraph, GLoader gLoader, int level)
    {
        if (level > 2)
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
            gGraph.displayObject.gameObject.transform.localScale = (Vector3.one * 80);
            gGraph.position = new Vector3(171, 53, 1000);
        }
        else
        {
            gGraph.visible = false;
            gLoader.visible = true;
            gLoader.url = UrlUtil.GetTitleBg(level);
        }
    }

    /// <summary>
    /// 获取称号背景，如果称号背景是特效需特殊获得
    /// </summary>
    GameObject GetFxObject(int id)
    {
        GameObject go;
        List<GameObject> wrappers;
        if (titleDic.TryGetValue(id, out wrappers))
        {
            if (wrappers.Count > 0)
            {
                go = wrappers[0];
                wrappers.RemoveAt(0);
            }
            else
            {
                go = FXMgr.CreateObject(UrlUtil.GetTitleBg(id));
            }
        }
        else
        {
            wrappers = new List<GameObject>();
            titleDic.Add(id, wrappers);
            go = FXMgr.CreateObject(UrlUtil.GetTitleBg(id));
        }
        go.SetActive(true);
        return go;
    }

    void RemoveToDic(GameObject go, int id)
    {
        go.SetActive(false);
        List<GameObject> gos;
        if (titleDic.TryGetValue(id, out gos))
        {
            gos.Add(go);
        }
        else
        {
            gos = new List<GameObject>();
            gos.Add(go);
            titleDic.Add(id, gos);
        }
    }

    //查看来源
    void OnClickLook(int index)
    {
        GameTitleConfig titleConfig = gameTitleConfigs[index];
        PlayerProp playerProp = new PlayerProp()
        {
            prop_id = titleConfig.id,
            prop_count = 0,
        };
        UIMgr.Ins.showNextPopupView<PropSourcesView, PlayerProp>(playerProp);
    }

    void OnClickRemove()
    {
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("titleId", 0);

        GameMonoBehaviour.Ins.RequestInfoPost<AvatarFrame>(NetHeaderConfig.SET_TITLE, wWWForm, Request);
    }

    void OnClickWear()
    {
        if (currentId == title.current)
        {
            UIMgr.Ins.showErrorMsgWindow("当前已佩戴该称号!");
            return;
        }
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("titleId", currentId);
        GameMonoBehaviour.Ins.RequestInfoPost<AvatarFrame>(NetHeaderConfig.SET_TITLE, wWWForm, Request);
    }

    void OnClickItem(EventContext context)
    {
        //当前可视item中的index
        int index = titleList.GetChildIndex((GObject)context.data);
        //真实index
        int realIndex = titleList.ChildIndexToItemIndex(index);
        if (title.own.Contains(gameTitleConfigs[realIndex].id))
        {
            currentId = gameTitleConfigs[realIndex].id;
            titleList.RefreshVirtualList();
        }
    }

    void Request(AvatarFrame title)
    {
        List<TinyItem> tinyItems = new List<TinyItem>();
        if (title.playerAttr.charm != player.attribute.charm)
        {
            tinyItems.Add(new TinyItem("魅力", CommonUrlConfig.GetCharmUrl(), title.playerAttr.charm, player.attribute.charm));
            player.attribute.charm = title.playerAttr.charm;
        }
        if (title.playerAttr.intell != player.attribute.intell)
        {
            tinyItems.Add(new TinyItem("智慧", CommonUrlConfig.GetWisdomUrl(), title.playerAttr.intell, player.attribute.intell));
            player.attribute.intell = title.playerAttr.intell;
        }
        if (title.playerAttr.evn != player.attribute.evn)
        {
            tinyItems.Add(new TinyItem("环保", CommonUrlConfig.GetEnvUrl(), title.playerAttr.evn, player.attribute.evn));
            player.attribute.evn = title.playerAttr.evn;
        }
        if (title.playerAttr.mana != player.attribute.mana)
        {
            tinyItems.Add(new TinyItem("魔法", CommonUrlConfig.GetMagicUrl(), title.playerAttr.mana, player.attribute.mana));
            player.attribute.mana = title.playerAttr.mana;
        }
        tinyItems[0].voiceId = 0;
        player.title.current = title.current;
        player.title.own = title.own;

        this.title = player.title;
        UIMgr.Ins.showWindow<RoleUpgradeSuccessWindow, List<TinyItem>>(tinyItems);
        InitInfos();
        titleList.RefreshVirtualList();
        EventMgr.Ins.DispachEvent(EventConfig.REFRESH_PLAYER_TITLE);
    }
}

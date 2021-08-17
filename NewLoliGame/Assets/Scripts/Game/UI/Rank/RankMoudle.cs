using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

public class RankMoudle : BaseMoudle
{

    GList list;
    GTextField mineNameText;
    GTextField mineTitleText;
    GTextField mineRankText;
    GTextField mineNumText;
    GLoader mineFrameLoader;
    GGraph mineFrameGraph;
    GLoader mineTitleLoader;
    GGraph mineTitleGraph;

    GLoader iconLoader;
    GTextField titleText;
    Controller mainController;

    RankSingle rankSingle;
    NormalInfo normalInfo;
    string rankName;
    List<PlayerRankInfo> rankInfos;
    public override void InitMoudle(GComponent gComponent, int controllerIndex)
    {
        base.InitMoudle(gComponent, controllerIndex);
        InitUI();
        InitEvent();
    }

    public override void InitUI()
    {
        base.InitUI();
        list = SearchChild("n56").asCom.GetChild("n56").asList;
        GComponent mineCom = SearchChild("n61").asCom;
        mineNameText = mineCom.GetChild("n53").asTextField;
        mineTitleText = mineCom.asCom.GetChild("n54").asTextField;
        mineRankText = mineCom.asCom.GetChild("n51").asTextField;
        mineNumText = mineCom.asCom.GetChild("n55").asTextField;
        mainController = mineCom.GetController("c1");
        iconLoader = mineCom.asCom.GetChild("n61").asCom.GetChild("n0").asLoader;
        mineFrameLoader = mineCom.asCom.GetChild("n61").asCom.GetChild("n1").asLoader;
        mineFrameGraph = mineCom.asCom.GetChild("n61").asCom.GetChild("n2").asGraph;
        mineTitleLoader = mineCom.asCom.GetChild("n62").asCom.GetChild("n39").asLoader;
        mineTitleGraph = mineCom.asCom.GetChild("n62").asCom.GetChild("n40").asGraph;

        titleText = SearchChild("n8").asTextField;
    }


    public override void InitEvent()
    {
        base.InitEvent();

    }

    public override void InitData<D>(D data)
    {
        base.InitData(data);
        normalInfo = data as NormalInfo;

        string prefix = "";
        if (normalInfo.index == (int)RankType.Favor || normalInfo.index == (int)RankType.Time)
        {
            if (normalInfo.type == 0)
            {
                prefix = "角色总";
            }
            else
            {
                GameInitCardsConfig cardsConfig = JsonConfig.GameInitCardsConfigs.Find(a => a.card_id == normalInfo.type);
                prefix = cardsConfig.name_cn;
            }
        }

        rankSingle = RankDataMgr.Instance.RankSingle;
        rankInfos = new List<PlayerRankInfo>(rankSingle.list);
        rankName = RankDataMgr.Instance.typeName[normalInfo.index] + ":";
        SetTopThree();
        titleText.text = prefix + RankDataMgr.Instance.typeName[normalInfo.index] + "排行";

        RefershMine();
        list.SetVirtual();
        list.itemRenderer = RenderItem;
        SetItemEffect();
        if (rankInfos.Count < 47)
        {
            list.numItems = rankInfos.Count + 1;
        }
        else
        {
            list.numItems = rankInfos.Count;
        }

    }

    void SetTopThree()
    {
        List<PlayerRankInfo> top = new List<PlayerRankInfo>();
        for (int i = 0; i < 3; i++)
        {
            if (rankInfos.Count > 0)
            {
                top.Add(rankInfos[0]);
                rankInfos.RemoveAt(0);
            }
        }
        RankTopThree topThree = new RankTopThree
        {
            top_three = top,
            type = (RankType)normalInfo.index,
        };
        EventMgr.Ins.DispachEvent(EventConfig.SET_TOP_THREE, topThree);

    }

    void RenderItem(int index, GObject gObject)
    {
        GComponent gCom = gObject.asCom;
        Controller controller = gCom.GetController("c1");

        if (index < rankInfos.Count)
        {
            GComponent playerCom = gCom.GetChild("n0").asCom;
            GTextField nameText = playerCom.GetChild("n53").asTextField;
            GTextField playerTitleText = playerCom.GetChild("n54").asTextField;
            GTextField rankText = playerCom.GetChild("n51").asTextField;
            GTextField numText = playerCom.GetChild("n55").asTextField;
            GComponent iconGcom = playerCom.GetChild("n52").asCom;
            GLoader playerIconLoader = iconGcom.GetChild("n0").asLoader;
            GLoader frameLoader = iconGcom.GetChild("n1").asLoader;
            GGraph frameGraph = iconGcom.GetChild("n2").asGraph;
            PlayerRankInfo rankInfo = rankInfos[index];
            GLoader titleLoader = playerCom.GetChild("n57").asCom.GetChild("n39").asLoader;
            GGraph titleGraph = playerCom.GetChild("n57").asCom.GetChild("n40").asGraph;

            GameAvatarFrameConfig frameConfig = JsonConfig.GameAvatarFrameConfigs.Find(a => a.id == rankInfo.frame);
            SetFrame(frameGraph, frameLoader, frameConfig.source_id);


            controller.selectedIndex = 0;

            nameText.text = rankInfo.name;
            if (rankInfo.title != "0")
            {
                GameTitleConfig titleConfig = JsonConfig.GameTitleConfigs.Find(a => a.id == int.Parse(rankInfo.title));
                FXMgr.SetTitle(titleGraph, titleLoader, titleConfig.level, 80, new Vector3(127, 20, 1000));
                playerTitleText.text = titleConfig.name_cn;
            }
            else
            {
                FXMgr.SetTitle(titleGraph, titleLoader, 1, 80, new Vector3(127, 20, 1000));
                playerTitleText.text = "暂无";
            }
            rankText.text = index + 4 + "";
            numText.text = rankName + rankInfo.val;
            playerIconLoader.url = UrlUtil.GetRoleHeadIconUrl(rankInfo.avatar);
            playerCom.onClick.Set(() =>
            {
                OnClick(index);
            });
        }
        else
        {
            controller.selectedIndex = 1;
        }

    }

    Dictionary<int, List<GameObject>> framePool = new Dictionary<int, List<GameObject>>();
    Vector3 framePos = new Vector3(135f, 134, 1000);

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
            gGraph.displayObject.gameObject.transform.localScale = (Vector3.one * 100);
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

    void RefershMine()
    {
        PlayerRankInfo player = rankSingle.list.Find(a => a.id == rankSingle.player.id);
        //是否在排行里
        if (player == null)
        {
            mainController.selectedIndex = 1;
            mineNumText.text = rankName + rankSingle.player.val;
        }
        else
        {
            mainController.selectedIndex = 0;
            mineRankText.text = rankSingle.list.IndexOf(player) + 1 + "";
            mineNumText.text = rankName + player.val;
        }

        mineNameText.text = rankSingle.player.name;
        if (rankSingle.player.title != "0")
        {
            GameTitleConfig titleConfig = JsonConfig.GameTitleConfigs.Find(a => a.id == int.Parse(rankSingle.player.title));
            FXMgr.SetTitle(mineTitleGraph, mineTitleLoader, titleConfig.level, 80, new Vector3(127, 20, 1000));
            mineTitleText.text = titleConfig.name_cn;
        }
        else
        {
            FXMgr.SetTitle(mineTitleGraph, mineTitleLoader, 1, 80, new Vector3(127, 20, 1000));
            mineTitleText.text = "暂无";
        }
        iconLoader.url = UrlUtil.GetRoleHeadIconUrl(rankSingle.player.avatar);

        GameAvatarFrameConfig frameConfig = JsonConfig.GameAvatarFrameConfigs.Find(a => a.id == rankSingle.player.frame);
        FXMgr.SetFrame(mineFrameGraph, mineFrameLoader, frameConfig.source_id, 100, new Vector3(135, 134, 1000));

    }


    void OnClick(int index)
    {

        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("type", rankInfos[index].id);
        GameMonoBehaviour.Ins.RequestInfoPost<PlayerInfo>(NetHeaderConfig.RANK_PLAYER_ID, wWWForm, (PlayerInfo rankInfo) =>
        {
            rankInfo.player.isApply = rankInfos[index].applied ? 1 : 0;
            rankInfo.player.playerId = rankInfos[index].id;
            UIMgr.Ins.showNextPopupView<PlayerInfoView, PlayerInfo>(rankInfo);
        });
    }

    void SetItemEffect()
    {
        Vector3 pos = new Vector3();
        for (int i = 0; i < list.numChildren; i++)
        {
            GObject item = list.GetChildAt(i);

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

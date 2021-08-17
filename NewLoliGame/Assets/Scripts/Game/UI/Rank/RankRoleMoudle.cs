using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

public class RankRoleMoudle : BaseMoudle
{

    GList roleList;
    GTextField allTileText;
    GTextField titleText;

    NormalInfo info;
    string rankName;
    List<PlayerRankInfo> rankInfos;
    List<GameInitCardsConfig> roleConfig = new List<GameInitCardsConfig>();

    public override void InitMoudle(GComponent gComponent, int controllerIndex)
    {
        base.InitMoudle(gComponent, controllerIndex);
        InitUI();
        InitEvent();
    }

    public override void InitUI()
    {
        base.InitUI();
        roleList = SearchChild("n48").asList;
        allTileText = SearchChild("n34").asTextField;
        titleText = SearchChild("n8").asTextField;
        GetHaveIconRole();
    }

    public override void InitEvent()
    {
        base.InitEvent();

    }

    public override void InitData<D>(D data)
    {
        base.InitData(data);
        info = data as NormalInfo;
        SearchChild("n35").onClick.Set(() =>
        {
            OnClickLook(0);
        });
        if (info.index == (int)RankType.Favor)
        {
            allTileText.text = "角色总好感排行榜";
            titleText.text = "角色好感排行";
            if (RankDataMgr.Instance.FavorRankAll == null)
            {
                WWWForm wWWForm = new WWWForm();
                GameMonoBehaviour.Ins.RequestInfoPost<RankAll>(NetHeaderConfig.RANK_FAVOR_ALL, wWWForm, (RankAll rankInfo) =>
                {
                    RankDataMgr.Instance.FavorRankAll = rankInfo;
                    Refresh();
                });
            }
            else
            {
                Refresh();
            }
        }
        else
        {
            allTileText.text = "角色总时刻排行榜";
            titleText.text = "角色时刻排行";
            if (RankDataMgr.Instance.TimeRankAll == null)
            {
                WWWForm wWWForm = new WWWForm();
                GameMonoBehaviour.Ins.RequestInfoPost<RankAll>(NetHeaderConfig.RANK_CARD_ALL, wWWForm, (RankAll rankInfo) =>
                {
                    RankDataMgr.Instance.TimeRankAll = rankInfo;
                    Refresh();
                });
            }
            else
            {
                Refresh();
            }
        }
    }

    void Refresh()
    {
        RankAll rankAll = info.index == (int)RankType.Favor ? RankDataMgr.Instance.FavorRankAll : RankDataMgr.Instance.TimeRankAll;
        RankTopThree rankTopThree = new RankTopThree
        {
            top_three = rankAll.top_three,
            type = (RankType)info.index,
        };
        EventMgr.Ins.DispachEvent(EventConfig.SET_TOP_THREE, rankTopThree);
        rankInfos = rankAll.list;
        rankName = RankDataMgr.Instance.typeName[info.index];

        rankInfos.Sort(delegate (PlayerRankInfo a, PlayerRankInfo b)
        {
            return int.Parse(a.attr).CompareTo(int.Parse(b.attr));
        });
        roleList.SetVirtual();
        roleList.itemRenderer = RenderItem;
        roleList.numItems = roleConfig.Count;
        SetItemEffect();
    }


    void RenderItem(int index, GObject gObject)
    {
        GComponent gComponent = gObject.asCom;
        GTextField roleText = gComponent.GetChild("n41").asTextField;
        GTextField nameText = gComponent.GetChild("n45").asTextField;
        GTextField titleText = gComponent.GetChild("n46").asTextField;
        GLoader iconLoader = gComponent.GetChild("n37").asCom.GetChild("n37").asCom.GetChild("n0").asLoader;
        GLoader titleLoader = gComponent.GetChild("n48").asCom.GetChild("n39").asLoader;
        GGraph titleGraph = gComponent.GetChild("n48").asCom.GetChild("n40").asGraph;

        roleText.text = roleConfig[index].name_cn + rankName + "排行榜";
        iconLoader.url = UrlUtil.GetStoryHeadIconUrl(roleConfig[index].card_id);
        PlayerRankInfo rankInfo = rankInfos.Find(a => a.attr == roleConfig[index].card_id.ToString());
        if (rankInfo == null || rankInfo != null && string.IsNullOrEmpty(rankInfo.name))
        {
            nameText.text = "暂无";
            titleText.text = "暂无";
            FXMgr.SetTitle(titleGraph, titleLoader, 1, 80, new Vector3(127, 16, 1000));

        }
        else
        {
            nameText.text = rankInfo.name;
            if (rankInfo.title != "0")
            {
                GameTitleConfig titleConfig = JsonConfig.GameTitleConfigs.Find(a => a.id == int.Parse(rankInfo.title));
                FXMgr.SetTitle(titleGraph, titleLoader, titleConfig.level, 80, new Vector3(127, 16, 1000));
                titleText.text = titleConfig.name_cn;
            }
            else
            {
                FXMgr.SetTitle(titleGraph, titleLoader, 1, 80, new Vector3(127, 16, 1000));
                titleText.text = "暂无";
            }
        }

        gComponent.GetChild("n47").onClick.Set(() =>
        {
            OnClickLook(roleConfig[index].card_id);
        });
    }

    void OnClickLook(int type)
    {
        string key = type == 0 ? "Main" : type.ToString();
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("type", key);
        string url = info.index == (int)RankType.Favor ? NetHeaderConfig.RANK_FAVOR_ID : NetHeaderConfig.RANK_CARD_ID;
        GameMonoBehaviour.Ins.RequestInfoPost(url, wWWForm, (RankSingle rankSingle) =>
        {
            RankDataMgr.Instance.RankSingle = rankSingle;
            RankDataMgr.Instance.RankType = (RankType)info.index;

            NormalInfo normalInfo = new NormalInfo
            {
                index = info.index,
                type = type,
            };
            baseView.GoToMoudle<RankMoudle, NormalInfo>((int)RankView.MoudleType.Rank, normalInfo);
        });
    }

    void GetHaveIconRole()
    {
        foreach (var i in JsonConfig.GameInitCardsConfigs)
        {
            if(i.type!=0 && i.type != 1)
            roleConfig.Add(i);

        }
    }

    void SetItemEffect()
    {
        Vector3 pos = new Vector3();
        for (int i = 0; i < roleList.numChildren; i++)
        {
            GObject item = roleList.GetChildAt(i);

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

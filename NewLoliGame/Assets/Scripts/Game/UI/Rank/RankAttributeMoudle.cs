using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

public class RankAttributeMoudle : BaseMoudle
{

    GTextField allTileText;
    RankAll rankAll;

    GTextField titleText;
    readonly List<string> attrList = new List<string>
    {
        "Charm",
        "Intell",
        "Evn",
        "Mana",
    };

    List<GComponent> attrComList;

    public override void InitMoudle(GComponent gComponent, int controllerIndex)
    {
        base.InitMoudle(gComponent, controllerIndex);
        InitUI();
        InitEvent();
    }

    public override void InitUI()
    {
        base.InitUI();
        allTileText = SearchChild("n34").asTextField;
        GList attributeList = SearchChild("n67").asList;
        titleText = SearchChild("n8").asTextField;
        attrComList = new List<GComponent>();
        for (int i = 0; i < attributeList._children.Count; i++)
        {
            GComponent gCom = attributeList.GetChildAt(i).asCom;
            attrComList.Add(gCom);
        }
    }



    public override void InitEvent()
    {
        base.InitEvent();

    }


    public override void InitData()
    {
        base.InitData();
        allTileText.text = "角色总属性排行榜";
        titleText.text = "角色属性排行";
        SearchChild("n35").onClick.Set(() => { OnClickLook(0); });

        if (RankDataMgr.Instance.AttrRankAll == null)
        {
            WWWForm wWWForm = new WWWForm();
            GameMonoBehaviour.Ins.RequestInfoPost<RankAll>(NetHeaderConfig.RANK_ATTR_ALL, wWWForm, (RankAll rankInfo) =>
            {
                RankDataMgr.Instance.AttrRankAll = rankInfo;
                Refresh();
            });
        }
        else
        {
            Refresh();
        }

    }


    void Refresh()
    {
        rankAll = RankDataMgr.Instance.AttrRankAll;
        RankTopThree rankTopThree = new RankTopThree
        {
            top_three = rankAll.top_three,
            type = RankType.Attr,
        };
        EventMgr.Ins.DispachEvent(EventConfig.SET_TOP_THREE, rankTopThree);
        RenderItem();
    }

    void RenderItem()
    {
        for (int i = 0; i < attrList.Count; i++)
        {
            PlayerRankInfo rankInfo = rankAll.list.Find(a => a.attr == attrList[i]);
            GComponent gComponent = attrComList[i];
            GTextField nameText = gComponent.GetChild("n45").asTextField;
            GTextField playerTitleText = gComponent.GetChild("n46").asTextField;
            GLoader titleLoader = gComponent.GetChild("n51").asCom.GetChild("n39").asLoader;
            GGraph titleGraph = gComponent.GetChild("n51").asCom.GetChild("n40").asGraph;
            int index = i;
            gComponent.GetChild("n47").onClick.Set(() =>
            {
                OnClickLook(index + 1);
            });
            nameText.text = rankInfo.name;
            if (rankInfo.title != "0")
            {
                GameTitleConfig titleConfig = JsonConfig.GameTitleConfigs.Find(a => a.id == int.Parse(rankInfo.title));
                FXMgr.SetTitle(titleGraph, titleLoader, titleConfig.level, 80, new Vector3(127, 16, 1000));
                playerTitleText.text = titleConfig.name_cn;
            }
            else
            {
                FXMgr.SetTitle(titleGraph, titleLoader, 1, 80, new Vector3(127, 16, 1000));
                playerTitleText.text = "暂无";
            }
        }
    }

    void OnClickLook(int index)
    {
        WWWForm wWWForm = new WWWForm();

        switch (index)
        {
            case 1:
                wWWForm.AddField("type", "Charm");
                break;
            case 2:
                wWWForm.AddField("type", "Intell");
                break;
            case 3:
                wWWForm.AddField("type", "Evn");
                break;
            case 4:
                wWWForm.AddField("type", "Mana");
                break;
            case 0:
                wWWForm.AddField("type", "Main");
                break;
        }

        GameMonoBehaviour.Ins.RequestInfoPost(NetHeaderConfig.RANK_ATTR_TYPE, wWWForm, (RankSingle rankSingle) =>
        {
            RankDataMgr.Instance.RankSingle = rankSingle;
            RankDataMgr.Instance.RankType = RankType.Attr;

            NormalInfo info = new NormalInfo
            {
                index = index + 2,
            };
            baseView.GoToMoudle<RankMoudle, NormalInfo>((int)RankView.MoudleType.Rank, info);
        });
    }

}

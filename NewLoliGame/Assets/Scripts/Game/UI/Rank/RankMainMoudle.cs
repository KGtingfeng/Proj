using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using System;

public class RankMainMoudle : BaseMoudle
{

    GList rankList;
    GGraph topThreeGraph;

    public override void InitMoudle(GComponent gComponent, int controllerIndex)
    {
        base.InitMoudle(gComponent, controllerIndex);
        InitUI();
        InitEvent();
    }

    public override void InitUI()
    {
        base.InitUI();
        rankList = SearchChild("n7").asList;
        topThreeGraph = SearchChild("n68").asGraph;
        FXMgr.CreateEffectWithGGraph(topThreeGraph, new Vector3(388, 429), "UI_rank");
    }

    public override void InitEvent()
    {
        base.InitEvent();
        rankList.GetChildAt(0).onClick.Set(OnClickRoleRank);

        rankList.GetChildAt(1).onClick.Set(OnClickTimeRank);

        rankList.GetChildAt(2).onClick.Set(OnClickAttributeRank);
    }
    public override void InitData<D>(D data)
    {
        base.InitData(data);
        NormalInfo normalInfo = data as NormalInfo;
        switch (normalInfo.index)
        {
            case 0:
                OnClickRoleRank();
                break;
            case 1:
                OnClickTimeRank();
                break;
            case 2:
                OnClickAttributeRank();
                break;
        }
    }

    //好感排行
    void OnClickRoleRank()
    {
        WWWForm wWWForm = new WWWForm();
        GameMonoBehaviour.Ins.RequestInfoPost<RankAll>(NetHeaderConfig.RANK_FAVOR_ALL, wWWForm, (RankAll rankInfo) =>
        {
            RankDataMgr.Instance.FavorRankAll = rankInfo;
            NormalInfo normalInfo = new NormalInfo
            {
                index = (int)RankType.Favor,
            };
            Action action = () =>
            {
                baseView.GoToMoudle<RankRoleMoudle, NormalInfo>((int)RankView.MoudleType.Role, normalInfo);
            };
            baseView.StartCoroutine(GotoTmpEffectView(action));

        });
    }
    //时刻排行
    void OnClickTimeRank()
    {
        WWWForm wWWForm = new WWWForm();
        GameMonoBehaviour.Ins.RequestInfoPost<RankAll>(NetHeaderConfig.RANK_CARD_ALL, wWWForm, (RankAll rankInfo) =>
        {
            RankDataMgr.Instance.TimeRankAll = rankInfo;
            NormalInfo normalInfo = new NormalInfo
            {
                index = (int)RankType.Time,
            };
            Action action = () =>
            {
                baseView.GoToMoudle<RankRoleMoudle, NormalInfo>((int)RankView.MoudleType.Role, normalInfo);

            };
            baseView.StartCoroutine(GotoTmpEffectView(action));

        });
    }
    //属性排行
    void OnClickAttributeRank()
    {
        WWWForm wWWForm = new WWWForm();
        GameMonoBehaviour.Ins.RequestInfoPost<RankAll>(NetHeaderConfig.RANK_ATTR_ALL, wWWForm, (RankAll rankInfo) =>
        {
            RankDataMgr.Instance.AttrRankAll = rankInfo;
            Action action = () =>
            {
                baseView.GoToMoudle<RankAttributeMoudle>((int)RankView.MoudleType.Attribute);
            };
            baseView.StartCoroutine(GotoTmpEffectView(action));

        });

    }

    IEnumerator GotoTmpEffectView(Action action)
    {
        TouchScreenView.Ins.PlayTmpEffect();
        yield return new WaitForSeconds(0.8f);
        action();
    }
}

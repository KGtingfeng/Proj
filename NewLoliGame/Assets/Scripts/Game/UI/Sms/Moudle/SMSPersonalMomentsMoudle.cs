using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using FairyGUI;

public class SMSPersonalMomentsMoudle : BaseMoudle
{
    List<PlayerTimeline> moments;

    GList momentList;
    GButton btnMore;
    GTextField nameText;
    public override void InitMoudle(GComponent gComponent, int controllerIndex)
    {
        base.InitMoudle(gComponent, controllerIndex);
        InitUI();
        InitEvent();
    }

    public override void InitUI()
    {
        base.InitUI();
        momentList = SearchChild("n80").asList;
        btnMore = SearchChild("n21").asButton;
        btnMore.GetController("c1").selectedIndex = 0;
        nameText = SearchChild("n19").asTextField;
        InitEvent();
    }

    public override void InitEvent()
    {
        base.InitEvent();
        momentList.scrollPane.onPullDownRelease.Set(() =>
        {
            momentList.RefreshVirtualList();
        });
    }

    public override void InitData<D>(D data)
    {
        base.InitData(data);
        moments = data as List<PlayerTimeline>;
        btnMore.onClick.Set(OnClickMore);
        if (SMSDataMgr.Ins.ComeForm == (int)SMSView.MoudleType.TYPE_MORE_MOMENTS)
            btnMore.visible = false;
        else
            btnMore.visible = true;
        GameCellTimelineConfig gameCell = DataUtil.GetGameCellTimelineConfig(moments[0].id);
        if (gameCell.actor_id != 0)
            nameText.text = GameTool.GetActorName(gameCell.actor_id);
        else
            nameText.text = "我";
        EventMgr.Ins.ReplaceEvent<GameTimelineNodeConfig>(EventConfig.MOMENTS_REPLY, Reply);
        RefreshList();
        momentList.scrollPane.ScrollTop();
    }


    void RefreshList()
    {
        momentList.SetVirtual();
        momentList.itemRenderer = RendererItem;
        momentList.numItems = moments.Count;
        momentList.ScrollToView(0);
        GameTool.SetListEffectOne(momentList);

    }

    void RendererItem(int index, GObject gObject)
    {
        GComponent gComponent = gObject.asCom;
        GTextField nameText = gComponent.GetChild("n12").asTextField;
        GComponent favorCom = gComponent.GetChild("n4").asCom;
        GImage favorImage = gComponent.GetChild("n3").asImage;
        GImage gImage = favorCom.GetChild("n23").asCom.GetChild("n23").asImage;
        GTextField favorText = favorCom.GetChild("n21").asTextField;
        GLoader iconLoader = gComponent.GetChild("n5").asCom.GetChild("n5").asCom.GetChild("n5").asLoader;
        Controller controller = gComponent.GetController("c1");
        int controllerIndex = 0;
        string[] moment = moments[index].nodes.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        GameTimelineNodeConfig nodeConfig = DataUtil.GetGameTimelineNodeConfig(int.Parse(moment[0]));
        GameTimelinePointConfig pointConfig = DataUtil.GetGameTimelinePointConfig(nodeConfig.point_id);
        GameCellTimelineConfig gameCell = DataUtil.GetGameCellTimelineConfig(nodeConfig.sms_id);
        iconLoader.url = UrlUtil.GetStoryHeadIconUrl(gameCell.actor_id);
        //我发的不显示好感
        if (gameCell.actor_id == 0)
        {
            nameText.text = GameData.Player.name;
            favorCom.visible = false;
            favorImage.visible = false;
        }
        else
        {
            favorCom.visible = true;
            favorImage.visible = true;
            int level;
            Role actorNick = GameData.OwnRoleList.Find(a => a.id == gameCell.actor_id);
            if (actorNick == null)
            {
                GameInitCardsConfig gameInitCards = JsonConfig.GameInitCardsConfigs.Find(a => a.card_id == gameCell.actor_id);
                nameText.text = gameInitCards.name_cn;
                level = 1;
            }
            else
            {
                nameText.text = actorNick.name;
                level = GameTool.FavorLevel(actorNick.actorFavorite);
            }
            favorText.text = level.ToString();
            GameTool.SetLevelProgressBar(gImage, level);
        }

        GComponent gcom = gComponent.GetChild("n14").asCom;
        GLoader loaderCom = gcom.GetChild("n3").asLoader;
        GTextField content = gcom.GetChild("n2").asTextField;
        GTextField replyText = gComponent.GetChild("n11").asTextField;
        GGraph textBg = gComponent.GetChild("n10").asGraph;
        GButton reply = gComponent.GetChild("n9").asButton;
        reply.onClick.Set(() => { OnClickReply(pointConfig); });

        string str = DataUtil.ReplaceCharacterWithStarts(pointConfig.content1);
        str = GameTool.Conversion(str);
        content.text = str;
        if (pointConfig.card_id != 0)
        {
            gcom.GetController("c1").selectedIndex = 1;
            loaderCom.url = UrlUtil.GetSmsImageURL(pointConfig.card_id);
        }
        else
            gcom.GetController("c1").selectedIndex = 0;
        if (moment.Length > 1)
        {
            controllerIndex = 1;
            textBg.visible = true;
        }
        else
        {
            if (gameCell.actor_id == 0)
                controllerIndex = 1;
            textBg.visible = false;
        }
        replyText.text = SMSDataMgr.GetReply(moment, gameCell);
        controller.selectedIndex = controllerIndex;
    }

    /// <summary>
    /// 点击回复
    /// </summary>
    void OnClickReply(GameTimelinePointConfig pointConfig)
    {
        MomentsInfo info = new MomentsInfo();
        info.type = MomentsInfo.TYPE_REPLY;
        info.PointConfig = pointConfig;
        UIMgr.Ins.showNextPopupView<SMSMomentsReplyView, MomentsInfo>(info);
    }

    /// <summary>
    /// 回复
    /// </summary>
    void Reply(GameTimelineNodeConfig nodeConfig)
    {
        GameCellTimelineConfig gameCell = DataUtil.GetGameCellTimelineConfig(nodeConfig.sms_id);
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("timelineId", gameCell.id);
        wWWForm.AddField("nodeId", nodeConfig.id);
        GameMonoBehaviour.Ins.RequestInfoPost<PlayerTimeline>(NetHeaderConfig.CELL_SET_MOMENT, wWWForm, RequestReply);
        RefreshList();
    }

    void RequestReply(PlayerTimeline timeline)
    {
        AwardInfo info = new AwardInfo();
        info.award = timeline.awards;
        info.extra = timeline.extra;
        GameTool.GetAwards(info);
        baseView.StartCoroutine(SetTimeShow(timeline));
    }

    IEnumerator SetTimeShow(PlayerTimeline timeline)
    {
        PlayerTimeline sms = new PlayerTimeline();
        sms.id = timeline.timeline_id;
        sms.timeline_id = timeline.timeline_id;
        string[] nodes = timeline.nodes.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        StringBuilder text = new StringBuilder();
        text.Append(nodes[0] + "," + nodes[1]);

        sms.nodes = text.ToString();
        PlayerTimeline time = moments.Find(a => a.timeline_id == timeline.id);
        moments.Remove(time);
        moments.Insert(0, sms);
        SMSDataMgr.Ins.MomentList.Add(sms);
        RefreshList();
        for (int i = 2; i < nodes.Length; i++)
        {
            yield return new WaitForSeconds(5f);
            text.Append("," + nodes[i]);
            sms.nodes = text.ToString();
        }
        SMSDataMgr.Ins.MomentList.Remove(sms);
    }

    void OnClickMore()
    {
        baseView.GoToMoudle<SMSMoreMomentsMoudle, List<PlayerTimeline>>((int)SMSView.MoudleType.TYPE_MORE_MOMENTS, moments);
    }
}

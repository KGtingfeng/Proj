using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using FairyGUI;

public class SMSMomentsMoudle : BaseMoudle
{
    public static SMSMomentsMoudle ins;
    GList momentList;
    GList btnList;
    Controller friendController;
    GButton postMoments;
    List<PlayerTimeline> moments;
    Controller redpointController;
    WaitForSeconds longWait = new WaitForSeconds(5f);
    public override void InitMoudle(GComponent gComponent, int controllerIndex)
    {
        base.InitMoudle(gComponent, controllerIndex);
        InitUI();
        InitEvent();
        ins = this;
    }

    public override void InitUI()
    {
        base.InitUI();
        momentList = SearchChild("n71").asList;
        postMoments = SearchChild("n73").asButton;
        btnList = SearchChild("n6").asList;
        friendController = btnList.GetChildAt(1).asButton.GetController("c1");
        redpointController = postMoments.GetController("c1");
    }

    public override void InitEvent()
    {
        base.InitEvent();
        postMoments.onClick.Set(OnClickPost);
        momentList.scrollPane.onPullDownRelease.Set(() =>
        {
            momentList.RefreshVirtualList();
        });
        EventMgr.Ins.RegisterEvent<PlayerTimeline>(EventConfig.MOMENTS_RELEASE, Release);
    }

    public override void InitData<D>(D data)
    {
        base.InitData(data);
        moments = data as List<PlayerTimeline>;
        EventMgr.Ins.ReplaceEvent<GameTimelineNodeConfig>(EventConfig.MOMENTS_REPLY, Reply);
        momentList.scrollPane.ScrollTop();
        momentList.scrollPane.onPullDownRelease.Set(OnPullDownRelease);
        if (RedpointMgr.Ins.PostTimelineHaveRedpoint())
        {
            redpointController.selectedIndex = 1;
        }
        else
        {
            RedpointMgr.Ins.RemoveTimelineRedpoint();
            redpointController.selectedIndex = 0;
            friendController.selectedIndex = 0;
        }
        Sort();
        RefreshList();
        //if (GameData.isOpenGuider)
        //{
        //    StoryCacheMgr.storyCacheMgr.GetStoryGameSave("Newbie", GameGuideConfig.TYPE_MONENT, (storyGameSave) =>
        //    {
        //        if (storyGameSave.IsDefault)
        //        {
        //            GameData.guiderCurrent = GuiderInfoLinked.GetCurrGuiderInfo(GameGuideConfig.TYPE_MONENT, 1);
        //            UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);
        //            StoryCacheMgr.storyCacheMgr.SaveProgress("Newbie", "1", GameGuideConfig.TYPE_MONENT);
        //        }
        //    });
        //}       
    }

    void Sort()
    {
        moments.Sort(delegate (PlayerTimeline roleA, PlayerTimeline roleB)
        {
            if (GameTool.GetDateTime(roleA.update_time) == GameTool.GetDateTime(roleB.update_time))
                return 0;
            return GameTool.GetDateTime(roleB.update_time).CompareTo(GameTool.GetDateTime(roleA.update_time));
        });
    }

    void RefreshList()
    {
        momentList.SetVirtual();
        momentList.itemRenderer = RendererItem;
        momentList.numItems = moments.Count;
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
            iconLoader.onClick.Clear();
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
            iconLoader.onClick.Set(() => { OnClickIcon(gameCell.actor_id); });
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
    /// 点击发送
    /// </summary>
    public void OnClickPost()
    {
        WWWForm wWWForm = new WWWForm();
        GameMonoBehaviour.Ins.RequestInfoPost<List<PlayerTimeline>>(NetHeaderConfig.CELL_SEND_MOMENT, wWWForm, RequestQuerySend);
    }

    void RequestQuerySend(List<PlayerTimeline> sms)
    {
        RedpointMgr.Ins.RemoveTimelineRedpoint();
        redpointController.selectedIndex = 0;
        friendController.selectedIndex = 0;
        if (sms.Count == 0)
        {
            UIMgr.Ins.showErrorMsgWindow("当前没有动态可发布！");
        }
        else
        {
            MomentsInfo info = new MomentsInfo();
            info.type = MomentsInfo.TYPE_POST;
            info.sms = sms;
            UIMgr.Ins.showNextPopupView<SMSMomentsReplyView, MomentsInfo>(info);
        }

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
        Debug.Log("reply set moment timelineId = " + gameCell.id + "   nodeId = " + nodeConfig.id);
        GameMonoBehaviour.Ins.RequestInfoPost<PlayerTimeline>(NetHeaderConfig.CELL_SET_MOMENT, wWWForm, RequestReply);
    }

    public void NewbieOnClickReply()
    {
        string[] moment = moments[0].nodes.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        GameTimelineNodeConfig nodeConfig = DataUtil.GetGameTimelineNodeConfig(int.Parse(moment[0]));
        GameTimelinePointConfig pointConfig = DataUtil.GetGameTimelinePointConfig(nodeConfig.point_id);
        OnClickReply(pointConfig);
    }


    void RequestReply(PlayerTimeline timeline)
    {
        GetAward(timeline);
        SMSView.view.StartCoroutine(SetTimeShow(timeline));
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
        PlayerTimeline time = moments.Find(a => a.timeline_id == timeline.timeline_id);
        moments.Remove(time);
        moments.Insert(0, sms);
        SMSDataMgr.Ins.MomentList.Add(sms);
        RefreshList();
        for (int i = 2; i < nodes.Length; i++)
        {
            yield return longWait;
            text.Append("," + nodes[i]);
            sms.nodes = text.ToString();
        }
        SMSDataMgr.Ins.MomentList.Remove(sms);
    }

    /// <summary>
    /// 发布
    /// </summary>
    void Release(PlayerTimeline sms)
    {
        GameCellTimelineConfig gameCell = DataUtil.GetGameCellTimelineConfig(sms.id);
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("timelineId", sms.id);
        wWWForm.AddField("nodeId", gameCell.startPoint);
        GameMonoBehaviour.Ins.RequestInfoPost<PlayerTimeline>(NetHeaderConfig.CELL_SET_MOMENT, wWWForm, RequestPost);
    }

    void RequestPost(PlayerTimeline timeline)
    {
        GetAward(timeline);
        SMSView.view.StartCoroutine(SetMyTimeShow(timeline));
        momentList.scrollPane.ScrollTop();
    }

    IEnumerator SetMyTimeShow(PlayerTimeline timeline)
    {
        PlayerTimeline sms = new PlayerTimeline();
        sms.id = timeline.timeline_id;
        sms.timeline_id = timeline.timeline_id;
        SMSDataMgr.Ins.MomentList.Add(sms);
        string[] nodes = timeline.nodes.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        StringBuilder text = new StringBuilder();
        text.Append(nodes[0]);
        sms.nodes = text.ToString();
        moments.Insert(0, sms);
        RefreshList();
        momentList.scrollPane.ScrollTop();
        for (int i = 1; i < nodes.Length; i++)
        {
            yield return longWait;
            text.Append("," + nodes[i]);
            sms.nodes = text.ToString();
        }
        SMSDataMgr.Ins.MomentList.Remove(sms);
    }

    void GetAward(PlayerTimeline timeline)
    {
        AwardInfo info = new AwardInfo();
        info.award = timeline.awards;
        info.extra = timeline.extra;
        GameTool.GetAwards(info);
    }

    /// <summary>
    /// 前往个人信息
    /// </summary>
    /// <param name="actorId"></param>
    void OnClickIcon(int actorId)
    {
        SMSDataMgr.Ins.Moudle = SMSView.MoudleType.TYPE_MOMENTS;
        NormalInfo normalInfo = new NormalInfo();
        normalInfo.index = actorId;
        baseView.GoToMoudle<SMSPersonalInfoMoudle, NormalInfo>((int)SMSView.MoudleType.TYPE_PERSONAL_INFO, normalInfo);
    }

    void OnPullDownRelease()
    {
        GComponent header = momentList.scrollPane.header;
        Controller controller = header.GetController("c1");
        if (controller.selectedIndex == 0 && header.height > header.sourceHeight)
        {
            controller.selectedIndex = 1;
            momentList.scrollPane.LockHeader(header.sourceHeight);

            Timers.inst.Add(2, 1, (object param) =>
            {
                RefreshList();
                controller.selectedIndex = 0;
                momentList.scrollPane.LockHeader(0);

            });
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

/// <summary>
/// 朋友圈回复与发布
/// </summary>
[ViewAttr("Game/UI/D_Message", "D_Message", "Frame_postreply_father", true)]
public class SMSMomentsReplyView : BaseView
{
    public static SMSMomentsReplyView ins;
    //回复
    GList replyList;
    //发布
    GList releaseList;
    MomentsInfo info;
    List<PlayerTimeline> sms;
    public override void InitUI()
    {
        base.InitUI();
        replyList = SearchChild("n13").asCom.GetChild("n4").asList;
        releaseList = SearchChild("n13").asCom.GetChild("n3").asList;
        controller = SearchChild("n13").asCom.GetController("c1");
        InitEvent();
        ins = this;
    }

    public override void InitEvent()
    {
        base.InitEvent();
        ui.onClick.Set(() =>
        {
            onDeleteAnimation<SMSMomentsReplyView>();
        });
       
    }

    public override void InitData<D>(D data)
    {
        OnShowAnimation();
        base.InitData(data);
        info = data as MomentsInfo;
        controller.selectedIndex = info.type;
        if (info.type == MomentsInfo.TYPE_REPLY)
            RefreshReply();
        else
        {
            sms = info.sms;
            RefreshRelease();
        }
        if (GameData.guiderCurrent != null && GameData.guiderCurrent.guiderInfo.flow == 9 && GameData.guiderCurrent.guiderInfo.step == 3)
        {
            GameData.guiderCurrent = GuiderInfoLinked.GetCurrGuiderInfo(9, 4);
            UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);

        }
    }

    GameTimelinePointConfig pointConfig;
    void RefreshReply()
    {
        GameTimelineNodeConfig nodeConfig = DataUtil.GetGameTimelineNodeConfig(info.PointConfig.point1);
        pointConfig = DataUtil.GetGameTimelinePointConfig(nodeConfig.point_id);
        replyList.itemRenderer = ReplyItemRenderer;
        replyList.numItems = pointConfig.Contents.Count;
        replyList.onClickItem.Set(OnClickReply);
    }

    void ReplyItemRenderer(int index, GObject gObject)
    {
        GButton gButton = gObject.asButton;
        string content = GameTool.Conversion(pointConfig.Contents[index].content);
        gButton.title = content;
    }

    void OnClickReply(EventContext context)
    {
        int selectedIndex = replyList.GetChildIndex((GObject)context.data);
        int itemIndex = replyList.ChildIndexToItemIndex(selectedIndex);
        GameTimelineNodeConfig nodeConfig = DataUtil.GetGameTimelineNodeConfig(pointConfig.Contents[itemIndex].point);
        EventMgr.Ins.DispachEvent(EventConfig.MOMENTS_REPLY, nodeConfig);
        OnHideAnimation();
    }

    public void NewbieReply()
    {
        GameTimelineNodeConfig nodeConfig = DataUtil.GetGameTimelineNodeConfig(pointConfig.Contents[0].point);
        EventMgr.Ins.DispachEvent(EventConfig.MOMENTS_REPLY, nodeConfig);
        OnHideAnimation();
    }

    void RefreshRelease()
    {

        releaseList.itemRenderer = ReleaseItemRenderer;
        releaseList.numItems = sms.Count;
        releaseList.onClickItem.Set(OnClickRelease);
    }

    void ReleaseItemRenderer(int index, GObject gObject)
    {
        GButton gButton = gObject.asButton;
        gButton.title = sms[index].name;
    }

    void OnClickRelease(EventContext context)
    {
        int selectedIndex = releaseList.GetChildIndex((GObject)context.data);
        int itemIndex = releaseList.ChildIndexToItemIndex(selectedIndex);
        EventMgr.Ins.DispachEvent(EventConfig.MOMENTS_RELEASE, sms[itemIndex]);
        OnHideAnimation();
    }

    public void NewbieRelease()
    {
        EventMgr.Ins.DispachEvent(EventConfig.MOMENTS_RELEASE, sms[0]);
        OnHideAnimation();
    }

}

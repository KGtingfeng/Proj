using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

public class SMSMoreMomentsMoudle : BaseMoudle
{
    List<PlayerTimeline> timelines;
    GList recordList;
    GTextField nameText;
    int type;
    public override void InitMoudle(GComponent gComponent, int controllerIndex)
    {
        base.InitMoudle(gComponent, controllerIndex);
        InitUI();
        InitEvent();
    }

    public override void InitUI()
    {
        base.InitUI();
        recordList = SearchChild("n82").asList;
        nameText = SearchChild("n81").asCom.GetChild("n36").asTextField;
        nameText.text = "历史";

    }

    public override void InitEvent()
    {
        base.InitEvent();
        EventMgr.Ins.RegisterEvent(EventConfig.MORE_MOMENTS_REBACK, GotoPersonalMoments);
    }


    public override void InitData<D>(D data)
    {
        base.InitData(data);
        timelines = data as List<PlayerTimeline>;
        nameText.text = "历史";
        type = SMSDataMgr.Ins.ComeForm;
        Refresh();
    }

    void Refresh()
    {
        recordList.SetVirtual();
        recordList.itemRenderer = RendererItem;
        recordList.numItems = timelines.Count;
        recordList.onClickItem.Set(OnClickItem);
    }

    void RendererItem(int index, GObject gObject)
    {
        GButton gButton = gObject.asButton;
        GameCellTimelineConfig gameCell = DataUtil.GetGameCellTimelineConfig(timelines[index].id);
        gButton.title = gameCell.title;

    }


    void OnClickItem(EventContext context)
    {
        int selectedIndex = recordList.GetChildIndex((GObject)context.data);
        int itemIndex = recordList.ChildIndexToItemIndex(selectedIndex);
        SMSDataMgr.Ins.ComeForm = (int)SMSView.MoudleType.TYPE_MORE_MOMENTS;
        List<PlayerTimeline> timeline = new List<PlayerTimeline>();
        timeline.Add(timelines[itemIndex]);
        baseView.GoToMoudle<SMSPersonalMomentsMoudle, List<PlayerTimeline>>((int)SMSView.MoudleType.TYPE_PERSONAL_MOMENTS, timeline);

    }

    void GotoPersonalMoments()
    {
        GameCellTimelineConfig gameCell = DataUtil.GetGameCellTimelineConfig(timelines[0].id);
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("actorId", gameCell.actor_id);
        GameMonoBehaviour.Ins.RequestInfoPostHaveData<List<PlayerTimeline>>(NetHeaderConfig.CELL_LIST_MOMENT, wWWForm, RequestGotoPersonalMoment, false);
    }

    void RequestGotoPersonalMoment(List<PlayerTimeline> timelines)
    {
        SMSDataMgr.Ins.ComeForm = type;
        if (timelines != null && timelines.Count > 0)
        {
            foreach (PlayerTimeline sms in SMSDataMgr.Ins.MomentList)
            {
                PlayerTimeline smsListIndex = timelines.Find(a => a.timeline_id == sms.timeline_id);
                if (smsListIndex != null)
                {
                    int index = timelines.IndexOf(smsListIndex);
                    timelines.Remove(smsListIndex);
                    timelines.Insert(index, sms);
                }
            }
            baseView.GoToMoudle<SMSPersonalMomentsMoudle, List<PlayerTimeline>>((int)SMSView.MoudleType.TYPE_PERSONAL_MOMENTS, timelines);
        }

    }
}

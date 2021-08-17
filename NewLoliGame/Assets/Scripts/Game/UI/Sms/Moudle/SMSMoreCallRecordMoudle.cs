using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using System;

public class SMSMoreCallRecordMoudle : BaseMoudle
{
    GList callList;
    List<SmsListIndex> smsLists;
    public override void InitMoudle(GComponent gComponent, int controllerIndex)
    {
        base.InitMoudle(gComponent, controllerIndex);
        InitUI();
        InitEvent();
    }

    public override void InitUI()
    {
        base.InitUI();
        callList = SearchChild("n62").asList;
        InitEvent();
    }

    public override void InitEvent()
    {
        base.InitEvent();
        EventMgr.Ins.RegisterEvent<GameCellSmsConfig>(EventConfig.SMS_CALL_FINISH_CALL_RECORD, CallFinish);
        EventMgr.Ins.RegisterEvent<GameCellSmsConfig>(EventConfig.SMS_CALL_START_CALL_RECORD, CallStart);
    }

    public override void InitData<D>(D data)
    {
        base.InitData(data);
        smsLists = data as List<SmsListIndex>;
        if (smsLists != null)
            RefreshList();
    }

    void RefreshList()
    {
        Debug.LogError("Count: " + smsLists.Count);
        callList.SetVirtual();
        callList.itemRenderer = RendererItem;
        callList.numItems = smsLists.Count;
        callList.onClickItem.Set(OnClickItem);
    }

    void RendererItem(int index, GObject gObject)
    {
        GComponent gCom = gObject.asCom;

        Controller controller1 = gCom.GetController("c1");
        Controller controller2 = gCom.GetController("c2");
        GTextField nameText = gCom.GetChild("title").asTextField;
        SmsListIndex sms = smsLists[index];
        GameCellSmsConfig cellSmsConfig = DataUtil.GetGameCellSmsConfig(sms.sms_id);
        nameText.text = cellSmsConfig.title;
        controller1.selectedIndex = cellSmsConfig.message_type == 11 ? 1 : 0;
        switch (sms.story_status)
        {
            case SmsListIndex.TYPE_HAVE_DONE:
                controller2.selectedIndex = 2;
                break;
            case SmsListIndex.TYPE_UNREAD:
                controller2.selectedIndex = 1;
                break;
            case SmsListIndex.TYPE_READED_UNDONE:
            case SmsListIndex.TYPE_READED:
                controller2.selectedIndex = 0;
                break;
        }
    }

    void OnClickItem(EventContext context)
    {
        int selectedIndex = callList.GetChildIndex((GObject)context.data);
        int itemIndex = callList.ChildIndexToItemIndex(selectedIndex);
        SmsListIndex smsList = smsLists[itemIndex];
        CallDataMgr.Ins.isCalling = false;
        CallDataMgr.Ins.isViewSms = true;
        UIMgr.Ins.showNextPopupView<CallInterfaceView, SmsListIndex>(smsList);
    }

    /// <summary>
    /// 电话或视频结束调用
    /// </summary>
    /// <param name="smsNodeConfig"></param>
    void CallFinish(GameCellSmsConfig cellSmsConfig)
    {
        SmsListIndex sms = smsLists.Find(a => a.sms_id == cellSmsConfig.id);
        if (sms != null && sms.story_status != SmsListIndex.TYPE_HAVE_DONE)
        {
            sms.story_status = SmsListIndex.TYPE_HAVE_DONE;
            RefreshList();
        }
    }

    /// <summary>
    /// 电话或视频开始调用
    /// </summary>
    /// <param name="smsNodeConfig"></param>
    void CallStart(GameCellSmsConfig cellSmsConfig)
    {
        SmsListIndex sms = smsLists.Find(a => a.sms_id == cellSmsConfig.id);
        if (sms != null && sms.story_status == SmsListIndex.TYPE_UNREAD)
        {
            sms.story_status = SmsListIndex.TYPE_READED_UNDONE;
            RefreshList();
        }
    }
}

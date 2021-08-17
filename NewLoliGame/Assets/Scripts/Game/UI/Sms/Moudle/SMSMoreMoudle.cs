using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

public class SMSMoreMoudle : BaseMoudle
{
    public static SMSMoreMoudle ins;
    GList moreList;
    GButton btnSMSRecord;
    GButton btnCallRecord;
    GButton btnBg;

    Controller redpointController;
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
        moreList = SearchChild("n53").asList;
        btnSMSRecord = moreList.GetChildAt(0).asButton;
        btnCallRecord = moreList.GetChildAt(1).asButton;
        btnBg = moreList.GetChildAt(2).asButton;
        redpointController = btnCallRecord.GetController("c1");
    }

    public override void InitEvent()
    {
        base.InitEvent();

        btnSMSRecord.onClick.Set(OnClickSms);
        btnCallRecord.onClick.Set(OnClickCall);
        btnBg.onClick.Set(OnClickBg);
    }

    public override void InitData()
    {
        base.InitData();
        redpointController.selectedIndex = RedpointMgr.Ins.callRedpoint.Contains(SMSDataMgr.Ins.CurrentRole) ? 1 : 0;
    }

    void OnClickSms()
    {
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("type", (int)TypeConfig.Consume.Message);
        wWWForm.AddField("actorId", SMSDataMgr.Ins.CurrentRole);
        GameMonoBehaviour.Ins.RequestInfoPost<List<SmsListIndex>>(NetHeaderConfig.CELL_LIST_PACK, wWWForm, RequestGotoSmsRecord);
    }

    void RequestGotoSmsRecord(List<SmsListIndex> cellLists)
    {
        SMSDataMgr.Ins.ComeForm = (int)SMSView.MoudleType.TYPE_MORE;
        if (cellLists != null && cellLists.Count > 0)
            baseView.GoToMoudle<SMSMoreRecordMoudle, List<SmsListIndex>>((int)SMSView.MoudleType.TYPE_MORE_RECORD, cellLists);
        else
            baseView.GoToMoudle<SMSKongMoudle, string>((int)SMSView.MoudleType.TYPE_KONG, ((int)SMSKongMoudle.loaderType.SMS).ToString());
    }

    public void OnClickCall()
    {
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("type", (int)TypeConfig.Consume.Mobile);
        wWWForm.AddField("actorId", SMSDataMgr.Ins.CurrentRole);
        GameMonoBehaviour.Ins.RequestInfoPost<List<SmsListIndex>>(NetHeaderConfig.CELL_LIST_PACK, wWWForm, RequestGotoCallRecord);
    }

    void RequestGotoCallRecord(List<SmsListIndex> cellLists)
    {
        SMSDataMgr.Ins.ComeForm = (int)SMSView.MoudleType.TYPE_MORE;
        if (cellLists != null && cellLists.Count > 0)
            baseView.GoToMoudle<SMSMoreCallRecordMoudle, List<SmsListIndex>>((int)SMSView.MoudleType.TYPE_MORE_CALL, cellLists);
        else
            baseView.GoToMoudle<SMSKongMoudle, string>((int)SMSView.MoudleType.TYPE_KONG, ((int)SMSKongMoudle.loaderType.CALL).ToString());
    }

    void OnClickBg()
    {
        if (SMSDataMgr.Ins.CurrentRole != 0)
        {
            WWWForm wWWForm = new WWWForm();
            wWWForm.AddField("actor_id", SMSDataMgr.Ins.CurrentRole);
            GameMonoBehaviour.Ins.RequestInfoPostList<GameMomentConfig>(NetHeaderConfig.PLAYER_MOMENT, wWWForm, () =>
            {
                baseView.GoToMoudle<SMSSetCallBgMoudle, NormalInfo>((int)SMSView.MoudleType.TYPE_MORE_BG, null);
            });
        }
    }
}

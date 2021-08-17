using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;
using System;

/// <summary>
/// 来电提示
/// </summary>
public class CallReminderMoudle : BaseMoudle
{
    GLoader headIcon;
    GTextField nameTextField;

    SmsListIndex smsListIndex;

    bool isRefused;

    public override void InitMoudle(GComponent gComponent, int controllerIndex)
    {
        base.InitMoudle(gComponent, controllerIndex);

        InitUI();
        InitEvent();
    }

    public override void InitUI()
    {
        headIcon = SearchChild("n6").asLoader;
        nameTextField = SearchChild("n11").asTextField;
    }

    public override void InitData<D>(D data)
    {
        ViewEffect();
        base.InitData(data);

        smsListIndex = data as SmsListIndex;
        
        headIcon.url = UrlUtil.GetStoryHeadIconUrl(CallDataMgr.Ins.cardsConfig.card_id);
        //smsListIndex存在昵称时替换
        nameTextField.text = CallDataMgr.Ins.cardsConfig.name_cn;
    }

    public override void InitEvent()
    {
        SearchChild("n4").onClick.Set(onClickReceive);

        SearchChild("n5").onClick.Set(() =>
        {
            CallDataMgr.Ins.Dispose();
            baseView.StopAllCoroutines();
            UIMgr.Ins.HideView<CallView>();
        });
    }

    void onClickReceive() {




        Action<SmsListIndex> callback = (SmsListIndex sms) =>
        {
            baseView.GoToMoudle<CallInterfaceMoudle, SmsListIndex>((int)CallView.MoudleType.CallInterface, sms);
        };
        CallView.view.ReceiveCall(CallDataMgr.Ins.cellSmsConfig.startPoint, callback);
    }

    //每次打开都会弹出
    void ViewEffect()
    {
        Vector3 vector = ui.position;
        ui.SetPosition(vector.x, vector.y - 450, 0);
        ui.TweenMoveY(vector.y, 1f).SetEase(EaseType.QuadIn).OnComplete(CallView.view.RefusedToAnswerAuto);
    }
    
}

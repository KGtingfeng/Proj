using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using System;

[ViewAttr("Game/UI/T_Common", "T_Common", "Messagecall")]
public class SMSPopView : BaseView
{
    GComponent message;
    GLoader iconLoader;
    GTextField nameText;
    GTextField contentText;

    GComponent call;
    GLoader callIconLoader;
    GTextField callNameText;
    string callActorName;

    GComponent moment;
    GLoader momentIcon;
    GTextField momentNameText;
    GTextField momentContext;


    GameCellSmsConfig gameCellSms;
    GameSmsPointConfig gameSmsPoint;
    PushPopInfo popInfo;
    public override void InitUI()
    {
        base.InitUI();
        controller = ui.GetController("c1");
        message = SearchChild("n1").asCom;
        iconLoader = message.GetChild("n5").asCom.GetChild("n5").asCom.GetChild("n5").asLoader;
        nameText = message.GetChild("n48").asTextField;
        contentText = message.GetChild("n49").asTextField;

        call = SearchChild("n0").asCom;
        callNameText = call.GetChild("n8").asTextField;
        callIconLoader = call.GetChild("n5").asLoader;

        moment = SearchChild("n2").asCom;
        momentIcon = moment.GetChild("n5").asCom.GetChild("n5").asCom.GetChild("n5").asLoader;
        momentNameText = moment.GetChild("n48").asTextField;
        momentContext = moment.GetChild("n49").asTextField;
        InitEvent();
    }

    public override void InitEvent()
    {
        base.InitEvent();

        call.GetChild("n2").onClick.Set(() =>                      //接听
        {
            GotoCallInterfaceView(true, popInfo.smsListIndex);
        });

        call.GetChild("n4").onClick.Set(RefusedCall);               //拒绝

        callIconLoader.onClick.Set(() =>
        {
            GotoCallInterfaceView(false, popInfo.smsListIndex);
        });
    }


    public override void InitData<D>(D data)
    {
        base.InitData(data);
        popInfo = data as PushPopInfo;
        EventMgr.Ins.DispachEvent(EventConfig.MUSIC_MAIN_TIPS);

        if (popInfo.type == 0)
        {
            gameCellSms = DataUtil.GetGameCellSmsConfig(popInfo.smsListIndex.sms_id);
            if (gameCellSms != null)
            {
                gameSmsPoint = DataUtil.GetGameSmsPointConfig(gameCellSms.startPoint);
                switch (gameCellSms.message_type)
                {
                    case (int)TypeConfig.Consume.Mobile:

                        InitCall();

                        break;
                    case (int)TypeConfig.Consume.Message:
                        InitSms();

                        break;
                }
            }
        }
        else
        {
            InitMoment();
        }
    }

    void InitSms()
    {
        if (gameSmsPoint.title == "0")
        {
            UIMgr.Ins.HideView<SMSPopView>();
        }
        else
        {
            controller.selectedIndex = 1;
            controller.selectedIndex = 0;
            iconLoader.url = UrlUtil.GetStoryHeadIconUrl(gameCellSms.actor_id);
            nameText.text = GameTool.GetActorName(popInfo.smsListIndex.actor_id);
            string content = gameSmsPoint.content1;
            switch (gameSmsPoint.type)
            {
                case (int)TypeConfig.SMSType.TYPE_Hongbao:
                    content = "【红包】";
                    break;
                case (int)TypeConfig.SMSType.TYPE_Image:
                    content = "【照片】";
                    break;
            }
            content = DataUtil.ReplaceCharacterWithStarts(content);
            content = GameTool.GetCutText(content, 9);
            contentText.text = GameTool.Conversion(content);
            message.onClick.Set(() =>
            {
                OnClickSms(RequestGotoSms, message);
            });
            StartCoroutine(CloseView(message));
        }
    }

    void InitMoment()
    {
        GameTimelineNodeConfig n = DataUtil.GetGameTimelineNodeConfig(popInfo.nodeId);
        GameTimelinePointConfig p = DataUtil.GetGameTimelinePointConfig(n.point_id);
        GameCellTimelineConfig cell = DataUtil.GetGameCellTimelineConfig(n.sms_id);
        controller.selectedIndex = 0;
        controller.selectedIndex = 2;
        if (p.title == "0")
        {
            Debug.LogError("SPS------  GameTimelineNodeConfig is error ,nodeId = " + n.id);
            UIMgr.Ins.HideView<SMSPopView>();
            return;
        }
        momentIcon.url = UrlUtil.GetStoryHeadIconUrl(int.Parse(p.title));
        momentNameText.text = GameTool.GetActorName(int.Parse(p.title));
        if (p.type == 6)
            momentContext.text = "回复了你的朋友圈！";
        else
        {
            if (cell.actor_id == 0)
                momentContext.text = "回复了你的朋友圈！";
            else if (cell.startPoint == n.id)
                momentContext.text = "发布了一条新的朋友圈！";
            else
            {
                StopAllCoroutines();
                UIMgr.Ins.HideView<SMSPopView>();
                return;
            }
        }
        moment.onClick.Set(() =>
        {
            OnClickSms(RequestGotoMoment, moment);
        });
        StartCoroutine(CloseView(moment));
    }

    WaitForSeconds wait = new WaitForSeconds(2f);
    IEnumerator CloseView(GComponent gComponent)
    {
        yield return wait;
        gComponent.TweenMoveY(-218, 1f);
        yield return wait;
        UIMgr.Ins.HideView<SMSPopView>();
    }

    SMSView view;
    void OnClickSms(Action<List<SmsListIndex>> action, GComponent gComponent)
    {
        if (MainView.ins.hasHightfive)
            return;
        StopCoroutine(CloseView(gComponent));
        gComponent.onClick.Clear();
        WWWForm wWWForm = new WWWForm();
        GameMonoBehaviour.Ins.RequestInfoPost<List<SmsListIndex>>(NetHeaderConfig.CELL_LIST_INDEX, wWWForm, action);
    }

    void RequestGotoSms(List<SmsListIndex> smsLists)
    {
        view = UIMgr.Ins.showNextView<SMSView, List<SmsListIndex>>(smsLists) as SMSView;
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("actorId", gameCellSms.actor_id);
        GameMonoBehaviour.Ins.RequestInfoPostHaveData<SmsListIndex>(NetHeaderConfig.CELL_QUERY_MESSAGE, wWWForm, GotoSms, false);
    }

    void GotoSms(SmsListIndex smsList)
    {
        view.GoToMoudle<SMSMoudle, SmsListIndex>((int)SMSView.MoudleType.TYPE_SMS, smsList);
        UIMgr.Ins.HideView<SMSPopView>();
    }

    void RequestGotoMoment(List<SmsListIndex> smsLists)
    {
        view = UIMgr.Ins.showNextView<SMSView, List<SmsListIndex>>(smsLists) as SMSView;
        WWWForm wWWForm = new WWWForm();
        GameMonoBehaviour.Ins.RequestInfoPostHaveData<List<PlayerTimeline>>(NetHeaderConfig.CELL_QUERY_MOMENT, wWWForm, GotoMoment, false);
    }

    void GotoMoment(List<PlayerTimeline> timelines)
    {
        view.GoToMoudle<SMSMomentsMoudle, List<PlayerTimeline>>((int)SMSView.MoudleType.TYPE_MOMENTS, timelines);
        UIMgr.Ins.HideView<SMSPopView>();
    }

    # region 来电提醒 call_in2

    void InitCall()
    {
        Debug.Log(GetType() + "/InitCall()");
        AudioClip audioClip = Resources.Load(SoundConfig.PHONE_AUDIO_EFFECT_URL + (int)SoundConfig.PhoneAudioId.Call) as AudioClip;
        GRoot.inst.PlayEffectSound(audioClip);
        controller.selectedIndex = 0;
        controller.selectedIndex = 1;
        callIconLoader.url = UrlUtil.GetStoryHeadIconUrl(gameCellSms.actor_id);
        //callNameText.text = GameTool.GetActorName(popInfo.smsListIndex.actor_id);
        
        callNameText.text = callActorName + "来电.     ";
        EventMgr.Ins.RegisterEvent(EventConfig.REJECT_THE_CALL, RefusedCall);
        StartCoroutine(AutoRefusedCall());
        StartCoroutine(ShowCallReminderEffect());
    }

    WaitForSeconds dotShowInterval = new WaitForSeconds(0.2f);
    IEnumerator ShowCallReminderEffect()
    {
        GameInitCardsConfig gameInitCard = JsonConfig.GameInitCardsConfigs.Find(a => a.card_id == popInfo.smsListIndex.actor_id);
        callActorName = gameInitCard.name_cn;
        Debug.Log(callActorName + "给您来电了");
        while (true)
        {
            for (int i = 0; i < 6; i++)
            {
                if (i % 6 == 0)
                    callNameText.text = callActorName + "来电.     ";
                else
                    callNameText.text = StrUtil.ReplaceFirst(callNameText.text," ", "·");
                yield return dotShowInterval;
            }
        }
    }

    string ReplaceFirst(string text)
    {
        int index = text.IndexOf(" ");
        return text.Substring(0, index) + "·" + text.Substring(index + 1);
    }
    #endregion *********来电下拉框*********

    void GotoCallInterfaceView(bool isAgree, SmsListIndex smsListIndex)
    {
        if (MainView.ins.hasHightfive)
            return;
        GRoot.inst.StopEffectSound();
        CallDataMgr.Ins.isViewSms = isAgree;
        CallDataMgr.Ins.isCalling = isAgree;
        UIMgr.Ins.showNextPopupView<CallInterfaceView, SmsListIndex>(smsListIndex);
        UIMgr.Ins.HideView<SMSPopView>();
    }

    IEnumerator AutoRefusedCall()
    {
        yield return new WaitForSeconds(10f);
        RefusedCall();
    }

    void RefusedCall()
    {
        GRoot.inst.StopEffectSound();
        call.TweenMoveY(-462, 1f).SetEase(EaseType.QuadInOut).OnComplete(() =>
        {
            UIMgr.Ins.HideView<SMSPopView>();
        });
    }
}

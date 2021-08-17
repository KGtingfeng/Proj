using FairyGUI;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

/// <summary>
/// 交互图 - 通话详情部分
/// </summary>
[ViewAttr("Game/UI/D_call", "D_call", "Call")]
public class CallView : BaseView
{
    public static CallView view;
    public bool isGuide;
    public enum MoudleType
    {
        CallReminder = 0,
        CallInterface,
    };

    Dictionary<MoudleType, string> moudleNamePairs = new Dictionary<MoudleType, string>()
    {
        {MoudleType.CallReminder,"n0"},
        {MoudleType.CallInterface,"n1"},
    };

    public override void InitUI()
    {
        view = this;
        controller = ui.GetController("c1");

        InitEvent();
    }

    public override void InitData<D>(D data)
    {
        base.InitData(data);

        SmsListIndex smsListIndex = data as SmsListIndex;
        if (smsListIndex != null)
        {
            CallDataMgr.Ins.RefreshSMSInfo(smsListIndex.sms_id);
            GetCorrentCallInterface(smsListIndex);
            //if (GameData.isOpenGuider)
            //{
            //    WWWForm wWWForm = new WWWForm();
            //    wWWForm.AddField("nodeId", GameGuideConfig.TYPE_CALL);
            //    wWWForm.AddField("key", "Newbie");
            //    GameMonoBehaviour.Ins.RequestInfoPostHaveData<List<StoryGameSave>>(NetHeaderConfig.STROY_LOAD_GAME, wWWForm, (List<StoryGameSave> storyGameSaves) =>
            //    {
            //        if (storyGameSaves.Count > 0)
            //        {
            //            string[] save = storyGameSaves[0].value.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            //            GameData.guiderCurrent = GuiderInfoLinked.GetCurrGuiderInfo(int.Parse(save[0]), int.Parse(save[1]));
            //            if (GameData.guiderCurrent != null)
            //            {
            //                string[] roll_to = GameData.guiderCurrent.guiderInfo.roll_to.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            //                if (roll_to.Length < 2 || !GameData.isOpenGuider)
            //                {
            //                    isGuide = false;
            //                    return;
            //                }
            //                GameData.guiderCurrent = GuiderInfoLinked.GetCurrGuiderInfo(int.Parse(roll_to[0]), int.Parse(roll_to[1]));
            //                if (GameData.guiderCurrent != null)
            //                {
            //                    isGuide = true;
            //                }
            //            }
            //        }
            //        else
            //        {
            //            isGuide = true;
            //        }

            //    }, false);
            //}

        }
    }

    //获得当前来电展示界面
    void GetCorrentCallInterface(SmsListIndex smsListIndex)
    {
        if (CallDataMgr.Ins.isViewSms ||
            (SMSView.view != null && SMSDataMgr.Ins.IsOnSms && SMSDataMgr.Ins.CurrentRole == CallDataMgr.Ins.cellSmsConfig.actor_id))
        {
            OnShowAnimation();
            GoToMoudle<CallInterfaceMoudle, SmsListIndex>((int)MoudleType.CallInterface, smsListIndex);
            return;
        }
        GoToMoudle<CallReminderMoudle, SmsListIndex>((int)MoudleType.CallReminder, smsListIndex);
    }

    /// <summary>
    /// 保存节点
    /// </summary>
    public void QuerySaveNode(int nodeId, Action<SmsListIndex> callBack)
    {
        SmsListIndex smsListIndex = new SmsListIndex();
        smsListIndex.sms_id = 13;
        callBack(smsListIndex);


        //WWWForm wWWForm = new WWWForm();
        //wWWForm.AddField("actorId", CallDataMgr.Ins.cellSmsConfig.actor_id);
        //wWWForm.AddField("messageId", CallDataMgr.Ins.cellSmsConfig.id);
        //wWWForm.AddField("nodeId", nodeId);
        //GameMonoBehaviour.Ins.RequestInfoPost<SmsListIndex>(NetHeaderConfig.CELL_SET_STEP, wWWForm, callBack);

    }

    /// <summary>
    /// 自动拒绝电话
    /// </summary>
    public void RefusedToAnswerAuto()
    {
        StartCoroutine(Refused());
    }

    IEnumerator Refused()
    {
        yield return new WaitForSeconds(5f);

        CallDataMgr.Ins.Dispose();
        //电话小框关闭需要上移效果
        if (controller.selectedIndex == 0)
        {
            ui.TweenMoveY(ui.position.y - 450, 1f).SetEase(EaseType.QuadOut).OnComplete(() =>
            {
                UIMgr.Ins.HideView<CallView>();
            });
        }
        else
            UIMgr.Ins.HideView<CallView>();
    }

    public void ReceiveCall(int nodeId, Action<SmsListIndex> callBack)
    {
        //EventMgr.Ins.DispachEvent(EventConfig.SMS_CALL_START_MAIN, CallDataMgr.Ins.cellSmsConfig);
        //EventMgr.Ins.DispachEvent(EventConfig.SMS_CALL_START_CALL_RECORD, CallDataMgr.Ins.cellSmsConfig);

        CallDataMgr.Ins.isViewSms = true;
        CallDataMgr.Ins.isCalling = true;
        StopAllCoroutines();
        QuerySaveNode(nodeId, callBack);
    }


    public override void GoToMoudle<T, D>(int index, D data)
    {
        MoudleType moudleType = (MoudleType)index;
        BaseMoudle baseMoudle = FindMoudle<T>();
        if (baseMoudle == null)
        {
            baseMoudle = (BaseMoudle)Activator.CreateInstance(typeof(T));
            baseMoudles.Add(baseMoudle);
            GComponent gComponent = null;

            string componmentName;
            if (moudleNamePairs.TryGetValue(moudleType, out componmentName))
            {
                GObject gObject = ui.GetChild(componmentName);
                if (gObject == null)
                {
                    Debug.Log(componmentName + " not find, please check it");
                    return;
                }
                gComponent = gObject.asCom;
            }
            baseMoudle.baseView = this;
            baseMoudle.InitMoudle(gComponent, index);
        }
        baseMoudle.InitData(data);
        SwitchController(index);
    }
}

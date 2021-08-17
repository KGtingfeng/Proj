using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FairyGUI;

[ViewAttr("Game/UI/D_Message", "D_Message", "Message")]
public class SMSView : BaseView
{
    public static SMSView view;
    GLoader bgLoader;

    GButton btnChat;
    GButton btnFriend;
    GButton btnCintact;
    GList btnList;

    public enum MoudleType
    {
        //主界面
        TYPE_MAIN,
        //聊天
        TYPE_SMS,
        //聊天发送
        TYPE_SMS_SEND,
        //聊天获得
        TYPE_SMS_GET,
        //更多
        TYPE_MORE,
        //聊天记录列表
        TYPE_MORE_RECORD,
        //单个聊天记录
        TYPE_RECORD,
        //各种空
        TYPE_KONG,
        //电话记录列表
        TYPE_MORE_CALL,
        //电话背景
        TYPE_MORE_BG,
        //朋友圈
        TYPE_MOMENTS,
        //通讯录
        TYPE_ADDRESS_BOOK,
        //角色信息
        TYPE_PERSONAL_INFO,
        //个人朋友圈或单个朋友圈记录
        TYPE_PERSONAL_MOMENTS,
        //朋友圈记录列表
        TYPE_MORE_MOMENTS,
    };

    readonly Dictionary<MoudleType, string> moudleNamePairs = new Dictionary<MoudleType, string>()
    {
        { MoudleType.TYPE_ADDRESS_BOOK,"n75"},
        { MoudleType.TYPE_PERSONAL_INFO,"n22"}
    };

    public override void InitUI()
    {
        base.InitUI();
        view = this;
        controller = ui.GetController("c1");
        bgLoader = SearchChild("n0").asLoader;
        btnList = SearchChild("n6").asList;

        btnChat = btnList.GetChildAt(0).asButton;
        btnFriend = btnList.GetChildAt(1).asButton;
        btnCintact = btnList.GetChildAt(2).asButton;


        InitEvent();
    }

    public override void InitEvent()
    {
        base.InitEvent();
        //返回
        SearchChild("n4").onClick.Set(OnClickBack);
        //聊天
        btnChat.onClick.Set(OnClickChat);
        //朋友圈
        btnFriend.onClick.Set(OnClickFriend);
        //通讯录
        btnCintact.onClick.Set(OnClickCintact);
        EventMgr.Ins.RegisterEvent<NormalInfo>(EventConfig.GOTO_PERSONAL_MPMENT, GotoPersonalMoments);
    }

    public override void InitData<D>(D data)
    {
        onShow();
        base.InitData(data);
        List<SmsListIndex> smsLists = data as List<SmsListIndex>;
        SMSDataMgr.Ins.IsOnPhone = true;
        btnList.selectedIndex = 0;
        bgLoader.url = UrlUtil.GetMessageBg("1");
        GoToMoudle<SMSMainMoudle, List<SmsListIndex>>((int)MoudleType.TYPE_MAIN, smsLists);
        if (GameData.OwnRoleList == null || (GameData.OwnRoleList != null && GameData.OwnRoleList.Count == 0))
        {
            WWWForm wWWForm = new WWWForm();
            GameMonoBehaviour.Ins.RequestInfoPostList<Role>(NetHeaderConfig.ACTOR_LIST, wWWForm, null, true);
        }
    }

    public void OnClickBack()
    {
        switch (controller.selectedIndex)
        {
            case (int)MoudleType.TYPE_MOMENTS:
            case (int)MoudleType.TYPE_MAIN:
            case (int)MoudleType.TYPE_ADDRESS_BOOK:
                SMSDataMgr.Ins.IsOnPhone = false;
                StopAllCoroutines();
                SMSDataMgr.Ins.MomentList.Clear();
                UIMgr.Ins.showMainView<SMSView>();
                break;
            case (int)MoudleType.TYPE_SMS:
            case (int)MoudleType.TYPE_SMS_SEND:
            case (int)MoudleType.TYPE_SMS_GET:
                EventMgr.Ins.DispachEvent(EventConfig.SMS_MOUDLE_LEAVE);
                WWWForm wWWForm = new WWWForm();
                GameMonoBehaviour.Ins.RequestInfoPost<PlayerRedpoint>(NetHeaderConfig.RED_POINT, wWWForm, () =>
                {
                    GoToMoudle<SMSMainMoudle>((int)MoudleType.TYPE_MAIN);
                });
                break;
            case (int)MoudleType.TYPE_MORE:
                GotoSMSMoudle();
                break;
            case (int)MoudleType.TYPE_MORE_RECORD:
            case (int)MoudleType.TYPE_MORE_CALL:
            case (int)MoudleType.TYPE_KONG:
            case (int)MoudleType.TYPE_PERSONAL_MOMENTS:
                RecordBack();
                break;
            case (int)MoudleType.TYPE_MORE_MOMENTS:
                EventMgr.Ins.DispachEvent(EventConfig.MORE_MOMENTS_REBACK);
                break;
            case (int)MoudleType.TYPE_MORE_BG:
                GoToMoudle<SMSMoreMoudle>((int)MoudleType.TYPE_MORE);
                break;
            case (int)MoudleType.TYPE_RECORD:
                GoToMoudle<SMSMoreRecordMoudle>((int)MoudleType.TYPE_MORE_RECORD);
                break;
            case (int)MoudleType.TYPE_PERSONAL_INFO:
                PersonalInfoBack();
                break;
            default:
                Debug.Log("Error Phone Reback  MoudleType!");
                break;
        }
    }


    void GotoSMSMoudle()
    {
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("actorId", SMSDataMgr.Ins.CurrentRole);
        GameMonoBehaviour.Ins.RequestInfoPostHaveData<SmsListIndex>(NetHeaderConfig.CELL_QUERY_MESSAGE, wWWForm, RequestGotoSMS, false);
    }

    void RequestGotoSMS(SmsListIndex smsList)
    {
        GoToMoudle<SMSMoudle, SmsListIndex>((int)MoudleType.TYPE_SMS, smsList);
    }

    void OnClickChat()
    {
        if (controller.selectedIndex != (int)MoudleType.TYPE_MAIN)
            GoToMoudle<SMSMainMoudle>((int)MoudleType.TYPE_MAIN);
    }
    /// <summary>
    /// 朋友圈
    /// </summary>
    void OnClickFriend()
    {
        WWWForm wWWForm = new WWWForm();
        GameMonoBehaviour.Ins.RequestInfoPostHaveData<List<PlayerTimeline>>(NetHeaderConfig.CELL_QUERY_MOMENT, wWWForm, RequestGotoMoment);
    }

    void RequestGotoMoment(List<PlayerTimeline> smsList)
    {
        foreach (PlayerTimeline sms in SMSDataMgr.Ins.MomentList)
        {
            PlayerTimeline smsListIndex = smsList.Find(a => a.timeline_id == sms.timeline_id);
            if (smsListIndex != null)
            {
                int index = smsList.IndexOf(smsListIndex);
                sms.update_time = smsListIndex.update_time;
                smsList.Insert(index, sms);
                smsList.Remove(smsListIndex);
            }
        }
        GoToMoudle<SMSMomentsMoudle, List<PlayerTimeline>>((int)MoudleType.TYPE_MOMENTS, smsList);
    }
    /// <summary>
    /// 通讯录
    /// </summary>
    void OnClickCintact()
    {
        if (controller.selectedIndex != (int)MoudleType.TYPE_ADDRESS_BOOK)
        {
            WWWForm wWWForm = new WWWForm();
            GameMonoBehaviour.Ins.RequestInfoPostList<Role>(NetHeaderConfig.ACTOR_LIST, wWWForm, () =>
            {
                GoToMoudle<SMSAddressBookMoudle>((int)MoudleType.TYPE_ADDRESS_BOOK);
            });
        }
    }
    /// <summary>
    /// 角色信息返回
    /// </summary>
    void PersonalInfoBack()
    {
        switch (SMSDataMgr.Ins.Moudle)
        {
            case MoudleType.TYPE_ADDRESS_BOOK:
                GoToMoudle<SMSAddressBookMoudle>((int)MoudleType.TYPE_ADDRESS_BOOK);
                break;
            case MoudleType.TYPE_SMS:
                GotoSMSMoudle();
                break;
            case MoudleType.TYPE_MOMENTS:
                OnClickFriend();
                break;
        }
    }
    /// <summary>
    /// 各种记录、朋友圈返回
    /// </summary>
    void RecordBack()
    {
        //WWWForm wWWForm = new WWWForm();
        //GameMonoBehaviour.Ins.RequestInfoPost<PlayerRedpoint>(NetHeaderConfig.RED_POINT, wWWForm, () =>
        //{
            switch (SMSDataMgr.Ins.ComeForm)
            {
                case (int)MoudleType.TYPE_MORE:
                    GoToMoudle<SMSMoreMoudle>((int)MoudleType.TYPE_MORE);
                    break;
                case (int)MoudleType.TYPE_PERSONAL_INFO:
                    GoToMoudle<SMSPersonalInfoMoudle>((int)MoudleType.TYPE_PERSONAL_INFO);
                    break;
                case (int)MoudleType.TYPE_ADDRESS_BOOK:
                    GoToMoudle<SMSAddressBookMoudle>((int)MoudleType.TYPE_ADDRESS_BOOK);
                    break;
                case (int)MoudleType.TYPE_MORE_MOMENTS:
                    GoToMoudle<SMSMoreMomentsMoudle>((int)MoudleType.TYPE_MORE_MOMENTS);
                    break;
                default:
                    Debug.Log("Error Cmoe  Form  MoudleType  -> " + SMSDataMgr.Ins.ComeForm);
                    break;
        }
        //}, false);

    }

    /// <summary>
    /// 设置底部选中按钮
    /// </summary>
    /// <param name="index"></param>
    void SelectListBtn(MoudleType moudleType)
    {
        switch (moudleType)
        {
            case MoudleType.TYPE_MAIN:
                btnList.selectedIndex = 0;
                break;
            case MoudleType.TYPE_MOMENTS:
                btnList.selectedIndex = 1;
                break;
            case MoudleType.TYPE_ADDRESS_BOOK:
                btnList.selectedIndex = 2;
                break;
        }
    }

    void GotoPersonalMoments(NormalInfo actorId)
    {
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("actorId", actorId.index);
        GameMonoBehaviour.Ins.RequestInfoPostHaveData<List<PlayerTimeline>>(NetHeaderConfig.CELL_LIST_MOMENT, wWWForm, RequestGotoPersonalMoment, false);
    }

    void RequestGotoPersonalMoment(List<PlayerTimeline> timelines)
    {
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
            GoToMoudle<SMSPersonalMomentsMoudle, List<PlayerTimeline>>((int)MoudleType.TYPE_PERSONAL_MOMENTS, timelines);
        }
        else
        {
            GoToMoudle<SMSKongMoudle, string>((int)MoudleType.TYPE_KONG, ((int)SMSKongMoudle.loaderType.MOMENTS).ToString());
        }
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
            else
                gComponent = ui;
            baseMoudle.baseView = this;
            baseMoudle.InitMoudle(gComponent, index);
        }
        baseMoudle.InitData<D>(data);
        SwitchController(index);
        SelectListBtn(moudleType);
    }

    public override void GoToMoudle<T>(int index)
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
            else
                gComponent = ui;
            baseMoudle.baseView = this;
            baseMoudle.InitMoudle(gComponent, index);
        }
        baseMoudle.InitData();
        SwitchController(index);
        SelectListBtn(moudleType);
    }

    public override void onShow()
    {
        base.onShow();
        ui.alpha = 1;
        ui.visible = true;
        gameObject.SetActive(true);
    }

    public override void onHide()
    {
        base.onHide();
        SMSDataMgr.Ins.IsOnPhone = false;
        StopAllCoroutines();
        SMSDataMgr.Ins.MomentList.Clear();
        ui.visible = false;
        gameObject.SetActive(false);
    }
    public Vector2 GetButtonPos(int index)
    {
        return SearchChild("n" + index).asCom.position;


    }
}



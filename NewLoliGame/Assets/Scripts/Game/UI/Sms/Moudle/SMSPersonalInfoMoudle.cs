using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

public class SMSPersonalInfoMoudle : BaseMoudle
{
    GComponent headIconCom;
    GLoader headIcon;

    GComponent nameCom;
    Controller nameController;
    GTextField nickName;
    GTextField name;

    GProgressBar bar;
    GComponent xin_bar;

    GList btnList;

    Controller callRedpoint;

    List<Role> ownRoles
    {
        get { return GameData.OwnRoleList; }
    }

    int currentType = GameInitCardsConfig.TYPE_ROLE;

    List<string> btnUrlList = new List<string>() {
        "ui://srheglvmam6o3c",
        "ui://srheglvmam6o3a",
    };

    enum BtnType
    {
        STORY,
        GIFT,
    }

    GameInitCardsConfig currentCardsConfig;
    Role role = null;

    public override void InitMoudle(GComponent gComponent, int controllerIndex)
    {
        base.InitMoudle(gComponent, controllerIndex);
        InitUI();
        InitEvent();
    }

    public override void InitUI()
    {
        headIconCom = SearchChild("n22").asCom.GetChild("n5").asCom;
        headIcon = headIconCom.GetChild("n5").asLoader;

        nameCom = SearchChild("n19").asCom;
        nameController = nameCom.GetController("c1");
        nickName = nameCom.GetChild("n19").asTextField;
        name = nameCom.GetChild("n20").asTextField;

        bar = SearchChild("n23").asProgress;
        xin_bar = SearchChild("n25").asCom;
        btnList = SearchChild("n27").asList;
        callRedpoint = SearchChild("n28").asCom.GetController("c1");
    }

    public override void InitData<D>(D data)
    {
        base.InitData(data);
        NormalInfo normalInfo = data as NormalInfo;
        int actorId = normalInfo.index;

        currentCardsConfig = DataUtil.GetGameInitCard(actorId);
        if (ownRoles == null)
        {
            QueryActorListInfo();
            return;
        }
        initInfos();
    }

    public override void InitData()
    {
        base.InitData();
        callRedpoint.selectedIndex = RedpointMgr.Ins.callRedpoint.Contains(currentCardsConfig.card_id) ? 1 : 0;

    }

    public override void InitEvent()
    {
        //修改
        nameCom.GetChild("n21").onClick.Set(() =>
        {
            Extrand extrand = new Extrand();
            extrand.type = ModifyPlayerInfo.MODIFY_ADDRESSBOOK;
            extrand.msg = currentCardsConfig.card_id + "";
            extrand.callBack = initInfos;
            UIMgr.Ins.showNextPopupView<ChangePlayerNameView, Extrand>(extrand);
        });
        //还原
        nameCom.GetChild("n22").onClick.Set(QueryRestoreName);
        //录音
        SearchChild("n28").onClick.Set(OnClickCall);
        //朋友圈
        SearchChild("n29").onClick.Set(() =>
        {
            NormalInfo normalInfo = new NormalInfo();
            normalInfo.index = currentCardsConfig.card_id;
            SMSDataMgr.Ins.ComeForm = (int)SMSView.MoudleType.TYPE_PERSONAL_INFO;
            EventMgr.Ins.DispachEvent(EventConfig.GOTO_PERSONAL_MPMENT, normalInfo);
        });
        //聊天
        SearchChild("n30").onClick.Set(OnClickSms);
    }

    void initInfos()
    {
        role = ownRoles.Find(a => a.id == currentCardsConfig.card_id);

        headIcon.url = UrlUtil.GetStoryHeadIconUrl(currentCardsConfig.card_id);

        name.text = "原名: " + currentCardsConfig.name_cn;
        currentType = currentCardsConfig.type;

        //临时数据
        if (role != null)
        {
            bool isOriginal = role.name.Trim() == (currentCardsConfig.name_cn) || role.name.Trim() == "";
            nameController.selectedIndex = isOriginal ? 0 : 1;
            nickName.text = isOriginal ? currentCardsConfig.name_cn : role.name;
            SMSDataMgr.Ins.SetProgressInfo(bar, xin_bar, role);
            SmsListIndex smsList = SMSDataMgr.Ins.SmsIndexList.Find(a => a.actor_id == currentCardsConfig.card_id);
            if (smsList != null)
            {
                smsList.name = nickName.text;
            }
        }
        else
            nameController.selectedIndex = 0;

        AddBtnItems();
        callRedpoint.selectedIndex = RedpointMgr.Ins.callRedpoint.Contains(currentCardsConfig.card_id) ? 1 : 0;
    }

    void AddBtnItems()
    {
        btnList.RemoveChildrenToPool();

        for (int i = 0; i < btnUrlList.Count; i++)
        {
            GComponent gCom = btnList.AddItemFromPool(btnUrlList[i]).asCom;
        }
        btnList.onClickItem.Set(OnClickBtn);
    }

    void OnClickBtn(EventContext context)
    {
        int selectedIndex = btnList.GetChildIndex((GObject)context.data);
        NormalInfo normalInfo = new NormalInfo();
        //在btnUrlList中的位置
        switch (selectedIndex)
        {
            case 0:
                //故事
                UIMgr.Ins.showNextPopupView<DollStoryView, GameInitCardsConfig>(currentCardsConfig);
                break;
            case 1:
                //送礼
                normalInfo.type = (int)InteractiveView.MoudleType.SEND_GIFTS;
                normalInfo.index = currentCardsConfig.card_id;
                UIMgr.Ins.showNextPopupView<InteractiveView, NormalInfo>(normalInfo);
                SMSView.view.onHide();

                break;
        }
    }

    void QueryActorListInfo()
    {
        WWWForm wWWForm = new WWWForm();
        GameMonoBehaviour.Ins.RequestInfoPostList<Role>(NetHeaderConfig.ACTOR_LIST, wWWForm, () =>
        {
            initInfos();
        });
    }

    //还原
    void QueryRestoreName()
    {
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("actorId", currentCardsConfig.card_id);
        wWWForm.AddField("nickname", currentCardsConfig.name_cn);
        GameMonoBehaviour.Ins.RequestInfoPost<Role>(NetHeaderConfig.ACTOR_EDIT, wWWForm, initInfos);
    }


    void OnClickSms()
    {
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("type", (int)TypeConfig.Consume.Message);
        wWWForm.AddField("actorId", currentCardsConfig.card_id);
        GameMonoBehaviour.Ins.RequestInfoPost<List<SmsListIndex>>(NetHeaderConfig.CELL_LIST_PACK, wWWForm, RequestGotoSmsRecord);

    }

    void RequestGotoSmsRecord(List<SmsListIndex> cellLists)
    {
        SMSDataMgr.Ins.ComeForm = (int)SMSView.MoudleType.TYPE_PERSONAL_INFO;
        if (cellLists != null && cellLists.Count > 0)
            baseView.GoToMoudle<SMSMoreRecordMoudle, List<SmsListIndex>>((int)SMSView.MoudleType.TYPE_MORE_RECORD, cellLists);
        else
            baseView.GoToMoudle<SMSKongMoudle, string>((int)SMSView.MoudleType.TYPE_KONG, ((int)SMSKongMoudle.loaderType.SMS).ToString());

    }

    void OnClickCall()
    {
        Debug.LogError("type: " + TypeConfig.Consume.Mobile);
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("type", (int)TypeConfig.Consume.Mobile);
        wWWForm.AddField("actorId", currentCardsConfig.card_id);
        GameMonoBehaviour.Ins.RequestInfoPost<List<SmsListIndex>>(NetHeaderConfig.CELL_LIST_PACK, wWWForm, RequestGotoCallRecord);
    }

    void RequestGotoCallRecord(List<SmsListIndex> cellLists)
    {
        SMSDataMgr.Ins.ComeForm = (int)SMSView.MoudleType.TYPE_PERSONAL_INFO;
        if (cellLists != null && cellLists.Count > 0)
            baseView.GoToMoudle<SMSMoreCallRecordMoudle, List<SmsListIndex>>((int)SMSView.MoudleType.TYPE_MORE_CALL, cellLists);
        else
            baseView.GoToMoudle<SMSKongMoudle, string>((int)SMSView.MoudleType.TYPE_KONG, ((int)SMSKongMoudle.loaderType.CALL).ToString());
    }

}

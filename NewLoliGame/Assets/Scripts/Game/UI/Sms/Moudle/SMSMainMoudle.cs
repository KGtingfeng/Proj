using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using System;

public class SMSMainMoudle : BaseMoudle
{

    static SMSMainMoudle ins;
    public static SMSMainMoudle Ins
    {
        get
        {
            return ins;
        }
    }
    GList roleList;
    Controller chatController;
    Controller friendController;
    Controller cintactController;
    GList btnList;
    List<SmsListIndex> smsLists;
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
        roleList = SearchChild("n16").asList;
        btnList = SearchChild("n6").asList;
        chatController = btnList.GetChildAt(0).asCom.GetController("c1");
        friendController = btnList.GetChildAt(1).asCom.GetController("c1");
        cintactController = btnList.GetChildAt(2).asCom.GetController("c1");
    }

    public override void InitEvent()
    {
        base.InitEvent();
        EventMgr.Ins.RegisterEvent<SMSInfo>(EventConfig.SMS_ADD_INFO, RefreshRole);
        EventMgr.Ins.RegisterEvent<GameCellSmsConfig>(EventConfig.SMS_CALL_START_MAIN, CallStart);
        EventMgr.Ins.RegisterEvent<GameCellSmsConfig>(EventConfig.SMS_CALL_FINISH_MAIN, CallFinish);
    }

    public override void InitData<D>(D data)
    {
        base.InitData(data);
        smsLists = data as List<SmsListIndex>;
        SMSDataMgr.Ins.SmsIndexList = smsLists;
        SortRole();
        RefreshRoleList();
        if (GameData.isGuider)
        { 
            UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);
        }
    }

    void SortRole()
    {
        smsLists.Sort(delegate (SmsListIndex roleA, SmsListIndex roleB)
        {
            if (GameTool.GetDateTime(roleA.update_time) == GameTool.GetDateTime(roleB.update_time))
                return 0;
            return GameTool.GetDateTime(roleB.update_time).CompareTo(GameTool.GetDateTime(roleA.update_time));
        });
    }

    public override void InitData()
    {
        base.InitData();
        RefreshRoleList();
    }

    void RefreshRoleList()
    {
        roleList.SetVirtual();
        roleList.itemRenderer = ItemRenderer;
        roleList.numItems = smsLists.Count;
        chatController.selectedIndex = RedpointMgr.Ins.smsRedpoint.Count > 0 ? 1 : 0;
        friendController.selectedIndex = RedpointMgr.Ins.timelineRedpoint.Count > 0 ? 1 : 0;
        cintactController.selectedIndex = RedpointMgr.Ins.callRedpoint.Count > 0 ? 1 : 0;
        GameTool.SetListEffectOne(roleList);

    }

    void ItemRenderer(int index, GObject gObject)
    {
        GComponent item = gObject.asCom;
        SmsListIndex info = smsLists[index];
        GComponent gCom = item.GetChild("n0").asCom;
        Controller controller0 = gCom.GetController("c1");
        Controller controller1 = gCom.GetController("c2");

        GTextField nameText = gCom.GetChild("n12").asTextField;
        GTextField content = gCom.GetChild("n13").asTextField;
        GLoader iconLoder = gCom.GetChild("n5").asCom.GetChild("n5").asCom.GetChild("n5").asLoader;
        GTextField favorText = gCom.GetChild("n9").asCom.GetChild("n21").asTextField;
        GObject favorImage = gCom.GetChild("n9").asCom.GetChild("n23").asCom.GetChild("n23");
        GameTool.SetLevelProgressBar(favorImage, 1);
        iconLoder.url = UrlUtil.GetStoryHeadIconUrl(info.actor_id);
        nameText.text = info.name;
        int level = GameTool.FavorLevel(float.Parse(info.favour));
        favorText.text = level.ToString();
        GameTool.SetLevelProgressBar(favorImage, level);
        gCom.onClick.Set(() => { OnClickItem(index); });
        if (string.IsNullOrEmpty(info.cell_node))
        {
            content.text = "";
            controller0.selectedIndex = 0;
            Debug.Log("smsId   " + info.sms_id + "  cell_node  is null!");
            return;
        }

        string[] nodes = info.cell_node.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);

        int pointNode = int.Parse(nodes[nodes.Length - 1]);
        GameSmsNodeConfig gameSmsNode = JsonConfig.GameSmsNodeConfigs.Find(a => a.id == pointNode);
        GameSmsPointConfig gameSmsPoint = JsonConfig.GameSmsPointConfigs.Find(a => a.id == gameSmsNode.point_id);
        GameCellSmsConfig gameCellSms = JsonConfig.GameCellSmsConfigs.Find(a => a.id == gameSmsNode.sms_id);
        string context = "";
        switch (gameCellSms.message_type)
        {
            case (int)TypeConfig.Consume.Mobile:
                controller0.selectedIndex = 1;
                context = "本次通话：" + gameCellSms.title;
                break;
            case (int)TypeConfig.Consume.Video:
                controller0.selectedIndex = 2;
                context = "本次视频：" + gameCellSms.title;
                break;
            case (int)TypeConfig.Consume.Message:
                controller0.selectedIndex = 0;
                if (gameSmsPoint.type == (int)TypeConfig.SMSType.TYPE_Image)
                    context = "【图片】";
                else if (gameSmsPoint.type == (int)TypeConfig.SMSType.TYPE_Hongbao || gameSmsPoint.type == (int)TypeConfig.SMSType.TYPE_AWARD)
                    context = "【红包】";
                else if (gameSmsPoint.type == (int)TypeConfig.SMSType.TYPE_Dialogue)
                    context = gameSmsPoint.content1;
                break;
        }
        context = DataUtil.ReplaceCharacterWithStarts(context);
        context = GameTool.GetCutText(context, 14);
        context = GameTool.Conversion(context);
        content.text = context;
        switch (info.story_status)
        {
            case SmsListIndex.TYPE_READED:
            case SmsListIndex.TYPE_HAVE_DONE:
                controller1.selectedIndex = 0;
                break;
            case SmsListIndex.TYPE_READED_UNDONE:
                controller1.selectedIndex = 1;
                break;
            case SmsListIndex.TYPE_NOT_START:
            case SmsListIndex.TYPE_UNREAD:
                controller1.selectedIndex = 2;
                break;
        }
         
    }

    void OnClickItem(int index)
    {
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("actorId", smsLists[index].actor_id);
        GameMonoBehaviour.Ins.RequestInfoPostHaveData<SmsListIndex>(NetHeaderConfig.CELL_QUERY_MESSAGE, wWWForm, GotoSms);
    }

    public void NewbieGotoSms()
    {
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("actorId", smsLists[0].actor_id);
        GameMonoBehaviour.Ins.RequestInfoPostHaveData<SmsListIndex>(NetHeaderConfig.CELL_QUERY_MESSAGE, wWWForm, GotoSms);
    }

    void GotoSms(SmsListIndex smsList)
    {
        if (smsList != null)
            baseView.GoToMoudle<SMSMoudle, SmsListIndex>((int)SMSView.MoudleType.TYPE_SMS, smsList);
        else
            Debug.Log("SmsListIndex is null !");
    }


    /// <summary>
    /// 聊天或推送更新主界面
    /// </summary>
    /// <param name="info"></param>
    void RefreshRole(SMSInfo info)
    {
        SmsListIndex sms = smsLists.Find(a => a.actor_id == info.gameCellConfig.actor_id);
        if (sms != null)
        {
            if (!string.IsNullOrEmpty(sms.cell_node))
            {
                string[] nodes = sms.cell_node.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (int.Parse(nodes[nodes.Length - 1]) != info.gameNodeConfig.id)
                    InsertSmsToTop(info, sms);
            }
            else
                InsertSmsToTop(info, sms);
        }
        else
        {
            SmsListIndex smsList = new SmsListIndex();
            smsList.sms_id = info.gameCellConfig.id;
            if (info.isRead)
                smsList.story_status = SmsListIndex.TYPE_READED;
            else
                smsList.story_status = SmsListIndex.TYPE_UNREAD;
            smsList.cell_node += info.gameNodeConfig.id;
            smsList.actor_id = info.gameCellConfig.actor_id;
            Role role = GameData.OwnRoleList.Find(a => a.id == info.gameCellConfig.actor_id);
            smsList.favour = role.actorFavorite.ToString();
            smsList.name = role.name;
            smsLists.Insert(0, smsList);
        }
        RefreshRoleList();
    }

    void InsertSmsToTop(SMSInfo info, SmsListIndex sms)
    {
        if (info.isRead)
            sms.story_status = SmsListIndex.TYPE_READED;
        else
            sms.story_status = SmsListIndex.TYPE_UNREAD;
        sms.cell_node += "," + info.gameNodeConfig.id;
        smsLists.Remove(sms);
        smsLists.Insert(0, sms);
    }

    /// <summary>
    /// 电话或视频结束调用
    /// </summary>
    /// <param name="smsNodeConfig"></param>
    void CallFinish(GameCellSmsConfig cellSmsConfig)
    {
        SmsListIndex sms = smsLists.Find(a => a.actor_id == cellSmsConfig.actor_id);
        if (sms != null && !string.IsNullOrEmpty(sms.cell_node))
        {
            string[] nodes = sms.cell_node.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (int.Parse(nodes[nodes.Length - 1]) == cellSmsConfig.startPoint)
                sms.story_status = SmsListIndex.TYPE_HAVE_DONE;
        }
    }

    /// <summary>
    /// 电话或视频开始调用
    /// </summary>
    /// <param name="smsNodeConfig"></param>
    void CallStart(GameCellSmsConfig cellSmsConfig)
    {
        SmsListIndex sms = smsLists.Find(a => a.actor_id == cellSmsConfig.actor_id);
        if (sms != null && sms.story_status == SmsListIndex.TYPE_UNREAD && !string.IsNullOrEmpty(sms.cell_node))
        {
            string[] nodes = sms.cell_node.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (int.Parse(nodes[nodes.Length - 1]) == cellSmsConfig.startPoint)
                sms.story_status = SmsListIndex.TYPE_READED_UNDONE;
        }
    }


    
}

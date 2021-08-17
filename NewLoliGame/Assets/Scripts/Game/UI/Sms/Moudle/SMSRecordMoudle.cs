using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using System;

public class SMSRecordMoudle : BaseMoudle
{
    enum ItemType
    {
        CHAT0_TEXT,
        CHAT_HONGBAO,
        CHAT_IMAGE,
        CHAT_LINE,
        CHAT_TEXT,
        CHAT_RESET,
    }

    readonly Dictionary<ItemType, string> itemUrlPairs = new Dictionary<ItemType, string>()
    {
         { ItemType.CHAT0_TEXT,"ui://srheglvmf8rry"},
         { ItemType.CHAT_HONGBAO,"ui://srheglvmf8rrv"},
         { ItemType.CHAT_IMAGE,"ui://srheglvmf8rru"},
         { ItemType.CHAT_LINE,"ui://srheglvmf8rr11"},
         { ItemType.CHAT_TEXT,"ui://srheglvmzee8n"},
         { ItemType.CHAT_RESET,"ui://srheglvmoi2k3x"},

    };

    GList smsList;
    GTextField nameText;
    List<int> nodeList = new List<int>();
    SmsListIndex smsListIndex;
    GameCellSmsConfig cellSmsConfig;
    public override void InitMoudle(GComponent gComponent, int controllerIndex)
    {
        base.InitMoudle(gComponent, controllerIndex);
        InitUI();
        InitEvent();
    }

    public override void InitUI()
    {
        base.InitUI();
        smsList = SearchChild("n31").asList;
        nameText = SearchChild("n19").asTextField;
    }

    public override void InitData<D>(D data)
    {
        base.InitData(data);
        smsListIndex = data as SmsListIndex;
        string node = smsListIndex.pass_node;
        GetNodeList(node);
        smsList.RemoveChildrenToPool();
        cellSmsConfig = DataUtil.GetGameCellSmsConfig(smsListIndex.sms_id);
        RefreshPoint(cellSmsConfig.startPoint);
        SmsListIndex sms = SMSDataMgr.Ins.SmsIndexList.Find(a => a.actor_id == smsListIndex.actor_id);
        nameText.text = sms.name;
        if (cellSmsConfig.content_type == GameCellSmsConfig.TYPE_DAILY)
        {
            AddListItem(ItemType.CHAT_RESET, null);
        }

    }

    void GetNodeList(string node)
    {
        nodeList.Clear();
        string[] nodes = node.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string i in nodes)
        {
            nodeList.Add(int.Parse(i));
        }
    }

    void RefreshPoint(int node)
    {
        if (node == 0)
            AddListItem(ItemType.CHAT_LINE, null);
        else
        {
            GameSmsNodeConfig nodeConfig = DataUtil.GetGameSmsNodeConfig(node);
            GameSmsPointConfig pointConfig = DataUtil.GetGameSmsPointConfig(nodeConfig.point_id);
            if (pointConfig != null)
            {
                switch (pointConfig.type)
                {
                    case (int)TypeConfig.SMSType.TYPE_Hongbao:
                    case (int)TypeConfig.SMSType.TYPE_Image:
                    case (int)TypeConfig.SMSType.TYPE_Dialogue:
                        ShowPoint(pointConfig);
                        RefreshPoint(pointConfig.point1);
                        break;
                    case (int)TypeConfig.SMSType.TYPE_AWARD:
                        RefreshPoint(pointConfig.point1);
                        break;
                    case (int)TypeConfig.SMSType.TYPE_Select:
                        if (nodeList.Contains(pointConfig.point1))
                            RefreshPoint(pointConfig.point1);
                        else if (nodeList.Contains(pointConfig.point2))
                            RefreshPoint(pointConfig.point2);
                        else
                            RefreshPoint(pointConfig.point3);
                        break;
                }
            }
        }
    }

    void ShowPoint(GameSmsPointConfig currentPoint)
    {
        switch (currentPoint.type)
        {
            case (int)TypeConfig.SMSType.TYPE_Dialogue:
                if (currentPoint.title == "0")
                    AddListItem(ItemType.CHAT0_TEXT, currentPoint);
                else
                    AddListItem(ItemType.CHAT_TEXT, currentPoint);
                break;
            case (int)TypeConfig.SMSType.TYPE_Hongbao:
                AddListItem(ItemType.CHAT_HONGBAO, currentPoint);
                break;
            case (int)TypeConfig.SMSType.TYPE_Image:
                AddListItem(ItemType.CHAT_IMAGE, currentPoint);
                break;
        }
    }

    /// <summary>
    /// 创建一条聊天
    /// </summary>
    /// <param name="type"></param>
    void AddListItem(ItemType type, GameSmsPointConfig currentPoint)
    {
        GComponent gCom = smsList.AddItemFromPool(itemUrlPairs[type]).asCom;
        if (type != ItemType.CHAT_LINE && type != ItemType.CHAT_RESET)
        {
            GLoader iconLoader = gCom.GetChild("n5").asCom.GetChild("n5").asCom.GetChild("n5").asLoader;
            gCom.GetChild("n10").onClick.Clear();
            iconLoader.url = UrlUtil.GetStoryHeadIconUrl(int.Parse(currentPoint.title));
        }
        switch (type)
        {
            case ItemType.CHAT_TEXT:
                GTextField contentText = gCom.GetChild("n2").asTextField;
                contentText.maxWidth = 396;
                string text = DataUtil.ReplaceCharacterWithStarts(currentPoint.content1);
                contentText.text = GameTool.Conversion(text);
                break;
            case ItemType.CHAT0_TEXT:
                GTextField content0Text = gCom.GetChild("n2").asTextField;
                content0Text.maxWidth = 396;
                content0Text.text = GameTool.Conversion(currentPoint.content1);
                Controller controllerText = gCom.GetController("c1");
                controllerText.selectedIndex = 1;
                break;
            case ItemType.CHAT_HONGBAO:
                gCom.GetChild("n9").onClick.Set(() =>
                {
                    UIMgr.Ins.showErrorMsgWindow("该红包已领取！");
                });
                GTextField fromRoleText = gCom.GetChild("n6").asTextField;
                GTextField contentHongbao = gCom.GetChild("n8").asTextField;
                fromRoleText.text = "来自" + JsonConfig.GameInitCardsConfigs.Find(a => a.card_id == int.Parse(currentPoint.title)).name_cn + "的红包";
                string hongbaoText = DataUtil.ReplaceCharacterWithStarts(currentPoint.content1);
                contentHongbao.text = GameTool.Conversion(hongbaoText);
                break;
            case ItemType.CHAT_IMAGE:
                GLoader imageLoader = gCom.GetChild("n6").asCom.GetChild("n5").asLoader;
                imageLoader.url = UrlUtil.GetSmsImageURL(currentPoint.card_id);
                gCom.GetChild("n6").asCom.GetChild("n5").onClick.Set(() =>
                {
                    UIMgr.Ins.showNextPopupView<SMSImagePopView, string>(UrlUtil.GetSmsImageURL(currentPoint.card_id));
                });
                break;
            case ItemType.CHAT_LINE:
                break;
            case ItemType.CHAT_RESET:
                gCom.GetChild("n7").onClick.Set(OnClickReset);
                break;
        }
        smsList.scrollPane.ScrollBottom();
    }

    Extrand extrand;
    void OnClickReset()
    {

        if (extrand == null)
        {
            extrand = new Extrand();
            extrand.type = 1;
            extrand.key = "提示";
            TinyItem item = ItemUtil.GetTinyItem(cellSmsConfig.reset_cost);
            extrand.item = item;
            extrand.msg = "重置短信";
            extrand.callBack = RequestReset;
        }
        UIMgr.Ins.showNextPopupView<PopupDialogView, Extrand>(extrand);
    }

    void RequestReset()
    {

        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("smsId", smsListIndex.sms_id);
        wWWForm.AddField("actorId", smsListIndex.actor_id);
        GameMonoBehaviour.Ins.RequestInfoPost<NormalInfo>(NetHeaderConfig.CELL_SMS_RESET, wWWForm, GotoSms);

    }

    void GotoSms()
    {
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("actorId", smsListIndex.actor_id);
        GameMonoBehaviour.Ins.RequestInfoPostHaveData<SmsListIndex>(NetHeaderConfig.CELL_QUERY_MESSAGE, wWWForm, CallbackSms);
    }

    void CallbackSms(SmsListIndex smsList)
    {
        baseView.GoToMoudle<SMSMoudle, SmsListIndex>((int)SMSView.MoudleType.TYPE_SMS, smsList);
    }
}

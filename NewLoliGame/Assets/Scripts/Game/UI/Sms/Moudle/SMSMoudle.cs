using System;
using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

public class SMSMoudle : BaseMoudle
{

    static SMSMoudle ins;
    public static SMSMoudle Ins
    {
        get
        {
            return ins;
        }
    }

    enum ItemType
    {
        //我-电话
        CHAT0_CALL,
        //我-文本
        CHAT0_TEXT,
        //我-视频
        CHAT0_VIDEO,
        //角色-电话
        CHAT_CALL,
        //角色-红包
        CHAT_HONGBAO,
        //角色-图片
        CHAT_IMAGE,
        //线，短信包结束
        CHAT_LINE,
        //角色-文本
        CHAT_TEXT,
        //角色-视频
        CHAT_VIDEO,
    }

    readonly Dictionary<ItemType, string> itemUrlPairs = new Dictionary<ItemType, string>()
    {
         { ItemType.CHAT0_CALL,"ui://srheglvmf8rrz"},
         { ItemType.CHAT0_TEXT,"ui://srheglvmf8rry"},
         { ItemType.CHAT0_VIDEO,"ui://srheglvmf8rr10"},
         { ItemType.CHAT_CALL,"ui://srheglvmf8rrs"},
         { ItemType.CHAT_HONGBAO,"ui://srheglvmf8rrv"},
         { ItemType.CHAT_IMAGE,"ui://srheglvmf8rru"},
         { ItemType.CHAT_LINE,"ui://srheglvmf8rr11"},
         { ItemType.CHAT_TEXT,"ui://srheglvmzee8n"},
         { ItemType.CHAT_VIDEO,"ui://srheglvmf8rrt"},
    };
    GButton btnMore;
    Controller moreRedpoint;
    Controller bottomController;
    GList smsList;
    GTextField sendText;
    GList sendList;
    GTextField nameText;
    GameSmsPointConfig currentSmsPoint;
    GameSmsNodeConfig currentSmsNode;
    SmsListIndex info;
    SmsListIndex mainInfo;
    GButton buySms;
    GButton buyCall;

    bool IsSend;

    List<int> nodeList = new List<int>();
    List<int> passNodeList = new List<int>();

    WaitForSeconds sortWait = new WaitForSeconds(0.2f);
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
        btnMore = SearchChild("n21").asButton;
        btnMore.GetController("c1").selectedIndex = 1;
        sendText = SearchChild("n41").asTextField;
        smsList = SearchChild("n31").asList;
        sendList = SearchChild("n36").asList;
        nameText = SearchChild("n19").asTextField;
        buySms = SearchChild("n43").asButton;
        buySms.GetController("c1").selectedIndex = 1;

        buyCall = SearchChild("n42").asButton;
        buyCall.GetController("c1").selectedIndex = 1;
        bottomController = SearchChild("n32").asButton.GetController("c1");
        moreRedpoint = btnMore.GetController("c1");
    }

    public override void InitEvent()
    {
        base.InitEvent();
        //底部
        SearchChild("n32").onClick.Set(OnClickBottom);
        //发送
        SearchChild("n34").onClick.Set(OnClickSend);
        smsList.onClick.Set(() =>
        {
            if (baseView.controller.selectedIndex == 2 || baseView.controller.selectedIndex == 3)
            {
                baseView.controller.selectedIndex = 1;
                if (IsSend)
                    bottomController.selectedIndex = 1;
                ScreenToBottom();
            }
        });
        buyCall.onClick.Set(() => { OnClickBuy(TypeConfig.Consume.Mobile); });
        buySms.onClick.Set(() => { OnClickBuy(TypeConfig.Consume.Message); });

        EventMgr.Ins.RegisterEvent(EventConfig.SMS_MOUDLE_LEAVE, LeaveSmsMoudle);
        EventMgr.Ins.RegisterEvent<NormalInfo>(EventConfig.SMS_REFRESH_REDBAO, RefreshRedbao);
        EventMgr.Ins.RegisterEvent<SMSInfo>(EventConfig.SMS_PUSH_REFRESH, SmsPushRefresh);
        EventMgr.Ins.RegisterEvent<SMSInfo>(EventConfig.SMS_CALL_PUSH_REFRESH, CallPushRefresh);
    }

    public override void InitData<D>(D data)
    {
        btnMore.onClick.Set(GotoMoreMoudle);
        base.InitData(data);
        info = data as SmsListIndex;
        SMSDataMgr.Ins.CurrentRole = info.actor_id;
        baseView.StopCoroutine("ShowMessageEffect");
        smsList.RemoveChildrenToPool();
        bottomController.selectedIndex = 0;
        SMSDataMgr.Ins.IsOnSms = true;
        mainInfo = SMSDataMgr.Ins.SmsIndexList.Find(a => a.actor_id == info.actor_id);
        //刷新主界面状态
        mainInfo.story_status = info.story_status;
        mainInfo.cell_node = info.cell_node;
        nameText.text = mainInfo.name;
        GetNodeList(info.pass_node, passNodeList);
        GetNodeList(info.cell_node, nodeList);
        HistoryRecord();
        InitNode();

        moreRedpoint.selectedIndex = RedpointMgr.Ins.callRedpoint.Contains(info.actor_id) ? 1 : 0;
        if (GameData.isGuider)
        {
            UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);
        }

    }

    /// <summary>
    /// 初始化节点
    /// </summary>
    void InitNode()
    {
        currentSmsNode = null;
        currentSmsPoint = null;
        GameCellSmsConfig smsConfig = DataUtil.GetGameCellSmsConfig(info.sms_id);
        //不是短信
        if (smsConfig.message_type != (int)TypeConfig.Consume.Message)
        {
            Debug.LogError("cell type is error!  smsId = " + smsConfig.id);
            return;
        }
        if (info.story_status != SmsListIndex.TYPE_HAVE_DONE)
        {
            GetLastNode();
            //如果cell_node没有任何该smsid下的node
            if (currentSmsNode == null)
            {
                currentSmsNode = DataUtil.GetGameSmsNodeConfig(smsConfig.startPoint);
                currentSmsPoint = DataUtil.GetGameSmsPointConfig(currentSmsNode.point_id);
                NextPointSelect();
            }
            else
            {
                //剧情没有结束
                if (currentSmsPoint.point1 != 0)
                {
                    if (currentSmsPoint.type == (int)TypeConfig.SMSType.TYPE_Hongbao)
                    {
                        string[] awards = info.award_node.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        List<string> awardList = new List<string>(awards);
                        if (awardList.Find(a => a == currentSmsPoint.point1.ToString()) != null)
                        {
                            GameSmsNodeConfig n = DataUtil.GetGameSmsNodeConfig(currentSmsPoint.point1);
                            GameSmsPointConfig p = DataUtil.GetGameSmsPointConfig(n.point_id);
                            currentSmsNode = DataUtil.GetGameSmsNodeConfig(p.point1);
                            currentSmsPoint = DataUtil.GetGameSmsPointConfig(currentSmsNode.point_id);
                            NextPointSelect();
                        }
                    }
                    else
                        NextPointSelect();
                }
                else
                {
                    currentSmsPoint = null;
                    currentSmsNode = null;
                    RequestSave(null, 0);
                }
            }
        }
    }

    /// <summary>
    /// 找到最后一个节点
    /// </summary>
    void GetLastNode()
    {
        for (int i = nodeList.Count - 1; i >= 0; i--)
        {
            GameSmsNodeConfig n = DataUtil.GetGameSmsNodeConfig(nodeList[i]);
            if (n.sms_id == info.sms_id)
            {
                GameSmsPointConfig p = DataUtil.GetGameSmsPointConfig(n.point_id);
                if (p.point1 == 0)
                {
                    currentSmsNode = n;
                    currentSmsPoint = p;
                }
                else
                {
                    if (p.type == (int)TypeConfig.SMSType.TYPE_Hongbao)
                    {
                        currentSmsNode = n;
                        currentSmsPoint = p;
                    }
                    else
                    {
                        currentSmsNode = DataUtil.GetGameSmsNodeConfig(p.point1);
                        currentSmsPoint = DataUtil.GetGameSmsPointConfig(currentSmsNode.point_id);
                    }
                }
                break;
            }
        }
    }

    #region 走剧情
    /// <summary>
    /// 保存节点
    /// </summary>
    void SavePoint(int point)
    {
        if (!SMSDataMgr.Ins.IsOnSms)
            return;
        RefreshMain();
        //单机测试
        //RefreshPoint(point);
        RequestSave((SmsSave smsList) =>
        {
            if (smsList == null)
                return;
            //发放奖励
            AwardInfo info = new AwardInfo();
            info.award = smsList.award;
            info.extra = smsList.extra;
            GameTool.GetAwards(info);
            RefreshPoint(point);
        }, point);
    }

    void RequestSave(Action<SmsSave> callback, int nodeId)
    {
        Debug.Log("********************sms  set nodeId = " + nodeId + "   smsId  =" + info.sms_id);
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("actorId", SMSDataMgr.Ins.CurrentRole);
        wWWForm.AddField("smsId", info.sms_id);
        wWWForm.AddField("nodeId", nodeId);
        GameMonoBehaviour.Ins.RequestInfoPostHaveData(NetHeaderConfig.CELL_SET_STEP, wWWForm, callback, false);
    }

    /// <summary>
    /// set 0时如果有新消息
    /// </summary>
    void RequestNew(SmsSave sms)
    {
        WWWForm wWWForm = new WWWForm();
        GameMonoBehaviour.Ins.RequestInfoPostHaveData<List<SmsListIndex>>(NetHeaderConfig.CELL_LIST_INDEX, wWWForm, callbackGetNew, false);
    }

    void callbackGetNew(List<SmsListIndex> smsLists)
    {
        if (mainInfo == null)
        {
            Debug.Log(" mainInfo  is null! ");
            return;
        }
        mainInfo = smsLists.Find(a => a.actor_id == mainInfo.actor_id);
        if (mainInfo != null && mainInfo.story_status != SmsListIndex.TYPE_HAVE_DONE && info.sms_id != mainInfo.sms_id)
        {
            info.sms_id = mainInfo.sms_id;
            GameCellSmsConfig gameCell = DataUtil.GetGameCellSmsConfig(mainInfo.sms_id);
            currentSmsNode = DataUtil.GetGameSmsNodeConfig(gameCell.startPoint);
            currentSmsPoint = DataUtil.GetGameSmsPointConfig(currentSmsNode.point_id);
            NextPointSelect();
        }
    }

    /// <summary>
    /// 刷新为下一个剧情节点
    /// </summary>
    void RefreshPoint(int point)
    {
        if (!SMSDataMgr.Ins.IsOnSms)
            return;
        GameSmsNodeConfig nodeConfig = DataUtil.GetGameSmsNodeConfig(point);
        GameSmsPointConfig pointConfig = DataUtil.GetGameSmsPointConfig(nodeConfig.point_id);
        //剧情结束
        if (pointConfig.point1 == 0)
        {
            AddListItem(ItemType.CHAT_LINE);
            currentSmsPoint = null;
            currentSmsNode = null;
            RequestSave(RequestNew, 0);
        }
        else
        {
            currentSmsNode = DataUtil.GetGameSmsNodeConfig(pointConfig.point1);
            currentSmsPoint = DataUtil.GetGameSmsPointConfig(currentSmsNode.point_id);
            NextPointSelect();
        }
    }

    void NextPointSelect()
    {
        switch (currentSmsPoint.type)
        {
            case (int)TypeConfig.SMSType.TYPE_Dialogue:
                ShowDialogue(ItemType.CHAT_TEXT);
                break;
            case (int)TypeConfig.SMSType.TYPE_Select:
                ShowSelect();
                break;
            case (int)TypeConfig.SMSType.TYPE_Hongbao:
                ShowDialogue(ItemType.CHAT_HONGBAO);
                break;
            case (int)TypeConfig.SMSType.TYPE_Image:
                ShowDialogue(ItemType.CHAT_IMAGE);
                break;
        }
    }

    void ShowDialogue(ItemType type)
    {
        if (currentSmsPoint.title == "0")
        {
            GComponent gCom = AddListItem(ItemType.CHAT0_TEXT);
            gCom.GetController("c1").selectedIndex = 1;
            SavePoint(currentSmsNode.id);
        }
        else
        {
            GComponent item = smsList.AddItemFromPool(itemUrlPairs[ItemType.CHAT_TEXT]).asCom;
            baseView.StartCoroutine(ShowWaitTextEffect(item, type));
        }
    }

    /// <summary>
    /// 文字正在输入中
    /// </summary>
    IEnumerator ShowWaitTextEffect(GComponent gCom, ItemType type)
    {
        smsList.scrollPane.ScrollBottom();
        GTextField gText = gCom.GetChild("n2").asTextField;
        GLoader iconLoader = gCom.GetChild("n5").asCom.GetChild("n5").asCom.GetChild("n5").asLoader;
        iconLoader.url = UrlUtil.GetStoryHeadIconUrl(int.Parse(currentSmsPoint.title));

        int times;
        if (type == ItemType.CHAT_TEXT)
            times = currentSmsPoint.content1.Length / 2;
        else
            times = 6;
        for (int i = 0; i < times; i++)
        {
            if (i % 6 == 0)
                gText.text = "正在输入中·     ";
            else
                gText.text = ReplaceFirst(gText.text);
            yield return sortWait;
        }
        smsList.RemoveChildToPool(gCom);
        AddListItem(type);
        SavePoint(currentSmsNode.id);
    }

    /// <summary>
    /// 置换第一个空格为"·"
    /// </summary>
    string ReplaceFirst(string text)
    {
        int index = text.IndexOf(" ");
        return text.Substring(0, index) + "·" + text.Substring(index + 1);
    }

    void ShowSelect()
    {
        IsSend = true;
        bottomController.selectedIndex = 1;
        sendList.selectedIndex = -1;
        sendText.text = "";
        GObject[] lists = sendList.GetChildren();
        sendList.onClickItem.Set(OnClickSendItem);
        SetSelectItem(lists[0].asCom, currentSmsPoint.content1);
        if (!string.IsNullOrEmpty(currentSmsPoint.content2))
            SetSelectItem(lists[1].asCom, currentSmsPoint.content2);
        else
            lists[1].visible = false;
        if (!string.IsNullOrEmpty(currentSmsPoint.content3))
            SetSelectItem(lists[2].asCom, currentSmsPoint.content3);
        else
            lists[2].visible = false;
    }

    void SetSelectItem(GComponent gComponent, string content)
    {
        gComponent.GetController("button").selectedIndex = 0;
        gComponent.visible = true;
        string context = DataUtil.ReplaceCharacterWithStarts(content);
        context = GameTool.GetCutText(context, 15);
        gComponent.asButton.title = GameTool.Conversion(context);

    }

    /// <summary>
    /// 选择
    /// </summary>
    void OnClickSendItem()
    {
        string send = "";
        switch (sendList.selectedIndex)
        {
            case 0:
                send = currentSmsPoint.content1;
                break;
            case 1:
                send = currentSmsPoint.content2;
                break;
            case 2:
                send = currentSmsPoint.content3;
                break;
        }
        send = GameTool.GetCutText(send, 16);
        sendText.text = GameTool.Conversion(send);
    }

    /// <summary>
    /// 发送
    /// </summary>
    public void OnClickSend()
    {
        if (sendList.selectedIndex == -1)
        {
            UIMgr.Ins.showErrorMsgWindow("请先选择要说的话");
            return;
        }
        int point = currentSmsPoint.point1;
        switch (sendList.selectedIndex)
        {
            case 1:
                point = currentSmsPoint.point2;
                break;
            case 2:
                point = currentSmsPoint.point3;
                break;
        }
        baseView.controller.selectedIndex = 1;
        IsSend = false;
        bottomController.selectedIndex = 0;
        ScreenToBottom();
        currentSmsNode = JsonConfig.GameSmsNodeConfigs.Find(a => a.id == point);
        currentSmsPoint = JsonConfig.GameSmsPointConfigs.Find(a => a.id == currentSmsNode.point_id);
        NextPointSelect();
    }
    #endregion

    #region 历史记录
    void HistoryRecord()
    {
        foreach (int id in nodeList)
        {
            GameSmsNodeConfig gameSmsNode = DataUtil.GetGameSmsNodeConfig(id);
            GameSmsPointConfig gameSmsPoint = DataUtil.GetGameSmsPointConfig(gameSmsNode.point_id);
            GameCellSmsConfig gameCellSms = DataUtil.GetGameCellSmsConfig(gameSmsNode.sms_id);
            if (gameCellSms.message_type == (int)TypeConfig.Consume.Mobile)
                gameSmsPoint.type = (int)TypeConfig.Consume.Mobile;
            else if (gameCellSms.message_type == (int)TypeConfig.Consume.Video)
                gameSmsPoint.type = (int)TypeConfig.Consume.Video;
            NextHistoryPointSelect(gameSmsPoint);
            if (gameCellSms.message_type == (int)TypeConfig.Consume.Message && gameSmsPoint.point1 == 0)
                AddListItem(ItemType.CHAT_LINE, gameSmsPoint);
        }
    }

    void NextHistoryPointSelect(GameSmsPointConfig gameSmsPoint)
    {
        switch (gameSmsPoint.type)
        {
            case (int)TypeConfig.SMSType.TYPE_Dialogue:
                if (gameSmsPoint.title == "0")
                    AddListItem(ItemType.CHAT0_TEXT, gameSmsPoint, true);
                else
                    AddListItem(ItemType.CHAT_TEXT, gameSmsPoint);
                break;
            case (int)TypeConfig.SMSType.TYPE_Hongbao:
                AddListItem(ItemType.CHAT_HONGBAO, gameSmsPoint);
                break;
            case (int)TypeConfig.SMSType.TYPE_Image:
                AddListItem(ItemType.CHAT_IMAGE, gameSmsPoint);
                break;
            case (int)TypeConfig.Consume.Video:
                if (gameSmsPoint.title == "0")
                    AddListItem(ItemType.CHAT0_VIDEO, gameSmsPoint, true);
                else
                    AddListItem(ItemType.CHAT_VIDEO, gameSmsPoint);
                break;
            case (int)TypeConfig.Consume.Mobile:
                if (gameSmsPoint.title == "0")
                    AddListItem(ItemType.CHAT0_CALL, gameSmsPoint, true);
                else
                    AddListItem(ItemType.CHAT_CALL, gameSmsPoint);
                break;
        }
    }

    void GetNodeList(string nodeStr, List<int> list)
    {
        list.Clear();
        string[] nodes = nodeStr.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string i in nodes)
            list.Add(int.Parse(i));
    }


    #endregion

    #region 创建一条聊天
    /// <summary>
    /// 创建一条聊天
    /// </summary>
    GComponent AddListItem(ItemType type, GameSmsPointConfig gameSmsPoint = null, bool isReaded = false)
    {
        GameSmsPointConfig currentPoint;
        if (gameSmsPoint == null)
            currentPoint = DataUtil.GetGameSmsPointConfig(currentSmsPoint.id);
        else
            currentPoint = DataUtil.GetGameSmsPointConfig(gameSmsPoint.id);
        GComponent gCom = smsList.AddItemFromPool(itemUrlPairs[type]).asCom;
        //设置头像
        if (type != ItemType.CHAT_LINE)
        {
            GLoader iconLoader;
            iconLoader = gCom.GetChild("n5").asCom.GetChild("n5").asCom.GetChild("n5").asLoader;
            string url;
            if (currentPoint.title == "0")
                url = UrlUtil.GetStoryHeadIconUrl(0);
            else
            {
                url = UrlUtil.GetStoryHeadIconUrl(int.Parse(currentPoint.title));
                gCom.GetChild("n10").onClick.Set(() => { GotoRoleInfo(int.Parse(currentPoint.title)); });
            }
            iconLoader.url = url;
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
                SetReaded(gCom, isReaded);
                break;
            case ItemType.CHAT0_CALL:
                SetCall(gCom, currentPoint);
                SetReaded(gCom, isReaded);
                break;
            case ItemType.CHAT_CALL:
                SetCall(gCom, currentPoint);
                break;
            case ItemType.CHAT0_VIDEO:
                SetVideo(gCom, currentPoint);
                SetReaded(gCom, isReaded);
                break;
            case ItemType.CHAT_VIDEO:
                SetVideo(gCom, currentPoint);
                break;
            case ItemType.CHAT_HONGBAO:
                GameSmsNodeConfig currentNode = DataUtil.GetGameSmsNodeConfig(currentPoint.id);
                gCom.GetChild("n9").onClick.Set(() =>
                {
                    OnClickHongbao(currentNode, currentPoint);
                });
                GTextField fromRoleText = gCom.GetChild("n6").asTextField;
                GTextField contentHongbao = gCom.GetChild("n8").asTextField;
                fromRoleText.text = "来自" + JsonConfig.GameInitCardsConfigs.Find(a => a.card_id == int.Parse(currentPoint.title)).name_cn + "的红包";
                contentHongbao.text = currentPoint.content1;
                break;
            case ItemType.CHAT_IMAGE:
                GLoader imageLoader = gCom.GetChild("n6").asCom.GetChild("n5").asLoader;
                string url = UrlUtil.GetSmsImageURL(currentPoint.card_id);
                imageLoader.url = url;
                gCom.GetChild("n6").asCom.GetChild("n5").onClick.Set(() =>
                {
                    UIMgr.Ins.showNextPopupView<SMSImagePopView, string>(url);
                });
                break;
        }
        smsList.scrollPane.ScrollBottom();
        return gCom;
    }
    /// <summary>
    /// 设置电话
    /// </summary>
    void SetCall(GComponent gCom, GameSmsPointConfig gameSmsPoint)
    {
        GTextField contentCall = gCom.GetChild("n2").asTextField;
        GameSmsNodeConfig gameSmsNode = JsonConfig.GameSmsNodeConfigs.Find(a => a.id == gameSmsPoint.id);
        contentCall.text = "本次通话：" + JsonConfig.GameCellSmsConfigs.Find(a => a.id == gameSmsNode.sms_id).title;
        gCom.GetChild("n1").onClick.Set(() =>
        {
            OnClickCall(gameSmsNode);
        });
    }
    /// <summary>
    /// 设置视频
    /// </summary>
    void SetVideo(GComponent gCom, GameSmsPointConfig gameSmsPoint)
    {
        GTextField contentVideo = gCom.GetChild("n2").asTextField;
        GameSmsNodeConfig gameSmsNode0 = JsonConfig.GameSmsNodeConfigs.Find(a => a.id == gameSmsPoint.id);
        contentVideo.text = "本次视频：" + JsonConfig.GameCellSmsConfigs.Find(a => a.id == gameSmsNode0.sms_id).title;
        gCom.GetChild("n1").onClick.Set(() =>
        {
            OnClickCall(gameSmsNode0);
        });
    }
    /// <summary>
    /// 设置是否已读
    /// </summary>
    void SetReaded(GComponent gCom, bool isReaded)
    {
        Controller controllerText = gCom.GetController("c1");
        controllerText.selectedIndex = 0;
        if (isReaded)
            controllerText.selectedIndex = 1;
    }

    void OnClickHongbao(GameSmsNodeConfig nodeConfig, GameSmsPointConfig pointConfig)
    {
        string[] nodes = info.award_node.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        List<string> nodeLists = new List<string>(nodes);
        if (nodeLists.Count == 0 || nodeLists.Find(a => a == pointConfig.point1.ToString()) == null)
        {
            info.award_node += "," + pointConfig.point1;
            UIMgr.Ins.showNextPopupView<SMSRedBaoView, GameSmsNodeConfig>(nodeConfig);
        }
        else
            UIMgr.Ins.showErrorMsgWindow("该红包已领取");
    }

    void RefreshRedbao(NormalInfo point)
    {
        SavePoint(point.index);
    }

    void OnClickCall(GameSmsNodeConfig smsNodeConfig)
    {
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("smsId", smsNodeConfig.sms_id);
        wWWForm.AddField("type ", (int)TypeConfig.Consume.Mobile);
        GameMonoBehaviour.Ins.RequestInfoPost<List<SmsListIndex>>(NetHeaderConfig.CELL_QUERY_VOICE, wWWForm, GotoPlayCallLog);
    }

    /// <summary>
    /// 播放历史记录
    /// </summary>
    void GotoPlayCallLog(List<SmsListIndex> smsLists)
    {
        if (smsLists != null && smsLists.Count > 0)
        {
            CallDataMgr.Ins.isViewSms = true;
            CallDataMgr.Ins.isCalling = false;
            UIMgr.Ins.showNextPopupView<CallInterfaceView, SmsListIndex>(smsLists[0]);
        }
    }
    #endregion

    Extrand commonTipsInfo;
    void OnClickBuy(TypeConfig.Consume type)
    {
        if (commonTipsInfo == null)
        {
            commonTipsInfo = new Extrand();
            commonTipsInfo.key = "提示";
            commonTipsInfo.type = 1;
        }
        GameConsumeConfig consumeConfig = JsonConfig.GameConsumeConfigs.Find(a => a.type == (int)type);
        TinyItem item = ItemUtil.GetTinyItem(consumeConfig.pay);
        commonTipsInfo.item = item;
        commonTipsInfo.msg = "开启更多的话题？";

        commonTipsInfo.callBack = () => { BuyPack(type); };
        UIMgr.Ins.showNextPopupView<PopupDialogView, Extrand>(commonTipsInfo);
    }

    void BuyPack(TypeConfig.Consume type)
    {
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("actorId", SMSDataMgr.Ins.CurrentRole);
        wWWForm.AddField("type", (int)type);
        GameMonoBehaviour.Ins.RequestInfoPost<MessageBuy>(NetHeaderConfig.CELL_MESSAGE_BUY, wWWForm, RequestBuyPackSuccess);
    }

    void RequestBuyPackSuccess(MessageBuy info)
    {
        baseView.controller.selectedIndex = 1;
        UIMgr.Ins.showErrorMsgWindow(MsgException.BUY_SUCCESSFULLY);
        WWWForm wWWForm = new WWWForm();
        GameMonoBehaviour.Ins.RequestInfoPost<PushInfo>(NetHeaderConfig.CELL_CHECK_MESSAGE, wWWForm, null, false);

    }

    /// <summary>
    /// 点击输入框
    /// </summary>
    public void OnClickBottom()
    {
        if (IsSend)
        {
            baseView.controller.selectedIndex = 2;
            bottomController.selectedIndex = 0;
        }
        else if (currentSmsPoint == null)
            baseView.controller.selectedIndex = 3;
        ScreenToBottom();

    }

    void ScreenToBottom()
    {
        if (smsList.numChildren > 0)
            smsList.ScrollToView(smsList.numChildren - 1, true);
        //list有缓动，等动效结束再将list滑动到底部
        smsList.onGearStop.Set(() =>
        {
            if (smsList.numChildren > 0)
                smsList.ScrollToView(smsList.numChildren - 1, true);
        });
    }

    /// <summary>
    /// 离开聊天界面
    /// </summary>
    void LeaveSmsMoudle()
    {
        SMSDataMgr.Ins.IsOnSms = false;
        IsSend = false;
        SMSView.view.StopAllCoroutines();
        ShowPopMessage(currentSmsPoint);
    }

    /// <summary>
    /// 如果娃娃有未完成对话，在主界面继续播
    /// </summary>
    public void ShowPopMessage(GameSmsPointConfig pointConfig)
    {
        if (pointConfig == null)
            return;
        if (pointConfig.type != (int)TypeConfig.SMSType.TYPE_Select && pointConfig.type != (int)TypeConfig.SMSType.TYPE_AWARD)
        {
            if (pointConfig.title != "0")
                SMSView.view.StartCoroutine(ShowMessageEffect(pointConfig));
            else
                SMSView.view.StartCoroutine(ShowMessageEffect(JsonConfig.GameSmsPointConfigs.Find(a => a.id == pointConfig.point1)));
        }
    }


    IEnumerator ShowMessageEffect(GameSmsPointConfig pointConfig)
    {
        GameSmsNodeConfig nodeConfig = JsonConfig.GameSmsNodeConfigs.Find(a => a.id == pointConfig.id);
        GameCellSmsConfig smsConfig = JsonConfig.GameCellSmsConfigs.Find(a => a.id == nodeConfig.sms_id);
        if (!SMSDataMgr.Ins.IsOnSms || (SMSDataMgr.Ins.IsOnSms && SMSDataMgr.Ins.CurrentRole != smsConfig.actor_id))
        {
            float wait = 0;
            switch (pointConfig.type)
            {
                case (int)TypeConfig.SMSType.TYPE_Hongbao:
                case (int)TypeConfig.SMSType.TYPE_Image:
                    wait = 1f;
                    break;
                case (int)TypeConfig.SMSType.TYPE_Dialogue:
                    wait = pointConfig.content1.Length * 0.1f;
                    break;
            }
            yield return new WaitForSeconds(wait);
            SMSInfo info = new SMSInfo(pointConfig, nodeConfig, smsConfig);
            EventMgr.Ins.DispachEvent(EventConfig.SMS_ADD_INFO, info);

            if (pointConfig.point1 != 0)
                ShowPopMessage(JsonConfig.GameSmsPointConfigs.Find(a => a.id == pointConfig.point1));
        }
    }
    /// <summary>
    /// 刷新主界面
    /// </summary>
    void RefreshMain()
    {
        GameCellSmsConfig smsConfig = DataUtil.GetGameCellSmsConfig(currentSmsNode.sms_id);
        SMSInfo info = new SMSInfo(currentSmsPoint, currentSmsNode, smsConfig, true);
        EventMgr.Ins.DispachEvent(EventConfig.SMS_ADD_INFO, info);
    }

    /// <summary>
    /// 前往个人信息
    /// </summary>
    void GotoRoleInfo(int actorId)
    {
        SMSDataMgr.Ins.Moudle = SMSView.MoudleType.TYPE_SMS;
        NormalInfo normalInfo = new NormalInfo();
        normalInfo.index = actorId;
        baseView.GoToMoudle<SMSPersonalInfoMoudle, NormalInfo>((int)SMSView.MoudleType.TYPE_PERSONAL_INFO, normalInfo);
    }

    /// <summary>
    /// 短信推送
    /// </summary>
    void SmsPushRefresh(SMSInfo info)
    {
        this.info.sms_id = info.gameNodeConfig.sms_id;
        currentSmsNode = info.gameNodeConfig;
        currentSmsPoint = info.gamePointConfig;
        NextPointSelect();
    }

    /// <summary>
    /// 电话推送
    /// </summary>
    void CallPushRefresh(SMSInfo info)
    {
        string[] nodes = this.info.pass_node.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        //如果已经接过推送
        if (nodes.Length > 0 && int.Parse(nodes[nodes.Length - 1]) == info.gameNodeConfig.id)
            return;
        if (info.gameCellConfig.message_type == (int)TypeConfig.Consume.Mobile)
        {
            if (info.gamePointConfig.title == "0")
                AddListItem(ItemType.CHAT0_CALL, info.gamePointConfig);
            else
                AddListItem(ItemType.CHAT_CALL, info.gamePointConfig);
        }
        else if (info.gameCellConfig.message_type == (int)TypeConfig.Consume.Video)
        {
            if (info.gamePointConfig.title == "0")
                AddListItem(ItemType.CHAT0_VIDEO, info.gamePointConfig);
            else
                AddListItem(ItemType.CHAT_VIDEO, info.gamePointConfig);
        }
        this.info.pass_node += "," + info.gameNodeConfig.id;
        EventMgr.Ins.DispachEvent(EventConfig.SMS_ADD_INFO, info);
    }

    public void GotoMoreMoudle()
    {
        LeaveSmsMoudle();
        baseView.GoToMoudle<SMSMoreMoudle>((int)SMSView.MoudleType.TYPE_MORE);
    }

    /// <summary>
    /// 新手选择对话
    /// </summary>
    public void NewbieChoose()
    {
        sendList.selectedIndex = 0;
        OnClickSendItem();
    }

}

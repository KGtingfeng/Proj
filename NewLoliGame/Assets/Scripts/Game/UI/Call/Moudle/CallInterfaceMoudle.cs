using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using System;

/// <summary>
/// 通话界面
/// </summary>
public class CallInterfaceMoudle : BaseMoudle
{
    public static CallInterfaceMoudle ins;
    enum TYPE
    {
        INCOMINGCALL = 0,
        CALL,
        SELECT,
        DIALOGUE
    }

    enum SmsPointType
    {
        Common = 1,
        Select
    }

    TypingEffect typingEffect;

    GameSmsPointConfig gameSmsPointConfig;

    Controller controller;
    GLoader bgLoader;
    GTextField nameTextField;
    GList selectBtnList;

    GComponent dialogueCom;
    GTextField dialogueName;
    GTextField dialogueInfo;

    bool isGoNextNode;
    //点击次数
    public int autoClickTime = 0;

    CallDataMgr callDataMgr
    {
        get { return CallDataMgr.Ins; }
    }

    public int currentNodeId = 0;

    SmsListIndex smsListIndex;

    public override void InitMoudle(GComponent gComponent, int controllerIndex)
    {
        base.InitMoudle(gComponent, controllerIndex);
        InitUI();
        InitEvent();
        ins = this;
    }

    public override void InitUI()
    {
        controller = ui.GetController("c1");
        selectBtnList = SearchChild("n13").asList;
        bgLoader = SearchChild("n1").asLoader;
        nameTextField = SearchChild("n5").asTextField;

        dialogueCom = SearchChild("n17").asCom;
        dialogueName = dialogueCom.GetChild("n3").asTextField;

        dialogueInfo = dialogueCom.GetChild("n4").asTextField;
        typingEffect = new TypingEffect(dialogueInfo);

        InitEvent();
    }

    public override void InitData<D>(D data)
    {
        smsListIndex = data as SmsListIndex;

        autoClickTime = 0;
        //speed = 1.5f;
        //暂时仅处理聊天部分逻辑(聊天、回看)
        GetBg();

    }

    public override void InitEvent()
    {
        base.InitEvent();
        SearchChild("n1").onClick.Set(() =>
        {
            onComplete = null;
            if (controller.selectedIndex != (int)TYPE.CALL && controller.selectedIndex != (int)TYPE.INCOMINGCALL)
                GoToNextNode();
        });

        SearchChild("n3").onClick.Set(() =>
        {
            CallView.view.ReceiveCall(currentNodeId, GetPassNodeInfo);
            controller.selectedIndex = (int)TYPE.DIALOGUE;
            GoToNextNode();
        });

        SearchChild("n4").onClick.Set(ClickEndCall);

        SearchChild("n10").onClick.Set(ClickEndCall);

        //自动
        SearchChild("n18").onClick.Set(() =>
        {
            autoClickTime++;
            if (autoClickTime % 2 == 1)
            {
                onComplete = GoToNextNode;
                onComplete?.Invoke();
            }
            else
            {
                onComplete = null;
            }
        });
    }

    void GetBg()
    {
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("key", CellVoiceBgSaveInfo.Key + CallDataMgr.Ins.cellSmsConfig.actor_id);
        GameMonoBehaviour.Ins.RequestInfoPost<CellVoiceBgSaveInfo>(NetHeaderConfig.STORY_LOAD, wWWForm, Init);
    }

    void Init(CellVoiceBgSaveInfo cellVoiceBgSaveInfo)
    {
        string bgName = smsListIndex.actor_id + "bg1";
        if (cellVoiceBgSaveInfo != null && !string.IsNullOrEmpty(cellVoiceBgSaveInfo.value))
            bgName = cellVoiceBgSaveInfo.value;
        bgLoader.url = UrlUtil.GetCallBgUrl(bgName);
        nameTextField.text = callDataMgr.cardsConfig.name_cn;

        GetPassNodeInfo(smsListIndex);
        currentNodeId = callDataMgr.cellSmsConfig.startPoint;
        gameSmsPointConfig = DataUtil.GetGameSmsPointConfig(currentNodeId);

        if (gameSmsPointConfig != null)
        {
            isGoNextNode = true;
            GetCurrenViewtInfo();
        }
    }


    /// <summary>
    ///控制当前展示界面
    /// </summary>
    void GetCurrenViewtInfo()
    {
        if (callDataMgr.isViewSms)
        {
            controller.selectedIndex = (int)TYPE.DIALOGUE;
            GoToNextNode();
            return;
        }
        if (gameSmsPointConfig.title == "0")
        {
            controller.selectedIndex = (int)TYPE.CALL;
            baseView.StartCoroutine(OtherReceiveCall());
            return;
        }
        controller.selectedIndex = (int)TYPE.INCOMINGCALL;
        CallView.view.RefusedToAnswerAuto();
    }


    //对方接听电话
    IEnumerator OtherReceiveCall()
    {
        yield return new WaitForSeconds(3f);
        controller.selectedIndex = (int)TYPE.DIALOGUE;
        GoToNextNode();
    }

    IEnumerator AutoEndCall()
    {
        yield return new WaitForSeconds(3f);
        ClickEndCall();
    }

    void ClickEndCall()
    {
        callDataMgr.Dispose();
        baseView.StopAllCoroutines();
        UIMgr.Ins.HideView<CallView>();
    }

    #region 对话
    public void ShowDetailInfo()
    {
        switch (gameSmsPointConfig.type)
        {
            case (int)SmsPointType.Common:
                controller.selectedIndex = 3;
                //dialogueName.text = gameSmsPointConfig.title != "0" ? CallDataMgr.Ins.cardsConfig.name_cn : GameData.Player.name;
                dialogueName.text = gameSmsPointConfig.title != "0" ? CallDataMgr.Ins.cardsConfig.name_cn : "0";
                dialogueInfo.text = gameSmsPointConfig.content1;
                InitTypeEffect();

                if (CallView.view.isGuide)
                {
                    GameData.guiderCurrent = GuiderInfoLinked.GetCurrGuiderInfo(11, 2);
                    UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);
                }
                break;
            case (int)SmsPointType.Select:
                controller.selectedIndex = 2;
                initSelectList();
                if (CallView.view.isGuide)
                {
                    GameData.guiderCurrent = GuiderInfoLinked.GetCurrGuiderInfo(11, 3);
                    UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);
                }
                break;
        }
    }

    void initSelectList()
    {
        int count = 1;
        if (gameSmsPointConfig.content2.Trim() != "")
            count = 2;
        if (gameSmsPointConfig.content3.Trim() != "")
            count = 3;
        selectBtnList.itemRenderer = RenderListItem;
        selectBtnList.numItems = count;
    }

    void RenderListItem(int index, GObject obj)
    {
        int point = 0;
        GButton gButton = obj.asButton;
        switch (index)
        {
            case 0:
                gButton.title = gameSmsPointConfig.content1;
                point = gameSmsPointConfig.point1;
                break;
            case 1:
                gButton.title = gameSmsPointConfig.content2;
                point = gameSmsPointConfig.point2;
                break;
            case 2:
                gButton.title = gameSmsPointConfig.content3;
                point = gameSmsPointConfig.point3;
                break;
        }
        gButton.onClick.Set(() =>
        {
            GotoSaveNode(point);
        });
    }

    public void GoToNextNode()
    {
        if (SpeedPrint())
        {
            if (!isGoNextNode)
                return;
            //查找历史节点
            int nodeId = GetSelectedNodeId(gameSmsPointConfig);
            if (nodeId != 0)
                gameSmsPointConfig = DataUtil.GetGameSmsPointConfig(nodeId);

            GRoot.inst.StopDialogSound();
            ShowDetailInfo();
            //获得奖励
            GetNodesAward(gameSmsPointConfig);

            isGoNextNode = gameSmsPointConfig.type == (int)SmsPointType.Common && gameSmsPointConfig.point1 != 0;
            GameSmsNodeConfig gameSmsNodeConfig = DataUtil.GetGameSmsNodeConfig(gameSmsPointConfig.point1);
            if (gameSmsNodeConfig != null)
            {
                gameSmsPointConfig = DataUtil.GetGameSmsPointConfig(gameSmsNodeConfig.id);
                currentNodeId = gameSmsPointConfig.id;
            }
            else
            {
                GotoSaveNode(gameSmsPointConfig.point1);
            }
        }

    }

    void GotoSaveNode(int pointId)
    {
        Debug.LogError("pointId:  " + pointId);

        gameSmsPointConfig = DataUtil.GetGameSmsPointConfig(pointId);
        CallView.view.QuerySaveNode(pointId, (SmsListIndex smsListIndex) =>
        {
            if (pointId == 0)
            {
                baseView.StartCoroutine(AutoEndCall());
                
                return;
            }
            isGoNextNode = true;
            GoToNextNode();

            this.smsListIndex = smsListIndex;
            GetPassNodeInfo(smsListIndex);
        });
    }

    public override void InitTypeEffect()
    {
        base.InitTypeEffect();
        typingEffect.Start();
        typingEffects.Add(typingEffect);

        PrintTex();
    }

    void GetNodesAward(GameSmsPointConfig currentPointConfig)
    {
        GameSmsNodeConfig gameSmsNode = DataUtil.GetGameSmsNodeConfig(currentPointConfig.id);
        if (!string.IsNullOrEmpty(gameSmsNode.awards) && gameSmsNode.awards != "0" && !isGetAward(gameSmsNode))
        {
            GotoSaveAwardNode(gameSmsNode);
        }
    }

    /// <summary>
    /// 获取奖励，保存节点
    /// </summary>
    void GotoSaveAwardNode(GameSmsNodeConfig currentSmsNodeConfig)
    {
        CallView.view.QuerySaveNode(currentSmsNodeConfig.id, (SmsListIndex smsListIndex) =>
        {
            TinyItem tinyItem = ItemUtil.GetTinyItem(currentSmsNodeConfig.awards);
            UIMgr.Ins.showNextPopupView<CommonGetPropsView, TinyItem>(tinyItem);

            UpgradeInfo info = new UpgradeInfo();
            info.actorId = smsListIndex.actor_id;
            info.extra = smsListIndex.extra;
            baseView.StartCoroutine(GameTool.ShowEffect(info));
        });
    }

    #endregion


    #region  已通过节点，历史记录
    List<int> passNodes = new List<int>();
    List<int> awardNodes = new List<int>();

    public void GetPassNodeInfo(SmsListIndex smsListIndex)
    {
        awardNodes.Clear();
        passNodes.Clear();
        if (smsListIndex.award_node != null)
            awardNodes = GetSplitList(smsListIndex.award_node);
        if (smsListIndex.pass_node != null)
            passNodes = GetSplitList(smsListIndex.pass_node);

    }

    private int GetSelectedNodeId(GameSmsPointConfig gameSmsPoint)
    {
        if (gameSmsPoint != null && gameSmsPoint.type == 2)
        {
            if (passNodes.Contains(gameSmsPoint.point1))
                return gameSmsPoint.point1;
            if (passNodes.Contains(gameSmsPoint.point2))
                return gameSmsPoint.point2;
            if (passNodes.Contains(gameSmsPoint.point3))
                return gameSmsPoint.point3;
        }
        return 0;
    }

    bool isGetAward(GameSmsNodeConfig currentGameSmsNode)
    {
        return awardNodes.Contains(currentGameSmsNode.id);
    }

    #endregion
    List<int> GetSplitList(string info)
    {
        string[] splitNodes = info.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        List<int> tmpList = new List<int>();
        for (int i = 0; i < splitNodes.Length; i++)
        {
            tmpList.Add(int.Parse(splitNodes[i]));
        }
        return tmpList;
    }

}
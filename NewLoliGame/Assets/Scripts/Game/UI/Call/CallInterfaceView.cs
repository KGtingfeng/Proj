using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using System;
using System.Linq;

[ViewAttr("Game/UI/D_call", "D_call", "Call_fullscreen", true)]
public class CallInterfaceView : BaseView
{
    public static CallInterfaceView ins;
    public bool isGuide;

    enum SmsPointType
    {
        Common = 1,
        Select
    }

    /// <summary>
    /// 展示界面
    /// </summary>
    enum TYPE
    {
        //
        INCOMINGCALL = 0,
        CALL,
        SELECT,
        DIALOGUE
    }

    int playType = 0;
    enum AutoModle
    {
        //普通
        Normal,
        //自动x1
        Auto_X1,
        //自动x2
        Auto_x2,
    }
    //自动播放按钮url
    List<string> autoUrl = new List<string>() {
        "ui://6f05wxg5mjv5f",
        "ui://6f05wxg5f9t3q3c",
        "ui://6f05wxg5f9t3q3d" };

    TypingEffect typingEffect;
    GLoader bgLoader;
    GTextField nameTextField;
    GList selectBtnList;

    GLoader iconLoader;

    GComponent dialogueCom;
    GTextField dialogueName;
    GTextField dialogueMyName;
    GTextField dialogueInfo;

    GComponent autoBtn;
    //GLoader auto1Loader;
    //GLoader auto2Loader;
    Controller autoController;
    Controller dialogueController;
    //可播放下一节点
    bool isGoNextNode;
    //最后一个节点需重置为0
    bool isReset = false;
    /// <summary>
    /// 背景信息
    /// </summary>
    CellVoiceBgSaveInfo cellVoiceBgSaveInfo;
    string bgName;

    public int currentNodeId = 0;

    SmsListIndex smsListIndex;

    GameSmsPointConfig currentSmsPointConfig;
    int actorId;
    public override void InitUI()
    {
        controller = ui.GetController("c1");
        selectBtnList = SearchChild("n13").asList;
        bgLoader = SearchChild("n1").asLoader;
        nameTextField = SearchChild("n5").asTextField;

        autoBtn = SearchChild("n18").asCom;
        //auto1Loader = autoBtn.GetChild("n0").asLoader;
        //auto2Loader = autoBtn.GetChild("n1").asLoader;
        autoController = autoBtn.GetController("c1");

        dialogueCom = SearchChild("n17").asCom;
        dialogueName = dialogueCom.GetChild("n3").asTextField;
        dialogueMyName = dialogueCom.GetChild("n7").asTextField;
        dialogueController = dialogueCom.GetController("c1");
        dialogueInfo = dialogueCom.GetChild("n4").asTextField;
        typingEffect = new TypingEffect(dialogueInfo);
        iconLoader = SearchChild("n21").asLoader;
        InitEvent();
        ins = this;
    }

    public override void InitData<D>(D data)
    {
        OnShowAnimation();
        base.InitData(data);
        EventMgr.Ins.DispachEvent(EventConfig.STORY_BREAKE_MAIN);

        smsListIndex = data as SmsListIndex;
        CallDataMgr.Ins.RefreshSMSInfo(smsListIndex.sms_id);
        actorId = smsListIndex.actor_id;
        //默认信息
        nameTextField.text = (string.IsNullOrEmpty(smsListIndex.name)) ? CallDataMgr.Ins.cardsConfig.name_cn : smsListIndex.name;

        currentNodeId = CallDataMgr.Ins.cellSmsConfig.startPoint;
        currentSmsPointConfig = JsonConfig.GameSmsPointConfigs.Find(a => a.id == currentNodeId);
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
        GetPassNodeInfo(smsListIndex);
        GetCurrentBg();


    }

    public override void InitEvent()
    {
        SearchChild("n1").onClick.Set(() =>
        {
            SetingNormal();
            if (controller.selectedIndex != (int)TYPE.CALL && controller.selectedIndex != (int)TYPE.INCOMINGCALL)
                GoToNextNode();
        });

        SearchChild("n3").onClick.Set(StartPlayDialog);

        SearchChild("n4").onClick.Set(ClickEndCall);

        SearchChild("n10").onClick.Set(ClickEndCall);

        //自动
        SearchChild("n18").onClick.Set(() =>
        {
            playType++;
            if (playType > (int)AutoModle.Auto_x2)
            {
                SetingNormal();
            }
            else
            {
                speed = playType == (int)AutoModle.Auto_x2 ? 0.25f : 0.5f;
                onComplete = GoToNextNode;
                autoController.selectedIndex = playType;
                //SetSppeedBtnUrl(autoUrl[playType]);
                if (currentTypingEffect != null)
                    currentTypingEffect.ChangeSpeed(speed);
                if (currentTypingEffect == null)
                    onComplete?.Invoke();
            }
        });

    }

    /// <summary>
    /// 初始化当前界面信息
    /// </summary>
    void InitCurrentView()
    {
        List<GameCellVoiceBackgroundConfig> bgConfigs = JsonConfig.GameCellVoiceBackgroundConfigs.FindAll(a => a.limit.Equals("") && a.actor_id == actorId && a.type == 1);
        bgName = bgConfigs[0].assets;
        if (cellVoiceBgSaveInfo != null && cellVoiceBgSaveInfo.value != null && !cellVoiceBgSaveInfo.value.Equals(""))
            bgName = cellVoiceBgSaveInfo.value;
        bgLoader.url = UrlUtil.GetCallBgUrl(bgName);

        if (!CallDataMgr.Ins.isViewSms)
        {
            AudioClip audioClip = Resources.Load(SoundConfig.PHONE_AUDIO_EFFECT_URL + (int)SoundConfig.PhoneAudioId.Call) as AudioClip;
            GRoot.inst.PlayEffectSound(audioClip);

            if (currentSmsPointConfig.title == "0")
            {
                controller.selectedIndex = (int)TYPE.CALL;
                StartCoroutine(OtherReceiveCall());
            }
            else
            {
                bgLoader.url = UrlUtil.GetCallBgUrl("2");
                controller.selectedIndex = (int)TYPE.INCOMINGCALL;
                iconLoader.url = UrlUtil.GetStoryHeadIconUrl(actorId);
                StartCoroutine(RefuseCall(10f));
            }
            return;
        }
        StartPlayDialog();


    }

    //开始播放剧情
    void StartPlayDialog()
    {
        GRoot.inst.StopEffectSound();
        StopAllCoroutines();
        EventMgr.Ins.DispachEvent(EventConfig.SMS_CALL_START_MAIN, CallDataMgr.Ins.cellSmsConfig);
        EventMgr.Ins.DispachEvent<GameCellSmsConfig>(EventConfig.SMS_CALL_START_CALL_RECORD, CallDataMgr.Ins.cellSmsConfig);
        bgLoader.url = UrlUtil.GetCallBgUrl(bgName);
        isGoNextNode = true;

        if (!passNodes.Contains(currentNodeId))
        {
            QuerySaveNode(currentNodeId, (SmsListIndex sms) =>
            {
                smsListIndex = sms;
                GetPassNodeInfo(sms);
                GoToNextNode();
            });
        }
        else
            GoToNextNode();
    }
    bool IsShowSelectGuide;
    bool IsShowFirstGuide;
    #region 播放对话
    public void ShowDetailInfo()
    {
        switch (currentSmsPointConfig.type)
        {
            case (int)SmsPointType.Common:
                {
                    Debug.Log("SmsPointType.Common");
                    controller.selectedIndex = (int)TYPE.DIALOGUE;
                    if (currentSmsPointConfig.title != "0")
                    {
                        dialogueController.selectedIndex = 0;
                        dialogueName.text = CallDataMgr.Ins.cardsConfig.name_cn;
                    }
                    else
                    {
                        dialogueController.selectedIndex = 1;
                        dialogueMyName.text = GameData.Player.name;
                    }
                    string text = DataUtil.ReplaceCharacterWithStarts(currentSmsPointConfig.content1);

                    dialogueInfo.text = text;
                    InitTypeEffect();
                    if (isGuide)
                    {
                        if (!IsShowFirstGuide)
                        {
                            IsShowFirstGuide = true;
                            GameData.guiderCurrent = GuiderInfoLinked.GetCurrGuiderInfo(11, 1);
                            UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);
                        }
                        else
                        {
                            GameData.guiderCurrent = GuiderInfoLinked.GetCurrGuiderInfo(11, 2);
                            UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);
                        }
                        
                    }
                }
                break;
            case (int)SmsPointType.Select:
                {
                    Debug.Log("SmsPointType.Select");
                    controller.selectedIndex = (int)TYPE.SELECT;
                    selectBtnList.itemRenderer = RenderListItem;
                    selectBtnList.numItems = currentSmsPointConfig.storyConiditionItems.Count;
                    if (isGuide)
                    {
                        if (!IsShowSelectGuide)
                        {
                            IsShowSelectGuide = true;
                            GameData.guiderCurrent = GuiderInfoLinked.GetCurrGuiderInfo(11, 3);
                            UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);
                        }
                        else
                        {
                            UIMgr.Ins.HideView<NewbieGuideView>();
                        }
                        //else
                        //{
                        //    GameData.guiderCurrent = GuiderInfoLinked.GetCurrGuiderInfo(11, 2);
                        //    UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);
                        //}
                    }
                   
                }
                break;
        }
    }
    void RenderListItem(int index, GObject obj)
    {
        GButton gButton = obj.asButton;
        string content = DataUtil.ReplaceCharacterWithStarts(currentSmsPointConfig.storyConiditionItems[index].content);
        gButton.title = GameTool.GetCutText(content, 12);
        int point = currentSmsPointConfig.storyConiditionItems[index].point;
        gButton.onClick.Set(() =>
        {
            SaveSelectNodes(point);
        });
    }

    /// <summary>
    /// 播放节点控制
    /// </summary>
    public void GoToNextNode()
    {
        if (SpeedPrint())
        {
            if (!isGoNextNode)
                return;
            //GRoot.inst.StopDialogSound();

            //查找历史节点
            int nodeId = GetSelectedNodeId(currentSmsPointConfig);
            if (nodeId != 0)
                currentSmsPointConfig = DataUtil.GetGameSmsPointConfig(nodeId);

            ShowDetailInfo();
            isGoNextNode = currentSmsPointConfig.type == (int)SmsPointType.Common && currentSmsPointConfig.point1 != 0;
            GameSmsNodeConfig nextNodeConfig = DataUtil.GetGameSmsNodeConfig(currentSmsPointConfig.point1);
            if (nextNodeConfig != null)
            {
                currentSmsPointConfig = DataUtil.GetGameSmsPointConfig(nextNodeConfig.id);
                currentNodeId = nextNodeConfig.id;
            }
            else
            {
                isReset = false;
                if (!passNodes.Contains(currentSmsPointConfig.id))
                {
                    isReset = true;
                    SaveEndPointNodes(currentSmsPointConfig.id);
                }
                else
                {
                    StartCoroutine(RefuseCall(1f));
                }
            }
        }
    }

    #endregion

    #region SaveNodes

    /// <summary>
    /// 保存选择节点
    /// </summary>
    /// <param name="nodeId"></param>
    void SaveSelectNodes(int nodeId)
    {
        QuerySaveNode(nodeId, (SmsListIndex smsListIndex) =>
        {
            GetAward(smsListIndex);

            currentSmsPointConfig = DataUtil.GetGameSmsPointConfig(nodeId);
            isGoNextNode = true;
            GoToNextNode();
        });
    }

    void SaveEndPointNodes(int endNodeId)
    {
        Debug.LogError("nodeId: " + endNodeId);
        QuerySaveNode(endNodeId, (SmsListIndex smsListIndex) =>
        {
            GetAward(smsListIndex);
            StartCoroutine(RefuseCall(2f));
        });
    }

    /// <summary>
    /// 保存节点
    /// </summary>
    public void QuerySaveNode(int nodeId, Action<SmsListIndex> callBack)
    {
        Debug.Log("********************call  set nodeId " + nodeId + "   smsid   =" + CallDataMgr.Ins.cellSmsConfig.id);
        
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("actorId", CallDataMgr.Ins.cellSmsConfig.actor_id);
        wWWForm.AddField("smsId", CallDataMgr.Ins.cellSmsConfig.id);
        wWWForm.AddField("nodeId", nodeId);
        GameMonoBehaviour.Ins.RequestInfoPostHaveData(NetHeaderConfig.CELL_SET_STEP, wWWForm, callBack, false);
    }

    void GetAward(SmsListIndex sms)
    {
        if (awardNodes.Contains(currentNodeId) || sms.award.Trim().Equals("") || sms.award.Equals("0"))
            return;

        AwardInfo info = new AwardInfo();
        info.award = sms.award;
        info.extra = sms.extra;
        GameTool.GetAwards(info);
    }

    SMSView view;
    void GotoSms()
    {
        WWWForm wWWForm = new WWWForm();
        GameMonoBehaviour.Ins.RequestInfoPost<List<SmsListIndex>>(NetHeaderConfig.CELL_LIST_INDEX, wWWForm, RequestGotoMain);
    }

    void RequestGotoMain(List<SmsListIndex> smsLists)
    {
        view = UIMgr.Ins.showNextView<SMSView, List<SmsListIndex>>(smsLists) as SMSView;
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("actorId", actorId);
        GameMonoBehaviour.Ins.RequestInfoPostHaveData<SmsListIndex>(NetHeaderConfig.CELL_QUERY_MESSAGE, wWWForm, GotoSms, false);
    }

    void GotoSms(SmsListIndex smsList)
    {
        view.GoToMoudle<SMSMoudle, SmsListIndex>((int)SMSView.MoudleType.TYPE_SMS, smsList);
        UIMgr.Ins.HideView<CallInterfaceView>();
    }



    #endregion

    #region IEnumerator And Refused
    /// <summary>
    /// 对方接听电话
    /// </summary>
    /// <returns></returns>
    IEnumerator OtherReceiveCall()
    {
        yield return new WaitForSeconds(2f);
        GRoot.inst.StopEffectSound();
        CallDataMgr.Ins.isCalling = true;
        StartPlayDialog();
    }

    /// <summary>
    /// 挂断电话
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    IEnumerator RefuseCall(float time)
    {
        yield return new WaitForSeconds(time);
        ClickEndCall();
    }

    void ClickEndCall()
    {
        GRoot.inst.StopEffectSound();
        SetingNormal();
        SpeedPrint();

        if (currentSmsPointConfig != null && currentSmsPointConfig.point1 == 0)
        {
            EventMgr.Ins.DispachEvent(EventConfig.SMS_CALL_FINISH_MAIN, CallDataMgr.Ins.cellSmsConfig);
            EventMgr.Ins.DispachEvent(EventConfig.SMS_CALL_FINISH_CALL_RECORD, CallDataMgr.Ins.cellSmsConfig);

            if (isReset)
                QuerySaveNode(0, null);
        }
        if (CallDataMgr.Ins.isCalling && !SMSDataMgr.Ins.IsOnSms || (SMSDataMgr.Ins.IsOnSms && SMSDataMgr.Ins.CurrentRole != actorId))
            GotoSms();

        CallDataMgr.Ins.Dispose();
        UIMgr.Ins.HideView<CallInterfaceView>();
    }
    #endregion

    /// <summary>
    /// 获得通话背景
    /// </summary>
    void GetCurrentBg()
    {
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("key", CellVoiceBgSaveInfo.Key + CallDataMgr.Ins.cellSmsConfig.actor_id);
        GameMonoBehaviour.Ins.RequestInfoPost(NetHeaderConfig.STORY_LOAD, wWWForm, (CellVoiceBgSaveInfo cellVoiceBgSaveInfo) =>
        {
            this.cellVoiceBgSaveInfo = cellVoiceBgSaveInfo;
            InitCurrentView();
        });
    }



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
    #endregion

    void PlayDialogMusic()
    {
        if (currentSmsPointConfig.sound_id != 0 && !isGoNextNode)
        {
            AudioClip audioClip = ResourceUtil.LoadStoryDialogSound(currentSmsPointConfig.sound_id);
            GRoot.inst.PlayDialogSound(audioClip);
        }
    }

    public override void InitTypeEffect()
    {
        base.InitTypeEffect();
        typingEffect.Start();
        typingEffects.Add(typingEffect);

        PrintTex();
    }
    private void SetingNormal()
    {
        onComplete = null;
        speed = 1.0f;
        playType = (int)AutoModle.Normal;
        autoController.selectedIndex = playType;
        //SetSppeedBtnUrl(autoUrl[playType]);

        if (currentTypingEffect != null)
            currentTypingEffect.ChangeSpeed(speed);
    }
    void SetSppeedBtnUrl(string url)
    {
        //auto1Loader.url = url;
        //auto2Loader.url = url;
    }

    public override void onHide()
    {
        base.onHide();
        GRoot.inst.StopEffectSound();

    }
}
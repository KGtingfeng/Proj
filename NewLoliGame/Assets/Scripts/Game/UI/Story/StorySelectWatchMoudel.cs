using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;
using System;

public class StorySelectWatchMoudel : BaseMoudle
{
    GList loopList;
    GLoader backgroundLoader;
    Controller controller;
    GTextField tipText;
    GTextField countDownTipText;
    GComponent group;
    GButton startBtn;
    GButton confirmBtn;
    //获得
    //GTextField getNumText;
    //GLoader getImgLoader;

    int ROTATE_TIME = 0;
    int ROTATEING_TIME = 1;
    int ROTATEING_SUCCESS = 2; 
    GamePointConfig gamePointConfig; 

    //int answer = 1;
    List<int> answerList = new List<int>
    {
        1,2,3,4
    };

    List<int> poolList = new List<int>();
    CommonBigTipsInfo commonBigTipsInfo;


    GObject hourRotateObj;
    GObject minRotateObj;
    public override void InitMoudle(GComponent gComponent, int controllerIndex)
    {
        base.InitMoudle(gComponent, controllerIndex);
        controller = ui.GetController("c1");

        backgroundLoader = SearchChild("n0").asLoader;
        tipText = SearchChild("n4").asTextField;

        group = SearchChild("n37").asCom;
        hourRotateObj = SearchChild("n35");
        minRotateObj = SearchChild("n34");
        startBtn = SearchChild("n40").asButton;
        confirmBtn = SearchChild("n45").asButton;
        countDownTipText = SearchChild("n41").asTextField;
        commonBigTipsInfo = new CommonBigTipsInfo();
        commonBigTipsInfo.isShowText = false;
        commonBigTipsInfo.url = "ui://82yrhfjgiv3yq6h";
        InitEvent();
    }



    public override void InitData<D>(D data)
    {
        base.InitData(data);

        gamePointConfig = data as GamePointConfig;
        if (gamePointConfig != null)
        {
            backgroundLoader.url = UrlUtil.GetStoryBgUrl(gamePointConfig.background_id);
            controller.selectedIndex = ROTATE_TIME;
            ResetTime();
        }

    }




    public override void InitEvent()
    {
        base.InitEvent();
        //tips
        SearchChild("n2").onClick.Set(() =>
        {
            UIMgr.Ins.showWindow<CommonBigTipsWindows, CommonBigTipsInfo>(commonBigTipsInfo);
        });



        //hour
        LongPressGesture longPressHourBtn = new LongPressGesture(group.GetChild("n37"));
        //gesture.once = true;
        longPressHourBtn.trigger = 0.01f;
        longPressHourBtn.interval = 0.01f;
        longPressHourBtn.onAction.Set(OnLongPressHourBtn);



        LongPressGesture longPressMinuBtn = new LongPressGesture(group.GetChild("n38"));
        longPressMinuBtn.trigger = 0.01f;
        longPressMinuBtn.interval = 0.01f;
        longPressMinuBtn.onAction.Set(OnLongPressMinueBtn);

        //over
        SearchChild("n32").onClick.Set(GoToNextNode);
        //break
        SearchChild("n1").onClick.Set(() =>
        {
            Timers.inst.Remove(CountDown);
            EventMgr.Ins.DispachEvent(EventConfig.STORY_BREACK_STORY);
        });

        //SearchChild("n40").onClick.Set(OnClickConfirm);

        startBtn.onClick.Set(() =>
        { 
            isIdle = false;
            controller.selectedIndex = ROTATEING_TIME;
            StartRotate();
 
        });

        confirmBtn.onClick.Set(() =>
        {
            OnClickConfirm();
        });

        //skip

        SearchChild("n3").onClick.Set(() =>
        {
            NormalInfo normalInfo = new NormalInfo();
            normalInfo.index = gamePointConfig.id;
            normalInfo.type = GameConsumeConfig.STORY_PASS_NODE_TYPE;
            EventMgr.Ins.DispachEvent(EventConfig.STORY_SKIP_NODE, normalInfo);
        });
    }

    bool isIdle = true;
    int totalTime;
    void ResetTime()
    {
        //默认为11点10分
        hourRotateObj.rotation = 30.0f * 11;
        minRotateObj.rotation = 10 * 6;
        tipText.text = gamePointConfig.content1;//  "请长按转动分针与时针，确认时间！";//gamePointConfig.title;
        //controller.selectedIndex = SELECT_WATCH;
        group.visible = false;
        startBtn.title = "开始操作";
        totalTime = 20;
        countDownTipText.text = "倒计时 " + totalTime + "s";

        isIdle = true;
        controller.selectedIndex = ROTATE_TIME;


    }

    int count = 10;


    void CountDown(object param)
    {
        if (totalTime == 0)
        {
            Timers.inst.Remove(CountDown);
            ResetTime();
            return;
        }
        countDownTipText.text = "倒计时 " + totalTime + "s";
        totalTime--;
    }

    void StartRotate()
    {
        startBtn.title = "确定";
        group.visible = true;
        countDownTipText.text = "倒计时 " + totalTime + "s";
        tipText.text = gamePointConfig.content1;
        totalTime--;
        Timers.inst.Add(1f, 0, CountDown);
    }

    void OnLongPressHourBtn(EventContext context)
    {
        if (!isIdle)
        {
            float angel = hourRotateObj.rotation + 1;
            if (angel >= 360 || angel <= -360)
                angel = 0;
            hourRotateObj.rotation = angel;
        }


    }


    void OnLongPressMinueBtn(EventContext context)
    {
        if (!isIdle)
        {
            float angel = minRotateObj.rotation + 1;
            if (angel >= 360 || angel <= -360)
                angel = 0;
            minRotateObj.rotation = angel;
        }


    }
 

    void OnClickConfirm()
    {
        //时针和分针没有到3点30分
        double hour = hourRotateObj.rotation / 30.0f;
        double minue = minRotateObj.rotation / 6.0f;
        Debug.Log("hour:" + hour + "  minu:" + minue);
        Timers.inst.Remove(CountDown);
        //时针和分钟左右可以有6°的误差
        if (hour >= 2.8 && hour <= 3.2 && minue >= 29 && minue <= 31)
        {

            tipText.text = "";
            controller.selectedIndex = ROTATEING_SUCCESS;

        }
        else
        {
            ResetTime();
            UIMgr.Ins.showErrorMsgWindow(MsgException.SELECT_WATCHE_ERROR);
        }


    }


    void GoToNextNode()
    {

        NormalInfo normalInfo = new NormalInfo();
        normalInfo.index = gamePointConfig.point1;
        //EventMgr.Ins.DispachEvent(EvenConfig.STORY_TRIGGER_NEXT_NODE, normalInfo);
        EventMgr.Ins.DispachEvent(EventConfig.STORY_RECORD_SELECT_NODE, normalInfo);
    }




}

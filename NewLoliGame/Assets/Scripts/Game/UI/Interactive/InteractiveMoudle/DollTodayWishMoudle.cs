using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using System;


public class DollTodayWishMoudle : BaseMoudle
{

    GTextField contentText;
    GTextField faverText;
    GTextField countDownText;
    Controller controller;
    GTextField numText;
    GTextField nameText;
    GLoader iconLoader;

    ActorTask actorTask;
    GameTaskConfig gameTask;
    TinyItem tinyItem;
    static readonly string COUNT_DOWN = "心愿倒计时：\n";
    static readonly string FAVOR_ADD = "好感度增加：";
    public override void InitMoudle(GComponent gComponent, int controllerIndex)
    {

        base.InitMoudle(gComponent, controllerIndex);
        InitUI();
        InitEvent();
    }

    public override void InitUI()
    {
        base.InitUI();
        contentText = SearchChild("n59").asTextField;
        faverText = SearchChild("n64").asTextField;
        countDownText = SearchChild("n65").asTextField;
        controller = ui.GetController("c1");
        numText = SearchChild("n61").asCom.GetChild("n64").asTextField;
        iconLoader = SearchChild("n61").asCom.GetChild("n62").asLoader;
        nameText = SearchChild("n61").asCom.GetChild("n65").asTextField;
    }

    public override void InitEvent()
    {
        base.InitEvent();
        //来源
        SearchChild("n61").onClick.Set(() =>
        {

            UIMgr.Ins.showNextPopupView<PropSourcesView, PlayerProp>(actorTask.task.playerProp);
        });

        //送礼
        SearchChild("n66").onClick.Set(() =>
        {
            OnClickPresent();
            //RequestPresentSuccess();
        });

    }

    public override void InitData<D>(D data)
    {
        base.InitData(data);
        actorTask = data as ActorTask;
        RefreshInfo();
        EventMgr.Ins.RegisterEvent(EventConfig.REFRESH_TODAY_WISH, RefreshTodayWish);
        EventMgr.Ins.RegisterEvent(EventConfig.REFRSH_COUNTDOWN, RefreshCountDown);
    }

    void RefreshInfo()
    {
        if (actorTask.isFinish)
            controller.selectedIndex = 1;
        else
            controller.selectedIndex = 0;

        gameTask = JsonConfig.GameTaskConfigs.Find(a => a.task_id == actorTask.task.task_id);
        contentText.text = gameTask.task_description;
        tinyItem = ItemUtil.GetTinyItem(gameTask.task_condition);
        numText.text = actorTask.task.playerProp.prop_count + "/" + tinyItem.num;
        iconLoader.url = UrlUtil.GetItemIconUrl(actorTask.task.playerProp.prop_id);
        nameText.text = JsonConfig.GamePropConfigs.Find(a => a.prop_id == tinyItem.id).prop_name;
        TinyItem favorItem = ItemUtil.GetTinyItem(gameTask.task_award);
        faverText.text = FAVOR_ADD + favorItem.num;
        date = TimeUtil.getTime(actorTask.end_time);
        TimerCountDown timercd = countDownText.displayObject.gameObject.GetComponent<TimerCountDown>();
        if (timercd == null)
        {
            timercd = countDownText.displayObject.gameObject.AddComponent<TimerCountDown>();
        }
        CountDown(timercd);
    }

    DateTime date;
    TimerCountDown tcd;
    void CountDown(TimerCountDown tcd)
    {

        int totalTime = (int)(date - TimeUtil.getServerTime()).TotalSeconds;
        this.tcd = tcd;
        tcd.TotalSeconds(totalTime);
        countDownText.text = COUNT_DOWN + tcd.s.ToString();

        tcd.OnChange = delegate (TimeSpan ts, int s)
        {
            countDownText.text = COUNT_DOWN + ts.ToString();
        };
        tcd.OnOver = delegate ()
        {
            TimerCountDown t = countDownText.displayObject.gameObject.AddComponent<TimerCountDown>();
            CountDown(t);
        };
    }

    void RefreshCountDown()
    {
        int totalTime = (int)(date - TimeUtil.getServerTime()).TotalSeconds;
        tcd.TotalSeconds(totalTime);
        countDownText.text = COUNT_DOWN + tcd.s.ToString();
    }

    void OnClickPresent()
    {

        if (tinyItem.num > actorTask.task.playerProp.prop_count)
        {
            UIMgr.Ins.showErrorMsgWindow("拥有的礼物数量不足！");
            return;
        }

        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("actorId", InteractiveDataMgr.ins.CurrentPlayerActor.actor_id);
        wWWForm.AddField("propId", tinyItem.id);
        wWWForm.AddField("num", tinyItem.num);
        InteractiveDataMgr.ins.presentProp = tinyItem.id;
        GameMonoBehaviour.Ins.RequestInfoPost<PlayerActor>(NetHeaderConfig.PRESENT_PROP, wWWForm, RequestPresentSuccess);
    }

    void RequestPresentSuccess()
    {
        controller.selectedIndex = 1;
        WWWForm wWWForm = new WWWForm();
        GameMonoBehaviour.Ins.RequestInfoPost<PlayerRedpoint>(NetHeaderConfig.RED_POINT, wWWForm, null, false);
    }

    void RefreshTodayWish()
    {
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("actorId", InteractiveDataMgr.ins.CurrentPlayerActor.actor_id);
        GameMonoBehaviour.Ins.RequestInfoPost<ActorTask>(NetHeaderConfig.TODAY_WISH, wWWForm, Request);
    }

    void Request(ActorTask task)
    {
        actorTask = task;
        RefreshInfo();
    }
}

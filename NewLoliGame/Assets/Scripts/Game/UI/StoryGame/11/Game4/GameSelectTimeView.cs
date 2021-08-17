using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using Spine.Unity;
using Spine;
using UnityEngine;

[ViewAttr("Game/UI/Y_Game4", "Y_Game4", "Game")]
public class GameSelectTimeView : BaseGameView
{
    GLoader backgroundLoader;
    GTextField countDownTipText;
    GButton startBtn;
    GButton confirmBtn;
    GComponent group;

    GGraph biaodiGgraph;
    GGraph butdiGgraph;
    GComponent fxCom;
    GGraph spineGgraph;
    GGraph bgGgraph;

    int ROTATE_TIME = 0;
    int ROTATEING_TIME = 1;
    int ROTATEING_SUCCESS = 2;


    GObject hourRotateObj;
    GObject minRotateObj;

    public override void InitUI()
    {
        base.InitUI();
        controller = ui.GetController("c1");

        backgroundLoader = SearchChild("n0").asLoader;

        hourRotateObj = SearchChild("n14");
        minRotateObj = SearchChild("n15");
        startBtn = SearchChild("n40").asButton;
        confirmBtn = SearchChild("n9").asButton;
        countDownTipText = SearchChild("n6").asTextField;
        group = SearchChild("n8").asCom; 
        spineGgraph = new GGraph();
        fxCom = SearchChild("n19").asCom;
        fxCom.AddChild(spineGgraph);
        bgGgraph = SearchChild("n20").asGraph;
        InitEvent();
    }

    public override void InitData<D>(D data)
    {
        base.InitData(data);

        storyGameInfo = data as StoryGameInfo;

        backgroundLoader.url = UrlUtil.GetGameBGUrl(19);
        controller.selectedIndex = ROTATE_TIME;
        ResetTime();
        biaodiGgraph = FXMgr.CreateEffectWithScale(fxCom, new Vector2(213, -24), "UI_game4_biaodi", 162, -1);
        butdiGgraph = FXMgr.CreateEffectWithScale(fxCom, new Vector2(294, 525), "UI_game4_butdi", 162, -1);
    }

    public override void InitData()
    {
        base.InitData();
        backgroundLoader.url = UrlUtil.GetGameBGUrl(19);
        controller.selectedIndex = ROTATE_TIME;
        biaodiGgraph = FXMgr.CreateEffectWithScale(fxCom, new Vector2(213, -24), "UI_game4_biaodi", 162,-1);
        butdiGgraph = FXMgr.CreateEffectWithScale(fxCom, new Vector2(294, 525), "UI_game4_butdi", 162,-1);
        ResetTime();
    }


    public override void InitEvent()
    {
        base.InitEvent();
        //返回
        SearchChild("n1").onClick.Set(() =>
        {
            Timers.inst.Remove(CountDown);
            EventMgr.Ins.DispachEvent(EventConfig.STORY_BREACK_STORY);
        });
        //skip
        SearchChild("n3").onClick.Set(SkipGame);
        //tips
        SearchChild("n2").onClick.Set(OnClickTips);

        //hour
        LongPressGesture longPressHourBtn = new LongPressGesture(group.GetChild("n37"));
        longPressHourBtn.trigger = 0.01f;
        longPressHourBtn.interval = 0.01f;
        longPressHourBtn.onAction.Set(OnLongPressHourBtn);

        LongPressGesture longPressMinuBtn = new LongPressGesture(group.GetChild("n38"));
        longPressMinuBtn.trigger = 0.01f;
        longPressMinuBtn.interval = 0.01f;
        longPressMinuBtn.onAction.Set(OnLongPressMinueBtn);

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

        backgroundLoader.onClick.Set(() =>
        {
            if (controller.selectedIndex == 2)
            {
                OnComplete();
            }
        });

        EventMgr.Ins.ReplaceEvent(EventConfig.STORY_GAME_QUIT, Destroy<GameSelectTimeView>);
        EventMgr.Ins.ReplaceEvent(EventConfig.STORY_DELETE_GAME_INFO, DeleteGameInfo);

    }

    bool isIdle = true;
    int totalTime;
    void ResetTime()
    {
        //默认为10点40分
        hourRotateObj.rotation = 30.0f * 10.5f;
        minRotateObj.rotation = 40 * 6;
        totalTime = 20;
        countDownTipText.text = "倒计时 " + totalTime + "s";

        isIdle = true;
        controller.selectedIndex = ROTATE_TIME;
    }


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

        countDownTipText.text = "倒计时 " + totalTime + "s";
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
        if (hour >= 11.9 || hour <= 0.1 && minue >= 59.9 || minue <= 0.1)
        {

            controller.selectedIndex = ROTATEING_SUCCESS;
            StartCoroutine(ClockEffect());
        }
        else
        {
            ResetTime();
            UIMgr.Ins.showErrorMsgWindow(MsgException.SELECT_WATCHE_ERROR);

            GameTipsInfo gameTipsInfo = new GameTipsInfo()
            {
                context = "再仔细观察一下四时钟现场的线索吧",
                title = "游戏失败",
                btnTitle = "再次观察",
                isShowBtn = true,
                callBack = GotoNextNode,
            };
            UIMgr.Ins.showNextPopupView<Game4FailView, GameTipsInfo>(gameTipsInfo);
        }
    }

    IEnumerator ClockEffect()
    {
        biaodiGgraph.visible = false;
        butdiGgraph.visible = false;
        SkeletonAnimation skeletonAnimation = FXMgr.LoadSpineEffect("Game4", spineGgraph, new Vector2(619, 872), 100);
        FXMgr.CreateEffectWithScale(fxCom, new Vector2(249, 0), "UI_game4_biaoding", 162,-1);

        TrackEntry trackEntry = skeletonAnimation.AnimationState.SetAnimation(0, "animation", true);
        trackEntry.Loop = false;
        trackEntry.AnimationStart = 0f;
        trackEntry.AnimationEnd = 1.5f;
        skeletonAnimation.timeScale = 1;

        yield return new WaitForSeconds(1.49f);
        TrackEntry trackEntry0 = skeletonAnimation.AnimationState.SetAnimation(0, "animation", true);
        trackEntry0.AnimationStart = 1.5f;
        trackEntry0.AnimationEnd = 1.5f;
        skeletonAnimation.timeScale = 1;
    }


    Extrand extrand;
    void OnClickTips()
    {
        GetTip();
        //if (extrand == null)
        //{
        //    extrand = new Extrand();
        //    extrand.type = 1;
        //    extrand.key = "提示";
        //    GameNodeConsumeConfig list = JsonConfig.GameNodeConsumeConfigs.Find(a => a.node_id == storyGameInfo.gamePointConfig.id && a.type == storyGameInfo.gamePointConfig.type);
        //    TinyItem item = ItemUtil.GetTinyItem(list.pay);
        //    extrand.item = item;
        //    extrand.msg = "获得提示";
        //    extrand.callBack = RequestGetTips;
        //}
        //UIMgr.Ins.showNextPopupView<PopupDialogView, Extrand>(extrand);
    }

    void RequestGetTips()
    {
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("nodeId", storyGameInfo.gamePointConfig.id);
        wWWForm.AddField("type", storyGameInfo.gamePointConfig.type);
        wWWForm.AddField("index", 0);
        GameMonoBehaviour.Ins.RequestInfoPost<PlayerProperty>(NetHeaderConfig.STROY_STEP_CONSUME, wWWForm, GetTip);
    }

    void GetTip()
    {

        UIMgr.Ins.showNextPopupView<GameImageTipsView>();
    }



    /// <summary>
    /// 前往中间剧情
    /// </summary>
    public override void GotoNextNode()
    {
        Timers.inst.Remove(CountDown);
        base.GotoNextNode();
    }
}

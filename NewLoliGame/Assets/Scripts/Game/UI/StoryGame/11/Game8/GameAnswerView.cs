using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

[ViewAttr("Game/UI/Y_Game8", "Y_Game8", "Game")]
public class GameAnswerView : BaseGameView
{
    private GLoader bgLoader;
    private GTextField timeText;
    private GTextField numText;
    private GTextField questionText;
    private GComponent aniCom;
    private Transition transition;
    private GComponent startCom;
    private GGraph maskGraph;
    private GGraph bgGraph;
    List<GButton> btnList = new List<GButton>();
    AudioClip countdown;

    List<GameNodeConsumeConfig> consumeConfigs;
    private readonly List<string> questionList = new List<string>()
    {
        "一年中哪个月只有28天？",
        "一年365天是以什么计算的？",
        "一个月是以什么计算的？",
        "阴历是以什么为计算单位的？",
        "pm 是指一天中的什么时候？",
        "英文ten to six 是几点？",
        "一天24小时是什么概念？",
        "一周7天是什么概念？",
        "一瞬间是多久？",
        "太阳光多久到地球，我们看见的阳光是多久前的？",
    };

    private readonly List<List<string>> optionList = new List<List<string>>()
    {
        new List<string>(){ "A.2月", "B.闰月", "C.每个月都有" },
        new List<string>(){ "A.月球绕地球一圈", "B.地球自转一圈", "C.地球绕太阳一圈" },
        new List<string>(){ "A.月亮绕地球一圈", "B.地球自转一圈", "C.地球绕太阳一圈" },
        new List<string>(){ "A.太阳", "B地球", "C.月亮" },
        new List<string>(){ "A.上午", "B.下午", "C.全天" },
        new List<string>(){ "A.6:10", "B.5:50", "C.10:06" },
        new List<string>(){ "A.月球绕地球一圈", "B.地球自转一圈", "C.地球绕太阳一天" },
        new List<string>(){ "A.月的阴晴周期", "B.太阳的周期", "C.地球的周期" },
        new List<string>(){ "A.没有特定概念", "B.指光在真空中移动1cm的时间", "C.月光到地球的时间" },
        new List<string>(){ "A.现在的", "B.1年前的", "C.8小时前" },
    };

    private readonly int[] answer = { 0, 2, 0, 2, 1, 1, 1, 0, 0, 2 };

    int currentQuestion;
    public override void InitUI()
    {
        base.InitUI();
        bgLoader = SearchChild("n0").asLoader;
        timeText = SearchChild("n6").asTextField;
        numText = SearchChild("n7").asTextField;
        questionText = SearchChild("n8").asTextField;
        startCom = SearchChild("n12").asCom;
        aniCom = startCom.GetChild("n2").asCom;
        aniCom.visible = false;
        transition = aniCom.GetTransition("t0");
        bgGraph = SearchChild("n18").asGraph;
        for (int i = 0; i < 3; i++)
        {
            int index = i;
            GButton gButton = SearchChild("n" + (14 + i)).asButton;
            btnList.Add(gButton);
            gButton.onClick.Set(() => { OnClickAnswer(index); });
        }
        maskGraph = SearchChild("n13").asGraph;
        maskGraph.visible = false;
        maskGraph.sortingOrder = 1;
        startCom.sortingOrder = 1;
        FXMgr.CreateEffectWithGGraph(bgGraph, new Vector2(361, 815), "UI_framestar");
        InitEvent();
    }

    public override void InitEvent()
    {
        base.InitEvent();
        //返回
        SearchChild("n2").onClick.Set(() =>
        {
            Timers.inst.Remove(CountDown);

            EventMgr.Ins.DispachEvent(EventConfig.STORY_BREACK_STORY);
        });
        //skip
        SearchChild("n4").onClick.Set(() =>
        {
            Timers.inst.Remove(CountDown);

            SkipGame();
        });
        //tips
        SearchChild("n3").onClick.Set(OnClickTips);

        startCom.onClick.Set(OnClickStart);

        EventMgr.Ins.ReplaceEvent(EventConfig.STORY_GAME_QUIT, Destroy<GameAnswerView>);
        EventMgr.Ins.ReplaceEvent(EventConfig.STORY_DELETE_GAME_INFO, DeleteGameInfo);
    }

    public override void InitData<D>(D data)
    {
        base.InitData(data);
        storyGameInfo = data as StoryGameInfo;

        bgLoader.url = UrlUtil.GetGameBGUrl(24);
        consumeConfigs = JsonConfig.GameNodeConsumeConfigs.FindAll(a => a.node_id == storyGameInfo.gamePointConfig.id && a.type == storyGameInfo.gamePointConfig.type);
    }

    public override void InitData()
    {
        base.InitData();
        bgLoader.url = UrlUtil.GetGameBGUrl(24);
    }

    private void SetQuestion(int index)
    {
        numText.text = "第" + (index + 1) + "/10题";
        questionText.text = questionList[index];
        for (int i = 0; i < 3; i++)
        {
            btnList[i].title = optionList[index][i];
        }
    }

    void OnClickAnswer(int index)
    {
        StopCoroutine("GetTip");
        maskGraph.visible = false;
        btnList[answer[currentQuestion]].sortingOrder = 0;

        if (index == answer[currentQuestion])
        {
            if (currentQuestion == 9)
            {
                OnComplete();
                return;
            }
            Extrand extrand = new Extrand()
            {
                type = 0,
                callBack = () =>
                {
                    UIMgr.Ins.HideView<GameAnswerTipsView>();
                    currentQuestion++;
                    SetQuestion(currentQuestion);
                },
        };
            UIMgr.Ins.showNextPopupView<GameAnswerTipsView, Extrand>(extrand);
        }
        else
        {
            Timers.inst.Remove(CountDown);

            Extrand extrand = new Extrand()
            {
                type = 2,
                callBack = () =>
                {
                    Timers.inst.Remove(CountDown);

                    GameFaile();
                    UIMgr.Ins.HideView<GameAnswerTipsView>();
                },
                extrand = RequestResurrection,
                item = consumeConfigs[2].Pay,

            };
            UIMgr.Ins.showNextPopupView<GameAnswerTipsView, Extrand>(extrand);
        }
    }

    int totalTime;
    void CountDown(object param)
    {
        if (totalTime == 0)
        {
            Timers.inst.Remove(CountDown);
            StopCoroutine("GetTip");
            maskGraph.visible = false;
            btnList[answer[currentQuestion]].sortingOrder = 0;
            timeText.text = totalTime + "秒";

            Extrand extrand = new Extrand()
            {
                type = 1,
                callBack = () =>
                {
                    Timers.inst.Remove(CountDown);

                    GameFaile();
                    UIMgr.Ins.HideView<GameAnswerTipsView>();
                },
                extrand = RequestAddtime,
                item = consumeConfigs[1].Pay,

            };
            UIMgr.Ins.showNextPopupView<GameAnswerTipsView, Extrand>(extrand);


            return;
        }
        timeText.text = totalTime + "秒";
        totalTime--;
    }

    void StartRotate()
    {
        totalTime = 40;
        timeText.text = totalTime + "秒";
        totalTime--;
        Timers.inst.Add(1f, 0, CountDown);
    }

    void GameFaile()
    {
        startCom.visible = true; ;
        startCom.onClick.Set(OnClickStart);

    }

    void OnClickStart()
    {
        startCom.onClick.Clear();
        StartCoroutine(StartEffect());
    }

    IEnumerator StartEffect()
    {
        aniCom.visible = true;
        transition.Play();
        if (countdown == null)
            countdown = Resources.Load(SoundConfig.GetGameAudioUrl(SoundConfig.GameAudioId.Countdown)) as AudioClip;
        GRoot.inst.PlayEffectSound(countdown);

        yield return new WaitForSeconds(3.1f);
        aniCom.visible = false;
        startCom.visible = false;
        StartRotate();
        currentQuestion = 0;
        SetQuestion(0);
    }

    private void OnClickTips()
    {
        StartCoroutine(GetTip());
        //Extrand extrand = new Extrand();
        //extrand.type = 1;
        //extrand.key = "提示";
        //GameNodeConsumeConfig list = JsonConfig.GameNodeConsumeConfigs.Find(a => a.node_id == storyGameInfo.gamePointConfig.id && a.type == storyGameInfo.gamePointConfig.type);
        //TinyItem item = ItemUtil.GetTinyItem(list.pay);
        //extrand.item = item;
        //extrand.msg = "获得提示";
        //extrand.callBack = RequestGetTips;

        //UIMgr.Ins.showNextPopupView<PopupDialogView, Extrand>(extrand);

    }

    void RequestGetTips()
    {
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("nodeId", storyGameInfo.gamePointConfig.id);
        wWWForm.AddField("type", storyGameInfo.gamePointConfig.type);
        wWWForm.AddField("index", 0);
        GameMonoBehaviour.Ins.RequestInfoPost<PlayerProperty>(NetHeaderConfig.STROY_STEP_CONSUME, wWWForm, () => { StartCoroutine(GetTip()); });
    }

    IEnumerator GetTip()
    {
        maskGraph.visible = true;
        btnList[answer[currentQuestion]].sortingOrder = 2;
        yield return new WaitForSeconds(3f);
        maskGraph.visible = false;
        btnList[answer[currentQuestion]].sortingOrder = 0;

    }

    /// <summary>
    /// 请求再答一次
    /// </summary>
    void RequestResurrection()
    {
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("nodeId", storyGameInfo.gamePointConfig.id);
        wWWForm.AddField("type", storyGameInfo.gamePointConfig.type);
        wWWForm.AddField("index", 1);
        GameMonoBehaviour.Ins.RequestInfoPost<PlayerProperty>(NetHeaderConfig.STROY_STEP_CONSUME, wWWForm, Resurrection);
    }

    void Resurrection()
    {
        UIMgr.Ins.HideView<GameAnswerTipsView>();
        Timers.inst.Add(1f, 0, CountDown);

    }

    /// <summary>
    /// 请求加时
    /// </summary>
    void RequestAddtime()
    {
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("nodeId", storyGameInfo.gamePointConfig.id);
        wWWForm.AddField("type", storyGameInfo.gamePointConfig.type);
        wWWForm.AddField("index", 2);
        GameMonoBehaviour.Ins.RequestInfoPost<PlayerProperty>(NetHeaderConfig.STROY_STEP_CONSUME, wWWForm, Addtime);
    }

    void Addtime()
    {
        UIMgr.Ins.HideView<GameAnswerTipsView>();
        totalTime += 10;
        Timers.inst.Add(1f, 0, CountDown);

    }

}

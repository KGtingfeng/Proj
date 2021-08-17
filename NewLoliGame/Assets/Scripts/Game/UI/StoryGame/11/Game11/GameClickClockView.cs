using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

[ViewAttr("Game/UI/Y_Game11", "Y_Game11", "Game")]
public class GameClickClockView : BaseGameView
{
    const int GAME_TIME = 26;
    GLoader bgLoader1;
    GLoader bgLoader2;
    GLoader bgLoader3;
    GLoader bgLoader4;
    GComponent gProgress;
    GImage progressImage;
    Transition transition;
    GComponent transitionCom;
    GComponent kong;
    AudioClip countdown;
    AudioClip failAudio;
    AudioClip putDownAudio;

    const string url = "ui://wakg4v19cn0hg";

    bool isStart;
    public override void InitUI()
    {
        base.InitUI();
        bgLoader1 = SearchChild("n9").asLoader;
        bgLoader2 = SearchChild("n10").asLoader;
        bgLoader3 = SearchChild("n11").asLoader;
        bgLoader4 = SearchChild("n12").asLoader;
        gProgress = SearchChild("n0").asCom;
        progressImage = gProgress.GetChild("n5").asCom.GetChild("n5").asImage;
        transitionCom = SearchChild("n4").asCom.GetChild("n3").asCom;
        transition = transitionCom.GetTransition("t0");
        transitionCom.visible = false;
        kong = SearchChild("n14").asCom;
        controller = ui.GetController("c1");
        InitEvent();
    }

    public override void InitEvent()
    {
        base.InitEvent();
        //返回
        SearchChild("n3").onClick.Set(() =>
        {
            EventMgr.Ins.DispachEvent(EventConfig.STORY_BREACK_STORY);
        });
        //skip
        SearchChild("n2").onClick.Set(SkipGame);
        //tips
        SearchChild("n1").onClick.Set(OnClickTips);

        SearchChild("n4").onClick.Set(() =>
        {
            if (!isStart)
                StartCoroutine("StartGame");
        });

        EventMgr.Ins.ReplaceEvent(EventConfig.STORY_GAME_QUIT, Destroy<GameClickClockView>);
        EventMgr.Ins.ReplaceEvent(EventConfig.STORY_DELETE_GAME_INFO, DeleteGameInfo);

    }


    public override void InitData<D>(D data)
    {
        base.InitData(data);
        storyGameInfo = data as StoryGameInfo;
        bgLoader1.url = UrlUtil.GetGameBGUrl(26);
        bgLoader2.url = UrlUtil.GetGameBGUrl(27);
        bgLoader3.url = UrlUtil.GetGameBGUrl(28);
        bgLoader4.url = UrlUtil.GetGameBGUrl(29);

    }


    public override void InitData()
    {
        base.InitData();
        bgLoader1.url = UrlUtil.GetGameBGUrl(26);
        bgLoader2.url = UrlUtil.GetGameBGUrl(27);
        bgLoader3.url = UrlUtil.GetGameBGUrl(28);
        bgLoader4.url = UrlUtil.GetGameBGUrl(29);
    }


    void InitGame()
    {
        progressImage.y = 481;
        progressImage.TweenMoveY(0, GAME_TIME).SetEase(EaseType.Linear);
        bgLoader1.y = 0;
        bgLoader2.y = 1682;
        bgLoader3.y = 3393;
        bgLoader4.y = 5106;
        River(bgLoader1);
        River(bgLoader2);
        River(bgLoader3);
        River(bgLoader4);
    }

    GGraph idle;
    GGraph ding;
    IEnumerator StartGame()
    {
        SearchChild("n2").visible = false;
        SearchChild("n1").visible = false;
        isStart = true;
        transition.Play();
        transitionCom.visible = true;
        if (countdown == null)
            countdown = Resources.Load(SoundConfig.GetGameAudioUrl(SoundConfig.GameAudioId.Countdown)) as AudioClip;
        GRoot.inst.PlayEffectSound(countdown);

        yield return new WaitForSeconds(3.1f);
        transitionCom.visible = false;

        InitGame();
        SwitchController(1);
        for (int i = 0; i < 4; i++)
        {
            GComponent gCom = CreateFromPool();
            gCom.onClick.Set(GameFail);
            idle = FXMgr.CreateEffectWithScale(gCom, new Vector3(-99, -537), "G11_idle", 162, 7);
            FXMgr.CreateEffectWithScale(gCom, new Vector3(-99, -537), "G11_xing1", 162, 4);
            FXMgr.CreateEffectWithScale(gCom, new Vector3(-99, -537), "G11_xing2", 162, 4);
            FXMgr.CreateEffectWithScale(gCom, new Vector3(-99, -537), "G11_xing3", 162, 4);
            ding = FXMgr.CreateEffectWithScale(gCom, new Vector3(-99, -537), "G11_tap_di", 162, 7);

            gCom.scale = Vector2.one * 0.2f;
            gCom.TweenMoveY(1000, 6);
            gCom.TweenScale(Vector2.one, 4);

            yield return new WaitForSeconds(4);

            gCom.onClick.Set(() =>
            {

                StartCoroutine(OnClickClock(gCom));
            });
            StartCoroutine("WaitClick");

            yield return new WaitForSeconds(2.5f);

        }

        yield return new WaitForSeconds(2f);
        Finish();
    }

    IEnumerator WaitClick()
    {

        yield return new WaitForSeconds(2.1f);
        GameFail();
    }


    void GameFail()
    {
        if (failAudio == null)
            failAudio = Resources.Load(SoundConfig.GAME_AUDIO_EFFECT_URL + (int)SoundConfig.GameAudioId.Error) as AudioClip;
        GRoot.inst.PlayEffectSound(failAudio);
        StopAllCoroutines();
        TweenManager.KillAllTween();
        SwitchController(0);
        SearchChild("n2").visible = true;
        SearchChild("n1").visible = true;
        foreach (GObject gObject in components)
        {
            kong.RemoveChild(gObject);
            gObject.Dispose();
        }
        components.Clear();
        isStart = false;
        Extrand extrand = new Extrand()
        {
            type = 1,
            callBack = SkipGame,
        };
        UIMgr.Ins.showNextPopupView<Game11TipsView, Extrand>(extrand);
    }


    IEnumerator OnClickClock(GComponent gComponent)
    {
        gComponent.onClick.Clear();
        FXMgr.CreateEffectWithScale(gComponent, new Vector3(-99, -537), "G11_tap_ding", 162, 3);
        if (putDownAudio == null)
            putDownAudio = Resources.Load(SoundConfig.GAME_AUDIO_EFFECT_URL + (int)SoundConfig.GameAudioId.ClickClock) as AudioClip;
        GRoot.inst.PlayEffectSound(putDownAudio);
        StopCoroutine("WaitClick");
        gComponent.GetChild("n14").visible = false;
        gComponent.GetChild("n15").visible = false;
        gComponent.GetChild("n16").visible = false;
        gComponent.GetChild("n17").visible = false;
        idle.visible = false;
        ding.visible = false;
        yield return new WaitForSeconds(2);
        gComponent.visible = false;
    }


    /// <summary>
    /// 游戏过关
    /// </summary>
    void Finish()
    {
        StopAllCoroutines();
        TweenManager.KillAllTween();
        
        GameTipsInfo info = new GameTipsInfo()
        {
            context = "你已经找到了所有关键时间!",
            callBack = OnComplete
        };
        UIMgr.Ins.showNextPopupView<GameSuccessView, GameTipsInfo>(info);
    }

    /// <summary>
    /// 背景流动
    /// </summary>
    void River(GLoader gLoader)
    {
        gLoader.TweenMoveY(gLoader.y - 1682, 10).SetEase(EaseType.Linear).OnComplete(() =>
        {
            //Debug.LogError(gLoader.gameObjectName + "   " + gLoader.y);
            //if (gLoader.y <= -1682)
            //{
            //    gLoader.y = 5106;
            //}
            River(gLoader);
        });
    }

    List<GComponent> components = new List<GComponent>();
    public GComponent CreateFromPool()
    {
        GLoader gObject;
        gObject = new GLoader();
        gObject.url = url;
        GComponent go = gObject.component;
        go.sortingOrder = 0;
        gObject.sortingOrder = 0;
        kong.AddChild(gObject);
        int random = Random.Range(0, 8);

        go.y = -500;
        go.x = 10 + random * 50;
        components.Add(go);
        return go;
    }


    void OnClickTips()
    {
        Extrand extrand = new Extrand()
        {
            type = 0,
        };
        UIMgr.Ins.showNextPopupView<Game11TipsView, Extrand>(extrand);
    }
}

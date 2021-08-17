using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

[ViewAttr("Game/UI/Y_Game7", "Y_Game7", "Game")]
public class GameConnectJinmaiView : BaseGameView
{

    private GLoader bgTop;
    private GLoader bgBottom;
    private GButton startBtn;
    private Transition transition;
    private Transition transitionBg;
    private GGraph tipGraph;
    GGraph lineGraph;
    AudioClip clickFail;
    AudioClip clickSuccess;
    AudioClip connectJinmai;

    private readonly List<float> transitionList = new List<float>()
    {
        0,1.3f,2.7f,4.167f,5.6f,6.4f
    };

    List<GComponent> clicks = new List<GComponent>();

    //private readonly List<Vector2> jinMaiPos = new List<Vector2>()
    //{
    //    new Vector2(60,-196),
    //    new Vector2(-145,402),
    //    new Vector2(-208,-40),
    //    new Vector2(-67,307),
    //    new Vector2(59,498),
    //};

    public override void InitUI()
    {
        base.InitUI();
        bgTop = SearchChild("n9").asCom.GetChild("n9").asCom.GetChild("n0").asLoader;
        bgBottom = SearchChild("n9").asCom.GetChild("n9").asCom.GetChild("n9").asLoader;
        startBtn = SearchChild("n4").asButton;
        tipGraph = SearchChild("n11").asGraph;
        controller = ui.GetController("c1");
        transition = SearchChild("n9").asCom.GetChild("n9").asCom.GetTransition("t0");
        transition.Play();
        transition.SetPaused(true);
        transitionBg = SearchChild("n9").asCom.GetTransition("t0");
        transitionBg.Play();
        transitionBg.SetPaused(true);
        lineGraph = SearchChild("n9").asCom.GetChild("n9").asCom.GetChild("n10").asGraph;
        FXMgr.CreateEffectWithGGraph(lineGraph, new Vector2(0, 0), "G7_line");
        SetJinMai();




        InitEvent();
    }

    private void SetJinMai()
    {
        for (int i = 0; i < 5; i++)
        {

            int index = i;
            GComponent gComponent = SearchChild("n9").asCom.GetChild("n9").asCom.GetChild("n" + (i + 11)).asCom;
            gComponent.onClick.Set(() => { OnClickJinMai(index); });
            clicks.Add(gComponent);

        }
    }

    public override void InitEvent()
    {
        base.InitEvent();
        //返回
        SearchChild("n1").onClick.Set(() =>
        {
            EventMgr.Ins.DispachEvent(EventConfig.STORY_BREACK_STORY);
        });
        //skip
        SearchChild("n3").onClick.Set(SkipGame);
        //tips
        SearchChild("n2").onClick.Set(OnClickTips);

        startBtn.onClick.Set(OnClickStart);

        EventMgr.Ins.ReplaceEvent(EventConfig.STORY_GAME_QUIT, Destroy<GameConnectJinmaiView>);
        EventMgr.Ins.ReplaceEvent(EventConfig.STORY_DELETE_GAME_INFO, DeleteGameInfo);
    }

    public override void InitData<D>(D data)
    {
        base.InitData(data);
        storyGameInfo = data as StoryGameInfo;
        bgTop.url = UrlUtil.GetGameBGUrl(22);
        bgBottom.url = UrlUtil.GetGameBGUrl(23);

    }

    public override void InitData()
    {
        base.InitData();
        bgTop.url = UrlUtil.GetGameBGUrl(22);
        bgBottom.url = UrlUtil.GetGameBGUrl(23);
    }

    private void OnClickStart()
    {
        controller.selectedIndex = 1;
        isStart = true;
        StartCoroutine(NextPoint(0));
    }

    bool canConnect;
    bool isStart;
    int currentJinMai;
    private IEnumerator JinMaiEffect()
    {
        canConnect = false;
        if (currentJinMai == 0)
        {
            FXMgr.CreateEffectWithScale(clicks[currentJinMai], new Vector3(71, 78, 0), "Game7_tap2s");

        }
        else
        {
            FXMgr.CreateEffectWithScale(clicks[currentJinMai], new Vector3(71, 78, 0), "Game7_tap3s");

        }
        yield return new WaitForSeconds(2f);
        canConnect = true;
        Debug.LogError("canConnect");
        yield return new WaitForSeconds(1.5f);
        GameFaile();
    }

    private void OnClickJinMai(int index)
    {
        if (index == currentJinMai && isStart)
        {
            StopCoroutine("JinMaiEffect");
            if (canConnect)
            {
                if (clickSuccess == null)
                    clickSuccess = Resources.Load(SoundConfig.GAME_AUDIO_EFFECT_URL + (int)SoundConfig.GameAudioId.ClickSuccess) as AudioClip;
                GRoot.inst.PlayEffectSound(clickSuccess);
                FXMgr.CreateEffectWithScale(clicks[index], new Vector3(71, 78, 0), "Game7_tap_after");
                if (index == 4)
                {
                    Extrand extrand = new Extrand()
                    {
                        type = 2,
                        callBack = () =>
                        {
                            TweenManager.KillAllTween();
                            UIMgr.Ins.HideView<GameConnectJinmaiTipsView>();
                            OnComplete();
                        },
                    };
                    UIMgr.Ins.showNextPopupView<GameConnectJinmaiTipsView, Extrand>(extrand);

                }
                else
                {
                    StartCoroutine(NextPoint(index + 1));

                }

            }
            else
            {
                if (clickFail == null)
                    clickFail = Resources.Load(SoundConfig.GAME_AUDIO_EFFECT_URL + (int)SoundConfig.GameAudioId.ClickFail) as AudioClip;
                GRoot.inst.PlayEffectSound(clickFail);
                GameFaile();
            }
            canConnect = false;

        }
    }

    private IEnumerator NextPoint(int index)
    {
        Transform go = lineGraph.displayObject.gameObject.transform.Find("G7_line(Clone)");
        ChangeStartcolor(go, 1f);
        Transform t = go.Find("Particle System");
        ChangeStartcolor(t, 1f);
        if (connectJinmai == null)
            connectJinmai = Resources.Load(SoundConfig.GAME_AUDIO_EFFECT_URL + (int)SoundConfig.GameAudioId.ConnectJinmai) as AudioClip;
        GRoot.inst.PlayEffectSound(connectJinmai);
        currentJinMai = index;
        transition.SetPaused(false);
        transitionBg.SetPaused(false);
        yield return new WaitForSeconds(transitionList[index + 1] - transitionList[index]);
        transition.SetPaused(true);
        transitionBg.SetPaused(true);
        StartCoroutine("JinMaiEffect");


        totalTime = 10;
        Timers.inst.Add(0.1f, 0, CountDown);

    }



    int totalTime;
    void CountDown(object param)
    {
        totalTime -= 2;
        Transform go = lineGraph.displayObject.gameObject.transform.Find("G7_line(Clone)");
        ChangeStartcolor(go, 0.1f * totalTime);
        Transform t = go.Find("Particle System");
        ChangeStartcolor(t, 0.1f * totalTime);


        if (totalTime == 0)
        {
            Timers.inst.Remove(CountDown);

            return;
        }
    }

    void ChangeStartcolor(Transform t, float a)
    {
        ParticleSystem particle = t.GetComponent<ParticleSystem>();
        var main = particle.main;
        ParticleSystem.MinMaxGradient startColor = main.startColor;
        Color color = startColor.color;
        color.a = a;
        startColor.color = color;
        main.startColor = startColor;
    }


    private void GameFaile()
    {
        controller.selectedIndex = 0;
        transition.Stop();
        transition.Play();
        transition.SetPaused(true);
        transitionBg.Stop();
        transitionBg.Play();
        transitionBg.SetPaused(true);
        isStart = false;
        Extrand extrand = new Extrand()
        {
            type = 1,
            callBack = () =>
            {
                GotoNextNode();
                UIMgr.Ins.HideView<GameConnectJinmaiTipsView>();
            },
        };
        UIMgr.Ins.showNextPopupView<GameConnectJinmaiTipsView, Extrand>(extrand);

    }

    private void OnClickTips()
    {
        Extrand extrand = new Extrand()
        {
            type = 0,
            callBack = () => { UIMgr.Ins.HideView<GameConnectJinmaiTipsView>(); },
        };

        UIMgr.Ins.showNextPopupView<GameConnectJinmaiTipsView, Extrand>(extrand);
    }

}

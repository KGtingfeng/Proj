using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

[ViewAttr("Game/UI/Y_Game16", "Y_Game16", "Game")]
public class GameAviodView : BaseGameView
{
    const int GAME_TIME = 30;
    public static GameAviodView instance;
    GComponent sceneCom;
    GButton returnBtn;
    GButton skipBtn;
    GButton tipsBtn;
    GLoader bg0;
    GLoader bg1;
    GLoader bg2;
    GTextField startTips;
    Transition transition;
    GComponent transitionCom;
    GComponent clickBg;
    GObject role;

    AudioClip countdown;
    SwipeGesture swipe;
    GProgressBar gProgress;
    GImage progressImage;
    AudioClip failAudio;

    Vector2 pos;

    public override void InitUI()
    {
        base.InitUI();
        instance = this;
        sceneCom = SearchChild("n7").asCom;
        returnBtn = SearchChild("n4").asButton;
        skipBtn = SearchChild("n6").asButton;
        tipsBtn = SearchChild("n5").asButton;
        bg0 = sceneCom.GetChild("n0").asLoader;
        bg1 = sceneCom.GetChild("n1").asLoader;
        bg2 = sceneCom.GetChild("n2").asLoader;
        startTips = sceneCom.GetChild("n11").asCom.GetChild("n2").asTextField;
        transitionCom = sceneCom.GetChild("n11").asCom.GetChild("n3").asCom;
        transition = transitionCom.GetTransition("t0");
        transitionCom.visible = false;
        startTips.text = "蓝色光线很危险，和舒言一起小心躲避它们！闪电球会突然发射光线，注意及时闪避！点击屏幕开始游戏！";
        controller = sceneCom.GetController("c1");
        clickBg = sceneCom.GetChild("n12").asCom;
        role = sceneCom.GetChild("n5");
        gProgress = sceneCom.GetChild("n9").asProgress;
        progressImage = gProgress.GetChild("n5").asCom.GetChild("n5").asImage;
        pos = role.position;

        role.displayObject.gameObject.AddComponent<RoleItem>();

        InitEvent();
    }

    public override void InitEvent()
    {
        base.InitEvent();
        //返回
        returnBtn.onClick.Set(() =>
        {
            EventMgr.Ins.DispachEvent(EventConfig.STORY_BREACK_STORY);
        });
        //skip
        skipBtn.onClick.Set(SkipGame);
        //tips
        tipsBtn.onClick.Set(OnClickTips);

        sceneCom.GetChild("n11").onClick.Set(() => { StartCoroutine("StartGame"); });

        swipe = new SwipeGesture(clickBg);
        swipe.onBegin.Set(OnSwipeBegin);
        swipe.onMove.Set(OnSwipeMove);

        EventMgr.Ins.ReplaceEvent(EventConfig.STORY_GAME_QUIT, Destroy<GameAviodView>);
        EventMgr.Ins.ReplaceEvent(EventConfig.STORY_DELETE_GAME_INFO, DeleteGameInfo);

    }

    public override void InitData<D>(D data)
    {
        base.InitData(data);
        storyGameInfo = data as StoryGameInfo;


        bg0.url = UrlUtil.GetGameBGUrl(33);
        bg1.url = UrlUtil.GetGameBGUrl(34);
        bg2.url = UrlUtil.GetGameBGUrl(35);
    }

    public override void InitData()
    {
        base.InitData();
        bg0.url = UrlUtil.GetGameBGUrl(33);
        bg1.url = UrlUtil.GetGameBGUrl(34);
        bg2.url = UrlUtil.GetGameBGUrl(35);
    }

    void InitGame()
    {
        progressImage.y = 481;
        progressImage.TweenMoveY(0, GAME_TIME + 4).SetEase(EaseType.Linear);
        bg0.y = -3206;
        bg1.y = -1603;
        bg2.y = 0;
        River(bg0);
        River(bg1);
        River(bg2);

    }


    IEnumerator StartGame()
    {
        sceneCom.GetChild("n11").onClick.Clear();
        role.position = pos;
        transition.Play();
        transitionCom.visible = true;
        if (countdown == null)
            countdown = Resources.Load(SoundConfig.GetGameAudioUrl(SoundConfig.GameAudioId.Countdown)) as AudioClip;
        GRoot.inst.PlayEffectSound(countdown);

        yield return new WaitForSeconds(3.1f);
        transitionCom.visible = false;
        InitGame();
        SwitchController(1);
        for (int i = 0; i < 3; i++)
        {
            StartCoroutine("CreateLie");
            for (int j = 0; j < 5; j++)
            {
                CreateFromPool();
                yield return new WaitForSeconds(2f);
            }
        }

        GameFinish();
    }

    void GameFinish()
    {
        TweenManager.KillAllTween();

        controller.selectedIndex = 2;
        clickBg.onClick.Set(OnComplete);
    }

    public void GameFail()
    {
        if (failAudio == null)
            failAudio = Resources.Load(SoundConfig.GAME_AUDIO_EFFECT_URL + (int)SoundConfig.GameAudioId.Error) as AudioClip;
        GRoot.inst.PlayEffectSound(failAudio);
        TweenManager.KillAllTween();
        StopAllCoroutines();
        SwitchController(0);
        foreach (GObject gObject in components)
        {
            sceneCom.RemoveChild(gObject);
            gObject.Dispose();
        }
        components.Clear();
        sceneCom.GetChild("n11").onClick.Set(() => { StartCoroutine("StartGame"); });

        if (gameTipsInfo == null)
            gameTipsInfo = new GameTipsInfo();
        gameTipsInfo.isShowBtn = true;
        gameTipsInfo.context = "你被蓝色光线击中了!";
        gameTipsInfo.callBack = () =>
        {
            SwitchController(0);
        };
        gameTipsInfo.skip = () =>
        {
            SkipGame();
        };
        UIMgr.Ins.showNextPopupView<GameFailTipsView, GameTipsInfo>(gameTipsInfo);

    }

    float shipX;
    float shipY;
    void OnSwipeBegin()
    {
        shipX = role.x;
        shipY = role.y;
    }

    void OnSwipeMove()
    {

        Vector2 move = swipe.position;
        if (shipX + move.x > 0 && shipX + move.x < GRoot.inst.width - 180)
        {
            role.x = shipX + move.x;

        }
        if (shipY + move.y > 500 && shipY + move.y < 1450)
            role.y = shipY + move.y;

    }


    /// <summary>
    /// 背景流动
    /// </summary>
    void River(GLoader gLoader)
    {
        gLoader.TweenMoveY(gLoader.y + 1603, 16).SetEase(EaseType.Linear).OnComplete(() =>
        {
            if (gLoader.y == 1603)
                gLoader.y = -3206;
            River(gLoader);
        });
    }

    const string objUrl = "ui://14eg8dl0n8t1b";
    const string lieUrl = "ui://14eg8dl0suu9d";
    List<GObject> components = new List<GObject>();
    public GComponent CreateFromPool()
    {
        GLoader gObject;
        gObject = new GLoader();
        gObject.url = objUrl;
        GComponent go = gObject.component;
        go.size = new Vector2(50, 50);
        go.SetPivot(0.5f, 0.5f);
        go.pivotAsAnchor = true;
        go.sortingOrder = 0;
        gObject.sortingOrder = 0;
        sceneCom.AddChildAt(gObject, 4);
        int random = Random.Range(0, 3);
        BlueLineItem blueLineItem = go.displayObject.gameObject.AddComponent<BlueLineItem>();
        GGraph gGraph = FXMgr.CreateEffectWithScale(go, new Vector2(16, 622), "G16_line");
        gGraph.rotation = 90;

        //if (random != 0)
        //{
        int ran = Random.Range(0, 8);
        go.y = -600;
        go.x = 50 + ran * 100;
        go.TweenMoveY(2000, 7).SetEase(EaseType.Linear);
        blueLineItem.Init(true);

        GLoader gObject1;
        gObject1 = new GLoader();
        gObject1.url = objUrl;
        GComponent go1 = gObject1.component;
        go1.size = new Vector2(50, 50);
        go1.SetPivot(0.5f, 0.5f);
        go1.pivotAsAnchor = true;
        go1.sortingOrder = 0;
        gObject1.sortingOrder = 0;
        sceneCom.AddChildAt(gObject1, 4);

        BlueLineItem blueLineItem1 = go1.displayObject.gameObject.AddComponent<BlueLineItem>();
        GGraph gGraph1 = FXMgr.CreateEffectWithScale(go1, new Vector2(16, 622), "G16_line");
        gGraph1.rotation = 90;
        go1.y = -600;
        go1.x = GRoot.inst.width - go.x;
        go1.TweenMoveY(2000, 7).SetEase(EaseType.Linear);
        blueLineItem1.Init(true);

        components.Add(gObject1);

        //}
        //else
        //{
        //    int ran = Random.Range(0, 8);
        //    go.rotation = -90;
        //    go.y = ran * 100;
        //    go.x = -500 - go.height;
        //    go.TweenMoveX(GRoot.inst.width + 100, 8);
        //    go.TweenMoveY(go.y + 1000, 6);
        //    blueLineItem.Init(false);
        //}
        components.Add(gObject);
        return go;
    }

    IEnumerator CreateLie()
    {

        GLoader gObject;
        gObject = new GLoader();
        gObject.url = lieUrl;

        GComponent gComponent = gObject.component;
        sceneCom.AddChildAt(gObject, 4);

        components.Add(gObject);
        gObject.y = -250;
        int random = Random.Range(0, 2);
        if (random == 0)
        {
            gObject.x = 750 - GRoot.inst.width;
        }
        else
        {
            gObject.x = GRoot.inst.width - 200;
        }
        gObject.TweenMoveY(2000, 13);
        //yield return new WaitForSeconds(1f);

        for (int i = 0; i < 2; i++)
        {
            CreateLieChild(gComponent, random != 0, i);
            yield return new WaitForSeconds(3f);

        }


    }

    public GComponent CreateLieChild(GComponent lie, bool isRight, int random)
    {
        GLoader gObject;
        gObject = new GLoader();
        gObject.url = objUrl;
        GComponent go = gObject.component;
        gObject.sortingOrder = 0;
        lie.AddChild(gObject);

        BlueLineItem blueLineItem = go.displayObject.gameObject.AddComponent<BlueLineItem>();
        GGraph gGraph = FXMgr.CreateEffectWithScale(go, new Vector2(19, 651), "G16_line");
        gGraph.rotation = 90;
        if (isRight)
        {
            if (random == 1)
            {
                go.rotation = 90;
                gObject.position = new Vector2(338, 120);
                // go.position = new Vector3(679, 104);
                go.TweenMoveX(go.position.x - 2000, 10);

            }
            else
            {
                go.rotation = 45;
                //go.position = new Vector3(496, -279);
                gObject.position = new Vector2(330, -91);
                go.TweenMoveY(go.position.y + 1000, 10);
                go.TweenMoveX(go.position.x - 1000, 10);

            }

            blueLineItem.Init(false, 2);
        }
        else
        {
            if (random == 1)
            {
                go.rotation = -90;
                //go.position = new Vector3(-427, -137);
                gObject.position = new Vector2(55, 118);
                go.TweenMoveX(go.position.x + 2000, 10);

            }
            else
            {
                go.rotation = -45f;
                //go.position = new Vector3(-275, -251);
                gObject.position = new Vector2(-181, -183);
                go.TweenMoveY(go.position.y + 1000, 10);
                go.TweenMoveX(go.position.x + 1000, 10);

            }

            blueLineItem.Init(false, 1);
        }
        return go;
    }

    GameTipsInfo gameTipsInfo;
    void OnClickTips()
    {
        if (gameTipsInfo == null)
            gameTipsInfo = new GameTipsInfo();
        gameTipsInfo.isShowBtn = false;
        gameTipsInfo.context = "操作头像，避开所有蓝色光线；左右的闪电球会突然发射光线，要注意闪避。";
        UIMgr.Ins.showNextPopupView<GameFailTipsView, GameTipsInfo>(gameTipsInfo);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using Spine.Unity;
using Spine;

[ViewAttr("Game/UI/Y_Game2", "Y_Game2", "Game3")]
public class GameJumpShipView : BaseGameView
{
    public static GameJumpShipView instance;

    readonly List<string> imageName = new List<string>
    {
        "ui://eq86i8lrsuo01q",
        "ui://eq86i8lrsuo01r",
    };
    const int GAME_TIME = 30;
    GComponent shipCom;
    GGraph shipGraph;
    GGraph finishShipGraph;
    GButton returnBtn;
    GButton skipBtn;
    GButton tipsBtn;

    GLoader bgLoader1;
    GLoader bgLoader2;
    GLoader bgLoader3;
    GLoader finishBg;
    GComponent childCom;

    GComponent clickBg;
    Transition transition;
    GComponent transitionCom;
    GProgressBar gProgress;
    GImage progressImage;

    GGraph huashui;

    GComponent moveAnime;
    GComponent clickAnime;

    SwipeGesture swipe;
    int stoneNum;
    public bool isFail;
    public bool isJump;
    bool isStart;
    bool isTurning;
    SkeletonAnimation shipAni;
    AudioClip audioClip;
    AudioClip countdown;
    ShipItem shipItem;

    string newbieKey = "Newbie";
    bool isNewbie;
    int newbieType;

    public override void InitUI()
    {
        instance = this;
        base.InitUI();
        controller = ui.GetController("c1");
        bgLoader1 = SearchChild("n1").asLoader;
        bgLoader2 = SearchChild("n2").asLoader;
        bgLoader3 = SearchChild("n3").asLoader;
        finishBg = SearchChild("n33").asLoader;
        returnBtn = SearchChild("n13").asButton;
        skipBtn = SearchChild("n28").asButton;
        tipsBtn = SearchChild("n27").asButton;
        shipCom = SearchChild("n29").asCom;
        shipGraph = shipCom.GetChild("n0").asGraph;
        finishShipGraph = SearchChild("n34").asCom.GetChild("n0").asGraph;
        clickBg = SearchChild("n38").asCom;
        childCom = SearchChild("n30").asCom;
        gProgress = SearchChild("n22").asProgress;
        progressImage = gProgress.GetChild("n5").asCom.GetChild("n5").asImage;
        transitionCom = SearchChild("n32").asCom.GetChild("n3").asCom;
        transition = transitionCom.GetTransition("t0");
        transitionCom.visible = false;

        moveAnime = SearchChild("n39").asCom;
        clickAnime = SearchChild("n42").asCom;
        moveAnime.visible = false;
        clickAnime.visible = false;

            huashui = FXMgr.CreateEffectWithScale(shipCom, new Vector2(-1, -1), "Game3_huashui", 80, -1);
        huashui.visible = true;
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
        SearchChild("n32").asCom.GetChild("n0").onClick.Set(() =>
        {
            if (!isStart)
            {
                StartCoroutine(StartGame());
                isStart = true;
            }
        });
        SearchChild("n34").onClick.Set(OnComplete);
        EventMgr.Ins.ReplaceEvent(EventConfig.STORY_GAME_QUIT, Destroy<GameJumpShipView>);
        EventMgr.Ins.RemoveEvent(EventConfig.STORY_DELETE_GAME_INFO);

        swipe = new SwipeGesture(clickBg);
        swipe.onBegin.Set(OnSwipeBegin);
        swipe.onMove.Set(OnSwipeMove);
        swipe.onEnd.Set(OnSwipeEnd);
        clickBg.onClick.Set(OnJump);
    }

    public override void InitData<D>(D data)
    {
        base.InitData(data);
        bgLoader1.url = UrlUtil.GetGameBGUrl(17);
        bgLoader2.url = UrlUtil.GetGameBGUrl(16);
        bgLoader3.url = UrlUtil.GetGameBGUrl(15);
        storyGameInfo = data as StoryGameInfo;
        
            stoneNum = (int)GRoot.inst.width / 150;
            shipAni = FXMgr.LoadSpineEffect("huachuan", shipGraph, new Vector2(), 60, "daiji");
            shipItem = shipCom.displayObject.gameObject.AddComponent<ShipItem>();
            shipGraph.sortingOrder = 1;
            shipCom.position = new Vector2(GRoot.inst.width / 2, 1350);

        if (storyGameInfo.gameSaves.Find(a => a.ckey == newbieKey) == null)
        {
            isNewbie = true;
        }
    }

    public override void InitData()
    {
        base.InitData();
        bgLoader1.url = UrlUtil.GetGameBGUrl(17);
        bgLoader2.url = UrlUtil.GetGameBGUrl(16);
        bgLoader3.url = UrlUtil.GetGameBGUrl(15);
        
            stoneNum = (int)GRoot.inst.width / 150;
            shipAni = FXMgr.LoadSpineEffect("huachuan", shipGraph, new Vector2(), 60, "daiji");
            shipItem = shipCom.displayObject.gameObject.AddComponent<ShipItem>();
            shipGraph.sortingOrder = 1;
            shipCom.position = new Vector2(GRoot.inst.width / 2, 1350);

            isNewbie = true;
    }

    void OnJump(EventContext context)
    {
        if (context.inputEvent.isDoubleClick && !isJump)
        {
            isJump = true;
            if (audioClip == null)
                audioClip = Resources.Load(SoundConfig.GetGameAudioUrl(SoundConfig.GameAudioId.Jump)) as AudioClip;
            GRoot.inst.PlayEffectSound(audioClip);
            shipAni.AnimationState.SetAnimation(0, "tiao", false);
            shipAni.timeScale = 2;
            shipItem.box.enabled = false;
            StartCoroutine(JumpTime());
            if (isNewbie && newbieType == 2)
            {
                newbieType = 3;
                clickAnime.visible = false;
                isNewbie = false;
                GameTool.SaveGameInfo(newbieKey, "1", storyGameInfo.gamePointConfig.id);
            }
        }
    }

    IEnumerator JumpTime()
    {
        huashui.visible = false;
        yield return new WaitForSeconds(0.7f);
        shipItem.box.enabled = true;
        isJump = false;
        shipX = shipCom.x;
        shipY = shipCom.y;
        lastX = 0;

        swipe.position = Vector2.zero;
        GGraph shuihua = CreateSplashFromPool();
        if (!isFail)
        {
            huashui.visible = true;
            shuihua.TweenMoveY(shuihua.y + 555.9f, 3).OnComplete(() =>
            {
                RemoveSplashToPool(shuihua);
            });
        }
    }

    float shipX;
    float shipY;
    float lastX;
    void OnSwipeBegin()
    {
        shipX = shipCom.x;
        shipY = shipCom.y;
        lastX = 0;
    }

    void OnSwipeMove()
    {
        if (!isJump && !isFail)
        {
            Vector2 move = swipe.position;
            if (shipX + move.x * 0.7f > 50 && shipX + move.x * 0.7f < GRoot.inst.width - 30)
            {
                shipCom.x = shipX + move.x * 0.7f;
                if (lastX > swipe.position.x && !isTurning)
                {
                    StartCoroutine(TurnTo("zuo"));
                    if (isNewbie && newbieType == 0)
                    {
                        newbieType = 1;
                        moveAnime.GetController("c1").selectedIndex = 1;
                    }
                }
                else if (lastX < swipe.position.x && !isTurning)
                {
                    StartCoroutine(TurnTo("you"));
                    if (isNewbie && newbieType == 1)
                    {
                        newbieType = 2;
                        moveAnime.visible = false;
                        clickAnime.visible = true;
                    }
                }
            }
            if (shipY + move.y * 0.7f > 1000 && shipY + move.y * 0.7f < 1550)
                shipCom.y = shipY + move.y * 0.7f;
        }
    }

    void OnSwipeEnd()
    {
        StartCoroutine(TurnTo("daiji"));
    }

    IEnumerator TurnTo(string animationName)
    {
        isTurning = true;
        shipAni.AnimationState.SetAnimation(0, animationName, false);
        shipAni.timeScale = 1;
        lastX = swipe.position.x;
        yield return new WaitForSeconds(0.3f);
        shipAni.AnimationState.SetAnimation(0, "daiji", false);
        shipAni.timeScale = 1;
        isTurning = false;

    }

    void InitGame()
    {
        progressImage.y = 481;
        progressImage.TweenMoveY(0, GAME_TIME + 4).SetEase(EaseType.Linear);
        isFail = false;
        isJump = false;
        shipCom.displayObject.gameObject.transform.localRotation = new Quaternion(0, 0, 0, 0);
        shipGraph.rotation = 0;
        shipGraph.pivot = new Vector2(0.5f, 0.5f);
        shipCom.position = new Vector2(GRoot.inst.width / 2, 1350);

        returnBtn.visible = false;
        tipsBtn.visible = false;
        skipBtn.visible = false;
        bgLoader1.y = 0;
        bgLoader2.y = -1850;
        bgLoader3.y = -3700;
        River(bgLoader1);
        River(bgLoader2);
        River(bgLoader3);
        for (int i = stones.Count - 1; i >= 0; i--)
            RemoveToPool(stones[i]);
        for (int i = swirls.Count - 1; i >= 0; i--)
            RemoveSwirlToPool(swirls[i]);
        for (int i = splashs.Count - 1; i >= 0; i--)
            RemoveSplashToPool(splashs[i]);
        


    }

    WaitForSeconds waitTwoSeconds = new WaitForSeconds(2f);
    WaitForSeconds waitOneSeconds = new WaitForSeconds(1f);

    IEnumerator StartGame()
    {
        transition.Play();
        transitionCom.visible = true;
        if (countdown == null)
            countdown = Resources.Load(SoundConfig.GetGameAudioUrl(SoundConfig.GameAudioId.Countdown)) as AudioClip;
        GRoot.inst.PlayEffectSound(countdown);

        yield return new WaitForSeconds(3.1f);
        SwitchController(1);
        if (isNewbie)
        {
            newbieType = 0;
            moveAnime.visible = true;
            moveAnime.GetController("c1").selectedIndex = 0;
            yield return new WaitUntil(()=> { return newbieType == 3; });
        }

        InitGame();
        for (int i = 0; i < GAME_TIME; i++)
        {
            int type = GetBarrierType();
            if (type == 0)
            {
                CreateSwirlFromPool();
                yield return waitOneSeconds;//降低齿轮出现的频率，每次要生成齿轮前先等待一秒。
                i++;
                for (int j = 0; j < stoneNum; j++)
                    CreateFromPool(j);
            }
            else
            {
                CreateSwirlFromPool();
            }
            yield return waitOneSeconds;
        }
        yield return waitTwoSeconds;
        isJump = true;
        yield return waitTwoSeconds;

        Finish();
    }

    int oldType = 0;
    int times = 1;
    /// <summary>
    /// 获取障碍类型，每种类型最多连续出现3次
    /// </summary>
    /// <returns></returns>
    int GetBarrierType()
    {
        int type = Random.Range(0, 2);
        if (type == oldType)
        {
            if (times > 2)
            {
                type = type == 0 ? 1 : 0;
                oldType = type;
                times = 1;
            }
            else
                times++;
        }
        else
        {
            oldType = type;
            times = 1;
        }
        return type;
    }

    /// <summary>
    /// 背景流动
    /// </summary>
    void River(GLoader gLoader)
    {
        gLoader.TweenMoveY(gLoader.y + 1850, 5).SetEase(EaseType.Linear).OnComplete(() =>
        {
            if (gLoader.y == 1850)
                gLoader.y = -3700;
            River(gLoader);
        });
    }

    /// <summary>
    /// 撞到零件
    /// </summary>
    public void OnCollisionStone()
    {
        if (!isFail && !isJump)
        {
            StopAllCoroutines();
            TweenManager.KillAllTween();
            Fail();
        }
    }

    /// <summary>
    /// 撞到漩涡
    /// </summary>
    public void OnCollisionSwirl()
    {
        if (!isFail && !isJump)
        {
            StopAllCoroutines();
            TweenManager.KillAllTween();
            TweenLoop();
            Fail();
        }
    }

    void TweenLoop()
    {
        shipGraph.TweenRotate(shipGraph.rotation + 3600, 5).SetEase(EaseType.Linear).OnComplete(() =>
         {
             TweenLoop();
         });

    }

    /// <summary>
    /// 游戏失败
    /// </summary>
    public void Fail()
    {
        isFail = true;
        isStart = false;
        ShowButton();

        bgLoader1.y = 0;
        bgLoader2.y = -1850;
        bgLoader3.y = -3700;

        if (gameTipsInfo == null)
            gameTipsInfo = new GameTipsInfo();
        gameTipsInfo.isShowBtn = true;
        gameTipsInfo.callBack = () =>
        {
            TweenManager.KillAllTween();
            SwitchController(0);
        };
        gameTipsInfo.skip = () =>
        {
            TweenManager.KillAllTween();
            SkipGame();
        };
        UIMgr.Ins.showNextPopupView<GameFailTipsView, GameTipsInfo>(gameTipsInfo);
    }

    /// <summary>
    /// 游戏过关
    /// </summary>
    void Finish()
    {
        StopAllCoroutines();
        TweenManager.KillAllTween();
        ShowButton();
        StartCoroutine(ScenceChange());
    }

    void ShowButton()
    {

        huashui.visible = false;
        returnBtn.visible = true;
        tipsBtn.visible = true;
        skipBtn.visible = true;
    }

    IEnumerator ScenceChange()
    {
        UIMgr.Ins.showNextPopupView<GameScenceChangeView>();
        yield return waitOneSeconds;
        shipGraph.visible = false;
        finishBg.url = UrlUtil.GetGameBGUrl(14);
        SkeletonAnimation shipAnimation = FXMgr.LoadSpineEffect("Chuan", finishShipGraph, new Vector2(179, 12), 100, "chuan");
        FXMgr.CreateEffectWithGGraph(finishShipGraph, new Vector2(-421, 234), "Game2_daoying", 130);

        TrackEntry trackEntry0 = shipAnimation.AnimationState.SetAnimation(0, "chuan", true);
        trackEntry0.AnimationStart = 3f;
        trackEntry0.AnimationEnd = 6.3f;
        shipAnimation.timeScale = 1;
        controller.selectedIndex = 2;
    }

    #region Pool

    List<GObject> stonePool = new List<GObject>();
    List<GObject> stones = new List<GObject>();
    public void CreateFromPool(int i)
    {
        GLoader gObject;
        StoneItem item;
        int range = Random.Range(0, 6);
        if (stonePool.Count == 0)
        {
            gObject = new GLoader();
            gObject.url = imageName[0];
            GObject go = gObject.component;
            go.sortingOrder = 0;
            gObject.sortingOrder = 0;
            childCom.AddChild(gObject);
            item = gObject.displayObject.gameObject.AddComponent<StoneItem>();
        }
        else
        {
            gObject = stonePool[0].asLoader;
            stonePool.RemoveAt(0);
            item = gObject.displayObject.gameObject.GetComponent<StoneItem>();
        }
        gObject.visible = true;
        gObject.y = -400 + range * 40;
        gObject.x = 150 * i + 70;
        item.Init(gObject, range);

        stones.Add(gObject);

    }

    public void RemoveToPool(GObject gObject)
    {
        stones.Remove(gObject);
        gObject.visible = false;
        stonePool.Add(gObject);
    }

    List<GObject> swirlPool = new List<GObject>();
    List<GObject> swirls = new List<GObject>();
    public void CreateSwirlFromPool()
    {
        GLoader gObject;
        SwirlItem item;
        int range = Random.Range(70, (int)GRoot.inst.width - 150);
        if (swirlPool.Count == 0)
        {
            gObject = new GLoader();
            gObject.url = imageName[1];
            gObject.sortingOrder = 0;

            childCom.AddChild(gObject);
            item = gObject.displayObject.gameObject.AddComponent<SwirlItem>();
        }
        else
        {
            gObject = swirlPool[0].asLoader;
            swirlPool.RemoveAt(0);
            item = gObject.displayObject.gameObject.GetComponent<SwirlItem>();
        }
        gObject.visible = true;
        gObject.y = -400;
        gObject.x = range;
        item.Init(gObject);
        swirls.Add(gObject);
    }

    public void RemoveSwirlToPool(GObject gObject)
    {
        swirls.Remove(gObject);
        gObject.visible = false;
        swirlPool.Add(gObject);
    }

    List<GObject> splashPool = new List<GObject>();
    List<GObject> splashs = new List<GObject>();
    public GGraph CreateSplashFromPool()
    {
        GGraph gObject;
        if (splashPool.Count == 0)
        {
            gObject = FXMgr.CreateEffectWithScale(ui, shipCom.position, "Game3_shuihua", 80, -1, ui.GetChildIndex(childCom) - 1);
        }
        else
        {
            gObject = splashPool[0].asGraph;
            splashPool.RemoveAt(0);
        }
        gObject.position = shipCom.position;
        gObject.visible = true;
        splashs.Add(gObject);
        return gObject;
    }

    public void RemoveSplashToPool(GObject gObject)
    {
        splashs.Remove(gObject);
        gObject.visible = false;
        splashPool.Add(gObject);
    }
    #endregion


    GameTipsInfo gameTipsInfo;
    void OnClickTips()
    {
        if (gameTipsInfo == null)
            gameTipsInfo = new GameTipsInfo();
        gameTipsInfo.isShowBtn = false;
        UIMgr.Ins.showNextPopupView<GameFailTipsView, GameTipsInfo>(gameTipsInfo);
    }

    public override void SkipGame()
    {
        SwitchController(0);
        base.SkipGame();
    }
}

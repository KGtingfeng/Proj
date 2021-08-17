using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;
using Spine.Unity;

[ViewAttr("Game/UI/Y_Game18", "Y_Game18", "Game18")]
public class GameClickDefendsView : BaseGameView
{
    GButton backBtn;
    GButton skipBtn;
    GLoader bgLoader;
    GComponent sceneCom;
    GProgressBar defendBar;
    GTextField countDownText;

    GComponent tapAnime;
    //发射高度
    float lineStartY;
    //多出的防御值
    int exEnergy;
    //倒数
    float countDown;
    //游戏是否结束
    bool isEnd = false;
    bool isNewPlayerGame = false;

    GComponent talk;
    Transition transition;
    GTextField talkText;
    List<int> talkList = new List<int>();

    /// <summary>
    /// 每一次光线发射间断分为（射在途中的时长+cd间隔）
    /// </summary>


    //间隔
    float cdTime =1f;

    /// <summary>
    /// 能量条满能量是100，外加额外能量储存值
    /// </summary>

    //初始能量值
    int startEnergy = 20;

    //被攻击减少的能量
    int subEnergy =12;

    //点击一次储存的能量
    int addEnergy = 4;

    //储存能量值最大值
    int mixExEnergy = 24;


    bool isReStart;
    AudioClip attactAudio;
    AudioClip clickAudio;

    bool isNewbie;
    int newbieTimes;
    string newbieKey = "newbie";

    GComponent fxCom;
    GGraph nvGraph;
    GGraph xlGraph;
    GGraph fxGraph;
    SkeletonAnimation nv;
    SkeletonAnimation xl;

    Transition t;

    //用来辨别是否开始了下一把，以免重复调用闪电携程
    int life = 0;
    public override void InitUI()
    {
        base.InitUI();
        sceneCom = SearchChild("n0").asCom;

        bgLoader = SearchChild("n3").asLoader;
        controller = sceneCom.GetController("c1");
        backBtn = SearchChild("n1").asButton;
        skipBtn = SearchChild("n2").asButton;
        //defendImage = sceneCom.GetChild("n6").asImage;
        defendBar = sceneCom.GetChild("n0").asProgress;
        countDownText = sceneCom.GetChild("n17").asTextField;
        tapAnime = sceneCom.GetChild("n22").asCom;
        tapAnime.visible = false;
        fxCom = sceneCom.GetChild("n23").asCom;

        talk = sceneCom.GetChild("n5").asCom;
        talkText = talk.GetChild("n6").asTextField;
        transition = talk.GetTransition("t0");

        talk.visible = false;
        nvGraph = new GGraph();
        xlGraph = new GGraph();
        fxCom.AddChild(nvGraph);
        fxCom.AddChild(xlGraph);
        nv = FXMgr.LoadSpineEffect("posui", nvGraph, new Vector2(601,346), 100, "ani");
        xl = FXMgr.LoadSpineEffect("posui", xlGraph, new Vector2(606, 1120), 100, "ani");
        nv.skeleton.SetSkin("nv");
        xl.skeleton.SetSkin("xl");
        fxGraph = FXMgr.CreateEffectWithScale(fxCom, new Vector2(), "UI_game18_posuilizi", 162, -1);
        fxGraph.visible = false;
        t = ui.GetTransition("t0");
        attactAudio = Resources.Load(SoundConfig.GAME_AUDIO_EFFECT_URL + (int)SoundConfig.GameAudioId.MDL_Attact) as AudioClip;
        clickAudio = Resources.Load(SoundConfig.GAME_AUDIO_EFFECT_URL + (int)SoundConfig.GameAudioId.MDL_Click) as AudioClip;
        InitEvent();
        
    }
    public override void InitEvent()
    {
        base.InitEvent();
        //返回
        backBtn.onClick.Set(() =>
        {
            EventMgr.Ins.DispachEvent(EventConfig.STORY_BREACK_STORY);
        });
        //跳过
        skipBtn.onClick.Set(SkipGame);
        ui.onClick.Set(() =>
        {
            if (!isEnd)
            {
                GRoot.inst.PlayEffectSound(clickAudio);
                BallTween();
                AddDefendEnergy();
                FXMgr.CreateEffectWithScale(fxCom, new Vector3(250, 64), "UI_game18_tap", 160, 2);

            }

        });
        EventMgr.Ins.ReplaceEvent(EventConfig.STORY_GAME_QUIT, Destroy<GameClickDefendsView>);
        EventMgr.Ins.RemoveEvent(EventConfig.STORY_DELETE_GAME_INFO);
    }

    public override void InitData<D>(D data)
    {

        base.InitData(data);
        storyGameInfo = data as StoryGameInfo;
        isNewPlayerGame = GameData.isGuider;
        bgLoader.url = UrlUtil.GetGameBGUrl(38);
        startEnergy = 24;
        defendBar.value = startEnergy;
        exEnergy = 0;
        if (storyGameInfo.gameSaves.Count == 0)
        {
            isNewbie = true;
            newbieTimes = 0;
        }
        NewPlayerTipsShow();
        SetTalk("保持点击，帮我蓄力。", true, 1);

    }
    public override void InitData()
    {
        base.InitData();
        bgLoader.url = UrlUtil.GetGameBGUrl(38);
        isNewPlayerGame = true;
        defendBar.value = startEnergy;
        exEnergy = 0;
        isNewbie = true;
        SetTalk("保持点击，帮我蓄力。", true, 1);

        NewPlayerTipsShow();
       
    }
    void ReStart()
    {
        //defendImage.scale = Vector2.one;
        startEnergy = 24;
        defendBar.value = startEnergy;
        exEnergy = 0;
        isEnd = false;
        GameStart();
    }
 
    void GameStart()
    {
        isEnd = false;
        life++;
        StartCoroutine(ShotController());
        AttactGoWrapper();
        BallGoWrapper();
        nv.Initialize(true);
        xl.Initialize(true);
        nv.skeleton.SetSkin("nv");
        xl.skeleton.SetSkin("xl");
        nv.AnimationState.SetAnimation(0, "ani", false);
        xl.AnimationState.SetAnimation(0, "ani", false);
        
        
        nv.timeScale = 0;
        xl.timeScale = 0;

        if (attactGoWrapper2==null)
            AttactRefrashGoWrapper();
        fxGraph.visible = false;
    }

    IEnumerator ShotController()
    {
        int lifeIndex = life;
        SubDefendEnergy();
        if (!isEnd)
        {
            t.Play();
            //isEffect = true;
            DoShake();
            AttactRefrashGoWrapper();
        }
        
        GRoot.inst.PlayEffectSound(attactAudio);
        
        yield return new WaitForSeconds(cdTime);
        if (!isEnd&&life == lifeIndex)
        {
            StartCoroutine(ShotController());
        }
    }

    

    Renderer renderer;
    ParticleSystem particle;
    void GetAlphe()
    {
        if (ballGoWrapper != null)
        {
            float alpha = (float)(defendBar.value / 100)+0.7f;
            renderer.material.SetFloat("_MainIntentiy", alpha);
            Color color = particle.startColor;
            color.a = (float)(defendBar.value / 100)+31/255f;
            particle.startColor = color;
        }
    }

    void AddDefendEnergy()
    {

        if (defendBar.value < 100)
        {
            defendBar.value += addEnergy;
            GetAlphe();

        }
        else if (exEnergy < mixExEnergy)
        {
            exEnergy += addEnergy;
        }
        if (isNewbie&& newbieTimes>=5)
        {
            isNewbie = false;
            tapAnime.visible = false;
            //GameTool.SaveGameInfo(newbieKey, "1", storyGameInfo.gamePointConfig.id);
        }
        else
        {
            newbieTimes++;
        }
    }
    void SubDefendEnergy()
    {
        if (exEnergy > subEnergy)
        {
            exEnergy -= subEnergy;
        }
        else if (defendBar.value > 0)
        {
            defendBar.value -= subEnergy;
            GetAlphe();
            if (defendBar.value >= 85)
            {
                    SetTalk("坚持住！曼多拉要撑不住了！", true,5);
            }
            else if (defendBar.value >= 70)
            {
                SetTalk("别做无谓的抵抗了！", false,4);
            }
            else if (defendBar.value >= 55)
            {
                SetTalk("我相信你一定能够帮助我", true,3);
            }
            else if (defendBar.value >= 40)
            {
                SetTalk("姐姐，接受我的攻击吧！", false,2);
            }
        }
        else if (defendBar.value <= 0)
        {
            if (!isNewPlayerGame)
                LoseGame();
        }

    }
 
    private void Update()
    {
        if (!isEnd)
        {
             
            
            if (exEnergy > 0 && defendBar.value >= 100)
            {
                countDown += Time.deltaTime;

                if (countDown >= 3)
                    EndGame();
            }
            else
                countDown = 0;
            if (countDown > 0)
            {
                countDownText.text = (int)(4 - countDown) + "";
            }
            else
                countDownText.text = "";
        }
        else
            countDownText.text = "";
    }
    void EndGame()
    {
        TweenManager.KillTweens(ballGoWrapper, TweenPropType.Scale, false);

        SetTalk("不～不可能～难道这是人类的力量吗？", false,6);
        DoShake();

        AudioClip electric = Resources.Load(SoundConfig.GAME_AUDIO_EFFECT_URL + (int)SoundConfig.GameAudioId.SUI) as AudioClip;
        GRoot.inst.PlayEffectSound(electric);

        isEnd = true;
        if (attactGoWrapper !=null)
        {
            attactGoWrapper.visible = false; ;
        }
        fxGraph.displayObject.position = new Vector2(244, 0);
        nv.timeScale = 1;
        nv.AnimationState.SetAnimation(0, "ani", false);
        fxGraph.visible = true;
        StartCoroutine(WaitWinEffect());
        CloseAllGW();
    }

    IEnumerator WaitWinEffect()
    {
        yield return new WaitForSeconds(2.4f);
        GameTipsInfo info = new GameTipsInfo()
        {
            context = "成功抵御了曼多拉的攻击！!",
            callBack = OnComplete
        };
        UIMgr.Ins.showNextPopupView<GameSuccessView, GameTipsInfo>(info);
    }

    void LoseGame()
    {
        isEnd = true;
        xl.timeScale = 1;
        xl.AnimationState.SetAnimation(0, "ani", false);
        fxGraph.displayObject.position = new Vector2(244, 788);
        fxGraph.visible = true;
        StartCoroutine(WaitLossEffect());
        TweenManager.KillTweens(ballGoWrapper, TweenPropType.Scale, false);

        CloseAllGW();
    }

    IEnumerator WaitLossEffect()
    {
        yield return new WaitForSeconds(2.4f);
        GameTipsInfo info = new GameTipsInfo()
        {
            context = "你没有避开曼多拉的攻击!",
            callBack = ReStart
        };
        UIMgr.Ins.showNextPopupView<GameFailView, GameTipsInfo>(info);
    }

    void NewPlayerTipsShow()
    { 
        backBtn.visible = !isNewPlayerGame;
        skipBtn.visible = !isNewPlayerGame;
         
            GameStart(); 
        if (isNewbie)
        {
            tapAnime.visible = true;
        }
    }

    void CloseAllGW()
    {
        attactGoWrapper2.Dispose();
        attactGoWrapper2 = null;
        attactGoWrapper.Dispose();
        attactGoWrapper = null;
        ballGoWrapper.Dispose();
        ballGoWrapper = null;
    }

    GGraph attactGoWrapper;
    GGraph ballGoWrapper;
    GGraph attactGoWrapper2; 
    void AttactGoWrapper()
    {
        if (attactGoWrapper == null)
        {
            attactGoWrapper = FXMgr.CreateEffectWithScale(fxCom, new Vector3(250,38), "UI_game18_nvwangidle", 160, -1);
            
        }
        else
        {
            attactGoWrapper.visible = true ;
        }
    }
    void AttactRefrashGoWrapper()
    { 
        if (attactGoWrapper2 == null)
        {
            attactGoWrapper2 = FXMgr.CreateEffectWithScale(fxCom, new Vector3(241,36), "UI_game18_nvwangidle2", 160, -1);

        }
        else
        {
            attactGoWrapper2.Dispose();
            attactGoWrapper2 = null;
            if (!isEnd)
                AttactRefrashGoWrapper();
        }
    }
    void BallGoWrapper()
    {
        if (ballGoWrapper == null)
        {

            ballGoWrapper = FXMgr.CreateEffectWithScale(fxCom, new Vector3(605,1164), "UI_game18_qiu_new", 169, -1);

            
            ballGoWrapper.pivot = Vector2.one * 0.5f;
            ballGoWrapper.pivotAsAnchor = true;
            Debug.LogError(ballGoWrapper.displayObject.gameObject.name);
            renderer = ballGoWrapper.displayObject.gameObject.transform.Find("UI_game18_qiu_new(Clone)").GetComponent<Renderer>();
            particle = ballGoWrapper.displayObject.gameObject.transform.Find("UI_game18_qiu_new(Clone)/waihuan (1)").GetComponent<ParticleSystem>();

        }
        else
            attactGoWrapper.visible=true;
         GetAlphe();
    }

    void SetTalk(string content,bool isXinlin,int index)
    {
        if (talkList.Contains(index))
            return;
        talkList.Add(index);
        talk.visible = true;
        talkText.text = content;
        //talk.alpha = 0;
        //talk.TweenFade(1, 1f);
        transition.Play();

        talk.position = new Vector2(67,872);
        if(!isXinlin)
        talk.position = new Vector2(67,114);
    }

    void DoShake()
    {
        GTween.Shake(sceneCom.displayObject.gameObject.transform.localPosition, 3.5f, 0.5f).SetTarget(sceneCom.displayObject.gameObject).OnUpdate(
        (GTweener tweener) =>
        {
            sceneCom.displayObject.gameObject.transform.localPosition = new Vector3(tweener.value.x, tweener.value.y, sceneCom.displayObject.gameObject.transform.localPosition.z);
        });
    } 
    void BallTween()
    {

        TweenManager.KillTweens(ballGoWrapper, TweenPropType.Scale, false);
        ballGoWrapper.scale = Vector3.one * 162;
        ballGoWrapper.TweenScale(Vector3.one * 162 * 1.15f, 0.5f).OnComplete(() =>
        {
            ballGoWrapper.TweenScale(Vector3.one * 162, 1f);
        });
    }

}

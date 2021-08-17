using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

[ViewAttr("Game/UI/Y_Game_17", "Y_Game_17", "Game17")]
public class FindDoorSPGameView : BaseGameView
{
    public static FindDoorSPGameView ins;
    GLoader bgLoader;
    GComponent sceneCom;
    List<GComponent> suipianList;
    List<int> getList = new List<int>();
    List<GComponent> bottomComs = new List<GComponent>();
    AudioClip putDownAudio;
    GButton backBtn;
    GButton skipBtn;
    GButton tipsBtn; 
    /// <summary>
    /// 碎片数
    /// </summary>
    const int SP_COUNT = 6;

    /// <summary>
    /// 是否是新手引导状态
    /// </summary>
    bool isNewPlayer = false;


    public override void InitUI()
    {
        ins = this;
        base.InitUI();
        sceneCom = SearchChild("n4").asCom;

        bgLoader = SearchChild("n0").asLoader;
        controller = sceneCom.GetController("c1");
        backBtn = SearchChild("n1").asButton;
        skipBtn = SearchChild("n3").asButton;
        tipsBtn = SearchChild("n2").asButton;

        InitEvent();
    }

    void InitSuipian()
    {
        suipianList = new List<GComponent>();
        for (int i = 0; i < SP_COUNT; i++)
        {
            GComponent gCom = sceneCom.GetChild("n" + (5 + i)).asCom;
            int index = i;
            gCom.onClick.Set(() =>
            {
                OnClickCom(index);
            });

            suipianList.Add(gCom);

            GComponent bottomCom = sceneCom.GetChild("n" + (12 + i)).asCom;
            bottomComs.Add(bottomCom);

        }
    }
    public override void InitData<D>(D data)
    {
        base.InitData(data);
        isNewPlayer = GameData.isGuider;
        storyGameInfo = data as StoryGameInfo;
        bgLoader.url = UrlUtil.GetGameBGUrl(39);
        InitSuipian();


        //controller.selectedIndex = isNewPlayer ? 2 : 0;
        if (isNewPlayer)
            SetNewPlayerGame();
        GameStart();
    }

    public override void InitData()
    {
        base.InitData();
        bgLoader.url = UrlUtil.GetGameBGUrl(39);
        InitSuipian();
        isNewPlayer = true;
        //controller.selectedIndex = isNewPlayer ? 2 : 0;
        if (isNewPlayer)
            SetNewPlayerGame();
        GameStart();
    }

    /// <summary>
    /// 游戏开始前的引导逻辑可在此处做
    /// </summary>
    void GameStart()
    {

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
        //提示
        tipsBtn.onClick.Set(OnClickTips);
        ui.onClick.Set(() =>
        {
            if (controller.selectedIndex == 2)
                controller.selectedIndex = 0;
        });
        //sceneCom.GetChild("n42").onClick.Set(OnComplete);
        EventMgr.Ins.ReplaceEvent(EventConfig.STORY_GAME_QUIT, Destroy<FindDoorSPGameView>);
        EventMgr.Ins.ReplaceEvent(EventConfig.STORY_DELETE_GAME_INFO, DeleteGameInfo);
    }

    public void OnClickCom(int index)
    {

        if (!getList.Contains(index))
            getList.Add(index);

        GetEffect(index);

        if (putDownAudio == null)
            putDownAudio = Resources.Load(SoundConfig.GAME_AUDIO_EFFECT_URL + (int)SoundConfig.GameAudioId.GetItems) as AudioClip;
        GRoot.inst.PlayEffectSound(putDownAudio);


        if (getList.Count == SP_COUNT)
        {
            //FXMgr.CreateEffectWithScale(sceneCom.GetChild("n18").asCom, new Vector2(-40, -580), "G13_finish", 162, -1);
            StartCoroutine(GameOver());
        }
        //else if (getList.Count == 1 && isNewPlayer)
        //{
        //    //Debug.Log("dsgsafgsdfgsdfgsdfgs");
        //    guider.callback = ()=> { 
        //    tipsCom.visible = true;
        //    };
        //    guider.guiderInfo.contents = "游戏中还可以使用提示，来试试吧！";
        //    UIMgr.Ins.showNextPopupView<NewbieXinlingView, GuiderInfoLinked>(guider);
        //    AudioClip audioClip = Resources.Load<AudioClip>(UrlUtil.GetNewbieBgmUrl("102_2"));
        //    GRoot.inst.PlayEffectSound(audioClip);
        //    tipsBtn.touchable = true;
        //}
        //else if (getList.Count == 2 && isNewPlayer)
        //{
        //    tipsCom.visible = false;
        //    if (isNewPlayer && isFirstClick)
        //        NewPlayerTipsEffect();
        //}
    }
    IEnumerator GameOver()
    {
        yield return new WaitForSeconds(1.5f);
        GameTipsInfo info = new GameTipsInfo()
        {
            context = "你成功的找到了大门的碎片！",
            callBack = OnComplete
        };
        UIMgr.Ins.showNextPopupView<GameSuccessView, GameTipsInfo>(info);
    }

    /// <summary>
    /// 拾取动画
    /// </summary>
    void GetEffect(int index)
    {
        suipianList[index].touchable = false;
        suipianList[index].TweenMove(new Vector2(275, 812), 0.6f).OnComplete(() =>
        {
            suipianList[index].TweenFade(0, 1f);
            suipianList[index].TweenMove(bottomComs[index].position, 1f).OnComplete(() =>
            {
                suipianList[index].visible = false;
                bottomComs[index].GetController("c1").selectedIndex = 1;
                suipianList[index].touchable = true;
            });
        });
    }


    Extrand extrand;
    void OnClickTips()
    {
        //if (!isNewPlayer)
        //{
        //    if (extrand == null)
        //    {
        //        extrand = new Extrand();
        //        extrand.type = 1;
        //        extrand.key = "提示";
        //        List<GameNodeConsumeConfig> list = JsonConfig.GameNodeConsumeConfigs.FindAll(a => a.node_id == storyGameInfo.gamePointConfig.id && a.type == storyGameInfo.gamePointConfig.type);
        //        TinyItem item = ItemUtil.GetTinyItem(list[0].pay);
        //        extrand.item = item;
        //        extrand.msg = "获得提示";
        //        extrand.callBack = RequestGetTips;
        //    }
        //    UIMgr.Ins.showNextPopupView<PopupDialogView, Extrand>(extrand);
        //}
        //else
            GetTip();
    }

    void RequestGetTips()
    {
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("nodeId", storyGameInfo.gamePointConfig.id);
        wWWForm.AddField("type", storyGameInfo.gamePointConfig.type);
        wWWForm.AddField("index", 0);
        GameMonoBehaviour.Ins.RequestInfoPost<PlayerProperty>(NetHeaderConfig.STROY_STEP_CONSUME, wWWForm, GetTip);
    }

   public  void GetTip()
    {
        for (int i = 0; i < 6; i++)
        {
            if (!getList.Contains(i))
            {
                OnClickCom(i);
                break;
            }
        }

        if (isNewPlayer)
        {
            for (int i = 0; i < 6; i++)
            {
                if (!getList.Contains(i))
                {
                    suipianList[i].touchable = true;
                }
            }
        }
    }

    GuiderInfoLinked guider;
    /// <summary>
    /// 新手引导逻辑
    /// </summary>
    void SetNewPlayerGame()
    {
        backBtn.visible = false;
        skipBtn.visible = false;

        GameData.guiderCurrent = GuiderInfoLinked.GetCurrGuiderInfo(1, 9);
        UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);

        isFirstClick = true;
    }
    bool isFirstClick;
    void NewPlayerTipsEffect()
    {
        isFirstClick = false;
        for (int i = 1; i < SP_COUNT; i++)
        {
            suipianList[i].touchable = true;
        }
    }
     
}

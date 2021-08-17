using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using Spine.Unity;
using UnityEngine;

[ViewAttr("Game/UI/Y_Game20", "Y_Game20", "Game20")]
public class GameAttackMDLView : BaseGameView
{

    GButton backBtn;
    GButton skipBtn;
    GButton tipsBtn;
    GLoader bgLoader;
    GComponent sceneCom;
    List<GComponent> partComs = new List<GComponent>();
    bool gameOver;


    bool isNewPlayerGame = false;
    public override void InitUI()
    {
        base.InitUI();
        sceneCom = SearchChild("n3").asCom;

        bgLoader = SearchChild("n4").asLoader;
        controller = sceneCom.GetController("c1");
        backBtn = SearchChild("n0").asButton;
        skipBtn = SearchChild("n2").asButton;
        tipsBtn = SearchChild("n1").asButton;
        InitPart();


        InitEvent();

    }
    public override void InitData()
    {
        base.InitData();
        bgLoader.url = UrlUtil.GetGameBGUrl(40);
        isNewPlayerGame = false;
        controller.selectedIndex = isNewPlayerGame ? 1 : 0;
        isFirstClick = false;
        PartControll();
        NewPlayerTipsShow();
    }
    public override void InitData<D>(D data)
    {
        base.InitData(data);
        storyGameInfo = data as StoryGameInfo;
        isNewPlayerGame = GameData.isGuider;
        bgLoader.url = UrlUtil.GetGameBGUrl(40);
        controller.selectedIndex = isNewPlayerGame ? 1 : 0;
        isFirstClick = true;
        PartControll();
        NewPlayerTipsShow();

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
        //tipsBtn.onClick.Set(GetTip);
        ui.onClick.Set(() =>
        {
            if (controller.selectedIndex == 2)
                OnComplete();
        });

        EventMgr.Ins.ReplaceEvent(EventConfig.STORY_GAME_QUIT, Destroy<GameAttackMDLView>);
        EventMgr.Ins.ReplaceEvent(EventConfig.STORY_DELETE_GAME_INFO, DeleteGameInfo);
    }
    void InitPart()
    {
        for (int i = 0; i < 9; i++)
        {
            int index = i;
            GComponent com = sceneCom.GetChild("n" + i).asCom;
            com.onClick.Set(() =>
            {
                RotateCom(index);
            });
            partComs.Add(com);
        }
    }

    AudioClip clip;
    void RotateCom(int index)
    {
         clip = Resources.Load(SoundConfig.GAME_AUDIO_EFFECT_URL + (int)SoundConfig.GameAudioId.SPIN) as AudioClip;
        GRoot.inst.PlayEffectSound(clip);
        GComponent com = partComs[index];
        com.touchable = false;
        com.TweenRotate(com.rotation + 90, 0.2f).OnComplete(() =>
        {
            if (!gameOver)
                com.touchable = true;
            CheckIsAllRight();
        });
        if (isNewPlayerGame&&isFirstClick)
            NewGamePart();
    }

    void CheckIsAllRight()
    {
        bool isRight = true;
        for (int i = 0; i < partComs.Count; i++)
        {
            if ((partComs[i].rotation - 180)%360 != 0)
            {
                isRight = false;
                break;
            }
        }
        if (isRight)
        {
            for (int i = 0; i < partComs.Count; i++)
            {
                partComs[i].touchable = false;
            }
            gameOver = true;
            Debug.Log("GameOver");
            StartCoroutine(GoToComplet());

        }
    }
    IEnumerator GoToComplet()
    {
        AudioClip attactAudio = Resources.Load(SoundConfig.GAME_AUDIO_EFFECT_URL + (int)SoundConfig.GameAudioId.MDL_Fix) as AudioClip;
        GRoot.inst.PlayEffectSound(attactAudio);
        CompleteGoWrapper();
        yield return new WaitForSeconds(1.37f);
        CompleteGoWrapper2();
        ComplateSpine();
        controller.selectedIndex = 2;
       
    }
    List<int> newGameIndexs = new List<int>() { 0, 3, 5, 1 };
    void PartControll()
    {
        if (isNewPlayerGame)
        {
            for (int i = 0; i < partComs.Count; i++)
            {
                partComs[i].touchable = false;
                if (newGameIndexs.Contains(i))
                {
                    partComs[i].rotation = 90; 
                }
            }
            TeachNewGame();

        }
        else
        {
            for (int i = 0; i < partComs.Count; i++)
            {
                int times = Random.Range(3, 8);

                partComs[i].rotation = times * 90; 
            }
        }

    }
    bool isFirstClick = true;
    void TeachNewGame()
    {
        GComponent com = partComs[newGameIndexs[0]];
        com.touchable = true;
        com.alpha = 0.5f;
        
    }

    void NewGamePart()
    {
        isFirstClick = false;
        controller.selectedIndex = 0;
        for (int i = 0; i < partComs.Count; i++)
        {
            if (newGameIndexs.Contains(i))
            {
                partComs[i].touchable = true;
                partComs[i].alpha = 1;
            }
        }
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
        //    List<GameNodeConsumeConfig> list = JsonConfig.GameNodeConsumeConfigs.FindAll(a => a.node_id == storyGameInfo.gamePointConfig.id && a.type == storyGameInfo.gamePointConfig.type);
        //    TinyItem item = ItemUtil.GetTinyItem(list[0].pay);
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
        for (int i = 0; i < partComs.Count; i++)
        {
            int times =4- (int)(partComs[i].rotation-180)/90;
            GComponent com = partComs[i];
            com.TweenRotate(com.rotation + 360 + 90 * times, 0.8f + 0.2f * times); 

        }
        StartCoroutine(TipsGameOver());
    }
    IEnumerator TipsGameOver()
    {
        for (int i = 0; i < partComs.Count; i++)
        {
            partComs[i].touchable = false;
        }
        yield return new WaitForSeconds(2);
        gameOver = true;
        Debug.Log("GameOver");
        //controller.selectedIndex = 2;
         
        StartCoroutine(GoToComplet());
    }

    void NewPlayerTipsShow()
    {
        skipBtn.visible = !isNewPlayerGame; 
        backBtn.visible = !isNewPlayerGame;
        
    }


    GoWrapper completeGoWrapper;
    GoWrapper completeGoWrapper2;

    void CompleteGoWrapper()
    {
        if (completeGoWrapper == null)
        {
            Object prefab = Resources.Load("Game/GFX/Prefabs/Game20_ding");
            GGraph effect = sceneCom.GetChild("n17").asGraph;
            if (prefab != null)
            {
                GameObject go = (GameObject)Object.Instantiate(prefab);
                //go.transform.localScale = new Vector3(160, 160, 160);
                completeGoWrapper = new GoWrapper(go);
                effect.SetNativeObject(completeGoWrapper);
            }
        }
        else
        {
            completeGoWrapper.gameObject.SetActive(true);
        }
    }
    void CompleteGoWrapper2()
    {
        if (completeGoWrapper2 == null)
        {
            Object prefab = Resources.Load("Game/GFX/Prefabs/Game20_di");
            GGraph effect = sceneCom.GetChild("n18").asGraph;
            if (prefab != null)
            {
                GameObject go = (GameObject)Object.Instantiate(prefab);
                //go.transform.localScale = new Vector3(160, 160, 160);
                completeGoWrapper2 = new GoWrapper(go);
                effect.SetNativeObject(completeGoWrapper2);
            }
        }
        else
        {
            completeGoWrapper2.gameObject.SetActive(true);
        }
    }
    void ComplateSpine()
    {
        GGraph gGraph = sceneCom.GetChild("n19").asGraph;
        SkeletonAnimation skeletonAnimation = FXMgr.LoadSpineEffect("fazhen", gGraph, new Vector2(371, 757),100);
        skeletonAnimation.timeScale = 1;
    }
}

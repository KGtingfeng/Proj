using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using Spine;
using Spine.Unity;
using UnityEngine;

[ViewAttr("Game/UI/Y_Game21", "Y_Game21", "Game21")]
public class GamePuzzleDoorView : BaseGameView
{
    public static GamePuzzleDoorView ins;
    GButton backBtn;
    GButton skipBtn;
    GButton tipsBtn;
    GLoader bgLoader;
    GComponent sceneCom;
    List<GComponent> suipianList = new List<GComponent>();
    List<GComponent> bottomList = new List<GComponent>();
    Dictionary<int, Vector2> defauldPos = new Dictionary<int, Vector2>();
    Dictionary<int, Vector2> defauldScale = new Dictionary<int, Vector2>();
    List<Vector2> targetPos = new List<Vector2>() {
        new Vector2(242,556),
        new Vector2(305,720),
        new Vector2(505,592),
        new Vector2(469,925),
        new Vector2(229,1023),
        new Vector2(516,1027),
    };
    List<int> putDownList = new List<int>();
    AudioClip putDownAudio;
    GComponent newTipsCom;
    bool isNewPlayerGame = false;

    public override void InitUI()
    {
        ins = this;
        base.InitUI();
        sceneCom = SearchChild("n3").asCom;

        bgLoader = SearchChild("n4").asLoader;
        controller = sceneCom.GetController("c1");
        backBtn = SearchChild("n0").asButton;
        skipBtn = SearchChild("n2").asButton;
        tipsBtn = SearchChild("n1").asButton;
        newTipsCom = sceneCom.GetChild("n48").asCom;
        newTipsCom.visible = false;
        InitEvent();

    }

    public override void InitData()
    {
        base.InitData();
        bgLoader.url = UrlUtil.GetGameBGUrl(39);
        isNewPlayerGame = true;
        InitSP();
        if (isNewPlayerGame)
            ShowNewbie();
        NewPlayerTipsShow();


    }
    public override void InitData<D>(D data)
    {
        base.InitData(data);
        storyGameInfo = data as StoryGameInfo;
        isNewPlayerGame = GameData.isGuider;
        bgLoader.url = UrlUtil.GetGameBGUrl(39);
        InitSP();
        if (isNewPlayerGame)
            ShowNewbie();
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
        tipsBtn.onClick.Set(()=> {
            if (isNewPlayerGame)
            {
                ShowTips();

            }
            else
                OnClickTips();
            tipsBtn.touchable = false;
        });
        //tipsBtn.onClick.Set(ShowTips);
        
        backBtn.onDrop.Set(BGOnDrop);
        skipBtn.onDrop.Set(BGOnDrop);
        tipsBtn.onDrop.Set(BGOnDrop);
        sceneCom.onDrop.Set(BGOnDrop);


        EventMgr.Ins.ReplaceEvent(EventConfig.STORY_GAME_QUIT, Destroy<GamePuzzleDoorView>);
        EventMgr.Ins.ReplaceEvent(EventConfig.STORY_DELETE_GAME_INFO, DeleteGameInfo);
    }

    void InitSP()
    {
        for (int i = 0; i < 6; i++)
        {
            GComponent gCom = sceneCom.GetChild("n" + (53 + i)).asCom;
            int index = i;
            gCom.onDrop.Set((EventContext context) => { ShipOnDrop(index, context); });
            suipianList.Add(gCom);

            GComponent gComponent = sceneCom.GetChild("n" + (14 - i)).asCom;
            gComponent.draggable = true;
            if (isNewPlayerGame&&index!=0)
                gComponent.touchable = false;
            bottomList.Add(gComponent);
            defauldPos.Add(i, gComponent.position);
            defauldScale.Add(i, gComponent.scale);
            gComponent.onDragStart.Set(() => { OnDragStart(index); });
            gComponent.onDragEnd.Set(() => { OnDragEnd(index); });
        }
    }

    void OnDragStart(int index)
    {
        bottomList[index].TweenScale(Vector2.one, 0.2f);
        bottomList[index].touchable = false;
        bottomList[index].sortingOrder = 2;
        Vector2 pos = GameTool.MousePosToUI(Input.mousePosition, sceneCom);
        bottomList[index].position = pos;
    }

    void OnDragEnd(int index)
    {
        GObject obj = GRoot.inst.touchTarget;
        bottomList[index].touchable = true;

        while (obj != null)
        {
            if (obj.hasEventListeners("onDrop"))
            {
                obj.RequestFocus();
                obj.DispatchEvent("onDrop", index);
                return;
            }
            obj = obj.parent;
        }

    }
    void BGOnDrop(EventContext context)
    {
        int dragIndex = int.Parse(context.data.ToString());
        DropOnError(dragIndex);
    }

    void ShipOnDrop(int index, EventContext context)
    {
        Debug.Log(index);
        int dragIndex = int.Parse(context.data.ToString());
        if (index == dragIndex)
            DropOnTrue(dragIndex);
        else
            DropOnError(dragIndex);

    }
    void DropOnTrue(int index)
    {
        if (index == 0)
        {
            for (int i = 1; i < bottomList.Count; i++)
            {
                bottomList[i].touchable = true;
            }
            tipsBtn.touchable = true;
        }
        bottomList[index].TweenMove(targetPos[index], 0.2f);
        bottomList[index].touchable = false;
        putDownList.Add(index);
        if (putDownAudio == null)
            putDownAudio = Resources.Load(SoundConfig.GAME_AUDIO_EFFECT_URL + (int)SoundConfig.GameAudioId.PutDown) as AudioClip;
        GRoot.inst.PlayEffectSound(putDownAudio);
        newTipsCom.visible = false;

        if (putDownList.Count == 6)
        {
            ComplateGoWrapper();
            StartCoroutine(ComplateSpine());
        }
    }

    void DropOnError(int index)
    {
        bottomList[index].touchable = false;
        bottomList[index].sortingOrder = 0;
        //Vector2 pos = GameTool.MousePosToUI(Input.mousePosition, bottomSubCom);
        //pos.y = -pos.y;
        //bottomList[index].position = pos;
        bottomList[index].TweenMove(defauldPos[index], 1f).OnComplete(() =>
        {
            bottomList[index].touchable = true;
        });
        bottomList[index].TweenScale(defauldScale[index], 1f);
    }



    Extrand extrand;
    void OnClickTips()
    {
        ShowTips();
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
        GameMonoBehaviour.Ins.RequestInfoPost<PlayerProperty>(NetHeaderConfig.STROY_STEP_CONSUME, wWWForm, ShowTips);
    }





    void ShowTips()
    {
        int index = 0;
        for (int i = 0; i < 6; i++)
        {
            if (!putDownList.Contains(i))
            {
                index = i;
            }
        }
        bottomList[index].TweenScale(Vector2.one, 1f);
        bottomList[index].TweenMove(targetPos[index], 1f).OnComplete(() =>
        {
            DropOnTrue(index);
            tipsBtn.touchable = true;
        });

    }
   public void ShowNewTipsCom()
    {
        newTipsCom.visible = isNewPlayerGame;
    }

    private void ShowNewbie()
    {
        GameData.guiderCurrent = GuiderInfoLinked.GetCurrGuiderInfo(1, 11);
        UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);

    }
    void NewPlayerTipsShow()
    {
        newTipsCom.visible = false;
        backBtn.visible = !isNewPlayerGame;
        skipBtn.visible = !isNewPlayerGame;
        tipsBtn.touchable = !isNewPlayerGame;
    }


    GoWrapper complateGoWrapper;
    void ComplateGoWrapper()
    {
        if (complateGoWrapper == null)
        {
            Object prefab = Resources.Load("Game/GFX/Prefabs/Game21_wancheng");
            GGraph effect = SearchChild("n5").asGraph;

            if (prefab != null)
            {
                GameObject go = (GameObject)Object.Instantiate(prefab);
                go.gameObject.transform.localScale = new Vector3(1, 1, 1);
                complateGoWrapper = new GoWrapper(go);
                effect.SetNativeObject(complateGoWrapper);

            }
        }
        else
            complateGoWrapper.gameObject.SetActive(true);
    }
    IEnumerator ComplateSpine()
    {
        AudioClip clip = Resources.Load(SoundConfig.GAME_AUDIO_EFFECT_URL + (int)SoundConfig.GameAudioId.SUCCESS) as AudioClip;
        GRoot.inst.PlayEffectSound(clip);

        yield return new WaitForSeconds(1f);
        controller.selectedIndex = 1;
        GGraph gGraph = sceneCom.GetChild("n59").asGraph;
        SkeletonAnimation skeletonAnimation = FXMgr.LoadSpineEffect("jingzi", gGraph, new Vector2(930, 550), 140, "cameout");
        TrackEntry track = skeletonAnimation.AnimationState.SetAnimation(0, "cameout", false);
        skeletonAnimation.timeScale = 1;
        yield return new WaitForSeconds(3f);
        TrackEntry track2 = skeletonAnimation.AnimationState.SetAnimation(0, "cameout", true);
        skeletonAnimation.timeScale = 1;
        track2.AnimationStart = 1f;
        track2.AnimationEnd = 3f;

        ui.onClick.Set(() =>
        {
            if (controller.selectedIndex == 1)
                OnComplete();
        });
    }
    void LoopSpine()
    {

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

/// <summary>
/// 罗马废墟放置宝石
/// </summary>
[ViewAttr("Game/UI/Y_Game9", "Y_Game9", "Game")]
public class GameGemFillingView : BaseGameView
{
    private GComponent sceneCom;
    private GLoader bg;
    private GObject fragment;

    GComponent tap;
    GComponent tap1;

    readonly List<Vector3> orderPos = new List<Vector3>()
    {
        new Vector3(334,538,0),
        new Vector3(411,560,0),
        new Vector3(469,617,0),
        new Vector3(492,697,0),
        new Vector3(469,772,0),
        new Vector3(411,830,0),

        new Vector3(334,852,0),
        new Vector3(255,830,0),
        new Vector3(198,772,0),
        new Vector3(170,697,0),
        new Vector3(198,617,0),
        new Vector3(255,560,0),
    };

    /// <summary>
    /// 在底部位置
    /// </summary>
    readonly List<Vector2> defauldPos = new List<Vector2>()
    {
        new Vector2(138,1275),
        new Vector2(305,1275),
        new Vector2(472,1275),
        new Vector2(639,1275),
        new Vector2(54,1342),
        new Vector2(220,1342),
        new Vector2(387,1342),
        new Vector2(554,1342),
        new Vector2(138,1409),
        new Vector2(305,1409),
        new Vector2(472,1409),
        new Vector2(639,1409),
    };

    readonly int[] truePos = { 10, 6, 0, 8, 11, 9, 5, 3, 7, 1, 2, 4 };
    List<int> putDownGemList = new List<int>();
    List<int> putDownList = new List<int>();
    List<GComponent> bottomList = new List<GComponent>();
    List<GComponent> holeList = new List<GComponent>();
    List<GGraph> holeLoaderList = new List<GGraph>();
    AudioClip putDownAudio;
    AudioClip OpenGear;
    AudioClip error;

    bool isNewbie;
    string newbieKey = "newbie";

    public override void InitUI()
    {
        base.InitUI();
        sceneCom = SearchChild("n6").asCom;
        controller = sceneCom.GetController("c1");
        bg = sceneCom.GetChild("n0").asCom.GetChild("n1").asLoader;
        fragment = sceneCom.GetChild("n0").asCom.GetChild("n5");
        tap = sceneCom.GetChild("n40").asCom;
        tap1 = sceneCom.GetChild("n41").asCom;
        tap.visible = false;
        tap1.visible = false;
        for (int i = 0; i < 12; i++)
        {
            int index = i;
            GComponent gCom = sceneCom.GetChild("n" + (6 + i)).asCom;
            //gCom.draggable = true;
            gCom.onDragStart.Set(() => { OnDragStart(index); });
            gCom.onDragEnd.Set(() => { OnDragEnd(index); });
            gCom.onDrop.Set(BGOnDrop);
            gCom.onClick.Set(()=> {
                ChangeGemBrightness(lastBottomId, 0);
                ChangeGemBrightness(index, 0.2f);
                ShowHoleLight();
            });
            bottomList.Add(gCom);
            //defauldPos.Add(gCom.position);
        }
        for (int i = 0; i < 12; i++)
        {
            int index = i;
            GComponent gCom = sceneCom.GetChild("n" + (24 + i)).asCom;
            gCom.onClick.Set(()=> {
                ChangeGemBrightness(lastBottomId, 0);
                OnClick(index);
                HideHoleLight();
                lastBottomId = -1;
            });
            gCom.onDrop.Set((EventContext context) =>
            {
                OnDrop(index, context);
            });
            holeList.Add(gCom);
        }
        if (holeLoaderList.Count ==0)
        {
            for (int i = 0; i < holeList.Count; i++)
            {
                GGraph gGraph = FXMgr.CreateEffectWithScale(holeList[i], new Vector2(-344, -859), "G9_tips", 162, -1, -1);
                gGraph.displayObject.gameObject.SetActive(false);
                holeLoaderList.Add(gGraph);
            }
        }
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
        SearchChild("n5").onClick.Set(SkipGame);
        //tips
        SearchChild("n4").onClick.Set(OnClickTips);

        sceneCom.GetChild("n23").onClick.Set(OnClickStart);

        sceneCom.GetChild("n37").asCom.onDrop.Set(BGOnDrop);

        EventMgr.Ins.ReplaceEvent(EventConfig.STORY_GAME_QUIT, Destroy<GameGemFillingView>);
        EventMgr.Ins.RemoveEvent(EventConfig.STORY_DELETE_GAME_INFO);

    }

    public override void InitData<D>(D data)
    {
        base.InitData(data);
        storyGameInfo = data as StoryGameInfo;
        bg.url = UrlUtil.GetGameBGUrl(25);
        if (storyGameInfo.gameSaves.Count > 0)
        {
            isNewbie = true;
        }
    }

    public override void InitData()
    {
        base.InitData();
        bg.url = UrlUtil.GetGameBGUrl(25);
        //sceneCom.GetChild("n0").asCom.GetChild("n3").asCom.GetController("c1").selectedIndex = 1;
        //controller.selectedIndex = 2;
        //sceneCom.GetChild("n23").onClick.Set(() => { StartCoroutine(OnClickComplete()); });
        //FXMgr.CreateEffectWithGGraph(sceneCom.GetChild("n38").asGraph, new Vector2(386, 719), "G9_finish");
        isNewbie = true;
    }

    void OnDragStart(int index)
    {
        bottomList[index].touchable = false;

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

    void OnDrop(int index, EventContext context)
    {
        tap.visible = false;
        tap1.visible = false;
        int dragIndex = int.Parse(context.data.ToString());
        Debug.Log(index+"  "+dragIndex);
        bottomList[dragIndex].TweenMove(holeList[index].position, 1f).OnComplete(() =>
        {
            bottomList[dragIndex].touchable = false;
        });
        putDownList.Add(index);
        putDownGemList.Add(dragIndex);
        if (putDownAudio == null)
            putDownAudio = Resources.Load(SoundConfig.GAME_AUDIO_EFFECT_URL + (int)SoundConfig.GameAudioId.PutDown) as AudioClip;
        GRoot.inst.PlayEffectSound(putDownAudio);
        if (putDownList.Count == 12)
        {
            CheckOrder();
        }
    }
    void OnClick(int index)
    {
        if (lastBottomId!=-1)
        {
            tap.visible = false;
            tap1.visible = false;
            bottomList[lastBottomId].TweenMove(holeList[index].position, 1f);
            bottomList[lastBottomId].touchable = false;
            putDownList.Add(index);
            putDownGemList.Add(lastBottomId);
            if (putDownAudio == null)
                putDownAudio = Resources.Load(SoundConfig.GAME_AUDIO_EFFECT_URL + (int)SoundConfig.GameAudioId.PutDown) as AudioClip;
            GRoot.inst.PlayEffectSound(putDownAudio);
            if (putDownList.Count == 12)
            {
                CheckOrder();
            }
        }
    }
    void BGOnDrop(EventContext context)
    {
        int dragIndex = int.Parse(context.data.ToString());
        DropOnError(dragIndex);
    }


    void DropOnError(int index)
    {
        bottomList[index].TweenMove(defauldPos[index], 1f).OnComplete(() =>
        {
            bottomList[index].touchable = true;
        });
    }

    /// <summary>
    /// 检查顺序
    /// </summary>
    void CheckOrder()
    {
        for (int i = 0; i < putDownList.Count; i++)
        {
            if (putDownList[i] != truePos[i])
            {
                OrderError();
                return;
            }
        }

        for (int i = 0; i < bottomList.Count; i++)
        {
            bottomList[i].visible = false;
        }
        SearchChild("n4").onClick.Clear();
        sceneCom.GetChild("n0").asCom.GetChild("n3").asCom.GetController("c1").selectedIndex = 1;
        controller.selectedIndex = 2;
        sceneCom.GetChild("n23").onClick.Set(() => { StartCoroutine(OnClickComplete()); });
        FXMgr.CreateEffectWithGGraph(sceneCom.GetChild("n38").asGraph, new Vector2(386, 719), "G9_finish");
    }

    void OrderError()
    {
        if (error == null)
            error = Resources.Load(SoundConfig.GAME_AUDIO_EFFECT_URL + (int)SoundConfig.GameAudioId.Error) as AudioClip;
        GRoot.inst.PlayEffectSound(error);
        UIMgr.Ins.showErrorMsgWindow(MsgException.GAME_PUZZLE_ERROR);
        putDownList.Clear();
        putDownGemList.Clear();
        for (int i = 0; i < bottomList.Count; i++)
        {
            DropOnError(i);
        }
    }

    private void OnClickStart()
    {
        controller.selectedIndex = 1;
        if (isNewbie)
        {
            isNewbie = false;
            tap.visible = true;
            tap1.visible = true;
            GameTool.SaveGameInfo(newbieKey, "1", storyGameInfo.gamePointConfig.id);
        }
    }

    IEnumerator OnClickComplete()
    {
        sceneCom.GetChild("n23").onClick.Clear();
        if (OpenGear == null)
            OpenGear = Resources.Load(SoundConfig.GAME_AUDIO_EFFECT_URL + (int)SoundConfig.GameAudioId.OpenGear) as AudioClip;
        GRoot.inst.PlayEffectSound(OpenGear);
        controller.selectedIndex = 3;
        Transition transition = sceneCom.GetChild("n0").asCom.GetTransition("t0");
        transition.Play();

        yield return new WaitForSeconds(2.4f);
        FXMgr.CreateEffectWithGGraph(sceneCom.GetChild("n39").asGraph, new Vector2(383, 858), "G9_inside");
        sceneCom.GetChild("n23").onClick.Set(FragmentEffect);

    }

    void FragmentEffect()
    {
        sceneCom.GetChild("n23").onClick.Clear();
        fragment.sortingOrder = 1;
        fragment.TweenScale(new Vector2(0.4f, 0.4f), 2f).OnComplete(() =>
        {

            fragment.TweenMoveY(2000, 3f);
        });
        sceneCom.GetChild("n23").onClick.Set(OnComplete);

    }
    int lastBottomId = -1;
    //选中效果；改变宝石图片的亮度与对比度
    void ChangeGemBrightness(int id, float bright)
    {
        if (id != -1)
        {
            GObject obj = bottomList[id].asCom.GetChild("n6");
            if (obj.filter is ColorFilter filter)
            {
                filter.Reset();
                filter.AdjustBrightness(bright);
                filter.AdjustContrast(bright);
               
            }
            lastBottomId = id;
        }
        

    }

    void ShowHoleLight()
    {
        for (int i = 0; i < holeList.Count; i++)
        {
            if (!putDownList.Contains(i))
            {
                holeLoaderList[i].displayObject.gameObject.SetActive(true);

            }
        }
    }
    void HideHoleLight()
    {
        for (int i = 0; i < holeList.Count; i++)
        {
            holeLoaderList[i].displayObject.gameObject.SetActive(false);
        }
    }
    bool isFirst = true;
    private void OnClickTips()
    {
        if (isFirst)
        {
            GameTipsInfo gameTipsInfo;
            gameTipsInfo = new GameTipsInfo();
            gameTipsInfo.isShowBtn = false;
            gameTipsInfo.context = "这好像是罗马字母的倒影！";
            UIMgr.Ins.showNextPopupView<StoryTipsView, GameTipsInfo>(gameTipsInfo);

            isFirst = false;
        }
        else
        {
            if (putDownList.Count < 12)
            {
                GetTip();
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

        }

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
        for (int i = 0; i < putDownList.Count; i++)
        {
            if (putDownList[i] != truePos[i])
            {
                for (int j = putDownList.Count - 1; j >= i; j--)
                {
                    DropOnError(putDownGemList[j]);
                    putDownGemList.RemoveAt(j);
                    putDownList.RemoveAt(j);
                }
            }
        }
        for (int j = 0; j < 12; j++)
        {
            if (!putDownGemList.Contains(j))
            {
                OnDropTrue(truePos[putDownList.Count], j);
                break;
            }
        }
        //FXMgr.CreateEffectWithScale(holeList[truePos[putDownList.Count]], new Vector2(-344, -859), "G9_tips");
    }

    void OnDropTrue(int index,int dragIndex)
    {
        tap.visible = false;
        tap1.visible = false;
        Debug.Log(index + "  " + dragIndex);
        bottomList[dragIndex].TweenMove(holeList[index].position, 1f).OnComplete(() =>
        {
            bottomList[dragIndex].touchable = false;
        });
        putDownList.Add(index);
        putDownGemList.Add(dragIndex);
        if (putDownAudio == null)
            putDownAudio = Resources.Load(SoundConfig.GAME_AUDIO_EFFECT_URL + (int)SoundConfig.GameAudioId.PutDown) as AudioClip;
        GRoot.inst.PlayEffectSound(putDownAudio);
        if (putDownList.Count == 12)
        {
            CheckOrder();
        }
    }
}

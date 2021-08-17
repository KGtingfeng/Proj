using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using Spine.Unity;
using Spine;

public class GamePuzzleFourClockMoudle : BaseMoudle
{
    /// <summary>
    /// 碎片正确位置
    /// </summary>
    readonly List<Vector3> orderPos = new List<Vector3>()
    {
        new Vector3(377,738,0),
        new Vector3(378,686,0),
        new Vector3(383,725,0),
        new Vector3(376,745,0),
        new Vector3(290,402,0),
        new Vector3(464,407,0),
    };
    /// <summary>
    /// 碎片在底部位置
    /// </summary>
    readonly List<Vector2> defauldPos = new List<Vector2>()
    {
        new Vector2(98,1419),
        new Vector2(182,1419),
        new Vector2(276,1420),
        new Vector2(401,1419),
        new Vector2(533,1418),
        new Vector2(644,1419),
    };
    /// <summary>
    /// 碎片在底部缩放
    /// </summary>
    readonly List<Vector2> defauldScale = new List<Vector2>()
    {
        new Vector2(1.153f,1.153f),
        new Vector2(0.507f,0.507f),
        new Vector2(0.516f,0.516f),
        new Vector2(0.264f,0.264f),
        new Vector2(0.854f,0.77f),
        new Vector2(0.8f,0.8f),
    };

    List<GComponent> bottomList = new List<GComponent>();
    GComponent dropGcom;
    List<int> putDownList = new List<int>();

    AudioClip putDownAudio;
    StoryGameInfo storyGameInfo;
    BaseGameView baseGameView;

    GGraph spineGgraph;
    GGraph bgGgraph;
    public override void InitMoudle(GComponent gComponent, int controllerIndex)
    {
        base.InitMoudle(gComponent, controllerIndex);
        InitUI();
        InitEvent();
    }

    public override void InitUI()
    {
        base.InitUI();
        GetList(ui, 25, bottomList);
        dropGcom = SearchChild("n34").asCom;
        spineGgraph = SearchChild("n35").asGraph;
        bgGgraph = SearchChild("n36").asGraph;
        baseGameView = baseView as BaseGameView;
        storyGameInfo = baseGameView.storyGameInfo;
    }

    public override void InitEvent()
    {
        base.InitEvent();
        baseView.ui.GetChild("n2").onClick.Set(OnClickTips);
        SearchChild("n12").asCom.onDrop.Set(BGOnDrop);
        dropGcom.onDrop.Set(OnDrop);
        baseView.ui.GetChild("n1").asCom.onDrop.Set(BGOnDrop);
        baseView.ui.GetChild("n2").asCom.onDrop.Set(BGOnDrop);
        baseView.ui.GetChild("n3").asCom.onDrop.Set(BGOnDrop);
    }

    void GetList(GComponent gCom, int indexr, List<GComponent> gList)
    {
        for (int i = 0; i < 6; i++)
        {
            int index = i;
            GComponent com = gCom.GetChild("n" + (i + indexr)).asCom;

            com.touchable = true;
            com.draggable = true;
            com.onDragStart.Set(() =>
            {
                OnDragStart(index);
            });
            com.onDragEnd.Set(() =>
            {
                OnDragEnd(index);
            });
            com.onDrop.Set(BGOnDrop);
            com.GetChild("n26").visible = false;
            gList.Add(com);
        }
    }


    void OnDragStart(int index)
    {
        bottomList[index].TweenScale(Vector2.one, 0.5f);
        bottomList[index].TweenRotate(0, 0.5f);
        bottomList[index].touchable = false;
        bottomList[index].sortingOrder = 2;
        Vector2 pos = GameTool.MousePosToUI(Input.mousePosition, ui);
        bottomList[index].TweenMove(pos, 0.1f);
    }

    void OnDragEnd(int index)
    {
        GObject obj = GRoot.inst.touchTarget;

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

    void OnDrop(EventContext context)
    {
        int dragIndex = int.Parse(context.data.ToString());
        DropOnTrue(dragIndex);
    }

    void BGOnDrop(EventContext context)
    {
        int dragIndex = int.Parse(context.data.ToString());
        DropOnError(dragIndex);
    }

    void DropOnTrue(int index)
    {
        bottomList[index].sortingOrder = putDownList.Count;
        bottomList[index].TweenMove(orderPos[index], 1f).OnComplete(() =>
        {
            bottomList[index].touchable = false;
        });
        putDownList.Add(index);
        if (putDownAudio == null)
            putDownAudio = Resources.Load(SoundConfig.GAME_AUDIO_EFFECT_URL + (int)SoundConfig.GameAudioId.PutDown) as AudioClip;
        GRoot.inst.PlayEffectSound(putDownAudio);
        if (putDownList.Count == 6)
        {
            CheckOrder();
        }
    }

    void DropOnError(int index)
    {
        bottomList[index].TweenMove(defauldPos[index], 1f).OnComplete(() =>
        {
            bottomList[index].touchable = true;
        });
        bottomList[index].TweenScale(defauldScale[index], 1f);
        bottomList[index].sortingOrder = 0;
    }

    /// <summary>
    /// 检查顺序
    /// </summary>
    void CheckOrder()
    {
        for (int i = 0; i < 3; i++)
        {
            if (putDownList[i] != 2 - i)
            {
                OrderError();
                return;
            }
        }
        GameTool.SaveGameInfo(GamePuzzleFourClockView.puzzleKey, "1", storyGameInfo.gamePointConfig.id);
        baseView.StartCoroutine(Complete());
    }

    void OrderError()
    {
        UIMgr.Ins.showErrorMsgWindow(MsgException.GAME_PUZZLE_ERROR);
        putDownList.Clear();
        for (int i = 0; i < bottomList.Count; i++)
        {
            DropOnError(i);
        }
    }

    IEnumerator Complete()
    {
        yield return new WaitForSeconds(1f);
        Controller controller = ui.GetController("c1");
        controller.selectedIndex = 2;
        FXMgr.CreateEffectWithGGraph(bgGgraph, new Vector2(0, 0), "UI_Game5", 162);
        yield return new WaitForSeconds(0.78f);
        bgGgraph.TweenMoveY(142, 1);
        SkeletonAnimation skeletonAnimation = FXMgr.LoadSpineEffect("Game5", spineGgraph, new Vector2(358, 872), 100);

        TrackEntry trackEntry = skeletonAnimation.AnimationState.SetAnimation(0, "animation", true);
        trackEntry.Loop = false;
        trackEntry.AnimationStart = 0f;
        trackEntry.AnimationEnd = 1.1f;
        skeletonAnimation.timeScale = 1;

        yield return new WaitForSeconds(1.1f);
        TrackEntry trackEntry0 = skeletonAnimation.AnimationState.SetAnimation(0, "animation", true);
        trackEntry0.AnimationStart = 1.1f;
        trackEntry0.AnimationEnd = 3.1f;
        skeletonAnimation.timeScale = 1;

        SearchChild("n12").onClick.Set(() =>
        {
            baseGameView.OnComplete();
        });
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
        //    GameNodeConsumeConfig list = JsonConfig.GameNodeConsumeConfigs.Find(a => a.node_id == storyGameInfo.gamePointConfig.id && a.type == storyGameInfo.gamePointConfig.type);
        //    TinyItem item = ItemUtil.GetTinyItem(list.pay);
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
        wWWForm.AddField("index", 1);
        GameMonoBehaviour.Ins.RequestInfoPost<PlayerProperty>(NetHeaderConfig.STROY_STEP_CONSUME, wWWForm, GetTip);
    }

    void GetTip()
    {
        for (int i = 0; i < 3; i++)
        {
            if (putDownList.Count <= i || putDownList[i] != 2 - i)
            {
                ShowTip(i);
                return;
            }
        }
        List<int> tips = new List<int>() { 3, 4, 5 };

        for (int i = 3; i < putDownList.Count; i++)
        {
            tips.Remove(putDownList[i]);
        }
         ShowTip(tips[0]);

    }

    void ShowTip(int index)
    {
        if (index < 3)
        {
            for (int i = putDownList.Count - 1; i >= index; i--)
            {
                DropOnError(putDownList[i]);
                putDownList.RemoveAt(i);
            }
            bottomList[2-index].TweenScale(Vector2.one, 0.5f);
            bottomList[2-index].TweenRotate(0, 0.5f);
            bottomList[2-index].touchable = false;
            bottomList[2-index].sortingOrder = 2;
            DropOnTrue(2 - index);

        }
        else
        {
            bottomList[index].TweenScale(Vector2.one, 0.5f);
            bottomList[index].TweenRotate(0, 0.5f);
            bottomList[index].touchable = false;
            bottomList[index].sortingOrder = 2;
            DropOnTrue(index);
        }

    }
}

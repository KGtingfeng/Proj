using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using System;
using Spine.Unity;
using Spine;

public class GamePuzzleMoudle : BaseMoudle
{
    /// <summary>
    /// 拼对光位置
    /// </summary>
    readonly List<Vector3> guangPos = new List<Vector3>()
    {
        new Vector3(-155,-596,0),
        new Vector3(223,-632,0),
        new Vector3(-39,-586,0),
        new Vector3(330,-730,0),
        new Vector3(140,-620,0),
        new Vector3(40,-620,0),
    };
    /// <summary>
    /// 碎片正确位置
    /// </summary>
    readonly List<Vector3> orderPos = new List<Vector3>()
    {
        new Vector3(170,683,0),
        new Vector3(539,688,0),
        new Vector3(316,725,0),
        new Vector3(624,601,0),
        new Vector3(436,712,0),
        new Vector3(343,673,0),
    };
    /// <summary>
    /// 碎片在底部位置
    /// </summary>
    readonly List<Vector3> defauldPos = new List<Vector3>()
    {
        new Vector3(127,1322,0),
        new Vector3(255,1340,0),
        new Vector3(394,1330,0),
        new Vector3(520,1328,0),
        new Vector3(634,1338,0),
        new Vector3(347,1449,68),
    };
    /// <summary>
    /// 碎片在底部缩放
    /// </summary>
    readonly List<Vector2> defauldScale = new List<Vector2>()
    {
        new Vector2(0.528f,0.531f),
        new Vector2(0.688f,0.688f),
        new Vector2(0.555f,0.555f),
        new Vector2(0.792f,0.792f),
        new Vector2(0.603f,0.603f),
        new Vector2(0.677f,0.677f),
    };
    List<GComponent> bottomList = new List<GComponent>();
    List<GComponent> clickAreaList = new List<GComponent>();
    List<int> putDownList=new List<int>();

    GComponent mask;
    GGraph shipGraph;
    GGraph shuihuaQian;
    GGraph shuihuaHou;
    GGraph waitGraph;
    GGraph shadowGraph;

    AudioClip putDownAudio;
    public override void InitMoudle(GComponent gComponent, int controllerIndex)
    {
        base.InitMoudle(gComponent, controllerIndex);
        InitUI();
        InitEvent();
    }
    public override void InitUI()
    {
        base.InitUI();
        shuihuaHou = SearchChild("n47").asGraph;
        shadowGraph = SearchChild("n48").asGraph;
        shipGraph = SearchChild("n49").asGraph;
        shuihuaQian = SearchChild("n50").asGraph;
        waitGraph = SearchChild("n51").asGraph;
        mask = SearchChild("n26").asCom;

        GetList(ui, 12, bottomList);
        GetClickArea();
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
            gList.Add(com);
        }
    }

    void GetClickArea()
    {
        clickAreaList = new List<GComponent>();
        for (int i = 0; i < 6; i++)
        {
            int index = i;
            GComponent com = SearchChild("n" + (i + 40)).asCom;
            com.onDrop.Set((EventContext context) =>
            {
                ShipOnDrop(index, context);
            });
            clickAreaList.Add(com);
        }
    }

    public override void InitEvent()
    {
        base.InitEvent();
        baseView.ui.GetChild("n2").asCom.onDrop.Set(BGOnDrop);
        baseView.ui.GetChild("n3").asCom.onDrop.Set(BGOnDrop);
        baseView.ui.GetChild("n4").asCom.onDrop.Set(BGOnDrop);
        mask.onDrop.Set(BGOnDrop);
        //tips
        baseView.SearchChild("n3").onClick.Set(OnClickTips);
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

    void ShipOnDrop(int index, EventContext context)
    {
        int dragIndex = int.Parse(context.data.ToString());
        switch (index)
        {
            case 0:
            case 1:
            case 3:
            case 4:
                if (dragIndex == index)
                    DropOnTrue(dragIndex);
                else
                    DropOnError(dragIndex);
                break;
            case 2:
                if (dragIndex == 2 || dragIndex == 5)
                {
                    DropOnTrue(dragIndex);
                    if (dragIndex == 5)
                        bottomList[dragIndex].sortingOrder = 1;
                }
                else
                    DropOnError(dragIndex);
                break;
            case 5:
                if (dragIndex == index)
                {
                    DropOnTrue(dragIndex);
                    bottomList[dragIndex].sortingOrder = 1;
                }
                else
                    DropOnError(dragIndex);
                break;
        }

    }

    void BGOnDrop(EventContext context)
    {
        int dragIndex = int.Parse(context.data.ToString());
        DropOnError(dragIndex);
    }

    void DropOnTrue(int index)
    { 
        bottomList[index].sortingOrder = 0;
        bottomList[index].TweenMove(orderPos[index], 0.1f);
        putDownList.Add(index);
        if (putDownAudio == null)
            putDownAudio = Resources.Load(SoundConfig.GAME_AUDIO_EFFECT_URL + (int)SoundConfig.GameAudioId.PutDown) as AudioClip;
        GRoot.inst.PlayEffectSound(putDownAudio);
        FXMgr.CreateEffectWithScale(ui, guangPos[index], "Game2_fit", 162, 1);
        if (putDownList.Count == 6)
        {
            GameTool.SaveGameInfo(GamePuzzleView.puzzleKey, "1", GamePuzzleView.view.storyGameInfo.gamePointConfig.id);
            baseView.StartCoroutine(Complete());
        }
    }

    void DropOnError(int index)
    {
        bottomList[index].sortingOrder = 0;
        bottomList[index].TweenMove(defauldPos[index], 1f).OnComplete(() =>
        {
            bottomList[index].touchable = true;
        });
        bottomList[index].TweenScale(defauldScale[index], 1f);
        bottomList[index].TweenRotate(defauldPos[index].z, 1f);
    }

    WaitForSeconds sortWait = new WaitForSeconds(0.5f);
    IEnumerator Complete()
    {
        FXMgr.CreateEffectWithScale(ui, new Vector2(328, 688), "Game2_wancheng", 1);
        yield return sortWait;
        //船下落
        baseView.controller.selectedIndex = 2;
        FXMgr.CreateEffectWithGGraph(waitGraph, new Vector2(245, -69), "Game2_wancheng2", 162);
        SkeletonAnimation shipAnimation = FXMgr.LoadSpineEffect("Chuan", shipGraph, new Vector2(-69, 12), 100, "chuan");
        TrackEntry trackEntry = shipAnimation.AnimationState.SetAnimation(0, "chuan", true);
        trackEntry.Loop = false;
        trackEntry.AnimationStart = 0f;
        trackEntry.AnimationEnd = 3f;
        shipAnimation.timeScale = 1;
        yield return sortWait;
        waitGraph.TweenMove(new Vector2(214, 442), 1.45f);
        yield return new WaitForSeconds(0.85f);
        //砸到水面
        AudioClip audioClip = Resources.Load(SoundConfig.GAME_AUDIO_EFFECT_URL + (int)SoundConfig.GameAudioId.Diving) as AudioClip;
        GRoot.inst.PlayEffectSound(audioClip);
        FXMgr.CreateEffectWithGGraph(shuihuaHou, new Vector2(230, 64), "Gmae2_shuihua_hou", 162);
        FXMgr.CreateEffectWithGGraph(shadowGraph, new Vector2(-669, 234), "Game2_daoying", 130);
        FXMgr.CreateEffectWithGGraph(shuihuaQian, new Vector2(199, 52), "Gmae2_shuihua_qian", 162);
        yield return new WaitForSeconds(1.65f);
        //船待机动画循环
        TrackEntry trackEntry0 = shipAnimation.AnimationState.SetAnimation(0, "chuan", true);
        trackEntry0.AnimationStart = trackEntry.TrackTime;
        trackEntry0.AnimationEnd = 6.3f;
        shipAnimation.timeScale = 1;


        ui.onClick.Set(() =>
        { 
                GamePuzzleView.view.OnComplete();
        });
    }

    Extrand extrand;
    void OnClickTips()
    {
        DoTips();
        //if (extrand == null)
        //{
        //    extrand = new Extrand();
        //    extrand.type = 1;
        //    extrand.key = "提示";
        //    List<GameNodeConsumeConfig> list = JsonConfig.GameNodeConsumeConfigs.FindAll(a => a.node_id == GamePuzzleView.view.storyGameInfo.gamePointConfig.id && a.type == GamePuzzleView.view.storyGameInfo.gamePointConfig.type);
        //    TinyItem item = ItemUtil.GetTinyItem(list[0].pay);
        //    extrand.item = item;
        //    extrand.msg = "获得提示";
        //    extrand.callBack = DoTips;
        //}
        //UIMgr.Ins.showNextPopupView<PopupDialogView, Extrand>(extrand);
    }

    void DoTips()
    {
        for (int i = 0; i < 6; i++)
        {
            if (!putDownList.Contains(i))
            {
                bottomList[i].TweenScale(Vector2.one, 0.5f);
                bottomList[i].TweenRotate(0, 0.5f);
                bottomList[i].touchable = false;
                bottomList[i].sortingOrder = 2;
                DropOnTrue(i);
                break;

            }
        }
    }
}

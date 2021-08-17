using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

public class GamePuzzleWatchMoudle : BaseMoudle
{
    /// <summary>
    /// 碎片正确位置
    /// </summary>
    readonly List<Vector3> orderPos = new List<Vector3>()
    {
        new Vector3(297,496,0),
        new Vector3(177,524,0),
        new Vector3(398,567,0),
        new Vector3(275,600,0),
        new Vector3(488,757,0),
        new Vector3(324,451,0),
    };
    /// <summary>
    /// 碎片在底部位置
    /// </summary>
    readonly List<Vector3> defauldPos = new List<Vector3>()
    {
        new Vector3(124,1503,0),
        new Vector3(245,1504,-50),
        new Vector3(333,1500,192),
        new Vector3(437,1495,30),
        new Vector3(531,1499,-3),
        new Vector3(630,1502,43),
    };
    /// <summary>
    /// 碎片在底部缩放
    /// </summary>
    readonly List<Vector2> defauldScale = new List<Vector2>()
    {
        new Vector2(0.775f,0.775f),
        new Vector2(0.447f,0.447f),
        new Vector2(0.382f,0.382f),
        new Vector2(0.39f,0.39f),
        new Vector2(0.345f,0.345f),
        new Vector2(0.467f,0.467f),
    };

    List<GComponent> bottomList = new List<GComponent>();
    List<GComponent> clickAreaList = new List<GComponent>();
    List<GComponent> putList = new List<GComponent>();
    List<int> putDownList = new List<int>();

    AudioClip putDownAudio;
    StoryGameInfo storyGameInfo;
    BaseGameView baseGameView;
    public override void InitMoudle(GComponent gComponent, int controllerIndex)
    {
        base.InitMoudle(gComponent, controllerIndex);
        InitUI();
        InitEvent();
    }

    public override void InitUI()
    {
        base.InitUI();
        GetList(ui, 12, bottomList, true);
        GetList(ui, 27, putList, false);

        GetClickArea();
        baseGameView = baseView as BaseGameView;
        storyGameInfo = baseGameView.storyGameInfo;
    }

    public override void InitEvent()
    {
        base.InitEvent();
        baseView.ui.GetChild("n2").onClick.Set(OnClickTips);
        SearchChild("n38").asCom.onDrop.Set(BGOnDrop);
        baseView.ui.GetChild("n1").asCom.onDrop.Set(BGOnDrop);
        baseView.ui.GetChild("n2").asCom.onDrop.Set(BGOnDrop);
        baseView.ui.GetChild("n3").asCom.onDrop.Set(BGOnDrop);
    }

    void GetList(GComponent gCom, int indexr, List<GComponent> gList, bool isBottom)
    {
        for (int i = 0; i < 6; i++)
        {
            int index = i;
            GComponent com = gCom.GetChild("n" + (i + indexr)).asCom;
            if (isBottom)
            {
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
                com.GetChild("n12").visible = false;
            }
            else
            {
                com.GetChild("n29").visible = false;
            }

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
        DropOnError(index);
    }

    void ShipOnDrop(int index, EventContext context)
    {
        int dragIndex = int.Parse(context.data.ToString());

        if (dragIndex == index)
            DropOnTrue(dragIndex);
        else
            DropOnError(dragIndex);
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
        if (putDownList.Count == 6)
        {
            GameTool.SaveGameInfo(GamePuzzleWatchView.puzzleWatchKey, "1", storyGameInfo.gamePointConfig.id);
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

    IEnumerator Complete()
    {
        yield return new WaitForSeconds(1f);
        FXMgr.CreateEffectWithScale(ui, new Vector2(328, 688), "Game3_huaibiaoding", 1, -1);
        baseView.controller.selectedIndex = 2;
        FXMgr.CreateEffectWithScale(ui, new Vector2(328, 688), "Game3_huaibiaodi", 1, -1);
        GGraph gGraph = new GGraph();
        ui.AddChild(gGraph);
        FXMgr.LoadSpineEffect("huaibiao", gGraph, new Vector2(415, 808), 120, "Idle");

        yield return new WaitForSeconds(1f);
        SearchChild("n38").onClick.Set(() =>
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
        wWWForm.AddField("index", 0);
        GameMonoBehaviour.Ins.RequestInfoPost<PlayerProperty>(NetHeaderConfig.STROY_STEP_CONSUME, wWWForm, GetTip);

    }

    void GetTip()
    {
        for (int i = 0; i < bottomList.Count; i++)
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

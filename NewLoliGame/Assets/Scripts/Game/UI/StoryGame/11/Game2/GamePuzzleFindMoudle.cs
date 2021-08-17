using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FairyGUI;

public class GamePuzzleFindMoudle : BaseMoudle
{
    /// <summary>
    /// 碎片回到对应位置
    /// </summary>
    readonly List<Vector2> posList = new List<Vector2>()
    {
        new Vector2(84,1257),
        new Vector2(209,1304),
        new Vector2(355,1307),
        new Vector2(507,1273),
        new Vector2(594,1289),
        new Vector2(190,1413),
    };
    /// <summary>
    /// 光位置
    /// </summary>
    readonly List<Vector2> guangPosList = new List<Vector2>()
    {
       new Vector2(-183,-12),
       new Vector2(-53,-12),
       new Vector2(77,-12),
       new Vector2(210,-12),
       new Vector2(325,-12),
       new Vector2(69,124),
    };
    List<GComponent> gList = new List<GComponent>();
    List<GComponent> bottomList = new List<GComponent>();
    List<int> gettedFrament=new List<int>();
    GImage shadow;
    AudioClip getItemAudio;
    public override void InitMoudle(GComponent gComponent, int controllerIndex)
    {
        base.InitMoudle(gComponent, controllerIndex);
        InitUI();
        InitEvent();
    }

    public override void InitUI()
    {
        base.InitUI();
        shadow = SearchChild("n11").asImage;
        GetList(ui, 5, gList);
        GetList(ui, 12, bottomList, false);
    }

    public override void InitEvent()
    {
        base.InitEvent();
        //tips
        baseView.SearchChild("n3").onClick.Set(OnClickTips);
    }

    void GetList(GComponent gCom, int indexr, List<GComponent> gList, bool isFrament = true)
    {
        for (int i = 0; i < 6; i++)
        {
            int index = i;
            GComponent com = gCom.GetChild("n" + (i + indexr)).asCom;
            if (isFrament)
                com.onClick.Set(() => { OnClickFrament(index); });
            else
                com.visible = false;
            gList.Add(com);
        }
    }

    void OnClickFrament(int index)
    { 
        gettedFrament.Add(index);
        if (index == 5)
        {
            shadow.visible = false;
            shadow.displayObject.gameObject.SetActive(false);
        }
        gList[index].onClick.Clear();
        PlayGetAudio();
        GetEffect(gList[index], posList[index], index);
        if (gettedFrament.Count >= 6)
        {
            GameTool.SaveGameInfo(GamePuzzleView.puzzleFramentsKey, "1", GamePuzzleView.view.storyGameInfo.gamePointConfig.id);
            baseView.StartCoroutine(WaitFramentFly());
        }
    }

    IEnumerator WaitFramentFly()
    {
        yield return new WaitForSeconds(2f);
        GotoNextStep();
    }


    GameTipsInfo gameTipsInfo;
    void GotoNextStep()
    {
        if (gameTipsInfo == null)
        {
            gameTipsInfo = new GameTipsInfo();
            gameTipsInfo.isShowBtn = true;
            gameTipsInfo.context = "你已经找到了所有碎片，快去修复时间轻舟吧！";
            gameTipsInfo.callBack = () => { baseView.StartCoroutine(SceneJump()); };

        }
        UIMgr.Ins.showNextPopupView<StoryTipsView, GameTipsInfo>(gameTipsInfo);
    }

    IEnumerator SceneJump()
    {
        UIMgr.Ins.showNextPopupView<GameScenceChangeView>();
        yield return new WaitForSeconds(1f);
        baseView.GoToMoudle<GamePuzzleMoudle>((int)GamePuzzleView.MoudleType.TYPE_PUZZLE);
    }

    /// <summary>
    /// 拾取动画
    /// </summary>
    void GetEffect(GObject gObject, Vector2 pos, int index)
    {
        PlayGetAudio();
        gObject.TweenMove(new Vector2(375, 812), 0.6f).OnComplete(() =>
        {
            gObject.TweenMove(pos, 1f).OnComplete(() =>
            {
                FXMgr.CreateEffectWithScale(ui, guangPosList[index], "Game2_fit", 162, 1);
                gObject.visible = false;
                bottomList[index].visible = true;
            });
        });
    }

    void PlayGetAudio()
    {
        if (getItemAudio == null)
            getItemAudio = Resources.Load(SoundConfig.GAME_AUDIO_EFFECT_URL + (int)SoundConfig.GameAudioId.GetItems) as AudioClip;
        GRoot.inst.PlayEffectSound(getItemAudio);
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
        for(int i = 0; i < 6; i++)
        {
            if (!gettedFrament.Contains(i))
            {
                OnClickFrament(i);
                break;
            }
        }
    }
}

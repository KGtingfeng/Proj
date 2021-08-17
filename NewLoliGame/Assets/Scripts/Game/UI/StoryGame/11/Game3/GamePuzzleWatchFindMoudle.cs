using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using System;
using UnityEngine;

public class GamePuzzleWatchFindMoudle : BaseMoudle
{

    /// <summary>
    /// 碎片回到对应位置
    /// </summary>
    readonly List<Vector2> posList = new List<Vector2>()
    {
        new Vector2(87,1466),
        new Vector2(185,1466),
        new Vector2(294,1460),
        new Vector2(391,1457),
        new Vector2(520,1431),
        new Vector2(596,1456),
    };


    List<GComponent> gList = new List<GComponent>();
    List<GComponent> bottomList = new List<GComponent>();
    List<int> gettedFrament=new List<int>();
    AudioClip getItemAudio;
    StoryGameInfo storyGameInfo;

    public override void InitMoudle(GComponent gComponent, int controllerIndex)
    {
        base.InitMoudle(gComponent, controllerIndex);
        InitUI();
        InitEvent();
    }

    public override void InitUI()
    {
        base.InitUI();
        GetList(ui, 5, gList);
        GetList(ui, 12, bottomList, false);
        BaseGameView baseGameView = baseView as BaseGameView;
        storyGameInfo = baseGameView.storyGameInfo;
    }

    public override void InitEvent()
    {
        base.InitEvent();
        baseView.ui.GetChild("n2").onClick.Set(OnClickTips);

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
        gList[index].onClick.Clear();
        PlayGetAudio();
        GetEffect(gList[index], posList[index], index);
        if (gettedFrament.Count >= 6)
        {
            GameTool.SaveGameInfo(GamePuzzleWatchView.puzzleWatchFindKey, "1", storyGameInfo.gamePointConfig.id);
            Complete();
        }
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
                gObject.visible = false;
                bottomList[index].visible = true;
            });
        });
    }

    void Complete()
    {
        GameTipsInfo gameTipsInfo = new GameTipsInfo
        {
            isShowBtn = true,
            context = "你已经找到了所有碎片，快去修复怀表吧！",
            callBack = () => { baseView.StartCoroutine(SceneJump()); },
        };

        UIMgr.Ins.showNextPopupView<StoryTipsView, GameTipsInfo>(gameTipsInfo);

    }

    IEnumerator SceneJump()
    {
        UIMgr.Ins.showNextPopupView<GameScenceChangeView>();
        yield return new WaitForSeconds(1f);
        baseView.GoToMoudle<GamePuzzleWatchMoudle>((int)GamePuzzleWatchView.MoudleType.TYPE_PUZZLE);
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
        for (int i = 0; i < 6; i++)
        {
            if (!gettedFrament.Contains(i))
            {
                OnClickFrament(i);
                break;
            }
        }
    }
}

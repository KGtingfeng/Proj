using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using System;

[ViewAttr("Game/UI/Y_Game5", "Y_Game5", "Game")]
public class GamePuzzleFourClockView : BaseGameView
{
    GLoader bgLoader;
    GComponent scene;
    public enum MoudleType
    {
        //找
        TYPE_FIND,
        //拼
        TYPE_PUZZLE,
    };
    readonly Dictionary<MoudleType, string> moudleNamePairs = new Dictionary<MoudleType, string>()
    {
        {MoudleType.TYPE_FIND,"n11"},
        {MoudleType.TYPE_PUZZLE,"n11"},
    };

    /// <summary>
    /// 碎片保存Key
    /// </summary>
    public static readonly string puzzleFramentsKey = "PuzzleFraments";
    /// <summary>
    /// 拼图保存Key
    /// </summary>
    public static readonly string puzzleKey = "puzzle";
    public override void InitUI()
    {
        base.InitUI();
        controller = SearchChild("n11").asCom.GetController("c1");
        bgLoader = SearchChild("n0").asLoader;
        scene = SearchChild("n11").asCom;
        InitEvent();
    }

    public override void InitEvent()
    {
        base.InitEvent();
        //返回
        SearchChild("n1").onClick.Set(() =>
        {
            EventMgr.Ins.DispachEvent(EventConfig.STORY_BREACK_STORY);
        });
        //skip
        SearchChild("n3").onClick.Set(SkipGame);
        //tips
        SearchChild("n2").onClick.Set(OnClickTips);
        EventMgr.Ins.ReplaceEvent(EventConfig.STORY_GAME_QUIT, Destroy<GamePuzzleFourClockView>);
        EventMgr.Ins.ReplaceEvent(EventConfig.STORY_DELETE_GAME_INFO, DeleteGameInfo);
    }

    public override void InitData<D>(D data)
    {
        base.InitData(data);
        storyGameInfo = data as StoryGameInfo;
        bgLoader.url = UrlUtil.GetGameBGUrl(20);
        StoryGameSave save = storyGameInfo.gameSaves.Find(a => a.ckey == puzzleFramentsKey);
        if (save == null)
            GoToMoudle<GamePuzzleFourClockFindMoudle>((int)MoudleType.TYPE_FIND);
        else if (storyGameInfo.gameSaves.Find(a => a.ckey == puzzleKey) == null)
            GoToMoudle<GamePuzzleFourClockMoudle>((int)MoudleType.TYPE_PUZZLE);
        else
            OnComplete();

    }

    public override void InitData()
    {
        base.InitData();
        bgLoader.url = UrlUtil.GetGameBGUrl(20);
        GoToMoudle<GamePuzzleFourClockFindMoudle>((int)MoudleType.TYPE_FIND);
    }

    GameTipsInfo gameTipsInfo;
    void OnClickTips()
    {
        if (gameTipsInfo == null)
        {
            gameTipsInfo = new GameTipsInfo();
            gameTipsInfo.isShowBtn = false;
            gameTipsInfo.context = "找到碎片，修复四时钟";
        }
        UIMgr.Ins.showNextPopupView<StoryTipsView, GameTipsInfo>(gameTipsInfo);
    }


    public override void GoToMoudle<T>(int index)
    {
        MoudleType moudleType = (MoudleType)index;
        BaseMoudle baseMoudle = FindMoudle<T>();

        if (baseMoudle == null)
        {
            baseMoudle = (BaseMoudle)Activator.CreateInstance(typeof(T));
            baseMoudles.Add(baseMoudle);
            GComponent gComponent = null;
            string componmentName;
            if (moudleNamePairs.TryGetValue(moudleType, out componmentName))
            {
                GObject gObject = ui.GetChild(componmentName);
                if (gObject == null)
                {
                    Debug.Log(componmentName + " not find, please check it");
                    return;
                }
                gComponent = gObject.asCom;
            }
            else
                gComponent = ui;
            baseMoudle.baseView = this;
            baseMoudle.InitMoudle(gComponent, index);
        }
        baseMoudle.InitData();
        SwitchController(index);
    }


}

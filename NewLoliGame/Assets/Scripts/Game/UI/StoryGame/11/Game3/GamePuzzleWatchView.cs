using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using System;

[ViewAttr("Game/UI/Y_Game3", "Y_Game3", "Game3")]
public class GamePuzzleWatchView : BaseGameView
{
    GLoader bgLoader;
    public enum MoudleType
    {
        //找
        TYPE_FIND,
        //拼
        TYPE_PUZZLE,
    };
    /// <summary>
    /// 碎片保存Key
    /// </summary>
    public static readonly string puzzleWatchFindKey = "PuzzleWatchFind";
    /// <summary>
    /// 拼图保存Key
    /// </summary>
    public static readonly string puzzleWatchKey = "puzzleWatch";
    readonly Dictionary<MoudleType, string> moudleNamePairs = new Dictionary<MoudleType, string>()
    {
        {MoudleType.TYPE_FIND,"n4"},
        {MoudleType.TYPE_PUZZLE,"n4"},
    };


    public override void InitUI()
    {
        base.InitUI();
        controller = SearchChild("n4").asCom.GetController("c1");
        bgLoader = SearchChild("n0").asLoader;
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

        EventMgr.Ins.ReplaceEvent(EventConfig.STORY_GAME_QUIT, Destroy<GamePuzzleWatchView>);
        EventMgr.Ins.ReplaceEvent(EventConfig.STORY_DELETE_GAME_INFO, DeleteGameInfo);
    }

    public override void InitData<D>(D data)
    {
        base.InitData(data);
        storyGameInfo = data as StoryGameInfo;
        bgLoader.url = UrlUtil.GetGameBGUrl(18);
        StoryGameSave save = storyGameInfo.gameSaves.Find(a => a.ckey == puzzleWatchFindKey);
        if (save == null)
            GoToMoudle<GamePuzzleWatchFindMoudle>((int)MoudleType.TYPE_FIND);
        else if (storyGameInfo.gameSaves.Find(a => a.ckey == puzzleWatchKey) == null)
            GoToMoudle<GamePuzzleWatchMoudle>((int)MoudleType.TYPE_PUZZLE);
        else
            OnComplete();

    }

    public override void InitData()
    {
        base.InitData();
        bgLoader.url = UrlUtil.GetGameBGUrl(18);

        GoToMoudle<GamePuzzleWatchFindMoudle>((int)MoudleType.TYPE_FIND);

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

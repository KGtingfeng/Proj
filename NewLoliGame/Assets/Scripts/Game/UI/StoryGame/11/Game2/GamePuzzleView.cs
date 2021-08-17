using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using System;
/// <summary>
/// 拼船游戏
/// </summary>
[ViewAttr("Game/UI/Y_Game2", "Y_Game2", "Game2")]
public class GamePuzzleView : BaseGameView
{
    public static GamePuzzleView view;
    GLoader bgLoader;
    GGraph biaopan;
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
        {MoudleType.TYPE_FIND,"n26"},
        {MoudleType.TYPE_PUZZLE,"n26"},
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
        view = this;
        controller = SearchChild("n26").asCom.GetController("c1");
        bgLoader = SearchChild("n0").asLoader;
        biaopan = SearchChild("n53").asGraph;
        scene = SearchChild("n26").asCom;
        FXMgr.CreateEffectWithGGraph(biaopan, new Vector2(298, -33), "Game2_biaopanguang", 162);
        InitEvent();
    }

    public override void InitEvent()
    {
        base.InitEvent();
        //返回
        SearchChild("n2").onClick.Set(() =>
        {
            EventMgr.Ins.DispachEvent(EventConfig.STORY_BREACK_STORY);
        });
        //skip
        SearchChild("n4").onClick.Set(SkipGame);
        
        
        EventMgr.Ins.ReplaceEvent(EventConfig.STORY_GAME_QUIT, Destroy<GamePuzzleView>);
        EventMgr.Ins.ReplaceEvent(EventConfig.STORY_DELETE_GAME_INFO, DeleteGameInfo);
    }

    public override void InitData<D>(D data)
    {
        base.InitData(data);
        storyGameInfo = data as StoryGameInfo;
        bgLoader.url = UrlUtil.GetGameBGUrl(14);
        StoryGameSave save = storyGameInfo.gameSaves.Find(a => a.ckey == puzzleFramentsKey);
        if (save == null)
            GoToMoudle<GamePuzzleFindMoudle>((int)MoudleType.TYPE_FIND);
        else if (storyGameInfo.gameSaves.Find(a => a.ckey == puzzleKey) == null)
            GoToMoudle<GamePuzzleMoudle>((int)MoudleType.TYPE_PUZZLE);
        else
            OnComplete();

    }

    public override void InitData()
    {
        base.InitData();
        bgLoader.url = UrlUtil.GetGameBGUrl(14);
        GoToMoudle<GamePuzzleFindMoudle>((int)MoudleType.TYPE_FIND);
    }

    GameTipsInfo gameTipsInfo;
    


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

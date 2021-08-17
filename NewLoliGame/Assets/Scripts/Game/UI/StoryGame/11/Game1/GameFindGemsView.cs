using System;
using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

[ViewAttr("Game/UI/Y_Game1", "Y_Game1", "Game1")]
public class GameFindGemsView : BaseGameView
{

    public enum MoudleType
    {
        //找
        TYPE_FIND,
        //拼
        TYPE_PUT,
    };
    readonly Dictionary<MoudleType, string> moudleNamePairs = new Dictionary<MoudleType, string>()
    {
        {MoudleType.TYPE_FIND,"n0"},
        {MoudleType.TYPE_PUT,"n24"},
    };

    /// <summary>
    /// 宝石保存Key
    /// </summary>
    public const string GEMS_KEY = "gems";
    /// <summary>
    /// 剑碎片保存Key
    /// </summary>
    public const string SWORD_KEY = "swords";
    /// <summary>
    /// 被砍藤蔓保存Key
    /// </summary>
    public const string TB_KEY = "treeBranch";
    /// <summary>
    /// 宝石装在哪个洞保存Key
    /// </summary>
    public const string HOLE_KEY = "holeGem";


    public override void InitUI()
    {
        base.InitUI();
        controller = ui.GetController("c1");
        InitEvent();
    }

    public override void InitEvent()
    {
        base.InitEvent();
        //返回
        SearchChild("n25").onClick.Set(() =>
        {
            EventMgr.Ins.DispachEvent(EventConfig.PUT_GEMS_SAVE);
            EventMgr.Ins.DispachEvent(EventConfig.STORY_BREACK_STORY);
            //EventMgr.Ins.DispachEvent(EventConfig.STORY_DELETE_GAME_INFO);
        });

        //skip
        SearchChild("n27").onClick.Set(SkipGame);
        EventMgr.Ins.ReplaceEvent(EventConfig.STORY_GAME_QUIT, Destroy<GameFindGemsView>);
        EventMgr.Ins.ReplaceEvent(EventConfig.STORY_DELETE_GAME_INFO, DeleteGameInfo);
    }


    public override void InitData<D>(D data)
    {
        base.InitData(data);
        storyGameInfo = data as StoryGameInfo;

        if (IsSceneFind())
            GoToMoudle<GemFindMoudle, StoryGameInfo>((int)MoudleType.TYPE_FIND, storyGameInfo);
        else
            GoToMoudle<GemPutMoudle, StoryGameInfo>((int)MoudleType.TYPE_PUT, storyGameInfo);
    }

    public override void InitData()
    {
        base.InitData();
        GoToMoudle<GemPutMoudle, StoryGameInfo>((int)MoudleType.TYPE_PUT, storyGameInfo);

    }

    /// <summary>
    /// 分析保存信息
    /// </summary>
    bool IsSceneFind()
    {
        StoryGameSave gameSave = storyGameInfo.gameSaves.Find(a => a.ckey == GEMS_KEY);
        if (gameSave != null)
        {
            string[] saves = gameSave.value.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (saves.Length >= 6)
                return false;
        }
        return true;
    }

    public override void Destroy<T>()
    {
        StopAllCoroutines();
        base.Destroy<T>();
    }

    public override void GoToMoudle<T, D>(int index, D data)
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
            baseMoudle.InitMoudle<D>(gComponent, index, data);
        }
        baseMoudle.InitData<D>(data);
        SwitchController(index);
    }

    public override void DeleteGameInfo()
    {
        DeleteByKey(GEMS_KEY);
        DeleteByKey(SWORD_KEY);
        DeleteByKey(TB_KEY);
        DeleteByKey(HOLE_KEY);
    }

    void DeleteByKey(string key)
    {

        StoryCacheMgr.storyCacheMgr.Delete(key, storyGameInfo.gamePointConfig.id);
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("nodeId", storyGameInfo.gamePointConfig.id);
        wWWForm.AddField("key", key);
        GameMonoBehaviour.Ins.RequestInfoPost<int>(NetHeaderConfig.STROY_DELETE_BY_KEY, wWWForm, null);
    }
}

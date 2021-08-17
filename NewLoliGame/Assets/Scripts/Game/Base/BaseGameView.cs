using FairyGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 小游戏
/// </summary>
public class BaseGameView : BaseView
{
    public StoryGameInfo storyGameInfo;

    /// <summary>
    /// 跳过游戏
    /// </summary>
    public virtual void SkipGame()
    {
        NormalInfo normalInfo = new NormalInfo();
        normalInfo.index = storyGameInfo.gamePointConfig.id;
        normalInfo.type = GameConsumeConfig.STORY_PASS_NODE_TYPE;
        EventMgr.Ins.DispachEvent(EventConfig.STORY_SKIP_NODE, normalInfo);
    }

    /// <summary>
    /// 删除View
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public virtual void Destroy<T>()
    {
        EventMgr.Ins.RemoveEvent(EventConfig.STORY_GAME_QUIT);
        UIMgr.Ins.HideView<T>();
        UIConfig.buttonSoundVolumeScale = 0.5f;

    }

    /// <summary>
    /// 游戏完成
    /// </summary>
    public virtual void OnComplete()
    {
        NormalInfo normalInfo = new NormalInfo();
        normalInfo.index = storyGameInfo.gamePointConfig.point1;
        EventMgr.Ins.DispachEvent(EventConfig.STORY_RECORD_SELECT_NODE, normalInfo);
        DeleteGameInfo();
    }

    /// <summary>
    /// 删除游戏信息
    /// </summary>
    public virtual void DeleteGameInfo()
    {

        StoryCacheMgr.storyCacheMgr.Delete(storyGameInfo.gamePointConfig.id);   

        EventMgr.Ins.RemoveEvent(EventConfig.STORY_DELETE_GAME_INFO);
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("nodeId", storyGameInfo.gamePointConfig.id);
        GameMonoBehaviour.Ins.RequestInfoPost<int>(NetHeaderConfig.STROY_DELETE_GAME, wWWForm, null);
    }

    /// <summary>
    /// 前往中间剧情
    /// </summary>
    public virtual void GotoNextNode()
    {
        if (storyGameInfo.gamePointConfig.point2 != 0)
        {
            NormalInfo normalInfo = new NormalInfo();
            normalInfo.index = storyGameInfo.gamePointConfig.point2;
            EventMgr.Ins.DispachEvent(EventConfig.STORY_TRIGGER_NEXT_NODE, normalInfo);
        }
    }
}

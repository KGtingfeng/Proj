using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsyncRequestMgr : MonoBehaviour
{
    public static AsyncRequestMgr asyncRequestMgr;


    public enum GetNetType
    {
        None,
        /// <summary>
        /// 渠道开关
        /// </summary>
        FunctionOff,
        /// <summary>
        /// 小红点
        /// </summary>
        RedPoint,
        /// <summary>
        /// 故事进去基本引导
        /// </summary>
        StoryGameSave,


    }

    private void Awake()
    {
        asyncRequestMgr = this;
    }




    public void AsyRequestInfo(GetNetType getNetType)
    {
        switch (getNetType)
        {
            case GetNetType.FunctionOff:
                StartCoroutine(AsyGetGameFunctionOffInfo());
                break;
            case GetNetType.RedPoint:
                StartCoroutine(AsyncGetRedPoint());
                break;
            case GetNetType.StoryGameSave:
                StartCoroutine(AsyGetStoryMessage());
                break;
        }
    }


    IEnumerator AsyGetGameFunctionOffInfo()
    {
        GameMonoBehaviour.Ins.RequestInfoPostWithoutLoading(NetHeaderConfig.CONFIG_GAME_ALL, new WWWForm(), (List<ChannelSwitchConfig> configs) =>
        {
            GameData.Configs = configs;
            ChannelSwitchConfig guid = configs.Find(a => a.key == ChannelSwitchConfig.KEY_GUID);
            if (guid != null)
            {
                if (guid.value == 1)
                    GameData.isOpenGuider = true;
                else
                    GameData.isOpenGuider = false;
            }
            else
            {
                GameData.isOpenGuider = false;
            }
        });

        yield return null;
    }


    IEnumerator AsyncGetRedPoint()
    {
        GameMonoBehaviour.Ins.RequestInfoPost<PlayerRedpoint>(NetHeaderConfig.RED_POINT,
           new WWWForm(), null, false);
        yield return null;
    }


    IEnumerator AsyGetStoryMessage()
    {
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("nodeId", 0);
        wWWForm.AddField("key", "Newbie");
        GameMonoBehaviour.Ins.RequestInfoPostHaveData<List<StoryGameSave>>(NetHeaderConfig.STROY_LOAD_GAME, wWWForm, (List<StoryGameSave> storyGameSaves) =>
        {
            StoryCacheMgr.storyCacheMgr.ServertSychronizedToLocal("Newbie", 0, storyGameSaves);

        }, false);
        yield return null;
    }

}

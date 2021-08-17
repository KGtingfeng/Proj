using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

//暂时没用
public class FriendDataMgr
{
    #region 单例
    private static FriendDataMgr instance;
    private FriendDataMgr() { }

    public static FriendDataMgr Ins
    {
        get
        {
            if (instance == null)
            {
                instance = new FriendDataMgr();
            }
            return instance;
        }
    }
    #endregion

    public List<FriendInfo> friendInfos;


    public void RequestFriendInfos(int page, int pageSize, Action<Friend> callback)
    {
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("page", page);
        wWWForm.AddField("pageSize", pageSize);
        //好友列表
        GameMonoBehaviour.Ins.RequestInfoPostHaveData(NetHeaderConfig.FRIEND_LIST, wWWForm, (Friend friendInfos) =>
        {
            ResolveGiftInfo(friendInfos);
            callback(friendInfos);
        });
    }

    private void ResolveGiftInfo(Friend friend)
    {
        foreach (FriendInfo item in friend.list)//设置礼品默认状态
        {
            item.presented = 1;     //未赠送
            item.presentedMe = 2;   //空
        }

        List<GiftCondition> sentList = friend.SentList;
        List<GiftCondition> receivedList = friend.ReceivedList;

        if (sentList != null && sentList.Count > 0)//更新送礼状态
        {
            foreach (GiftCondition condition in sentList)
            {
                FriendInfo info = friend.list.Find(a => a.id == condition.playerId);
                if (info != null)
                {
                    info.presented = condition.status;
                    info.giftId = condition.id;
                }
            }
        }

        if (receivedList != null && receivedList.Count > 0)//更新收礼状态
        {
            foreach (GiftCondition condition in receivedList)
            {
                FriendInfo info = friend.list.Find(a => a.id == condition.fromId);
                if (info != null)
                {
                    info.presentedMe = condition.status;
                    info.giftId = condition.id;
                }
            }
        }
    }

    Dictionary<int, List<GameObject>> framePool = new Dictionary<int, List<GameObject>>();
    Vector3 framePos = new Vector3(113f, 86, 1000);
    public void SetFrame(GGraph gGraph, GLoader gLoader, int level)
    {
        if (level > 2000)
        {
            gGraph.visible = true;
            gLoader.visible = false;
            if (string.IsNullOrEmpty(gGraph.url))
            {
                gGraph.url = level.ToString();
                GameObject go = GetFxObject(level);
                GoWrapper goWrapper = new GoWrapper();
                goWrapper.SetWrapTarget(go, true);
                gGraph.SetNativeObject(goWrapper);
            }
            else if (gGraph.url != level.ToString())
            {
                GoWrapper wrapper = gGraph.displayObject as GoWrapper;
                RemoveToDic(wrapper.wrapTarget, int.Parse(gGraph.url));
                gGraph.url = level.ToString();
                GameObject go = GetFxObject(level);
                wrapper.SetWrapTarget(go, true);
            }
            gGraph.displayObject.gameObject.transform.localScale = (Vector3.one * 45);
            gGraph.position = framePos;
        }
        else
        {
            gGraph.visible = false;
            gLoader.visible = true;
            gLoader.url = UrlUtil.GetAvatarFrame(level);
        }
    }

    /// <summary>
    /// 获取称号背景，如果称号背景是特效需特殊获得
    /// </summary>
    GameObject GetFxObject(int id)
    {
        GameObject go;
        List<GameObject> wrappers;
        if (framePool.TryGetValue(id, out wrappers))
        {
            if (wrappers.Count > 0)
            {
                go = wrappers[0];
                wrappers.RemoveAt(0);
            }
            else
            {
                go = FXMgr.CreateObject(UrlUtil.GetAvatarFrame(id));
            }
        }
        else
        {
            wrappers = new List<GameObject>();
            framePool.Add(id, wrappers);
            go = FXMgr.CreateObject(UrlUtil.GetAvatarFrame(id));
        }
        go.SetActive(true);
        return go;
    }

    void RemoveToDic(GameObject go, int id)
    {
        go.SetActive(false);
        List<GameObject> gos;
        if (framePool.TryGetValue(id, out gos))
        {
            gos.Add(go);
        }
        else
        {
            gos = new List<GameObject>();
            gos.Add(go);
            framePool.Add(id, gos);
        }
    }
}

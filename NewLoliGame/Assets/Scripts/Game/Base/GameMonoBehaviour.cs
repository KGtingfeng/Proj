using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

public class GameMonoBehaviour : MonoBehaviour
{
    private static GameMonoBehaviour ins;
    public static GameMonoBehaviour Ins
    {
        get
        {
            return ins;
        }
    }
    private void Awake()
    {
        ins = this;
    }



    public IEnumerator DelaySecondDoSth(Action action, float second = 0.2f)
    {
        yield return new WaitForSeconds(second);
        if (action != null)
            action();
    }


    public void DealyDoAction(Action action, float second = 0.2f)
    {
        StartCoroutine(DelaySecondDoSth(action, second));
    }

    private void Update()
    {
        if (!Stage.inst._audioDialog.isPlaying)
        {
            Stage.inst._audioBackGround.volume = 1;
        }
    }



    //public void RequestInfoPost(string method, WWWForm wWWForm, Action callBack)
    //{
    //    UIMgr.Ins.ShowNetLoadinWindow();
    //    StartCoroutine(NetMgr.Ins.RequestInfoPost(method, wWWForm, callBack));
    //}


    public void RequestInfoPost<T>(string method, WWWForm wWWForm, Action callBack, bool showLoading = true)
    {
        if (showLoading)
        {
            SmallLoadingMgr.smallLoadingMgr.Push(method);
            //UIMgr.Ins.ShowNetLoadinWindow();
        }

        StartCoroutine(NetMgr.Ins.RequestInfoPost<T>(method, wWWForm, callBack));
    }

    public void RequestInfoPostList<T>(string method, WWWForm wWWForm, Action callBack, bool showLoading = true)
    {
        //if (showLoading)
        //    UIMgr.Ins.ShowNetLoadinWindow();
        if (showLoading)
        {
            SmallLoadingMgr.smallLoadingMgr.Push(method);

        }
        StartCoroutine(NetMgr.Ins.RequestInfoListPost<T>(method, wWWForm, callBack));
    }


    public void RequestInfoPost<T>(string method, WWWForm wWWForm, Action<T> callBack)
    {

        //UIMgr.Ins.ShowNetLoadinWindow();
        SmallLoadingMgr.smallLoadingMgr.Push(method);
        StartCoroutine(NetMgr.Ins.RequestInfoPostCbInfo<T>(method, wWWForm, callBack));
    }

    public void RequestInfoPostWithoutLoading<T>(string method, WWWForm wWWForm, Action<T> callBack)
    {
        StartCoroutine(NetMgr.Ins.RequestInfoPostCbInfo<T>(method, wWWForm, callBack));
    }

    public void RequestInfoPostHaveData<T>(string method, WWWForm wWWForm, Action<T> callBack, bool showLoading = true)
    {
        if (showLoading)
        {
            SmallLoadingMgr.smallLoadingMgr.Push(method);
            //UIMgr.Ins.ShowNetLoadinWindow();
        }
        StartCoroutine(NetMgr.Ins.RequestInfoPostCbInfo<T>(method, wWWForm, callBack));
    }

    public void RequestInfoGet<T>(string method, Action callBack)
    {
        //UIMgr.Ins.ShowNetLoadinWindow();
        SmallLoadingMgr.smallLoadingMgr.Push(method);
        StartCoroutine(NetMgr.Ins.RequestInfoGet<T>(method, callBack));
    }



}

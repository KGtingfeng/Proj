using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using LitJson;

public class NetMgr
{
    private static NetMgr netMgr;
    public static NetMgr Ins
    {
        get
        {
            if (netMgr == null)
                netMgr = new NetMgr();
            return netMgr;
        }
    }

    private NetMgr()
    {

    }

    readonly String ERROR = "1";

    public IEnumerator RequestInfoPost<T>(string method, WWWForm wWWForm, Action callBack)
    {
        string requestUrl = DoRequestHead(method, wWWForm);
        using (UnityWebRequest www = UnityWebRequest.Post(requestUrl, wWWForm))
        {
            yield return www.SendWebRequest();
            UIMgr.Ins.HideNetLoadinWindow();
            if (www.isNetworkError || www.isHttpError)
            {
                SmallLoadingMgr.smallLoadingMgr.Popup(method);
                Debug.Log(method + "     " + www.error);
                if (method != NetHeaderConfig.ACTOR_SKIN_LIST)
                    UIMgr.Ins.showErrorMsgWindow(www.error);
            }
            else
            {
                DoPostCallBack<T>(method, callBack, www);
            }
        }
    }



    public IEnumerator RequestInfoGet<T>(string method, Action callBack)
    {

        string requestUrl = NetHeaderConfig.IP + method + "?userToken=" + GameData.token;
        Debug.Log("Get requestUrl <color=green>" + requestUrl + "</color>");
        using (UnityWebRequest www = UnityWebRequest.Get(requestUrl))
        {
            yield return www.SendWebRequest();
            //UIMgr.Ins.HideNetLoadinWindow();
            SmallLoadingMgr.smallLoadingMgr.Popup(method);
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                DoPostCallBack<T>(method, callBack, www);
            }
        }
    }

    public IEnumerator RequestInfoListPost<T>(string method, WWWForm wWWForm, Action callBack)
    {

        string requestUrl = DoRequestHead(method, wWWForm);
        using (UnityWebRequest www = UnityWebRequest.Post(requestUrl, wWWForm))
        {
            yield return www.SendWebRequest();
            //UIMgr.Ins.HideNetLoadinWindow();
            SmallLoadingMgr.smallLoadingMgr.Popup(method);
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                DoPostCallBackList<T>(method, callBack, www);
            }
        }
    }


    public IEnumerator RequestInfoPostCbInfo<T>(string method, WWWForm wWWForm, Action<T> callBack)
    {
        string requestUrl = DoRequestHead(method, wWWForm);
        using (UnityWebRequest www = UnityWebRequest.Post(requestUrl, wWWForm))
        {
            yield return www.SendWebRequest();
            //UIMgr.Ins.HideNetLoadinWindow();
            SmallLoadingMgr.smallLoadingMgr.Popup(method);
            if (www.isNetworkError || www.isHttpError)
            {
                // Debug.LogError(www.downloadHandler.text);
                Debug.Log(www.error);
                UIMgr.Ins.showErrorMsgWindow(www.error);
            }
            else
            {
                DoPostCallBackParam(method, callBack, www);
            }
            if (method == NetHeaderConfig.SERVER_HEARTBEAT)
                EventMgr.Ins.DispachEvent(EventConfig.GET_HEARTBEAT);

        }
    }



    /// <summary>
    /// 公用请求头
    /// </summary>
    /// <returns>The request head.</returns>
    /// <param name="method">Method.</param>
    /// <param name="wWWForm">W WWF orm.</param>
    private static string DoRequestHead(string method, WWWForm wWWForm)
    {
        wWWForm.AddField("userToken", GameData.token);
        if (GameData.User != null)
        {
            wWWForm.AddField("userId", GameData.User.id);
        }
        if (GameData.playerId > 0)
        {
            wWWForm.AddField("playerId", GameData.playerId);
        }
        string requestUrl = NetHeaderConfig.IP + method;
        Debug.Log("post requestUrl <color=green>" + requestUrl + "</color>");

        return requestUrl;
    }



    /// <summary>
    /// 回调函数不携带参数，解析按照列表对象解析
    /// </summary>
    /// <param name="method">Method.</param>
    /// <param name="callBack">Call back.</param>
    /// <param name="www">Www.</param>
    /// <typeparam name="T">The 1st type parameter.</typeparam>
    private static void DoPostCallBackList<T>(string method, Action callBack, UnityWebRequest www)
    {
        string content = www.downloadHandler.text;
        Debug.Log("<color=green>"+ method + "   Response Data: " + www.downloadHandler.text + "</color>");
        ResponseInforList<T> data = JsonUtility.FromJson<ResponseInforList<T>>(content);
        if (data.status.success == Status.OK)
        {
            ResponseDispatcher.Dispatcher<T>(method, data.entity, data.status);
            callBack?.Invoke();
        }
        else
        {
            Debug.Log("request error  code " + data.status.code + "  msg: " + data.status.msg);
            UIMgr.Ins.showErrorMsgWindow(data.status.msg);
        }

    }

    /// <summary>
    /// 毁掉函数不携带参数，普通对象解析
    /// </summary>
    /// <param name="method">Method.</param>
    /// <param name="callBack">Call back.</param>
    /// <param name="www">Www.</param>
    /// <typeparam name="T">The 1st type parameter.</typeparam>
    private static void DoPostCallBack<T>(string method, Action callBack, UnityWebRequest www)
    {
        string content = www.downloadHandler.text;
        Debug.Log("<color=green>Response Data: " + content + "</color>");
        ResponseInfor<T> data = JsonUtility.FromJson<ResponseInfor<T>>(content);
        if (data.status.success == Status.OK)
        {
            ResponseDispatcher.Dispatcher<T>(method, data.entity, data.status);
            callBack?.Invoke();
        }
        else
        {
            Debug.Log("request error  code " + data.status.code + "  msg: " + data.status.msg);
            UIMgr.Ins.showErrorMsgWindow(data.status.msg);
        }

        //UIMgr.Ins.HideNetLoadinWindow();
        SmallLoadingMgr.smallLoadingMgr.Popup(method);

        UIMgr.Ins.HideNetLoadinWindow();

    }

    /// <summary>
    /// 回调函数携带参数
    /// </summary>
    /// <param name="method">Method.</param>
    /// <param name="callBack">Call back.</param>
    /// <param name="www">Www.</param>
    /// <typeparam name="T">The 1st type parameter.</typeparam>
    private static void DoPostCallBackParam<T>(string method, Action<T> callBack, UnityWebRequest www)
    {
        string content = www.downloadHandler.text;
        Debug.Log("<color=green>"+method +  "   Response Data: " + www.downloadHandler.text + "</color>");
        ResponseInfor<T> data = JsonUtility.FromJson<ResponseInfor<T>>(content);
        if (data.status.success == Status.OK)
        {
            ResponseDispatcher.Dispatcher<T>(method, data.entity, data.status);
            if (callBack != null)
                callBack(data.entity);
        }
        else
        {
            Debug.Log("request error  code " + data.status.code + "  msg: " + data.status.msg);
            UIMgr.Ins.showErrorMsgWindow(data.status.msg);
        }
    }


    string checkResponseResult(JsonData jsonData)
    {
        string code = jsonData["errorCode"].ToString();
        if (code.Equals(ERROR))
            return jsonData["message"].ToString();
        return "";

    }




}

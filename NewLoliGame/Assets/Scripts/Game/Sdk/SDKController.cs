using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class SDKController : MonoBehaviour
{
    public static SDKController Instance;


#if UNITY_IPHONE
    [DllImport("__Internal")]
    static extern void RegisterIOSPush();
#endif

    private void Awake()
    {
        Instance = gameObject.GetComponent<SDKController>();
    }

    public void OnRegisterPushSuccess(string token)
    {
        Debug.Log("TPush Register Success！ token=" + token);
    }

    public void OnRegisterPushAccountSuccess(string token)
    {
        Debug.Log("TPush Register Account Success！");
    }

    public void OnHeartBeat(string msg)
    {
        ResponseInfor<ServerTime> data = JsonUtility.FromJson<ResponseInfor<ServerTime>>(msg);
        if (ServiceObject.ins != null) {
            ServiceObject.ins.Callback(data.entity);
        }
    }

    public void RegisterTPush()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        //AndroidJavaClass ajc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        //AndroidJavaObject jo = ajc.GetStatic<AndroidJavaObject>("currentActivity");
        //jo.Call("InitTPush");
#endif
#if UNITY_IPHONE && !UNITY_EDITOR
        RegisterIOSPush();
#endif
    }

    public void RegisterTPushAccount(string account)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        //AndroidJavaClass ajc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        //AndroidJavaObject jo = ajc.GetStatic<AndroidJavaObject>("currentActivity");
        //jo.Call("InitTPushAccont",account);
#endif
    }



    public void StartService()
    {
        string requestUrl = NetHeaderConfig.IP + NetHeaderConfig.SERVER_HEARTBEAT;

#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass ajc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject jo = ajc.GetStatic<AndroidJavaObject>("currentActivity");
        jo.Call("StartService", requestUrl, GameData.token, GameData.User.id.ToString(), GameData.playerId.ToString());
#endif
    }

    public void StopService()
    {

#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass ajc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject jo = ajc.GetStatic<AndroidJavaObject>("currentActivity");
        jo.Call("StopService");
#endif
    }

    public void OnEnterLoginMoudle()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaObject androidJavaObject = new AndroidJavaObject("com.guys.go.time.sand.QuickSdkMgr");
        androidJavaObject.Call("onLoginEnter");
#endif
    }

}

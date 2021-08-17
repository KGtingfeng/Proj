using FairyGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServiceObject : MonoBehaviour
{
    public static ServiceObject ins;
    WaitForSeconds wait = new WaitForSeconds(10f);
    public void Init()
    {
        ins = this;
        //EventMgr.Ins.RegisterEvent(EventConfig.GET_HEARTBEAT, RequestHeartbeat);
        //RequestHeartbeat();
    }

    void RequestHeartbeat()
    {
        //StartCoroutine(getHeartbeat());
    }

    IEnumerator getHeartbeat()
    {
        yield return wait;
        QueryHeartbeat();
    }

    void QueryHeartbeat()
    {
        WWWForm wWWForm = new WWWForm();
        GameMonoBehaviour.Ins.RequestInfoPostHaveData<ServerTime>(NetHeaderConfig.SERVER_HEARTBEAT, wWWForm, Callback, false);
    }

    public void Callback(ServerTime serverTime)
    {
        GameData.Delta_T = serverTime.currentTime - TimeUtil.TimeStamp() * 1000;
        //EventMgr.Ins.DispachEvent(EventConfig.REFRSH_COUNTDOWN);
        //SwitchConfig realName = GameData.SwitchConfigs.Find(a => a.key == SwitchConfig.KEY_REALNAME);
        //SwitchConfig time = GameData.SwitchConfigs.Find(a => a.key == SwitchConfig.KEY_TIME);
        //Debug.LogError("SPS       " + serverTime.currentTime);
        //if (realName != null && realName.value == "1" &&
        //time != null && time.value == "1" && GameData.User.status == 1)
        //{
        //    switch (serverTime.type)
        //    {
        //        case 0: break;
        //        case 1:
        //            ShowTimeLimit();
        //            break;
        //        case 2:
        //            ShowDurationLimit(float.Parse(serverTime.limitHour));
        //            break;
        //    }
        //}
    }

    Extrand extrand;
    public void ShowDurationLimit(float time)
    {
        if (extrand == null)
        {
            extrand = new Extrand
            {
                key = "确定",
                callBack = GotoLogin,
                type = 1
            };
        }
        extrand.msg = "根据健康系统限制，由于你是未成年玩家，非节假日仅能游戏" + time.ToString("0.00") + "小时。你今天已经进行游戏" + time.ToString("0.00") + "小时，不能继续游戏，请注意休息";
        UIMgr.Ins.showNextPopupView<RealNameView, Extrand>(extrand);

    }

    public void ShowTimeLimit()
    {
        if (extrand == null)
        {
            extrand = new Extrand
            {
                key = "确定",
                callBack = GotoLogin,
                type = 1
            };
        }
        extrand.msg = "根据健康系统限制，由于你是未成年玩家，每日22点 - 次日8点无法登陆，注意休息！明天再来吧！";
        UIMgr.Ins.showNextPopupView<RealNameView, Extrand>(extrand);
    }

    void GotoLogin()
    {
        GRoot.inst.HidePopup();
        EventMgr.Ins.Dispose();
        UIMgr.Ins.showViewWithReleaseOthers<LoginView>();
        ServiceObject service = FindObjectOfType<ServiceObject>();
        Destroy(service.gameObject);
    }

    private void OnDestroy()
    {
        EventMgr.Ins.RemoveEvent(EventConfig.GET_HEARTBEAT);
        StopAllCoroutines();

    }

}

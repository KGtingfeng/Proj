using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ViewAttr("Game/UI/M_Daily", "M_Daily", "Frame_reminder")]
public class SignInReminderView : BaseView
{
    WelfareInfo info;
    public override void InitUI()
    {
        base.InitUI();

        InitEvent();
    }
    public override void InitData<D>(D data)
    {
        OnShowAnimation();
        base.InitData(data);
        info = data as WelfareInfo;
    }

    public override void InitEvent()
    {
        base.InitEvent();

        SearchChild("n7").onClick.Set(() =>
        {
            OnHideAnimation();
        });

        SearchChild("n4").onClick.Set(() => //普通签到
        {
            int curDay = info.curTimes + 1;
            WWWForm wWWForm = new WWWForm();
            wWWForm.AddField("playerId", curDay);
            GameMonoBehaviour.Ins.RequestInfoPostHaveData<PropMake>(NetHeaderConfig.WELFARE_DAILY_CHECK, wWWForm, (PropMake propMake) =>
            {
                Debug.Log("领取" + curDay + "礼物");
                TouchScreenView.Ins.ShowPropsTost(propMake.playerProp);
                OnHideAnimation();
            });
        });

        SearchChild("n5").onClick.Set(() => //双倍签到
        {

        });
    }


}

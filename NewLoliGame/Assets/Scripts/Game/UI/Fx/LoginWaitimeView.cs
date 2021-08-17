using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

[ViewAttr("Game/UI/D_Login", "D_Login", "ani_guochang")]
public class LoginWaitimeView : BaseView
{
    CallBackList callBackList;
    Transition transition;
    public override void InitUI()
    {

        base.InitUI();
        transition = ui.GetTransition("t0");

        EventMgr.Ins.RegisterEvent(EventConfig.LOGIN_HIDE, Hide);
    }

    public override void InitData<D>(D data)
    {
        base.InitData(data);
        callBackList = data as CallBackList;

        Debug.Log("video test:A");
        if (callBackList != null)
        {
            Debug.Log("video test: B");
            transition.Play(1, 0, 0, 0.4f, () =>
            {
                callBackList.callBack1();
            });

        }
    }


    void Hide()
    {
        transition.Play(1, 0, 0.4f, 0.8f, () =>
        {
            UIMgr.Ins.HideView<LoginWaitimeView>();
        });
    }
}

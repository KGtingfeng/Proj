using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

[ViewAttr("Game/UI/D_Login", "D_Login", "ani_guochang")]
public class LoginAnimationView : BaseView
{
    CallBackList callBackList;
    Transition transition;
    public override void InitUI()
    {

        base.InitUI();
        transition = ui.GetTransition("t0");

    }

    public override void InitData<D>(D data)
    {
        base.InitData(data);
        callBackList = data as CallBackList;

        if (callBackList != null)
        {
            transition.SetHook("change", () =>
            {
                callBackList.callBack1?.Invoke();
            });
 
            transition.Play(()=> {
                onHide();
            });

        }
    }

}

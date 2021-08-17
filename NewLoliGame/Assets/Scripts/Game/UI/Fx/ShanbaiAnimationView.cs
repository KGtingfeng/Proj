using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

[ViewAttr("Game/UI/Fx_Ani", "Fx_Ani", "shanbai")]
public class ShanbaiAnimationView : BaseView
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

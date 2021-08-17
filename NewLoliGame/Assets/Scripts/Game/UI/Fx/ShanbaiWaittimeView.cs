using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

/// <summary>
/// 可手动控制时间
/// </summary>
[ViewAttr("Game/UI/Fx_Ani", "Fx_Ani", "shanbai")]
public class ShanbaiWaittimeView : BaseView
{
    CallBackList callBackList;
    Transition transition;
    public override void InitUI()
    {

        base.InitUI();
        transition = ui.GetTransition("t0");

        EventMgr.Ins.RegisterEvent(EventConfig.SHANBAI_HIDE, Hide);
    }

    public override void InitData<D>(D data)
    {
        base.InitData(data);
        callBackList = data as CallBackList;

        if (callBackList != null)
        {
             
            transition.Play(1,0,0,0.4f,()=> {
                callBackList.callBack1();
            });

        }
    }


    void Hide()
    {
        transition.Play(1, 0, 0.4f, 0.8f, () => {
            UIMgr.Ins.HideView<ShanbaiWaittimeView>();
        });
    }

}

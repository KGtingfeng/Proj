using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;
[ViewAttr("Game/UI/T_Common", "T_Common", "Frame_common_tips")]
public class TipsWindow : BaseWindow
{
    public override void InitUI()
    {
        CreateWindow<TipsWindow>();
        duration = 1f;
    }

    override protected void DoShowAnimation()
    {
        InitData();
        base.DoShowAnimation();
    }


    override protected void OnShown()
    {
        base.OnShown();

    }

    public override void InitData()
    {
        //base.InitData();
        ErrorMsg errorMs = message as ErrorMsg;
        SearchChild("n35").text = errorMs.msg;
    }

    override protected void OnHide()
    {

    }
}

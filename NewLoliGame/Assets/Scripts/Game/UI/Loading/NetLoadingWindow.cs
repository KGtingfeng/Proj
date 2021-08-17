using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;


[ViewAttr("Game/UI/T_Common", "T_Common", "Common")]
public class NetLoadingWindow : BaseWindow
{
    public static NetLoadingWindow _window;
    public override void InitUI()
    {
        CreateWindow<NetLoadingWindow>();
        _window = this;

    }


    public override void HideWindow()
    {
        //GRoot.inst.HidePopup(this);
        Hide();
    }

    protected override void OnShown()
    {
         
    }
}

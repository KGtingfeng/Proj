using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

[ViewAttr("Game/UI/T_Common", "T_Common", "Frame_bubble")]
public class ShowRoleTalkWindow : BaseWindow
{

    public override void InitUI()
    {
        
        CreateWindow<ShowRoleTalkWindow>();
        duration = 1.5f; 
        modal = false;
    }

   
    override protected void DoHideAnimation()
    {
         
    }

    override protected void OnShown()
    {
         

    }

    public override void InitData()
    {
        _window = this;
        ErrorMsg errorMs = message as ErrorMsg;
        SearchChild("n1").text = errorMs.msg;
    }

   
}

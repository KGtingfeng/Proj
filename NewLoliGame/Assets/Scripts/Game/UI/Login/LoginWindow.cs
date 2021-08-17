using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

[ViewAttr("Game/UI/D_Login", "D_Login", "Frame_login")]
public class LoginWindow : BaseWindow
{
 

    public override void InitUI()
    {
        CreateWindow<LoginWindow>(); 
        Debug.Log(window);

        SearchChild("n12").onClick.Add(() =>
        {

            Debug.Log("sss");
            Hide();

        });


        SearchChild("n13").onClick.Add(() =>
        {
            Debug.Log("onClick login btn");
        });


    }


    public override void InitData()
    {
        base.InitData();
        Debug.Log("InitData");
    }

    protected override void OnShown()
    {

    }

}

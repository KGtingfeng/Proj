using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

public class ModeifyPwdMoudle : BaseMoudle
{
    GTextInput accountTextInput;
    GTextInput oldTextInput;
    GTextInput firstPwdTextInput;
    GTextInput seconPwdTextInput;

    string accountName = "";
    string oldPwd="";
    string firstPwd = "";
    string secondPwd = "";
    public override void InitMoudle(GComponent gComponent, int controllerIndex)
    {
        base.InitMoudle(gComponent, controllerIndex);
        InitUI();
        InitEvent();
    }

    public override void InitUI()
    {
        accountTextInput = SearchChild("n8").asTextInput;
        oldTextInput = SearchChild("n10").asTextInput;
        firstPwdTextInput = SearchChild("n15").asTextInput;
        seconPwdTextInput = SearchChild("n16").asTextInput;
    }

    public override void InitEvent()
    {
        accountTextInput.onFocusOut.Set(() =>
        {
            accountName = accountTextInput.text.Trim();
        });
        oldTextInput.onFocusOut.Set(() =>
        {
            oldPwd = oldTextInput.text.Trim();
        });
        firstPwdTextInput.onFocusOut.Set(() =>
        {
            firstPwd = firstPwdTextInput.text.Trim();
        });
        seconPwdTextInput.onFocusOut.Set(() =>
        {
            secondPwd = seconPwdTextInput.text.Trim();
        }
        );
        //enter ModifyBtn
        SearchChild("n20").onClick.Set(() =>
        {
            baseView.SwitchController((int)LoginView.MoudleType.Login);
        });
        //back
        //GObject gObject = baseView.SearchChild("n36");
        //gObject.onClick.Set(() => baseView.SwitchController((int)LoginView.MoudleType.Login));

    }

    
}

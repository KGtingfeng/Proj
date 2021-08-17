using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

[ViewAttr("Game/UI/Z_Main", "Z_Main", "Frame_activecode")]
public class ActiveCodeView : BaseView
{
    GTextInput input;
    public override void InitUI()
    {
        base.InitUI();
        input = SearchChild("n9").asTextInput;
        InitEvent();
    }

    public override void InitData()
    {
        OnShowAnimation();
        base.InitData();
    }

    public override void InitEvent()
    {
        base.InitEvent();

        //点击空白处（蒙板）退出
        SearchChild("n8").onClick.Set(() =>
        {
            OnHideAnimation();
        });

        SearchChild("n4").onClick.Set(OnClickGet);

    }

    private void OnClickGet()
    {
        string code = input.text.Trim();
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("code", code);
        GameMonoBehaviour.Ins.RequestInfoPostHaveData<PropMake>(NetHeaderConfig.CODE_AWARD, wWWForm, (PropMake propMake) =>
        {
            input.text = "";
            TouchScreenView.Ins.ShowPropsTost(propMake.playerProp);
        });

    }
}

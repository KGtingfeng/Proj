using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

[ViewAttr("Game/UI/D_Message", "D_Message", "Dialogueimag", true)]
public class SMSImagePopView : BaseView
{
    GLoader gLoader;
    public override void InitUI()
    {
        base.InitUI();
        gLoader = SearchChild("n5").asLoader;
    }


    public override void InitData<D>(D data)
    {
        OnShowAnimation();
        base.InitData(data);
        string url = data as string;
        gLoader.url = url;
        gLoader.onClick.Set(OnHideAnimation);
    }

    public override void OnHideAnimation()
    {
        base.OnHideAnimation();
        UIMgr.Ins.HideView<SMSImagePopView>();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

[ViewAttr("Game/UI/Y_Game_common", "Y_Game_common", "frame_defeated", true)]
public class GameFailView : BaseView
{
    GameTipsInfo info;
    public override void InitUI()
    {
        base.InitUI();
        InitEvent();
    }
    public override void InitData<D>(D data)
    {
        base.InitData(data);
        info = data as GameTipsInfo;
        SearchChild("n18").asTextField.text = info.context;

    }
    public override void InitEvent()
    {
        base.InitEvent();
        SearchChild("n19").asButton.onClick.Set(()=>
        {
            onHide();
            info.callBack();
        });
    }
}
using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

public class DollNotHaveMoudle : BaseMoudle
{
    GameInitCardsConfig doll;
    public override void InitMoudle<T>(GComponent gComponent, int controllerIndex, T data)
    {
        base.InitMoudle(gComponent, controllerIndex, data);
        doll = data as GameInitCardsConfig;
        InitEvent();
    }

    public override void InitEvent()
    {
        SearchChild("n37").onClick.Add(Reback);
        SearchChild("n10").onClick.Add(()=>
        {
            UIMgr.Ins.showNextPopupView<DollStoryView, GameInitCardsConfig>(doll);
        });

    }

    public override void Reback()
    {
        baseView.SwitchController((int)RoleGropView.MoudleType.RoleInfo);
    }


    public override void InitData<D>(D data)
    {
        doll = data as GameInitCardsConfig;
    }

}

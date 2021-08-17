using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

[ViewAttr("Game/UI/Y_Game7", "Y_Game7", "tips")]
public class GameConnectJinmaiTipsView : BaseView
{

    public override void InitUI()
    {
        base.InitUI();
        controller = ui.GetController("c1");

        InitEvent();
    }

    public override void InitData<D>(D data)
    {
        base.InitData(data);
        Extrand extrand = data as Extrand;

        controller.selectedIndex = extrand.type;
        SearchChild("n22").onClick.Set(extrand.callBack.Invoke);

    }
}

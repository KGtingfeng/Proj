using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ViewAttr("Game/UI/Y_Game4", "Y_Game4", "Tips")]
public class GameImageTipsView : BaseView
{
    public override void InitUI()
    {
        base.InitUI();

        InitEvent();
    }

    public override void InitEvent()
    {
        base.InitEvent();
        SearchChild("n13").onClick.Set(() => { onDeleteAnimation<GameImageTipsView>(); });
    }

    public override void InitData()
    {
        OnShowAnimation();
        base.InitData();

    }
}

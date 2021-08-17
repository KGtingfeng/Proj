using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

/// <summary>
/// 场景变换
/// </summary>
[ViewAttr("Game/UI/Y_Game1", "Y_Game1", "Ani_zhuanchang")]
public class GameScenceChangeView : BaseView
{

    public override void InitData()
    {
        base.InitData();
        ui.GetTransition("t0").Play();
        StartCoroutine(Destroy());
    }

    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(2f);
        UIMgr.Ins.HideView<GameScenceChangeView>();
    }
}

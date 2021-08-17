using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ViewAttr("Game/UI/X_Beginner_guidance", "X_Beginner_guidance", "frame_tip")]
public class DailyTaskTipsView : BaseView
{

    public override void InitUI()
    {
        base.InitUI();
        controller = ui.GetController("c1");

    }

    NormalInfo info;
    public override void InitData<D>(D data)
    {
        base.InitData(data);
        info = data as NormalInfo;
        controller.selectedIndex = info.type;
        StartCoroutine(Hide());
    }

    IEnumerator Hide()
    {
        yield return new WaitForSeconds(3f);
        onHide();
    }

    public override void onHide()
    {
        base.onHide();
        StopAllCoroutines();
    }

}

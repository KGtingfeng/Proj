using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

[ViewAttr("Game/UI/K_Openning", "K_Openning", "Openning")]
public class VideoButtonView : BaseView
{
    Transition transition;
    Transition transition1;
    Extrand extrand;
    GComponent button;

    public override void InitUI()
    {
        base.InitUI();
        controller = SearchChild("n0").asCom.GetController("c1");
        button = SearchChild("n0").asCom.GetChild("n0").asCom;
        transition = button.GetTransition("t2");
        transition1 = ui.GetTransition("t0");
        button.onClick.Set(OnClick);
    }

    bool canClick;
    public override void InitData<D>(D data)
    {
        base.InitData(data);
        extrand = data as Extrand;
        controller.selectedIndex = extrand.type;
        button.asButton.selected = false;
        transition.PlayReverse();
        transition1.Play();
        canClick = true;
    }

    void OnClick()
    {
        if (!canClick)
            return;
        canClick = false;
        FXMgr.CreateEffectWithScale(button, new Vector3(133, 20, -4), "UI_Opening", 162, 3f);
        transition.Play();
        transition1.PlayReverse();

        StartCoroutine(WaitHide());
    }

    IEnumerator WaitHide()
    {

        extrand.callBack?.Invoke();
        yield return new WaitForSeconds(3f);
        onHide();
    }

}

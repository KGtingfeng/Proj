using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

[ViewAttr("Game/UI/D_Login", "D_Login", "Frame_update_tips")]
public class GameTipsView : BaseView
{
    GTextField title;
    GTextField content;

    Extrand extrand;
    public override void InitUI()
    {
        base.InitUI();

        title = SearchChild("n2").asTextField;
        content = SearchChild("n3").asTextField;

        InitEvent();
    }

    public override void InitEvent()
    {
        base.InitEvent();
        SearchChild("n4").onClick.Set(OnClickCannel);
        SearchChild("n5").onClick.Set(OnClickConfirm);
    }

    public override void InitData<D>(D data)
    {
        base.InitData(data);
        extrand = data as Extrand;
        title.text = extrand.key;
        content.text = extrand.msg;
    }

    void OnClickConfirm()
    {
        extrand.callBack.Invoke();
        onHide();
    }

    void OnClickCannel()
    {
        onHide();
    }
}

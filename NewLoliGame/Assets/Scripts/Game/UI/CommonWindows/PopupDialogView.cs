using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using FairyGUI;

[ViewAttr("Game/UI/T_Common", "T_Common", "Frame_update", true)]
public class PopupDialogView : BaseView
{
    Extrand extrand;
    GTextField titleText;
    GTextField contextText;

    GTextField costText;
    GLoader costLoader;
    GTextField getText;
    GGroup gGroup;
    public override void InitUI()
    {
        base.InitUI();
        titleText = SearchChild("n2").asTextField;
        contextText = SearchChild("n3").asTextField;
        costText = SearchChild("n12").asTextField;
        costLoader = SearchChild("n11").asLoader;
        controller = ui.GetController("c1");
        getText = SearchChild("n14").asTextField;
        gGroup = SearchChild("n16").asGroup;
        InitEvent();
    }


    public override void InitData<D>(D data)
    {
        base.InitData(data);
        OnShowAnimation();
        extrand = data as Extrand;
        if (extrand != null)
        {
            if (extrand.type == 0)
            {
                controller.selectedIndex = 0;
                titleText.text = extrand.key;
                contextText.text = extrand.msg;
            }
            else
            {
                controller.selectedIndex = 1;
                titleText.text = extrand.key;
                costText.text = extrand.item.num + "";
                costLoader.url = CommonUrlConfig.GetConsumeItemUrl((TypeConfig.Consume)extrand.item.type);
                getText.text = extrand.msg;
                gGroup.EnsureBoundsCorrect();

            }

        }
    }

    public override void InitEvent()
    {
        base.InitEvent();

        SearchChild("n4").onClick.Set(OnHideAnimation);
        SearchChild("n5").onClick.Set(() =>
        {
            OnHideAnimation();
            extrand.callBack?.Invoke();
        });
    }
}

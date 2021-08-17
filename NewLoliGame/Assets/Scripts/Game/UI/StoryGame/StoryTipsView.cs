using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

[ViewAttr("Game/UI/Y_Game2", "Y_Game2", "tips", true)]
public class StoryTipsView : BaseView
{
    GTextField finishTipsText;
    GTextField tipsText;
    GTextField titleText;
    GTextField btnTitleText;

    GButton gButton;
    public override void InitUI()
    {
        base.InitUI();
        tipsText = SearchChild("n24").asTextField;
        titleText = SearchChild("n23").asTextField;
        finishTipsText = SearchChild("n26").asTextField;
        controller = ui.GetController("c1");
        gButton = SearchChild("n28").asButton;
        btnTitleText = gButton.GetChild("n0").asButton.GetChild("title").asTextField;
        InitEvent();
    }

    public override void InitEvent()
    {
        base.InitEvent();

    }

    public override void InitData<D>(D data)
    {
        base.InitData(data);
        GameTipsInfo tipsInfo = data as GameTipsInfo;
        if (tipsInfo.isShowBtn)
        {
            controller.selectedIndex = 1;
            gButton.onClick.Set(() =>
            {
                tipsInfo.callBack?.Invoke();
                UIMgr.Ins.HideView<StoryTipsView>();
            });
            finishTipsText.text = tipsInfo.context;
            if (tipsInfo.btnTitle != null)
            {
                btnTitleText.text = tipsInfo.btnTitle;
            }
            if (tipsInfo.title != null)
            {
                titleText.text = tipsInfo.title;
            }
        }
        else
        {
            controller.selectedIndex = 0;
            SearchChild("n22").onClick.Set(() =>
            {
                UIMgr.Ins.HideView<StoryTipsView>();
            });
            tipsText.text = tipsInfo.context;
        }
    }
}

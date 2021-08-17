using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

[ViewAttr("Game/UI/Y_Game2", "Y_Game2", "tips2", true)]
public class GameFailTipsView : BaseView
{
    GButton gButton;
    GButton skip;
    GTextField content;
    GTextField fail;
    GTextField numText;
    public override void InitUI()
    {
        base.InitUI();
        controller = ui.GetController("c1");
        gButton = SearchChild("n32").asButton;
        skip = SearchChild("n34").asButton;
        content = SearchChild("n24").asTextField;
        fail = SearchChild("n26").asTextField;
        numText = skip.GetChild("n4").asTextField;
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
                UIMgr.Ins.HideView<GameFailTipsView>();
            });
            skip.onClick.Set(() =>
            {
                tipsInfo.skip?.Invoke();
                UIMgr.Ins.HideView<GameFailTipsView>();
            });
            if (!string.IsNullOrEmpty(tipsInfo.context))
            {
                fail.text = tipsInfo.context;
            }
            GameConsumeConfig gameConsumeConfig = DataUtil.GetConsumeConfig(GameConsumeConfig.STORY_PASS_NODE_TYPE);

            numText.text = gameConsumeConfig.tinyItem.num.ToString();
        }
        else
        {
            controller.selectedIndex = 0;
            SearchChild("n22").onClick.Set(() =>
            {
                UIMgr.Ins.HideView<GameFailTipsView>();
            });
            if (!string.IsNullOrEmpty(tipsInfo.context))
            {
                content.text = tipsInfo.context;
            }
        }
    }
}

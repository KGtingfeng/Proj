using FairyGUI;

[ViewAttr("Game/UI/Y_Game4", "Y_Game4", "Fail")]
public class Game4FailView : BaseView
{
    GTextField tipsText;
    GTextField titleText;
    GButton gButton;
    public override void InitUI()
    {
        base.InitUI();
        titleText = SearchChild("n15").asTextField;
        tipsText = SearchChild("n16").asTextField;
        controller = ui.GetController("c1");
        gButton = SearchChild("n18").asButton;
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
                UIMgr.Ins.HideView<Game4FailView>();
            });
            tipsText.text = tipsInfo.context;
            if (string.IsNullOrEmpty(tipsInfo.title))
            {
                titleText.text = tipsInfo.title;
            }
            if (string.IsNullOrEmpty(tipsInfo.btnTitle))
            {
                gButton.title = tipsInfo.btnTitle;
            }
        }
        else
        {
            controller.selectedIndex = 0;

            tipsText.text = tipsInfo.context;
        }

    }
}

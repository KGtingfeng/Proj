using FairyGUI;

[ViewAttr("Game/UI/Y_Game11", "Y_Game11", "tips")]
public class Game11TipsView : BaseView
{

    Extrand info;
    public override void InitUI()
    {
        base.InitUI();
        controller = ui.GetController("c1");

        InitEvent();
    }

    public override void InitEvent()
    {
        base.InitEvent();
        SearchChild("n34").onClick.Set(OnClickNext);
        SearchChild("n32").onClick.Set(OnClickBack);
        SearchChild("n22").onClick.Set(OnClickBack);
    }

    public override void InitData<D>(D data)
    {
        OnShowAnimation();
        base.InitData(data);
        info = data as Extrand;
        GameConsumeConfig gameConsumeConfig = DataUtil.GetConsumeConfig(GameConsumeConfig.STORY_PASS_NODE_TYPE);
        SearchChild("n34").asButton.GetChild("n4").asTextField.text = gameConsumeConfig.tinyItem.num + "";
        controller.selectedIndex = info.type;
        if (info.type == 2)
            SearchChild("n22").onClick.Set(OnClickCompelet);

    }

    void OnClickNext()
    {
        info.callBack.Invoke();
        OnHideAnimation();

    }

    void OnClickBack()
    {
        OnHideAnimation();
    }

    void OnClickCompelet()
    {
        info.extrand.Invoke();
        OnHideAnimation();

    }
}

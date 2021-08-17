using FairyGUI;

[ViewAttr("Game/UI/Y_Game_common", "Y_Game_common", "Frame_tu")]
public class GameImageView : BaseView
{

    GLoader gLoader;
    public override void InitUI()
    {
        base.InitUI();
        gLoader = SearchChild("n28").asLoader;

        InitEvent();
    }

    public override void InitEvent()
    {
        base.InitEvent();

        SearchChild("n27").onClick.Set(onHide);
    }

    public override void InitData<D>(D data)
    {
        base.InitData(data);
        gLoader.url = data as string;
    }
}

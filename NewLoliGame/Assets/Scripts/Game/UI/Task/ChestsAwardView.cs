
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

[ViewAttr("Game/UI/R_Task", "R_Task", "Frame_taskreview")]
public class ChestsAwardView : BaseView
{

    GList itemList;
    string award;
    List<TinyItem> tinyItems;
    public override void InitUI()
    {
        base.InitUI();
        itemList = SearchChild("n4").asList;
        InitEvent();
    }

    public override void InitEvent()
    {
        base.InitEvent();
        SearchChild("n6").onClick.Set(OnHideAnimation);
    }

    public override void InitData<D>(D data)
    {
        OnShowAnimation();
        base.InitData(data);
        award = data as string;
        tinyItems = ItemUtil.GetTinyItmeList(award);

        itemList.SetVirtual();
        itemList.itemRenderer = RenderItem;
        itemList.numItems = tinyItems.Count;


    }


    void RenderItem(int index, GObject gObject)
    {
        GComponent gCom = gObject.asCom;
        GLoader iconLoader = gCom.GetChild("n62").asLoader;
        GTextField numText = gCom.GetChild("n64").asTextField;
        iconLoader.url = UrlUtil.GetPropsIconUrl(tinyItems[index]);
        numText.text = tinyItems[index].num + "";

        gCom.onClick.Set(() =>
        {
            OnClickItem(index);
        });
    }

    private void OnClickItem(int index)
    {
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("propId", tinyItems[index].id);
        GameMonoBehaviour.Ins.RequestInfoPost(NetHeaderConfig.PLAYER_PROP_INFO, wWWForm, (PlayerProp playerProp) =>
        {
            playerProp.prop_type = tinyItems[index].type;
            ShopMallDataMgr.ins.CurrentPropInfo = playerProp;
            UIMgr.Ins.showNextPopupView<CommonPropTipsView, int>(tinyItems[index].id);

        });
    }


}

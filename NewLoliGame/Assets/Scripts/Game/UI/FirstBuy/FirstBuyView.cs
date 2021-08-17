using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

[ViewAttr("Game/UI/M_Daily", "M_Daily", "Frame_first_buy")]
public class FirstBuyView : BaseView
{

    GList list;
    GameMallConfig mallConfig;
    GamePropConfig propConfig;

    List<TinyItem> items;
    public override void InitUI()
    {
        base.InitUI();
        list = SearchChild("n20").asList;
        controller = ui.GetController("c1");
        InitEvent();
    }

    public override void InitEvent()
    {
        base.InitEvent();

        //点击空白处退出，返回主页面
        SearchChild("n21").onClick.Set(() =>
        {
            OnHideAnimation();
            EventMgr.Ins.DispachEvent(EventConfig.MAIN_OPEN_EFFECT);
            EventMgr.Ins.DispachEvent(EventConfig.REFRESH_MAIN_RED_POINT);
        });

        SearchChild("n15").onClick.Set(OnClickGet);

        SearchChild("n18").onClick.Set(() =>
        {
            Debug.Log("asdasd");
            UIMgr.Ins.showNextPopupView<ShopMallView>();
            OnHideAnimation();
        });
    }

    public override void InitData()
    {
        OnShowAnimation();
        base.InitData();
        mallConfig = JsonConfig.GameMallConfigs.Find(a => a.mall_id == 400001);
        propConfig = JsonConfig.GamePropConfigs.Find(a => a.prop_id == mallConfig.prop_id);

        items = ItemUtil.GetMallItemList(propConfig.pack_list);
        list.itemRenderer = ItemRenderer;
        list.numItems = items.Count;

        if (GameData.Player.charge_prop == -1)
        {
            controller.selectedIndex = 1;
        }
        else if (GameData.Player.charge_prop == 0)
        {
            controller.selectedIndex = 0;

        }
        else
        {
            controller.selectedIndex = 2;

        }
    }

    private void ItemRenderer(int index, GObject o)
    {
        GComponent gCom = o.asCom;
        Controller controller = gCom.GetController("c1");
        GLoader icon = gCom.GetChild("n14").asLoader;
        GTextField numText = gCom.GetChild("n4").asTextField;
        //GTextField nameText = gCom.GetChild("n13").asTextField;
        icon.url = "Game/Props/" + items[index].id;
        numText.text = "X" + items[index].num;
        GamePropConfig gameProp = JsonConfig.GamePropConfigs.Find(a => a.prop_id == items[index].id);
        //nameText.text = gameProp.prop_name;
        gCom.onClick.Set(()=> {
            OnClickAward(items[index].id);
        });
        controller.selectedIndex = 3;
    }

    void OnClickAward(int id)
    {

        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("propId", id);
        GameMonoBehaviour.Ins.RequestInfoPost<PlayerProp>(NetHeaderConfig.PLAYER_PROP_INFO, wWWForm, (PlayerProp playerProp) =>
        {
            UIMgr.Ins.showNextPopupView<CommonPropTipsView, int>(id);
        });
    }
    private void OnClickGet()
    {
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("mallId", mallConfig.mall_id);
        wWWForm.AddField("num", 1);
        GameMonoBehaviour.Ins.RequestInfoPost<PropMake>(NetHeaderConfig.MALL_BUY, wWWForm, (PropMake propMake) =>
        {
            controller.selectedIndex = 2;
            GameData.Player.charge_prop = 1;
            TouchScreenView.Ins.ShowPropsTost(propMake.playerProp);
        });
    }

}

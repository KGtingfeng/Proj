using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

[ViewAttr("Game/UI/S_Shop", "S_Shop", "Frame_buy")]
public class BuyMallView : BaseView
{
    GTextField mallName;

    GLoader mallGLoader;
    GTextField mallOwnNum;
    GTextField mallIntroduction;
    //购买数量
    GTextField buyNumText;
    GTextField buyPriceText;
    //消耗类型
    GLoader consumeLoader;

    GList packageList;
    GTextField descriptionText;
    GTextField limitInfoText;
    List<TinyItem> packTinyItems;

    GameMallConfig gameMallConfig;
    GamePropConfig propConfig;
    //单个物品价格
    TinyItem tinyItem;
    //物品最大消耗
    int maxConsumeNum;
    //购买数量
    int num = 1;

    //增加一个后的总价
    float nextPrice = 0;
    public override void InitUI()
    {
        base.InitUI();

        controller = ui.GetController("c1");
        mallGLoader = SearchChild("n32").asLoader;
        mallName = SearchChild("n15").asTextField;
        mallOwnNum = SearchChild("n31").asTextField;
        mallIntroduction = SearchChild("n10").asTextField;

        consumeLoader = SearchChild("n24").asLoader;
        buyNumText = SearchChild("n16").asTextField;
        buyPriceText = SearchChild("n23").asTextField;

        packageList = SearchChild("n40").asList;
        limitInfoText = SearchChild("n38").asTextField;
        descriptionText = SearchChild("n36").asTextField;
        InitEvent();
    }

    public override void InitData<D>(D data)
    {
        OnShowAnimation();
        base.InitData(data);
        gameMallConfig = data as GameMallConfig;
        propConfig = DataUtil.GetGamePropConfig(gameMallConfig.prop_id);
        if (propConfig == null)
            return;
        maxConsumeNum = GetMaxConsume();
        mallName.text = propConfig.prop_name;
        num = 1;

        if (gameMallConfig.mall_id >= 700000 && gameMallConfig.mall_id < 900000)
        {
            controller.selectedIndex = 1;
            RefreshMallList();
            return;
        }
        controller.selectedIndex = 0;
        RefreshMallItemInfos();
    }

    public override void InitEvent()
    {
        base.InitEvent();
        LongClickInfo();
        SearchChild("n18").onClick.Set(OnHideAnimation);
        SearchChild("n17").onClick.Set(onClickBuy);

    }

    /// <summary>
    /// 更新物品信息
    /// </summary>
    void RefreshMallItemInfos()
    {
        mallGLoader.url = UrlUtil.GetItemIconUrl(gameMallConfig.prop_id);
        mallIntroduction.text = propConfig.description;

        tinyItem = ItemUtil.GetTinyItem(gameMallConfig.cost);
        consumeLoader.url = CommonUrlConfig.GetConsumeItemUrl((TypeConfig.Consume)tinyItem.type);
        RefreshConsume();
        refreshMallOwnNumInfo(gameMallConfig.prop_id);
    }

    /// <summary>
    /// 礼包列表
    /// </summary>
    void RefreshMallList()
    {
        descriptionText.text = propConfig.description;
        limitInfoText.visible = propConfig.limit != 0;
        limitInfoText.text = gameMallConfig.limit_num + "/" + gameMallConfig.limit_num;

        packTinyItems = ItemUtil.GetMallItemList(propConfig.pack_list);
        if (packTinyItems != null)
        {
            packageList.itemRenderer = RenderListItem;
            packageList.numItems = packTinyItems.Count;
        }
    }

    void RenderListItem(int index, GObject obj)
    {
        obj.SetPivot(0.5f, 0.5f);
        GComponent itemCom = obj.asCom;
        GLoader gLoader = itemCom.GetChild("n26").asLoader;
        gLoader.url = UrlUtil.GetItemIconUrl(packTinyItems[index].id);
        GTextField gTextField = itemCom.GetChild("n41").asTextField;
        gTextField.text = "X" + packTinyItems[index].num;

        itemCom.onClick.Set(() =>
        {
            refreshMallOwnNumInfo(packTinyItems[index].id);
        });
    }

    /// <summary>
    /// 单个物品拥有数量
    /// </summary>
    void refreshMallOwnNumInfo(int propId)
    {
        if (ShopMallDataMgr.ins.CurrentPropInfo != null && ShopMallDataMgr.ins.CurrentPropInfo.prop_id == propId)
        {
            initMallOwnInfo(propId);
            return;
        }
        QueryPlayerPropNum(propId);
    }

    /// <summary>
    /// 初始化物品信息
    /// </summary>
    /// <param name="propId"></param>
    void initMallOwnInfo(int propId)
    {
        if (controller.selectedIndex == 0)
        {
            mallOwnNum.text = "拥有数量：" + ShopMallDataMgr.ins.CurrentPropInfo.prop_count;
            return;
        }
        else
        {
            UIMgr.Ins.showNextPopupView<CommonPropTipsView, int>(propId);
        }
    }


    /// <summary>
    /// 获取可使用最大价格
    /// </summary>
    /// <returns></returns>
    int GetMaxConsume()
    {
        TinyItem tinyItem = ItemUtil.GetTinyItem(gameMallConfig.cost);
        switch (tinyItem.type)
        {
            case (int)TypeConfig.Consume.Diamond:
                return GameData.Player.diamond;
            case (int)TypeConfig.Consume.Star:
                return GameData.Player.love;
        }
        return 0;
    }


    /// <summary>
    /// 根据购买数量刷新价格
    /// </summary>
    /// <param name="num"></param>
    void RefreshConsume()
    {
        float price = num * tinyItem.num;
        nextPrice = (num + 1) * tinyItem.num;
        if (gameMallConfig.discount != 100)
        {
            price = price * 0.01f * gameMallConfig.discount;
            nextPrice = nextPrice * 0.01f * gameMallConfig.discount;
        }
        buyPriceText.text = "" + price;
        buyNumText.text = "" + num;
    }

    /// <summary>
    /// 请求物品数量
    /// </summary>
    void QueryPlayerPropNum(int propId)
    {
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("propId", propId);
        GameMonoBehaviour.Ins.RequestInfoPost<PlayerProp>(NetHeaderConfig.PLAYER_PROP_INFO, wWWForm, () =>
        {
            initMallOwnInfo(propId);
        }, false);
    }

    void MallBuyCallBack(PropMake propMake)
    {
        OnHideAnimation();
        List<PlayerProp> playerProp = propMake.playerProp;
        if (playerProp != null && playerProp.Count > 0)
        {
            string url = "";
            TinyItem tiny = new TinyItem();
            if (playerProp.Count > 1)
            {
                foreach (PlayerProp prop in playerProp)
                {
                    prop.prop_count = packTinyItems.Find(a => a.id == prop.prop_id).num;
                }
                TouchScreenView.Ins.ShowPropsTost(playerProp);
                return;
            }
            GamePropConfig propConfig = DataUtil.GetGamePropConfig(playerProp[0].prop_id);
            if (propConfig != null)
            {
                int propNum;
                if (gameMallConfig.mall_id >= 700000 && gameMallConfig.mall_id < 900000)
                    propNum = packTinyItems.Find(a => a.id == playerProp[0].prop_id).num;
                else
                    propNum = num;
                url = UrlUtil.GetItemIconUrl(playerProp[0].prop_id);
                tiny = new TinyItem
                {
                    name = propConfig.prop_name,
                    url = url,
                    num = propNum,
                };
                UIMgr.Ins.showNextPopupView<CommonGetPropsView, TinyItem>(tiny);
                //UIMgr.Ins.showWindow<RoleAttributeUpgradeSuccessWindow, TinyItem>(tiny);
            }
        }
    }


    void onClickBuy()
    {
        if (AntiAddictionMgr.Instance.CanBuy(gameMallConfig))
        {
            Debug.LogError("mallId:  " + gameMallConfig.mall_id);
            WWWForm wWWForm = new WWWForm();
            wWWForm.AddField("mallId", gameMallConfig.mall_id);
            wWWForm.AddField("num", num);
            GameMonoBehaviour.Ins.RequestInfoPost<PropMake>(NetHeaderConfig.MALL_BUY, wWWForm, MallBuyCallBack);
        }
    }
    void LongClickInfo()
    {
        LongPressGesture minusBtn = new LongPressGesture(SearchChild("n13"));
        minusBtn.trigger = 0.01f;
        minusBtn.interval = 0.1f;
        minusBtn.onAction.Set(OnLongPressMinusNumBtn);

        LongPressGesture plusBtn = new LongPressGesture(SearchChild("n12"));
        plusBtn.trigger = 0.01f;
        plusBtn.interval = 0.1f;
        plusBtn.onAction.Set(OnLongPressPlusNumBtn);

    }

    void OnLongPressMinusNumBtn(EventContext context)
    {
        if (num > 1)
        {
            num -= 1;
            RefreshConsume();
        }
    }

    void OnLongPressPlusNumBtn(EventContext context)
    {
        if (nextPrice <= maxConsumeNum)
        {
            num += 1;
            RefreshConsume();
        }
    }


}

using FairyGUI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ViewAttr("Game/UI/S_Shop", "S_Shop", "Shop")]
public class ShopMallView : BaseView
{
    public static ShopMallView view;
    GLoader bgLoader;

    GTextField love;
    GTextField diamond;

    GList topBtnList;
    GList goodsList;
    GList cardList;
    //商品
    GLoader goodsIcon;
    GTextField goodsName;
    //限购
    GGroup buyTimesGroup;
    GTextField buyTimes;

    //原价]
    Controller priceController;
    GTextField originalNum;
    GTextField consumeNum;
    GLoader consumeTypeIcon;

    Controller hotController;
    GComponent hotGcom;
    GObject discountObj;

    GObject timeLimitObj;
    GTextField timeLimitInfo;
    GTextField timeLimitText;
    /// <summary>
    /// 当前页面展示的商品
    /// </summary>
    List<GameMallConfig> currentGameMalls;
    //topBtnList 选中按钮
    int selectIndex = 0;

    public override void InitUI()
    {
        base.InitUI();
        view = this;
        bgLoader = SearchChild("n14").asLoader;
        love = SearchChild("n19").asCom.GetChild("n15").asTextField;
        diamond = SearchChild("n19").asCom.GetChild("n16").asTextField;

        topBtnList = SearchChild("n9").asList;
        goodsList = SearchChild("n13").asList;
        goodsList.scrollPane.decelerationRate = 0.5f;
         cardList = SearchChild("n18").asList;
        controller = ui.GetController("c1");
        InitEvent();
    }

    public override void InitData()
    {
        //OnShowAnimation();
        onShow();
        base.InitData();
        bgLoader.url = UrlUtil.GetShopBgUrl("Shop");
        RefreshTopInfo();

        QueryMallInfo();
    }

    public override void InitData<D>(D data)
    {
        onShow();
        base.InitData(data);
        NormalInfo normalInfo = data as NormalInfo;
        bgLoader.url = UrlUtil.GetShopBgUrl("Shop");
        RefreshTopInfo();

        QueryMallInfo(normalInfo.type);

    }

    public override void InitEvent()
    {
        base.InitEvent();
        SearchChild("n8").onClick.Set(() =>
        {
            EventMgr.Ins.DispachEvent(EventConfig.REFRESH_PROPS);
            EventMgr.Ins.DispachEvent(EventConfig.REFRESH_MAKE_GIFTS);
            UIMgr.Ins.showMainView<ShopMallView>();
        });
        SearchChild("n19").asCom.GetChild("n13").onClick.Set(() =>
        {
            UIMgr.Ins.showNextPopupView<BuyStarsView>();
        });
        //click diamondBtn 
        SearchChild("n19").asCom.GetChild("n14").onClick.Set(() =>
        {
            refreshMallsInfo(0);
        });
        topBtnList.onClickItem.Set(OnClickItem);

        EventMgr.Ins.RegisterEvent(EventConfig.REFRESH_SHOPMALL_TOP_INFO, RefreshTopInfo);
    }

    /// <summary>
    /// 刷新顶部货币信息
    /// </summary>
    public void RefreshTopInfo()
    {
        love.text = GameData.Player.love + "";
        diamond.text = GameData.Player.diamond + "";
    }

    /// <summary>
    /// 点击顶部钻石刷新
    /// </summary>
    /// <param name="index"></param>
    public void refreshMallsInfo(int index)
    {
        topBtnList.selectedIndex = index;
        selectIndex = index;
        currentGameMalls = ShopMallDataMgr.ins.GetCurrentGameMalls(index);
        if (index == 0)
        {
            controller.selectedIndex = 4;
            RefreshCardList();
        }
        else
        {
            controller.selectedIndex = 0;
            RefreshGoodsList();

        }
    }

    private void RefreshCardList()
    {
        for (int i = 0; i < currentGameMalls.Count; i++)
        {
            GComponent gCom = cardList.GetChildAt(i).asCom;
            GTextField getNumText = gCom.GetChild("n7").asTextField;
            GTextField dailyNumText = gCom.GetChild("n11").asTextField;
            GTextField costNumText = gCom.GetChild("n8").asTextField;
            GTextField cdText = gCom.GetChild("n10").asTextField;
            TinyItem cost = ItemUtil.GetTinyItem(currentGameMalls[i].cost);
            TinyItem award = ItemUtil.GetTinyItem(currentGameMalls[i].award);
            TinyItem daily = ItemUtil.GetTinyItem(currentGameMalls[i].extra);

            costNumText.text = cost.num.ToString();
            getNumText.text = award.num.ToString();
            dailyNumText.text = daily.num.ToString();
            cdText.text = "";
            if (ShopMallDataMgr.ins.cardsInfo != null)
            {
                PlayerProp prop = ShopMallDataMgr.ins.cardsInfo.Find(a => a.prop_id == currentGameMalls[i].prop_id);
                if (prop != null)
                {
                    cdText.text = "有效期:" + prop.day + "天";
                }
            }
            int index = i;
            gCom.onClick.Set(() => { OnClickBuyCard(currentGameMalls[index]); });
        }
    }

    private void OnClickBuyCard(GameMallConfig gameMall)
    {
        Extrand extrand = new Extrand();
        extrand.key = "" + gameMall.mall_id;
        TinyItem cost = ItemUtil.GetTinyItem(gameMall.cost);

        extrand.msg = "确认消费" + cost.num + "元，获得" + gameMall.name;
        extrand.callBack = () =>
        {

            TinyItem tiny = ItemUtil.GetTinyItem(gameMall.award);
            UIMgr.Ins.showNextPopupView<CommonGetPropsView, TinyItem>(tiny);
            if (ShopMallDataMgr.ins.cardsInfo == null)
            {
                ShopMallDataMgr.ins.cardsInfo = new List<PlayerProp>();
            }
            PlayerProp prop = ShopMallDataMgr.ins.cardsInfo.Find(a => a.prop_id == gameMall.prop_id);
            if (prop == null)
            {
                prop = new PlayerProp()
                {
                    prop_id = gameMall.prop_id,
                    day = 0,
                };
                ShopMallDataMgr.ins.cardsInfo.Add(prop);
            }
            if (gameMall.mall_id == currentGameMalls[0].mall_id)
            {
                prop.day += 30;
            }
            else
            {
                prop.day += 90;
            }
            refreshMallsInfo(topBtnList.selectedIndex);

        };
        UIMgr.Ins.showNextPopupView<RechargeTipsView, Extrand>(extrand);
    }

    void RefreshGoodsList()
    {
        goodsList.SetVirtual();

        goodsList.itemRenderer = RenderListItem;
        goodsList.numItems = currentGameMalls.Count;
        goodsList.ScrollToView(0);

        SetGoodsItemEffect();
    }

    void SetGoodsItemEffect()
    {
        Vector3 pos = new Vector3();
        for (int i = 0; i < goodsList.numChildren; i++)
        {
            GObject item = goodsList.GetChildAt(i);

            item.alpha = 0;

            pos = GetItemPos(i, item);
            item.SetPosition(pos.x, pos.y + 100, pos.z);
            float time = i * 0.1f;

            item.TweenMoveY(pos.y, (time + 0.1f)).SetEase(EaseType.CubicOut).OnStart(() =>
            {
                item.TweenFade(1, (time + 0.15f)).SetEase(EaseType.QuadOut);
            });
        }
    }


    Vector3 GetItemPos(int index, GObject gObject)
    {
        Vector3 pos = gObject.position;
        if (pos == Vector3.zero)
        {
            pos.x = index * 218;
            pos.y = (index / 3) * 356;
        }
        return pos;
    }

    /// <summary>
    /// 获得单个item上的组件
    /// </summary>
    /// <param name="item"></param>
    void InitGoodsItems(GComponent item)
    {
        goodsIcon = item.GetChild("n26").asLoader;
        goodsName = item.GetChild("n21").asTextField;

        buyTimesGroup = item.GetChild("n28").asGroup;
        buyTimes = item.GetChild("n15").asTextField;

        priceController = item.GetController("c1");
        consumeNum = item.GetChild("n25").asTextField;
        originalNum = item.GetChild("n29").asTextField;
        consumeTypeIcon = item.GetChild("n24").asLoader;

        hotGcom = item.GetChild("n16").asCom;
        hotController = hotGcom.GetController("c1");

        timeLimitObj = item.GetChild("n19");
        timeLimitInfo = item.GetChild("n22").asTextField;
        timeLimitText = item.GetChild("n30").asTextField;
    }

    //初始化限购数量信息
    void InitItemLimitTimeInfo(GameMallConfig gameMall, GGroup buyGroup, GTextField times)
    {
        if (gameMall.limit_num != 0)
        {
            int buyTimes = ShopMallDataMgr.ins.GetLimitMallBoughtNum(gameMall.mall_id);
            buyGroup.visible = true;
            times.text = (gameMall.limit_num - buyTimes).ToString();
            return;
        }
        buyTimesGroup.visible = false;
    }

    void RenderListItem(int index, GObject obj)
    {
        obj.SetPivot(0.5f, 0.5f);
        GComponent itemCom = obj.asCom;
        GameMallConfig gameMall = currentGameMalls[index];

        InitGoodsItems(itemCom);

        int iconId = gameMall.prop_id == 0 ? gameMall.mall_id : gameMall.prop_id;
        goodsIcon.url = UrlUtil.GetItemIconUrl(iconId);
        goodsName.text = gameMall.name;

        //商品的价格信息
        TinyItem tinyItem = ItemUtil.GetTinyItem(gameMall.cost);
        consumeTypeIcon.url = CommonUrlConfig.GetConsumeItemUrl((TypeConfig.Consume)tinyItem.type);
        //折扣
        int discountPrice = 0;
        priceController.selectedIndex = ShopMallDataMgr.ins.isHasDiscount(tinyItem.num, gameMall.discount, out discountPrice) ? 0 : 1;
        originalNum.text = tinyItem.num + "";
        consumeNum.text = discountPrice + "";
        //限购
        InitItemLimitTimeInfo(gameMall, buyTimesGroup, buyTimes);

        //hot /new 切换
        int label_type = gameMall.label_type;
        hotGcom.visible = label_type != 0;
        hotController.selectedIndex = label_type == 1 ? 1 : 0;
        //限时
        bool isLimitTime = ShopMallDataMgr.ins.isLimitTime(gameMall, out TimeSpan span);
        timeLimitObj.visible = isLimitTime;
        timeLimitInfo.visible = isLimitTime;
        timeLimitText.visible = isLimitTime;
        if (isLimitTime)
        {
            timeLimitInfo.text = "";
            CountDown(obj, timeLimitInfo, span);
        }

        itemCom.onClick.Set(() => { OnClickMallItem(gameMall, discountPrice); });
        obj.gameObjectName = "" + index;

    }

    /// <summary>
    ///按钮点击切换
    /// </summary>
    /// <param name="context"></param>
    void OnClickItem(EventContext context)
    {
        selectIndex = topBtnList.GetChildIndex((GObject)context.data);
        currentGameMalls = ShopMallDataMgr.ins.GetCurrentGameMalls(selectIndex);
        if (selectIndex == 0)
        {
            controller.selectedIndex = 4;
            RefreshCardList();
        }
        else
        {
            controller.selectedIndex = 0;
            RefreshGoodsList();

        }
    }

    /// <summary>
    ///单个商品点击获得商品信息及购买
    /// </summary>
    /// <param name="gameMall"></param>
    void OnClickMallItem(GameMallConfig gameMall, int discountPrice)
    {

        if (selectIndex == 1)
        {
            Extrand extrand = new Extrand();
            extrand.key = "" + gameMall.mall_id;
            extrand.msg = "确认消费" + discountPrice + "元，获得" + gameMall.description;
            extrand.callBack = () =>
            {

                TinyItem tiny = ItemUtil.GetTinyItem(gameMall.award);
                UIMgr.Ins.showNextPopupView<CommonGetPropsView, TinyItem>(tiny);

            };
            UIMgr.Ins.showNextPopupView<RechargeTipsView, Extrand>(extrand);
            return;
        }
        UIMgr.Ins.showNextPopupView<BuyMallView, GameMallConfig>(gameMall);
    }

    void CountDown(GObject go, GTextField timeTextField, TimeSpan span)
    {
        if (span.Days > 1)
        {
            timeTextField.text = "限时出售: " + span.Days + "天" + span.Hours + "小时";
            return;
        }
        Action<TimeSpan> onChange = (TimeSpan t) =>
        {
            timeTextField.text = "限时出售: " + string.Format("{0:T}", t);
        };
        Action onOver = CountDownOver;
        int totalSeconds = (int)span.TotalSeconds;
        TimeCountdown timeCountdown = new TimeCountdown();
        timeCountdown.initBase(onChange, onOver, go);
        timeCountdown.countdown(totalSeconds);
    }

    DateTime serverDateTime;

    void CountDownOver()
    {
        ShopMallDataMgr.ins.RefreshGameMallDic();
        RefreshGoodList();
    }


    /// <summary>
    /// 请求限购商品购买次数
    /// </summary>
    void QueryMallInfo(int index = 0)
    {
        WWWForm wWWForm = new WWWForm();
        GameMonoBehaviour.Ins.RequestInfoPostList<PlayerMall>(NetHeaderConfig.MALL_INFO, wWWForm, () =>
        {
            ShopMallDataMgr.ins.RefreshGameMallDic();
            refreshMallsInfo(index);
            WWWForm wWForm = new WWWForm();
            GameMonoBehaviour.Ins.RequestInfoPostHaveData<List<PlayerProp>>(NetHeaderConfig.PLAYER_CARD_INFO, wWForm, (List<PlayerProp> props) =>
            {
                ShopMallDataMgr.ins.cardsInfo = props;
                refreshMallsInfo(index);

            }, false);
        });
    }

    //刷新商品信息
    public void RefreshGoodList()
    {
        refreshMallsInfo(topBtnList.selectedIndex);

    }


}

using System.Collections.Generic;
using FairyGUI;
using UnityEngine;
using System;
using System.Linq;

public class DollUpgradeMoudle : BaseMoudle
{
    public static DollUpgradeMoudle ins;

    GameInitCardsConfig doll;
    GTextField nameText;
    GTextField lvText;

    GLoader bgGLoaderA;
    GLoader bgGLoaderB;

    GLoader attrLoaderA;
    GLoader attrLoaderB;

    GTextField nameAText;
    GTextField nameBText;

    GTextField charmText;
    GTextField wisdomText;

    GTextField consumeText;
    GLoader consumeLoader;
    GLoader bodyLoader;

    Controller upgradeController;
    Controller combineController;
    Controller c2Controller;


    /***********未获得**********/
    GTextField consumeBuyText;
    GLoader consumeBuyLoader;
    GButton roleFragmentBtn;
    GLoader roleFragmentLoader;
    PlayerProp playerProp;

    int cardIndex = 0;
    Controller controller;
    GameCardLevelConfig gameCardLevelConfig;
    GameInitCardsConfig gameInitCardsConfig;

    //娃娃升级相关组件
    Dictionary<int, Controller> redPointDic = new Dictionary<int, Controller>();
    GComponent upgradeCom;
    GTextField numText;
    GLoader costLoader;
    GTextField costNumText;

    GButton storyBtn;
    GButton levelupBtn;

    public override void InitMoudle<T>(GComponent gComponent, int controllerIndex, T data)
    {
        Debug.Log(gComponent.GetController("c1"));
        controller = gComponent.GetController("c1");
        base.InitMoudle(gComponent, controllerIndex, data);
        doll = data as GameInitCardsConfig;
        InitUI();
        InitEvent();
        ins = this;
    }


    public override void InitUI()
    {
        nameText = SearchChild("n20").asTextField;
        lvText = SearchChild("n21").asTextField;
        bodyLoader = SearchChild("n26").asLoader;
        charmText = SearchChild("n17").asTextField;
        wisdomText = SearchChild("n18").asTextField;

        bgGLoaderA = SearchChild("n39").asLoader;
        bgGLoaderB = SearchChild("n40").asLoader;

        attrLoaderA = SearchChild("n13").asLoader;
        attrLoaderB = SearchChild("n14").asLoader;

        nameAText = SearchChild("n15").asTextField;
        nameBText = SearchChild("n16").asTextField;

        levelupBtn = SearchChild("n23").asButton;
        consumeText = levelupBtn.GetChild("n5").asTextField;
        consumeLoader = levelupBtn.GetChild("n24").asLoader;

        //娃娃升级
        upgradeCom = SearchChild("n45").asCom;
        numText = upgradeCom.GetChild("n42").asTextField;

        costLoader = upgradeCom.GetChild("n24").asLoader;
        costNumText = upgradeCom.GetChild("n45").asTextField;
        c2Controller = ui.GetController("c2");
        /***********未获得**********/
        consumeBuyLoader = SearchChild("n32").asLoader;
        consumeBuyText = SearchChild("n33").asTextField;
        //头像碎片
        roleFragmentBtn = SearchChild("n30").asButton;
        roleFragmentLoader = roleFragmentBtn.GetChild("n3").asLoader;
        upgradeController = levelupBtn.GetController("c1");
        combineController = SearchChild("n29").asCom.GetController("c1");

        storyBtn = SearchChild("n10").asButton;

        nameText.displayObject.gameObject.AddComponent<UITweenFade>().SetTweenFade(0.5f);
        lvText.displayObject.gameObject.AddComponent<UITweenFade>().SetTweenFade(0.5f);
        bgGLoaderA.displayObject.gameObject.AddComponent<UITweenFade>().SetTweenFade(0.5f);

        bodyLoader.displayObject.gameObject.AddComponent<UITweenScale>().SetTweenScale(Vector2.zero,0.5f,0.5f);

        GGroup gGroup1 = SearchChild("n46").asGroup;
        GGroup gGroup2 = SearchChild("n47").asGroup;
        attrLoaderA.displayObject.gameObject.AddComponent<UITweenMove>().SetTweenGroup(gGroup2, new Vector2(gGroup2.x, gGroup2.y + 100), 0.5f, 1f);
        attrLoaderB.displayObject.gameObject.AddComponent<UITweenMove>().SetTweenGroup(gGroup1, new Vector2(gGroup1.x, gGroup1.y + 100), 0.5f, 1f);
        storyBtn.displayObject.gameObject.AddComponent<UITweenMove>().SetTweenMove( new Vector2(storyBtn.x, storyBtn.y + 100), 0.5f, 1f);

        levelupBtn.displayObject.gameObject.AddComponent<UITweenMove>().SetTweenMove(new Vector2(levelupBtn.x, levelupBtn.y + 100), 0.5f, 1.5f);
    }


    public override void InitData<D>(D data)
    {
        doll = data as GameInitCardsConfig;

        bgGLoaderA.url = UrlUtil.GetCommonBgUrl("roleshow1");
        bgGLoaderB.url = UrlUtil.GetCommonBgUrl("roleshow2");
        Refresh();
        if (GameData.isOpenGuider)
        {
            StoryCacheMgr.storyCacheMgr.GetStoryGameSave("Newbie", GameGuideConfig.TYPE_DOLL_UPGRADE, (storyGameSave) =>
            {
                if (storyGameSave.IsDefault)
                {
                    GameData.guiderCurrent = GuiderInfoLinked.GetCurrGuiderInfo(GameGuideConfig.TYPE_DOLL_UPGRADE, 1);
                    UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);
                    StoryCacheMgr.storyCacheMgr.SaveProgress("Newbie", "1", GameGuideConfig.TYPE_DOLL_UPGRADE);
                }
            });
        }
    }

    public override void InitEvent()
    {
        //close btn
        SearchChild("n27").onClick.Set(Reback);
        //story
        storyBtn.onClick.Set(() =>
        {
            UIMgr.Ins.showNextPopupView<DollStoryView, GameInitCardsConfig>(doll);
        });
        //suit
        SearchChild("n8").onClick.Set(() =>
        {
            baseView.GoToMoudle<DollSuitMoudle, GameInitCardsConfig>((int)RoleGropView.MoudleType.DollSuits, doll);
        });

        //rightShiftBtn
        SearchChild("n7").onClick.Set(() =>
        {
            if (cardIndex == 0)
                cardIndex = GameData.Dolls.Count;
            cardIndex--;

            doll = GameData.Dolls[cardIndex];
            Refresh();
        });
        //leftShiftBtn
        SearchChild("n6").onClick.Set(() =>
        {
            if (cardIndex == (GameData.Dolls.Count - 1))
                cardIndex = -1;
            cardIndex++;

            doll = GameData.Dolls[cardIndex];
            Refresh();
        });
        /***********未获得**********/
        //buy
        SearchChild("n31").onClick.Set(BuyDoll);
        //合成
        SearchChild("n29").onClick.Set(composeDoll);

        roleFragmentBtn.onClick.Set(() =>
        {
            UIMgr.Ins.showNextPopupView<PropSourcesView, PlayerProp>(playerProp);
        });
        //roleFragmentCom.onClick.Set(() => { UIMgr.Ins.showNextPopupView<PropSourcesView, PlayerProp>(playerProp); });
        //娃娃升级
        InitLongPress();
        SearchChild("n23").onClick.Set(OnUpgradeClick); 
        upgradeCom.GetChild("n35").onClick.Set(() => { UpgradeDoll(); });//确认
        upgradeCom.GetChild("n34").onClick.Set(() => { Debug.Log("取消"); c2Controller.selectedIndex = 0; });//取消
        //加
        upgradeCom.GetChild("n38").onClick.Set(() =>
        {
            OnLongPressAdd();
            isPressAdd = false;
        });
        //减
        upgradeCom.GetChild("n39").onClick.Set(() =>
        {
            OnLongPressReduce();
            isPressReduce = false;
        });
    }

    //刷新角色信息
    void Refresh()
    {
        //Debug.Log("id: " + doll.card_id);
        gameCardLevelConfig = DataUtil.GetCardLevelConfig(doll.card_id, doll.init_level + 1);
        gameInitCardsConfig = DataUtil.GetGameInitCard(doll.card_id);

        nameText.text = doll.name_cn;
        cardIndex = DataUtil.FindDollIndex(doll.card_id);
        int skinId = doll.OwnSkins == null || doll.OwnSkins.Trim().Equals("") ? 0 : doll.skin_id;
        bodyLoader.url = UrlUtil.GetDollSkinIconUrl(doll.card_id, skinId);

        int selectIndex = 0;
        List<TinyItem> tinyItems;

        bool isOwnDoll = DataUtil.IsOwnDoll(doll.card_id);
        lvText.text = isOwnDoll ? doll.init_level.ToString() + "级" : "0级";
        selectIndex = isOwnDoll ? 0 : 1;
        if (!isOwnDoll)
        {
            RequestFragment(doll.prop_id);
            TinyItem tinyItem = ItemUtil.GetTinyItem(gameInitCardsConfig.price);
            if (tinyItem != null)
            {
                consumeBuyText.text = tinyItem.num + "";
                consumeBuyLoader.url = CommonUrlConfig.GetConsumeItemUrl((TypeConfig.Consume)tinyItem.type);
            }
            tinyItems = ItemUtil.GetTinyItemForDollConfig(gameInitCardsConfig);
        }
        else
        {
            tinyItems = ItemUtil.GetTinyItemForDollConfig(doll);
        }
        refreshAttrLoaderInfo(tinyItems);

        //consume 
        if (gameCardLevelConfig != null)
        {
            TinyItem tinyItem = ItemUtil.GetTinyItem(gameCardLevelConfig.consume);
            if (tinyItem != null)
            {
                consumeText.text = tinyItem.num + "";
                consumeLoader.url = CommonUrlConfig.GetConsumeItemUrl((TypeConfig.Consume)tinyItem.type);
            }
        }
        if (controller.selectedIndex != selectIndex)
            controller.selectedIndex = selectIndex;
        combineController.selectedIndex = RedpointMgr.Ins.dollCombineRedpoint.Contains(doll.card_id) ? 1 : 0;
        upgradeController.selectedIndex = RedpointMgr.Ins.dollUpgradeRedpoint.Contains(doll.card_id) ? 1 : 0;

    }

    /// <summary>
    /// 刷新属性信息
    /// </summary>
    /// <param name="tinyItems"></param>

    void refreshAttrLoaderInfo(List<TinyItem> tinyItems)
    {
        if (tinyItems.Count >= 2)
        {
            attrLoaderA.url = tinyItems[0].url;
            nameAText.text = tinyItems[0].name;
            charmText.text = "+" + tinyItems[0].num;

            attrLoaderB.url = tinyItems[1].url;
            nameBText.text = tinyItems[1].name;
            wisdomText.text = "+" + tinyItems[1].num;
        }
        else
        {
            Debug.Log("not enough count " + tinyItems.Count);
            attrLoaderA.url = "";
            nameAText.text = "undefined";
            charmText.text = "-1";

            attrLoaderB.url = "";
            nameBText.text = "undefined";
            wisdomText.text = "-1";
        }

    }
    /*************Net********/


    void BuyDoll()
    {
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("cardId", doll.card_id);
        GameMonoBehaviour.Ins.RequestInfoPost<Player>(NetHeaderConfig.DOLL_BUY, wWWForm, BuyDollSuccess);
    }

    void BuyDollSuccess()
    {
        WWWForm wWWForm = new WWWForm();
        GameMonoBehaviour.Ins.RequestInfoPost<PlayerRedpoint>(NetHeaderConfig.RED_POINT, wWWForm, () =>
        {
            EventMgr.Ins.DispachEvent(EventConfig.REFRESH_ROLEGROUP_RED_POINT);

            Refresh();
        }, false);
        doll = GameData.Dolls.FirstOrDefault(a => a.card_id == doll.card_id);
        Refresh();
    }

    /// <summary>
    /// 合成
    /// </summary>
    void composeDoll()
    {
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("cardId", doll.card_id);
        GameMonoBehaviour.Ins.RequestInfoPost<Player>(NetHeaderConfig.DOLL_COMPOSE, wWWForm, BuyDollSuccess);
    }

    /// <summary>
    /// 请求物品数量
    /// </summary>
    void QueryPlayerPropNum(int propId)
    {
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("propId", propId);
        GameMonoBehaviour.Ins.RequestInfoPost<PlayerProp>(NetHeaderConfig.PLAYER_PROP_INFO, wWWForm, (PlayerProp prop) =>
        {
            roleFragmentBtn.title = prop.prop_count + "";
        });
    }

    public override void Reback()
    {
        baseView.GoToMoudle<RoleInfoMoudle, HolderData>((int)RoleGropView.MoudleType.RoleInfo, null);
    }

    void RequestFragment(int id)
    {
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("propId", id);

        GameMonoBehaviour.Ins.RequestInfoPost<ActorFragmentRespo>(NetHeaderConfig.FRAGMENT, wWWForm, RquestSuccess);
    }

    void RquestSuccess(ActorFragmentRespo respo)
    {
        if (playerProp == null)
            playerProp = new PlayerProp();
        playerProp.prop_id = respo.propId;
        playerProp.prop_count = respo.own;
        roleFragmentBtn.title = respo.own + "/" + respo.need;
        roleFragmentLoader.url = UrlUtil.GetItemIconUrl(respo.propId);
    }

    //娃娃升级
    //需升到的等级
    int upgradeNum;
    //原等级
    int level;
    //花费
    int cost;
    //当前选择的娃娃
    GameInitCardsConfig curDoll;
    Player player
    {
        get { return GameData.Player; }
    }
    void InitLongPress()
    {
        LongPressGesture longPressAdd = new LongPressGesture(upgradeCom.GetChild("n38"));
        longPressAdd.trigger = 0.2f;
        longPressAdd.interval = 0.1f;
        longPressAdd.onAction.Set(OnLongPressAdd);

        LongPressGesture longPressReduce = new LongPressGesture(upgradeCom.GetChild("n39"));
        longPressReduce.trigger = 0.2f;
        longPressReduce.interval = 0.1f;
        longPressReduce.onAction.Set(OnLongPressReduce);
    }

    bool isPressAdd;
    void OnLongPressAdd()
    {

        if (upgradeNum < GameData.Player.level)
        {
            TinyItem tinyItem = ItemUtil.GetTinyItem(DataUtil.GetCardLevelConfig(cardIndex, upgradeNum + 1).consume);
            if (cost + tinyItem.num > player.love)
            {
                UIMgr.Ins.showErrorMsgWindow(MsgException.LOVE_NOT_ENOUGH);
                return;
            }

            upgradeNum++;
            cost += tinyItem.num;
            numText.text = upgradeNum + "";
            if (c2Controller.selectedIndex == 1)
            {
                costNumText.text = cost + "";
            }
        }
        else
        {
            if (!isPressAdd)
            {
                UIMgr.Ins.showErrorMsgWindow(MsgException.LEVEL_CONDITION);
                isPressAdd = true;
            }

        }

    }

    bool isPressReduce;
    void OnLongPressReduce()
    {
        if (upgradeNum > level + 1)
        {
            TinyItem tinyItem = ItemUtil.GetTinyItem(DataUtil.GetCardLevelConfig(cardIndex, upgradeNum).consume);
            cost -= tinyItem.num;
            upgradeNum--;
            numText.text = upgradeNum + "";
            if (c2Controller.selectedIndex == 1)
            {
                costNumText.text = cost + "";
            }
        }
        else
        {
            if (!isPressReduce)
            {
                UIMgr.Ins.showErrorMsgWindow("升级等级不能低于娃娃当前等级！");
                isPressReduce = true;
            }
        }

    }


    public void OnUpgradeClick()//当升级按钮被点击
    {
        curDoll = GameData.Dolls[cardIndex];
        level = curDoll.init_level;
        if (level >= GameData.Player.level)     //娃娃已经升级到最高等级
        {
            UIMgr.Ins.showErrorMsgWindow(MsgException.LEVEL_CONDITION);//LEVEL_CONDITION = "娃娃等级不能超过您的等级哦！";
            return;
        }
        c2Controller.selectedIndex = 1;//显示升级窗口

        numText.text = (level + 1) + "";
        upgradeNum = level + 1;
        string consume = DataUtil.GetCardLevelConfig(curDoll.card_id, upgradeNum).consume;
        TinyItem tinyItem = ItemUtil.GetTinyItem(consume);


        cost = tinyItem.num;
        costLoader.url = CommonUrlConfig.GetConsumeItemUrl((TypeConfig.Consume)tinyItem.type);
        costNumText.text = cost + "";

    }
    public void UpgradeDoll()
    {
        c2Controller.selectedIndex = 0;
        if (doll.init_level < GameData.Player.level)
        {
            WWWForm wWWForm = new WWWForm();
            wWWForm.AddField("cardId", doll.card_id);
            wWWForm.AddField("level", upgradeNum - level);
            GameMonoBehaviour.Ins.RequestInfoPost<Player>(NetHeaderConfig.UPGRADE_DOLL_LEVEL, wWWForm, UpgradeDollSuccess);
        }
        else
        {
            UIMgr.Ins.showErrorMsgWindow(MsgException.LEVEL_CONDITION);
        }
    }

    void UpgradeDollSuccess()
    {
        doll = GameData.Dolls[cardIndex];
        WWWForm wWWForm = new WWWForm();
        GameMonoBehaviour.Ins.RequestInfoPost<PlayerRedpoint>(NetHeaderConfig.RED_POINT, wWWForm, () =>
        {
            EventMgr.Ins.DispachEvent(EventConfig.REFRESH_ROLEGROUP_RED_POINT);

            Refresh();
        }, false);
        Refresh();
    }
}

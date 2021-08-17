using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using Spine.Unity;
using System.Linq;

[ViewAttr("Game/UI/J_Role_Growup", "J_Role_Growup", "Frame_attribute_levelup2")]
public class RoleAttribuiteUpgradeView : BaseView
{


    static RoleAttribuiteUpgradeView ins;
    public static RoleAttribuiteUpgradeView Ins
    {
        get
        {
            return ins;
        }
    }
    List<string> attribuiteList = new List<string>()
    {
        "魅力：",
        "智慧：",
        "环保：",
        "魔法："
    };

    List<string> bookList = new List<string>()
    {
        "魅力之书",
        "智慧之书",
        "环保之书",
        "魔法之书"
    };

    List<string> textrueName = new List<string>()
    {
        "UI_book_meili",
        "UI_book_zhihui",
        "UI_book_huanbai",
        "UI_book_mofa"
    };

    List<Texture> textures = new List<Texture>();

    GLoader bgGLoader;

    /// <summary>
    /// 魅力
    /// </summary>
    GTextField charmText;
    /// <summary>
    /// 环保
    /// </summary>
    GTextField evnText;
    /// <summary>
    /// 智慧
    /// </summary>
    GTextField intellText;
    /// <summary>
    /// 魔法
    /// </summary>
    GTextField manaText;
    /// <summary>
    /// 魅力等级
    /// </summary>
    GTextField charmLevelText;
    /// <summary>
    /// 环保等级
    /// </summary>
    GTextField evnLevelText;
    /// <summary>
    /// 智慧等级
    /// </summary>
    GTextField intellLevelText;
    /// <summary>
    /// 魔法等级
    /// </summary>
    GTextField manaLevelText;

    Dictionary<int, Controller> redPointDic = new Dictionary<int, Controller>();
    GComponent upgradeCom;
    GTextField numText;
    GLoader costLoader;
    GTextField costNumText;


    GTextField loveText;
    GTextField diamondText;


    PlayerAttrLevel playerAttriLevel
    {
        get
        {
            return GameData.Player.attrLevel;
        }
    }

    Player player
    {
        get { return GameData.Player; }
    }

    GoWrapper goWrapper;
    SkeletonAnimation skeletonAnimation;
    GGraph spineGraph;


    GTextField upgradeNumText;
    GTextField upgradeNameText;
    GTextField upgradeBookText;
    GLoader upgradeLoader;

    public override void InitUI()
    {
        bgGLoader = SearchChild("n11").asLoader;

        charmText = SearchChild("n18").asTextField;
        evnText = SearchChild("n20").asTextField;
        intellText = SearchChild("n22").asTextField;
        manaText = SearchChild("n24").asTextField;

        charmLevelText = SearchChild("n17").asTextField;
        evnLevelText = SearchChild("n19").asTextField;
        intellLevelText = SearchChild("n21").asTextField;
        manaLevelText = SearchChild("n23").asTextField;

        controller = ui.GetController("c1");

        upgradeCom = SearchChild("n29").asCom;
        numText = upgradeCom.GetChild("n42").asTextField;
        costLoader = upgradeCom.GetChild("n24").asLoader;
        costNumText = upgradeCom.GetChild("n45").asTextField;

        loveText = SearchChild("n36").asTextField;
        diamondText = SearchChild("n37").asTextField;

        spineGraph = SearchChild("n30").asCom.GetChild("n8").asGraph;
        upgradeNumText = SearchChild("n30").asCom.GetChild("n7").asCom.GetChild("n14").asTextField;
        upgradeNameText = SearchChild("n30").asCom.GetChild("n7").asCom.GetChild("n10").asTextField;
        upgradeBookText = SearchChild("n30").asCom.GetChild("n6").asTextField;
        upgradeLoader = SearchChild("n30").asCom.GetChild("n7").asCom.GetChild("n6").asLoader;
        redPointDic.Add(1, SearchChild("n13").asCom.GetController("c1"));
        redPointDic.Add(0, SearchChild("n14").asCom.GetController("c1"));
        redPointDic.Add(3, SearchChild("n15").asCom.GetController("c1"));
        redPointDic.Add(2, SearchChild("n16").asCom.GetController("c1"));

        SearchChild("n29").asCom.GetChild("n32").asTextField.text = "属性书升级";
        SearchChild("n29").asCom.GetChild("n41").asTextField.text = "选择属性书提升的等级";

        foreach (string name in textrueName)
        {
            Texture texture = Resources.Load(UrlUtil.GetTextureUrl(name)) as Texture;
            textures.Add(texture);
        }

        InitEvent();
        ins = this;
    }

    public override void InitEvent()
    {
        //close btn
        SearchChild("n39").onClick.Set(onHide);
        //魅力
        SearchChild("n14").onClick.Set(() => { Upgrade(1); });
        //智力
        SearchChild("n16").onClick.Set(() => { Upgrade(2); });
        //环境
        SearchChild("n13").onClick.Set(() => { Upgrade(3); });
        //魔法
        SearchChild("n15").onClick.Set(() => { Upgrade(4); });
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
        //确认
        upgradeCom.GetChild("n35").onClick.Set(() => { UpgradeAttribute(type); });
        //取消
        upgradeCom.GetChild("n34").onClick.Set(() => { controller.selectedIndex = 0; });

        InitLongPress();

        SearchChild("n34").onClick.Set(() =>
        {
            UIMgr.Ins.showNextPopupView<BuyStarsView>();
        });
        //click diamondBtn 
        SearchChild("n35").onClick.Set(() =>
        {
            UIMgr.Ins.showNextPopupView<ShopMallView>();
            //RequestAddPlayerRes("1,0,1000");
        });

        //关闭升级成功页面
        SearchChild("n30").asCom.GetChild("n1").onClick.Set(() =>
        {
            controller.selectedIndex = 0;
        });

        //注册事件
        EventMgr.Ins.RegisterEvent<TinyItem>(EventConfig.DOLL_ATTRIBUTE_UPGRADE, ShowUpgradeAttribueInfo);
        EventMgr.Ins.RegisterEvent(EventConfig.REFRESH_ATTRIBUITE_TOP_INFO, InitTopInfo);
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

    public override void InitData()
    {
        base.InitData();
        controller.selectedIndex = 0;
        InitTopInfo();
        Refresh();
        isPressAdd = false;
        isPressReduce = false;
        EventMgr.Ins.RegisterEvent(EventConfig.REFRESH_ATTRIBUITE_RED_POINT, RefreshRedpoint);
        if (GameData.isOpenGuider)
        {
            StoryCacheMgr.storyCacheMgr.GetStoryGameSave("Newbie", GameGuideConfig.TYPE_ATTRIBUTE_UPGRADE, (storyGameSave) =>
            {
                if (storyGameSave.IsDefault)
                {
                    GameData.guiderCurrent = GuiderInfoLinked.GetCurrGuiderInfo(GameGuideConfig.TYPE_ATTRIBUTE_UPGRADE, 1);
                    UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);
                    StoryCacheMgr.storyCacheMgr.SaveProgress("Newbie", "1", GameGuideConfig.TYPE_ATTRIBUTE_UPGRADE);
                }
            });
        }
    }

    void InitTopInfo()
    {
        //love
        loveText.text = player.love + "";
        //money
        diamondText.text = player.diamond + "";
    }

    void Refresh()
    {
        bgGLoader.url = UrlUtil.GetRolegrowupBgUrl("BG_attributelevelup");
        RefreshRedpoint();

        charmLevelText.text = "等级：" + playerAttriLevel.charmLv;
        evnLevelText.text = "等级：" + playerAttriLevel.envLv;
        intellLevelText.text = "等级：" + playerAttriLevel.intellLv;
        manaLevelText.text = "等级：" + playerAttriLevel.manaLv;
        charmText.text = "魅力+" + DataUtil.GetPlayerAttrLevelConfig(playerAttriLevel.charmLv).charm;
        evnText.text = "环保+" + DataUtil.GetPlayerAttrLevelConfig(playerAttriLevel.envLv).evn;
        intellText.text = "智慧+" + DataUtil.GetPlayerAttrLevelConfig(playerAttriLevel.intellLv).intell;
        manaText.text = "魔法+" + DataUtil.GetPlayerAttrLevelConfig(playerAttriLevel.manaLv).mana;
    }

    void RefreshRedpoint()
    {
        for (int i = 0; i < RedpointMgr.Ins.attributeRedpoint.Count; i++)
        {
            redPointDic[i].selectedIndex = RedpointMgr.Ins.attributeRedpoint[i] > 0 ? 1 : 0;
        }
    }

    //需升到的等级
    int upgradeNum;
    //原等级
    int level;
    //类型
    int type;
    //花费
    int cost;
    //旧属性
    int oldAttribuite;
    void Upgrade(int index)
    {
        controller.selectedIndex = 1;
        level = playerAttriLevel.charmLv;
        oldAttribuite = DataUtil.GetPlayerAttrLevelConfig(playerAttriLevel.charmLv).charm;
        string consume = DataUtil.GetPlayerAttrLevelConfig(playerAttriLevel.charmLv + 1).consume;
        switch (index)
        {
            case (int)TypeConfig.CIEM.INTELL:
                consume = DataUtil.GetPlayerAttrLevelConfig(playerAttriLevel.intellLv + 1).consume;
                oldAttribuite = DataUtil.GetPlayerAttrLevelConfig(playerAttriLevel.charmLv).intell;
                level = playerAttriLevel.intellLv;
                break;
            //环保
            case (int)TypeConfig.CIEM.ENV:
                consume = DataUtil.GetPlayerAttrLevelConfig(playerAttriLevel.envLv + 1).consume;
                oldAttribuite = DataUtil.GetPlayerAttrLevelConfig(playerAttriLevel.charmLv).evn;
                level = playerAttriLevel.envLv;
                break;
            //魔法
            case (int)TypeConfig.CIEM.MAGIC:
                consume = DataUtil.GetPlayerAttrLevelConfig(playerAttriLevel.manaLv + 1).consume;
                oldAttribuite = DataUtil.GetPlayerAttrLevelConfig(playerAttriLevel.charmLv).mana;
                level = playerAttriLevel.manaLv;
                break;
        }
        type = index;
        numText.text = (level + 1) + "";
        upgradeNum = level + 1;
        TinyItem tinyItem = ItemUtil.GetTinyItem(consume);
        costLoader.url = CommonUrlConfig.GetConsumeItemUrl((TypeConfig.Consume)tinyItem.type);
        costNumText.text = tinyItem.num + "";
        cost = tinyItem.num;
    }

    void UpgradeAttribute(int index)
    {
        //1:魅力
        //2:智力
        //3:环境
        //4:魔法

        if (upgradeNum <= GameData.Player.level)
        {
            WWWForm wWWForm = new WWWForm();
            wWWForm.AddField("playerId", GameData.playerId);
            wWWForm.AddField("attrs", index);
            wWWForm.AddField("num", upgradeNum - level);
            GameMonoBehaviour.Ins.RequestInfoPost<Player>(NetHeaderConfig.UPGRADE_PLAYER_ATTRIBUTE, wWWForm, UpgradeAttributeCallBack);
        }
        else
        {
            UIMgr.Ins.showErrorMsgWindow(MsgException.LEVEL_ATTRIBUTE);
        }
    }

    void UpgradeAttributeCallBack()
    {
        InitTopInfo();
        WWWForm wWWForm = new WWWForm();
        GameMonoBehaviour.Ins.RequestInfoPost<PlayerRedpoint>(NetHeaderConfig.RED_POINT, wWWForm, () =>
        {
            EventMgr.Ins.DispachEvent(EventConfig.REFRESH_ROLEGROUP_RED_POINT);
            Refresh();
        }, false);
        Refresh();

    }

    void ShowUpgradeAttribueInfo(TinyItem tinyItem)
    {
        StartCoroutine(ShowUpgradeEffect(tinyItem));
    }

    Renderer bookRenderer;
    IEnumerator ShowUpgradeEffect(TinyItem tinyItem)
    {
        controller.selectedIndex = 3;
        FXMgr.CreateEffectWithGGraph(SearchChild("n44").asCom.GetChild("n0").asGraph, new Vector3(610, 791), "UI_shuxingshengji_fazhen", 162, 5f);
        AudioClip audioClip = ResourceUtil.LoadEffect(SoundConfig.CommonEffectId.AttribuevUpgradeFx);
        GRoot.inst.PlayEffectSound(audioClip);

        yield return new WaitForSeconds(3.2f);
        controller.selectedIndex = 2;
        if (goWrapper == null)
            GetFxSpine();
        if (GameData.isGuider && GameData.guiderCurrent.guiderInfo.flow == 3)
        {
            GameData.guiderCurrent = GuiderInfoLinked.GetCurrGuiderInfo(3,5);
            UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);
        }
        upgradeLoader.url = tinyItem.url;
        StartCoroutine(TotalTextAnima(tinyItem.num));
        upgradeNameText.text = attribuiteList[tinyItem.type];
        upgradeBookText.text = "阅读[" + bookList[tinyItem.type] + "]";
        yield return new WaitForSeconds(0.05f);

        bookRenderer.material.mainTexture = textures[tinyItem.type];
    }

    private IEnumerator TotalTextAnima(int totalPower)
    {
        int times = 5;
        int partNum = (totalPower - oldAttribuite) / times;

        int tempTotal = oldAttribuite;
        upgradeNumText.text = "+" + oldAttribuite.ToString();
        for (var i = 0; i < times; i++)
        {
            yield return new WaitForSeconds(0.05f);
            tempTotal += partNum;
            upgradeNumText.text = "+" + tempTotal.ToString();
            if (i == times - 1)
            {
                upgradeNumText.text = "+" + totalPower.ToString();
            }
        }
    }
    void GetFxSpine()
    {
        Object prefab = Resources.Load(UrlUtil.GetFxSpineUrl("piaodai"));
        GameObject go = (GameObject)Instantiate(prefab);

        Component[] components = go.GetComponentsInChildren(typeof(SkeletonAnimation));
        if (components != null && components.Length > 0)
        {
            skeletonAnimation = components[0] as SkeletonAnimation;
        }

        skeletonAnimation.AnimationState.SetAnimation(0, "animation", true);
        if (goWrapper == null)
            goWrapper = new GoWrapper(go);
        goWrapper.wrapTarget = go;
        spineGraph.SetNativeObject(goWrapper);
        spineGraph.displayObject.gameObject.transform.localPosition = new Vector3(218, -996, 0);
        spineGraph.displayObject.gameObject.transform.localScale = new Vector3(100, 100, 100);
        GGraph bookGraph = FXMgr.CreateEffectWithScale(SearchChild("n30").asCom, new Vector2(-228, 18), "UI_bookfx", 162, -1);
        bookRenderer = bookGraph.displayObject.gameObject.transform.Find("UI_bookfx(Clone)").Find("UI_book").GetComponent<Renderer>();
        FXMgr.CreateEffectWithScale(SearchChild("n30").asCom, new Vector2(-228, 24), "UI_framestar", 162, -1);
    }

    public override void onHide()
    {
        base.onHide();
        ui.visible = false;
        gameObject.SetActive(false);
        EventMgr.Ins.RemoveEvent(EventConfig.REFRESH_ATTRIBUITE_RED_POINT);

    }

    bool isPressAdd;
    void OnLongPressAdd()
    {
        if (upgradeNum < GameData.Player.level)
        {
            TinyItem tinyItem = ItemUtil.GetTinyItem(DataUtil.GetPlayerAttrLevelConfig(upgradeNum + 1).consume);
            if (cost + tinyItem.num > player.love)
            {
                UIMgr.Ins.showErrorMsgWindow(MsgException.LOVE_NOT_ENOUGH);
                return;
            }

            upgradeNum++;
            cost += tinyItem.num;
            numText.text = upgradeNum + "";
            if (controller.selectedIndex == 1)
            {
                costNumText.text = cost + "";
            }
        }
        else
        {
            if (!isPressAdd)
            {
                UIMgr.Ins.showErrorMsgWindow(MsgException.LEVEL_ATTRIBUTE);
                isPressAdd = true;
            }

        }

    }

    bool isPressReduce;
    void OnLongPressReduce()
    {
        if (upgradeNum > level + 1)
        {
            TinyItem tinyItem = ItemUtil.GetTinyItem(DataUtil.GetPlayerAttrLevelConfig(upgradeNum).consume);
            cost -= tinyItem.num;
            upgradeNum--;
            numText.text = upgradeNum + "";
            if (controller.selectedIndex == 1)
            {
                costNumText.text = cost + "";
            }
        }
        else
        {
            if (!isPressReduce)
            {
                UIMgr.Ins.showErrorMsgWindow("升级等级不能低于属性当前等级！");
                isPressReduce = true;
            }
        }

    }

    /// <summary>
    /// 点击智慧
    /// </summary>
    public void NewbieUpgrade()
    {
        Upgrade(2);
    }

    /// <summary>
    /// 点击确认
    /// </summary>
    public void NewbieConfim()
    {
        UpgradeAttribute(type);
    }

    /// <summary>
    /// 关闭升级成功
    /// </summary>
    public void NewbieClose()
    {
        controller.selectedIndex = 0;
    }
}

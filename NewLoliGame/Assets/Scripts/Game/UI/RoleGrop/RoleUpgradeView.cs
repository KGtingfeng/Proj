using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using System.Linq;


[ViewAttr("Game/UI/J_Role_Growup", "J_Role_Growup", "Frame_levelup", true)]
public class RoleUpgradeView : BaseView
{

    static RoleUpgradeView ins;
    public static RoleUpgradeView Ins
    {
        get
        {
            return ins;
        }
    }

    GProgressBar progressBar;
    GTextField lvText;
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
    GTextField juniorText;
    GTextField mmiddleText;
    GTextField hightText;

    GObject levelupOne;
    GObject levelupTen;

    List<PlayerProp> props;
    PlayerAttribute playerAttribute
    {
        get
        {
            return GameData.Player.attribute;
        }
    }
    public override void InitUI()
    {
        base.InitUI();
        progressBar = SearchChild("n2").asProgress;
        lvText = SearchChild("n3").asTextField;

        GComponent baseCom = SearchChild("n31").asCom;
        charmText = baseCom.GetChild("n44").asTextField;
        evnText = baseCom.GetChild("n45").asTextField;
        intellText = baseCom.GetChild("n46").asTextField;
        manaText = baseCom.GetChild("n47").asTextField;
        juniorText = SearchChild("n24").asTextField;
        mmiddleText = SearchChild("n25").asTextField;
        hightText = SearchChild("n26").asTextField;

        levelupOne = SearchChild("n28");
        levelupTen = SearchChild("n29");

        progressBar.displayObject.gameObject.AddComponent<UITweenMove>().SetTweenMove(new Vector2(progressBar.x, progressBar.y+50), 0.5f);
        lvText.displayObject.gameObject.AddComponent<UITweenFade>().SetTweenFade(0.5f);

        baseCom.displayObject.gameObject.AddComponent<UITweenMove>().SetTweenMove(new Vector2(baseCom.x, baseCom.y + 50), 0.5f,0.3f);

        GGroup gGroup = SearchChild("n38").asGroup;
        juniorText.displayObject.gameObject.AddComponent<UITweenMove>().SetTweenGroup(gGroup,new Vector2(gGroup.x, gGroup.y + 50), 0.5f, 0.6f);

        levelupOne.displayObject.gameObject.AddComponent<UITweenMove>().SetTweenMove(new Vector2(levelupOne.x, levelupOne.y + 50), 0.5f, 0.9f);
        levelupTen.displayObject.gameObject.AddComponent<UITweenFade>().SetTweenFade(0.5f, 0.9f);

        ins = this;
        InitEvent();

    }


    public override void InitData<D>(D data)
    {
        OnShowAnimation();
        base.InitData(data);
        props = data as List<PlayerProp>;
        GamePropConfig configJunior = JsonConfig.GamePropConfigs.Find(a => a.prop_id == 2010);
        GamePropConfig configMiddle = JsonConfig.GamePropConfigs.Find(a => a.prop_id == 2011);
        GamePropConfig configHight = JsonConfig.GamePropConfigs.Find(a => a.prop_id == 2012);
        
        juniorText.text = "经验+" + configJunior.Used.num;
        mmiddleText.text = "经验+" + configMiddle.Used.num;
        hightText.text = "经验+" + configHight.Used.num;

        //设置默认图片 临时数据
        initExpBtnIcon("n21", 2010);
        initExpBtnIcon("n22", 2011);
        initExpBtnIcon("n23", 2012);
        Refresh();

        if (GameData.isOpenGuider)
        {
            StoryCacheMgr.storyCacheMgr.GetStoryGameSave("Newbie", GameGuideConfig.TYPE_ROLE_UPGRADE, (storyGameSave) =>
            {
                if (storyGameSave.IsDefault)
                {
                    GameData.guiderCurrent = GuiderInfoLinked.GetCurrGuiderInfo(5, 1);
                    UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);
                    StoryCacheMgr.storyCacheMgr.SaveProgress("Newbie", "1", 5);
                }
            });
        }
    }

    public override void InitEvent()
    {
        base.InitEvent();
        //upgrade 1 time
        levelupOne.onClick.Set(() =>
        {
            UpGradePlayerLv(1);

        });

        //upgrade 10 times
        levelupTen.onClick.Set(() =>
        {
            UpGradePlayerLv(10);
        });

        //close
        SearchChild("n35").onClick.Set(OnHideAnimation);


    }

    void initExpBtnIcon(string expGComName, int expIconIndex)
    {
        GButton expCom = SearchChild(expGComName).asButton;
        int num = props.Find(a => a.prop_id == expIconIndex).prop_count;
        expCom.title = num + "";
        GLoader expLoader = expCom.GetChild("n3").asLoader;
        expLoader.url = UrlUtil.GetItemIconUrl(expIconIndex);
        expCom.onClick.Set(() =>
        {
            PlayerProp playerProp = new PlayerProp();
            playerProp.prop_id = expIconIndex;
            playerProp.prop_count = num;
            UIMgr.Ins.showNextPopupView<PropSourcesView, PlayerProp>(playerProp);

        });
    }


    void Refresh()
    {

        lvText.text = GameData.Player.level.ToString() + "级";

        PlayerLevelConfig playerLevelConfig = DataUtil.GetPlayerLevelConfig();
        if (playerLevelConfig != null)
        {
            progressBar.max = playerLevelConfig.out_mid_num;
            progressBar.min = playerLevelConfig.add_mid_num;
            progressBar.value = GameData.Player.exp;

            charmText.text = "+" + playerLevelConfig.charm;
            evnText.text = "+" + playerLevelConfig.evn;
            intellText.text = "+" + playerLevelConfig.intell;
            manaText.text = "+" + playerLevelConfig.mana;
        }
        else
        {
            Debug.Log("not find level config " + GameData.Player.level);
        }
    }

    public void NewbieUpgradeLv()
    {
        UpGradePlayerLv(1);
    }

    public void NewbieOnhide()
    {
        OnHideAnimation();
    }

    void UpGradePlayerLv(int lv)
    {

        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("playerId", GameData.playerId);
        wWWForm.AddField("upLevel", lv);

        GameMonoBehaviour.Ins.RequestInfoPost<Player>(NetHeaderConfig.UPGRADE_PLAYER_LEVEL, wWWForm, UpGradePlayerLvSuccess);

    }


    void UpGradePlayerLvSuccess()
    {
        Refresh();
        WWWForm wWWForm = new WWWForm();
        GameMonoBehaviour.Ins.RequestInfoPost<PlayerRedpoint>(NetHeaderConfig.RED_POINT, wWWForm, () =>
        {
            EventMgr.Ins.DispachEvent(EventConfig.REFRESH_ROLEGROUP_RED_POINT);
        }, false);
        EventMgr.Ins.DispachEvent(EventConfig.REFRESH_ROLE_INFO_MOUDEL);
        Refersh();

    }

    void Refersh()
    {
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("propId", "2010,2011,2012");
        GameMonoBehaviour.Ins.RequestInfoPost<List<PlayerProp>>(NetHeaderConfig.PLAYER_PROP_INFO, wWWForm, RequestProps);

    }

    void RequestProps(List<PlayerProp> playerProps)
    {
        this.props = playerProps;
        initExpBtnIcon("n21", 2010);
        initExpBtnIcon("n22", 2011);
        initExpBtnIcon("n23", 2012);
    }
 
}

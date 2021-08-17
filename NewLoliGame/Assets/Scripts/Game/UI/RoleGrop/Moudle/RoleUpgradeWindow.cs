using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using System.Linq;

[ViewAttr("Game/UI/J_Role_Growup", "J_Role_Growup", "Frame_levelup")]
public class RoleUpgradeWindow : BaseWindow
{

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

    PlayerAttribute playerAttribute
    {
        get
        {
            return GameData.Player.attribute;
        }
    }
    public override void InitUI()
    {
        CreateWindow<RoleUpgradeWindow>();

        progressBar = SearchChild("n2").asProgress;
        lvText = SearchChild("n3").asTextField;

        GComponent baseCom = SearchChild("n31").asCom;
        charmText = baseCom.GetChild("n44").asTextField;
        evnText = baseCom.GetChild("n45").asTextField;
        intellText = baseCom.GetChild("n46").asTextField;
        manaText = baseCom.GetChild("n47").asTextField;

        InitEvent();
        Refresh();
    }


    void initExpBtnIcon(string expGComName, int expIconIndex)
    {
        GComponent expCom = SearchChild(expGComName).asCom;
        GLoader expLoader = expCom.GetChild("n3").asLoader;
        expLoader.SetSize(expCom.width, expCom.height);
        expLoader.url = UrlUtil.GetItemIconUrl(expIconIndex);

    }


    public override void InitData()
    {
        base.InitData();
        //设置默认图片 临时数据
        initExpBtnIcon("n21", 1);
        initExpBtnIcon("n22", 2);
        initExpBtnIcon("n23", 3);
    }


    public override void InitEvent()
    {
        SearchChild("n35").onClick.Set(HideWindow);

        //upgrade 1 time
        SearchChild("n28").onClick.Set(() =>
        {
            UpGradePlayerLv(1);

        });

        //upgrade 10 times
        SearchChild("n29").onClick.Set(() =>
        {
            UpGradePlayerLv(10);
        });


    }




    void Refresh()
    {

        lvText.text = GameData.Player.level.ToString();

        PlayerLevelConfig playerLevelConfig = DataUtil.GetPlayerLevelConfig();
        if (playerLevelConfig != null)
        {

            float max = playerLevelConfig.out_mid_num - playerLevelConfig.add_mid_num;
            float currentValue = GameData.Player.exp - playerLevelConfig.add_mid_num;
            progressBar.max = max;
            progressBar.value = currentValue;
            progressBar.text = currentValue + "/" + max;

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
        EventMgr.Ins.DispachEvent(EventConfig.REFRESH_ROLE_INFO_MOUDEL);


    }

    protected override void OnShown()
    {

    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;


[ViewAttr("Game/UI/T_Common", "T_Common", "Frame_levelup")]
public class LevelUpWindow : BaseWindow
{

    List<PlayerLevelConfig> playerLevelConfigs;
    //商品属性信息
    List<TinyItem> tinyItems;

    GTextField oldLevel;
    GTextField newLevel;

    Transition trans;
    public override void InitUI()
    {
        CreateWindow<LevelUpWindow>();
        oldLevel = SearchChild("n5").asTextField;
        newLevel = SearchChild("n6").asTextField;

        trans = contentPane.GetTransition("t1");
    }
    public override void ShowWindow<D>(D data)
    {
        base.ShowWindow(data);
        playerLevelConfigs = data as List<PlayerLevelConfig>;
    }

    public override void InitData()
    {
        base.InitData();
        if (playerLevelConfigs != null)
        {
            oldLevel.text = playerLevelConfigs[0].level_id.ToString() + "级";
            newLevel.text = playerLevelConfigs[1].level_id.ToString() + "级";
            RefreshTinyItemInfo();
        }
        trans.Play();
        GameMonoBehaviour.Ins.StartCoroutine(ShowEffect());

    }


    void RefreshTinyItemInfo()
    {
        tinyItems = new List<TinyItem>();
        tinyItems.Add(new TinyItem("魅力", CommonUrlConfig.GetCharmUrl(), playerLevelConfigs[1].charm, playerLevelConfigs[0].charm));
        tinyItems.Add(new TinyItem("智慧", CommonUrlConfig.GetWisdomUrl(), playerLevelConfigs[1].intell, playerLevelConfigs[0].intell));
        tinyItems.Add(new TinyItem("环保", CommonUrlConfig.GetEnvUrl(), playerLevelConfigs[1].evn, playerLevelConfigs[0].evn));
        tinyItems.Add(new TinyItem("魔法", CommonUrlConfig.GetMagicUrl(), playerLevelConfigs[1].mana, playerLevelConfigs[0].mana));
        tinyItems[0].voiceId = (int)SoundConfig.CommonEffectId.RoleUpgrade;
    }

    IEnumerator ShowEffect()
    {
        yield return new WaitForSeconds(0.7f);
        AudioClip audioClip = ResourceUtil.LoadEffect(SoundConfig.CommonEffectId.RoleUpgradeFx);
        GRoot.inst.PlayEffectSound(audioClip);
        FXMgr.CreateEffectWithScale(contentPane, new Vector2(605, 750), "UI_gongxishengji1", 1, 3, 0);
        FXMgr.CreateEffectWithScale(contentPane, new Vector2(605, 750), "UI_gongxishengji", 1);
    }

    protected override void DoShowAnimation()
    {
        SmallToBig();
    }

    public override IEnumerator CloseIEnumerator()
    {
        yield return new WaitForSeconds(2.4f);
        DoHideAnimation();
        UIMgr.Ins.showWindow<RoleUpgradeSuccessWindow, List<TinyItem>>(tinyItems);

    }

}

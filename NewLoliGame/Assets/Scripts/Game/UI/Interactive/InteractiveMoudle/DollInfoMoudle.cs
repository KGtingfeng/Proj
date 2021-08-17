using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

public class DollInfoMoudle : BaseMoudle
{

    Controller listController;
    GList dollInfoBtnList;
    GComponent infoComponent;

    GTextField birthdayText;
    GTextField zodiacText;
    GTextField constellationText;
    GTextField ageText;
    GTextField residenceText;
    GTextField characterText;
    GTextField hobbyText;

    GComponent storyCom;
    GTextField storyText;

    /// <summary>
    /// 选中的角色配置信息
    /// </summary>
    //GameInitCardsConfig gameInitCardsConfig {
    //    get {return InteractiveDataMgr.ins.SelectRoleCardConfig; }
    //}

    public override void InitMoudle(GComponent gComponent, int controllerIndex)
    {
        base.InitMoudle(gComponent, controllerIndex);
        InitUI();
        //InitEvent();
    }

    public override void InitUI()
    {
        base.InitUI();
        dollInfoBtnList = SearchChild("n47").asList;
        infoComponent = SearchChild("n45").asCom;
        listController = infoComponent.GetController("c1");

        birthdayText = infoComponent.GetChild("n62").asTextField;
        zodiacText = infoComponent.GetChild("n63").asTextField;
        constellationText = infoComponent.GetChild("n64").asTextField;
        ageText = infoComponent.GetChild("n65").asTextField;
        residenceText = infoComponent.GetChild("n66").asTextField;
        characterText = infoComponent.GetChild("n67").asTextField;
        hobbyText = infoComponent.GetChild("n68").asTextField;
        storyCom = infoComponent.GetChild("n46").asCom;
        storyText = storyCom.GetChild("n46").asTextField;
    }

    public override void InitEvent()
    {
        base.InitEvent();
        dollInfoBtnList.onClickItem.Set(OnClickItem);
    }

    public override void InitData()
    {
        base.InitData();
        dollInfoBtnList.numItems = 1;
        dollInfoBtnList.GetChildAt(0).asButton.title = "个人故事";
        dollInfoBtnList.selectedIndex = 0;
        listController.selectedIndex = 1;

        refreshDollInfos();

    }

    void refreshDollInfos()
    {
        GameInitCardsConfig cardsConfig = JsonConfig.GameInitCardsConfigs.Find(a => a.card_id == InteractiveDataMgr.ins.CurrentPlayerActor.actor_id);
        storyText.text = cardsConfig.story;
    }

    void OnClickItem(EventContext context)
    {
        int itemIndex = dollInfoBtnList.GetChildIndex((GObject)context.data);
        listController.selectedIndex = itemIndex;
    }

}

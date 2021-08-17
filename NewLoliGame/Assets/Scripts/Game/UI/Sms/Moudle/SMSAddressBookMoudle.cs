using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using System.Linq;
using System;

public class SMSAddressBookMoudle : BaseMoudle
{
    public static SMSAddressBookMoudle ins;

    List<Role> ownRoles
    {
        get { return GameData.OwnRoleList; }
    }

    //角色列表
    List<GameInitCardsConfig> allCardsConfigs;

    GList roleList;
    Controller cintactController;

    public override void InitMoudle(GComponent gComponent, int controllerIndex)
    {
        base.InitMoudle(gComponent, controllerIndex);
        InitUI();
        InitEvent();
        ins = this;
    }

    public override void InitUI()
    {
        roleList = SearchChild("n1").asList;
        cintactController = baseView.SearchChild("n6").asList.GetChildAt(2).asButton.GetController("c1");

    }

    public override void InitData()
    {
        allCardsConfigs = GetSortGameInitCards();
        cintactController.selectedIndex = RedpointMgr.Ins.callRedpoint.Count > 0 ? 1 : 0;

        if (allCardsConfigs != null && allCardsConfigs.Count > 0)
        {
            roleList.SetVirtual();
            roleList.itemRenderer = RenderListItem;
            roleList.numItems = allCardsConfigs.Count;
            roleList.ScrollToView(0);
            GameTool.SetListEffectOne(roleList);

            //if (GameData.isOpenGuider)
            //{
            //    StoryCacheMgr.storyCacheMgr.GetStoryGameSave("Newbie", GameGuideConfig.TYPE_ADDRESS, (storyGameSave) =>
            //    {
            //        if (storyGameSave.IsDefault)
            //        {
            //            GameData.guiderCurrent = GuiderInfoLinked.GetCurrGuiderInfo(GameGuideConfig.TYPE_ADDRESS, 1);
            //            UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);
            //        StoryCacheMgr.storyCacheMgr.SaveProgress("Newbie", "1", GameGuideConfig.TYPE_ADDRESS);
            //        }
            //    });
            //}
        }
    }

    public override void InitEvent()
    {
        SearchChild("n15").onClick.Set(() =>
        {
            NormalInfo normalInfo = new NormalInfo();
            normalInfo.index = 0;
            SMSDataMgr.Ins.ComeForm = (int)SMSView.MoudleType.TYPE_ADDRESS_BOOK;
            EventMgr.Ins.DispachEvent(EventConfig.GOTO_PERSONAL_MPMENT, normalInfo);
        });
    }


    void RenderListItem(int index, GObject obj)
    {
        GComponent gComponent = obj.asCom;
        Controller controller = gComponent.GetController("c1");
        Controller redpointController = gComponent.GetController("c2");
        GComponent headIconCom = gComponent.GetChild("n5").asCom.GetChild("n5").asCom;
        GLoader headIcon = headIconCom.GetChild("n5").asLoader;
        headIcon.url = UrlUtil.GetStoryHeadIconUrl(allCardsConfigs[index].card_id);

        GTextField nameText = gComponent.GetChild("n4").asTextField;
        GProgressBar favorBar = gComponent.GetChild("n3").asProgress;

        Role role = ownRoles.Find(a => a.id == allCardsConfigs[index].card_id);
        if (role != null)
        {
            controller.selectedIndex = 0;
            nameText.text = role.name.Trim() == "" ? allCardsConfigs[index].name_cn : role.name;
            SMSDataMgr.Ins.SetProgressInfo(favorBar, gComponent.GetChild("n2").asCom, role);
        }
        else
        {
            controller.selectedIndex = 1;
            favorBar.value = 0;
            nameText.text = allCardsConfigs[index].name_cn;
        }

        gComponent.onClick.Set(() =>
        {
            if (role == null)
                GotoGetRole(allCardsConfigs[index]);
            else
            {
                SMSDataMgr.Ins.Moudle = SMSView.MoudleType.TYPE_ADDRESS_BOOK;

                NormalInfo normalInfo = new NormalInfo();
                normalInfo.index = allCardsConfigs[index].card_id;
                baseView.GoToMoudle<SMSPersonalInfoMoudle, NormalInfo>((int)SMSView.MoudleType.TYPE_PERSONAL_INFO, normalInfo);
            }
        });
        redpointController.selectedIndex = RedpointMgr.Ins.callRedpoint.Contains(allCardsConfigs[index].card_id) ? 1 : 0;
    }

    void GotoGetRole(GameInitCardsConfig gameInitCards)
    {
        SMSView.view.onHide();
        if (gameInitCards.card_id > 27)
        {
            NormalInfo normalInfo = new NormalInfo();
            normalInfo.index = 0;
            UIMgr.Ins.showNextPopupView<RoleGropView, NormalInfo>(normalInfo);
        }
        else
        {
            UIMgr.Ins.showNextPopupView<InteractiveView>();
        }

    }

    public void NewbieGotoRole()
    {
        SMSDataMgr.Ins.Moudle = SMSView.MoudleType.TYPE_ADDRESS_BOOK;

        NormalInfo normalInfo = new NormalInfo();
        normalInfo.index = GameGuideConfig.GuideActor;
        baseView.GoToMoudle<SMSPersonalInfoMoudle, NormalInfo>((int)SMSView.MoudleType.TYPE_PERSONAL_INFO, normalInfo);
    }

    List<GameInitCardsConfig> GetSortGameInitCards()
    {

        List<GameInitCardsConfig> allGameInitCards = GetTmpGameInitCardsConfig();
        //拥有的
        List<GameInitCardsConfig> currentGameInitCards = new List<GameInitCardsConfig>();

        GameInitCardsConfig cardsConfig = null;
        foreach (var ownRole in ownRoles)
        {
            cardsConfig = allGameInitCards.Find(a => a.card_id == ownRole.id);
            if (cardsConfig != null)
            {
                currentGameInitCards.Add(cardsConfig);
                allGameInitCards.Remove(cardsConfig);
            }
        }
        currentGameInitCards.Sort(delegate (GameInitCardsConfig roleA, GameInitCardsConfig roleB)
        {
            return roleA.card_id.CompareTo(roleB.card_id);
        });
        allGameInitCards.Sort(delegate (GameInitCardsConfig roleA, GameInitCardsConfig roleB)
        {
            return roleA.card_id.CompareTo(roleB.card_id);
        });
        currentGameInitCards.AddRange(allGameInitCards);

        return currentGameInitCards;
    }

    /// <summary>
    /// 仅展示有头像的
    /// </summary>
    public List<GameInitCardsConfig> GetTmpGameInitCardsConfig()
    {
        List<GameInitCardsConfig> allGameInitCards = new List<GameInitCardsConfig>();
        foreach (var item in DataUtil.GetGameInitCardsConfigs())
        {
            if (item.type == 0 && item.type != 1)
                continue;
            allGameInitCards.Add(item);
        }
        return allGameInitCards;

    }


}

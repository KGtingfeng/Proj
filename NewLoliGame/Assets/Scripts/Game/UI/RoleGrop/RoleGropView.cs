using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

[ViewAttr("Game/UI/J_Role_Growup", "J_Role_Growup", "Role", true)]
public class RoleGropView : BaseView
{
    public static RoleGropView ins;
    public enum MoudleType
    {
        RoleInfo = 0,
        RoleUpgrade,
        ChangeTitle,//更换称号

        DollUpgrade = 4, //娃娃升级
        DollSuits, //选择时装
        //Doll_NotHave,
        Doll_Story,
        Doll_Upgrade_Success,
        AttributeUpgrade = 9,//属性升级
    };
    GLoader titleLoader;
    GGraph titleGraph;
    GTextField titleText;
    GLoader bgLoader;
    GLoader frameLoader;
    GGraph frameGraph;
    GComponent topCom;
    Dictionary<MoudleType, string> moudleNamePairs = new Dictionary<MoudleType, string>()
    {
        {MoudleType.RoleInfo,"n0"},
        {MoudleType.RoleUpgrade,"n1"}, //window
        {MoudleType.ChangeTitle,"n3"},
        {MoudleType.AttributeUpgrade,"n11"},
        {MoudleType.DollUpgrade,"n5"},
        {MoudleType.DollSuits,"n6"},
        //{MoudleType.Doll_NotHave,"n7"},
        {MoudleType.Doll_Story,"n8"}, //window
    };



    public override void InitUI()
    {
        base.InitUI();
        ins = this;
        controller = ui.GetController("c1");
        ui.opaque = false;
        titleLoader = SearchChild("n0").asCom.GetChild("n41").asCom.GetChild("n39").asLoader;
        titleGraph = SearchChild("n0").asCom.GetChild("n41").asCom.GetChild("n40").asGraph;
        titleText = SearchChild("n0").asCom.GetChild("n9").asTextField;
        frameLoader = SearchChild("n0").asCom.GetChild("n4").asCom.GetChild("n18").asLoader;
        frameGraph = SearchChild("n0").asCom.GetChild("n4").asCom.GetChild("n19").asGraph;
       
        bgLoader = SearchChild("n12").asLoader;
        topCom = SearchChild("n14").asCom;
        InitEvent();
    }

    public override void InitData()
    {
        //OnShowAnimation();
        //GoToMoudle<RoleInfoMoudle, HolderData>((int)MoudleType.RoleInfo, null);
    }

    NormalInfo normalInfo;
    public override void InitData<D>(D data)
    {
        OnShowAnimation();
        base.InitData(data);
        bgLoader.url = UrlUtil.GetCommonBgUrl("role_BG");
        normalInfo = data as NormalInfo;
        GetTopText();
        if (normalInfo != null)
        {
            switch ((MoudleType)normalInfo.index)
            {
                case MoudleType.RoleInfo:
                    GoToMoudle<RoleInfoMoudle, NormalInfo>((int)MoudleType.RoleInfo, null);
                    break;
                case MoudleType.AttributeUpgrade:
                    GoToMoudle<RoleInfoMoudle, NormalInfo>((int)MoudleType.RoleInfo, normalInfo);
                    UIMgr.Ins.showNextPopupView<RoleAttribuiteUpgradeView>();

                    break;
                case MoudleType.DollUpgrade:
                    GameInitCardsConfig doll = null;
                    foreach (GameInitCardsConfig card in GameData.Dolls)
                    {
                        if (DataUtil.IsOwnDoll(card.card_id))
                        {
                            doll = card;
                            break;
                        }
                    }
                    GoToMoudle<DollUpgradeMoudle, GameInitCardsConfig>((int)MoudleType.DollUpgrade, doll);
                    break;
                case MoudleType.DollSuits:
                    GameInitCardsConfig gameDoll = null;
                    foreach (GameInitCardsConfig card in GameData.Dolls)
                    {
                        if (DataUtil.IsOwnDoll(card.card_id))
                        {
                            gameDoll = card;
                            break;
                        }
                    }
                    GoToMoudle<DollSuitMoudle, GameInitCardsConfig>((int)MoudleType.DollSuits, gameDoll);

                    break;
                default:

                    break;

            }

        }

        if (GameData.Player.title.current != 0)
        {
            GameTitleConfig titleConfig = JsonConfig.GameTitleConfigs.Find(a => a.id == GameData.Player.title.current);
            titleText.text = titleConfig.name_cn;
            FXMgr.SetTitle(titleGraph, titleLoader, titleConfig.level, 80, new Vector3(119, 19, 1000));
        }
        else
        {
            titleText.text = "暂无";
            FXMgr.SetTitle(titleGraph, titleLoader, 1, 80, new Vector3(119, 19, 1000));
        }
        if (GameData.Player.avatar_frame.current != 0)
        {
            GameAvatarFrameConfig frameConfig = JsonConfig.GameAvatarFrameConfigs.Find(a => a.id == GameData.Player.avatar_frame.current);
            FXMgr.SetFrame(frameGraph, frameLoader, frameConfig.source_id, 100, new Vector3(135, 136, 1000));
        }
        else
        {
            frameGraph.visible = false;
            frameLoader.visible = false;
        }
    }


    public override void InitEvent()
    {
        //注册事件
        //EventMgr.Ins.RegisterEvent<List<TinyItem>>(EventConfig.PLAYER_UPGRADE_LEVEL, ShowUpgradeLvInfo);
        EventMgr.Ins.RegisterEvent<List<PlayerLevelConfig>>(EventConfig.PLAYER_UPGRADE_LEVEL, ShowUpgradeLvInfo);
        EventMgr.Ins.RegisterEvent<List<TinyItem>>(EventConfig.DOLL_UPGRADE_LEVEL, ShowDollUpgradeEffect);
        EventMgr.Ins.RegisterEvent(EventConfig.REFRESH_ROLE_TOP_INFO, GetTopText);
        //click loveBtn星星
        topCom.GetChild("n13").onClick.Set(() =>
        {
            UIMgr.Ins.showNextPopupView<BuyStarsView>();
        });
        //click diamondBtn 钻石
        topCom.GetChild("n14").onClick.Set(() =>
        {
            UIMgr.Ins.showNextPopupView<ShopMallView>();
        });
        //close btn
        SearchChild("n13").onClick.Set(() =>
        {
            UIMgr.Ins.showNextView<MainView>();
        });
    }
    void GetTopText()
    {
        topCom.GetChild("n15").asTextField.text = GameData.Player.love + "";
        topCom.GetChild("n16").asTextField.text = GameData.Player.diamond + "";
    }

    //void ShowUpgradeLvInfo(List<TinyItem> tinyItems)
    //{
    //    UIMgr.Ins.showWindow<RoleUpgradeSuccessWindow, List<TinyItem>>(tinyItems);
    //}

    void ShowUpgradeLvInfo(List<PlayerLevelConfig> playerLevelConfigs)
    {
        if (playerLevelConfigs.Count < 2)
            return;
        UIMgr.Ins.showWindow<LevelUpWindow, List<PlayerLevelConfig>>(playerLevelConfigs);
    }

    void ShowDollUpgradeEffect(List<TinyItem> tinyItems)
    {
        StartCoroutine(StartEffect(tinyItems));
    }



    IEnumerator StartEffect(List<TinyItem> tinyItems)
    {
        GComponent gCom = SearchChild("n5").asCom;
        yield return 0;
        AudioClip audioClip = ResourceUtil.LoadEffect(SoundConfig.CommonEffectId.DollUpgradeFx);
        GRoot.inst.PlayEffectSound(audioClip);
        FXMgr.CreateEffectWithScale(gCom, new Vector2(596, 1435), "Levelbutton", 1);

        yield return new WaitForSeconds(0.2f);

        FXMgr.CreateEffectWithScale(gCom, new Vector2(230, 15), "Levelupwithmask", 162, 3, gCom.GetChildIndex(gCom.GetChild("n38")));
        FXMgr.CreateEffectWithScale(gCom, new Vector2(596, 315), "namelight", 1);

        yield return new WaitForSeconds(0.1f);
        FXMgr.CreateEffectWithScale(gCom, new Vector2(230, 15), "Levelupwithmask2", 162);

        yield return new WaitForSeconds(0.5f);
        FXMgr.CreateEffectWithScale(gCom, new Vector2(751, 1302), "Attributeframe", 1);
        FXMgr.CreateEffectWithScale(gCom, new Vector2(457, 1302), "Attributeframe", 1);
        yield return new WaitForSeconds(2f);
        UIMgr.Ins.showWindow<RoleUpgradeSuccessWindow, List<TinyItem>>(tinyItems);
    }


    public override void GoToMoudle<T, D>(int index, D data)
    {
        MoudleType moudleType = (MoudleType)index;
        BaseMoudle baseMoudle = FindMoudle<T>();
        if (baseMoudle == null)
        {
            baseMoudle = (BaseMoudle)Activator.CreateInstance(typeof(T));
            baseMoudles.Add(baseMoudle);
            GComponent gComponent = null;

            string componmentName;
            if (moudleNamePairs.TryGetValue(moudleType, out componmentName))
            {
                GObject gObject = ui.GetChild(componmentName);
                if (gObject == null)
                {
                    Debug.Log(componmentName + " not find, please check it");
                    return;
                }
                gComponent = gObject.asCom;
                gComponent.opaque = true;
            }


            baseMoudle.baseView = this;
            baseMoudle.InitMoudle<D>(gComponent, index, data);
        }
        baseMoudle.InitData<D>(data);

        SwitchController(index);
    }

    void GotoAttribuiteMoudle()
    {
        StartCoroutine(GotoAttribuite());
    }

    IEnumerator GotoAttribuite()
    {
        TouchScreenView.Ins.PlayTmpEffect();
        yield return new WaitForSeconds(0.8f);
        //GoToMoudle<RoleAttribuiteUpgradeMoudle, HolderData>((int)MoudleType.AttributeUpgrade, null);
        UIMgr.Ins.showNextPopupView<RoleAttribuiteUpgradeView>();
    }
    public Vector2 GetButtonPos(int index)
    {
        return SearchChild("n" + index).asCom.position;
    }

}

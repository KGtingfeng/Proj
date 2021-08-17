using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

[ViewAttr("Game/UI/H_Interactive", "H_Interactive", "Frame_buybg", true)]
public class BuyBackgroundView : BaseView
{

    GComponent gComponent;
    GTextField nameText;
    GTextField costText;
    GLoader costLoader;

    GLoader bgLoader;

    GButton debrisBtn;
    GLoader debrisIcon;

    ActorFragmentRespo fragmentRespo;
    GameActorSkinConfig skinConfig;

    Controller redpoint;

    NormalInfo normalInfo;
    public override void InitUI()
    {
        base.InitUI();
        gComponent = SearchChild("n1").asCom;
        bgLoader = gComponent.GetChild("n3").asCom.GetChild("n0").asLoader;

        nameText = gComponent.GetChild("n9").asTextField;
        costText = gComponent.GetChild("n7").asTextField;
        costLoader = gComponent.GetChild("n24").asLoader;
        debrisBtn = gComponent.GetChild("n4").asButton;
        debrisIcon = gComponent.GetChild("n4").asCom.GetChild("n3").asLoader;

        normalInfo = new NormalInfo();
        normalInfo.index = (int)SoundConfig.CommonEffectId.AttribuevUpgrade;
        redpoint = gComponent.GetChild("n5").asCom.GetController("c1");

        InitEvent();
    }

    public override void InitEvent()
    {
        base.InitEvent();
        //合成
        gComponent.GetChild("n5").onClick.Set(ComposeBackground);
        //购买
        gComponent.GetChild("n6").onClick.Set(BuyBackground);

        SearchChild("n0").onClick.Set(OnHideAnimation);

        debrisBtn.onClick.Set(() =>
        {
            PlayerProp playerProp = new PlayerProp();
            playerProp.prop_id = fragmentRespo.propId;
            playerProp.prop_count = fragmentRespo.own;
            UIMgr.Ins.showNextPopupView<PropSourcesView, PlayerProp>(playerProp);
        });

    }

    public override void InitData<D>(D data)
    {
        OnShowAnimation();
        base.InitData(data);
        fragmentRespo = data as ActorFragmentRespo;
        skinConfig = JsonConfig.GameActorSkinConfigs.Find(a => a.id == InteractiveDataMgr.ins.BackgroundId);
        nameText.text = skinConfig.name_cn;
        bgLoader.url = UrlUtil.GetSkinBgUrl(skinConfig.id);
        TinyItem tinyItem = ItemUtil.GetTinyItem(skinConfig.price);
        costText.text = tinyItem.num + "";
        costLoader.url = CommonUrlConfig.GetConsumeItemUrl((TypeConfig.Consume)tinyItem.type);
        GamePropFragmentConfig fragmentConfig = JsonConfig.GamePropFragmentConfigs.Find(a => a.prop_id == skinConfig.id);

        debrisBtn.title = fragmentRespo.own + "/" + fragmentRespo.need;
        debrisIcon.url = UrlUtil.GetItemIconUrl(fragmentRespo.propId);
        redpoint.selectedIndex = RedpointMgr.Ins.backgroundRedpoint.Contains(fragmentRespo.propId) ? 1 : 0;
    }

    public override void OnShowAnimation()
    {
        ui.visible = true;
        ui.displayObject.gameObject.SetActive(true);
        base.OnShowAnimation();
    }

    Extrand extrand;
    void BuyBackground()
    {
        if (extrand == null)
        {
            extrand = new Extrand();
            extrand.key = "提示";
        }
        extrand.msg = "确认购买该背景？";
        extrand.callBack = RequestBackgroundBuy;
        UIMgr.Ins.showNextPopupView<PopupDialogView, Extrand>(extrand);
    }

    void RequestBackgroundBuy()
    {
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("actorId", InteractiveDataMgr.ins.CurrentPlayerActor.actor_id);
        wWWForm.AddField("backgroundId", skinConfig.id);
        GameMonoBehaviour.Ins.RequestInfoPost<ActorProperty>(NetHeaderConfig.ACTOR_BACKGROUND_BUY, wWWForm, RequestSkinBuySuccess);
    }

    void RequestSkinBuySuccess()
    {
        EventMgr.Ins.DispachEvent(EventConfig.SHOW_BUY_SKIN_EFFECT);
        Refresh();
    }

    void ComposeBackground()
    {
        if (extrand == null)
        {
            extrand = new Extrand();
            extrand.key = "提示";
        }
        extrand.msg = "确认合成该背景？";
        extrand.callBack = RequestBackgroundCompose;
        UIMgr.Ins.showNextPopupView<PopupDialogView, Extrand>(extrand);
    }

    void RequestBackgroundCompose()
    {
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("actorId", InteractiveDataMgr.ins.CurrentPlayerActor.actor_id);
        wWWForm.AddField("backgroundId", skinConfig.id);
        GameMonoBehaviour.Ins.RequestInfoPost<ActorProperty>(NetHeaderConfig.ACTOR_BACKGROUND_COMPOSE, wWWForm, RequestComposeSuccess);
    }

    void RequestComposeSuccess()
    {
        EventMgr.Ins.DispachEvent(EventConfig.SHOW_BUY_SKIN_EFFECT);
        WWWForm wWWForm = new WWWForm();
        GameMonoBehaviour.Ins.RequestInfoPost<PlayerRedpoint>(NetHeaderConfig.RED_POINT, wWWForm, () =>
        {
            Refresh();
        }, false);
        Refresh();
    }

    void Refresh()
    {
        EventMgr.Ins.DispachEvent(EventConfig.MUSIC_COMMON_EFFECT, normalInfo);

        EventMgr.Ins.DispachEvent(EventConfig.REFRESH_APPEARANCE_LIST);
        EventMgr.Ins.DispachEvent(EventConfig.CHANGE_BACKGROUND, skinConfig.id);
        OnHideAnimation();
    }
}

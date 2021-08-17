using System;
using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using Spine.Unity;
using UnityEngine;

public class GetRoleMoudle : BaseMoudle
{

    GameInitCardsConfig doll;
    GLoader bgLoader;
    GLoader bodyLoader;
    NormalInfo normalInfo = new NormalInfo();
    GComponent fxCom;
    GLoader nameLoader;
    //特效部分
    Transition trans;
    public override void InitMoudle<T>(GComponent gComponent, int controllerIndex, T data)
    {
        base.InitMoudle(gComponent, controllerIndex, data);
        doll = data as GameInitCardsConfig;
        InitUI();
        normalInfo.index = (int)SoundConfig.CommonEffectId.GETDOLL;
    }


    public override void InitUI()
    {
        bgLoader = SearchChild("n9").asLoader;
        bodyLoader = SearchChild("n12").asCom.GetChild("n1").asCom.GetChild("n1").asLoader;

        normalInfo.index = (int)SoundConfig.CommonEffectId.GETDOLL;
        trans = ui.GetTransition("t5");
        fxCom = SearchChild("n27").asCom;
        nameLoader = SearchChild("n12").asCom.GetChild("n34").asLoader;
        InitEvent();

    }


    public override void InitData()
    {
        base.InitData();

        bgLoader.url = UrlUtil.GetChooseDollBgUrl("bg");
        //bodyLoader.url = "ui://J_Cha/" + ChooseRoleView.bodyUrl[doll.card_id];
        bodyLoader.url = UrlUtil.GetDollSkinIconUrl(doll.card_id, 0);

        nameLoader.url = UrlUtil.GeTinyItemUrl(doll.card_id);
        EventMgr.Ins.DispachEvent(EventConfig.MUSIC_COMMON_EFFECT, normalInfo);
        PlayerLoadFx();
    }

 


    public override void InitEvent()
    {
        fxCom.onClick.Set(() =>
        {

            //AsyncRequestMgr.asyncRequestMgr.GetRedPointsInfos();

            //WWWForm wWForm = new WWWForm();
            //GameMonoBehaviour.Ins.RequestInfoPost(NetHeaderConfig.CONFIG_GAME_ALL, wWForm,
            //    (List<ChannelSwitchConfig> configs) =>
            //{
            //    GameData.Configs = configs;
            //    ChannelSwitchConfig guid = configs.Find(a => a.key == ChannelSwitchConfig.KEY_GUID);
            //    if (guid != null)
            //    {
            //        if (guid.value == 1)
            //        {
            //            GameData.isOpenGuider = true;
            //        }
            //    }
            //    else
            //    {
            //        GameData.isOpenGuider = false;
            //    }
                if (GameData.isOpenGuider)
                {
                    GameData.isGuider = true;
                    GotoStory();
                }
                else
                {
                    TouchScreenView.Ins.StartCoroutine(GoToEffect());
                }

            //});
             
        });


    }

    IEnumerator GoToEffect()
    {
        TouchScreenView.Ins.PlayTmpEffect();

        yield return new WaitForSeconds(0.8f);
        UIMgr.Ins.showViewWithReleaseOthers<MainView>();

    }

    void GotoStory()
    {
        CallBackList callBackList = new CallBackList();
        callBackList.callBack1 = () => {
            UIMgr.Ins.showViewWithReleaseOthers<MainView>();
        };
        UIMgr.Ins.showNextPopupView<ShanbaiWaittimeView, CallBackList>(callBackList);
    }


    GGraph spine;
    SkeletonAnimation skeleton;
    GGraph fx;
    GGraph fx1;
    void PlayerLoadFx()
    {
        trans.Play();

        if (spine == null)
        {
            spine = new GGraph();
            fxCom.AddChild(spine);
            skeleton = FXMgr.LoadSpineEffect("huode", spine, new Vector2(211, 887), 100);
        }
        skeleton.gameObject.SetActive(true);
        skeleton.AnimationState.SetAnimation(0, "animation", false);
        skeleton.timeScale = 1;
        if (fx == null)
        {
            fx = FXMgr.CreateEffectWithScale(fxCom, new Vector3(463, 0, -4), "UI_huodejuese", 162, -1);
            fx1 = FXMgr.CreateEffectWithScale(fxCom, new Vector3(317, -57, -4), "UI_huode", 162, -1);
        }
        fx.displayObject.gameObject.SetActive(false);
        fx.displayObject.gameObject.SetActive(true);
        fx1.displayObject.gameObject.SetActive(false);
        fx1.displayObject.gameObject.SetActive(true);
    }



}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using Spine.Unity;

[ViewAttr("Game/UI/T_Common", "T_Common", "Choose_doll_final", true)]
public class GetRoleView : BaseView
{

    public static GetRoleView ins;
    GLoader bgLoader;
    GLoader bodyLoader; 

     
    NormalInfo normalInfo = new NormalInfo();
    TinyItem tinyItem;

    //特效部分
    Transition trans; 
    GComponent fxCom;
    GLoader nameLoader;


    public override void InitUI()
    {
        base.InitUI();
        ins = this;
        bgLoader = SearchChild("n9").asLoader;
        bodyLoader = SearchChild("n12").asCom.GetChild("n1").asCom.GetChild("n1").asLoader;
        
        normalInfo.index = (int)SoundConfig.CommonEffectId.GETDOLL;
        trans = ui.GetTransition("t5");
        fxCom = SearchChild("n27").asCom;
        nameLoader = SearchChild("n12").asCom.GetChild("n34").asLoader;
        InitEvent();
    }

    public override void InitData<D>(D data)
    {
        OnShowAnimation();

        tinyItem = data as TinyItem;
        bgLoader.url = UrlUtil.GetChooseDollBgUrl("bg");
        if (tinyItem != null)
        {
            string url = "";
            Debug.Log(tinyItem.id);
            switch (tinyItem.type)
            { 
                case (int)TypeConfig.Consume.Item:
                    url = UrlUtil.GeTinyItemUrl(tinyItem.id);
                    bodyLoader.url = UrlUtil.GetDollSkinIconUrl(tinyItem.id, tinyItem.num);
                    GameInitCardsConfig config = JsonConfig.GameInitCardsConfigs.Find(a => a.card_id == tinyItem.id);
                    break;
                default:
                    url = UrlUtil.GeTinyItemUrl(tinyItem.id);
                    bodyLoader.url = UrlUtil.GetDollSkinIconUrl(tinyItem.id, 0);
                    GameInitCardsConfig config1 = JsonConfig.GameInitCardsConfigs.Find(a => a.card_id == tinyItem.id);
                    break;
            }
            nameLoader.url = UrlUtil.GeTinyItemUrl(tinyItem.id);

        }
        fxCom.onClick.Clear();
        PlaySound();
        PlayerLoadFx();

    }

    public override void InitData()
    {
        base.InitData();
        bgLoader.url = UrlUtil.GetChooseDollBgUrl("BG_choosedoll_final");
        PlayerLoadFx();

    }


    GGraph spine;
    SkeletonAnimation skeleton;
    GameObject fx;
    GGraph fx1;
    void PlayerLoadFx()
    {
        trans.Play(()=> {
            if (GameData.isGuider)
            {
                UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);
            }
            fxCom.onClick.Set(() => {
                if (GameData.isGuider)
                    return;
                OnHideAnimation();
            });
        });
        
        if (spine == null)
        {
            spine = new GGraph();
            fxCom.AddChild(spine);
            skeleton = FXMgr.LoadSpineEffect("huode", spine, new Vector2(211,887), 100);

            fx = skeleton.transform.Find("SkeletonUtility-SkeletonRoot/root/bone/UI_huodejuese").gameObject;

        }
        skeleton.AnimationState.SetAnimation(0, "animation", false);
        skeleton.timeScale = 1;

        if (fx1 == null)
        {
            fx1 = FXMgr.CreateEffectWithScale(fxCom, new Vector3(317, -57, -4), "UI_huode", 162, -1);
        }
        fx.SetActive(false);
        fx.SetActive(true);
        fx1.displayObject.gameObject.SetActive(false);
        fx1.displayObject.gameObject.SetActive(true);
    }

    public override void InitEvent()
    {
        base.InitEvent();
        //share
        SearchChild("n2").onClick.Set(() => { });
        //close
        
    }

    void PlaySound()
    {
        EventMgr.Ins.DispachEvent(EventConfig.MUSIC_COMMON_EFFECT, normalInfo);
    }
 


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using System;

[ViewAttr("Game/UI/H_Interactive", "H_Interactive", "Frame_haogandu")]
public class FavorView : BaseView
{
    GLoader favorDownLoader;
    GTextField favorDownText;
    GTextField favorUpText;
    GLoader favorUpLoader;
    Transition transition;
    FavorItem favor;
    public override void InitUI()
    {
        base.InitUI();
        favorDownLoader = SearchChild("n10").asCom.GetChild("n3").asCom.GetChild("n3").asLoader;
        favorDownText = SearchChild("n10").asCom.GetChild("n7").asTextField;
        favorUpText = SearchChild("n1").asCom.GetChild("n7").asTextField;
        favorUpLoader = SearchChild("n1").asCom.GetChild("n3").asCom.GetChild("n3").asLoader;
        controller = ui.GetController("c1");
        pivot = new Vector2(0.6f, 0.44f);
        transition = ui.GetTransition("t0");
        InitEvent();
    }

    public override void InitData<D>(D data)
    {
        OnShowAnimation();
        base.InitData(data);
        StartCoroutine(CloseView());
        favor = data as FavorItem;

        if (favor.favor > 0)
        {
            controller.selectedIndex = 0;
            favorUpLoader.url = UrlUtil.GetStoryHeadIconUrl(favor.actorId);
            AudioClip audioClip = Resources.Load(SoundConfig.INTERACTIVE_AUDIO_EFFECT_URL + (int)SoundConfig.InteractiveAudioId.GetFavor) as AudioClip;
            GRoot.inst.PlayEffectSound(audioClip);
            favorUpText.text = favor.favor.ToString();
            FXMgr.CreateEffectWithScale(SearchChild("n1").asCom, new Vector3(-231, -397), "UI_haogandu", 162, 3);
        }
        else
        {
            AudioClip audioClip = Resources.Load(SoundConfig.INTERACTIVE_AUDIO_EFFECT_URL + (int)SoundConfig.InteractiveAudioId.FavorDown) as AudioClip;
            GRoot.inst.PlayEffectSound(audioClip);

            controller.selectedIndex = 2;
            favorDownLoader.url = UrlUtil.GetStoryHeadIconUrl(favor.actorId);
            favorDownText.text = favor.favor.ToString();

            FXMgr.CreateEffectWithScale(SearchChild("n10").asCom, new Vector3(-231, -397), "UI_haogandujiang", 162, 3);
        }
        transition.Play();
    }

    IEnumerator CloseView()
    {
        yield return new WaitForSeconds(2f);
        OnHideAnimation();
        if (GameData.isGuider )
        {
            if (GameData.guiderCurrent.guiderInfo.flow == 3)
            { 
                UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);
            } 
        }
    }

}

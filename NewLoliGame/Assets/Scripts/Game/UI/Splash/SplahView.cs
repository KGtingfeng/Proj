using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

[ViewAttr("Game/UI/S_Splash screen", "S_Splash screen", "Splash screen")]
public class SplahView : BaseView
{
    GLoader loader;
    GComponent gCom;

    public override void InitUI()
    {
        base.InitUI();
        controller = ui.GetController("c1");
        gCom = SearchChild("n8").asCom;
        loader = SearchChild("n7").asLoader;
        InitEvent();
    }

    public override void InitData()
    {
        ui.sortingOrder = 50;
        base.InitData();
        loader.url = UrlUtil.GetSplashImageUrl("BG_Splash1");
        StartCoroutine(SetSplashInfo());
    }

    IEnumerator SetSplashInfo()
    {

        loader.TweenFade(0, 2).SetEase(EaseType.QuadIn);
        yield return new WaitForSeconds(2f);
        gCom.alpha = 0;
        controller.selectedIndex = 1;
        gCom.GetChild("n0").asLoader.url = UrlUtil.GetSplashImageUrl("BG_Splash2");
        gCom.TweenFade(1, 2).SetEase(EaseType.QuadOut);
        //loader.TweenFade(1, 2).SetEase(EaseType.QuadOut);
        yield return new WaitForSeconds(2f);
        gCom.TweenFade(0, 1).SetEase(EaseType.Linear);
        TouchScreenView cc = UIMgr.Ins.showNextPopupView<TouchScreenView>() as TouchScreenView;
        yield return new WaitForSeconds(1f);
        //Debug.Log("a");
        //AsyncRequestMgr.asyncRequestMgr.GetGameFunctionOffInfo();
        //Debug.Log("c"); 
        UIMgr.Ins.showNextPopupView<LoadingView>();

    }



}


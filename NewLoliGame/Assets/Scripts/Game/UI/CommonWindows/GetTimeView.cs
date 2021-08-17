using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

[ViewAttr("Game/UI/T_Common", "T_Common", "gain_ime", true)]
public class GetTimeView : BaseView
{
    public static  GetTimeView ins;

    GLoader timeLoader;
    GLoader bgLoader;
    GTextField timeNameText;
    TinyItem tinyItem;
    GGraph fx;
    GComponent fxCom;

    Transition transition;
    public override void InitUI()
    {
        base.InitUI();
        bgLoader = SearchChild("n10").asLoader;
        timeNameText = SearchChild("n8").asTextField;
        transition = ui.GetTransition("t2");
        timeLoader = SearchChild("n0").asCom.GetChild("n0").asLoader;
        fxCom = SearchChild("n13").asCom;
        ins = this;
        bgLoader.url = UrlUtil.GetChooseDollBgUrl("bg");

        InitEvent();
    }

    public override void InitEvent()
    {
        base.InitEvent();
        SearchChild("n1").onClick.Set(OnClick);
    }


    public override void InitData<D>(D data)
    {
        base.InitData(data);
        tinyItem = data as TinyItem;
        GameMomentConfig gameMomentConfig = DataUtil.GetGameMomentConfig(tinyItem.id);

        timeLoader.url = UrlUtil.GetTimeUrl(tinyItem.id);
        Debug.Log(gameMomentConfig.title);
        timeNameText.text = gameMomentConfig.title;

        transition.Play();

        if (fx == null)
        { 
            fx = FXMgr.CreateEffectWithScale(fxCom, new Vector3(317, -57, -4), "UI_huode", 162, -1);
        }
        fx.displayObject.gameObject.SetActive(false);
        fx.displayObject.gameObject.SetActive(true);


        
    }

    void OnClick()
    {
        if (GameData.isGuider)
        {
            GameData.guiderCurrent = GuiderInfoLinked.GetCurrGuiderInfo(1, 7);
            UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);
        }
        else
        {
            onHide();
        }
    }



    public void OnNewbieClose()
    {
        onHide();
        GameData.guiderCurrent = GuiderInfoLinked.GetCurrGuiderInfo(1, 2);
        UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);
    }
}

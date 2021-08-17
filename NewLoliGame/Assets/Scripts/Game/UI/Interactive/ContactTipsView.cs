using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

[ViewAttr("Game/UI/H_Interactive", "H_Interactive", "Frame_contact")]
public class ContactTipsView : BaseView
{

    public static ContactTipsView ins;
    Extrand extrand;
    GameInitCardsConfig gameInitCardsConfig;
    GTextField consumeNum;
    //GTextField msgText;
    public override void InitUI()
    {
        ins = this;
        base.InitUI();
        consumeNum = SearchChild("n8").asTextField;
        //msgText = SearchChild("n3").asTextField;
        InitEvent();
    }

    public override void InitData<D>(D data)
    {
        OnShowAnimation();
        base.InitData(data);
        extrand = data as Extrand;
        gameInitCardsConfig = DataUtil.GetGameInitCard(int.Parse(extrand.key));

        TinyItem tinyItem = ItemUtil.GetTinyItem(gameInitCardsConfig.price);
        consumeNum.text = "" + tinyItem.num;

        //msgText.text = gameInitCardsConfig.gender == 0 ? "是否联系她？" : "是否联系他？";

        if (GameData.isGuider)
        {
            UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);
        }

    }

    public override void InitEvent()
    {
        base.InitEvent();

        SearchChild("n7").onClick.Set(OnHideAnimation);
        SearchChild("n6").onClick.Set(Confirm);
    }


    public void Confirm()
    {
        OnHideAnimation();
        extrand.callBack?.Invoke();
    }
}

using FairyGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ViewAttr("Game/UI/Z_Main", "Z_Main", "Frame_xingxingduihuan", true)]
public class BuyStarsView : BaseView
{
    int num = 1;
    int nextPrice = 0;

    GTextField numTextField;
    GTextField diamondText;
    GTextField lovaText;

    GameMallConfig mallConfig;
    TinyItem consumeItem;
    TinyItem awardItem;
    public override void InitUI()
    {
        numTextField = SearchChild("n12").asTextField;
        diamondText = SearchChild("n16").asTextField;
        lovaText = SearchChild("n17").asTextField;

        InitEvent();
    }

    public override void InitData()
    {
        OnShowAnimation();
        num = 1;
        mallConfig = DataUtil.GetGameMallConfig(1);

        consumeItem = ItemUtil.GetTinyItem(mallConfig.cost);
        awardItem = ItemUtil.GetTinyItem(mallConfig.award);
        numTextField.text = num + "";
        diamondText.text = consumeItem.num + "";
        lovaText.text = awardItem.num + "";
    }


    public override void InitEvent()
    {
        LongClickInfo();
        SearchChild("n15").onClick.Set(QueryBuyStar);
        SearchChild("n0").onClick.Set(OnHideAnimation);
    }

    void LongClickInfo()
    {
        LongPressGesture plusBtn = new LongPressGesture(SearchChild("n9"));
        plusBtn.trigger = 0.01f;
        plusBtn.interval = 0.1f;
        plusBtn.onAction.Set(OnLongPressPlusNumBtn);

        LongPressGesture minusBtn = new LongPressGesture(SearchChild("n10"));
        minusBtn.trigger = 0.01f;
        minusBtn.interval = 0.1f;
        minusBtn.onAction.Set(OnLongPressMinusNumBtn);
    }

    void OnLongPressMinusNumBtn(EventContext context)
    {
        if (num > 1)
        {
            num -= 1;
            numTextField.text = num + "";
        }
    }

    void OnLongPressPlusNumBtn(EventContext context)
    {
        nextPrice = (num + 1) * consumeItem.num;
        if (nextPrice <= GameData.Player.diamond)
        {
            num += 1;
            numTextField.text = num + "";
        }
    }

    void QueryBuyStar()
    {
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("num", num);
        GameMonoBehaviour.Ins.RequestInfoPost<PlayerProperty>(NetHeaderConfig.PLAYER_BUY_STAR, wWWForm, QueryBuyStarCallBack, false);
    }

    void QueryBuyStarCallBack()
    {
        TinyItem tiny = new TinyItem(awardItem.type.ToString(), awardItem.url, num * awardItem.num, awardItem.type);
        UIMgr.Ins.showNextPopupView<CommonGetPropsView, TinyItem>(tiny);
        WWWForm wWWForm = new WWWForm();
        GameMonoBehaviour.Ins.RequestInfoPost<PlayerRedpoint>(NetHeaderConfig.RED_POINT, wWWForm, () =>
        {
            EventMgr.Ins.DispachEvent(EventConfig.REFRESH_ATTRIBUITE_RED_POINT);
            EventMgr.Ins.DispachEvent(EventConfig.REFRESH_MAIN_RED_POINT);

        }, false);

        OnHideAnimation();
        EventMgr.Ins.DispachEvent(EventConfig.BUY_STAR_REFRESH);
    }

}

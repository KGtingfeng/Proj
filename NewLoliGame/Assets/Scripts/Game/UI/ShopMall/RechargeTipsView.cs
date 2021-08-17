using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

[ViewAttr("Game/UI/S_Shop", "S_Shop", "Frame_buytips")]
public class RechargeTipsView : BaseView
{
    //key=roleId  Callback=callback
    Extrand extrand;
    GameMallConfig gameMallConfig;
    GTextField titleText;
    GTextField contentText;

    public override void InitUI()
    {
        base.InitUI();
        titleText = SearchChild("n0").asTextField;
        contentText = SearchChild("n1").asTextField;
        InitEvent();
    }

    public override void InitData<D>(D data)
    {
        OnShowAnimation();
        base.InitData(data);
        extrand = data as Extrand;
        gameMallConfig = DataUtil.GetGameMallConfig(int.Parse(extrand.key));

        titleText.text = gameMallConfig.name;
        contentText.text = extrand.msg;
    }

    public override void InitEvent()
    {
        base.InitEvent();

        SearchChild("n3").onClick.Set(OnHideAnimation);
        SearchChild("n2").onClick.Set(onClickBuy);
    }

    void onClickBuy()
    {
        if (AntiAddictionMgr.Instance.CanBuy(gameMallConfig))
        {
            WWWForm wWWForm = new WWWForm();
            wWWForm.AddField("mallId", gameMallConfig.mall_id);
            wWWForm.AddField("num", 1);
            GameMonoBehaviour.Ins.RequestInfoPost<PropMake>(NetHeaderConfig.MALL_BUY, wWWForm, () =>
            {

                extrand.callBack?.Invoke();
                OnHideAnimation();

            });
        }
    }

}

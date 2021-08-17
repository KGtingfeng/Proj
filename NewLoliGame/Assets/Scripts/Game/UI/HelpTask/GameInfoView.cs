using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

[ViewAttr("Game/UI/B_Help_xinling", "B_Help_xinling", "Frame_makedoll")]
public class GameInfoView : BaseView
{

    GTextField titleText;
    GTextField contentText;

    GComponent propCom1;
    GComponent propCom2;
    GTextField propName1;
    GTextField propName2;

    NormalInfo info;
    GameXinlingConfig config;
    public override void InitUI()
    {
        base.InitUI();
        titleText = SearchChild("n1").asTextField;
        contentText = SearchChild("n2").asTextField;
        propName1 = SearchChild("n6").asTextField;
        propName2 = SearchChild("n7").asTextField;
        propCom1 = SearchChild("n4").asCom;
        propCom2 = SearchChild("n5").asCom;

        InitEvent();
    }

    public override void InitEvent()
    {
        base.InitEvent();
        SearchChild("n8").onClick.Set(OnClickGoto);
        SearchChild("n9").onClick.Set(OnHideAnimation);
    }

    public override void InitData<D>(D data)
    {
        OnShowAnimation();
        base.InitData(data);
        info = data as NormalInfo;
        config = JsonConfig.GameXinlingConfigs.Find(a => a.type == info.index);

        titleText.text = config.title;
        contentText.text = config.description;

        List<TinyItem> tinyItems = ItemUtil.GetTinyItmeList(config.award);
        propCom1.GetChild("n64").asTextField.text = tinyItems[0].num.ToString();
        propCom1.GetChild("n62").asLoader.url = tinyItems[0].url;
        GamePropConfig propConfig1 = JsonConfig.GamePropConfigs.Find(a => a.prop_id == tinyItems[0].id);
        propName1.text = propConfig1.prop_name;

        propCom2.GetChild("n64").asTextField.text = tinyItems[1].num.ToString();
        propCom2.GetChild("n62").asLoader.url = tinyItems[1].url;
        GamePropConfig propConfig2 = JsonConfig.GamePropConfigs.Find(a => a.prop_id == tinyItems[1].id);
        propName2.text = propConfig2.prop_name;

    }

    private void OnClickGoto()
    {
        TouchScreenView.Ins.PlayChangeEffect(GotoHelpView);
    }

    void GotoHelpView()
    {
        switch (info.index)
        {
            case GameXinlingConfig.TYPE_MAKEDOLLS:
                UIMgr.Ins.showNextPopupView<GameMakeDollsView>();

                break;
            case GameXinlingConfig.TYPE_PICKUP:
                UIMgr.Ins.showNextPopupView<GamePickupTrashView>();

                break;
            case GameXinlingConfig.TYPE_CLASSIFICATION:
                UIMgr.Ins.showNextPopupView<GameRubbishClassificationView>();

                break;
            case GameXinlingConfig.TYPE_FIND:
                UIMgr.Ins.showNextPopupView<GameFindDifferenceView>();

                break;
        }
        OnHideAnimation();
    }

}

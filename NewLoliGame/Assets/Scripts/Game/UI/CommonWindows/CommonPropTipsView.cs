using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

[ViewAttr("Game/UI/S_Shop", "S_Shop", "Frame_libaoreview")]
public class CommonPropTipsView : BaseView
{
    GLoader propLoader;
    GTextField propName;
    GTextField propNum;
    //介绍
    GTextField propDes;

    int propId;
    GamePropConfig propConfig;
    PlayerProp playerProp;
    public override void InitUI()
    {
        base.InitUI();
        propLoader = SearchChild("n3").asLoader;
        propName = SearchChild("n5").asTextField;
        propNum = SearchChild("n6").asTextField;
        propDes = SearchChild("n7").asTextField;
        InitEvent();
    }

    public override void InitData<D>(D data)
    {
        OnShowAnimation();
        base.InitData(data);
        propId = int.Parse(data.ToString());
        propConfig = DataUtil.GetGamePropConfig(propId);
        playerProp = ShopMallDataMgr.ins.CurrentPropInfo;
        initItems();
    }

    void initItems()
    {
        if (propConfig != null && playerProp != null)
        {
            TinyItem item = new TinyItem()
            {
                type = playerProp.prop_type,
                id = playerProp.prop_id,
            };
            propLoader.url = UrlUtil.GetPropsIconUrl(item);
            propName.text = propConfig.prop_name;
            propNum.text = "拥有数量：" + playerProp.prop_count;
            propDes.text = propConfig.description;
        }
    }

    public override void InitEvent()
    {
        base.InitEvent();
        SearchChild("n9").onClick.Set(OnHideAnimation);
    }


}

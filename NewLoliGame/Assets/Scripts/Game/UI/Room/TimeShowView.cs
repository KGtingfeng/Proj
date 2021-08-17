using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

[ViewAttr("Game/UI/F_Room", "F_Room", "frame_shikedatu")]
public class TimeShowView : BaseView
{

    GLoader bgLoader;
    GLoader timeLoader;

    GameMomentConfig momentConfig;
    public override void InitUI()
    {
        base.InitUI();
        bgLoader = SearchChild("n1").asLoader;
        timeLoader = SearchChild("n2").asCom.GetChild("n2").asLoader;
        InitEvent();
    }

    public override void InitEvent()
    {
        base.InitEvent();
        SearchChild("n3").onClick.Set(() =>
        {
            OnHideAnimation();
        });
    }

    public override void InitData<D>(D data)
    {
        OnShowAnimation();
        base.InitData(data);

        momentConfig = data as GameMomentConfig;

        bgLoader.url = UrlUtil.GetRoomBgUrl("frame_shikedatu");

        timeLoader.url = UrlUtil.GetTimeUrl(momentConfig.moment_id);

    }


}

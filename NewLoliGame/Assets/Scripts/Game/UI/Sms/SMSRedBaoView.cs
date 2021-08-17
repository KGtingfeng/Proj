using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

[ViewAttr("Game/UI/D_Message", "D_Message", "Frame_redbao")]
public class SMSRedBaoView : BaseView
{
    GLoader iconLoader;
    GTextField comeForm;
    GTextField content;

    GameSmsNodeConfig nodeConfig;
    GameSmsPointConfig gameSmsPoint;
    public override void InitUI()
    {
        base.InitUI();
        iconLoader = SearchChild("n0").asCom.GetChild("n3").asCom.GetChild("n5").asCom.GetChild("n5").asLoader;
        comeForm = SearchChild("n0").asCom.GetChild("title").asTextField;
        content = SearchChild("n0").asCom.GetChild("n4").asTextField;

        InitEvent();
    }

    public override void InitEvent()
    {
        base.InitEvent();
        SearchChild("n0").onClick.Set(OnClickRedBao);
    }

    public override void InitData<D>(D data)
    {
        base.InitData(data);
        nodeConfig = data as GameSmsNodeConfig;
        gameSmsPoint = JsonConfig.GameSmsPointConfigs.Find(a => a.id == nodeConfig.point_id);
        iconLoader.url = UrlUtil.GetStoryHeadIconUrl(int.Parse(gameSmsPoint.title));
        comeForm.text = "来自" + JsonConfig.GameInitCardsConfigs.Find(a => a.card_id == int.Parse(gameSmsPoint.title)).name_cn + "的红包";
        content.text = gameSmsPoint.content1;
    }

    void OnClickRedBao()
    {
        NormalInfo info = new NormalInfo();
        info.index = gameSmsPoint.point1;
        EventMgr.Ins.DispachEvent(EventConfig.SMS_REFRESH_REDBAO, info);
        onHide();
    }

    public override void onHide()
    {
        base.onHide();
        UIMgr.Ins.HideView<SMSRedBaoView>();
    }
}

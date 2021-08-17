using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using FairyGUI;

[ViewAttr("Game/UI/Y_Mail", "Y_Mail", "Frame_neiye")]
public class MailInfoView : BaseView
{

    GTextField content;
    GTextField titleText;
    GTextField timeText;
    GList itemList;
    GButton getBtn;
    MailInfo info;
    List<TinyItem> items = new List<TinyItem>();

    public override void InitUI()
    {
        base.InitUI();
        content = SearchChild("n19").asCom.GetChild("n19").asTextField;
        titleText = SearchChild("n16").asTextField;
        timeText = SearchChild("n17").asTextField;
        controller = ui.GetController("c1");
        itemList = SearchChild("n26").asList;
        getBtn = SearchChild("n23").asButton;
        InitEvent();
    }

    public override void InitEvent()
    {
        base.InitEvent();
        SearchChild("n27").onClick.Set(() =>
        {
            OnHideAnimation();
        });

    }

    public override void InitData<D>(D data)
    {
        OnShowAnimation();
        base.InitData(data);
        info = data as MailInfo;
        if (string.IsNullOrEmpty(info.extraData))
        {
            controller.selectedIndex = 0;
            getBtn.onClick.Set(ReadMail);
            getBtn.title = "确认";
        }
        else
        {
            controller.selectedIndex = 1;
            getBtn.onClick.Set(GetMailAward);
            getBtn.title = "领取";
            RefershList();
        }
        if (info.status == 1)
            getBtn.visible = false;
        else
            getBtn.visible = true;

        content.onClickLink.Set(OnClickLink);
        content.text = info.content;
        titleText.text = info.title;
        DateTime time = TimeUtil.getTime(info.createTime).AddDays(30);
        timeText.text = "到期时间：" + time.Year + "年" + time.Month + "月" + time.Day + "日";
    }

    void RefershList()
    {
        items.Clear();
        string[] item = info.extraData.Split(new char[1] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var str in item)
        {
            TinyItem tinyItem = ItemUtil.GetTinyItem(str);
            items.Add(tinyItem);
        }
        itemList.itemRenderer = RenderItem;
        itemList.numItems = items.Count;
        itemList.onClickItem.Set(OnClickItem);
    }

    void RenderItem(int index, GObject gObject)
    {
        GComponent gComponent = gObject.asCom;
        GTextField numText = gComponent.GetChild("n64").asTextField;
        GLoader itemLoader = gComponent.GetChild("n62").asLoader;
        numText.text = items[index].num.ToString();
        itemLoader.url = UrlUtil.GetPropsIconUrl(items[index]);
    }

    void OnClickItem(EventContext context)
    {
        int index = itemList.GetChildIndex((GObject)context.data);
        //真实index
        int realIndex = itemList.ChildIndexToItemIndex(index);
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("propId", items[realIndex].id);
        GameMonoBehaviour.Ins.RequestInfoPost(NetHeaderConfig.PLAYER_PROP_INFO, wWWForm, (PlayerProp playerProp) =>
        {
            playerProp.prop_type = items[index].type;
            ShopMallDataMgr.ins.CurrentPropInfo = playerProp;
            UIMgr.Ins.showNextPopupView<CommonPropTipsView, int>(items[index].id);

        });
    }

    void OnClickLink(EventContext context)
    {
        string url = (string)context.data;
        Application.OpenURL(url);
    }

    void GetMailAward()
    {
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("mailId", info.mailId);
        GameMonoBehaviour.Ins.RequestInfoPost<TaskAward>(NetHeaderConfig.GET_MAILS_AWARD, wWWForm, RequestMailInfo);
    }

    void RequestMailInfo(TaskAward playerProps)
    {
        GameTool.ShowAwards(playerProps);
        EventMgr.Ins.DispachEvent(EventConfig.GET_MAIL, info.mailId);
        OnHideAnimation();
    }

    void ReadMail()
    {
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("mailId", info.mailId);
        GameMonoBehaviour.Ins.RequestInfoPost<NormalInfo>(NetHeaderConfig.READ_MAIL, wWWForm, RequestReadMail);
    }

    void RequestReadMail()
    {
        EventMgr.Ins.DispachEvent(EventConfig.READ_MAIL, info.mailId);
        OnHideAnimation();
    }

}

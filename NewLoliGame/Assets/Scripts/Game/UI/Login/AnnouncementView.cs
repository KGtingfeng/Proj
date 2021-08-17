using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

[ViewAttr("Game/UI/D_Login", "D_Login", "Frame_announcement",true)]
public class AnnouncementView : BaseView
{

    List<Announcement> announcements;
    GComponent textComponent;
    GTextField titleTextField;
    GList _list;

    public override void InitUI()
    {
        base.InitUI();
        titleTextField = SearchChild("n2").asTextField;
        _list = SearchChild("n4").asList;
        InitEvent();
    }

    public override void InitData<D>(D data)
    {
        OnShowAnimation();
        base.InitData(data);
        announcements = data as List<Announcement>;
        if (announcements != null && announcements.Count > 0)
        {
            titleTextField.text = announcements[0].xtype == 1 ? "公告" : "服务协议";
            InitList();
        }
    }

    public override void InitEvent()
    {
        base.InitEvent();
        SearchChild("n6").onClick.Set(()=> {
            OnHideAnimation();
        });
    }

    void InitList()
    {
        _list.itemRenderer = RenderListItem;
        _list.numItems = announcements.Count;
    }

    void RenderListItem(int index, GObject obj)
    {
        GComponent gComponent = obj.asCom;
        textComponent = gComponent.GetChild("n3").asCom;

        GTextField titleText = textComponent.GetChild("n1").asTextField;
        titleText.text = announcements[index].title;

        GTextField bodyText = textComponent.GetChild("n0").asTextField;
        bodyText.text = announcements[index].body;
        //根据文字长度调整组件高度
        textComponent.height = titleText.height + bodyText.height;
    }
}

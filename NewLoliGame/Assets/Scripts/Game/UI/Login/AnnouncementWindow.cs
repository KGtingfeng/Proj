using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
[ViewAttr("Game/UI/D_Login", "D_Login", "Frame_announcement")]
public class AnnouncementWindow : BaseWindow
{
    List<Announcement> announcements;
    GComponent textComponent;
    GTextField titleTextField;
    GList _list;
    public override void InitUI()
    {
        CreateWindow<AnnouncementWindow>();
        titleTextField = SearchChild("n2").asTextField;
        _list = SearchChild("n4").asList;
        InitEvent();
    }


    public override void ShowWindow<D>(D data)
    {
        base.ShowWindow(data);
        announcements = data as List<Announcement>;
         
    }

    public override void InitData()
    {
        base.InitData();
        if (announcements != null && announcements.Count > 0)
        {
            titleTextField.text = announcements[0].title;
            InitList();
        }
    }

    protected override void OnShown()
    {

    }

    void InitList()
    {
        _list.itemRenderer = RenderListItem;
        _list.numItems = announcements.Count;
    }


    public override void InitEvent()
    {
        base.InitEvent();
        SearchChild("n6").onClick.Set(Hide);
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

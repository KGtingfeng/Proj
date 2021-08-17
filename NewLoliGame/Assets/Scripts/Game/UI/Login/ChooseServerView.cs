using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

[ViewAttr("Game/UI/D_Login", "D_Login", "Frame_chooseservers",true)]
public class ChooseServerView : BaseView
{
    public List<ServerList> serverLists;
    ServerList currentServer;
    GList _list;

    GLoader loader;
    GTextField zoneText;
    GTextField serverName;
    public override void InitUI()
    {
        base.InitUI();
        _list = SearchChild("n17").asList;

        InitEvent();
    }

    public override void InitData<D>(D data)
    {
        OnShowAnimation();
        base.InitData(data);
        serverLists=data as List<ServerList>;

        if (serverLists == null || serverLists.Count <= 0)
        {
            QueryServerList();
            return;
        }
        initListInfos();
    }

    public override void InitEvent()
    {
        base.InitEvent();
        SearchChild("n6").onClick.Set(()=> {
            OnHideAnimation();
        });
    }

   //请求服务器列表
    void QueryServerList()
    {
        WWWForm wWWForm = new WWWForm();
        GameMonoBehaviour.Ins.RequestInfoPostList<ServerList>(NetHeaderConfig.QUERY_SERVICELIST, wWWForm, () =>
        {
            if (SelectServiceMoudle.moudle != null)
            {
                serverLists = SelectServiceMoudle.serverList;
                initListInfos();
            }
        });
    }

    void initListInfos() {
        _list.itemRenderer = RenderListItem;
        _list.numItems = serverLists.Count;
    }

    void RenderListItem(int index, GObject obj) {
        GComponent gComponent = obj.asCom;

        currentServer = serverLists[index];

        loader = gComponent.GetChild("n9").asLoader;
        zoneText= gComponent.GetChild("n10").asTextField;
        serverName= gComponent.GetChild("n11").asTextField;

        loader.url = SelectServiceMoudle.serverTypeUrl[currentServer.id];
        zoneText.text = currentServer.zone;
        serverName.text = currentServer.server_name;
        gComponent.GetChild("n12").visible = currentServer.id == 1;

        gComponent.onClick.Set(()=> {
            SelectServiceMoudle.currentServer = serverLists[index];
            if (SelectServiceMoudle.moudle != null)
                SelectServiceMoudle.moudle.SynchronizeCurrentServer();
            OnHideAnimation();
        });

    }

}

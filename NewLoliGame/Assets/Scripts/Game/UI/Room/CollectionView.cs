using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;


[ViewAttr("Game/UI/F_Room", "F_Room", "Frame_collection")]
public class CollectionView : BaseView
{
    GLoader bgLoader;

    readonly Dictionary<int, GObject> gObjectDic = new Dictionary<int, GObject>();
    readonly Dictionary<int, string> roleDic = new Dictionary<int, string>
    {
        { 28,"n1"},
        { 29,"n2"},
        { 30,"n3"},
        { 31,"n4"},
        { 32,"n5"},
        { 33,"n6"},
        { 34,"n7"},
        { 8,"n8"},
        { 9,"n9"},
        { 10,"n10"},
        { 11,"n11"},
        { 12,"n12"},
        { 13,"n13"},
        { 16,"n16"},
        { 18,"n18"},
        { 19,"n19"},
        { 21,"n21"},
        { 22,"n22"},
        { 23,"n23"},
        { 26,"n26"},

    };

    List<Role> roles;
    public override void InitUI()
    {
        base.InitUI();
        bgLoader = SearchChild("n0").asLoader;

        SetRoleImage();
        InitEvent();
    }

    void SetRoleImage()
    {

        foreach (var role in roleDic)
        {
            GObject gObject = SearchChild(role.Value);
            gObjectDic.Add(role.Key, gObject);
        }
    }

    public override void InitEvent()
    {
        base.InitEvent();

        SearchChild("n28").onClick.Set(() =>
        {
            onHide();
        });
    }

    public override void InitData<D>(D data)
    {
        base.InitData(data);
        roles = data as List<Role>;
        bgLoader.url = UrlUtil.GetRoomBgUrl("04");
        SetRoles();
    }

    void SetRoles()
    {
        foreach (var go in gObjectDic)
        {
            if (roles.Find(a => a.id == go.Key) != null)
                go.Value.visible = true;
            else
                go.Value.visible = false;
        }

    }


}

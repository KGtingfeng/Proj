using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using System;

[ViewAttr("Game/UI/H_Interactive", "H_Interactive", "Frame_sources", true)]
public class PropSourcesView : BaseView
{
    GTextField nameText;
    GTextField numText;
    GTextField desText;
    GLoader iconLoader;
    GList gList;
    PlayerProp playerProp;
    public override void InitUI()
    {
        base.InitUI();
        nameText = SearchChild("n17").asTextField;
        numText = SearchChild("n6").asTextField;
        desText = SearchChild("n7").asTextField;
        iconLoader = SearchChild("n18").asLoader;
        gList = SearchChild("n15").asList;

        InitEvent();
    }

    public override void InitEvent()
    {
        base.InitEvent();
        //取消
        SearchChild("n16").onClick.Set(() => { onDeleteAnimation<PropSourcesView>(); });
    }

    List<int> sources = new List<int>();
    string[] source;
    public override void InitData<D>(D data)
    {
        OnShowAnimation();
        base.InitData(data);
        playerProp = data as PlayerProp;
        GamePropConfig propConfig = JsonConfig.GamePropConfigs.Find(a => a.prop_id == playerProp.prop_id);
        nameText.text = propConfig.prop_name;
        numText.text = "拥有数量：" + playerProp.prop_count;
        desText.text = propConfig.description;
        if (playerProp.prop_type == (int)TypeConfig.Consume.Time)
        {
            iconLoader.url = UrlUtil.GetItemIconUrl(8);
        }
        else
        {
            iconLoader.url = UrlUtil.GetItemIconUrl(playerProp.prop_id);
        }

        sources.Clear();
        source = propConfig.from.Split(new char[1] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        if (source.Length > 0)
        {
            foreach (string s in source)
            {
                string[] str = s.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);//str = "2,*"
                int from = int.Parse(str[0]) * 100;
                if (str.Length > 1)
                {
                    from += int.Parse(str[1]);
                }
                if (str[0] == "2")
                {
                    from = 200;//制作工坊
                }
                sources.Add(from);
            }
        }

        gList.itemRenderer = RenderListItem;
        gList.numItems = sources.Count;
        gList.selectedIndex = 0;
        gList.onClickItem.Set(OnClickItem);
    }

    void RenderListItem(int index, GObject obj)
    {
        GButton item = obj.asButton;
        GameFromConfig fromConfig = JsonConfig.GameFromConfigs.Find(a => a.id == sources[index]);
        if (fromConfig != null)
            item.title = fromConfig.name;
    }

    void OnClickItem(EventContext context)
    {
        int index = gList.GetChildIndex((GObject)context.data);
        //真实index
        int realIndex = gList.ChildIndexToItemIndex(index);
        JumpMgr.Ins.JumpView(source[realIndex]);
        onDeleteAnimation<PropSourcesView>();
    }

    public override void OnShowAnimation()
    {
        ui.visible = true;
        gameObject.SetActive(true);
        base.OnShowAnimation();
    }


}

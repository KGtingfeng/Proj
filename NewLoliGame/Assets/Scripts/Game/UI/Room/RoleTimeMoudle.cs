using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
public class RoleTimeMoudle : BaseMoudle
{

    GList timeList;
    GList btnList;
    GTextField titleText;
    GTextField numText;
    RoleTime roleTime;
    List<GameMomentConfig> currentMoments = new List<GameMomentConfig>();
    List<GameMomentConfig> allMoments = new List<GameMomentConfig>();
    List<GameMomentConfig> ownMoments;
    List<GameMomentConfig> notOwnMoments = new List<GameMomentConfig>();

    public override void InitMoudle(GComponent gComponent, int controllerIndex)
    {
        base.InitMoudle(gComponent, controllerIndex);
        InitUI();
        InitEvent();
    }

    public override void InitUI()
    {
        base.InitUI();

        timeList = SearchChild("n34").asList;
        titleText = SearchChild("n26").asTextField;
        numText = SearchChild("n27").asTextField;
        btnList = SearchChild("n33").asList;
    }

    public override void InitData<D>(D data)
    {
        base.InitData(data);
        roleTime = data as RoleTime;
        InitList();
        btnList.selectedIndex = 0;
        btnList.onClickItem.Set(OnClickBtn);
        timeList.SetVirtual();
        timeList.itemRenderer = RenderItem;
        timeList.onClickItem.Set(OnClickItem);
        GameInitCardsConfig config = JsonConfig.GameInitCardsConfigs.Find(a => a.card_id == roleTime.actorId);
        titleText.text = config.name_cn + "相册";
        numText.text = "已拥有:" + roleTime.configs.Count + "张";

        Refersh(0);
    }

    void Refersh(int index)
    {
        switch (index)
        {
            case 0:
                currentMoments = allMoments;
                break;
            case 1:
                currentMoments = ownMoments;
                break;
            case 2:
                currentMoments = notOwnMoments;
                break;
        }
        if (currentMoments.Count > 0)
            timeList.numItems = currentMoments.Count;
        else
            timeList.numItems = 1;
    }

    void RenderItem(int index, GObject gObject)
    {
        GComponent gComponent = gObject.asCom;
        Controller controller = gComponent.GetController("c1");
        if (currentMoments.Count > 0)
        {
            GTextField nameText = gComponent.GetChild("n17").asTextField;
            if (ownMoments.Find(a => a.moment_id == currentMoments[index].moment_id) == null)
            {
                controller.selectedIndex = 1;
                nameText.text = currentMoments[index].title;
            }
            else
            {
                controller.selectedIndex = 0;
                GLoader gLoader = gComponent.GetChild("n15").asLoader;
                gLoader.url = UrlUtil.GetMomentTime(currentMoments[index].moment_id);
                nameText.text = currentMoments[index].title;
            }
        }
        else
        {
            controller.selectedIndex = 2;
        }
    }

    void InitList()
    {
        List<GameMomentConfig> roleMoments = JsonConfig.GameMomentConfigs.FindAll(a => a.actor_id == roleTime.actorId);
        allMoments.Clear();
        ownMoments = roleTime.configs;
        notOwnMoments.Clear();

        foreach (var moment in roleMoments)
        {
            if (ownMoments.Find(a => a.moment_id == moment.moment_id) == null)
                notOwnMoments.Add(moment);
        }
        allMoments.AddRange(ownMoments);
        allMoments.AddRange(notOwnMoments);

    }

    void OnClickBtn()
    {
        Refersh(btnList.selectedIndex);
    }

    void OnClickItem(EventContext context)
    {
        int index = timeList.GetChildIndex((GObject)context.data);
        //真实index
        int realIndex = timeList.ChildIndexToItemIndex(index);

        if (currentMoments.Count > 0)
        {
            GameMomentConfig momentConfig = ownMoments.Find(a => a.moment_id == currentMoments[realIndex].moment_id);
            if (momentConfig != null)
            {
                UIMgr.Ins.showNextPopupView<TimeShowView, GameMomentConfig>(momentConfig);
            }
            else
            {
                PlayerProp prop = new PlayerProp()
                {
                    prop_id = 8000 + currentMoments[realIndex].moment_id,
                    prop_type = (int)TypeConfig.PropType.TIME,
                    prop_count = 0,
                };
                UIMgr.Ins.showNextPopupView<PropSourcesView, PlayerProp>(prop);

            }

        }
    }
}

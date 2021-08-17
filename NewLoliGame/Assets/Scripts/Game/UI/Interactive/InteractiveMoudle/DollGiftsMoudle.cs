using UnityEngine;
using System.Collections.Generic;
using FairyGUI;

public class DollGiftsMoudle : BaseMoudle
{
    public static DollGiftsMoudle ins;
    GList giftList;
    Controller controller;
    List<PlayerProp> playerProps;
    public override void InitMoudle(GComponent gComponent, int controllerIndex)
    {
        base.InitMoudle(gComponent, controllerIndex);
        InitUI();
        InitEvent();
    }

    public override void InitUI()
    {
        base.InitUI();
        giftList = SearchChild("n61").asList;
        controller = ui.GetController("c1");
        ins = this;
    }

    public override void InitEvent()
    {
        base.InitEvent();
        EventMgr.Ins.RegisterEvent(EventConfig.REFRESH_OWN_GIFT_LIST, Refrsh);
    }

    public override void InitData()
    {
        EventMgr.Ins.RegisterEvent(EventConfig.REFRESH_PROPS, RefreshProps);
        base.InitData();

        giftList.SetVirtual();
        Refrsh();
        giftList.onClickItem.Set(OnClickItem);


        if (GameData.isOpenGuider)
        {
            StoryCacheMgr.storyCacheMgr.GetStoryGameSave("Newbie", GameGuideConfig.TYPE_SEND_GIFT, (storyGameSave) =>
            {
                if (storyGameSave.IsDefault)
                {
                    GameData.guiderCurrent = GuiderInfoLinked.GetCurrGuiderInfo(GameGuideConfig.TYPE_SEND_GIFT, 1);
                    UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);
                    StoryCacheMgr.storyCacheMgr.SaveProgress("Newbie", "1", GameGuideConfig.TYPE_SEND_GIFT);
                }
            });
        }
    }

    void Refrsh()
    {
        playerProps = InteractiveDataMgr.ins.PlayerProps;
        playerProps.Sort(SortProps);
        if (playerProps.Count == 0)
        {
            controller.selectedIndex = 1;
        }
        else
        {
            controller.selectedIndex = 0;
        }

        giftList.itemRenderer = RenderListItem;
        giftList.numItems = playerProps.Count;
        giftList.RefreshVirtualList();
    }

    int SortProps(PlayerProp t1, PlayerProp t2)
    {
        if (t1.prop_count != t2.prop_count)
        {
            return t2.prop_count.CompareTo(t1.prop_count);
        }
        return t1.prop_id.CompareTo(t2.prop_id);
    }

    void RenderListItem(int index, GObject obj)
    {
        GComponent item = obj.asCom;
        item.GetChild("n62").asLoader.url = UrlUtil.GetItemIconUrl(playerProps[index].prop_id);
        item.GetChild("n64").asTextField.text = playerProps[index].prop_count + "";
        GamePropConfig propConfig = JsonConfig.GamePropConfigs.Find(a => a.prop_id == playerProps[index].prop_id);
        item.GetChild("n65").asTextField.text = propConfig.prop_name;
    }

    private void OnClickItem(EventContext context)
    {
        //当前可视item中的index
        int index = giftList.GetChildIndex((GObject)context.data);
        //真实index
        int realIndex = giftList.ChildIndexToItemIndex(index);
        GiftItem item = new GiftItem();
        item.type = GiftItem.SEND_GIFT;
        item.id = playerProps[realIndex].prop_id;
        EventMgr.Ins.DispachEvent(EventConfig.CLOSE_PRESENT_TALK);

        UIMgr.Ins.showNextPopupView<GiftsView, GiftItem>(item);
    }

    void RefreshProps()
    {
        WWWForm wWWForm = new WWWForm();
        GameMonoBehaviour.Ins.RequestInfoPostList<PlayerProp>(NetHeaderConfig.ACTOR_PROP_LIST, wWWForm, RequestRefresh);
    }

    void RequestRefresh()
    {
        Refrsh();
    }

    public void NewbieGift()
    {
        GiftItem item = new GiftItem();
        item.type = GiftItem.SEND_GIFT;
        item.id = 2013;
        EventMgr.Ins.DispachEvent(EventConfig.CLOSE_PRESENT_TALK);

        UIMgr.Ins.showNextPopupView<GiftsView, GiftItem>(item);
    }
}

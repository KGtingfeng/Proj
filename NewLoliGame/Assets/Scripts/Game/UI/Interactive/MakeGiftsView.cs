using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using System;

//制作工坊
[ViewAttr("Game/UI/H_Interactive", "H_Interactive", "Frame_giftslist", true)]
public class MakeGiftsView : BaseView
{
    public static MakeGiftsView ins;
    GList giftList;
    GList selectList;
    GTextField pageText;
    GButton leftBtn;
    GButton rightBtn;

    GLoader bgLoader;
    GTextField loveText;
    GTextField diamondText;

    List<GameMallConfig> tinyItems = new List<GameMallConfig>();
    List<GameMallConfig> canMakeItems = new List<GameMallConfig>();
    List<GameMallConfig> canLearnItems = new List<GameMallConfig>();
    List<GameMallConfig> currentItems = new List<GameMallConfig>();

    int currentPage;
    int totalPage;
    //分类索引
    int currentSelectIndex;
    Player PlayerInfo
    {
        get { return GameData.Player; }
    }
    public override void InitUI()
    {
        base.InitUI();
        controller = ui.GetController("c1");
        giftList = SearchChild("n64").asList;
        selectList = SearchChild("n63").asList;
        pageText = SearchChild("n79").asTextField;
        leftBtn = SearchChild("n78").asButton;
        rightBtn = SearchChild("n77").asButton;
        bgLoader = SearchChild("n89").asLoader;
        loveText = SearchChild("n85").asTextField;
        diamondText = SearchChild("n86").asTextField;

        InitEvent();
        ins = this;
    }

    public override void InitEvent()
    {
        base.InitEvent();
        SearchChild("n83").onClick.Set(() =>
        {
            UIMgr.Ins.showNextPopupView<BuyStarsView>();
        });
        //click diamondBtn 
        SearchChild("n84").onClick.Set(() =>
        {
            UIMgr.Ins.showNextPopupView<ShopMallView>();
        });

        SearchChild("n88").onClick.Set(() =>
        {
            EventMgr.Ins.DispachEvent(EventConfig.REFRESH_TODAY_WISH);
            EventMgr.Ins.RemoveEvent(EventConfig.REFRESH_MAKE_GIFTS);
            UIMgr.Ins.showMainView<MakeGiftsView>();
        });
        //上一页
        leftBtn.onClick.Set(() =>
        {
            if (currentPage > 0)
            {
                currentPage--;
                ChangePage(currentPage);
            }
        });
        //下一页
        rightBtn.onClick.Set(() =>
        {
            if (currentPage < totalPage - 1)
            {
                currentPage++;
                ChangePage(currentPage);
            }
        });

        EventMgr.Ins.RegisterEvent(EventConfig.REFRESH_MAKEGIFTS_TOP_INFO, InitTopInfo);
        EventMgr.Ins.RegisterEvent(EventConfig.REFRESH_ALL_GIFT_LIST, GetList);

    }

    void ChangePage(int page)
    {

        if (currentItems.Count <= 0)
        {
            controller.selectedIndex = 0;
        }
        else
        {
            if ((page + 1) != totalPage)
            {

                controller.selectedIndex = 4;
            }
            else
            {
                int count = currentItems.Count - page * 16;
                Debug.LogError("item " + currentItems.Count + "  count  " + count);
                controller.selectedIndex = (count / 4) + 1;
            }
        }
        leftBtn.visible = page != 0;
        rightBtn.visible = (page + 1) != totalPage;
        giftList.scrollPane.SetCurrentPageX(page, true);

        pageText.text = (page + 1) + "/" + (totalPage);
    }


    public override void InitData()
    {
        OnShowAnimation();

        EventMgr.Ins.RegisterEvent(EventConfig.REFRESH_MAKE_GIFTS, RefreshProps);

        base.InitData();
        bgLoader.url = UrlUtil.GetInteractiveBgUrl(2);
        selectList.selectedIndex = 0;
        selectList.onClickItem.Set(OnClickSelect);
        giftList.scrollPane.touchEffect = false;

        GetList();

        //if (GameData.isOpenGuider)
        //{
        //    WWWForm wWWForm = new WWWForm();
        //    wWWForm.AddField("nodeId", GameGuideConfig.TYPE_MAKEGIFT);
        //    wWWForm.AddField("key", "Newbie");
        //    GameMonoBehaviour.Ins.RequestInfoPostHaveData<List<StoryGameSave>>(NetHeaderConfig.STROY_LOAD_GAME, wWWForm, (List<StoryGameSave> storyGameSaves) =>
        //    {
        //        if (storyGameSaves.Count > 0)
        //        {
        //            string[] save = storyGameSaves[0].value.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        //            GameData.guiderCurrent = GuiderInfoLinked.GetCurrGuiderInfo(int.Parse(save[0]), int.Parse(save[1]));
        //            if (GameData.guiderCurrent != null)
        //            {
        //                string[] roll_to = GameData.guiderCurrent.guiderInfo.roll_to.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        //                if (roll_to.Length < 2 || !GameData.isOpenGuider)
        //                {
        //                    return;
        //                }
        //                if (GameData.guiderCurrent != null)
        //                {
        //                    UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);

        //                }
        //            }
        //        }
        //        else
        //        {
        //            GameData.guiderCurrent = GuiderInfoLinked.GetCurrGuiderInfo(19, 1);
        //            UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);
        //        }

        //    }, false);
        //}
    }

    NormalInfo giftInfo;
    public override void InitData<D>(D data)
    {
        InitData();

        giftInfo = data as NormalInfo;
        if (giftInfo == null) return;

        int giftId = currentItems.FindIndex(a => a.mall_id == giftInfo.index);
        //for (int i = 0; i < currentItems.Count; i++)
        //{
        //    if (currentItems[i].mall_id == giftInfo.index)
        //    {
        //        giftId = i;
        //        break;
        //    }
        //}

        if (canMakeItems.Find(a => a.mall_id == currentItems[giftId].mall_id) != null)
        {
            Debug.Log("传递的from值：" + giftInfo.index + "已学习，礼物ID为：" + giftId);
            OnClickMake(giftId);
        }
        else
        {
            Debug.Log("传递的from值：" + giftInfo.index + "未学习，礼物ID为：" + giftId);
            OnClickLearn(giftId);
        }
    }

    void Refrsh()
    {
        currentPage = 0;
        totalPage = currentItems.Count / 16 + (currentItems.Count % 16 == 0 ? 0 : 1);
        giftList.SetVirtual();
        giftList.itemRenderer = RenderListItem;
        giftList.numItems = currentItems.Count;
        currentSelectIndex = selectList.selectedIndex;
        ChangePage(currentPage);
    }

    void GetList()
    {
        tinyItems.Clear();
        canMakeItems.Clear();
        canLearnItems.Clear();
        List<int> ownId = new List<int>();
        if (!string.IsNullOrEmpty(InteractiveDataMgr.ins.CurrentPlayerActor.unlock_prop))
        {
            string[] ownSkins = InteractiveDataMgr.ins.CurrentPlayerActor.unlock_prop.Split(new Char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string skin in ownSkins)
            {
                ownId.Add(int.Parse(skin));
            }
        }
        List<GameMallConfig> gifts = JsonConfig.GameMallConfigs.FindAll(a => a.mall_id > 500000 && a.mall_id < 600000);

        if (gifts != null)
        {
            for (int i = gifts.Count - 1; i >= 0; i--)
            {
                if (ownId.Contains(gifts[i].mall_id))
                {
                    canMakeItems.Add(gifts[i]);
                    gifts.Remove(gifts[i]);
                }
            }
        }
        else
            return;
        canMakeItems.Sort(Sort);
        gifts.Sort(Sort);
        tinyItems.AddRange(canMakeItems);
        tinyItems.AddRange(gifts);
        canLearnItems = gifts;

        switch (selectList.selectedIndex)
        {
            case 0:
                currentItems = tinyItems;
                break;
            case 1:
                currentItems = canMakeItems;
                break;
            case 2:
                currentItems = canLearnItems;
                break;
        }
        Refrsh();
    }

    private int Sort(GameMallConfig tr1, GameMallConfig tr2)
    {
        return tr1.mall_id.CompareTo(tr2.mall_id);
    }

    void RenderListItem(int index, GObject obj)
    {
        GComponent item = obj.asCom;
        Controller controller = item.GetController("c1");
        TinyItem tinyItem;
        if (canMakeItems.Find(a => a.mall_id == currentItems[index].mall_id) != null)
        {
            tinyItem = ItemUtil.GetTinyItem(currentItems[index].cost);
            controller.selectedIndex = 0;
            item.onClick.Set(() => { OnClickMake(index); });
        }
        else
        {
            tinyItem = ItemUtil.GetTinyItem(currentItems[index].unlock_cost);
            controller.selectedIndex = 1;
            item.onClick.Set(() => { OnClickLearn(index); });
        }
        item.GetChild("n74").asTextField.text = currentItems[index].name;
        item.GetChild("n20").asLoader.url = CommonUrlConfig.GetConsumeItemUrl((TypeConfig.Consume)tinyItem.type);
        item.GetChild("n75").asTextField.text = tinyItem.num + "";
        item.GetChild("n78").asCom.GetChild("n78").asLoader.url = UrlUtil.GetItemIconUrl(currentItems[index].prop_id);
        SetRoleItemEffect(index, obj);
        item.gameObjectName = "" + index;
    }


    void SetRoleItemEffect(int i, GObject obj)
    {
        Vector3 pos = new Vector3(obj.position.x, obj.position.y, obj.position.z);
        int index = i % 16;
        pos.y = (int)(index / 4) * 306f;
        pos.x = (index % 4) * 247f;
        obj.SetPosition(pos.x, pos.y + 200, pos.z);
        float time = (index / 4) * 0.2f;
        //这里有一个特别奇怪的问题，打包后出现，不输出debug时切换页面第一排最后一个会消失，输出debug就不会
        //Debug.Log("index = (" + index + ")     pos =  (" + pos + ")");
        obj.TweenMoveY(pos.y, (time + 0.3f));
        obj.alpha = 1;
    }



    void OnClickSelect()
    {
        if (currentSelectIndex != selectList.selectedIndex)
        {
            switch (selectList.selectedIndex)
            {
                case 0:
                    currentItems = tinyItems;
                    break;
                case 1:
                    currentItems = canMakeItems;
                    break;
                case 2:
                    currentItems = canLearnItems;
                    break;
            }
            Refrsh();
        }
    }


    private void OnClickMake(int index)
    {
        GiftItem item = new GiftItem();
        item.type = GiftItem.MAKE_GIFT;
        item.id = currentItems[index].mall_id;
        UIMgr.Ins.showNextPopupView<GiftsView, GiftItem>(item);
    }

    private void OnClickLearn(int index)
    {
        GiftItem item = new GiftItem();
        item.type = GiftItem.LEARN_GIFT;
        item.id = currentItems[index].mall_id;
        UIMgr.Ins.showNextPopupView<GiftsView, GiftItem>(item);
    }

    public void NewbieLearn()
    {
        OnClickLearn(0);
    }

    public void NewbieMake()
    {
        OnClickMake(0);
    }

    public override void OnShowAnimation()
    {
        base.OnShowAnimation();
        InitTopInfo();
    }

    void InitTopInfo()
    {
        //love
        loveText.text = PlayerInfo.love + "";
        //money
        diamondText.text = PlayerInfo.diamond + "";
    }

    void RefreshProps()
    {
        WWWForm wWWForm = new WWWForm();
        GameMonoBehaviour.Ins.RequestInfoPostList<PlayerProp>(NetHeaderConfig.ACTOR_PROP_LIST, wWWForm, RequestRefresh);
    }

    void RequestRefresh()
    {
        GetList();
    }

}

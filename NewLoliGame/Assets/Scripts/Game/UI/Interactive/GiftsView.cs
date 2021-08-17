using System.Collections;
using UnityEngine;
using FairyGUI;
//送礼、制作、学习
[ViewAttr("Game/UI/H_Interactive", "H_Interactive", "Frame_gifts", true)]
public class GiftsView : BaseView
{
    public static GiftsView ins;
    GTextField nameText;
    GTextField haveNumText;
    GTextField descriptionText;
    GTextField costNumText;
    GLoader costLoader;
    GTextField sendNumText;
    GLoader iconLoager;

    GiftItem item;

    int sendNum;
    int totalNum;
    PlayerProp playerProp;
    GameMallConfig gameMall;
    TinyItem tinyItem;
    NormalInfo normalInfo;
    public static readonly string HAVE_NUM = "拥有数量：";
    public override void InitUI()
    {
        base.InitUI();
        controller = ui.GetController("c1");
        nameText = SearchChild("n8").asTextField;
        haveNumText = SearchChild("n9").asTextField;
        descriptionText = SearchChild("n10").asTextField;
        sendNumText = SearchChild("n16").asTextField;
        iconLoager = SearchChild("n5").asCom.GetChild("n6").asLoader;
        costLoader = SearchChild("n24").asLoader;
        costNumText = SearchChild("n23").asTextField;

        normalInfo = new NormalInfo();
        normalInfo.index = (int)SoundConfig.CommonEffectId.AttribuevUpgrade;

        InitEvent();
        ins = this;
    }

    public override void InitEvent()
    {
        base.InitEvent();
        //确定
        SearchChild("n17").onClick.Set(OnClickComfirm);
        //取消
        SearchChild("n18").onClick.Set(() => { onDeleteAnimation<GiftsView>(); });

        //加
        SearchChild("n12").onClick.Set(() =>
        {
            OnLongPressAdd();
            isPressAdd = false;
        });
        //减
        SearchChild("n13").onClick.Set(() =>
        {
            OnLongPressReduce();
            isPressReduce = false;
        });
        //来源
        SearchChild("n5").onClick.Set(() =>
        {
            UIMgr.Ins.showNextPopupView<PropSourcesView, PlayerProp>(playerProp);
            onDeleteAnimation<GiftsView>();
        });

        InitLongPress();
    }

    void InitLongPress()
    {
        LongPressGesture longPressAdd = new LongPressGesture(SearchChild("n12"));
        longPressAdd.trigger = 0.2f;
        longPressAdd.interval = 0.1f;
        longPressAdd.onAction.Set(OnLongPressAdd);

        LongPressGesture longPressReduce = new LongPressGesture(SearchChild("n13"));
        longPressReduce.trigger = 0.2f;
        longPressReduce.interval = 0.1f;
        longPressReduce.onAction.Set(OnLongPressReduce);
    }

    public override void InitData<D>(D data)
    {
        OnShowAnimation();
        base.InitData(data);
        item = data as GiftItem;
        sendNum = 1;
        sendNumText.text = sendNum + "";
        SetItemInfo();
        SwitchController(item.type);
        isPressAdd = false;
        isPressReduce = false;
    }

    void SetItemInfo()
    {
        if (item.type == GiftItem.SEND_GIFT)
        {
            playerProp = InteractiveDataMgr.ins.PlayerProps.Find(a => a.prop_id == item.id);
            GamePropConfig propConfig = JsonConfig.GamePropConfigs.Find(a => a.prop_id == item.id);
            nameText.text = propConfig.prop_name;
            totalNum = playerProp.prop_count;
            haveNumText.text = HAVE_NUM + playerProp.prop_count;
            descriptionText.text = propConfig.description;
            iconLoager.url = UrlUtil.GetItemIconUrl((int)propConfig.prop_id);
        }
        else
        {
            gameMall = JsonConfig.GameMallConfigs.Find(a => a.mall_id == item.id);
            playerProp = InteractiveDataMgr.ins.PlayerProps.Find(a => a.prop_id == gameMall.prop_id);
            nameText.text = gameMall.name;
            if (playerProp != null)
                haveNumText.text = HAVE_NUM + playerProp.prop_count + "";
            else
            {
                haveNumText.text = HAVE_NUM + "0";
                playerProp = new PlayerProp();
                playerProp.prop_id = gameMall.prop_id;
                playerProp.prop_count = 0;
            }

            descriptionText.text = gameMall.description;
            iconLoager.url = UrlUtil.GetItemIconUrl((int)gameMall.prop_id);
            if (item.type == GiftItem.MAKE_GIFT)
                tinyItem = ItemUtil.GetTinyItem(gameMall.cost);
            else
                tinyItem = ItemUtil.GetTinyItem(gameMall.unlock_cost);
            costLoader.url = CommonUrlConfig.GetConsumeItemUrl((TypeConfig.Consume)tinyItem.type);
            costNumText.text = tinyItem.num + "";

            if (tinyItem.type == (int)TypeConfig.Consume.Diamond)
                totalNum = GameData.Player.diamond / tinyItem.num;
            else if (tinyItem.type == (int)TypeConfig.Consume.Star)
                totalNum = GameData.Player.love / tinyItem.num;
        }
    }

    public void OnClickComfirm()
    {
        switch (item.type)
        {
            case 0:
                RequestPresentProp();
                break;
            case 1:
                RequestMakeProp();
                break;
            case 2:
                RequestStudyProp();
                break;
        }
    }

    //送礼
    void RequestPresentProp()
    {
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("actorId", InteractiveDataMgr.ins.CurrentPlayerActor.actor_id);
        wWWForm.AddField("propId", playerProp.prop_id);
        wWWForm.AddField("num", sendNum);
        InteractiveDataMgr.ins.presentProp = playerProp.prop_id;
        GameMonoBehaviour.Ins.RequestInfoPost<PlayerActor>(NetHeaderConfig.PRESENT_PROP, wWWForm, null);
        onDeleteAnimation<GiftsView>();
    }


    //学习制作礼物
    void RequestStudyProp()
    {
        if (tinyItem.type == (int)TypeConfig.Consume.Diamond && tinyItem.num * sendNum > GameData.Player.diamond)
        {
            UIMgr.Ins.showErrorMsgWindow(MsgException.DIAMOND_NOT_ENOUGH);
            return;
        }
        else if (tinyItem.type == (int)TypeConfig.Consume.Star && tinyItem.num * sendNum > GameData.Player.love)
        {
            UIMgr.Ins.showErrorMsgWindow(MsgException.LOVE_NOT_ENOUGH);
            return;
        }
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("actorId", InteractiveDataMgr.ins.CurrentPlayerActor.actor_id);
        wWWForm.AddField("mallId", gameMall.mall_id);
        GameMonoBehaviour.Ins.RequestInfoPost<ActorProperty>(NetHeaderConfig.PROP_STUDY, wWWForm, RequestStudyPropSuccess);
    }

    void RequestStudyPropSuccess()
    {
        EventMgr.Ins.DispachEvent(EventConfig.MUSIC_COMMON_EFFECT, normalInfo);
        UIMgr.Ins.showErrorMsgWindow("学习成功!");
        onDeleteAnimation<GiftsView>();
    }

    //制作礼物
    void RequestMakeProp()
    {
        if (tinyItem.type == (int)TypeConfig.Consume.Diamond && tinyItem.num > GameData.Player.diamond)
        {
            UIMgr.Ins.showErrorMsgWindow(MsgException.DIAMOND_NOT_ENOUGH);
            return;
        }
        else if (tinyItem.type == (int)TypeConfig.Consume.Star && tinyItem.num > GameData.Player.love)
        {
            UIMgr.Ins.showErrorMsgWindow(MsgException.LOVE_NOT_ENOUGH);
            return;
        }
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("actorId", InteractiveDataMgr.ins.CurrentPlayerActor.actor_id);
        wWWForm.AddField("mallId", gameMall.mall_id);
        wWWForm.AddField("num", sendNum);
        GameMonoBehaviour.Ins.RequestInfoPost<PropMake>(NetHeaderConfig.PROP_MAKE, wWWForm, RequestMakePropSuccess);
    }

    void RequestMakePropSuccess()
    {
        TinyItem tiny = new TinyItem();
        tiny.num = sendNum;
        tiny.url = UrlUtil.GetItemIconUrl(gameMall.prop_id);
        UIMgr.Ins.showNextPopupView<CommonGetPropsView, TinyItem>(tiny);
        onDeleteAnimation<GiftsView>();
    }

    public override void OnShowAnimation()
    {
        base.OnShowAnimation();
        gameObject.SetActive(true);
        ui.visible = true;
    }

    bool isPressAdd;
    void OnLongPressAdd()
    {
        if (sendNum < totalNum)
        {
            sendNum++;
            sendNumText.text = sendNum + "";
            if (controller.selectedIndex == 1)
                costNumText.text = (sendNum * tinyItem.num) + "";
        }
        else
        {
            if (!isPressAdd)
            {
                if (item.type == GiftItem.MAKE_GIFT)
                    UIMgr.Ins.showErrorMsgWindow(MsgException.LOVE_NOT_ENOUGH);
                else if (item.type == GiftItem.SEND_GIFT)
                    UIMgr.Ins.showErrorMsgWindow("送出数量不能高于拥有数量！");
            }
            isPressAdd = true;
        }

    }
    bool isPressReduce;
    void OnLongPressReduce()
    {
        if (sendNum > 1)
        {
            sendNum--;
            sendNumText.text = sendNum + "";
            if (controller.selectedIndex == 1)
                costNumText.text = (sendNum * tinyItem.num) + "";
        }
        else
        {
            if (!isPressReduce)
            {
                if (item.type == GiftItem.MAKE_GIFT)
                    UIMgr.Ins.showErrorMsgWindow("制作数量不能低于1！");
                else if (item.type == GiftItem.SEND_GIFT)
                    UIMgr.Ins.showErrorMsgWindow("送出数量不能低于1！");
            }
            isPressReduce = true;
        }

    }

}

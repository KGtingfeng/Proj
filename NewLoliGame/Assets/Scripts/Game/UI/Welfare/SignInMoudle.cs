using FairyGUI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignInMoudle : BaseMoudle
{
    /// <summary>
    /// "大礼包领取情况"_0可领取，1已领取，2不可领取
    /// </summary>
    Controller pageController;

    WelfareInfo welfareInfo;
    GList awardItemList;
    GTextField totalDayText;
    GTextField packText;
    GButton signInBtn;
    List<TinyItem> awardItems = new List<TinyItem>();

    int packDay;
    public override void InitMoudle(GComponent gComponent, int controllerIndex)
    {
        base.InitMoudle(gComponent, controllerIndex);
        InitUI();
        InitEvent();
    }

    public override void InitUI()
    {
        base.InitUI();
        awardItemList = SearchChild("n11").asList;
        pageController = ui.GetController("c1");
        totalDayText = SearchChild("n21").asTextField;
        signInBtn = SearchChild("n16").asButton;
        packText = SearchChild("n7").asTextField;
    }

    public override void InitEvent()
    {
        base.InitEvent();

        SearchChild("n13").onClick.Set(OnBigAwardClick);    //大礼包被点击
        //SearchChild("n15").onClick.Set(OnBigAwardClick);    //大礼包被点击

        signInBtn.onClick.Set(OnSignInClick); //签到
    }

    void OnBigAwardClick()
    {
        switch (pageController.selectedIndex)
        {
            case 0:         //可领取
                WWWForm wWWForm = new WWWForm();
                GameMonoBehaviour.Ins.RequestInfoPostHaveData<PropMake>(NetHeaderConfig.WELFARE_DAILY_AWARD, wWWForm, (PropMake propMake) =>
                {
                    pageController.selectedIndex = 1;
                    CheckRedPoint();
                    EventMgr.Ins.DispachEvent(EventConfig.WELFARE_TAB_RED_POINT);
                    TouchScreenView.Ins.ShowPropsTost(propMake.playerProp);
                });
                break;
            case 1:         //已领取
                UIMgr.Ins.showErrorMsgWindow("大礼包已领取");
                break;
            case 2:         //不可领取
                UIMgr.Ins.showErrorMsgWindow("大礼包当前不可领取");
                break;
            default:
                break;
        }
    }

    void OnSignInClick()
    {
        if (welfareInfo.signedToday == 1) //已签到
        {
            UIMgr.Ins.showErrorMsgWindow("今日已签到");
        }
        else//未签到
        {
            int curDay = welfareInfo.curTimes + 1;
            WWWForm wWWForm = new WWWForm();
            wWWForm.AddField("checkIndex", curDay);
            GameMonoBehaviour.Ins.RequestInfoPostHaveData<PropMake>(NetHeaderConfig.WELFARE_DAILY_CHECK, wWWForm, (PropMake propMake) =>
            {
                welfareInfo.curTimes++;
                welfareInfo.signedToday = 1;

                Refresh();
                TouchScreenView.Ins.ShowPropsTost(propMake.playerProp);

            });
        }
    }


    public override void InitData<D>(D data)
    {
        base.InitData();
        welfareInfo = data as WelfareInfo;
        GoWrapper();
        WelfareView view = baseView as WelfareView;
        ChannelSwitchConfig config = view.configs.Find(a => a.key == ChannelSwitchConfig.KEY_PACK);
        packDay = config.value;
        packText.text = "当月累计签到" + config.value + "天可领取礼包";

        string[] items = welfareInfo.awardData.Split(new char[1] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var str in items)
        {
            TinyItem tinyItem = ItemUtil.GetTinyItem(str);
            awardItems.Add(tinyItem);
        }

        awardItemList.SetVirtual();
        awardItemList.itemRenderer = RendererItem;
        awardItemList.numItems = items.Length;

        Refresh();
  
    }

    private void Refresh()
    {
        SetBigAward();
        awardItemList.RefreshVirtualList();
        SetItemEffect();
        CheckRedPoint();
        EventMgr.Ins.DispachEvent(EventConfig.WELFARE_TAB_RED_POINT);
        int day = welfareInfo.curTimes;
        totalDayText.text = day < 10 ? "0" + day : day + "";
    }

    private void RendererItem(int index, GObject gObject)
    {
        GComponent gComponent = gObject as GComponent;

        TinyItem item = awardItems[index];

        GamePropConfig propConfig = JsonConfig.GamePropConfigs.Find(a => a.prop_id == item.id);

        gComponent.GetChild("n4").asTextField.text = item.num.ToString();       //设置数量

        gComponent.GetChild("n14").asLoader.url = item.url;//设置图片url

        GTextField dateText = gComponent.GetChild("n6").asTextField;
        dateText.text = index < 9 ? 0 + (index + 1).ToString() : (index + 1).ToString();

        //根据签到天数,设置礼品状态
        //0可领取，1已领取，2不可领取
        Controller controller = gComponent.GetController("c1");
        if (index == welfareInfo.curTimes)
        {
            if (welfareInfo.signedToday == 1)
            {
                controller.selectedIndex = 2;
            }
            else
            {
                controller.selectedIndex = 0;   //今日未签到
            }

        }
        else if (index < welfareInfo.curTimes)
        {
            controller.selectedIndex = 1;
        }
        else
        {
            controller.selectedIndex = 2;
        }

        gComponent.onClick.Set(() =>        //奖品被点击,显示奖励详情
        {
            WWWForm wWWForm = new WWWForm();
            wWWForm.AddField("propId", awardItems[index].id);
            GameMonoBehaviour.Ins.RequestInfoPost(NetHeaderConfig.PLAYER_PROP_INFO, wWWForm, (PlayerProp playerProp) =>
            {
                playerProp.prop_type = awardItems[index].type;
                ShopMallDataMgr.ins.CurrentPropInfo = playerProp;
                UIMgr.Ins.showNextPopupView<CommonPropTipsView, int>(awardItems[index].id);

            });
        });

    }


    private void SetBigAward()
    {
        if (string.IsNullOrEmpty(welfareInfo.bigAwardData))
        {
            pageController.selectedIndex = 1;
        }
        else
        {
            if (welfareInfo.curTimes >= packDay)
            {
                pageController.selectedIndex = 0;
            }
            else
            {
                pageController.selectedIndex = 2;
            }
        }
    }

    void SetItemEffect()
    {
        Vector3 pos = new Vector3();
        for (int i = 0; i < awardItemList.numChildren; i++)
        {
            GObject item = awardItemList.GetChildAt(i);

            item.alpha = 0;

            pos = GetItemPos(i, item);
            item.SetPosition(pos.x, pos.y + 200, pos.z);
            float time = i * 0.1f;

            item.TweenMoveY(pos.y, (time + 0.1f)).SetEase(EaseType.CubicOut).OnStart(() =>
            {
                item.TweenFade(1, (time + 0.15f)).SetEase(EaseType.QuadOut);
            });
        }
    }

    Vector3 GetItemPos(int index, GObject gObject)
    {
        Vector3 pos = gObject.position;
        if (pos == Vector3.zero)
        {
            pos.y = index * 255;
        }
        return pos;
    }

    void CheckRedPoint()
    {
        if (welfareInfo.signedToday == 1 && pageController.selectedIndex != 0)
        {
            RedpointMgr.Ins.welfareRedpoint[0] = 0;
        }
        else
            RedpointMgr.Ins.welfareRedpoint[0] = 1;
        //signInBtn.GetController("c1").selectedIndex = welfareInfo.signedToday == 0 ? 1 : 0;
    }
    GoWrapper goWrapper;
    public void GoWrapper()
    {
        if (goWrapper == null)
        {
            UnityEngine.Object prefab = Resources.Load("Game/GFX/Prefabs/UI_qiandao");
            GGraph effect = SearchChild("n29").asGraph;
            if (prefab != null)
            {
                GameObject go = (GameObject)UnityEngine.GameObject.Instantiate(prefab);
                //go.transform.localScale = new Vector3(2, 2, 2);
                goWrapper = new GoWrapper(go);
                effect.SetNativeObject(goWrapper);
            }
        }
        else
        {
            goWrapper.gameObject.SetActive(true);
        }
    }
}

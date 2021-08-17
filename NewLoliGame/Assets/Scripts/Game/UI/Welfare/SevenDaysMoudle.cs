using FairyGUI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SevenDaysMoudle : BaseMoudle
{
    GList sevenDayList;
    SevenInfo info;
    List<GameSevenAdConfig> sevenDayAwardDatas;

    List<int> getted = new List<int>();
    public override void InitMoudle(GComponent gComponent, int controllerIndex)
    {
        base.InitMoudle(gComponent, controllerIndex);
        InitUI();
        InitEvent();
    }

    public override void InitUI()
    {
        base.InitUI();

        sevenDayList = SearchChild("n1").asList;
    }

    public override void InitEvent()
    {
        base.InitEvent();
    }

    public override void InitData<D>(D data)
    {
        base.InitData(data);

        info = data as SevenInfo;
        sevenDayAwardDatas = JsonConfig.GameSevenAdConfigs.FindAll(a => a.type == GameSevenAdConfig.SEVEN_TYPE);
        getted.Clear();
        string[] signed = info.signedToday.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (string s in signed)
        {
            getted.Add(int.Parse(s));
        }
        sevenDayList.SetVirtual();
        sevenDayList.itemRenderer = RenderItem;
        sevenDayList.numItems = sevenDayAwardDatas.Count;
        SetItemEffect();


    }

    void RenderItem(int index, GObject gObject)         //渲染七天礼物列表
    {
        GComponent gComponent = gObject as GComponent;

        gComponent.GetChild("n3").text = "第" + MathUtil.GetZHINDEX(index + 1) + "天登陆";

        RenderAwardList(index, gComponent);

        //根据签到天数,设置礼品状态
        //0已领取，1可领取、双倍，2暂未获得
        Controller controller = gComponent.GetController("c1");
        if (index < info.loginTime)
        {
            if (getted.Contains(index + 1))
            {
                controller.selectedIndex = 0;
            }
            else
            {
                controller.selectedIndex = 1;
            }
        }
        else
        {
            controller.selectedIndex = 2;
        }


        gComponent.GetChild("n7").onClick.Set(() =>
        {
            Debug.Log("第" + MathUtil.GetZHINDEX(index + 1) + "天礼物被领取");
            int curDay = index + 1;
            WWWForm wWWForm = new WWWForm();
            wWWForm.AddField("key", curDay);
            GameMonoBehaviour.Ins.RequestInfoPostHaveData<PropMake>(NetHeaderConfig.WELFARE_SEVEN_CHECK, wWWForm, (PropMake propMake) =>
            {
                getted.Add(curDay);
                sevenDayList.RefreshVirtualList();
                Debug.Log("领取" + curDay + "礼物");
                CheckRedPoint();
                TouchScreenView.Ins.ShowPropsTost(propMake.playerProp);
            });
        });

    }

    void RenderAwardList(int index, GComponent gComponent)      //当天礼物列表
    {
        List<TinyItem> awardItems = new List<TinyItem>();
        string[] item = sevenDayAwardDatas[index].award.Split(new char[1] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var str in item)
        {
            TinyItem tinyItem = ItemUtil.GetTinyItem(str);
            awardItems.Add(tinyItem);
        }

        GList todayAwardList = gComponent.GetChild("n10").asList;
        todayAwardList.scrollPane.touchEffect = false;
        todayAwardList.SetVirtual();
        todayAwardList.itemRenderer = (tempIndex, tempObj) =>
        {
            RenderTadayItem(tempIndex, tempObj, awardItems);
        };
        todayAwardList.numItems = awardItems.Count;
    }

    void RenderTadayItem(int tempIndex, GObject tempObj, List<TinyItem> awardItems)     //当天具体礼物
    {
        GComponent tempCom = tempObj as GComponent;

        TinyItem tempItem = awardItems[tempIndex];

        GamePropConfig propConfig = JsonConfig.GamePropConfigs.Find(a => a.prop_id == tempItem.id);

        tempCom.GetChild("n64").asTextField.text = tempItem.num.ToString();       //设置数量

        tempCom.GetChild("n62").asLoader.url = tempItem.url;//设置图片url



        tempCom.onClick.Set(() =>        //奖品被点击，显示详细信息
        {
            WWWForm wWWForm = new WWWForm();
            wWWForm.AddField("propId", tempItem.id);
            GameMonoBehaviour.Ins.RequestInfoPost(NetHeaderConfig.PLAYER_PROP_INFO, wWWForm, (PlayerProp playerProp) =>
            {
                playerProp.prop_type = tempItem.type;
                ShopMallDataMgr.ins.CurrentPropInfo = playerProp;
                UIMgr.Ins.showNextPopupView<CommonPropTipsView, int>(tempItem.id);

            });
        });
    }

    void SetItemEffect()
    {
        Vector3 pos = new Vector3();
        for (int i = 0; i < sevenDayList.numChildren; i++)
        {
            GObject item = sevenDayList.GetChildAt(i);

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
        RedpointMgr.Ins.welfareRedpoint[1] = info.loginTime > getted.Count ? 1 : 0;
        EventMgr.Ins.DispachEvent(EventConfig.WELFARE_TAB_RED_POINT);
    }
}

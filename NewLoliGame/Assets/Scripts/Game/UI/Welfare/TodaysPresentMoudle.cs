using FairyGUI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TodaysPresentMoudle : BaseMoudle
{
    GList awardList;
    List<GameSevenAdConfig> awards;
    List<int> getted = new List<int>();
    WelfareInfo welfare;
    public override void InitMoudle(GComponent gComponent, int controllerIndex)
    {
        base.InitMoudle(gComponent, controllerIndex);
        InitUI();
        InitEvent();
    }

    public override void InitUI()
    {
        base.InitUI();

        awardList = SearchChild("n5").asList;

    }

    public override void InitEvent()
    {
        base.InitEvent();
    }

    public override void InitData<Data>(Data data)
    {
        base.InitData<Data>(data);
        welfare = data as WelfareInfo;
        SetGetted();
        awards = JsonConfig.GameSevenAdConfigs.FindAll(a => a.type == GameSevenAdConfig.AD_TYPE);
        awardList.SetVirtual();
        awardList.itemRenderer = RenderItem;
        awardList.numItems = awards.Count;

        SetItemEffect();
    }

    void RenderItem(int index, GObject gObject)
    {
        GComponent gComponent = gObject as GComponent;

        //当前礼物信息
        GameSevenAdConfig info = awards[index];
        //礼物状态
        //0可领取，1已领取
        Controller controller = gComponent.GetController("c1");
        if (getted.Contains(index + 1))
        {
            controller.selectedIndex = 1;
        }
        else
        {
            controller.selectedIndex = 0;

        }

        TinyItem tinyItem = ItemUtil.GetTinyItem(info.award);

        GamePropConfig propConfig = JsonConfig.GamePropConfigs.Find(a => a.prop_id == tinyItem.id);

        gComponent.GetChild("n8").asTextField.text = propConfig.prop_name;     //设置名字

        gComponent.GetChild("n4").asTextField.text = tinyItem.num.ToString();       //设置数量

        gComponent.GetChild("n15").asLoader.url = tinyItem.url;//设置图片url

        gComponent.onClick.Set(() =>
        {
            if (getted.Contains(index + 1))
            {
                UIMgr.Ins.showErrorMsgWindow("当前礼物已被领取");
            }
            else
            {
                //点击看广告领取奖励
                WWWForm wWWForm = new WWWForm();
                wWWForm.AddField("checkIndex", index + 1);
                GameMonoBehaviour.Ins.RequestInfoPostHaveData<PropMake>(NetHeaderConfig.WELFARE_AD_CHECK, wWWForm, (PropMake propMake) =>
                {
                    Debug.Log("领取" + (index + 1) + "礼物");
                    getted.Add(index + 1);
                    awardList.RefreshVirtualList();
                    CheckRedPoint();
                    TouchScreenView.Ins.ShowPropsTost(propMake.playerProp);
                });
            }
        });

    }

    private void SetGetted()
    {
        getted.Clear();
        if (welfare.signed_list != "0")
        {
            string[] get = welfare.signed_list.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string g in get)
            {
                getted.Add(int.Parse(g));
            }
        }
    }

    void SetItemEffect()
    {
        Vector3 pos = new Vector3();
        for (int i = 0; i < awardList.numChildren; i++)
        {
            GObject item = awardList.GetChildAt(i);

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
        RedpointMgr.Ins.welfareRedpoint[2] = awards.Count > getted.Count ? 1 : 0;
        EventMgr.Ins.DispachEvent(EventConfig.WELFARE_TAB_RED_POINT);
    }
}


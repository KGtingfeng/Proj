using FairyGUI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 这个界面对应主界面上的福利按钮
/// </summary>
[ViewAttr("Game/UI/M_Daily", "M_Daily", "Daily")]
public class WelfareView : BaseView
{

    public static WelfareView ins;
    public enum MoudleType
    {
        /// <summary>
        /// 每日签到
        /// </summary>
        SIGN_IN = 0,
        /// <summary>
        /// 七日登陆
        /// </summary>
        SEVEN_DAYS,
        /// <summary>
        /// 每日福利
        /// </summary>
        TODAYS_PRESENT
    }

    readonly Dictionary<MoudleType, string> moudleNamePairs = new Dictionary<MoudleType, string>()
    {
        {MoudleType.SIGN_IN,"n2" },
        {MoudleType.SEVEN_DAYS,"n9" },
        {MoudleType.TODAYS_PRESENT,"n8" }
    };

    GList topBtnList;
    int selectType;
    GLoader bgLoader;
    GTextField love;
    GTextField diamond;
    GComponent topInfoCom;
    public List<ChannelSwitchConfig> configs;
    public override void InitUI()
    {
        base.InitUI();
        topBtnList = SearchChild("n1").asList;
        controller = ui.GetController("c1");
        bgLoader = SearchChild("n18").asLoader;

        topInfoCom = SearchChild("n37").asCom;
        love = topInfoCom.GetChild("n15").asTextField;
        diamond = topInfoCom.GetChild("n16").asTextField;

        InitEvent();
        ins = this;
    }

    public override void InitEvent()
    {
        base.InitEvent();
        //返回主界面
        SearchChild("n0").onClick.Set(() =>
        {
            UIMgr.Ins.showMainView<WelfareView>();
        });
        SearchChild("n37").asCom.GetChild("n13").onClick.Set(() =>
        {
            UIMgr.Ins.showNextPopupView<BuyStarsView>();
        });
        //click diamondBtn 
        SearchChild("n37").asCom.GetChild("n14").onClick.Set(() =>
        {
            UIMgr.Ins.showNextPopupView<ShopMallView>();
        });
        topBtnList.onClickItem.Set(() =>
        {
            if (selectType == topBtnList.selectedIndex) return; //卫语句:如果点击的就是现在的按钮，直接返回

            selectType = topBtnList.selectedIndex;
            switch ((MoudleType)selectType)
            {
                case MoudleType.SIGN_IN:
                    GotoSignInMoudle();
                    break;
                case MoudleType.SEVEN_DAYS:
                    GotoSevenDaysMoudle();
                    break;
                case MoudleType.TODAYS_PRESENT:
                    GotoTodaysPresentMoudle();
                    break;
                default:
                    break;
            }
        });

        EventMgr.Ins.RegisterEvent(EventConfig.REFRESH_WELFARE_TOP_INFO, RefreshTopInfo);
        EventMgr.Ins.RegisterEvent(EventConfig.WELFARE_TAB_RED_POINT, InitTopBtnList);
    }

    public override void InitData()
    {
        base.InitData();

    }

    public override void InitData<D>(D data)
    {
        base.InitData(data);
        configs = data as List<ChannelSwitchConfig>;
        
        SetTopBtn();

        bgLoader.url = UrlUtil.GetSkinBgUrl(7021);
        RefreshTopInfo();

        topBtnList.selectedIndex = (int)MoudleType.SIGN_IN;
        selectType = topBtnList.selectedIndex;
        GotoSignInMoudle();     //默认进入每日签到界面 
        InitTopBtnList();
    }

    private void SetTopBtn()
    {
        ChannelSwitchConfig seven = configs.Find(a => a.key == ChannelSwitchConfig.KEY_SEVEN);
        if (seven != null && seven.value == 0)
        {
            topBtnList.GetChildAt(1).visible = false;
        }
        else
        {
            topBtnList.GetChildAt(1).visible = true;
        }
        ChannelSwitchConfig ad = configs.Find(a => a.key == ChannelSwitchConfig.KEY_AD);
        if (ad != null && ad.value == 0)
        {
            topBtnList.GetChildAt(2).visible = false;
        }
        else
        {
            topBtnList.GetChildAt(2).visible = true;
        }
        ChannelSwitchConfig daily = configs.Find(a => a.key == ChannelSwitchConfig.KEY_DAILY);
        if (daily != null && daily.value == 0)
        {
            topBtnList.GetChildAt(0).visible = false;
        }
        else
        {
            topBtnList.GetChildAt(0).visible = true;
        }
    }

    void RefreshTopInfo()
    {
        love.text = GameData.Player.love + "";
        diamond.text = GameData.Player.diamond + "";
    }

    private void GotoSignInMoudle()
    {
        WWWForm wWWForm = new WWWForm();
        GameMonoBehaviour.Ins.RequestInfoPostHaveData<WelfareInfo>(NetHeaderConfig.WELFARE_DAILY_LIST, wWWForm, (WelfareInfo info) =>
        {
            GoToMoudle<SignInMoudle, WelfareInfo>(selectType, info);
        });
    }

    private void GotoSevenDaysMoudle()
    {
        WWWForm wWWForm = new WWWForm();
        GameMonoBehaviour.Ins.RequestInfoPostHaveData<SevenInfo>(NetHeaderConfig.WELFARE_SEVEN_LIST, wWWForm, (SevenInfo info) =>
        {
            GoToMoudle<SevenDaysMoudle, SevenInfo>(selectType, info);
        });

    }

    private void GotoTodaysPresentMoudle()
    {

        WWWForm wWWForm = new WWWForm();
        GameMonoBehaviour.Ins.RequestInfoPostHaveData<WelfareInfo>(NetHeaderConfig.WELFARE_AD_LIST, wWWForm, (WelfareInfo info) =>
        {

            GoToMoudle<TodaysPresentMoudle, WelfareInfo>(selectType, info);
        });

    }

    public void InitTopBtnList()
    {
        topBtnList.itemRenderer = TopBtnRenderer;
        topBtnList.numItems = 3;
    }

    void TopBtnRenderer(int index, GObject obj)
    {
        GComponent com = obj.asCom;
        com.GetController("c1").selectedIndex = RedpointMgr.Ins.welfareRedpoint[index];
    }

    #region 两个GoToMoudle方法
    public override void GoToMoudle<T, D>(int index, D data)
    {
        MoudleType moudleType = (MoudleType)index;
        BaseMoudle baseMoudle = FindMoudle<T>();

        if (baseMoudle == null)
        {
            baseMoudle = (BaseMoudle)Activator.CreateInstance(typeof(T));
            baseMoudles.Add(baseMoudle);
            GComponent gComponent = null;

            string componmentName;
            if (moudleNamePairs.TryGetValue(moudleType, out componmentName))
            {
                GObject gObject = ui.GetChild(componmentName);
                if (gObject == null)
                {
                    Debug.Log(componmentName + " not find, please check it");
                    return;
                }
                gComponent = gObject.asCom;
            }
            else
                gComponent = ui;

            baseMoudle.baseView = this;
            baseMoudle.InitMoudle<D>(gComponent, index, data);
        }
        baseMoudle.InitData<D>(data);
        SwitchController(index);
    }

    public override void GoToMoudle<T>(int index)
    {
        MoudleType moudleType = (MoudleType)index;
        BaseMoudle baseMoudle = FindMoudle<T>();

        if (baseMoudle == null)
        {
            baseMoudle = (BaseMoudle)Activator.CreateInstance(typeof(T));
            baseMoudles.Add(baseMoudle);
            GComponent gComponent = null;

            string componmentName;
            if (moudleNamePairs.TryGetValue(moudleType, out componmentName))
            {
                GObject gObject = ui.GetChild(componmentName);
                if (gObject == null)
                {
                    Debug.Log(componmentName + " not find, please check it");
                    return;
                }
                gComponent = gObject.asCom;
            }
            else
                gComponent = ui;

            baseMoudle.baseView = this;
            baseMoudle.InitMoudle(gComponent, index);
        }
        baseMoudle.InitData();
        SwitchController(index);
    }
    #endregion

    public void NewbieSeven()
    {
        topBtnList.selectedIndex = (int)MoudleType.SEVEN_DAYS;
        selectType = topBtnList.selectedIndex;
        GotoSevenDaysMoudle();
    }

    public void NewbieDaily()
    {
        topBtnList.selectedIndex = (int)MoudleType.TODAYS_PRESENT;
        selectType = topBtnList.selectedIndex;
        GotoTodaysPresentMoudle();
    }
   
}

using System;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;


[ViewAttr("Game/UI/F_Room", "F_Room", "Room")]
public class RoomView : BaseView
{
    public enum MoudleType
    {
        Room,
        Photo,
        Role,
        Time,
    }

    readonly Dictionary<MoudleType, string> moudleNamePairs = new Dictionary<MoudleType, string>()
    {

    };

    Player PlayerInfo
    {
        get { return GameData.Player; }
    }


    GTextField loveText;
    GTextField diamondText;
    GLoader bgLoader;
    public override void InitUI()
    {
        base.InitUI();
        controller = ui.GetController("c1");
        loveText = SearchChild("n36").asCom.GetChild("n15").asTextField;
        diamondText = SearchChild("n36").asCom.GetChild("n16").asTextField;
        bgLoader = SearchChild("n3").asLoader;
        InitEvent();
    }

    public override void InitEvent()
    {
        base.InitEvent();
        SearchChild("n36").asCom.GetChild("n13").onClick.Set(() =>
        {
            UIMgr.Ins.showNextPopupView<BuyStarsView>();
        });
        //click diamondBtn 
        SearchChild("n36").asCom.GetChild("n14").onClick.Set(() =>
        {
            UIMgr.Ins.showNextPopupView<ShopMallView>();
        });

        SearchChild("n12").onClick.Set(OnClickReturn);

        SearchChild("n13").onClick.Set(OnClickAlarm);

        SearchChild("n18").onClick.Set(GotoPhoto);
        SearchChild("n22").onClick.Set(() =>
        {
            GotoSelectRole();
        });
        SearchChild("n23").onClick.Set(GotoCollection);

        EventMgr.Ins.RegisterEvent(EventConfig.REFRESH_ROOM_TOP_INFO, InitTopInfo);

    }

    public override void InitData()
    {
        base.InitData();
        InitTopInfo();
        GotoRoom();
    }

    //从收集按钮进入房间相册
    public override void InitData<D>(D data)
    {
        base.InitData(data);
        InitTopInfo();
        GotoPhoto();
    }

    void InitTopInfo()
    {
        //love
        loveText.text = PlayerInfo.love + "";
        //money
        diamondText.text = PlayerInfo.diamond + "";
    }



    void OnClickReturn()
    {
        switch (controller.selectedIndex)
        {
            case (int)MoudleType.Room:
                UIMgr.Ins.showMainView<RoomView>();
                break;
            case (int)MoudleType.Photo:
                GotoRoom();
                break;
            case (int)MoudleType.Role:
                SwitchController((int)MoudleType.Photo);
                break;
            case (int)MoudleType.Time:
                GoToMoudle<SelectRoleTimeMoudle>((int)MoudleType.Role);
                break;
        }
    }

    void GotoRoom()
    {
        SwitchController((int)MoudleType.Room);
        if (DateTime.Now.Hour < 7 || DateTime.Now.Hour >= 19)
            bgLoader.url = UrlUtil.GetRoomBgUrl("01");
        else
            bgLoader.url = UrlUtil.GetRoomBgUrl("02");
    }

    void GotoPhoto()
    {
        SwitchController((int)MoudleType.Photo);
        bgLoader.url = UrlUtil.GetRoomBgUrl("03");
    }

    void OnClickAlarm()
    {
        WWWForm wWWForm = new WWWForm();
        GameMonoBehaviour.Ins.RequestInfoPostHaveData<AlarmClockInfo>(NetHeaderConfig.QUERY_ALARM, wWWForm, (AlarmClockInfo info) =>
        {
            UIMgr.Ins.showNextPopupView<AlarmClockView, AlarmClockInfo>(info);
        });
    }

    void GotoCollection()
    {
        WWWForm wWWForm = new WWWForm();
        GameMonoBehaviour.Ins.RequestInfoPost(NetHeaderConfig.ACTOR_LIST, wWWForm, (List<Role> roles) =>
        {
            UIMgr.Ins.showNextPopupView<CollectionView, List<Role>>(roles);

        });
    }

    void GotoSelectRole()
    {
        WWWForm wWWForm = new WWWForm();
        GameMonoBehaviour.Ins.RequestInfoPost(NetHeaderConfig.QUERY_TIME, wWWForm, (List<TimeChart> info) =>
        {

            GoToMoudle<SelectRoleTimeMoudle, List<TimeChart>>((int)MoudleType.Role, info);

        });
    }

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
            baseMoudle.InitMoudle(gComponent, index);
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
}

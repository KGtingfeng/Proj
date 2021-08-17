using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using System;


[ViewAttr("Game/UI/C_Achievement", "C_Achievement", "Achievement")]
public class AchievementView : BaseView
{

    Player playerInfo
    {
        get { return GameData.Player; }
    }

    public enum MoudleType
    {
        Main,
        Role,
        Task,
    };

    readonly Dictionary<MoudleType, string> moudleNamePairs = new Dictionary<MoudleType, string>()
    {

    };


    GTextField loveText;
    GTextField diamondText;
    GLoader bgLoader;

    public override void InitUI()
    {
        base.InitUI();
        loveText = SearchChild("n21").asCom.GetChild("n15").asTextField;
        diamondText = SearchChild("n21").asCom.GetChild("n16").asTextField;
        controller = ui.GetController("c1");
        bgLoader = SearchChild("n10").asLoader;

        InitEvent();
    }

    public override void InitEvent()
    {
        base.InitEvent();
        SearchChild("n21").asCom.GetChild("n13").onClick.Set(() =>
        {
            EventMgr.Ins.DispachEvent(EventConfig.CLOSE_INTERACTIVE_TALK);
            UIMgr.Ins.showNextPopupView<BuyStarsView>();
        });
        //click diamondBtn 
        SearchChild("n21").asCom.GetChild("n14").onClick.Set(() =>
        {
            EventMgr.Ins.DispachEvent(EventConfig.CLOSE_INTERACTIVE_TALK);
            UIMgr.Ins.showNextPopupView<ShopMallView>();
        });

        SearchChild("n8").onClick.Set(OnClickBack);

        EventMgr.Ins.RegisterEvent(EventConfig.REFRESH_ACHIEVEMENT_TOP_INFO, InitTopInfo);

    }

    public override void InitData<D>(D data)
    {
        OnShowAnimation();
        base.InitData(data);
        AchievementDataMgr.Ins.GetTask(data as DailyTask);
        InitTopInfo();
        bgLoader.url = UrlUtil.GetBgUrl("Achievement", "bg");
        GoToMoudle<AchievementMainMoudle>((int)MoudleType.Main);
        EventMgr.Ins.ReplaceEvent(EventConfig.BUY_STAR_REFRESH, BuyStarRefresh);
        if (GameData.isOpenGuider)
        {
           // StoryCacheMgr.storyCacheMgr.GetStoryGameSave("Newbie", GameGuideConfig.TYPE_ACHIEVEMENT, (storyGameSave) =>
           //{
           //    if (storyGameSave.IsDefault)
           //    {
           //        GameData.guiderCurrent = GuiderInfoLinked.GetCurrGuiderInfo(GameGuideConfig.TYPE_ACHIEVEMENT, 1);
           //        UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);

           //    }
           //});


            /*
           WWWForm wWWForm = new WWWForm();
           wWWForm.AddField("nodeId", GameGuideConfig.TYPE_ACHIEVEMENT);
           wWWForm.AddField("key", "Newbie");
           GameMonoBehaviour.Ins.RequestInfoPostHaveData<List<StoryGameSave>>(NetHeaderConfig.STROY_LOAD_GAME, wWWForm, (List<StoryGameSave> storyGameSaves) =>
             {
                 if (storyGameSaves.Count > 0)
                 {
                     string[] save = storyGameSaves[0].value.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                     GameData.guiderCurrent = GuiderInfoLinked.GetCurrGuiderInfo(int.Parse(save[0]), int.Parse(save[1]));
                     if (GameData.guiderCurrent != null)
                     {
                         string[] roll_to = GameData.guiderCurrent.guiderInfo.roll_to.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                         if (roll_to.Length < 2 || !GameData.isOpenGuider)
                         {
                             return;
                         }

                     }

                 }
                 else
                 {
                     GameData.guiderCurrent = GuiderInfoLinked.GetCurrGuiderInfo(GameGuideConfig.TYPE_ACHIEVEMENT, 1);
                     UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);
                 }

             }, false);     */
        }
    }

    void InitTopInfo()
    {
        //love
        loveText.text = playerInfo.love + "";
        //money
        diamondText.text = playerInfo.diamond + "";
    }
    //返回
    void OnClickBack()
    {
        switch (controller.selectedIndex)
        {
            case (int)MoudleType.Main:
                UIMgr.Ins.showMainView<AchievementView>();
                break;
            case (int)MoudleType.Role:
                GoToMoudle<AchievementMainMoudle>((int)MoudleType.Main);
                break;
            case (int)MoudleType.Task:
                switch (AchievementDataMgr.Ins.CurrentTaskType)
                {
                    case TaskType.Role:
                        GoToMoudle<AchievementRoleMoudle>((int)MoudleType.Role);
                        break;
                    default:
                        GoToMoudle<AchievementMainMoudle>((int)MoudleType.Main);
                        break;
                }
                break;
        }
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

    /// <summary>
    /// 任务界面购买星星刷新任务数据,所有在任务界面可弹出界面需加入
    /// </summary>
    void BuyStarRefresh()
    {
        WWWForm wWWForm = new WWWForm();
        GameMonoBehaviour.Ins.RequestInfoPost<DailyTask>(NetHeaderConfig.MISSION_LIST, wWWForm, (DailyTask newDailyTask) =>
        {

            AchievementDataMgr.Ins.GetTask(newDailyTask);
            if (controller.selectedIndex == (int)MoudleType.Task)
            {
                EventMgr.Ins.DispachEvent(EventConfig.REFRESH_ACHIEVEMENT_TASK);
            }
        });
    }

    public override void OnHideAnimation()
    {
        EventMgr.Ins.RemoveEvent(EventConfig.BUY_STAR_REFRESH);
        base.OnHideAnimation();
    }
}

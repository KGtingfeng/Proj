using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using System;

[ViewAttr("Game/UI/R_Task", "R_Task", "Task", true)]
public class TaskView : BaseView
{
    public static TaskView ins;

    GList taskList;
    GButton getAllBtn;
    GLoader bgLoader;
    List<GComponent> packList;
    List<GProgressBar> gProgresses;
    List<GameMissionConfig> missonConfigs;
    List<TaskInfo> taskInfos;
    GComponent topCom;
    List<List<TinyItem>> awardItems;

    DailyTask dailyTask;
    public override void InitUI()
    {
        base.InitUI();
        taskList = SearchChild("n16").asList;
        getAllBtn = SearchChild("n18").asButton;
        bgLoader = SearchChild("n40").asLoader;
        topCom = SearchChild("n39").asCom;
        GetPackList();
        InitEvent();
        ins = this;
    }

    void GetPackList()
    {
        packList = new List<GComponent>();
        gProgresses = new List<GProgressBar>();
        for (int i = 0; i < 5; i++)
        {
            GComponent gCom = SearchChild("n" + (i + 23)).asCom;
            packList.Add(gCom);

            GProgressBar gProgress = SearchChild("n" + (i + 30)).asProgress;

            gProgresses.Add(gProgress);
        }
    }

    public override void InitEvent()
    {
        base.InitEvent();
        SearchChild("n2").onClick.Set(() =>
        {
            UIMgr.Ins.showNextView<MainView>();
        });
        getAllBtn.onClick.Set(OnClickGetAll);
        //click loveBtn星星
        topCom.GetChild("n13").onClick.Set(() =>
        {
            UIMgr.Ins.showNextPopupView<BuyStarsView>();
        });
        //click diamondBtn 钻石
        topCom.GetChild("n14").onClick.Set(() =>
        {
            UIMgr.Ins.showNextPopupView<ShopMallView>();
        });
    }

    public override void InitData<D>(D data)
    {
        OnShowAnimation();
        base.InitData(data);
        dailyTask = data as DailyTask;

        bgLoader.url = UrlUtil.GetCommonBgUrl("task_BG");
        Refresh();
        EventMgr.Ins.ReplaceEvent(EventConfig.BUY_STAR_REFRESH, BuyStarRefresh);

        if (GameData.isGuider)
        {
            if (GameData.guiderCurrent.guiderInfo.flow == 5 && GameData.guiderCurrent.guiderInfo.step == 2)
            {
                GameData.guiderCurrent = GuiderInfoLinked.GetCurrGuiderInfo(5, 3);
            }
            
            
            UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);
        }
    }
    void GetTopText()
    {
        topCom.GetChild("n15").asTextField.text = GameData.Player.love + "";
        topCom.GetChild("n16").asTextField.text = GameData.Player.diamond + "";
    }

    void Refresh()
    {
        Sort();
        GetTopText();
        awardItems = new List<List<TinyItem>>();
        foreach (var misson in missonConfigs)
        {
            List<TinyItem> tinyItems = ItemUtil.GetTinyItmeList(misson.award);
            awardItems.Add(tinyItems);
        }
        taskList.SetVirtual();
        taskList.itemRenderer = RenderItem;
        taskList.numItems = missonConfigs.Count;
        taskList.ScrollToView(0);

        RenderPackList();
        SetItemEffect();
    }

    void Sort()
    {
        List<GameMissionConfig> gettedMissions = new List<GameMissionConfig>();
        List<GameMissionConfig> finishMissions = new List<GameMissionConfig>();
        List<GameMissionConfig> missions = new List<GameMissionConfig>();
        List<TaskInfo> gettedTasks = new List<TaskInfo>();
        List<TaskInfo> tasks = new List<TaskInfo>();
        List<TaskInfo> finishTasks = new List<TaskInfo>();

        for (int i = dailyTask.dailyTasks.Count - 1; i >= 0; i--)
        {
            GameMissionConfig missionConfig = JsonConfig.GameMissionConfigs.Find(a => a.id == dailyTask.dailyTasks[i].missionId);
            if (missionConfig.task_type != (int)TaskType.Daily)
                continue;
            switch (dailyTask.dailyTasks[i].state)
            {
                case 0:
                    missions.Add(missionConfig);
                    tasks.Add(dailyTask.dailyTasks[i]);
                    break;
                case 1:
                    finishMissions.Add(missionConfig);
                    finishTasks.Add(dailyTask.dailyTasks[i]);
                    break;
                case 2:
                    gettedMissions.Add(missionConfig);
                    gettedTasks.Add(dailyTask.dailyTasks[i]);
                    break;
            }
        }
        //对未完成任务根据完成百分比排序
        for (int i = 0; i < missions.Count - 1; i++)
        {
            int index = i;
            for (int j = i + 1; j < missions.Count; j++)
            {
                if ((float)tasks[index].progress / missions[index].progress < (float)tasks[j].progress / missions[j].progress)
                {
                    index = j;
                }
            }
            if (i != index)
            {
                GameMissionConfig gameMission = missions[i];
                missions[i] = missions[index];
                missions[index] = gameMission;
                TaskInfo taskInfo = tasks[i];
                tasks[i] = tasks[index];
                tasks[index] = taskInfo;
            }
        }
        getAllBtn.visible = finishMissions.Count > 0 ? true : false;

        missonConfigs = finishMissions;
        missonConfigs.AddRange(missions);
        missonConfigs.AddRange(gettedMissions);
        taskInfos = finishTasks;
        taskInfos.AddRange(tasks);
        taskInfos.AddRange(gettedTasks);
    }

    void RenderItem(int index, GObject gObject)
    {
        GComponent gComponent = gObject.asCom;
        Controller taskController = gComponent.GetController("c1");
        GProgressBar progressBar = gComponent.GetChild("n22").asProgress;
        GTextField desText = gComponent.GetChild("n13").asTextField;
        GList itemList = gComponent.GetChild("n21").asList;

        taskController.selectedIndex = taskInfos[index].state;
        desText.text = missonConfigs[index].desc;
        itemList.scrollPane.touchEffect = false;
        itemList.itemRenderer = (int i, GObject gOb) => { RenderAwardItem(i, gOb, index); };
        itemList.numItems = awardItems[index].Count;
        progressBar.max = missonConfigs[index].progress;
        progressBar.value = taskInfos[index].progress;

        gComponent.GetChild("n17").onClick.Set(() => { OnClickGoto(missonConfigs[index].from); });
        gComponent.GetChild("n18").onClick.Set(() => { OnClickGetTask(missonConfigs[index].id); });

    }

    void RenderAwardItem(int index, GObject gObject, int itemIndex)
    {
        GComponent gCom = gObject.asCom;
        GTextField numText = gCom.GetChild("n64").asTextField;
        GLoader iconLoader = gCom.GetChild("n62").asLoader;
        TinyItem tinyItem = awardItems[itemIndex][index];
        numText.text = tinyItem.num + "";
        iconLoader.url = UrlUtil.GetPropsIconUrl(tinyItem);
        gCom.onClick.Set(() => { OnClickAward(tinyItem); });
    }

    string[] chests;
    void RenderPackList()
    {
        int num = 0;
        int vitality = dailyTask.vitality;
        chests = dailyTask.vitalityChests.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < 5; i++)
        {
            GTextField numText = packList[i].GetChild("n26").asTextField;
            TinyItem tinyItem = ItemUtil.GetTinyItem(JsonConfig.GameChestsConfigs.Find(a => a.id == i + 1).need);
            Controller chestsContraller = packList[i].GetController("c1");
            int index = i;
            packList[i].onClick.Set(() => { OnClickGetChests(index); });
            numText.text = tinyItem.num + "";
            gProgresses[i].max = tinyItem.num - num;
            if (vitality > tinyItem.num)
            {
                gProgresses[i].value = tinyItem.num;
            }
            else
            {
                gProgresses[i].value = vitality - num;
                vitality = 0;
            }
            chestsContraller.selectedIndex = int.Parse(chests[i]);
            num = tinyItem.num;
        }
    }

    void SetItemEffect()
    {
        Vector3 pos = new Vector3();
        for (int i = 0; i < taskList.numChildren; i++)
        {
            GObject item = taskList.GetChildAt(i);

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

    //查看奖励
    void OnClickAward(TinyItem tinyItem)
    {

        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("propId", tinyItem.id);
        GameMonoBehaviour.Ins.RequestInfoPost<PlayerProp>(NetHeaderConfig.PLAYER_PROP_INFO, wWWForm, (PlayerProp playerProp) =>
        {
            playerProp.prop_type = tinyItem.type;
            ShopMallDataMgr.ins.CurrentPropInfo = playerProp;
            UIMgr.Ins.showNextPopupView<CommonPropTipsView, int>(tinyItem.id);
        });
    }

    //领取奖励
    void OnClickGetTask(int missionId)
    {
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("missionId", missionId);
        GameMonoBehaviour.Ins.RequestInfoPost<TaskAwardInfo>(NetHeaderConfig.MISSION_GET_AWARD, wWWForm, (TaskAwardInfo info) =>
        {
            ShowAwards(info);
            dailyTask = info.mission;
            Refresh();
        });
    }

    //前往
    void OnClickGoto(string from)
    {
        JumpMgr.Ins.JumpView(from);
        string[] str = from.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        if (!str[0].Equals("0"))
            OnHideAnimation();
    }

    public void NewbieGotoRolegroup()
    {
        string from = "4,4";
        JumpMgr.Ins.JumpView(from);
        string[] str = from.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        if (!str[0].Equals("0"))
            OnHideAnimation();
    }

    //全部领取
    public void OnClickGetAll()
    {
        List<string> str = new List<string>();
        for (int i = 0; i < taskInfos.Count; i++)
        {
            if (taskInfos[i].state != 1)
            {
                break;
            }
            str.Add(taskInfos[i].missionId.ToString());
        }
        string missionIds = string.Join(",", str);
        Debug.LogError("SPS------------- get all mission  " + missionIds);
        if (missionIds.Length == 0)
            return;
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("missionId", missionIds);
        GameMonoBehaviour.Ins.RequestInfoPost<TaskAwardInfo>(NetHeaderConfig.MISSION_GET_AWARD, wWWForm, (TaskAwardInfo info) =>
        {
            ShowAwards(info);
            dailyTask = info.mission;
            Refresh();
        });
        //GameTool.ShowLevelUpEffect("1;10");

    }

    //领取宝箱
    void OnClickGetChests(int index)
    {
        switch (chests[index])
        {
            case "0":
                GameChestsConfig chestsConfig = JsonConfig.GameChestsConfigs[index];
                UIMgr.Ins.showNextPopupView<ChestsAwardView, string>(chestsConfig.award);

                break;
            case "1":
                WWWForm wWWForm = new WWWForm();
                wWWForm.AddField("chestId", index.ToString());
                GameMonoBehaviour.Ins.RequestInfoPost<TaskAwardInfo>(NetHeaderConfig.MISSION_GET_CHESTS, wWWForm, (TaskAwardInfo info) =>
                {
                    ShowAwards(info);
                    dailyTask = info.mission;
                    Refresh();
                });
                break;
            case "2":
                UIMgr.Ins.showErrorMsgWindow("宝箱已领取!");
                break;
        }

    }
    /// <summary>
    /// 展示奖励，钻石、星星、活跃单独处理
    /// </summary>
    /// <param name="info"></param>
    void ShowAwards(TaskAwardInfo info)
    {
        List<PlayerProp> props = new List<PlayerProp>();
        if (info.extra != null && info.extra.Count > 0)
        {
            TaskAward taskAward = info.extra[0];
            if (taskAward.playerProp != null)
            {
                foreach (var prop in taskAward.playerProp)
                {
                    PlayerProp playerProp = props.Find(a => a.prop_id == prop.prop_id);
                    if (playerProp == null)
                    {
                        props.Add(prop);
                    }
                    else
                    {
                        playerProp.prop_count += prop.prop_count;
                    }
                }
            }
            if (!string.IsNullOrEmpty(info.extra[0].level))
            {
                GameTool.ShowLevelUpEffect(info.extra[0].level);
            }
             
            if (taskAward.love > GameData.Player.love)
            {
                PlayerProp playerProp = new PlayerProp()
                {
                    prop_id = (int)TypeConfig.Consume.Star,
                    prop_type = (int)TypeConfig.Consume.Star,
                    prop_count = taskAward.love - GameData.Player.love,
                };
                props.Add(playerProp);
                GameData.Player.love = taskAward.love;
            }
            if (taskAward.diamond > GameData.Player.diamond)
            {
                PlayerProp playerProp = new PlayerProp()
                {
                    prop_id = (int)TypeConfig.Consume.Diamond,
                    prop_type = (int)TypeConfig.Consume.Diamond,
                    prop_count = taskAward.diamond - GameData.Player.diamond,
                };
                props.Add(playerProp);
                GameData.Player.diamond = taskAward.diamond;
            };
            if (taskAward.exp > GameData.Player.exp)
            {
                PlayerProp playerProp = new PlayerProp()
                {
                    prop_id = (int)TypeConfig.Consume.EXP,
                    prop_type = (int)TypeConfig.Consume.EXP,
                    prop_count = taskAward.exp - GameData.Player.exp,
                };
                props.Add(playerProp);
                GameData.Player.exp = taskAward.exp;
            };
        }

        if (info.mission.vitality > dailyTask.vitality)
        {
            PlayerProp playerProp = new PlayerProp()
            {
                prop_id = (int)TypeConfig.Consume.Active,
                prop_type = (int)TypeConfig.Consume.Active,
                prop_count = info.mission.vitality - dailyTask.vitality,
            };
            props.Add(playerProp);
        }
        TouchScreenView.Ins.ShowPropsTost(props);
        EventMgr.Ins.DispachEvent(EventConfig.REFRESH_USER_TOP_INFO);
    }

    /// <summary>
    /// 任务界面购买星星刷新任务数据,所有在任务界面可弹出界面需加入
    /// </summary>
    void BuyStarRefresh()
    {

        WWWForm wWWForm = new WWWForm();
        GameMonoBehaviour.Ins.RequestInfoPost<DailyTask>(NetHeaderConfig.MISSION_LIST, wWWForm, (DailyTask newDailyTask) =>
        {
            dailyTask = newDailyTask;
            Refresh();
        });

    }

    public override void OnHideAnimation()
    {
        EventMgr.Ins.RemoveEvent(EventConfig.BUY_STAR_REFRESH);
        base.OnHideAnimation();
    }
}

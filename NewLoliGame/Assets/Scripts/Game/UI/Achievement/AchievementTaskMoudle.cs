using FairyGUI;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class AchievementTaskMoudle : BaseMoudle
{
    GTextField titleText;
    GTextField progressText;
    GButton getAllBtn;

    GList taskList;
    List<TaskInfo> taskInfos;
    NormalInfo normalInfo;
    List<List<TinyItem>> awardItems;

    List<GameMissionConfig> currentMissonConfigs;
    List<TaskInfo> currentTaskInfos;
    readonly List<string> titleList = new List<string>
    {
        "成长之路",
        "首次体验",
        "角色专辑",
        "收集之旅",
    };
    public override void InitMoudle(GComponent gComponent, int controllerIndex)
    {
        base.InitMoudle(gComponent, controllerIndex);
        InitUI();
        InitEvent();
    }

    public override void InitUI()
    {
        base.InitUI();
        taskList = SearchChild("n17").asList;
        progressText = SearchChild("n19").asTextField;
        titleText = SearchChild("n14").asTextField;
        getAllBtn = SearchChild("n18").asButton;
    }

    public override void InitEvent()
    {
        base.InitEvent();
        getAllBtn.onClick.Set(OnClickGetAll);
        EventMgr.Ins.RegisterEvent(EventConfig.REFRESH_ACHIEVEMENT_TASK, Refresh);
    }

    public override void InitData<D>(D data)
    {
        base.InitData(data);
        normalInfo = data as NormalInfo;
        Refresh();
    }

    void Refresh()
    {
        switch (normalInfo.type)
        {
            case (int)TaskType.Role:
                taskInfos = AchievementDataMgr.Ins.roleInfoDic[normalInfo.index];
                titleText.text = JsonConfig.GameInitCardsConfigs.Find(a => a.card_id == normalInfo.index).name_cn;
                break;
            case (int)TaskType.GrowUp:
                taskInfos = AchievementDataMgr.Ins.growupTaskInfos;
                titleText.text = titleList[normalInfo.type - 2];
                break;
            case (int)TaskType.First:
                taskInfos = AchievementDataMgr.Ins.firstTaskInfos;
                titleText.text = titleList[normalInfo.type - 2];
                break;
            case (int)TaskType.Collect:
                taskInfos = AchievementDataMgr.Ins.collectTaskInfos;
                titleText.text = titleList[normalInfo.type - 2];
                break;
        }
        GetMissionList();
        taskList.scrollPane.ScrollTop();
        awardItems = new List<List<TinyItem>>();
        for (int i = 0; i < currentMissonConfigs.Count; i++)
        {
            List<TinyItem> tinyItems = ItemUtil.GetTinyItmeList(currentMissonConfigs[i].award);
            awardItems.Add(tinyItems);
        }
        taskList.SetVirtual();
        taskList.itemRenderer = RenderItem;
        taskList.numItems = currentMissonConfigs.Count;
        getAllBtn.visible = currentTaskInfos[0].state == 1 ? true : false;
        taskList.RefreshVirtualList();
        SetGoodsItemEffect();
    }

    /// <summary>
    /// 获取显示任务
    /// </summary>
    void GetMissionList()
    {
        currentTaskInfos = new List<TaskInfo>();
        currentMissonConfigs = new List<GameMissionConfig>();
        List<GameMissionConfig> missionConfigs;
        int progress;
        float over;
        float total;
        //获取所有任务的起始任务
        switch (normalInfo.type)
        {
            case (int)TaskType.Role:
                missionConfigs = JsonConfig.GameMissionConfigs.FindAll(a => a.task_type == normalInfo.type && a.id / 100000 == normalInfo.index && a.series == 1);
                missionConfigs.AddRange(JsonConfig.GameMissionConfigs.FindAll(a => a.id / 100000 == normalInfo.index && a.series == 0));
                over = taskInfos.Count(a => a.state == 2);
                total = JsonConfig.GameMissionConfigs.Count(a => a.id / AchievementDataMgr.ROLE_TASK_ID == normalInfo.index);
                progress = (int)(over / total * 100);
                progressText.text = "成就进度：" + progress + "%";
                break;
            default:
                missionConfigs = JsonConfig.GameMissionConfigs.FindAll(a => a.task_type == normalInfo.type && a.series == 1);
                missionConfigs.AddRange(JsonConfig.GameMissionConfigs.FindAll(a => a.task_type == normalInfo.type && a.series == 0));
                over = taskInfos.Count(a => a.state == 2);
                total = JsonConfig.GameMissionConfigs.Count(a => a.task_type == normalInfo.type);
                progress = (int)(over / total * 100);
                progressText.text = "成就进度：" + progress + "%";
                break;
        }
        bool isFirst;
        for (int i = 0; i < missionConfigs.Count; i++)
        {
            GameMissionConfig missionConfig = missionConfigs[i];
            isFirst = false;
            //从起始任务往后查，如果已完成则往下查，为可领取或进行中退出循环,所有系列任务仅显示正在进行的任务
            do
            {
                TaskInfo taskInfo = taskInfos.Find(a => a.missionId == missionConfig.id);
                if (taskInfo == null)
                {
                    taskInfo = new TaskInfo()
                    {
                        missionId = missionConfigs[i].id,
                        state = 0,
                        progress = 0,
                    };
                    currentTaskInfos.Add(taskInfo);
                    currentMissonConfigs.Add(missionConfig);

                    isFirst = true;
                }
                else
                {
                    switch (taskInfo.state)
                    {
                        case 0:
                        case 1:
                            currentTaskInfos.Add(taskInfo);
                            currentMissonConfigs.Add(missionConfig);
                            isFirst = true;
                            break;
                        case 2:
                            currentTaskInfos.Add(taskInfo);
                            currentMissonConfigs.Add(missionConfig);
                            missionConfig = JsonConfig.GameMissionConfigs.Find(a => a.id == missionConfig.next);
                            break;
                    }
                }
            } while (missionConfig != null && !isFirst);

        }

        AchievementDataMgr.Ins.Sort(currentMissonConfigs, currentTaskInfos);
    }

    void RenderItem(int index, GObject gObject)
    {
        GComponent gCom = gObject.asCom;
        GTextField titleText = gCom.GetChild("n20").asTextField;
        GTextField desText = gCom.GetChild("n21").asTextField;
        GList awardList = gCom.GetChild("n22").asList;
        GProgressBar gProgress = gCom.GetChild("n26").asProgress;
        Controller taskController = gCom.GetController("c1");
        GameMissionConfig missionConfig = currentMissonConfigs[index];
        taskController.selectedIndex = currentTaskInfos[index].state;

        titleText.text = missionConfig.title;
        desText.text = missionConfig.desc;
        gProgress.max = missionConfig.progress;
        gProgress.value = currentTaskInfos[index].state != 0 ? missionConfig.progress : currentTaskInfos[index].progress;

        awardList.scrollPane.touchEffect = false;
        awardList.SetVirtual();
        awardList.itemRenderer = (int i, GObject gOb) => { RenderAwardItem(i, gOb, index); };
        awardList.numItems = awardItems[index].Count;
        gCom.GetChild("n28").onClick.Set(() => { OnClickGoto(missionConfig.from); });
        gCom.GetChild("n24").onClick.Set(() => { OnClickGet(missionConfig.id); });

    }

    void SetGoodsItemEffect()
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

    void RenderAwardItem(int index, GObject gObject, int itemIndex)
    {
        GComponent gCom = gObject.asCom;
        GTextField numText = gCom.GetChild("n64").asTextField;
        GLoader iconLoader = gCom.GetChild("n62").asLoader;
        TinyItem tinyItem = awardItems[itemIndex][index];
        numText.text = tinyItem.num + "";
        iconLoader.url = tinyItem.url;
        gCom.onClick.Set(() => { OnClickAward(tinyItem); });
    }

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

    void OnClickGet(int id)
    {
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("missionId", id);
        GameMonoBehaviour.Ins.RequestInfoPost<TaskAwardInfo>(NetHeaderConfig.MISSION_GET_AWARD, wWWForm, (TaskAwardInfo info) =>
        {
            RefreshTaskList(info.mission);
            ShowAwards(info);
            Refresh();
        });
    }

    void OnClickGoto(string from)
    {
        JumpMgr.Ins.JumpView(from);
        baseView.OnHideAnimation();
    }

    void OnClickGetAll()
    {
        List<string> str = new List<string>();
        for (int i = 0; i < currentTaskInfos.Count; i++)
        {
            if (currentTaskInfos[i].state != 1)
            {
                break;
            }
            str.Add(currentTaskInfos[i].missionId.ToString());
        }
        string missionIds = string.Join(",", str);
        Debug.LogError("SPS------------- get all mission  " + missionIds);
        if (missionIds.Length == 0)
            return;
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("missionId", missionIds);
        GameMonoBehaviour.Ins.RequestInfoPost<TaskAwardInfo>(NetHeaderConfig.MISSION_GET_AWARD, wWWForm, (TaskAwardInfo info) =>
        {
            RefreshTaskList(info.mission);
            ShowAwards(info);
            Refresh();
        });
    }

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
            GameTool.ShowLevelUpEffect(info.extra[0].level);
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
        TouchScreenView.Ins.ShowPropsTost(props);
        EventMgr.Ins.DispachEvent(EventConfig.REFRESH_USER_TOP_INFO);
        EventMgr.Ins.DispachEvent(EventConfig.REFRESH_ACHIEVEMENT_TOP_INFO);
    }

    void RefreshTaskList(DailyTask dailyTask)
    {
        AchievementDataMgr.Ins.RefreshTask(dailyTask);
        Refresh();
    }

}

using System.Collections.Generic;

public class AchievementDataMgr
{
    static AchievementDataMgr ins;
    public static AchievementDataMgr Ins
    {
        get
        {
            if (ins == null)
            {
                ins = new AchievementDataMgr();
            }
            return ins;
        }
    }
    /// <summary>
    /// 角色任务id起始
    /// </summary>
    public const int ROLE_TASK_ID = 100000;

    public TaskType CurrentTaskType { get; set; }
    public DailyTask dailyTask;
    public List<TaskInfo> growupTaskInfos;
    public List<TaskInfo> firstTaskInfos;
    public List<TaskInfo> roleTaskInfos;
    public List<TaskInfo> collectTaskInfos;

    public Dictionary<int, List<TaskInfo>> roleInfoDic;

    //将任务分为四个类型
    public void GetTask(DailyTask dailyTask)
    {
        this.dailyTask = dailyTask;
        growupTaskInfos = new List<TaskInfo>();
        firstTaskInfos = new List<TaskInfo>();
        roleTaskInfos = new List<TaskInfo>();
        collectTaskInfos = new List<TaskInfo>();
        for (int i = 0; i < dailyTask.tasksValue.Count; i++)
        {
            GameMissionConfig missionConfig = JsonConfig.GameMissionConfigs.Find(a => a.id == dailyTask.tasksValue[i].missionId);
            switch (missionConfig.task_type)
            {
                case (int)TaskType.GrowUp:
                    growupTaskInfos.Add(dailyTask.tasksValue[i]);
                    break;
                case (int)TaskType.First:
                    firstTaskInfos.Add(dailyTask.tasksValue[i]);
                    break;
                case (int)TaskType.Role:
                    roleTaskInfos.Add(dailyTask.tasksValue[i]);
                    break;
                case (int)TaskType.Collect:
                    collectTaskInfos.Add(dailyTask.tasksValue[i]);
                    break;
            }
        }

        string[] doneTask = dailyTask.doneTasks.Split(new char[1] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < doneTask.Length; i++)
        {
            GameMissionConfig missionConfig = JsonConfig.GameMissionConfigs.Find(a => a.id == int.Parse(doneTask[i]));
            TaskInfo taskInfo = new TaskInfo()
            {
                missionId = missionConfig.id,
                progress = missionConfig.progress,
                state = 2,
            };
            switch (missionConfig.task_type)
            {
                case (int)TaskType.GrowUp:
                    AddTask(growupTaskInfos, taskInfo);
                    break;
                case (int)TaskType.First:
                    AddTask(firstTaskInfos, taskInfo);
                    break;
                case (int)TaskType.Role:
                    AddTask(roleTaskInfos, taskInfo);
                    break;
                case (int)TaskType.Collect:
                    AddTask(collectTaskInfos, taskInfo);
                    break;
            }
        }
        GetRoleDic();
    }
    /// <summary>
    /// 排序，可领取->正在进行按完成度百分比排序->已领取
    /// </summary>
    /// <param name="gameMissions"></param>
    /// <param name="taskInfos"></param>
    public void Sort(List<GameMissionConfig> gameMissions, List<TaskInfo> taskInfos)
    {
        int index;
        for (int i = 0; i < gameMissions.Count - 1; i++)
        {
            if (taskInfos[i].state == 1)
                continue;
            index = i;
            for (int j = i + 1; j < gameMissions.Count; j++)
            {
                switch (taskInfos[j].state)
                {
                    case 1:
                        index = j;
                        j = gameMissions.Count;
                        break;
                    case 2:
                        continue;
                    case 0:
                        if (taskInfos[index].state == 2)
                        {
                            index = j;
                        }
                        else if ((float)taskInfos[index].progress / gameMissions[index].progress < (float)taskInfos[j].progress / gameMissions[j].progress)
                        {
                            index = j;
                        }
                        break;
                }
            }
            if (i != index)
            {
                GameMissionConfig gameMission = gameMissions[i];
                gameMissions[i] = gameMissions[index];
                gameMissions[index] = gameMission;
                TaskInfo taskInfo = taskInfos[i];
                taskInfos[i] = taskInfos[index];
                taskInfos[index] = taskInfo;
            }
        }

    }
    //将角色任务分开
    void GetRoleDic()
    {
        roleInfoDic = new Dictionary<int, List<TaskInfo>>();
        for (int i = 0; i < JsonConfig.GameInitCardsConfigs.Count; i++)
        {
            if (JsonConfig.GameInitCardsConfigs[i].type!= 0 && JsonConfig.GameInitCardsConfigs[i].type != 1)
            {
                List<TaskInfo> gameTasks = new List<TaskInfo>();
                roleInfoDic.Add(JsonConfig.GameInitCardsConfigs[i].card_id, gameTasks);
            }
        }
        for (int i = 0; i < roleTaskInfos.Count; i++)
        {
            List<TaskInfo> taskInfos;
            GameMissionConfig missionConfig = JsonConfig.GameMissionConfigs.Find(a => a.id == roleTaskInfos[i].missionId);
            if (roleInfoDic.TryGetValue(missionConfig.id / ROLE_TASK_ID, out taskInfos))
            {
                taskInfos.Add(roleTaskInfos[i]);
            }
            else
            {
                taskInfos = new List<TaskInfo>();
                taskInfos.Add(roleTaskInfos[i]);
                roleInfoDic.Add(missionConfig.id / ROLE_TASK_ID, taskInfos);
            }
        }
    }

    /// <summary>
    /// 领取任务奖励后刷新任务列表
    /// </summary>
    /// <param name="dailyTask"></param>
    public void RefreshTask(DailyTask dailyTask)
    {
        //刷新真正进行列表
        for (int i = 0; i < dailyTask.tasksValue.Count; i++)
        {
            GameMissionConfig missionConfig = JsonConfig.GameMissionConfigs.Find(a => a.id == dailyTask.tasksValue[i].missionId);
            switch (missionConfig.task_type)
            {
                case (int)TaskType.GrowUp:
                    AddTask(growupTaskInfos, dailyTask.tasksValue[i]);
                    break;
                case (int)TaskType.First:
                    AddTask(firstTaskInfos, dailyTask.tasksValue[i]);
                    break;
                case (int)TaskType.Role:
                    AddTask(roleTaskInfos, dailyTask.tasksValue[i]);
                    break;
                case (int)TaskType.Collect:
                    AddTask(collectTaskInfos, dailyTask.tasksValue[i]);
                    break;
            }
        }
        //刷新已完成列表
        string[] doneTask = dailyTask.doneTasks.Split(new char[1] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < doneTask.Length; i++)
        {
            GameMissionConfig missionConfig = JsonConfig.GameMissionConfigs.Find(a => a.id == int.Parse(doneTask[i]));
            TaskInfo taskInfo = new TaskInfo()
            {
                missionId = missionConfig.id,
                progress = missionConfig.progress,
                state = 2,
            };
            switch (missionConfig.task_type)
            {
                case (int)TaskType.GrowUp:
                    AddTask(growupTaskInfos, taskInfo);
                    break;
                case (int)TaskType.First:
                    AddTask(firstTaskInfos, taskInfo);
                    break;
                case (int)TaskType.Role:
                    AddTask(roleTaskInfos, taskInfo);
                    break;
                case (int)TaskType.Collect:
                    AddTask(collectTaskInfos, taskInfo);
                    break;
            }
        }
        GetRoleDic();
    }
    /// <summary>
    /// 增加任务，如果已存在则替换，不存在则插入
    /// </summary>
    /// <param name="taskInfos"></param>
    /// <param name="taskInfo"></param>
    void AddTask(List<TaskInfo> taskInfos, TaskInfo taskInfo)
    {
        TaskInfo info = taskInfos.Find(a => a.missionId == taskInfo.missionId);
        if (info == null)
        {
            taskInfos.Add(taskInfo);
        }
        else
        {
            info.progress = taskInfo.progress;
            info.state = taskInfo.state;
        }
    }

}

public enum TaskType
{
    //每日任务
    Daily = 1,
    //成长之星
    GrowUp,
    //首次体验
    First,
    //角色专辑
    Role,
    //收集之旅
    Collect,
}

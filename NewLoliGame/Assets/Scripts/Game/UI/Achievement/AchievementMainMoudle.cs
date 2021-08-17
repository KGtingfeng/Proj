using System.Collections.Generic;
using FairyGUI;
using System.Linq;

public class AchievementMainMoudle : BaseMoudle
{
    //成长之星
    GComponent growUp;
    //首次体验
    GComponent first;
    //角色专辑
    GComponent role;
    //收集之旅
    GComponent collect;

    Controller growUpController;
    Controller firstController;
    Controller roleController;
    Controller collectController;

    GTextField growText;
    GTextField firstText;
    GTextField roleText;
    GTextField collectText;
    GList buttonList;

    public override void InitMoudle(GComponent gComponent, int controllerIndex)
    {
        base.InitMoudle(gComponent, controllerIndex);
        InitUI();
        InitEvent();
    }

    public override void InitUI()
    {
        base.InitUI();
        buttonList = SearchChild("n9").asList;
        growUp = buttonList.GetChildAt(0).asCom;
        first = buttonList.GetChildAt(1).asCom;
        role = buttonList.GetChildAt(2).asCom;
        collect = buttonList.GetChildAt(3).asCom;
        growUpController = growUp.GetController("c1");
        firstController = first.GetController("c1");
        roleController = role.GetController("c1");
        collectController = collect.GetController("c1");
        growText = growUp.GetChild("n3").asTextField;
        firstText = first.GetChild("n3").asTextField;
        roleText = role.GetChild("n3").asTextField;
        collectText = collect.GetChild("n3").asTextField;
    }

    public override void InitEvent()
    {
        base.InitEvent();

        growUp.onClick.Set(OnClickGrowUp);
        first.onClick.Set(OnClickFirst);
        role.onClick.Set(OnClickRole);
        collect.onClick.Set(OnClickCollect);

    }

    public override void InitData()
    {
        base.InitData();

        growUpController.selectedIndex = SetRedPoint(AchievementDataMgr.Ins.growupTaskInfos);
        firstController.selectedIndex = SetRedPoint(AchievementDataMgr.Ins.firstTaskInfos);
        roleController.selectedIndex = SetRedPoint(AchievementDataMgr.Ins.roleTaskInfos);
        collectController.selectedIndex = SetRedPoint(AchievementDataMgr.Ins.collectTaskInfos);
        growText.text = Setprogress(AchievementDataMgr.Ins.growupTaskInfos, (int)TaskType.GrowUp) + "%";
        firstText.text = Setprogress(AchievementDataMgr.Ins.firstTaskInfos, (int)TaskType.First) + "%";
        roleText.text = Setprogress(AchievementDataMgr.Ins.roleTaskInfos, (int)TaskType.Role) + "%";
        collectText.text = Setprogress(AchievementDataMgr.Ins.collectTaskInfos, (int)TaskType.Collect) + "%";
    }

    void OnClickGrowUp()
    {
        AchievementDataMgr.Ins.CurrentTaskType = TaskType.GrowUp;
        NormalInfo normalInfo = new NormalInfo()
        {
            type = (int)TaskType.GrowUp,
        };
        baseView.GoToMoudle<AchievementTaskMoudle, NormalInfo>((int)AchievementView.MoudleType.Task, normalInfo);

    }

    void OnClickFirst()
    {
        AchievementDataMgr.Ins.CurrentTaskType = TaskType.First;
        NormalInfo normalInfo = new NormalInfo()
        {
            type = (int)TaskType.First,
        };
        baseView.GoToMoudle<AchievementTaskMoudle, NormalInfo>((int)AchievementView.MoudleType.Task, normalInfo);

    }

    void OnClickRole()
    {
        baseView.GoToMoudle<AchievementRoleMoudle>((int)AchievementView.MoudleType.Role);
    }

    void OnClickCollect()
    {
        AchievementDataMgr.Ins.CurrentTaskType = TaskType.Collect;
        NormalInfo normalInfo = new NormalInfo()
        {
            type = (int)TaskType.Collect,
        };
        baseView.GoToMoudle<AchievementTaskMoudle, NormalInfo>((int)AchievementView.MoudleType.Task, normalInfo);

    }

    int SetRedPoint(List<TaskInfo> taskInfos)
    {
        if (taskInfos == null)
            return 0;
        for (int i = 0; i < taskInfos.Count; i++)
        {
            if (taskInfos[i].state == 1)
                return 1;
        }
        return 0;
    }

    int Setprogress(List<TaskInfo> taskInfos, int type)
    {
        float over = taskInfos.Count(a => a.state == 2);
        float total = JsonConfig.GameMissionConfigs.Count(a => a.task_type == type);
        int progress = (int)(over / total * 100);
        return progress;
    }

}

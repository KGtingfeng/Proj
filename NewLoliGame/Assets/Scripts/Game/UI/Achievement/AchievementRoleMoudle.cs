using System.Collections.Generic;
using FairyGUI;
using UnityEngine;
using System.Linq;

public class AchievementRoleMoudle : BaseMoudle
{

    GList roleList;
    List<GameInitCardsConfig> roleConfig = new List<GameInitCardsConfig>();

    GTextField titleText;
    public override void InitMoudle(GComponent gComponent, int controllerIndex)
    {
        base.InitMoudle(gComponent, controllerIndex);
        InitUI();
        InitEvent();
    }

    public override void InitUI()
    {
        base.InitUI();
        roleList = SearchChild("n16").asList;
        titleText = SearchChild("n14").asTextField;


        for (int i = 0; i < JsonConfig.GameInitCardsConfigs.Count; i++)
        {
            if (JsonConfig.GameInitCardsConfigs[i].type != 0&& JsonConfig.GameInitCardsConfigs[i].type != 1)
                roleConfig.Add(JsonConfig.GameInitCardsConfigs[i]);
        }
    }

    public override void InitData()
    {
        base.InitData();

        roleList.onClickItem.Set(OnClickItem);
        roleList.itemRenderer = RenderItem;
        roleList.numItems = roleConfig.Count;
        titleText.text = "角色专题";
        SetGoodsItemEffect();
    }

    void RenderItem(int index, GObject gObject)
    {
        GComponent gCom = gObject.asCom;
        GLoader iconLoader = gCom.GetChild("n5").asCom.GetChild("n5").asLoader;
        GTextField progressText = gCom.GetChild("n17").asTextField;
        Controller roleController = gCom.GetController("c1");
        List<TaskInfo> taskInfos = AchievementDataMgr.Ins.roleInfoDic[roleConfig[index].card_id];
        roleController.selectedIndex = 0;
        if (taskInfos.Count > 0)
        {
            for (int i = 0; i < taskInfos.Count; i++)
            {
                if (taskInfos[i].state == 1)
                {
                    roleController.selectedIndex = 1;
                    break;
                }
            }
        }
        iconLoader.url = UrlUtil.GetStoryHeadIconUrl(roleConfig[index].card_id);
        float over = taskInfos.Count(a => a.state == 2);
        float total = JsonConfig.GameMissionConfigs.FindAll(a => a.id / 100000 == roleConfig[index].card_id).Count;
        int progress = (int)(over / total * 100);
        progressText.text = progress + "%";
    }

    void OnClickItem(EventContext context)
    {
        //当前可视item中的index
        int index = roleList.GetChildIndex((GObject)context.data);
        //真实index
        int realIndex = roleList.ChildIndexToItemIndex(index);
        AchievementDataMgr.Ins.CurrentTaskType = TaskType.Role;
        NormalInfo normalInfo = new NormalInfo()
        {
            type = (int)TaskType.Role,
            index = roleConfig[realIndex].card_id,
        };
        baseView.GoToMoudle<AchievementTaskMoudle, NormalInfo>((int)AchievementView.MoudleType.Task, normalInfo);

    }


    void SetGoodsItemEffect()
    {
        Vector3 pos = new Vector3();
        for (int i = 0; i < roleList.numChildren; i++)
        {
            GObject item = roleList.GetChildAt(i);

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
            pos.x = index % 2 == 0 ? 0f : 332f;
            pos.y = index / 2 * 336f;
        }
        return pos;
    }
}

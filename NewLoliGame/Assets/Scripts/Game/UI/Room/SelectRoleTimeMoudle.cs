using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using System;

public class SelectRoleTimeMoudle : BaseMoudle
{
    public static SelectRoleTimeMoudle ins;
    enum Type
    {
        All = 0,
        Fairy,
        Human,
    }
    GList roleList;
    GTextField titleText;
    GTextField numText;
    GList btnList;
    public List<Role> roles;

    List<int> noImage = new List<int>() { 28, 29, 30, 31, 32, 33, 34 };

    int selectType = 0;
    List<TimeChart> timeCharts;
    Dictionary<int, List<GameMomentConfig>> momentDic = new Dictionary<int, List<GameMomentConfig>>();
    //仙子、人类
    List<Role> fairyGameInitCardsConfigs;
    List<Role> humanGameInitCardsConfigs;
    //获取当前展示的角色信息
    List<Role> currentRoleList;
    public override void InitMoudle(GComponent gComponent, int controllerIndex)
    {
        base.InitMoudle(gComponent, controllerIndex);
        InitUI();
        InitEvent();
    }

    public override void InitUI()
    {
        base.InitUI();
        roleList = SearchChild("n32").asList;
        titleText = SearchChild("n26").asTextField;
        numText = SearchChild("n27").asTextField;
        btnList = SearchChild("n31").asList;
        ins = this;
    }

    public override void InitEvent()
    {
        base.InitEvent();
        btnList.onClickItem.Set(() =>
        {
            if (selectType != btnList.selectedIndex)
            {
                selectType = btnList.selectedIndex;
                GetCategoricalData(selectType);
                RefreshList();
            }
        });
    }

    public override void InitData<D>(D data)
    {
        base.InitData(data);
        timeCharts = data as List<TimeChart>;
        roleList.onClickItem.Set(OnClickItem);
        roleList.SetVirtual();
        roleList.itemRenderer = RenderItem;
        titleText.text = "时刻相册";
        numText.text = "已获得" + timeCharts.Count + "/" + JsonConfig.GameMomentConfigs.Count;
        if (GameData.OwnRoleList == null)
        {
            RequestRoleListInfo();
            return;
        }
        roles = new List<Role>(GameData.OwnRoleList);
        InitMoment();
        GetCategoricalData(0);
        btnList.selectedIndex = 0;
        RefreshList();

        if (GameData.isOpenGuider)
        {


            //StoryCacheMgr.storyCacheMgr.GetStoryGameSave("Newbie", GameGuideConfig.TYPE_TIME, (storyGameSave) =>
            //{
            //    if (storyGameSave.IsDefault)
            //    {
            //        GameData.guiderCurrent = GuiderInfoLinked.GetCurrGuiderInfo(17, 1);
            //        UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);
            //    }
            //});


            /*
           WWWForm wWWForm = new WWWForm();
           wWWForm.AddField("nodeId", GameGuideConfig.TYPE_TIME);
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
                   GameData.guiderCurrent = GuiderInfoLinked.GetCurrGuiderInfo(17, 1);
                   UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);
               }

           }, false);    */
        }
    }

    public override void InitData()
    {
        base.InitData();
        titleText.text = "时刻相册";
        RefreshList();


    }

    void RefreshList()
    {
        roleList.numItems = currentRoleList.Count;
        roleList.scrollPane.ScrollTop();
    }


    void RenderItem(int index, GObject gObject)
    {
        GComponent gComponent = gObject.asCom;
        Controller controller = gComponent.GetController("c1");
        GLoader gLoader = gComponent.GetChild("n11").asLoader;

        GTextField nameText;
        GTextField getText;
        gLoader.url = UrlUtil.GetMomentTimeRole(currentRoleList[index].id);

        controller.selectedIndex = index % 2;
        if (index % 2 == 0)
        {
            gLoader.scaleX = 1;
            nameText = gComponent.GetChild("n12").asTextField;
            getText = gComponent.GetChild("n13").asTextField;
        }
        else
        {
            gLoader.scaleX = -1;
            nameText = gComponent.GetChild("n14").asTextField;
            getText = gComponent.GetChild("n15").asTextField;
        }
        GameInitCardsConfig cardsConfig = JsonConfig.GameInitCardsConfigs.Find(a => a.card_id == currentRoleList[index].id);
        nameText.text = cardsConfig.name_cn;
        getText.text = "已拥有时刻数：" + momentDic[currentRoleList[index].id].Count;

    }

    void OnClickItem(EventContext context)
    {
        int index = roleList.GetChildIndex((GObject)context.data);
        //真实index
        int realIndex = roleList.ChildIndexToItemIndex(index);
        RoleTime roleTime = new RoleTime()
        {
            actorId = currentRoleList[realIndex].id,
            configs = momentDic[currentRoleList[realIndex].id],
        };
        baseView.GoToMoudle<RoleTimeMoudle, RoleTime>((int)RoomView.MoudleType.Time, roleTime);
    }

    public void NewbieGotoTime()
    {
        RoleTime roleTime = new RoleTime()
        {
            actorId = GameGuideConfig.GuideActor,
            configs = momentDic[GameGuideConfig.GuideActor],
        };
        baseView.GoToMoudle<RoleTimeMoudle, RoleTime>((int)RoomView.MoudleType.Time, roleTime);
    }

    void RequestRoleListInfo()
    {
        WWWForm wWWForm = new WWWForm();
        GameMonoBehaviour.Ins.RequestInfoPostList<Role>(NetHeaderConfig.ACTOR_LIST, wWWForm, () =>
        {
            btnList.selectedIndex = 0;
            roles = new List<Role>(GameData.OwnRoleList);
            InitMoment();

            GetCategoricalData(0);
            RefreshList();
        });
    }

    void InitMoment()
    {
        roles.Sort(delegate (Role roleA, Role roleB)
        {
            return roleA.id.CompareTo(roleB.id);
        });

        foreach (var role in roles)
        {
            if (momentDic.ContainsKey(role.id))
            {
                momentDic[role.id].Clear();
            }
            else
            {
                List<GameMomentConfig> configs = new List<GameMomentConfig>();
                momentDic.Add(role.id, configs);
            }
        }
        foreach (var time in timeCharts)
        {
            GameMomentConfig config = JsonConfig.GameMomentConfigs.Find(a => a.moment_id == time.moment_id);
            if (momentDic.ContainsKey(config.actor_id))
                momentDic[config.actor_id].Add(config);
        }
    }



    void GetCategoricalData(int selectIndex)
    {
        fairyGameInitCardsConfigs = new List<Role>();
        humanGameInitCardsConfigs = new List<Role>();

        for (int i = roles.Count - 1; i >= 0; i--)
        {
            GameInitCardsConfig cardsConfig = JsonConfig.GameInitCardsConfigs.Find(a => a.card_id == roles[i].id);
            if (cardsConfig.type == 1 || noImage.Contains(cardsConfig.card_id))
            {
                roles.RemoveAt(i);
                continue;
            }
            if (cardsConfig.status)
            {
                fairyGameInitCardsConfigs.Add(roles[i]);
            }
            else
            {
                humanGameInitCardsConfigs.Add(roles[i]);
            }

        }
        switch (selectIndex)
        {
            case (int)Type.All:
                currentRoleList = roles;
                break;
            case (int)Type.Human:
                currentRoleList = humanGameInitCardsConfigs;
                break;
            case (int)Type.Fairy:
                currentRoleList = fairyGameInitCardsConfigs;
                break;
        }

    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using System;


[ViewAttr("Game/UI/P_Rank", "P_Rank", "Rank")]
public class RankView : BaseView
{

    public enum MoudleType
    {
        Main,
        Role,
        Rank,
        Attribute,
    };

    readonly Dictionary<MoudleType, string> moudleNamePairs = new Dictionary<MoudleType, string>()
    {

    };


    GLoader bgLoader;
    List<GComponent> rankList;
    List<PlayerRankInfo> topThreeInfo;
    List<Vector3> topThreeFramePos = new List<Vector3>()
    {
       new Vector3(135,135,1000),
       new Vector3(135,133,1000),
       new Vector3(136,134,1000),
    };

    public override void InitUI()
    {
        base.InitUI();
        bgLoader = SearchChild("n0").asLoader;
        controller = ui.GetController("c1");
        GetTopThreeList();
        InitEvent();
    }

    public override void InitEvent()
    {
        base.InitEvent();
        SearchChild("n2").onClick.Set(OnClickBack);



        EventMgr.Ins.RegisterEvent<RankTopThree>(EventConfig.SET_TOP_THREE, SetTopThree);
    }

    public override void InitData()
    {
        OnShowAnimation();

        base.InitData();
        bgLoader.url = UrlUtil.GetBgUrl("Rank", "bg");
        GoToMoudle<RankMainMoudle>((int)MoudleType.Main);
        if (GameData.isOpenGuider)
        {
                   
            //StoryCacheMgr.storyCacheMgr.GetStoryGameSave("Newbie", GameGuideConfig.TYPE_RANK, (storyGameSave) =>
            //{
            //    if (storyGameSave.IsDefault)
            //    {
            //        GameData.guiderCurrent = GuiderInfoLinked.GetCurrGuiderInfo(GameGuideConfig.TYPE_RANK, 1);
            //        UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);
            //    }
            //});


            /*
          WWWForm wWWForm = new WWWForm();
          wWWForm.AddField("nodeId", GameGuideConfig.TYPE_RANK);
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
                  GameData.guiderCurrent = GuiderInfoLinked.GetCurrGuiderInfo(GameGuideConfig.TYPE_RANK, 1);
                  UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);
              }

          }, false); */
        }
    }

    public override void InitData<D>(D data)
    {
        OnShowAnimation();
        base.InitData(data);

        bgLoader.url = UrlUtil.GetBgUrl("Rank", "bg");
        GoToMoudle<RankMainMoudle, NormalInfo>((int)MoudleType.Main, data as NormalInfo);
    }


    void GetTopThreeList()
    {
        rankList = new List<GComponent>();
        for (int i = 3; i <= 5; i++)
        {
            GComponent gCom = SearchChild("n" + i).asCom;
            GComponent headCom = gCom.GetChild("n26").asCom;
            int index = i - 3;
            headCom.onClick.Set(() => { OnClickTopThree(index); });
            rankList.Add(gCom);
        }
    }

    //设置前三
    void SetTopThree(RankTopThree topThree)
    {
        topThreeInfo = topThree.top_three;
        for (int i = 0; i < rankList.Count; i++)
        {
            GComponent gCom = rankList[i];
            GTextField nameText = gCom.GetChild("n22").asTextField;
            GTextField titleText = gCom.GetChild("n23").asTextField;
            GTextField valText = gCom.GetChild("n24").asTextField;
            GLoader iconLoader = gCom.GetChild("n26").asCom.GetChild("n0").asLoader;
            Controller controller = gCom.GetController("c1");
            GLoader frameLoader = gCom.GetChild("n26").asCom.GetChild("n1").asLoader;
            GGraph frameGraph = gCom.GetChild("n26").asCom.GetChild("n2").asGraph;
            GImage gImage = gCom.GetChild("n10").asImage;
            GLoader titleLoader = gCom.GetChild("n28").asCom.GetChild("n39").asLoader;
            GGraph titleGraph = gCom.GetChild("n28").asCom.GetChild("n40").asGraph;

            if (topThree.top_three.Count > i)
            {
                controller.selectedIndex = 0;
                nameText.text = topThree.top_three[i].name;
                valText.text = RankDataMgr.Instance.typeName[(int)topThree.type] + ":" + topThree.top_three[i].val;
                iconLoader.url = UrlUtil.GetRoleHeadIconUrl(topThree.top_three[i].avatar);

                GameAvatarFrameConfig frameConfig = JsonConfig.GameAvatarFrameConfigs.Find(a => a.id == topThree.top_three[i].frame);
                FXMgr.SetFrame(frameGraph, frameLoader, frameConfig.source_id, 100, topThreeFramePos[i]);


                if (topThree.top_three[i].title != "0")
                {
                    GameTitleConfig titleConfig = JsonConfig.GameTitleConfigs.Find(a => a.id == int.Parse(topThree.top_three[i].title));
                    FXMgr.SetTitle(titleGraph, titleLoader, titleConfig.level, 70, new Vector3(123, 20, 1000));
                    titleText.text = titleConfig.name_cn;
                }
                else
                {
                    titleText.text = "暂无";
                    FXMgr.SetTitle(titleGraph, titleLoader, 1, 70, new Vector3(123, 20, 1000));
                }
                gImage.visible = false;
            }
            else
            {
                frameGraph.visible = false;
                frameLoader.visible = false;
                controller.selectedIndex = 1;
                nameText.text = "暂无";
                titleText.text = "暂无";
                valText.text = "0";
                FXMgr.SetTitle(titleGraph, titleLoader, 1, 70, new Vector3(123, 22, 1000));
                gImage.visible = true;

            }
        }
    }

    //返回
    void OnClickBack()
    {
        switch (controller.selectedIndex)
        {
            case (int)MoudleType.Main:
                UIMgr.Ins.showMainView<RankView>();
                break;
            case (int)MoudleType.Role:
                GoToMoudle<RankMainMoudle>((int)MoudleType.Main);
                break;
            case (int)MoudleType.Rank:
                switch (RankDataMgr.Instance.RankType)
                {
                    case RankType.Favor:
                        NormalInfo favorInfo = new NormalInfo
                        {
                            index = (int)RankType.Favor,
                        };
                        GoToMoudle<RankRoleMoudle, NormalInfo>((int)MoudleType.Role, favorInfo);

                        break;
                    case RankType.Time:
                        NormalInfo timeInfo = new NormalInfo
                        {
                            index = (int)RankType.Time,
                        };
                        GoToMoudle<RankRoleMoudle, NormalInfo>((int)MoudleType.Role, timeInfo);

                        break;
                    case RankType.Charm:
                    case RankType.Eve:
                    case RankType.Intell:
                    case RankType.Mana:
                    case RankType.Attr:
                        GoToMoudle<RankAttributeMoudle>((int)MoudleType.Attribute);

                        break;
                }
                break;
            case (int)MoudleType.Attribute:
                GoToMoudle<RankMainMoudle>((int)MoudleType.Main);

                break;
        }
    }

    void OnClickTopThree(int index)
    {
        if (index < topThreeInfo.Count)
        {
            WWWForm wWWForm = new WWWForm();
            wWWForm.AddField("type", topThreeInfo[index].id);
            GameMonoBehaviour.Ins.RequestInfoPost<PlayerInfo>(NetHeaderConfig.RANK_PLAYER_ID, wWWForm, (PlayerInfo rankInfo) =>
            {
                rankInfo.player.isApply = topThreeInfo[index].applied ? 1 : 0;
                rankInfo.player.playerId = topThreeInfo[index].id;
                UIMgr.Ins.showNextPopupView<PlayerInfoView, PlayerInfo>(rankInfo);
            });
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
}

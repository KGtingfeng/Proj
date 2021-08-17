using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;
using System.Linq;

public class StorySelectMoudle : BaseMoudle
{
    static StorySelectMoudle ins;
    public static StorySelectMoudle Ins
    {
        get
        {
            return ins;
        }
    }


    List<GameStoryConfig> gameStoryConfigs
    {
        get { return JsonConfig.GameStoryConfigs; }
    }

    //身份
    enum IDENTITY
    {
        All = 0,
        Human,
        Fairy,
    }

    GLoader headLoader;
    GLoader bgLoader;
    
    Dictionary<int, List<int>> endingDic = new Dictionary<int, List<int>>();

    GList loopList;
    GList selectList;
    public override void InitMoudle(GComponent gComponent, int controllerIndex)
    {
        base.InitMoudle(gComponent, controllerIndex);
        loopList = SearchChild("n13").asList;
        selectList = SearchChild("n23").asList;
        bgLoader = SearchChild("n31").asLoader;

        InitData();
        InitEvent();
        ins = this;
    }


    public override void InitData<D>(D data)
    {
        base.InitData(data);
        StoryView.view.GetTopText();
        QueryGameStoryConfigs(0);
        InitStoryListItem();
        bgLoader.url = UrlUtil.GetCommonBgUrl("story_BG");
        selectList.selectedIndex = 0;
        selectList.onClickItem.Set(() =>
        {
            if (currentSelectIndex != selectList.selectedIndex)
            {
                QueryGameStoryConfigs(selectList.selectedIndex);
                InitStoryListItem();
            }
        });

        baseView.GoToMoudle<StorySelectChapterMoudle>((int)StoryView.MoudleType.SelectChapter);

    }

    public override void InitData()
    {
        QueryGameStoryConfigs(0);
        InitStoryListItem();
        bgLoader.url = UrlUtil.GetCommonBgUrl("story_BG");
  
        selectList.selectedIndex = 0;
        selectList.onClickItem.Set(() =>
        {
            if (currentSelectIndex != selectList.selectedIndex)
            {
                QueryGameStoryConfigs(selectList.selectedIndex);
                InitStoryListItem();
            }
        });
    }
   

    public override void InitEvent()
    {
        
    }

    int currentSelectIndex;
    void InitStoryListItem()
    {
        loopList.RemoveCacheItem();
        loopList.itemRenderer = RenderListItem;
        if (GameData.isGuider)
        {
            loopList.numItems = 1;
        }
        else
        {
            loopList.numItems = current.Count;
        }
        currentSelectIndex = selectList.selectedIndex;
    }

    List<GameStoryConfig> current;
    List<GameStoryConfig> fairyGameStoryConfigs;
    List<GameStoryConfig> humanGameStoryConfigs;
    void QueryGameStoryConfigs(int selectIndex)
    {
        //分那女
        if (fairyGameStoryConfigs == null)
        {
            fairyGameStoryConfigs = new List<GameStoryConfig>();
            humanGameStoryConfigs = new List<GameStoryConfig>();

            foreach (GameStoryConfig item in gameStoryConfigs)
            {
                GameInitCardsConfig gameInitCardsConfig = JsonConfig.GameInitCardsConfigs.FirstOrDefault(a => a.card_id == item.actor_id);
                if (gameInitCardsConfig != null)
                {
                    if (gameInitCardsConfig.status)
                    {
                        fairyGameStoryConfigs.Add(item);
                    }
                    else
                    {
                        humanGameStoryConfigs.Add(item);
                    }
                }

                QueryEndingDic(item);
            }
        }

        switch (selectIndex)
        {
            //所有
            case (int)IDENTITY.All:
                current = gameStoryConfigs;
                break;
            //人类
            case (int)IDENTITY.Human:
                current = humanGameStoryConfigs;
                break;
            //仙子
            case (int)IDENTITY.Fairy:
                current = fairyGameStoryConfigs;
                break;
            default:
                break;
        }


    }

    void QueryEndingDic(GameStoryConfig item)
    {
        List<GameChapterConfig> chapterConfigs = JsonConfig.GameChapterConfigs.FindAll(a => a.actor_id == item.actor_id);

        string[] nodes = chapterConfigs[chapterConfigs.Count - 1].all_node.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
        foreach (string node in nodes)
        {
            GameNodeConfig gameNodeConfig = JsonConfig.GameNodeConfigs.Find(a => a.id == int.Parse(node));
            GamePointConfig gamePointConfig = JsonConfig.GamePointConfigs.Find(a => a.id == gameNodeConfig.point_id);
            if (gamePointConfig.point1 == 0)
            {
                List<int> ending;
                if (!endingDic.ContainsKey(item.actor_id))
                {
                    ending = new List<int>();
                    endingDic.Add(item.actor_id, ending);
                }
                else
                {
                    ending = endingDic[item.actor_id];
                }
                ending.Add(gameNodeConfig.id);
            }
        }
    }


    void RenderListItem(int index, GObject obj)
    {
        obj.alpha = 0;
        GameStoryConfig gameStoryConfig = current[index];
        GComponent gComponent = obj.asCom;
        GTextField gTextField = gComponent.GetChild("n8").asTextField;
        GTextField progressTextField = gComponent.GetChild("n12").asTextField;
        GTextField favorText = gComponent.GetChild("n11").asTextField;
        GTextField endingText = gComponent.GetChild("n9").asTextField;


        headLoader = gComponent.GetChild("n5").asLoader;
        headLoader.url = UrlUtil.GetStoryHeadIconUrl(current[index].actor_id);
        //name
        GameInitCardsConfig gameInitCardsConfig = DataUtil.GetGameInitCard(gameStoryConfig.actor_id);
        if (gameInitCardsConfig != null)
        {
            gComponent.GetChild("n10").text = gameInitCardsConfig.name_cn;
        }

        endingText.text = "结局收集：" + QueryGetEnding(gameInitCardsConfig.card_id) + "/" + endingDic[gameInitCardsConfig.card_id].Count;

        //总章节数
        int chapterCount = DataUtil.GetChapters(gameStoryConfig.actor_id).Count;
        PlayerStoryInfo playerStoryInfo = StoryDataMgr.ins.QueryStoryInfo(gameStoryConfig.actor_id);

        Controller itemController = gComponent.GetController("c1");
        Controller redpointController = gComponent.GetChild("n13").asCom.GetController("c1");
        if (playerStoryInfo != null)
        {
            switch (playerStoryInfo.story_status)
            {
                case PlayerStoryInfo.TYPE_PROCEING:
                case PlayerStoryInfo.TYPE_RESTART:
                    {
                        itemController.selectedIndex = 0;
                        IntoChapter(gameStoryConfig, gComponent, "n13");
                        float chapterId = StoryDataMgr.ins.QueryStoryRealChapter(playerStoryInfo.chapter_id, gameStoryConfig.actor_id);
                        gTextField.text = chapterId + "/" + chapterCount + "章";
                        int process = (int)(chapterId / chapterCount * 100);
                        progressTextField.text = "剧情进度:" + process + "%";
                    }
                    break;
                case PlayerStoryInfo.TYPE_OVER:
                    {
                        itemController.selectedIndex = 1;
                        IntoChapter(gameStoryConfig, gComponent, "n19");
                        //reset
                        GameConsumeConfig gameConsumeConfig = DataUtil.GetConsumeConfig(GameConsumeConfig.STORY_RESET_TYPE);
                        if (gameConsumeConfig != null)
                        {
                            TinyItem tinyItem = ItemUtil.GetTinyItem(gameConsumeConfig.pay);
                            gComponent.GetChild("n17").asLoader.url = CommonUrlConfig.GetConsumeItemUrl((TypeConfig.Consume)tinyItem.type);
                            gComponent.GetChild("n18").text = tinyItem.num + "";
                        }
                        gComponent.GetChild("n14").onClick.Set(() =>
                        {
                            RequestResetRoleStory(gameStoryConfig.actor_id, gameConsumeConfig.id);
                        });
                        gTextField.text = chapterCount + "/" + chapterCount + "章";
                        progressTextField.text = "剧情进度:" + "100%";
                    }
                    break;
                default:
                    //未获得角色
                    itemController.selectedIndex = 2;
                    break;
            }
            favorText.text = "好感度：" + playerStoryInfo.extra.favour;
        }
        else
        {
            itemController.selectedIndex = 2;
            IntoChapter(gameStoryConfig, gComponent, "n13");
            gTextField.text = 0 + "/" + chapterCount + "章";
            progressTextField.text = "剧情进度:" + "0%";
            favorText.text = "好感度：0";
            gComponent.GetChild("n15").onClick.Set(() =>
            {
                RequestGetRole(gameStoryConfig.actor_id);
            });
        }
        redpointController.selectedIndex = RedpointMgr.Ins.storyRedpoint.Contains(gameStoryConfig.actor_id) ? 1 : 0;
        SetStoryItemEffect(index, obj);
    }

    /// <summary>
    /// 设置Item动态效果
    /// </summary>
    /// <param name="index"></param>
    /// <param name="obj"></param>
    void SetStoryItemEffect(int index, GObject obj)
    {
        Vector3 pos = new Vector3();
        pos = obj.position;
        pos.y = index * 222f;
        obj.SetPosition(pos.x, pos.y + 100, pos.z);
        float time = index * 0.2f;
        obj.TweenMoveY(pos.y, (time + 0.3f)).SetEase(EaseType.CubicOut).OnStart(() =>
        {
            obj.TweenFade(1, (time + 0.3f)).SetEase(EaseType.QuadOut);
        });
    }

    int QueryGetEnding(int actorId)
    {
        List<int> ending = endingDic[actorId];
        List<PlayerStoryInfo> infos = StoryDataMgr.ins.storyInfos.FindAll(a => a.actor_id == actorId);
        if (infos.Count == 0)
            return 0;
        if (string.IsNullOrEmpty(infos[infos.Count - 1].end_node))
            return 0;
        string[] nodes = infos[infos.Count - 1].end_node.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);

        return nodes.Length;

    }


    void IntoChapter(GameStoryConfig gameStoryConfig, GComponent gComponent, string childName)
    {
        gComponent.GetChild(childName).onClick.Set(() =>
        {
            GotoChapter(gameStoryConfig);
        });
    }

    public void NewbieGotoXinling()
    {

        GameStoryConfig gameStoryConfig = JsonConfig.GameStoryConfigs.Find(a => a.actor_id == GameGuideConfig.GuideActor);
        StoryDataMgr.ins.InitRoleStoryInfo(GameGuideConfig.GuideActor);

        GotoChapter(gameStoryConfig);

    }

    void GotoChapter(GameStoryConfig gameStoryConfig)
    {
        StoryDataMgr.ins.InitRoleStoryInfo(gameStoryConfig.actor_id);

        NormalInfo normalInfo = new NormalInfo();
        normalInfo.index = gameStoryConfig.actor_id;
        EventMgr.Ins.DispachEvent(EventConfig.STORY_INTO_CHAPTER, normalInfo);
        if (RedpointMgr.Ins.storyRedpoint.Contains(gameStoryConfig.actor_id))
        {
            RedpointMgr.Ins.storyRedpoint.Remove(gameStoryConfig.actor_id);
        }
    }


    void RequestResetRoleStory(int actorId, int id)
    {
        System.Action callBack = () =>
        {
            InitData();
            EventMgr.Ins.DispachEvent(EventConfig.REFRESH_PLAYER_PROPERTY_INFO);
        };
        WWWForm wWForm = new WWWForm();
        wWForm.AddField("actorId", actorId);
        wWForm.AddField("consumeId", id);
        GameMonoBehaviour.Ins.RequestInfoPostList<PlayerStoryInfo>(NetHeaderConfig.STROY_RESET, wWForm, callBack);

    }

    void RequestGetRole(int actorId)
    {
        System.Action questCallback = () =>
        {
            WWWForm wWWForm = new WWWForm();
            wWWForm.AddField("actorId", actorId);
            GameMonoBehaviour.Ins.RequestInfoPostList<Role>(NetHeaderConfig.ACTOR_BUY, wWWForm, CallBackGetRole);
        };
        Extrand extrand = new Extrand();
        extrand.key = actorId.ToString();
        extrand.callBack = questCallback;

        UIMgr.Ins.showNextPopupView<ContactTipsView, Extrand>(extrand);
    }

    void CallBackGetRole()
    {
        
        WWWForm wWWForm = new WWWForm();
        GameMonoBehaviour.Ins.RequestInfoPostList<PlayerStoryInfo>(NetHeaderConfig.STORY_GET_SOTRY_INFO, wWWForm, CallBack);
    }

    void CallBack()
    {
        QueryGameStoryConfigs(0);
        InitStoryListItem();
    }

    public void NewbieOnhide()
    {
        StoryDataMgr.ins.Dispose();
        UIMgr.Ins.showMainView<StoryView>();
    }
}

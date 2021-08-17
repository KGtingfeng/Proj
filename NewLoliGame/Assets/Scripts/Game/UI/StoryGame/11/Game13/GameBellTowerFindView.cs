using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

[ViewAttr("Game/UI/Y_Game13", "Y_Game13", "Game")]
public class GameBellTowerFindView : BaseGameView
{

    GLoader bgLoader;
    GComponent sceneCom;
    GComponent subCom;
    GComponent bottomSubCom;
    ScrollPane scrollPane;
    List<GComponent> suipianList;
    List<int> getList = new List<int>();
    List<int> bottom = new List<int>() { 15, 1, 4, 8, 12, 20, 3 };
    List<GComponent> bottomComs = new List<GComponent>();
    AudioClip putDownAudio;

    List<Vector2> tipsPos = new List<Vector2>()
    {
        new Vector2(91,52),
        new Vector2(34,54),
        new Vector2(15,10),
        new Vector2(73,47),
        new Vector2(42,22),
        new Vector2(47,33),
        new Vector2(29,27),
    };

    List<int> ruins = new List<int>() { 7, 5, 19, 15, 21, 11, 16, 10 };
    List<int> xila = new List<int>() { 9, 13, 6, 18, 8 };
    List<int> word = new List<int>() { 14 };

    List<int> ruinsPoint = new List<int>() { 461, 462, 463, 464, 465 };
    List<int> xilaPoint = new List<int>() { 203, 258, 263, 358, 365 };
    List<int> wordPoint = new List<int>() { 215, 239, 281, 305, 333 };

    public override void InitUI()
    {
        base.InitUI();
        sceneCom = SearchChild("n4").asCom;
        subCom = sceneCom.GetChild("n13").asCom;
        bgLoader = subCom.GetChild("n13").asLoader;
        controller = sceneCom.GetController("c1");
        bottomSubCom = sceneCom.GetChild("n17").asCom.GetChild("n18").asCom;
        scrollPane = bottomSubCom.scrollPane;
        InitSuipian();


        InitEvent();
    }

    void InitSuipian()
    {
        suipianList = new List<GComponent>();
        for (int i = 0; i < 7; i++)
        {
            GComponent gCom = subCom.GetChild("n" + (6 + i)).asCom;
            int index = i;
            gCom.onClick.Set(() =>
            {
                OnClickCom(index);
            });
            suipianList.Add(gCom);

            GComponent bottomCom = bottomSubCom.GetChild("n" + bottom[i]).asCom;
            bottomComs.Add(bottomCom);

        }
    }

    public override void InitEvent()
    {
        base.InitEvent();
        //返回
        SearchChild("n0").onClick.Set(() =>
        {
            EventMgr.Ins.DispachEvent(EventConfig.STORY_BREACK_STORY);
        });
        //skip
        SearchChild("n2").onClick.Set(SkipGame);
        //tips
        SearchChild("n1").onClick.Set(OnClickTips);

        sceneCom.GetChild("n18").onClick.Set(() =>
        {
            controller.selectedIndex = 1;
        });

        sceneCom.GetChild("n19").onClick.Set(() =>
        {
            OnComplete();
        });

        EventMgr.Ins.ReplaceEvent(EventConfig.STORY_GAME_QUIT, Destroy<GameBellTowerFindView>);
        EventMgr.Ins.ReplaceEvent(EventConfig.STORY_DELETE_GAME_INFO, DeleteGameInfo);

    }


    public override void InitData<D>(D data)
    {
        base.InitData(data);
        storyGameInfo = data as StoryGameInfo;
        bgLoader.url = UrlUtil.GetGameBGUrl(30);

        List<int> nodes = StoryDataMgr.ins.playerChapterInfo.GetPointsQueue;
        List<int> getted = new List<int>();
        foreach (int i in ruinsPoint)
        {
            if (nodes.Contains(i))
            {
                getted.AddRange(ruins);
                break;
            }
        }
        foreach (int i in xilaPoint)
        {
            if (nodes.Contains(i))
            {
                getted.AddRange(xila);
                break;
            }
        }
        foreach (int i in wordPoint)
        {
            if (nodes.Contains(i))
            {
                getted.AddRange(word);
                break;
            }
        }

        foreach (int i in getted)
        {
            GComponent bottomCom = bottomSubCom.GetChild("n" + i).asCom;
            bottomCom.GetController("c1").selectedIndex = 1;
        }
    }


    public override void InitData()
    {
        base.InitData();
        bgLoader.url = UrlUtil.GetGameBGUrl(30);
        controller.selectedIndex = 1;
    }

    void OnClickCom(int index)
    {
        getList.Add(index);
        GetEffect(index);

        if (putDownAudio == null)
            putDownAudio = Resources.Load(SoundConfig.GAME_AUDIO_EFFECT_URL + (int)SoundConfig.GameAudioId.GetItems) as AudioClip;
        GRoot.inst.PlayEffectSound(putDownAudio);


        if (getList.Count == 7)
        {
            controller.selectedIndex = 2;
            FXMgr.CreateEffectWithScale(sceneCom.GetChild("n18").asCom, new Vector2(-40, -580), "G13_finish", 162, -1);

        }
    }

    /// <summary>
    /// 拾取动画
    /// </summary>
    void GetEffect(int index)
    {
        scrollPane.ScrollToView(bottomComs[index]);
        suipianList[index].TweenMove(new Vector2(575, 812), 0.6f).OnComplete(() =>
        {
            Vector2 pos = bottomComs[index].position;
            pos.x -= scrollPane.posX;
            pos = bottomSubCom.TransformPoint(pos, subCom);
            suipianList[index].TweenMove(pos, 1f).OnComplete(() =>
            {
                suipianList[index].visible = false;
                bottomComs[index].GetController("c1").selectedIndex = 1;
            });
        });
    }

    Extrand extrand;
    void OnClickTips()
    {
        GetTip();
        //if (extrand == null)
        //{
        //    extrand = new Extrand();
        //    extrand.type = 1;
        //    extrand.key = "提示";
        //    List<GameNodeConsumeConfig> list = JsonConfig.GameNodeConsumeConfigs.FindAll(a => a.node_id == storyGameInfo.gamePointConfig.id && a.type == storyGameInfo.gamePointConfig.type);
        //    TinyItem item = ItemUtil.GetTinyItem(list[0].pay);
        //    extrand.item = item;
        //    extrand.msg = "获得提示";
        //    extrand.callBack = RequestGetTips;
        //}
        //UIMgr.Ins.showNextPopupView<PopupDialogView, Extrand>(extrand);
    }

    void RequestGetTips()
    {
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("nodeId", storyGameInfo.gamePointConfig.id);
        wWWForm.AddField("type", storyGameInfo.gamePointConfig.type);
        wWWForm.AddField("index", 0);
        GameMonoBehaviour.Ins.RequestInfoPost<PlayerProperty>(NetHeaderConfig.STROY_STEP_CONSUME, wWWForm, GetTip);
    }

    void GetTip()
    {
        for (int i = 0; i < 7; i++)
        {
            if (!getList.Contains(i))
            {
                OnClickCom(i);
                break;
            }
        }
    }
}

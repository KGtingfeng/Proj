using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

[ViewAttr("Game/UI/Y_Game14", "Y_Game14", "Game")]
public class GameRuinsFindView : BaseGameView
{
    GLoader bgLoader;
    GComponent bottomSubCom;
    ScrollPane scrollPane;
    List<GComponent> suipianList;
    GComponent arrowCom;
    List<int> getList = new List<int>();
    List<int> bottom = new List<int>() { 11, 19, 7, 16, 10, 21, 15, 5 };
    List<GComponent> bottomComs = new List<GComponent>();
    AudioClip putDownAudio;

    List<Vector2> tipsPos = new List<Vector2>()
    {
        new Vector2(43,24),
        new Vector2(25,36),
        new Vector2(44,19),
        new Vector2(59,24),
        new Vector2(42,22),
        new Vector2(30,36),
        new Vector2(56,39),
        new Vector2(72,46),
    };

    List<int> bell = new List<int>() { 12, 20, 3, 17, 1, 2, 4 };
    List<int> xila = new List<int>() { 9, 13, 6, 18, 8 };
    List<int> word = new List<int>() { 14 };

    List<int> bellPoint = new List<int>() { 196, 269, 323, 351, 371 };
    List<int> xilaPoint = new List<int>() { 203, 258, 263, 358, 365 };
    List<int> wordPoint = new List<int>() { 215, 239, 281, 305, 333 };


    public override void InitUI()
    {
        base.InitUI();

        bgLoader = SearchChild("n0").asLoader;
        controller = ui.GetController("c1");
        bottomSubCom = SearchChild("n13").asCom.GetChild("n18").asCom;
        scrollPane = bottomSubCom.scrollPane;
        InitSuipian();
        arrowCom = SearchChild("n18").asCom;
        arrowCom.visible = false;
        InitEvent();
    }

    void InitSuipian()
    {
        suipianList = new List<GComponent>();
        for (int i = 0; i < 8; i++)
        {
            GComponent gCom = ui.GetChild("n" + (3 + i)).asCom;
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
        SearchChild("n14").onClick.Set(() =>
        {
            EventMgr.Ins.DispachEvent(EventConfig.STORY_BREACK_STORY);
        });
        //skip
        SearchChild("n16").onClick.Set(SkipGame);
        //tips
        SearchChild("n15").onClick.Set(OnClickTips);


        ui.GetChild("n17").onClick.Set(() =>
        {
            OnComplete();
        });

        EventMgr.Ins.ReplaceEvent(EventConfig.STORY_GAME_QUIT, Destroy<GameRuinsFindView>);
        EventMgr.Ins.ReplaceEvent(EventConfig.STORY_DELETE_GAME_INFO, DeleteGameInfo);

    }


    public override void InitData<D>(D data)
    {
        base.InitData(data);
        storyGameInfo = data as StoryGameInfo;
        bgLoader.url = UrlUtil.GetGameBGUrl(31);

        List<int> nodes = StoryDataMgr.ins.playerChapterInfo.GetPointsQueue;
        List<int> getted = new List<int>();
        foreach (int i in bellPoint)
        {
            if (nodes.Contains(i))
            {
                getted.AddRange(bell);
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
        bgLoader.url = UrlUtil.GetGameBGUrl(31);
    }

    void OnClickCom(int index)
    {
        getList.Add(index);
        GetEffect(index);
        if (putDownAudio == null)
            putDownAudio = Resources.Load(SoundConfig.GAME_AUDIO_EFFECT_URL + (int)SoundConfig.GameAudioId.GetItems) as AudioClip;
        GRoot.inst.PlayEffectSound(putDownAudio);

        if (getList.Count == 8)
        {
            controller.selectedIndex = 1;

        }
    }

    /// <summary>
    /// 拾取动画
    /// </summary>
    void GetEffect(int index)
    {
        scrollPane.ScrollToView(bottomComs[index]);
        //找到碎片后，碎片飞入零件框的角度调整为从中间落入
        float posMidX = GRoot.inst.width / 2 - suipianList[index].width / 2;            
        suipianList[index].TweenMove(new Vector2(posMidX, 812), 0.6f).OnComplete(() =>
          {
              Vector2 pos = bottomComs[index].position;
              pos.x -= scrollPane.posX;
              pos = bottomSubCom.TransformPoint(pos, ui);
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
        //StartCoroutine(GetTip());
    }
 

    void GetTip()
    {
        for (int i = 0; i < 8; i++)
        {
            if (!getList.Contains(i))
            {
                OnClickCom(i);
                break;
            }
        }
    }


}

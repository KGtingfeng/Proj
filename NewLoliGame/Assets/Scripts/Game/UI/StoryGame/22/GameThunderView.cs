using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

[ViewAttr("Game/UI/Y_Game26", "Y_Game26", "Game_26")]
public class GameThunderView : BaseGameView
{
    GLoader bgLoader;
    GComponent com;
    GComponent clickCom;
    GObject icon;

    GGraph red;
    GGraph thunder;
    SwipeGesture swipe;
    AudioClip electric;

    List<Rect> roads = new List<Rect>()
    {
        new Rect(280,1379,295,1397),
        new Rect(368,1371,382,1397),
        new Rect(274,1207,295,1251),
        new Rect(175,1196,199,1200),
        new Rect(177,1082,200,1098),
        new Rect(505,1030,524,1050),
        new Rect(505,680,524,702),
        new Rect(333,680,345,704),
        new Rect(308,805,329,873),
        new Rect(97,805,122,824),
        new Rect(87,507,119,524),
        new Rect(352,478,401,493),


        new Rect(201,1397,453,1499),
        new Rect(295,1207,370,1397),
        new Rect(126,1207,295,1231),
        new Rect(126,1042,179,1207),
        new Rect(179,1042,628,1077),
        new Rect(526,639,628,1042),
        new Rect(320,635,526,685),
        new Rect(320,685,333,870),
        new Rect(55,821,333,870),
        new Rect(55,490,106,821),
        new Rect(106,490,423,506),
        new Rect(373,305,423,506),
        new Rect(269,199,517,305),
    };


    public override void InitUI()
    {
        base.InitUI();
        com = SearchChild("n4").asCom;
        bgLoader = com.GetChild("n7").asLoader;
        icon = com.GetChild("n12");
        clickCom= com.GetChild("n13").asCom;
        controller = com.GetController("c1");

        electric = Resources.Load(SoundConfig.GAME_AUDIO_EFFECT_URL + (int)SoundConfig.GameAudioId.ELECTRIC) as AudioClip;

        InitEvent();
    }

    public override void InitEvent()
    {
        base.InitEvent();

        EventMgr.Ins.ReplaceEvent(EventConfig.STORY_GAME_QUIT, Destroy<GameThunderView>);
        EventMgr.Ins.ReplaceEvent(EventConfig.STORY_DELETE_GAME_INFO, DeleteGameInfo);
        //返回
        SearchChild("n7").onClick.Set(() =>
        {
            EventMgr.Ins.DispachEvent(EventConfig.STORY_BREACK_STORY);
        });
        //跳过
        SearchChild("n8").onClick.Set(SkipGame);
        com.GetChild("n10").onClick.Set(StartGame);

        swipe = new SwipeGesture(clickCom);
        swipe.onBegin.Set(OnSwipeBegin);
        swipe.onMove.Set(OnSwipeMove);
        swipe.sensitivity = 2;
        FXMgr.CreateEffectWithScale(clickCom, new Vector3(235, 0), "Game26_idle", 162, -1);
        red= FXMgr.CreateEffectWithScale(clickCom, new Vector3(235, 0), "Game26_redtips", 162, -1);
        red.visible = false;
        thunder = FXMgr.CreateEffectWithScale(clickCom, new Vector3(235, 0), "Game26_shandian", 162, -1);
        thunder.visible = false;
    }

    public override void InitData<D>(D data)
    {
        base.InitData(data);
        storyGameInfo = data as StoryGameInfo;
        bgLoader.url = UrlUtil.GetGameBGUrl(48);
        ResetGame();


    }
    public override void InitData()
    {
        base.InitData();
        bgLoader.url = UrlUtil.GetGameBGUrl(48);
        ResetGame();

    }

    void ResetGame()
    {
        controller.selectedIndex = 0;
        icon.position = new Vector2(323, 1438);
        red.visible = false;
        thunder.visible = false;
    }

    void StartGame()
    {
        controller.selectedIndex = 1;
    }

    float shipX;
    float shipY;
    void OnSwipeBegin()
    {
        shipX = icon.x;
        shipY = icon.y;
    }

    void OnSwipeMove()
    {
        if (controller.selectedIndex == 0)
            return;
            Vector2 move = swipe.position;
        if (shipX + move.x > 30 && shipX + move.x  < GRoot.inst.width - 30)
            icon.x = shipX + move.x;
        if (shipY + move.y > 0 && shipY + move.y  < 1550)
            icon.y = shipY + move.y;
        CheckRole();
    }

    void CheckRole()
    {
        int roadIndex = -1;
        for (int i=0;i< roads.Count;i++)
        {
            if(icon.x>= roads[i].x&&icon.x<= roads[i].width&& icon.y >= roads[i].y && icon.y <= roads[i].height)
            {
                roadIndex = i;
                break;
            }
        }

        if (roadIndex == -1)
        {
           StartCoroutine(GameFaile());
            return;
        }

        if (roadIndex == roads.Count - 1)
            GameFinish();
    }

    IEnumerator GameFaile()
    {
        controller.selectedIndex = 0;
        red.visible = true;
        thunder.visible = true;
        Vector2 pos = icon.position;
        pos.x += 223;
        pos.y += 43;
        thunder.position = pos;
        GRoot.inst.PlayEffectSound(electric);

        yield return new WaitForSeconds(1f);
        GameTipsInfo info = new GameTipsInfo()
        {
            context = "庞尊的雷蛇电鞭击中了你!",
            callBack = ResetGame,
        };
        UIMgr.Ins.showNextPopupView<GameFailView, GameTipsInfo>(info);
    }

    void GameFinish()
    {
        controller.selectedIndex = 0;

        GameTipsInfo info = new GameTipsInfo()
            {
                context = "你成功躲开了庞尊雷蛇电鞭的攻击！",
                callBack = OnComplete
            };
            UIMgr.Ins.showNextPopupView<GameSuccessView, GameTipsInfo>(info);
            
    }
}

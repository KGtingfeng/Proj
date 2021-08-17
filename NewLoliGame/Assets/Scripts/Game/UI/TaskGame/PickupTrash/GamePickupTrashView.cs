using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

[ViewAttr("Game/UI/Y_Game22", "Y_Game22", "Game22")]
public class GamePickupTrashView : BaseView
{

    List<string> urls = new List<string>()
    {
        "ui://equh1laqvvawb",
        "ui://equh1laq8noae",
        "ui://equh1laq8noaf",
        "ui://equh1laqcf64i",
        "ui://equh1laqcf64j",
        "ui://equh1laqcf64k",
    };
    GComponent gComponent;

    GComponent clickCom;
    //钩子
    GObject hookO;
    Hook hook;
    GTextField contentText;
    GObject wall1;
    GObject wall2;
    GObject wall3;
    GLoader bgLoader;
    Vector2 min = new Vector2(30, 700);

    List<GameXinglingDlllConfig> configs = new List<GameXinglingDlllConfig>();
    GameXinlingConfig config;
    int getNum;
    int barrierNum;
    int trashNum;

    List<GComponent> items = new List<GComponent>();

    public override void InitUI()
    {
        base.InitUI();
        gComponent = SearchChild("n1").asCom;
        hookO = gComponent.GetChild("n2");
        contentText = gComponent.GetChild("n3").asTextField;
        clickCom = gComponent.GetChild("n42").asCom;
        bgLoader = SearchChild("n5").asLoader;
        hook = hookO.displayObject.gameObject.AddComponent<Hook>();
        hook.Init();
        wall1 = gComponent.GetChild("n43");
        wall2 = gComponent.GetChild("n44");
        wall3 = gComponent.GetChild("n45");
        BoxCollider2D boxCollider1 = wall1.displayObject.gameObject.AddComponent<BoxCollider2D>();
        boxCollider1.isTrigger = true;
        boxCollider1.size = new Vector2(100, 1624);
        boxCollider1.offset = new Vector2(50, -812);
        BoxCollider2D boxCollider2 = wall2.displayObject.gameObject.AddComponent<BoxCollider2D>();
        boxCollider2.isTrigger = true;
        boxCollider2.size = new Vector2(100, 1624);
        boxCollider2.offset = new Vector2(50, -812);
        BoxCollider2D boxCollider3 = wall3.displayObject.gameObject.AddComponent<BoxCollider2D>();
        boxCollider3.isTrigger = true;
        boxCollider3.size = new Vector2(800, 20);
        boxCollider3.offset = new Vector2(400, -10);

        FXMgr.CreateEffectWithScale(clickCom, new Vector3(254,-98), "UI_game22_shuizhu", 162, -1);
        InitEvent();
    }

    public override void InitEvent()
    {
        base.InitEvent();

        SearchChild("n0").onClick.Set(OnClickClose);
        //SearchChild("n4").onClick.Set(GameFinish);
        clickCom.onClick.Set(ThrowHook);
        EventMgr.Ins.RegisterEvent<GameXinglingDlllConfig>(EventConfig.GAME_THROW_BACK, ThrowBack);
        EventMgr.Ins.RegisterEvent(EventConfig.GAME_PICKUP_EXIT, OnHideAnimation);
        EventMgr.Ins.RegisterEvent(EventConfig.GAME_THROW_GET, GetThing);
        EventMgr.Ins.RegisterEvent(EventConfig.GAME_THROW, ThrowThing);

    }

    public override void InitData()
    {
        OnShowAnimation();
        base.InitData();
        if (GameData.Player.level > 50)
        {
            config = JsonConfig.GameXinlingConfigs.Find(a => a.type == GameXinlingConfig.TYPE_PICKUP && a.ckey == "3");
        }
        else if (GameData.Player.level > 20)
        {
            config = JsonConfig.GameXinlingConfigs.Find(a => a.type == GameXinlingConfig.TYPE_PICKUP && a.ckey == "2");
        }
        else
        {
            config = JsonConfig.GameXinlingConfigs.Find(a => a.type == GameXinlingConfig.TYPE_PICKUP && a.ckey == "1");
        }
        //config = JsonConfig.GameXinlingConfigs.Find(a => a.type == GameXinlingConfig.TYPE_PICKUP && a.ckey == "3");
        bgLoader.url = UrlUtil.GetBgUrl("Xinling", "2");
        string[] val = config.cval.Split(new char[1] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
        trashNum = int.Parse(val[0]);
        barrierNum = int.Parse(val[1]);
        configs.Clear();
        List<GameXinglingDlllConfig> trashConfig = JsonConfig.GameXinglingDlllConfigs.FindAll(a => a.type == 1);
        List<GameXinglingDlllConfig> barrierConfig = JsonConfig.GameXinglingDlllConfigs.FindAll(a => a.type == 2);
        for (int i = 0; i < trashNum; i++)
        {
            configs.Add(trashConfig[Random.Range(0, trashConfig.Count)]);
        }
        for (int i = 0; i < barrierNum; i++)
        {
            configs.Add(barrierConfig[Random.Range(0, barrierConfig.Count)]);
        }
        ListRandom(configs);
        getNum = 0;
        SetText();
        CreateItem();
        RiverGoWrapper();
        HookRoation();

        AudioClip audioClip = ResourceUtil.LoadBackGroundMusic(SoundConfig.CommonMusicId.PickUp);
        GRoot.inst.PlayBgSound(audioClip);
    }

    private void ListRandom(List<GameXinglingDlllConfig> myList)
    {
        int index = 0;
        GameXinglingDlllConfig temp = null;
        for (int i = 0; i < myList.Count; i++)
        {
            index = UnityEngine.Random.Range(0, myList.Count - 1);
            if (index != i)
            {
                temp = myList[i];
                myList[i] = myList[index];
                myList[index] = temp;
            }
        }
    }
    List<Vector2> posList = new List<Vector2>();
    List<GComponent> objList = new List<GComponent>();
    private void CreateItem()
    {
        posList.Clear();
        objList.Clear();
        for (int i = 0; i < items.Count; i++)
        {
            gComponent.RemoveChild(items[i]);
        }
        items.Clear();
        List<int> randomList = new List<int>();
        for (int i = 0; i < 36; i++)
        {
            randomList.Add(i);
        }

        for (int i = 0; i < configs.Count; i++)
        {
            GComponent obj = UIPackage.CreateObjectFromURL(urls[(configs[i].type - 1) * 3 + (int.Parse(configs[i].ckey) - 1)]).asCom;
            gComponent.AddChildAt(obj, gComponent.GetChildIndex(gComponent.GetChild("n1")));
            obj.GetChild("n1").asLoader.url = UrlUtil.GetRubbishUrl(configs[i].id);
            int random = Random.Range(0, randomList.Count);
            int randomX = Random.Range(0, 5);
            int randomY = Random.Range(0, 5);
            Vector2 pos = new Vector2(min.x + randomList[random] / 6 * 100 + randomX * 5, min.y + randomList[random] % 8 * 100 + randomY * 5);
            posList.Add(pos);
            randomList.RemoveAt(random);
            items.Add(obj);
            obj.position = SearchChild("n6").asGraph.position;
            
            obj.displayObject.gameObject.AddComponent<Barrier>().Init(configs[i]);
            objList.Add(obj);
        }
        GoAround();
    }

    void GoAround()
    {
        for (int i = 0; i < objList.Count; i++)
        {
            objList[i].TweenMove(posList[i], 0.5f);
        }
    }

    //出钩子
    private void ThrowHook()
    {
        TweenManager.KillAllTween();
        hook.Throw();
      
    }
    public void ThrowThing()
    {
        hookO.asCom.GetTransition("t0").Play(1, 0, 0, 0.3f, null);
    }

    public void GetThing()
    {
        hookO.asCom.GetTransition("t0").Play(1, 0, 0.3f, 0.6f, null);
    }
    private void ThrowBack(GameXinglingDlllConfig config)
    {
        HookRoation();
     
        if (config != null)
        {
            if (config.type == 1)
            {
                getNum++;
                SetText();
                if (getNum >= trashNum)
                {
                    GameFinish();
                }
            }
        }
    }

    private void SetText()
    {
        contentText.text = "打捞垃圾数\n" + getNum + "/" + trashNum;

    }

    private void GameFinish()
    {

        UIMgr.Ins.showNextPopupView<XinlingFinishView, GameXinlingConfig>(config);

    }
    //钩子旋转
    private void HookRoation()
    {
        float time = 1 - hookO.rotation / 60;
        hookO.TweenRotate(60, time).SetEase(EaseType.Linear).OnComplete(() =>
        {
            hookO.TweenRotate(-60, 2).SetEase(EaseType.Linear).OnComplete(() =>
            {
                HookRoation();
            });
        });
    }

    public override void OnHideAnimation()
    {
        TweenManager.KillAllTween();
        base.OnHideAnimation();

    }
    GoWrapper riverGW;
    void RiverGoWrapper()
    {
        if (riverGW == null)
        {
            GGraph g = gComponent.GetChild("n51").asGraph;
            riverGW = FXMgr.CreateEffectWithGGraph(g, "River", new Vector3(2, 1, 1), -1);
            riverGW.scale = new Vector3(162,93,162);
            riverGW.position += new Vector3(-1518, -293, 0);
        }
        else
        {
            riverGW.gameObject.SetActive(true);
        }
    }

    Extrand extrand;
    void OnClickClose()
    {
        if (extrand == null)
        {
            extrand = new Extrand();
            extrand.key = "提示";
            extrand.msg = "任务还未完成，现在退出将不会保存游戏进度；确认退出吗？";
            extrand.callBack = onHide;
        }
        UIMgr.Ins.showNextPopupView<GameTipsView, Extrand>(extrand);
    }

    public override void onHide()
    {
        TouchScreenView.Ins.PlayChangeEffect(() => {
            UIMgr.Ins.HideView<GamePickupTrashView>();
            AudioClip audioClip = ResourceUtil.LoadBackGroundMusic(SoundConfig.CommonMusicId.BgId);
            GRoot.inst.PlayBgSound(audioClip);
        });
    }
}

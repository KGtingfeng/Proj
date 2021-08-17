using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

//垃圾分类
[ViewAttr("Game/UI/Y_Game19", "Y_Game19", "Game19")]
public class GameRubbishClassificationView : BaseView
{
    string url = "ui://esksx6yih4fse";
    GTextField timeText;
    GComponent gComponent;
    GComponent otherCom;
    GComponent harmfulCom;
    GComponent recyclableCom;
    GComponent kitchenwasteCom;

    AudioClip fail;
    AudioClip success;


    GLoader bgLoader;

    GameXinlingConfig config;
    int limitTime;
    int rubbishNum;

    Vector2 min = new Vector2(80, 440);

    List<GameXinlingLjflConfig> other = new List<GameXinlingLjflConfig>();
    List<GameXinlingLjflConfig> harmful = new List<GameXinlingLjflConfig>();
    List<GameXinlingLjflConfig> recyclable = new List<GameXinlingLjflConfig>();
    List<GameXinlingLjflConfig> kitchenwaste = new List<GameXinlingLjflConfig>();
    List<GameXinlingLjflConfig> configs = new List<GameXinlingLjflConfig>();
    List<Vector2> itemPos = new List<Vector2>();
    List<GComponent> items = new List<GComponent>();
    public override void InitUI()
    {
        base.InitUI();
        gComponent = SearchChild("n2").asCom;
        otherCom = gComponent.GetChild("n1").asCom;
        harmfulCom = gComponent.GetChild("n0").asCom;
        recyclableCom = gComponent.GetChild("n2").asCom;
        kitchenwasteCom = gComponent.GetChild("n3").asCom;
        controller = gComponent.GetController("c1");

        timeText = gComponent.GetChild("n13").asTextField;
        bgLoader = gComponent.GetChild("n36").asLoader;
        InitEvent();
    }

    public override void InitEvent()
    {
        base.InitEvent();
        SearchChild("n0").onClick.Set(OnClickClose);
        SearchChild("n1").onClick.Set(OnClickTip);
        otherCom.onDrop.Set((EventContext context) =>
        {
            OnDrop(GameXinlingLjflConfig.TYPE_OTHER, context);
        });

        harmfulCom.onDrop.Set((EventContext context) =>
        {
            OnDrop(GameXinlingLjflConfig.TYPE_HARMFUL, context);
        });

        recyclableCom.onDrop.Set((EventContext context) =>
        {
            OnDrop(GameXinlingLjflConfig.TYPE_RECYCLABLE, context);
        });

        kitchenwasteCom.onDrop.Set((EventContext context) =>
        {
            OnDrop(GameXinlingLjflConfig.TYPE_KITCHENWASTE, context);
        });

        gComponent.GetChild("n34").asCom.onDrop.Set(BGOnDrop);
        SearchChild("n0").asCom.onDrop.Set(BGOnDrop);
        SearchChild("n1").asCom.onDrop.Set(BGOnDrop);

        EventMgr.Ins.RegisterEvent(EventConfig.GAME_CLASSIFICATION_EXIT, OnHideAnimation);
    }

    public override void InitData()
    {
        OnShowAnimation();
        base.InitData();
        bgLoader.url = UrlUtil.GetBgUrl("Xinling", "3");
        //int level = 51;
        if (GameData.Player.level > 50)
        {
            config = JsonConfig.GameXinlingConfigs.Find(a => a.type == GameXinlingConfig.TYPE_CLASSIFICATION && a.ckey == "3");
        }
        else if (GameData.Player.level > 20)
        {
            config = JsonConfig.GameXinlingConfigs.Find(a => a.type == GameXinlingConfig.TYPE_CLASSIFICATION && a.ckey == "2");
        }
        else
        {
            config = JsonConfig.GameXinlingConfigs.Find(a => a.type == GameXinlingConfig.TYPE_CLASSIFICATION && a.ckey == "1");
        }
        //config = JsonConfig.GameXinlingConfigs.Find(a => a.type == GameXinlingConfig.TYPE_CLASSIFICATION && a.ckey == "3");
        string[] val = config.cval.Split(new char[1] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
        rubbishNum = int.Parse(val[0]);
        limitTime = int.Parse(val[1]);

        RandomRubbish();
        CreateItem();
        StartGame();

        AudioClip audioClip = ResourceUtil.LoadBackGroundMusic(SoundConfig.CommonMusicId.Rubbish);
        GRoot.inst.PlayBgSound(audioClip);
    }

    private void RandomRubbish()
    {
        configs.Clear();
        other.Clear();
        harmful.Clear();
        recyclable.Clear();
        kitchenwaste.Clear();
        other = JsonConfig.GameXinlingLjflConfigs.FindAll(a => a.type == GameXinlingLjflConfig.TYPE_OTHER);
        harmful = JsonConfig.GameXinlingLjflConfigs.FindAll(a => a.type == GameXinlingLjflConfig.TYPE_HARMFUL);
        recyclable = JsonConfig.GameXinlingLjflConfigs.FindAll(a => a.type == GameXinlingLjflConfig.TYPE_RECYCLABLE);
        kitchenwaste = JsonConfig.GameXinlingLjflConfigs.FindAll(a => a.type == GameXinlingLjflConfig.TYPE_KITCHENWASTE);

        int otherNum = UnityEngine.Random.Range(2, rubbishNum - 6);
        int harmfulNum = UnityEngine.Random.Range(2, rubbishNum - 4 - otherNum);
        int recyclableNum = UnityEngine.Random.Range(2, rubbishNum - 2 - otherNum - harmfulNum);
        int kitchenwasteNum = rubbishNum - otherNum - harmfulNum - recyclableNum;
        for (int i = 0; i < otherNum; i++)
        {
            GameXinlingLjflConfig config = other[UnityEngine.Random.Range(0, other.Count)];
            configs.Add(config);
            other.Remove(config);
        }
        for (int i = 0; i < harmfulNum; i++)
        {
            GameXinlingLjflConfig config = harmful[UnityEngine.Random.Range(0, harmful.Count)];
            configs.Add(config);
            harmful.Remove(config);
        }
        for (int i = 0; i < recyclableNum; i++)
        {
            GameXinlingLjflConfig config = recyclable[UnityEngine.Random.Range(0, recyclable.Count)];
            configs.Add(config);
            recyclable.Remove(config);
        }
        for (int i = 0; i < kitchenwasteNum; i++)
        {
            GameXinlingLjflConfig config = kitchenwaste[Random.Range(0, kitchenwaste.Count)];
            configs.Add(config);
            kitchenwaste.Remove(config);
        }

    }
    private void CreateItem()
    {
        for (int i = 0; i < items.Count; i++)
        {
            gComponent.RemoveChild(items[i]);
        }
        items.Clear();
        itemPos.Clear();

        List<int> randomList = new List<int>();
        for (int i = 0; i < 25; i++)
        {
            randomList.Add(i);
        }
        for (int i = 0; i < rubbishNum; i++)
        {
            GComponent obj = UIPackage.CreateObjectFromURL(url).asCom;
            gComponent.AddChildAt(obj, gComponent.GetChildIndex(timeText));
            obj.touchable = true;
            obj.draggable = true;
            int index = i;
            obj.onDragStart.Set(() => { OnDragStart(index); });
            obj.onDragEnd.Set(() => { OnDragEnd(index); });
            items.Add(obj);
            int random = Random.Range(0, randomList.Count);
            int randomX = Random.Range(0, 5);
            int randomY = Random.Range(0, 5);
            Vector2 pos = new Vector2(min.x + randomList[random] / 5 * 117 + randomX * 5, min.y + randomList[random] % 6 * 106 + randomY * 5);
            randomList.RemoveAt(random);
            itemPos.Add(pos);
           
            obj.position = SearchChild("n3").asGraph.position;
            obj.GetChild("n5").asTextField.text = configs[i].description;
            obj.GetChild("n4").asLoader.url = UrlUtil.GetRubbishUrl(configs[i].id);
        }
        GoAround();
    }
    void GoAround()
    {
        for (int i = 0; i < items.Count; i++)
        {
            items[i].TweenMove(itemPos[i], 0.5f);
        }
    }
    int trueCount;
    private void StartGame()
    {
        timeText.text = "";
        if (limitTime > 0)
        {
            countDown = limitTime;
            Timers.inst.Add(1f, 0, CountDown);
            timeText.text = "剩余时间：" + countDown + "s";
        }
        controller.selectedIndex = 0;
        for (int i = 0; i < items.Count; i++)
        {
            items[i].position = itemPos[i];
            items[i].visible = true;
        }
        trueCount = 0;
    }

    void OnDragStart(int index)
    {
        items[index].touchable = false;
    }

    void OnDragEnd(int index)
    {

        GObject obj = GRoot.inst.touchTarget;
        items[index].touchable = true;

        while (obj != null)
        {
            if (obj.hasEventListeners("onDrop"))
            {
                obj.RequestFocus();
                obj.DispatchEvent("onDrop", index);
                return;
            }
            obj = obj.parent;
        }
    }

    void OnDrop(int index, EventContext context)
    {
        int dragIndex = int.Parse(context.data.ToString());
        if (index == configs[dragIndex].type)
        {
            OnDropTrue(dragIndex);
            if (success == null)
                success = Resources.Load(SoundConfig.GetGameAudioUrl(SoundConfig.GameAudioId.ClickSuccess)) as AudioClip;
            GRoot.inst.PlayEffectSound(success);
        }
        else
        {
            OnDropError(dragIndex);
            if (fail == null)
                fail = Resources.Load(SoundConfig.GetGameAudioUrl(SoundConfig.GameAudioId.Error)) as AudioClip;
            GRoot.inst.PlayEffectSound(fail);
        }

    }

    void BGOnDrop(EventContext context)
    {
        int dragIndex = int.Parse(context.data.ToString());
        OnDropError(dragIndex);
    }

    private void OnDropError(int index)
    {
        items[index].touchable = false;
        items[index].TweenMove(itemPos[index], 1).OnComplete(() =>
        {
            items[index].touchable = true;
        });
    }

    private void OnDropTrue(int index)
    {
        items[index].visible = false;
        trueCount++;
        if (trueCount >= items.Count)
        {
            GameFinish();
        }
    }

    int countDown;
    void CountDown(object param)
    {
        countDown--;
        timeText.text = "剩余时间：" + countDown + "s";

        if (countDown <= 0)
        {

            Timers.inst.Remove(CountDown);
            GameFail();
        }

    }

    private void GameFail()
    {
        GameTipsInfo info = new GameTipsInfo()
        {
            context = "任务失败了，再来一次吧！",
            callBack = StartGame,

        };
        UIMgr.Ins.showNextPopupView<GameFailView, GameTipsInfo>(info);
    }

    private void GameFinish()
    {
        Timers.inst.Remove(CountDown);

        UIMgr.Ins.showNextPopupView<XinlingFinishView, GameXinlingConfig>(config);

    }

    private void OnClickTip()
    {
        GameFinish();
    }

    Extrand extrand;
    void OnClickClose()
    {
        if (extrand == null)
        {
            extrand = new Extrand();
            extrand.key = "提示";
            extrand.msg = "任务还未完成，现在退出将不会保存游戏进度；确认退出吗？";
            extrand.callBack = Back;
        }
        UIMgr.Ins.showNextPopupView<GameTipsView, Extrand>(extrand);
    }

    void Back()
    {
        Timers.inst.Remove(CountDown);
        TouchScreenView.Ins.PlayChangeEffect(() => {
            UIMgr.Ins.HideView<GameRubbishClassificationView>();
            AudioClip audioClip = ResourceUtil.LoadBackGroundMusic(SoundConfig.CommonMusicId.BgId);
            GRoot.inst.PlayBgSound(audioClip);
        });
    }
}

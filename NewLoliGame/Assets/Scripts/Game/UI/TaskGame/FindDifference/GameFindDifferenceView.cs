using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using System.Linq;

[ViewAttr("Game/UI/Y_Game24", "Y_Game24", "Game24")]
public class GameFindDifferenceView : BaseView
{
    GComponent gComponent;
    GTextField timeText;
    GComponent imageCom1;
    GComponent imageCom2;
    GButton backBtn;
    GButton tipsBtn;

    GLoader bgLoader;
    GComponent sceneCom;
    GameXinlingConfig config;
    AudioClip fail;
    AudioClip success;

    GameXinlingYqzcConfig imageConfig;
    List<Vector2> rightPoses = new List<Vector2>();
    List<GComponent> rightComs1 = new List<GComponent>();
    List<GComponent> rightComs2 = new List<GComponent>();
    GList rightList;
    int limitTime;
    int findNum;
    int trueCount;
    public override void InitUI()
    {
        base.InitUI();
        sceneCom = SearchChild("n1").asCom;
        imageCom1 = sceneCom.GetChild("n4").asCom;
        imageCom2 = sceneCom.GetChild("n5").asCom;
        bgLoader = SearchChild("n2").asLoader;
        //controller = sceneCom.GetController("c1");
        backBtn = SearchChild("n0").asButton;
        tipsBtn = SearchChild("n3").asButton;
        timeText = sceneCom.GetChild("n0").asTextField;
        rightList = sceneCom.GetChild("n2").asList;
        InitEvent();
    }

    public override void InitEvent()
    {
        base.InitEvent();
        imageCom1.GetChild("n5").onClick.Set(() =>
        {
            OnClickWrong(imageCom1);
        });
        imageCom2.GetChild("n6").onClick.Set(() =>
        {
            OnClickWrong(imageCom2);
        });
        //ui.onClick.Set(ShowLocalVector);
        backBtn.onClick.Set(OnClickClose);
        tipsBtn.onClick.Set(GetTips);
        EventMgr.Ins.RegisterEvent(EventConfig.GAME_FIND_EXIT, Back);

    }
    public void Back()
    {
        Timers.inst.Remove(CountDown);
        TouchScreenView.Ins.PlayChangeEffect(() => {
            UIMgr.Ins.HideView<GameFindDifferenceView>();
            AudioClip audioClip = ResourceUtil.LoadBackGroundMusic(SoundConfig.CommonMusicId.BgId);
            GRoot.inst.PlayBgSound(audioClip);
        });
    }

    public override void InitData()
    {
        OnShowAnimation();
        base.InitData();
        bgLoader.url = UrlUtil.GetGameBGUrl(45);
        DisposItem(rightComs1);
        DisposItem(rightComs2);

        rightPoses.Clear();
        rightComs1.Clear();
        rightComs2.Clear();
        findNum = 0; 
        if (GameData.Player.level > 50)
        {
            config = JsonConfig.GameXinlingConfigs.Find(a => a.type == GameXinlingConfig.TYPE_FIND && a.ckey == "3");
        }
        else if (GameData.Player.level > 20)
        {
            config = JsonConfig.GameXinlingConfigs.Find(a => a.type == GameXinlingConfig.TYPE_FIND && a.ckey == "2");
        }
        else
        {
            config = JsonConfig.GameXinlingConfigs.Find(a => a.type == GameXinlingConfig.TYPE_FIND && a.ckey == "1");
        }
        List<GameXinlingYqzcConfig> gameXinlingYqzcConfigs = JsonConfig.GameXinlingYqzcConfigs.Where(a => a.image_id == config.ckey).ToList();
        imageConfig = gameXinlingYqzcConfigs[Random.Range(0, gameXinlingYqzcConfigs.Count)];
        GetPosition();
        imageCom1.touchable = true;
        imageCom2.touchable = true;
        imageCom1.GetChild("n5").asLoader.url = UrlUtil.GetFindImageUrl(imageConfig.id+"", 1);
        imageCom2.GetChild("n6").asLoader.url = UrlUtil.GetFindImageUrl(imageConfig.id+"", 2);

        StartGame();
        AddRight();
        InitList();

        AudioClip audioClip = ResourceUtil.LoadBackGroundMusic(SoundConfig.CommonMusicId.FindDifference);
        GRoot.inst.PlayBgSound(audioClip);
    }

    void DisposItem(List<GComponent> gComponents)
    {
        foreach (var item in gComponents)
        {
            item.Dispose();
        }
    }

    void InitList()
    {
        rightList.itemRenderer = RightListRenderer;
        rightList.numItems = trueCount;
    }

    void RightListRenderer(int index, GObject obj)
    {
        GComponent com = obj.asCom;
        com.GetController("c1").selectedIndex = index + 1 <= findNum ? 1 : 0;
    }

    //放置正确答案
    void AddRight()
    {
        for (int i = 0; i < rightPoses.Count; i++)
        {
            int index = i;
            GComponent com = UIPackage.CreateObject("Y_Game24", "Right").asCom;
            com.alpha = 0;
            com.position = rightPoses[i];
            com.onClick.Set(() =>
            {
                OnClickRight(index);
            });
            imageCom1.AddChild(com);
            rightComs1.Add(com);
            GComponent com2 = UIPackage.CreateObject("Y_Game24", "Right").asCom;
            com2.alpha = 0;
            com2.position = rightPoses[i];
            com2.onClick.Set(() =>
            {
                OnClickRight(index);
            });
            imageCom2.AddChild(com2);
            rightComs2.Add(com2);
        }


    }


    void OnClickRight(int index)
    {
        if (rightComs1[index].alpha == 0 && rightComs2[index].alpha == 0)
        {
            rightComs1[index].alpha = 1;
            rightComs2[index].alpha = 1;
            rightComs1[index].GetTransition("t0").Play();
            rightComs2[index].GetTransition("t0").Play();
            findNum++;
            RightListRenderer(findNum - 1, rightList.GetChildAt(findNum - 1));
            if (findNum == trueCount)
            {
                GameWin();
            }
            if (success == null)
                success = Resources.Load(SoundConfig.GetGameAudioUrl(SoundConfig.GameAudioId.ClickClock)) as AudioClip;
            GRoot.inst.PlayEffectSound(success);
        }


    }


    private void StartGame()
    {
        GetTime();
        timeText.text = "";
        if (limitTime > 0)
        {
            countDown = limitTime;
            Timers.inst.Add(1f, limitTime, CountDown);
            timeText.text = "剩余时间：" + countDown + "s";
        }

    }




    int countDown;
    void CountDown(object param)
    {
        countDown--;
        timeText.text = "剩余时间：" + countDown + "s";

        if (countDown <= 0)
        {
            GameFail();
        }

    }
    void GameWin()
    {
        timeText.text = "you win";
        GameFinish();
        CantTouch();
    }

    private void GameFail()
    {
        timeText.text = "GameOver";
        Timers.inst.Remove(CountDown);
        CantTouch();
        GameTipsInfo info = new GameTipsInfo()
        {
            isShowBtn = true,
            btnTitle = "重新开始",
            title = "帮助失败了",
            context = "帮助辛灵姐姐失败了，重新来过吧！",
            callBack = InitData
        };
        UIMgr.Ins.showNextPopupView<StoryTipsView, GameTipsInfo>(info);
    }
    void CantTouch()
    {
        imageCom1.touchable = false;
        imageCom2.touchable = false;
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

    void OnClickWrong(GComponent gComponent)
    {
        Vector2 pos4 = GameTool.MousePosToLocalUI(Input.mousePosition, gComponent);

        GComponent com = UIPackage.CreateObject("Y_Game24", "Wrong").asCom;
        //com.pivot = new Vector2(0.5f, 0.5f);
        com.position = pos4;
        gComponent.AddChild(com);
        //countDown -= 5;
        if (countDown <= 0)
            GameFail();
        StartCoroutine(DestroyCom(com));

        if (fail == null)
            fail = Resources.Load(SoundConfig.GetGameAudioUrl(SoundConfig.GameAudioId.Error)) as AudioClip;
        GRoot.inst.PlayEffectSound(fail);
    }
    IEnumerator DestroyCom(GComponent com)
    {
        yield return new WaitForSeconds(0.6f);
        com.parent.RemoveChild(com);
        com.Dispose();
    }

    void GetPosition()
    {
        string[] vecs = imageConfig.cval.Split(';');
        for (int i = 0; i < vecs.Length - 1; i++)
        {
            Vector2 vec = new Vector2();
            string[] vecString = vecs[i].Split(',');
            vec.x = int.Parse(vecString[0]);
            vec.y = int.Parse(vecString[1]);
            rightPoses.Add(vec);
        }

    }


    void GetTime()
    {
        string[] cval = config.cval.Split(',');
        limitTime = int.Parse(cval[1]);
        trueCount = int.Parse(cval[0]);
    }



    void GetTips()
    {
        for (int i = 0; i < rightComs1.Count; i++)
        {
            OnClickRight(i);
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
            extrand.callBack = Back;
        }
        UIMgr.Ins.showNextPopupView<GameTipsView, Extrand>(extrand);
    }
}

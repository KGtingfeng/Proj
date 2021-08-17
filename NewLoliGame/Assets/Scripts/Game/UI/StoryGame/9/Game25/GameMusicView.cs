using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

[ViewAttr("Game/UI/Y_Game25", "Y_Game25", "Game25")]
public class GameMusicView : BaseGameView
{

    GLoader bgLoader;
    GComponent com;
    GComponent fxCom;
    GComponent bgFxCom;
    GComponent tap;
    GGraph miss;
    GTextField scoreText;
    GTextField timeText;
    Transition countdownT;
    AudioClip audioClip;
    float soundTime;
    List<Vector2> blockBornPosList;
    List<GComponent> lightCom=new List<GComponent>();
    List<Queue<GComponent>> blockQueue = new List<Queue<GComponent>>();
    List<float> blockTime = new List<float>();

    List<string> btn = new List<string>();

    AudioClip click;
    AudioClip countdown;

    int score;
    public override void InitUI()
    {
        base.InitUI();
        bgLoader = SearchChild("n3").asLoader;
        com = SearchChild("n2").asCom;
        controller = com.GetController("c1");
        countdownT = com.GetChild("n14").asCom.GetTransition("t0");
        scoreText = com.GetChild("n13").asTextField;
        timeText = com.GetChild("n12").asTextField;
        fxCom = com.GetChild("n27").asCom;
        bgFxCom = com.GetChild("n28").asCom;
        tap= com.GetChild("n29").asCom;
        tap.visible = false;
        InitEvent();
    }

    public override void InitEvent()
    {
        base.InitEvent();

        EventMgr.Ins.ReplaceEvent(EventConfig.STORY_GAME_QUIT, Destroy<GameMusicView>);
        //返回
        SearchChild("n0").onClick.Set(() =>
        {
            EventMgr.Ins.DispachEvent(EventConfig.STORY_BREACK_STORY);
        });
        //跳过
        SearchChild("n1").onClick.Set(SkipGame);
        click = Resources.Load(SoundConfig.GAME_AUDIO_EFFECT_URL + (int)SoundConfig.GameAudioId.PIANO) as AudioClip;
        audioClip = Resources.Load(SoundConfig.GAME_MUSIC_URL + "1") as AudioClip;
        soundTime = audioClip.length;
        for (int i = 0; i < 4; i++)
        {
            Queue<GComponent> gComponents = new Queue<GComponent>();
            blockQueue.Add(gComponents);
            GComponent light = com.GetChild("n" + (6 + i)).asCom;
            lightCom.Add(light);
        }
        com.GetChild("n30").onClick.Set(()=> { OnClick(0); });
        com.GetChild("n31").onClick.Set(()=> { OnClick(1); });
        com.GetChild("n32").onClick.Set(()=> { OnClick(2); });
        com.GetChild("n33").onClick.Set(()=> { OnClick(3); });
        com.GetChild("n30").asButton.soundVolumeScale = 0;
        com.GetChild("n31").asButton.soundVolumeScale = 0;
        com.GetChild("n32").asButton.soundVolumeScale = 0;
        com.GetChild("n33").asButton.soundVolumeScale = 0;
        miss = FXMgr.CreateEffectWithScale(fxCom, new Vector3(232,0), "Game25_miss", 162, -1);
        miss.visible = false;
          FXMgr.CreateEffectWithScale(bgFxCom, new Vector3(302,372), "Game25_idle", 162, -1);
    }

    bool isNewbie;
    public override void InitData<D>(D data)
    {
        base.InitData(data);
        storyGameInfo = data as StoryGameInfo;
        bgLoader.url = UrlUtil.GetGameBGUrl(46);
        controller.selectedIndex = 1;
        if (blockBornPosList == null)
        {
            blockBornPosList = new List<Vector2>();
            for (int i = 0; i < 4; i++)
            {
                blockBornPosList.Add(com.GetChild("n" + (18 + i)).asGraph.position);
            }
        }
        ResetGame();
        GRoot.inst.StopBgSound();
        GameMusicTimelineConfig config = JsonConfig.GameMusicTimelineConfigs.Find(a => a.id == 1);
        string[] timelines = config.content.Split(new char[1] { ':' }, System.StringSplitOptions.RemoveEmptyEntries);
        foreach (string timeline in timelines)
        {
            string[] lines = timeline.Split(new char[1] { ';' }, System.StringSplitOptions.RemoveEmptyEntries);
            blockTime.Add(float.Parse(lines[0]));
            btn.Add(lines[1]);
        }


        if(storyGameInfo!=null && storyGameInfo.gameSaves.Count > 0)
        {
            isNewbie = false;
        }
        else
        {
            isNewbie = true;
        }
    }
    public override void InitData()
    {
        base.InitData();
        bgLoader.url = UrlUtil.GetGameBGUrl(46);
        if (blockBornPosList==null)
        {
            blockBornPosList = new List<Vector2>();
            for (int i = 0; i < 4; i++)
            {
                blockBornPosList.Add(com.GetChild("n" + (18 + i)).asGraph.position);
            }
        }
        ResetGame();

        GameMusicTimelineConfig config = JsonConfig.GameMusicTimelineConfigs.Find(a => a.id == 1);
        string[] timelines = config.content.Split(new char[1] { ':' }, System.StringSplitOptions.RemoveEmptyEntries);
        foreach(string timeline in timelines)
        {
            string[] lines = timeline.Split(new char[1] { ';' },System.StringSplitOptions.RemoveEmptyEntries);
            blockTime.Add(float.Parse(lines[0]));
            btn.Add(lines[1]);
        }
        isNewbie = true;

    }

    private void ResetGame()
    {
        miss.visible = false;
        controller.selectedIndex = 1;
        com.GetChild("n15").onClick.Set(StartGame);
        score = 0;
        scoreText.text = "得分:0";
    }

    int time;
    void StartGame()
    {
        com.GetChild("n15").onClick.Clear();
        controller.selectedIndex = 2;
        if (countdown == null)
            countdown = Resources.Load(SoundConfig.GetGameAudioUrl(SoundConfig.GameAudioId.Countdown)) as AudioClip;
        GRoot.inst.PlayEffectSound(countdown);

        countdownT.Play(() => { 
            GRoot.inst.PlayDialogSound(audioClip);
            controller.selectedIndex = 0;
            StartCoroutine(StartCreateBlock());
            time = (int)soundTime;
            Timers.inst.Add(1, 0, Countdown);
        timeText.text = "剩余时间:" + time;
        });
       
    }

    void Countdown(object param)
    {
        time--;
        timeText.text = "剩余时间:" + time;
        if (time <= 0)
        {
            Timers.inst.Remove( Countdown);
        }
    }

    void CreateBlock(int id)
    {
        GComponent block = UIPackage.CreateObject("Y_Game25", "music").asCom;
        com.AddChildAt(block,0);
        block.position = blockBornPosList[id];
        block.data = 0;
        
        blockQueue[id].Enqueue(block);
        block.TweenMoveY(1179f, 1.85f).SetEase(EaseType.Linear).OnComplete(() =>
        {
            lightCom[id].GetController("c1").selectedIndex = 1;
            if (isNewbie)
            {
                isNewbie = false;
                //GameTool.SaveGameInfo("music", "1", storyGameInfo.gamePointConfig.id);
                Time.timeScale = 0;
                tap.visible = true;
                Vector2 pos = lightCom[id].position;
                pos.x += 50;
                pos.y += 200;
                tap.position = pos;
            }
            block.TweenMoveY(1359f, 0.25f).SetEase(EaseType.Linear).OnComplete(() =>
            {
                lightCom[id].GetController("c1").selectedIndex = 0;
                GComponent com = blockQueue[id].Dequeue();
                if (com.data.ToString() == "0")
                {
                    Miss();
                }
                com.Dispose();
            });
        });
    }

    IEnumerator StartCreateBlock()
    {
        for (int i = 0; i < 4; i++)
        {
            blockQueue[i].Clear();
        }
        float lastTime = -1;
        for (int i = 0; i < blockTime.Count; i++)
        {
            string[] info = btn[i].Split(',');
            float time = blockTime[i];
            if (lastTime==-1)
            {
                yield return new WaitForSeconds(time);
                for(int j = 0; j < info.Length; j++)
                {
                    CreateBlock(int.Parse(info[j])-1);
                }
            }
            else
            {
                yield return new WaitForSeconds(time - lastTime);
                for (int j = 0; j < info.Length; j++)
                {
                    CreateBlock(int.Parse(info[j])-1);
                }
            }
            lastTime = time;
        }

    }

    private void Update()
    {
        if (controller.selectedIndex == 0)
        {
            if (!Stage.inst._audioDialog.isPlaying)
            {
                GameFinish();
            }
        }
    }

    void OnClick(int index)
    {
        if (blockQueue[index].Count > 0)
        {
            GComponent gCom = blockQueue[index].Peek();
            if (gCom.data.ToString() == "0")
            {
                int abs = (int)Mathf.Abs(gCom.position.y - lightCom[index].position.y);
                if (abs <= 95)
                {
                    score += 30;
                    scoreText.text = "得分:" + score;
                    gCom.data = 1;
                    Vector2 pos = lightCom[index].position;
                    pos.y = 686;
                    FXMgr.CreateEffectWithScale(bgFxCom, pos, "Game25_right", 162,2);
                    if (Time.timeScale == 0)
                    {
                        Time.timeScale = 1;
                        tap.visible = false;
                    }
                    GRoot.inst.PlayEffectSound(click);
                    return;
                }
                Miss();
            }
            
        }
        else
        {
            Miss();
        }
       
    }

    void Miss()
    {
        miss.visible = false;
        miss.visible = true;

    }

    void GameFinish()
    {
        controller.selectedIndex = 1;
        if (score >= 1000)
        {
            GameTipsInfo info = new GameTipsInfo()
            {
                context = "你完美的和思思一起完成了演奏！",
                callBack = OnComplete
            };
            UIMgr.Ins.showNextPopupView<GameSuccessView, GameTipsInfo>(info);
        }
        else
        {
            GameTipsInfo info = new GameTipsInfo()
            {
                context = "差一点就成功了!",
                callBack = ResetGame,
            };
            UIMgr.Ins.showNextPopupView<GameFailView, GameTipsInfo>(info);
        }
    }
}

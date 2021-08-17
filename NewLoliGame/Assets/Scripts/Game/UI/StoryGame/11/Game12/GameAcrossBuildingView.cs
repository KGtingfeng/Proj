using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using UnityEngine.Video;

/// <summary>
/// 穿越大楼游戏
/// </summary>
[ViewAttr("Game/UI/Y_Game12", "Y_Game12", "Game")]
public class GameAcrossBuildingView : BaseGameView
{
    GLoader videoLoader;
    VideoPlayer videoPlayer;
    GComponent sceneCom;
    GLoader bgLoder;
    GObject clickCircleCom;//用来表示被点击圆圈的位置
    GameObject go;
    VideoClip videoClip;
    GGraph fxGraph;
    AudioClip failAudio;
    AudioClip sucessAudio;

    GComponent tap;
    bool isStart;
    bool canClick;

    bool isNewbie;
    string newbieKey = "newbie";
    public override void InitUI()
    {
        base.InitUI();
        sceneCom = SearchChild("n3").asCom;
        controller = sceneCom.GetController("c1");
        videoLoader = sceneCom.GetChild("n6").asLoader;

        bgLoder = sceneCom.GetChild("n9").asLoader;
        bgLoder.url = UrlUtil.GetGameBGUrl(37);             //设置背景图片为视频第一帧

        fxGraph = sceneCom.GetChild("n7").asGraph;
        Object o = Resources.Load("Video/Prefabs/5");
        go = Instantiate(o) as GameObject;
        videoPlayer = go.GetComponent<VideoPlayer>();
        clickCircleCom = sceneCom.GetChild("n8");
        tap = sceneCom.GetChild("n10").asCom;
        tap.visible = false;
        InitEvent();

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

        sceneCom.GetChild("n3").onClick.Set(OnClickStart);

        clickCircleCom.onClick.Set(OnClickAcross);

        EventMgr.Ins.ReplaceEvent(EventConfig.STORY_GAME_QUIT, Destroy<GameAcrossBuildingView>);
        EventMgr.Ins.RemoveEvent(EventConfig.STORY_DELETE_GAME_INFO);
    }

    public override void InitData<D>(D data)
    {
        base.InitData(data);
        storyGameInfo = data as StoryGameInfo; 
        if (storyGameInfo.gameSaves.Count == 0)
        {
            isNewbie = true;
        }
    }

    public override void InitData()
    {
        base.InitData(); 
        isNewbie = true;

    }

    //private void Update()
    //{
    //    if (isStart)
    //    {
    //        videoLoader.texture = new NTexture(videoPlayer.texture);
    //    }

    //}

    private void OnClickStart()
    {
        isStart = true;
        controller.selectedIndex = 1;
        videoPlayer.time = 0;
        videoPlayer.Play();
        tap.visible = false;
        videoLoader.visible = true;
        videoLoader.touchable = true; 
        videoLoader.TweenMove(videoLoader.position, (float)videoPlayer.length).OnUpdate(() =>
        {
            videoLoader.texture = new NTexture(videoPlayer.texture);
        });
        StartCoroutine("PlayVideo");

    }
 
    readonly int clickTimes = 4;                //一共需要点击?次
    readonly float waitForClickTime = 1f;     //?秒内点击算成功
    readonly float waitForShowTime = 1f;      //?秒内变红
    float randomX = 0;
    float randomY = 0;
    private IEnumerator PlayVideo()
    {
        canClick = false;
        for (int i = 0; i < clickTimes; i++)
        {
            randomX = Random.Range(100, 650);
            randomY = Random.Range(400, 1300);
            clickCircleCom.SetPosition(randomX, randomY, 0);
            FXMgr.CreateEffectWithGGraph(fxGraph, new Vector3(randomX, randomY), "G12_tap1", 162, waitForShowTime * 2);
            yield return new WaitForSeconds(waitForShowTime);
            canClick = true;
            StartCoroutine("WaitClick");
            if (isNewbie)
            {
                isNewbie = false;
                tap.visible = true;
                tap.position = new Vector3(randomX+40, randomY+40);
                GameTool.SaveGameInfo(newbieKey, "1", storyGameInfo.gamePointConfig.id);
            }

            yield return new WaitForSeconds(waitForClickTime);
            tap.visible = false;

        }
        GameFinish();
    }

    private IEnumerator WaitClick()
    {
        yield return new WaitForSeconds(waitForClickTime);
        GameFail();
    }

    private void OnClickAcross()
    {
        if (canClick)
        {
            StopCoroutine("WaitClick");
            if (sucessAudio == null)
                sucessAudio = Resources.Load(SoundConfig.GAME_AUDIO_EFFECT_URL + (int)SoundConfig.GameAudioId.ClickClock) as AudioClip;
            GRoot.inst.PlayEffectSound(sucessAudio);
            canClick = false;
            FXMgr.CreateEffectWithGGraph(fxGraph, new Vector3(randomX, randomY), "G12_tap2", 162, 2);
        }
        else
        {
            GameFail();
        }
    }

    private void GameFinish()
    {
        Extrand extrand = new Extrand()
        {
            type = 2,
            extrand = OnComplete,
        };
        UIMgr.Ins.showNextPopupView<Game12TipsView, Extrand>(extrand);
    }


    private void GameFail()
    {
        StopCoroutine("PlayVideo");
        StopCoroutine("WaitClick");
        tap.visible = false;

        if (failAudio == null)
            failAudio = Resources.Load(SoundConfig.GAME_AUDIO_EFFECT_URL + (int)SoundConfig.GameAudioId.Error) as AudioClip;
        GRoot.inst.PlayEffectSound(failAudio);

        isStart = false;
        videoPlayer.Pause();
        videoPlayer.time = 0;
        controller.selectedIndex = 0;
      
        Extrand extrand = new Extrand()
        {
            type = 1,
            callBack = SkipGame,
        };
        UIMgr.Ins.showNextPopupView<Game12TipsView, Extrand>(extrand);
    }


    private void OnClickTips()
    {
        Extrand extrand = new Extrand()
        {
            type = 0,
        };
        UIMgr.Ins.showNextPopupView<Game12TipsView, Extrand>(extrand);
    }

    public override void Destroy<T>()
    {
        base.Destroy<T>();
        Destroy(go);
    }
}

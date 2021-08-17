using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

public class StoryChapterStaringMoudle : BaseMoudle
{
    static StoryChapterStaringMoudle ins;
    public static StoryChapterStaringMoudle Ins
    {
        get
        {
            return ins;
        }
    }

    TypingEffect titleTypingEffect;
    TypingEffect contextTypingEffect;
    GTextField titleText;
    GTextField contextText;
    GLoader backGroundLoader;
    GameChapterConfig gameChapterConfig;
    Transition transition;

    GComponent fxCom;

    public override void InitMoudle(GComponent gComponent, int controllerIndex)
    {
        base.InitMoudle(gComponent, controllerIndex);

        titleText = SearchChild("n1").asTextField;
        contextText = SearchChild("n2").asTextField;
        backGroundLoader = SearchChild("n3").asLoader;

        titleTypingEffect = new TypingEffect(titleText);
        contextTypingEffect = new TypingEffect(contextText);
        fxCom = SearchChild("n10").asCom;
         FXMgr.CreateEffectWithScale(fxCom, new Vector3(235, -24, -4), "UI_zhangjiexumu", 162, -1);
        transition = ui.GetTransition("t2");

        ins = this;
        InitEvent();
    }


    public override void InitData<D>(D data)
    {

        base.InitData(data);
        gameChapterConfig = data as GameChapterConfig;
        if (gameChapterConfig != null)
        {
            titleText.text = gameChapterConfig.name;
            contextText.text = gameChapterConfig.title;
            backGroundLoader.url = UrlUtil.GetStoryBgUrl(int.Parse(gameChapterConfig.background_id));
            InitTypeEffect();
            GamePointConfig gamePointConfig = DataUtil.GetPointConfig(gameChapterConfig.startPoint);
            EventMgr.Ins.DispachEvent(EventConfig.MUSIC_STORY_BG_MUSIC, gamePointConfig);
            if (GameData.isGuider)
            {
                GameData.guiderCurrent = GuiderInfoLinked.GetCurrGuiderInfo(1, 1);
                UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);
            }
            transition.Play();
        }

    }
    public override void InitTypeEffect()
    {
        base.InitTypeEffect();
        //titleTypingEffect.Start();
        contextTypingEffect.Start();
        //typingEffects.Add(titleTypingEffect);
        typingEffects.Add(contextTypingEffect);

        PrintTex();
    }


    public override void InitEvent()
    {
        SearchChild("n3").onClick.Set(OnClick);
    }

    public void OnClick()
    {
        if (SpeedPrint())
        {
            CallBackList callBackList = new CallBackList();
            callBackList.callBack1 = ContinueChapter;
            UIMgr.Ins.showNextPopupView<ShanbaiAnimationView, CallBackList>(callBackList);
        }
           
    }

    void ContinueChapter()
    {
        //这里是要做判断的 各个条件组成
        StoryInfo storyInfo = StoryDataMgr.ins.StoryInfo;
        List<GameChapterConfig> chapterConfigs = JsonConfig.GameChapterConfigs.FindAll(a => a.actor_id == storyInfo.actor_id);

        storyInfo.gameNodeConfig = DataUtil.GetNodeConfig(storyInfo.chapterId, storyInfo.node_id);
        EventMgr.Ins.DispachEvent(EventConfig.STORY_TRIGGER_LOOP, storyInfo);
    }

    void PrintText(object param)
    {
        if (!titleTypingEffect.Print())
        {
            Timers.inst.Remove(PrintText);
        }
    }
}

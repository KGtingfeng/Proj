using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

/// <summary>
/// 过场moudle
/// </summary>
public class StoryTransitionMoudle : BaseMoudle
{

    static StoryTransitionMoudle ins;
    public static StoryTransitionMoudle Ins
    {
        get
        {
            return ins;
        }
    }

    TypingEffect typingEffect;

    GComponent component;
    GTextField contextText;
    GLoader backGroundLoader;
    GamePointConfig gamePointConfig;
    int backGroundId = -1;
    public override void InitMoudle(GComponent gComponent, int controllerIndex)
    {
        base.InitMoudle(gComponent, controllerIndex);
        component = SearchChild("n10").asCom;
        backGroundLoader = SearchChild("n9").asLoader;


        contextText = component.GetChild("n3").asTextField;

        typingEffect = new TypingEffect(contextText);

        InitEvent();
        ins = this;
    }

    public override void InitData<D>(D data)
    {
        typingEffects.Clear();
        typingEffects.Add(typingEffect);
        gamePointConfig = data as GamePointConfig;
        if (gamePointConfig != null)
        {
            backGroundLoader.url = UrlUtil.GetStoryBgUrl(gamePointConfig.background_id);
            contextText.text = DataUtil.ReplaceCharacterWithStarts(gamePointConfig.content1);
            InitTypeEffect();
            if (gamePointConfig.type > 4)
            {
                backGroundLoader.TweenScale(Vector2.one * 1.5f, 1f);
            }
            else
            {
                backGroundLoader.scale = Vector2.one;
            }
            SearchChild("n2").visible = true;

            if (GameData.isGuider)
            {

                GameData.guiderCurrent = GuiderInfoLinked.GetCurrGuiderInfo(1, 12);
                UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);
                SearchChild("n2").visible = false;

            }
        }
    }


    public override void InitTypeEffect()
    {
        base.InitTypeEffect();

        typingEffect.Start();
        typingEffects.Add(typingEffect);
        PrintTex();
    }


    public override void InitEvent()
    {
        SearchChild("n2").onClick.Set(() =>
        {
            EventMgr.Ins.DispachEvent(EventConfig.STORY_BREACK_STORY);
        });
        SearchChild("n10").onClick.Set(() =>
        {
            OnClickNext();

        });

        //剧情回顾
        //SearchChild("n1").onClick.Add(() =>
        //{
        //    EventMgr.Ins.DispachEvent(EventConfig.STORY_PLAY_RECORD);
        //});


        //加速

        //SearchChild("n0").onClick.Add(() =>
        //{
        //    //EventMgr.Ins.DispachEvent(EvenConfig.STORY_BREACK_STORY);
        //}); 
    }

    private void OnClickNext()
    {
        if (SpeedPrint())
        {
            NormalInfo normalInfo = new NormalInfo();
            normalInfo.index = gamePointConfig.point1;
            Debug.Log(gamePointConfig.id);
            if (StoryDataMgr.ins.StoryInfo.isReRead)
            {
                StoryDataMgr.ins.StoryInfo.RemoveNodes();
                if (StoryDataMgr.ins.StoryInfo.nodes.Count > 0)
                {
                    normalInfo.index = StoryDataMgr.ins.StoryInfo.nodes[0];
                }
                else
                {
                    normalInfo.index = 0;
                }
            }

            EventMgr.Ins.DispachEvent(EventConfig.STORY_TRIGGER_NEXT_NODE, normalInfo);
        }
    }

    public void NewbieOnClickNext()
    {
        SpeedPrint();
            
        StoryDataMgr.ins.saveNodes.Add(gamePointConfig.id);
        WWWForm wWForm = new WWWForm();
        wWForm.AddField("actorId", GameGuideConfig.GuideActor);
        wWForm.AddField("chapterId", 1);
        wWForm.AddField("nodeId", 0);
        GameMonoBehaviour.Ins.RequestInfoPost<PlayerStoryInfo>(NetHeaderConfig.STORY_RECORD_NODE, wWForm, callBack);

        //EventMgr.Ins.DispachEvent(EventConfig.STORY_TRIGGER_NEXT_NODE, normalInfo);
    }

    void callBack()
    {
        AudioClip audioClip = ResourceUtil.LoadBackGroundMusic(SoundConfig.CommonMusicId.BgId);
        GRoot.inst.PlayBgSound(audioClip);

        TouchScreenView.Ins.PlayChangeEffect(() =>
        {
            UIMgr.Ins.showNextView<InteractiveView>();
        });
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using Spine.Unity;
public class BeginGame : MonoBehaviour
{
    public static BeginGame ins;
    RenderTexture renderTexture;

    private void Awake()
    {
        initUiInfos();

    }

    private void Start()
    {
        //AnnotationMethod();
        SplahView splahView = UIMgr.Ins.showNextPopupView<SplahView>() as SplahView;

        //CallBackList callBackList = new CallBackList();
        //callBackList.callBack1 = () =>
        //{
        //    Debug.Log("播放结束...");
        //};
        //UIMgr.Ins.showNextPopupView<StoryAnimationView, CallBackList>(callBackList);
    }

    void initUiInfos()
    {
        UIConfig.defaultFont = "Source Han Sans CN Normal";

        AudioClip audioClip = ResourceUtil.LoadEffect(SoundConfig.CommonEffectId.ClickBtn);
        UIConfig.buttonSound = new NAudioClip(audioClip);
        UIConfig.buttonSoundVolumeScale = 0.5f;

        ins = this;
        GameObject gameMb = new GameObject();
        gameMb.AddComponent<GameMonoBehaviour>();
        gameMb.AddComponent<WordAplhaEffect>();
        //gameMb.AddComponent<GameLife>();
        gameMb.AddComponent<AsyncRequestMgr>();
        gameMb.AddComponent<SmallLoadingMgr>();
        //gameMb.AddComponent<EventBehaviour>();
        gameMb.name = "GameMonoBehaviour";
        DontDestroyOnLoad(gameMb);
#if UNITY_ANDROID || UNITY_EDITOR
        if (SDKController.Instance == null)
        {
            GameObject go = new GameObject();
            go.AddComponent<SDKController>();
            go.name = "WDS_ANDROID_UNITY_MSG";
            DontDestroyOnLoad(go);
        }
#endif
        UIPackage.AddPackage("Game/UI/T_Common");
        UIPackage.AddPackage("Game/UI/X_Choose doll");
        UIPackage.AddPackage("Game/UI/J_Story");
        UIPackage.AddPackage("Game/UI/Y_Game13");
        UIPackage.AddPackage("Game/UI/Y_Game_common");
        UIPackage.AddPackage("Game/UI/X_Beginner_guidance");
        UIPackage.AddPackage("Game/UI/D_call");
        UIMgr.Ins.uiParent = transform;

    }

    //当前被注释的方法
    void AnnotationMethod()
    {

        //UIMgr.Ins.showNextView<GameAviodView>();
        UIMgr.Ins.showNextView<GameClickDefendsView>();
        //UIMgr.Ins.showNextView<GamePuzzleWatchView>();
        //loginView.InitData();

        //TouchScreenView cc = UIMgr.Ins.showNextPopupView<GameSelectTimeView>() as TouchScreenView;

        //RoleGropView loginView = UIMgr.Ins.showNextView<RoleGropView>() as RoleGropView;
        //loginView.InitData(new NormalInfo());
        //UIMgr.Ins.showWindow<StoryPlayRecordWindow, NormalInfo>(null);

        //UIMgr.Ins.showWindow<SelectCalendarWindow>();
        //UIMgr.Ins.showWindow<StoryNotEnoughAttributeWindow, GameInitCardsConfig>(null);
        //SelectCalendarView loginView = UIMgr.Ins.showNextView<SelectCalendarView>() as SelectCalendarView;
        //loginView.InitData();

        //注册过场动画
        //EventMgr.Ins.RegisterEvent<CallBackList>(EventConfig.STORY_ANIMATION_VIEW, ShowStoryAnimationView);
        //隐藏过场动画
        //EventMgr.Ins.RegisterEvent(EvenConfig.STORY_HIDE_ANIMATION_VIEW, HideStoryAnimation);

        //RequestStoryBaseInfo();
        //StoryView loginView = UIMgr.Ins.showNextView<StoryView>() as StoryView;
        //Debug.Log("loginView " + loginView);
        //loginView.InitData();




        //CreateRoleView loginView = UIMgr.Ins.showNextView<CreateRoleView>() as CreateRoleView;
        //Debug.Log("loginView " + loginView);
        //loginView.InitData();


        //UIMgr.Ins.ShowViewWithoutHideBefore<StoryAnimationView, NormalInfo>(null); 
        //DateTime dateTime = new DateTime();
        //Debug.Log(dateTime.Days)

        //Debug.Log( "天数： " + DateTime.DaysInMonth(2020, 2));

    }

    void RequestStoryBaseInfoSuccess()
    {
    }


    /// <summary>
    /// 显示过场动画
    /// </summary>
    /// <param name="callback">Callback.</param>
    void ShowStoryAnimationView(CallBackList callback)
    {
        UIMgr.Ins.ShowViewWithoutHideBefore<StoryAnimationView, CallBackList>(callback);
    }

    /// <summary>
    /// 隐藏过场动画
    /// </summary>
    void HideStoryAnimation()
    {
        UIMgr.Ins.HideView<StoryAnimationView>();
    }

}
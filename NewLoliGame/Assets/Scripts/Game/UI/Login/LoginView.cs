using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;


[ViewAttr("Game/UI/D_Login", "D_Login", "Login")]
public class LoginView : BaseView
{
    public static LoginView instance;

    GGraph fxGraph;
    public enum MoudleType
    {
        First = 0,
        Update,
        Loading,
        Announce,
        Login,
        Registe,
        SelectServer,
        ModeifyPwd,
        GetPwd
    };


    Dictionary<MoudleType, string> moudleNamePairs = new Dictionary<MoudleType, string>()
    {
        {MoudleType.Update,"n27"},
        {MoudleType.Loading,"n30"},
        {MoudleType.Announce,"n5"},
        {MoudleType.Login,"n7"},
        {MoudleType.Registe,"n8"},
        //{MoudleType.SelectServer,"n27"}, 特殊处理
        {MoudleType.ModeifyPwd,"n25"},
        {MoudleType.GetPwd,"n26"},
    };



    GButton loginButton;


    public override void InitUI()
    {
        base.InitUI();
        instance = this;
        controller = ui.GetController("c1");
        GLoader gLoader = SearchChild("n36").asLoader;
        gLoader.url = UrlUtil.GetLoginBgUrl();
        fxGraph = SearchChild("n43").asCom.GetChild("n17").asGraph;

        InitEvent();

        FXMgr.CreateEffectWithGGraph(fxGraph, new Vector3(697, 932, 1485), "UI_denglujiemian", 494);

        PlayerCommonBgMusic();

    }



    public override void InitEvent()
    {
        base.InitEvent();
        //login
        ui.GetChild("n2").onClick.Set(() =>
        {
            //UIMgr.Ins.ShowToastView<CommonGetPropsToastView, TinyItem>(null);
            GoToLoginMoudle();
        });

        //register
        ui.GetChild("n32").onClick.Set(() =>
        {
            GoToMoudle<RegisterMoudle>((int)MoudleType.Registe);

        });
        EventMgr.Ins.RegisterEvent(EventConfig.GOTO_VIEW_MAIN_EFFECT, GotoMain);
        //登录
        EventMgr.Ins.RegisterEvent(EventConfig.LOGIN_GOTO_LOGIN, GoToLoginMoudle);
        //注册背景音乐
        EventMgr.Ins.RegisterEvent(EventConfig.MUSIC_COMMON_BG_MUSIC, PlayerCommonBgMusic);
        //注册通用音效
        EventMgr.Ins.RegisterEvent<NormalInfo>(EventConfig.MUSIC_COMMON_EFFECT, PlayerCommonEffectMusic);
        EventRegisterMgr.Ins.RegistEvent();
    }

    //void Goto()
    //{
    //    StartCoroutine(Gotointer());
    //}

    //IEnumerator Gotointer()
    //{
    //    TouchScreenView.instance.PlayTmpEffect();
    //    yield return new WaitForSeconds(0.8f);
    //    UIMgr.Ins.showNextView<TestView>();
    //}

    void GotoMain()
    {
        CallBackList callBackList = new CallBackList();
        callBackList.callBack1 = () => {
        UIMgr.Ins.showViewWithReleaseOthers<MainView>();
        };
        UIMgr.Ins.showNextPopupView<LoginAnimationView, CallBackList>(callBackList);
        //StartCoroutine(GoToEffect());
    }


    IEnumerator GoToEffect()
    {
        TouchScreenView.Ins.PlayTmpEffect();
        SDKController.Instance.StartService();

        yield return new WaitForSeconds(0.8f);
        UIMgr.Ins.showViewWithReleaseOthers<MainView>();
    }

    void PlayerCommonBgMusic()
    {
        AudioClip audioClip = ResourceUtil.LoadBackGroundMusic(SoundConfig.CommonMusicId.BgId);
        GRoot.inst.PlayBgSound(audioClip);
    }

    void PlayerCommonEffectMusic(NormalInfo normalInfo)
    {
        AudioClip audioClip = ResourceUtil.LoadEffect((SoundConfig.CommonEffectId)normalInfo.index);
        GRoot.inst.PlayEffectSound(audioClip);
    }


    void GoToLoginMoudle()
    {
        //TinyItem tinyItem = new TinyItem();
        //tinyItem.name = "亮彩";
        //tinyItem.id = 1;
        //UIMgr.Ins.showNextPopupView<GetRoleView, TinyItem>(tinyItem);
        GoToMoudle<LoginMoudle>((int)MoudleType.Login);
    }

    public override void GoToMoudle<T>(int index)
    {
        MoudleType moudleType = (MoudleType)index;
        BaseMoudle baseMoudle = FindMoudle<T>();
        if (baseMoudle == null)
        {
            baseMoudle = (BaseMoudle)Activator.CreateInstance(typeof(T));
            baseMoudles.Add(baseMoudle);
            GComponent gComponent = null;
            //get componment
            if (moudleType != MoudleType.SelectServer)
            {
                string componmentName;
                if (moudleNamePairs.TryGetValue(moudleType, out componmentName))
                {
                    GObject gObject = ui.GetChild(componmentName);
                    if (gObject == null)
                    {
                        Debug.Log(componmentName + " not find, please check it");
                        return;
                    }
                    gComponent = gObject.asCom;
                }
            }
            else
            {
                gComponent = ui;
            }
            baseMoudle.baseView = this;
            baseMoudle.InitMoudle(gComponent, index);
        }
        baseMoudle.InitData();
        SwitchController(index);
    }


}

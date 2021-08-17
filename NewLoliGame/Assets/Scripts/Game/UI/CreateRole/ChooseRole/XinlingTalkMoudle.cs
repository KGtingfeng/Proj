using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using UnityEngine.Video;
using System.IO;
using Spine.Unity;

public class XinlingTalkMoudle : BaseMoudle
{

    GLoader bgLoader;
    GTextField content;
    GComponent headCom;
    GLoader headLoader;
    GTextField nameText;
    Controller controller;
    Controller headController;
    GLoader videoLoader;
    VideoPlayer videoPlayer;
    GameObject go;
    GameInitCardsConfig doll;

    GGraph role;

    GObject gGraph;

    TypingEffect typingEffect;
    RenderTexture renderTexture;
    SkeletonAnimation skeletonAnimation
    {
        get { return RoleMgr.roleMgr.GetSkeletonAnimation; }
    }

    Transition transitionMy;
    Transition transitionOther;
    Transition transitionTran;
    GLoader dialogBgLoader;
    GComponent finishCom;


    bool isPlaying;
    bool isEffect;

    public override void InitMoudle<T>(GComponent gComponent, int controllerIndex, T data)
    {
        base.InitMoudle(gComponent, controllerIndex, data);


        InitUI();
    }

    public override void InitUI()
    {
        bgLoader = SearchChild("n5").asLoader;
        headCom = SearchChild("n9").asCom;

        content = headCom.GetChild("n10").asCom.GetChild("n11").asTextField;
        typingEffect = new TypingEffect(content);

        controller = ui.GetController("c1");
        headLoader = headCom.GetChild("n10").asCom.GetChild("n1").asCom.GetChild("n6").asLoader;
        nameText = headCom.GetChild("n10").asCom.GetChild("n1").asCom.GetChild("n2").asTextField;
        headController = headCom.GetChild("n10").asCom.GetChild("n1").asCom.GetController("c1");
        role = SearchChild("n10").asGraph;
        videoLoader = SearchChild("n11").asLoader;

        gGraph = SearchChild("n13");

        transitionMy = headCom.GetTransition("t0");
        transitionOther = headCom.GetTransition("t1");
        transitionTran = headCom.GetTransition("t2");
        dialogBgLoader = headCom.GetChild("n10").asCom.GetChild("n10").asLoader;
        finishCom = headCom.GetChild("n10").asCom.GetChild("n14").asCom;
        renderTexture = Resources.Load(UrlUtil.GetVideoUrl("0")) as RenderTexture;
        SetRoleSpine();
        InitEvent();
    }
    GoWrapper goWrapper;
    RoleMappingCom roleMappingCom;
    void SetRoleSpine()
    {



        //GameObject go = FXMgr.CreateRoleSpine(18, 0);
        //skeletonAnimation = go.GetComponentsInChildren(typeof(SkeletonAnimation))[0] as SkeletonAnimation;
        //if (goWrapper == null)
        //{
        //    goWrapper = new GoWrapper(go);
        //}
        //else
        //{
        //    GameObject gobj = goWrapper.wrapTarget;
        //    goWrapper.wrapTarget = go;
        //    GameObject.Destroy(gobj);
        //}
        //role.SetNativeObject(goWrapper);
        //goWrapper.scale = Vector2.one * 108;

        //role.position = new Vector2(388, 1765);   

        FXMgr.CreateRoleSpineForAlpha(18, 0);
        GameObject go = RoleMgr.roleMgr.GetMapingCom();
        if (goWrapper == null)
        {
            goWrapper = new GoWrapper(go);
        }
        else
        {
            GameObject gobj = goWrapper.wrapTarget;
            goWrapper.wrapTarget = go;
            GameObject.Destroy(gobj);
        }
        role.SetNativeObject(goWrapper);
        go.transform.localPosition = new Vector3(369, -1107, 1000);
        go.transform.localScale = new Vector3(857.0313f, 1855.758f, 114.2708f);
        roleMappingCom = go.GetComponent<RoleMappingCom>();



    }

    public override void InitEvent()
    {
        SearchChild("n6").onClick.Set(OnClickGotoChoose);
        EventMgr.Ins.RegisterEvent(EventConfig.CHECK_SOUND, CheckSound);

        onTypingComplete = () =>
        {
            finishCom.visible = true;
        };
    }

    public override void InitData<D>(D data)
    {
        base.InitData(data);

        doll = data as GameInitCardsConfig;
        Debug.Log(GameData.guiderCurrent.guiderInfo.type + "      " + GameData.guiderCurrent.guiderInfo.step);
        Refresh();
    }
    void XinLinHeadCom()
    {
        headCom.visible = true;
        isEffect = true;
        nameText.text = "辛灵";
        headLoader.url = UrlUtil.GetDialogHeadIconUrl(18);
        headController.selectedIndex = 1;
        transitionOther.Play(1, 0, 0, 0.5f, () =>
        {
            isEffect = false;
        });

    }
    void SelfHeadCom()
    {
        headCom.visible = true;
        isEffect = true;
        nameText.text = "我";
        headLoader.url = UrlUtil.GetStorySelfHeadIconUrl(0);
        headController.selectedIndex = 0;
        transitionMy.Play(1, 0, 0, 0.5f, () =>
        {
            isEffect = false;
        });
    }

    private void RefreshInfo()
    {
        finishCom.visible = false;
        GRoot.inst.StopDialogSound();
        isPlaying = false;
        if (!string.IsNullOrEmpty(GameData.guiderCurrent.guiderInfo.cursor_axis))
        {
            SpineCtr.SwitchAnimationState(skeletonAnimation, new int[] { int.Parse(GameData.guiderCurrent.guiderInfo.cursor_axis) });
        }

        if (GameData.guiderCurrent.guiderInfo.type == 3)
        {
            content.text = "[color=#ffffff]" + GameData.guiderCurrent.guiderInfo.contents + "[/color]";
        }
        else
        {
            content.text = GameData.guiderCurrent.guiderInfo.contents;
        }
        typingEffect.Start();
        typingEffects.Add(typingEffect);
        PrintTex();

        if (GameData.guiderCurrent.guiderInfo.actor_voice != "0")
        {
            AudioClip audioClip = Resources.Load<AudioClip>(UrlUtil.GetNewbieBgmUrl(GameData.guiderCurrent.guiderInfo.actor_voice));
            GRoot.inst.PlayDialogSound(audioClip);
            isPlaying = true;
        }
    }

    void CheckSound()
    {
        if (isPlaying)
        {
            if (!GRoot.inst.GetDialogIsPlaying())
            {
                isPlaying = false;
                if (string.IsNullOrEmpty(GameData.guiderCurrent.guiderInfo.cursor_axis))
                    return;
                if (SpineConfig.talkSpine.Contains(int.Parse(GameData.guiderCurrent.guiderInfo.cursor_axis)))
                {
                    if (int.Parse(GameData.guiderCurrent.guiderInfo.cursor_axis) == 11)
                    {
                        SpineCtr.ResetAniamtionToIdle(skeletonAnimation);
                    }
                    else
                    {
                        SpineCtr.SwitchAnimationState(skeletonAnimation, new int[] { int.Parse(GameData.guiderCurrent.guiderInfo.cursor_axis) - 1 });
                    }
                }
            }
        }
    }


    private void OnClickGotoChoose()
    {
        TouchScreenView.Ins.PlayChangeEffect(() =>
        {
            baseView.GoToMoudle<ChooseRoleMoudle, Card>((int)ChooseRoleView.MoudleType.ChooseDoll, null);
        });

    }

    private void OnClickCreateRole()
    {

        CallBackList callBackList = new CallBackList();
        callBackList.callBack1 = () =>
        {
            UIMgr.Ins.showNextPopupView<CreateRoleView, GameInitCardsConfig>(doll);
        };

        UIMgr.Ins.showNextPopupView<ShanbaiAnimationView, CallBackList>(callBackList);
    }

    private void OnClickNext()
    {

        if (isEffect)
            return;
        GameData.guiderCurrent = GameData.guiderCurrent.Next;
        Debug.Log(GameData.guiderCurrent.guiderInfo.type + "      " + GameData.guiderCurrent.guiderInfo.step + "       !!");
        Refresh();
    }

    void Refresh()
    {
        headCom.visible = false;
        content.text = "";
        switch (GameData.guiderCurrent.guiderInfo.type)
        {
            case 1:
                if (!string.IsNullOrEmpty(GameData.guiderCurrent.guiderInfo.actor))
                    bgLoader.url = UrlUtil.GetGameBGUrl(int.Parse(GameData.guiderCurrent.guiderInfo.actor));
                ui.onClick.Set(OnClickNext);
                controller.selectedIndex = 3;
                if (GameData.guiderCurrent.guiderInfo.step == 2)
                {
                    isEffect = true;
                    bgLoader.alpha = 0.5f;
                    bgLoader.scale = Vector2.one * 1.5f;
                    bgLoader.TweenFade(1, 1f);
                    bgLoader.TweenScale(Vector2.one, 3.5f).OnComplete(() =>
                    {
                        RefreshInfo();
                        SelfHeadCom();
                        dialogBgLoader.url = "ui://6f05wxg5gab6q3n";
                    });
                }
                else
                {
                    RefreshInfo();
                    SelfHeadCom();
                    dialogBgLoader.url = "ui://6f05wxg5gab6q3n";
                }

                break;
            case 2:
                if (!string.IsNullOrEmpty(GameData.guiderCurrent.guiderInfo.actor))
                    bgLoader.url = UrlUtil.GetGameBGUrl(int.Parse(GameData.guiderCurrent.guiderInfo.actor));
                ui.onClick.Set(OnClickNext);
                controller.selectedIndex = 0;
                if (GameData.guiderCurrent.guiderInfo.step == 5)
                {
                    //role.visible = false;
                    if (roleMappingCom != null)
                        roleMappingCom.FadeOut();

                    isEffect = true;
                    bgLoader.alpha = 0.5f;
                    bgLoader.scale = Vector2.one * 1.3f;
                    bgLoader.TweenFade(1, 1f);
                    bgLoader.TweenScale(Vector2.one * 1.1f, 3f).OnComplete(() =>
                        {
                            bgLoader.TweenMoveX(bgLoader.x - 200, 1.5f).OnComplete(() =>
                            {
                                bgLoader.TweenMoveX(bgLoader.x + 200, 0.4f).OnComplete(() =>
                                {
                                    //role.visible = true;
                                    if (roleMappingCom != null)
                                        roleMappingCom.FadeIn();
                                    RefreshInfo();
                                    XinLinHeadCom();
                                    dialogBgLoader.url = "ui://6f05wxg5gab6q3n";
                                });
                            });
                        });
                }
                else
                {
                    RefreshInfo();
                    XinLinHeadCom();
                    dialogBgLoader.url = "ui://6f05wxg5gab6q3n";
                }

                break;
            case 3:
                if (!string.IsNullOrEmpty(GameData.guiderCurrent.guiderInfo.actor))
                    bgLoader.url = UrlUtil.GetGameBGUrl(int.Parse(GameData.guiderCurrent.guiderInfo.actor));
                headCom.visible = true;
                isEffect = true;
                ui.onClick.Set(OnClickNext);
                controller.selectedIndex = 2;
                headController.selectedIndex = 2;
                transitionTran.Play(1, 0, 0, 0.4f, () =>
                {
                    isEffect = false;
                    RefreshInfo();

                });
                dialogBgLoader.url = "ui://6f05wxg5gab6q3m";
                break;

            case 4:
                Debug.LogError("   videoPlayer  ");
                isEffect = false;
                if (GameData.guiderCurrent.guiderInfo.step == 4)
                {
                    CallBackList callBackList = new CallBackList();
                    callBackList.callBack1 = () =>
                    {
                        baseView.StartCoroutine(PlayVideo());
                    };
                    UIMgr.Ins.showNextPopupView<LoginWaitimeView, CallBackList>(callBackList);

                }
                else
                {
                    baseView.StartCoroutine(PlayVideo());
                }
                ui.onClick.Remove(OnClickNext);
                videoLoader.touchable = true;
                videoLoader.visible = false;
                GRoot.inst.StopBgSound();
                break;
            case 5:
                GRoot.inst.StopDialogSound();
                ui.onClick.Remove(OnClickNext);
                OnClickGotoChoose();
                return;
            case 6:
                GRoot.inst.StopDialogSound();
                OnClickCreateRole();
                return;
        }


    }

    void OnClickVideo()
    {
        baseView.StopAllCoroutines();
        if (GameData.guiderCurrent.guiderInfo.step == 1)
        {
            videoPlayer.Pause();
            CallBackList callBackList = new CallBackList();
            callBackList.callBack1 = () =>
            {
                videoPlayer.Stop();
                AudioClip audioClip = ResourceUtil.LoadBackGroundMusic(SoundConfig.CommonMusicId.BgId);
                GRoot.inst.PlayBgSound(audioClip);
                OnClickNext();
            };
            UIMgr.Ins.showNextPopupView<ShanbaiAnimationView, CallBackList>(callBackList);
        }
        else
        {
            CallBackList callBackList = new CallBackList();
            callBackList.callBack1 = () =>
            {
                //baseView.GoToMoudle<XinlingTalkMoudle, GameInitCardsConfig>((int)ChooseRoleView.MoudleType.Talk, doll);
                videoPlayer.Stop();
                AudioClip audioClip = ResourceUtil.LoadBackGroundMusic(SoundConfig.CommonMusicId.BgId);
                GRoot.inst.PlayBgSound(audioClip);
                OnClickNext();
            };
            UIMgr.Ins.showNextPopupView<StoryAnimationView, CallBackList>(callBackList);
        }

    }

    IEnumerator PlayVideo()
    {
        Object o = Resources.Load(UrlUtil.GetVideoUrl(GameData.guiderCurrent.guiderInfo.actor));
        GameObject go = GameObject.Instantiate(o) as GameObject;
        videoPlayer = go.GetComponent<VideoPlayer>();
        videoPlayer.time = 0;
        videoPlayer.isLooping = false;
        videoPlayer.playOnAwake = false;
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.targetTexture = renderTexture;
        videoPlayer.aspectRatio = VideoAspectRatio.NoScaling;
        videoPlayer.Prepare();
        yield return new WaitUntil(() => { return videoPlayer.isPrepared; });
        videoPlayer.Play();
        yield return new WaitUntil(() => { return videoPlayer.frame >= 1; });
        controller.selectedIndex = 4;

        videoLoader.texture = new NTexture(videoPlayer.targetTexture);
        videoLoader.visible = true;
        EventMgr.Ins.DispachEvent(EventConfig.LOGIN_HIDE);
        if (GameData.guiderCurrent.guiderInfo.step == 4)
        {

            baseView.StartCoroutine(WaitEffect(96, true));   
            //videoLoader.onClick.Add(OnClickVideo);
        }
        else
        {
            //baseView.StartCoroutine(WaitPause(26));   

            baseView.StartCoroutine(WaitPause(658));
            //videoLoader.onClick.Add(OnClickVideo);
        }
    }

    int type;
    IEnumerator WaitPause(float time)
    {
        //yield return new WaitForSeconds(time);
        yield return new WaitUntil(() => { return videoPlayer.frame >= time; });
        int frame = 13;
        if (videoPlayer.canSetPlaybackSpeed)
        {
            ;
            gGraph.x = 1;
            gGraph.TweenMoveX(0, 1f).SetEase(EaseType.Linear).OnUpdate(() =>
            {
                videoPlayer.SetDirectAudioVolume(0, gGraph.x);
                videoPlayer.playbackSpeed = gGraph.x;
            });
            frame = 0;
            yield return new WaitForSeconds(1f);

        }

        videoPlayer.Pause();

        Extrand extrand = new Extrand();
        extrand.type = type;
        type++;
        if (type == 3)
        {
            extrand.callBack = () =>
            {
                //baseView.StartCoroutine(WaitEffect(16f));
                baseView.StartCoroutine(WaitEffect(2234));
                videoPlayer.Play();
                RebackSpeed();
            };
            UIMgr.Ins.showNextPopupView<VideoButtonView, Extrand>(extrand);
        }
        else
        {
            if (type == 1)
            {
                extrand.callBack = () =>
                {
                    //baseView.StartCoroutine(WaitPause(29.8f));
                    baseView.StartCoroutine(WaitPause(1386 + frame));
                    videoPlayer.Play();
                    RebackSpeed();

                };
            }
            else
            {
                extrand.callBack = () =>
                {
                    //baseView.StartCoroutine(WaitPause(17.2f));
                    baseView.StartCoroutine(WaitPause(1819 + frame));
                    videoPlayer.Play();
                    RebackSpeed();

                };
            }
            UIMgr.Ins.showNextPopupView<VideoButtonView, Extrand>(extrand);
        }
    }

    void RebackSpeed()
    {
        if (videoPlayer.canSetPlaybackSpeed)
        {
            gGraph.x = 0;
            gGraph.TweenMoveX(1, 1f).SetEase(EaseType.Linear).OnUpdate(() =>
            {
                videoPlayer.SetDirectAudioVolume(0, gGraph.x);
                videoPlayer.playbackSpeed = gGraph.x;
            });
        }
        else
        {
            videoPlayer.SetDirectAudioVolume(0, 1);
            videoPlayer.playbackSpeed = 1;
        }
    }

    int tmpTimes;
    long beforeFrame;
    IEnumerator WaitEffect(float time, bool isSecond = false)
    {
        //yield return new WaitForSeconds(time);
        beforeFrame = 0;
        tmpTimes = 0;

        yield return new WaitUntil(() =>
        {

            //Debug.Log("video test   " + videoPlayer.frame + "  " + time);
            if (isSecond)
            {
                //Debug.Log("video test   frame " + videoPlayer.frame + " beforeFrame " + beforeFrame);
                if (beforeFrame != videoPlayer.frame)
                {
                    tmpTimes = 0;
                    beforeFrame = videoPlayer.frame;
                }
                else
                    tmpTimes++;

                if (tmpTimes >= 5)
                {
                    //Debug.Log("video test  three times 。。。");
                    return true;
                }
            }
            return videoPlayer.frame >= time;


        });
        videoPlayer.Pause();


        CallBackList callBackList = new CallBackList();
        callBackList.callBack1 = () =>
        {

            videoPlayer.Stop();
            AudioClip audioClip = ResourceUtil.LoadBackGroundMusic(SoundConfig.CommonMusicId.BgId);
            GRoot.inst.PlayBgSound(audioClip);
            //出UI
            OnClickNext();
        };

        if (GameData.guiderCurrent.guiderInfo.step == 4)
        {
            UIMgr.Ins.showNextPopupView<ShanbaiAnimationView, CallBackList>(callBackList);
        }
        else
        {
            UIMgr.Ins.showNextPopupView<LoginAnimationView, CallBackList>(callBackList);

        }

    }

}

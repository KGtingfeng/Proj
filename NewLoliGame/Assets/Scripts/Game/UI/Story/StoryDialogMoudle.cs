using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using Spine.Unity;
using System;

public class StoryDialogMoudle : BaseMoudle
{

    static StoryDialogMoudle ins;
    public static StoryDialogMoudle Ins
    {
        get
        {
            return ins;
        }
    }
    TypingEffect typingEffect;
    GLoader backgroundLoader;
    GLoader nobodyLoader;

    GTextField contentText;
    GGraph spineGraph;
    Transition dialogTransition;
    Transition nobodyTransition;

    GTextField npcNameText;
    GComponent dialogGCompon;
    GComponent dialogUi;
    Controller dialogController;
    GLoader dialogBgLoader;
    GLoader headIconLoader;

    GamePointConfig gamePointConfig;
    SkeletonAnimation skeletonAnimation
    {
        get { return RoleMgr.roleMgr.GetSkeletonAnimation; }
    }
    CallBackList callBackList;

    int lastCardId = -1;
    GameObject roleModel;
    GoWrapper goWrapper;

    GGraph bgEffectGgraph;

    Transition transitionMy;
    Transition transitionOther;
    Transition transitionTran;

    GComponent finishCom;

    //auto
    int playType = 0;
    enum AutoModle
    {
        //普通
        Normal,
        //自动x1
        Auto_X1,
        //自动x2
        Auto_x2,
    }


    Dictionary<int, string> contextColorKeyPair = new Dictionary<int, string>()
    {
        //正常
        {1,"#503174"},
        //生气
        {2,"#a55067"},
        //高兴
        {3,"#503174"},
        //旁白
        {4,"#ffffff"},
    };
    /// <summary>
    /// 对话框背景图
    /// </summary>
    Dictionary<int, string> dialogBgKeyPair = new Dictionary<int, string>()
    {
        //正常
        {1,"ui://6f05wxg5gab6q3n"},
        //生气
        {2,"ui://6f05wxg5gab6q3o"},
        //高兴
        {3,"ui://6f05wxg5gab6q3l"},
        //旁白
        {4,"ui://6f05wxg5gab6q3m"},
    };
    /// <summary>
    /// 是否在抖动，必须要抖动结束以后才能进行下一个对话
    /// </summary>
    bool isEffect;
    GButton autoBtn;
    Controller autoController;

    bool canNext;


    enum PrintProgess
    {
        /// <summary>
        /// 空闲状态
        /// </summary>
        Idle,

        /// <summary>
        /// 准备中
        /// </summary>
        Readying,
        /// <summary>
        /// 进行中
        /// </summary>
        Printing,

        /// <summary>
        /// 完成文字显示
        /// </summary>
        Over,

    };


    PrintProgess printProgress = PrintProgess.Idle;

    public override void InitMoudle(GComponent gComponent, int controllerIndex)
    {
        base.InitMoudle(gComponent, controllerIndex);
        backgroundLoader = SearchChild("n5").asLoader;
        spineGraph = SearchChild("n7").asGraph;
        bgEffectGgraph = SearchChild("n18").asGraph;
        GComponent nobodyRoot = SearchChild("n14").asCom;
        nobodyTransition = nobodyRoot.GetTransition("t0");
        nobodyRoot = nobodyRoot.GetChild("n0").asCom;
        nobodyLoader = nobodyRoot.GetChild("n1").asLoader;
        nobodyLoader.alpha = 0;


        dialogGCompon = SearchChild("n10").asCom.GetChild("n10").asCom;
        dialogGCompon.alpha = 0;

        transitionMy = SearchChild("n10").asCom.GetTransition("t0");

        transitionOther = SearchChild("n10").asCom.GetTransition("t1");
        transitionTran = SearchChild("n10").asCom.GetTransition("t2");
        contentText = dialogGCompon.GetChild("n11").asTextField;
        typingEffect = new TypingEffect(contentText);
        dialogBgLoader = dialogGCompon.GetChild("n10").asLoader;
        dialogUi = dialogGCompon.GetChild("n1").asCom;
        dialogController = dialogUi.GetController("c1");
        npcNameText = dialogUi.GetChild("n2").asTextField;
        dialogTransition = ui.GetTransition("t0");
        headIconLoader = dialogUi.GetChild("n6").asLoader;
        finishCom = dialogGCompon.GetChild("n14").asCom;
        finishCom.visible = false;


        callBackList = new CallBackList();
        callBackList.callBack1 = ChangeBg;
        callBackList.callBack2 = ShowDialogDetailInfo;
        //auto 
        autoBtn = SearchChild("n2").asButton;
        autoController = autoBtn.GetController("c1");
        InitEvent();
        ins = this;

        printAllAction = PrintAllText;
    }


    bool isPrintAll;

    public override void InitEvent()
    {
        base.InitEvent();
        //dialogGCompon.onClick.Set(GoToNextNode);
        //SearchChild("n12").onClick.Set(GoToNextNode);
        dialogGCompon.onClick.Set(() =>
        {
            GoToNextNode();
        });
        SearchChild("n12").onClick.Set(() =>
        {
            GoToNextNode();
        });


        SearchChild("n4").onClick.Set(() =>
        {
            if (GameData.isGuider || StoryDataMgr.ins.timeItem != null)
                return;
            EventMgr.Ins.DispachEvent(EventConfig.STORY_BREACK_STORY);

        });

        //回顾剧情
        SearchChild("n3").onClick.Set(
        () =>
        {
            //Time.timeScale = 1;
            //playType = (int)AutoModle.Normal;
            //SetSppeedBtnUrl(autoUrl[playType]);
            //onComplete = null;
            //EventMgr.Ins.DispachEvent(EventConfig.STORY_PLAY_RECORD);   
            Time.timeScale = 1;
            SetingNormal();
            EventMgr.Ins.DispachEvent(EventConfig.STORY_PLAY_RECORD);
        });

        //自动播放
        autoBtn.onClick.Set(() =>
        {
            playType++;
            if (playType > (int)AutoModle.Auto_x2)
            {
                onComplete = null;
                SetingNormal();

            }
            else
            {
                speed = playType == (int)AutoModle.Auto_x2 ? 0.5f : 1f;
                onComplete = GoToNextNode;
                autoController.selectedIndex = playType;
                if (currentTypingEffect != null)
                    currentTypingEffect.ChangeSpeed(speed);
                if (currentTypingEffect == null)
                {
                    Debug.Log("自动播放结束触发下一个节点");
                    onComplete?.Invoke();
                }

            }
        });

        //dispose
        EventMgr.Ins.RegisterEvent(EventConfig.STORY_DIALOG_DISPOSE, Dispose);
        //listen sound over
        EventMgr.Ins.RegisterEvent(EventConfig.MUSIC_DIALOG_PLAY_OVER, ListenSoundPlayOver);
        onTypingComplete = () =>
        {
            finishCom.visible = true;
        };
    }


    public override void InitData<D>(D data)
    {


        gamePointConfig = data as GamePointConfig;
        if (gamePointConfig != null)
        {
            printProgress = PrintProgess.Readying;
            //如果该节点有奖励可以领取，设置该节点的播放为正常状态 
            finishCom.visible = false;
            if (GetNodeAwardResult())
                SetingNormal();
            isGoNextNode = false;
            ShowDialog();
            //处理该节点是否有奖励
            DoAwardDialog();
            SearchChild("n4").visible = true;
            SearchChild("n2").visible = true;
            SearchChild("n3").visible = true;

            if (GameData.isGuider)
            {
                GameData.guiderCurrent = GuiderInfoLinked.GetCurrGuiderInfo(1, 2);
                UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);
                SearchChild("n4").visible = false;
                SearchChild("n2").visible = false;
                SearchChild("n3").visible = false;
            }

        }
    }


    void PrintAllText()
    {
        isPrintAll = true;
        printProgress = PrintProgess.Over;
    }
             
    private void SetingNormal()
    {
        onComplete = null;
        speed = 1.0f;
        playType = (int)AutoModle.Normal;
        autoController.selectedIndex = playType;
        if (currentTypingEffect != null)
            currentTypingEffect.ChangeSpeed(speed);

    }


    private void ShowDialog()
    {

        Debug.Log("ShowDialog " + printProgress);
        if (StoryDataMgr.ins.StoryInfo.isReRead)
        {
            StoryDataMgr.ins.StoryInfo.RemoveNodes();
        }
                     
        Action delayCall = () =>
        {
            CompareBackGroundId(gamePointConfig.background_id);
            if (gamePointConfig.type > 4)
            {
                backgroundLoader.TweenScale(Vector2.one * 1.5f, 1f);
            }
            else
            {
                backgroundLoader.scale = Vector2.one;
            }
        };

        //对话框aplha >0  如果之前的是普攻的对话狂 下一个是惊讶的对话框 就需要关闭之前的
        //if (dialogGCompon.alpha > 0 && frame_background_id != 2 && gamePointConfig.frame_background_id == 2)
        if (dialogGCompon.alpha > 0 && (frame_background_id != gamePointConfig.frame_background_id ||
            lastStoryTypeId != gamePointConfig.type))
        {

            dialogGCompon.TweenFade(0, 0.5f).OnComplete(() =>
            {
                delayCall();
            });
        }
        else
        {

            delayCall();
        }
    }

    void CompareBackGroundId(int bgId)
    {
        isGoNextNode = false;
        if (StoryDataMgr.ins.lastBgId != bgId && StoryDataMgr.ins.lastBgId > 0)
        {
            StoryDataMgr.ins.lastBgId = bgId;
            //dialogGCompon.visible = false;
            //nobodyLoader.visible = false;  
            Action callback = () =>
            {
                if (spineGraph != null && spineGraph.displayObject != null && spineGraph.displayObject.gameObject != null)
                {

                    if (roleModel != null && roleIsVisible)
                        roleModel.GetComponent<RoleMappingCom>().FadeOut(ShowAnimationView);

                    if (!roleIsVisible)
                        ShowAnimationView();
                    roleIsVisible = false;
                    //spineGraph.visible = false;
                }
                else
                {
                    ShowAnimationView();
                }
            };

            //first hide dialog
            if (nobodyLoader.visible && nobodyLoader.alpha > 0)
            {
                nobodyLoader.TweenFade(0, 0.5f).OnComplete(() =>
                {
                    callback();
                });
            }
            else
            {
                callback();
            }

        }
        else
        {
            StoryDataMgr.ins.lastBgId = bgId;
            backgroundLoader.alpha = 1;
            backgroundLoader.visible = true;
            backgroundLoader.url = UrlUtil.GetStoryBgUrl(StoryDataMgr.ins.lastBgId);
            ShowDialogDetailInfo();
        }

    }

    /// <summary>
    /// 过场动画
    /// </summary>
    void ShowAnimationView()
    {
        ResetDialogCache(() =>
      {
          EventMgr.Ins.DispachEvent(EventConfig.STORY_ANIMATION_VIEW, callBackList);
      });
    }


    void ResetDialogCache(Action callback)
    {
        lastStoryTypeId = -1;
        dialogUrl = "";
        if (dialogGCompon.alpha > 0)
        {
            dialogGCompon.TweenFade(0, 0.7f).OnComplete(() =>
            {
                callback?.Invoke();
            });
        }
        else
        {
            callback?.Invoke();
        }
    }


    public void ResetDialogPerson(Action callback)
    {
        lastStoryTypeId = -1;
        dialogUrl = "";




        Action subCallback = () =>
        {
            if (roleModel != null && roleIsVisible)
            {
                roleModel.GetComponent<RoleMappingCom>().FadeOut(() =>
                {
                    FadeOutLoaderPerson(callback);
                });
            }
            else
            {
                FadeOutLoaderPerson(callback);
            }
        };

        ResetDialogCache(subCallback);
    }


    void FadeOutLoaderPerson(Action callback)
    {
        nobodyLoader.TweenFade(0, 0.2f);
        if (backgroundLoader.alpha > 0)
        {

            backgroundLoader.TweenFade(0, 0.2f).OnComplete(() =>
            {
                callback();
            });
        }
        else
        {
            callback();
        }
    }


    void ChangeBg()
    {
        backgroundLoader.alpha = 0;
        backgroundLoader.TweenFade(1, 0.5f);
        backgroundLoader.url = UrlUtil.GetStoryBgUrl(StoryDataMgr.ins.lastBgId);
    }

    GoWrapper goWra;
    void ShowDialogDetailInfo()
    {
        contentText.text = "";
        //1、角色
        InitRoleOrBody();
    }



    void FadeDaialog()
    {
        skeletonAnimation.timeScale = 1;
        dialogGCompon.visible = true;
        //2、对话框 
        ShowDetailDialog();

        if (goWra != null)
        {
            GameObject go = goWra.wrapTarget;
            goWra.wrapTarget = null;
            GameObject.Destroy(go);
            goWra = null;
        }
        if (gamePointConfig.effect_id != 0)
        {
            if (gamePointConfig.effect_id == 5)
            {
                ScreenShake();
            }
            else
            {
                goWra = FXMgr.CreateEffectWithGGraph(bgEffectGgraph, Vector3.zero, "bg" + gamePointConfig.effect_id + "_lizi", 162);
            }
        }
    }

    void InitRoleOrBody()
    {

        if (gamePointConfig.card_id < 100)
        {
            //nobodyLoader.visible = false;
            if (nobodyLoader.alpha > 0)
            {
                nobodyLoader.TweenFade(0, 0.5f).OnComplete(InitRole);
            }
            else
            {

                InitRole();
            }
        }
        else
        {
            if (spineGraph != null && spineGraph.displayObject != null && spineGraph.displayObject.gameObject != null)
            {
                //todo:修改节奏
                if (roleModel != null && roleIsVisible)
                    roleModel.GetComponent<RoleMappingCom>().FadeOut();

                roleIsVisible = false;                    
                //spineGraph.visible = false;  
            }
            InitBodyImg();
        }

    }



    bool isGoNextNode;
    int second = 0;
    int beforeGatePointId = -1;
    /// <summary>
    /// 接入下一个节点
    /// </summary>
    public void GoToNextNode()
    {
        //Debug.Log("<color=yellow>触发...GoToNextNode</color> " + second +
        //    "  offsetSecond " + offsetSecond + " printProgress=" + printProgress);
        /*
       1、只有两次间隔时间大于1s
       2、当前状态是输出文字状态或者输出完成状态
       3、如果是输出状态就触发一次性全部输出
       4、如果已经全部输出完毕 那么离开把状态重置为空闲状态
       */
        int offsetSecond = Math.Abs(DateTime.Now.Second - second);
        if (offsetSecond > 0 && (printProgress == PrintProgess.Printing ||
                                    printProgress == PrintProgess.Over))
        {
            Debug.Log("<color=yellow>GoToNextNode</color> " + gamePointConfig.id + "  " + printProgress);
            second = DateTime.Now.Second;
            if (StoryDataMgr.ins.timeItem != null)
            {
                if (SpeedPrint() && printProgress == PrintProgess.Over)
                {
                    TinyItem item = new TinyItem(StoryDataMgr.ins.timeItem);
                    StoryDataMgr.ins.timeItem = null;
                    UIMgr.Ins.showNextPopupView<GetTimeView, TinyItem>(item);
                }
                return;
            }
            //抖动的时候继续
            if (isEffect)
                return;

            if (SpeedPrint() && printProgress == PrintProgess.Over)
            {
                printProgress = PrintProgess.Idle;
                finishCom.visible = true;
                beforeGatePointId = gamePointConfig.id;
                WaitGotoNext();
            }
        }
    }


    void WaitGotoNext()
    {

        isGoNextNode = true;
        GRoot.inst.StopDialogSound();
        ListenSoundPlayOver();
        if (!StoryDataMgr.ins.StoryInfo.isReRead)
        {
            NormalInfo normalInfo = new NormalInfo();
            normalInfo.index = gamePointConfig.point1;
            //Debug.LogError("<color=white> isRead=false</color> next node: " + gamePointConfig.content1 + "   " + gamePointConfig.point1);
            EventMgr.Ins.DispachEvent(EventConfig.STORY_TRIGGER_NEXT_NODE, normalInfo);
        }
        else
        {
            if (StoryDataMgr.ins.StoryInfo.nodes.Count > 0)
            {

                gamePointConfig = DataUtil.GetPointConfig(StoryDataMgr.ins.StoryInfo.nodes[0]);
                if (gamePointConfig != null)
                {
                    if (gamePointConfig.type != (int)TypeConfig.StoyType.TYPE_TRANSITION && gamePointConfig.type != (int)TypeConfig.StoyType.TYPE_TRANSITION_BG_ENLARGE)
                    {

                        ShowDialog();
                        //音效
                        EventMgr.Ins.DispachEvent(EventConfig.MUSIC_STORY_EFFECT, gamePointConfig);
                    }
                    else
                    {
                        NormalInfo normalInfo = new NormalInfo();
                        normalInfo.index = gamePointConfig.id;
                        EventMgr.Ins.DispachEvent(EventConfig.STORY_TRIGGER_NEXT_NODE, normalInfo);
                    }
                }
            }

        }

    }

    public override void InitTypeEffect()
    {
        base.InitTypeEffect();
        isPrintAll = false;
        printProgress = PrintProgess.Printing;
        second = 0;
        contentText.text = "[color=" + contextColorKeyPair[gamePointConfig.frame_background_id] + "]" + DataUtil.ReplaceCharacterWithStarts(gamePointConfig.content1) + "[/color]";

        typingEffect.Start();
        typingEffects.Add(typingEffect);
        PrintTex();

        if (playType == (int)AutoModle.Normal)
            PlayDialogMusic();




        //Debug.Log("<color=white>设置为true</color>");



    }



    int lastStoryTypeId = -1;
    int frame_background_id = -1;

    string dialogUrl = "";
    float fadeDuration = 0.5f;



    public void DoDialog(int showIndex, string url, int posX)
    {
        if (gamePointConfig.frame_background_id != 2)
        {
            /* 
             * 1、从其他类型过渡过来 在这之前alpha一定是为0
               2、从其它类型过来和自己同类型不同对话框其实是同一个东西
             */
            if (lastStoryTypeId != gamePointConfig.type ||
                gamePointConfig.frame_background_id != frame_background_id)
            {
                //调用分页
                SwitchSelectPage(showIndex);
                //设置背景
                SetDialogBg();
                //设置头像
                headIconLoader.url = url;
                dialogUrl = url;
                //设置位置
                dialogGCompon.x = posX;
                //from right middle
                PlayDialogMoveAnimation(0);
            }
            else
            {
                FadeIn();
            }
        }
        else
        {
            /*
                1、完全不同的两种
             */
            //设置背景
            SetDialogBg();
            ShowShakeDialog(showIndex, url);
        }
    }


    void SwitchSelectPage(int showIndex)
    {
        if (dialogController.selectedIndex != showIndex)
            dialogController.selectedIndex = showIndex;
    }


    void SetDialogBg()
    {
        if (dialogBgKeyPair.ContainsKey(gamePointConfig.frame_background_id))
        {
            dialogBgLoader.url = dialogBgKeyPair[gamePointConfig.frame_background_id];
        }
    }

    void PlayDialogMoveAnimation(float target)
    {
        dialogGCompon.TweenMoveX(target, fadeDuration).SetEase(EaseType.Linear);
        dialogGCompon.TweenFade(1, fadeDuration).OnComplete(() =>
        {
            InitTypeEffect();
            isEffect = false;
        });
    }

    void FadeIn()
    {
        Debug.Log("dialogGCompon " + dialogGCompon.visible + "  " + dialogGCompon.alpha);
        if (dialogGCompon.alpha == 0)
        {
            dialogGCompon.TweenFade(1, fadeDuration).SetEase(EaseType.Linear).OnComplete(() =>
            {
                InitTypeEffect();
                isEffect = false;
            });
        }
        else
        {
            InitTypeEffect();
            isEffect = false;
        }

    }


    void ShowDetailDialog()
    {
        dialogGCompon.visible = true;
        int showIndex = 0;
        isEffect = true;
        string title;
        string url = "";
        contentText.text = "";
        //Debug.LogError("type: >>>>>>>>>>>>>>>>>>>>>" + gamePointConfig.type);
        //Debug.Log("gamePointConfig.frame_background_id =" + gamePointConfig.frame_background_id);
        //frame_background_id = gamePointConfig.frame_background_id;
        switch (gamePointConfig.type)
        {
            case (int)TypeConfig.StoyType.TYPE_ROLE:
            case (int)TypeConfig.StoyType.TYPE_ROLE_BG_ENLARGE:
                showIndex = 1;
                title = gamePointConfig.title;
                int headId = gamePointConfig.card_id > 100 ? gamePointConfig.face_id : gamePointConfig.card_id;
                url = UrlUtil.GetDialogHeadIconUrl(headId);
                //设置头像
                headIconLoader.url = url;
                /**
                  逻辑梳理
                1、据有震惊和非震惊两种情况


                2、震惊情况
                    完全显示的不是同一中表现方式    

                3、非震惊情况

                    3.1 是从其他类别切换到当前类别
                        存在分页
                        改变背景
                        改变头像
                        改变文字
                        改变alpha
                        位置从右到左出现    

                    3.2 使用的是相同的类别
                        背景是否相同
                        位置是否在中间
                        alpha是否为0   
                 */

                DoDialog(showIndex, url, Screen.width);
                Debug.Log("gamePointConfig.frame_background_id =" + gamePointConfig.frame_background_id);
                break;
            case (int)TypeConfig.StoyType.TYPE_SELF:
            case (int)TypeConfig.StoyType.TYPE_SELF_BG_ENLARGE:
                title = GameData.Player.name;
                showIndex = 0;
                url = UrlUtil.GetStorySelfHeadIconUrl(0);


                //1、如果不是同一张就需要先淡出
                //2、设置位置
                //淡入           

                DoDialog(showIndex, url, -Screen.width);

                break;

            case (int)TypeConfig.StoyType.TYPE_ASIDE:
            case (int)TypeConfig.StoyType.TYPE_ASIDE_BG_ENLARGE:
                showIndex = 2;
                title = gamePointConfig.title;
                //    transitionTran.Play(1, 0, 0, 0.4f, () =>
                //{
                //    InitTypeEffect();
                //    isEffect = false;
                //});

                /**
                    旁白：
                    中心出来 
                 */
                if (gamePointConfig.frame_background_id != 2)
                {
                    /* 
                     * 1、从其他类型过渡过来 在这之前alpha一定是为0
                       2、从其它类型过来和自己同类型不同对话框其实是同一个东西
                     */
                    if (lastStoryTypeId != gamePointConfig.type ||
                        gamePointConfig.frame_background_id != frame_background_id)
                    {
                        //调用分页
                        SwitchSelectPage(showIndex);
                        //设置背景
                        SetDialogBg();
                        //设置头像
                        headIconLoader.url = url;
                        dialogUrl = url;
                        //设置位置
                        dialogGCompon.x = 0;
                        //渐变出来
                        FadeIn();
                    }
                    else
                    {
                        //这里使用的是和上一次一模一样的对话框   不存在alpha为0的情况
                        if (gamePointConfig.frame_background_id == frame_background_id)
                        {
                        }
                        else
                        {
                            Debug.Log("异常了 怎么会走这里呢.....>????");
                        }
                        //渐变出来
                        FadeIn();
                    }
                }
                else
                {
                    /*
                        1、完全不同的两种
                     */
                    //设置背景
                    SetDialogBg();
                    ShowShakeDialog(showIndex, url);
                }


                break;
            default:
                title = gamePointConfig.title;
                Debug.LogError(".........................<color=yellow>.............离这里了</color>");
                break;
        }


        lastStoryTypeId = gamePointConfig.type;
        frame_background_id = gamePointConfig.frame_background_id;
        npcNameText.text = title;

    }



    void ShowShakeDialog(int showIndex, string url)
    {
        //if (gamePointConfig.frame_background_id == 2)
        {
            dialogUrl = url;
            if (dialogController.selectedIndex != showIndex)
                dialogController.selectedIndex = showIndex;

            headIconLoader.url = url;
            dialogGCompon.TweenFade(1, 0.5f).OnComplete(() =>
            {
                DoShake();
            });
        }
    }


    int lastFaceId = -1;
    bool roleIsVisible;
    void InitRole()
    {
        Debug.Log("rolevisbile : " + roleIsVisible);
        if (gamePointConfig.card_id == 0)
        {
            if (spineGraph != null && spineGraph.displayObject != null && spineGraph.displayObject.gameObject != null)
            {
                if (roleModel != null && roleIsVisible)
                {
                    Debug.Log("alpha: " + roleModel.GetComponent<RoleMappingCom>().IsAlpha);
                    roleModel.GetComponent<RoleMappingCom>().FadeOut(FadeDaialog);
                }
                //spineGraph.visible = false;
                if (!roleIsVisible)
                    FadeDaialog();
                roleIsVisible = false;
            }
            else
            {
                 FadeDaialog(); 
            }
         

            return;
        }

        if (lastCardId != gamePointConfig.card_id)
        {
            lastCardId = gamePointConfig.card_id;
            if (roleModel == null)
            {
                UnityEngine.Object prefab = Resources.Load(UrlUtil.GetSpineUrl(gamePointConfig.card_id.ToString()));
                if (prefab != null)
                {
                    RoleMgr.roleMgr.CreateRole(prefab);
                    SpineCtr.SetIdleAnimation(skeletonAnimation);
                    roleModel = RoleMgr.roleMgr.GetMapingCom();
                    roleModel.transform.localPosition = new Vector3(17, -475, 1000);
                    roleModel.transform.localScale = new Vector3(857.0313f, 1855.758f, 114.2708f);
                }

            }
            if (roleModel != null)
            {
                goWrapper = new GoWrapper(roleModel);
                spineGraph.SetNativeObject(goWrapper);
            }

        }

        //Debug.Log(lastFaceId + " <color=yellow>说话啦</color>.............." + gamePointConfig.face_id);
        Action callback = () =>
        {
            FadeDaialog();
            //if (lastFaceId != gamePointConfig.face_id || gamePointConfig.face_id == SpineConfig.AC_TYPE_TALK)
            {
                //Debug.Log("<color=yellow> 当前需要过度的动作</color> :" + gamePointConfig.face_id);
                lastFaceId = gamePointConfig.face_id;
                SpineCtr.SwitchAnimationState(skeletonAnimation, new int[] { gamePointConfig.face_id });
                skeletonAnimation.timeScale = 1;
            }

        };

        RoleMappingCom roleMappingCom = roleModel.GetComponent<RoleMappingCom>();

        if (!roleIsVisible || roleModel.GetComponent<RoleMappingCom>().IsAlpha < 1.0f)
        {
            //Debug.LogError("来了。。fade in。");
            roleIsVisible = true;
            spineGraph.visible = true;
            //roleModel.GetComponent<RoleMappingCom>().FadeIn(FadeDaialog);
            SpineCtr.SwitchAnimationState(skeletonAnimation, new int[] { gamePointConfig.face_id });
            skeletonAnimation.timeScale = 0.1f;
            roleModel.GetComponent<RoleMappingCom>().FadeIn(callback);
        }
        else
        {
            Debug.LogError("来了 default ...fade in。");
            //FadeDaialog();
            callback();

        }
    }

    int bodyImgId = -1;
    void InitBodyImg()
    {
        //todo:条节奏
        //nobodyLoader.visible = true;
        //nobodyLoader.url = UrlUtil.GetStoryOtherUrl(gamePointConfig.card_id);    
        //nobodyTransition.Play();

        nobodyLoader.visible = true;
        bodyImgId = gamePointConfig.card_id;
        nobodyLoader.url = UrlUtil.GetStoryOtherUrl(gamePointConfig.card_id);
        if (nobodyLoader.alpha == 0)
        {
            nobodyLoader.TweenFade(1, 0.8f).OnComplete(ShowDetailDialog);
            nobodyTransition.Play();
        }
        else
        {
            ShowDetailDialog();
        }


    }


    #region 奖励关卡记录，该关卡不属于选择关卡

    void DoAwardDialog()
    {
        if (GetNodeAwardResult())
        {
            SpeedPrint();
            RequestRecordAwardNode();
        }

    }

    /// <summary>
    /// 获取对话节点奖励状态
    /// </summary>
    /// <returns><c>true</c>, if node award result was gotten, <c>false</c> otherwise.</returns>
    private bool GetNodeAwardResult()
    {
        int currentNodeId = StoryDataMgr.ins.StoryInfo.node_id;
        GameNodeConfig gameNodeConfig = DataUtil.GetNodeConfig(StoryDataMgr.ins.StoryInfo.chapterId, currentNodeId);
        //没有领取过该奖励
        bool isContains = StoryDataMgr.ins.playerChapterInfo.GetAwardPoints.Contains(currentNodeId);
        //判断普通关卡是否有奖励
        if (gameNodeConfig != null &&
           !string.IsNullOrEmpty(gameNodeConfig.awards) &&
           gameNodeConfig.awards != "0" &&
           !isContains)
        {
            return true;
        }
        return false;

    }

    /// <summary>
    /// 记录剧情对话奖励点
    /// </summary>
    void RequestRecordAwardNode()
    {
        System.Action callback = null;
        WWWForm wWForm = new WWWForm();
        wWForm.AddField("actorId", StoryDataMgr.ins.StoryInfo.actor_id);
        wWForm.AddField("chapterId", StoryDataMgr.ins.StoryInfo.chapterId);
        wWForm.AddField("nodeId", StoryDataMgr.ins.StoryInfo.node_id);
        GameMonoBehaviour.Ins.RequestInfoPost<PlayerStoryInfo>(NetHeaderConfig.STORY_RECORD_NODE, wWForm, callback);

    }



    public override void Dispose()
    {
        SetingNormal();
        lastCardId = -1;
        StoryDataMgr.ins.lastBgId = -1;
        //roleModel.
        if (roleModel != null)
        {
            UnityEngine.Object.Destroy(roleModel);
            roleModel = null;
            goWrapper.Dispose();
        }
        //skeletonAnimation = null;
    }


    #endregion


    int soundId;
    bool isPlay;
    void PlayDialogMusic()
    {
        //Debug.LogError("PlayDialogMusic   " + gamePointConfig.id + "   " + gamePointConfig.sound_id);
        if (gamePointConfig.sound_id != 0 && isPlay == false && !isGoNextNode)
        {
            isPlay = true;
            AudioClip audioClip = ResourceUtil.LoadStoryDialogSound(gamePointConfig.sound_id);
            GRoot.inst.PlayDialogSound(audioClip);
        }
    }

    /// <summary>
    /// 在语音播放结束以后 就会把spine切换到idle状态
    /// </summary>
    void ListenSoundPlayOver()
    {
        if (isPlay && gamePointConfig.sound_id != 0)
        {
            if (!GRoot.inst.GetDialogIsPlaying())
            {
                isPlay = false;
                if (gamePointConfig.face_id == 7)
                    SpineCtr.ResetAniamtionToIdle(skeletonAnimation);
                else if (SpineConfig.talkSpine.Contains(gamePointConfig.face_id))
                {
                    if (gamePointConfig.face_id == 11)
                    {
                        SpineCtr.ResetAniamtionToIdle(skeletonAnimation);
                    }
                    else
                    {
                        SpineCtr.SwitchAnimationState(skeletonAnimation, new int[] { gamePointConfig.face_id - 1 });
                    }
                }
            }
        }

    }

    void DoShake()
    {
        isEffect = true;
        GTween.Shake(dialogGCompon.displayObject.gameObject.transform.localPosition, 3.5f, 0.3f).SetTarget(dialogGCompon.displayObject.gameObject).OnUpdate(
        (GTweener tweener) =>
        {
            dialogGCompon.displayObject.gameObject.transform.localPosition = new Vector3(tweener.value.x, tweener.value.y, dialogGCompon.displayObject.gameObject.transform.localPosition.z);
        }).OnComplete(() =>
        {
            isEffect = false;
            //if (currentTypingEffect == null)
            //    onComplete?.Invoke();

            InitTypeEffect();

        });
    }

    void ScreenShake()
    {
        GTween.Shake(ui.displayObject.gameObject.transform.localPosition, 3.5f, 1f).SetTarget(ui.displayObject.gameObject).OnUpdate(
        (GTweener tweener) =>
        {
            ui.displayObject.gameObject.transform.localPosition = new Vector3(tweener.value.x, tweener.value.y, ui.displayObject.gameObject.transform.localPosition.z);
        });
    }



}

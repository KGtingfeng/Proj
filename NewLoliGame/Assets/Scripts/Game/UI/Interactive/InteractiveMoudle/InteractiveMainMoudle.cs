using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;
using Spine.Unity;

public class InteractiveMainMoudle : BaseMoudle
{
    public static InteractiveMainMoudle ins;

    GLoader bgLoader;
    GTextField nameText;
    GTextField favorLevelText;
    GTextField favorNameText;
    GProgressBar progressBar;
    GImage levelImage;
    GComponent levelProgressBar;
    GGraph spineGraph;
    GoWrapper goWrapper;
    SkeletonAnimation skeletonAnimation;
    GameRoleMainTipsConfig gameRoleMainTips;

    Controller backgroundController;
    Controller wishController;
    Controller phoneController;

    PlayerActor currentRole
    {
        get { return InteractiveDataMgr.ins.CurrentPlayerActor; }
    }

    public override void InitMoudle(GComponent gComponent, int controllerIndex)
    {
        base.InitMoudle(gComponent, controllerIndex);
        InitUI();
        InitEvent();
        ins = this;
    }

    public override void InitUI()
    {
        base.InitUI();
        bgLoader = SearchChild("n43").asLoader;
        nameText = SearchChild("n17").asTextField;
        favorLevelText = SearchChild("n20").asCom.GetChild("n21").asTextField;
        favorNameText = SearchChild("n18").asTextField;
        progressBar = SearchChild("n19").asProgress;
        spineGraph = SearchChild("n91").asCom.GetChild("n91").asGraph;

        levelProgressBar = SearchChild("n20").asCom;
        levelImage = levelProgressBar.GetChild("n23").asCom.GetChild("n23").asImage;
        backgroundController = SearchChild("n23").asCom.GetController("c1");
        wishController = SearchChild("n24").asCom.GetController("c1");
        phoneController = SearchChild("n27").asCom.GetController("statue");

        currentDollId = -1;
        currentBg = -1;
    }

    public override void InitEvent()
    {
        base.InitEvent();
        //EventMgr.Ins.RegisterEvent<int>(EventConfig.CHANGE_SPINE, InitDollInfo);
        EventMgr.Ins.RegisterEvent<GameActorSkinConfig>(EventConfig.CHANGE_SPINE, InitDollInfo);
        EventMgr.Ins.RegisterEvent<int>(EventConfig.CHANGE_BACKGROUND, InitBg);
        EventMgr.Ins.RegisterEvent(EventConfig.FAVOR_CHANGE, RefreshFavor);
        EventMgr.Ins.RegisterEvent(EventConfig.CLOSE_INTERACTIVE_TALK, CloseTalkWindow);
        InitSpineEvent();
    }

    private void InitSpineEvent()
    {
        //head
        SearchChild("n92").onClick.Set(() =>
        {
            PlayRoleSounds(RoleBodyType.Head);
        });
        //body
        SearchChild("n93").onClick.Set(() =>
        {
            PlayRoleSounds(RoleBodyType.Body);
        });
    }


    public override void InitData()
    {
        base.InitData();

        EventMgr.Ins.DispachEvent(EventConfig.INTERACTIVE_MAIN_EFFECT);
        RefreshInitInfos();
        backgroundController.selectedIndex = RedpointMgr.Ins.BackgroundHaveRedpoint() ? 1 : 0;

        //WWWForm wWWForm = new WWWForm();
        //GameMonoBehaviour.Ins.RequestInfoPost<PlayerRedpoint>(NetHeaderConfig.RED_POINT, wWWForm, () =>
        //{
        //    wishController.selectedIndex = RedpointMgr.Ins.wishRedpoint.Contains(currentRole.actor_id) ? 1 : 0;
        //    phoneController.selectedIndex = RedpointMgr.Ins.PhoneHaveRedpoint() ? 1 : 0;
        //}, false);

        if (GameData.isGuider)
        {
            UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);
        }

    }

    void RefreshInitInfos()
    {
        nameText.text = JsonConfig.GameInitCardsConfigs.Find(a => a.card_id == currentRole.actor_id).name_cn;
        RefreshFavor();
        if (InteractiveDataMgr.ins.ownSkins.Count > 0)
        {
            GameActorSkinConfig skinConfig = DataUtil.GetGameActorSkinConfig(InteractiveDataMgr.ins.ownSkins[0]);
            InitDollInfo(skinConfig);
            InitBg(InteractiveDataMgr.ins.ownBackgrounds[0]);
        }
        else
        {
            GameActorSkinConfig skinConfig = JsonConfig.GameActorSkinConfigs.Find(a => a.actor_id == currentRole.actor_id && a.type == GameActorSkinConfig.DEFAULT_SKINS_TYPE);
            InitDollInfo(skinConfig);
            GameActorSkinConfig bg = JsonConfig.GameActorSkinConfigs.Find(a => a.type == GameActorSkinConfig.FAIRY_BACKGROUND_TYPE);
            InitBg(bg.id);
        }
    }

    void RefreshFavor()
    {
        int level = GameTool.FavorLevel(currentRole.Favour);
        GameFavourTitleConfig titleConfig = JsonConfig.GameFavourTitleConfigs.Find(a => a.level_id == level);
        progressBar.max = titleConfig.level + 1;
        progressBar.value = currentRole.Favour;
        favorLevelText.text = level + "";
        favorNameText.text = titleConfig.name_cn;
        GameTool.SetLevelProgressBar(levelImage, level);
    }

    public int currentBg;
    void InitBg(int id)
    {
        if (InteractiveDataMgr.ins.ownBackgrounds.Count > 0)
        {
            currentBg = id;
            bgLoader.url = UrlUtil.GetSkinBgUrl(id);
        }
    }

    #region spine
    int currentDollId;
    int currentSkinUrl;
    void InitDollInfo(GameActorSkinConfig skinConfig)
    {
        Debug.Log("currentDollId: " + currentDollId + "    actor_id: " + skinConfig.actor_id + "  currentSkinUrl:  " + currentSkinUrl + "     skinId:  " + skinConfig.id);
        if (currentDollId == skinConfig.actor_id && currentSkinUrl == skinConfig.id)
            return;

        bool isSkin = skinConfig.type != 1;
        string path = isSkin ? (skinConfig.actor_id + "_" + skinConfig.id) : skinConfig.actor_id.ToString();

        UnityEngine.Object prefab = ResourceUtil.LoadSpineByName(path);
        if (prefab == null)
            prefab = ResourceUtil.LoadSpineByName(skinConfig.actor_id.ToString());
        GameObject go = (GameObject)UnityEngine.Object.Instantiate(prefab);

        go.transform.localPosition = new Vector3(30, -1010, 1000);
        go.transform.localScale = Vector3.one * 108f;
        Component[] components = go.GetComponentsInChildren(typeof(SkeletonAnimation));
        if (components != null && components.Length > 0)
        {
            skeletonAnimation = components[0] as SkeletonAnimation;
        }

        SpineCtr.SetIdleAnimation(skeletonAnimation);
        if (currentDollId == -1)
            goWrapper = new GoWrapper(go);
        else
        {
            GameObject gobj = goWrapper.wrapTarget;
            goWrapper.wrapTarget = go;
            Object.Destroy(gobj);
        }
        spineGraph.SetNativeObject(goWrapper);
        gameRoleMainTips = DataUtil.GetRoleMainTipsConfig(skinConfig.actor_id);
        currentDollId = skinConfig.actor_id;
        currentSkinUrl = skinConfig.id;
    }

    void PlayRoleSounds(RoleBodyType roleBodyType)
    {
        if (gameRoleMainTips == null)
            return;
        if (!GRoot.inst.GetDialogIsPlaying())
        {
            BodyInfo bodyInfo = null;
            switch (roleBodyType)
            {
                case RoleBodyType.Head:
                    bodyInfo = gameRoleMainTips.GetHeadInfo();
                    break;
                case RoleBodyType.Body:
                    bodyInfo = gameRoleMainTips.GetBodyInfo();
                    break;
            }
            PlaySound(bodyInfo);
            isPlayRoleDialog = true;
        }
    }

    private void PlaySound(BodyInfo bodyInfo)
    {
        SpineCtr.SwitchAnimationState(skeletonAnimation, new int[] { bodyInfo.faceId });

        ShowSpineTalk(bodyInfo.context);
        AudioClip audioClip = ResourceUtil.LoadMainRoleTipsMusic(bodyInfo.id + "/" + bodyInfo.voiceId);
        GRoot.inst.PlayDialogSound(audioClip);
    }

    private void ShowSpineTalk(string msg)
    {
        ErrorMsg errorMsg = new ErrorMsg(msg);
        UIMgr.Ins.showWindow<ShowRoleTalkWindow>(errorMsg);
    }

    public void CloseTalkWindow()
    {
        if (BaseWindow.window is ShowRoleTalkWindow && BaseWindow.window.contentPane.visible)
        {
            GRoot.inst.HideWindowImmediately(BaseWindow.window);
            if (GRoot.inst.GetDialogIsPlaying())
            {
                GRoot.inst.StopDialogSound();
            }
            SpineCtr.ResetAniamtionToIdle(skeletonAnimation);
        }
    }

    bool isPlayRoleDialog;
    public void ListenSoundPlayOver()
    {
        if (isPlayRoleDialog)
        {
            if (!GRoot.inst.GetDialogIsPlaying())
            {

                isPlayRoleDialog = false;
                //播放结束
                CloseTalkWindow();

            }
        }
    }


    #endregion


}

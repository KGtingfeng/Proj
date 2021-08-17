using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using System;
using UnityEngine;

public class ChangeAppearanceMoudle : BaseMoudle
{
    GButton saveBtn;
    GButton shareBtn;
    GGroup listGroup;
    GList selectList;
    GList appearanceList;
    GGraph spineGraph;
    GComponent spineGcom;
    List<string> textrueName = new List<string>()
    {
        "hlighta",
        "hlightb",
    };
    List<Texture> textures = new List<Texture>();

    PlayerActor playerActor
    {
        get
        {
            return InteractiveDataMgr.ins.CurrentPlayerActor;
        }
    }
    List<GameActorSkinConfig> currentList = new List<GameActorSkinConfig>();
    List<GameActorSkinConfig> currentSkinList = new List<GameActorSkinConfig>();
    List<GameActorSkinConfig> currentBackGroundList = new List<GameActorSkinConfig>();

    List<int> ownSkins
    {
        get
        {
            return InteractiveDataMgr.ins.ownSkins;
        }
    }
    List<int> ownBackgrounds
    {
        get
        {
            return InteractiveDataMgr.ins.ownBackgrounds;
        }
    }

    //当前选中的是皮肤还是背景
    int selectIndex;
    //存储选中的皮肤和背景id,itemIndex[0]是皮肤
    List<int> lastId = new List<int> { 0, 0 };
    //按钮位置
    float saveBtnPosX = 0;

    public override void InitMoudle(GComponent gComponent, int controllerIndex)
    {
        base.InitMoudle(gComponent, controllerIndex);
        InitUI();
        InitEvent();
    }
    NormalInfo normalInfo;
    public override void InitUI()
    {
        base.InitUI();
        saveBtn = SearchChild("n85").asButton;
        shareBtn = SearchChild("n84").asButton;
        listGroup = SearchChild("n88").asGroup;
        selectList = SearchChild("n81").asList;
        appearanceList = SearchChild("n87").asList;

        spineGraph = SearchChild("n91").asCom.GetChild("n91").asGraph;
        spineGcom = SearchChild("n91").asCom;
        normalInfo = new NormalInfo();
        normalInfo.index = (int)SoundConfig.CommonEffectId.AttribuevUpgrade;

    }

    public override void InitEvent()
    {
        base.InitEvent();
        saveBtn.onClick.Set(OnClickSave);
        SearchChild("n84").onClick.Set(ShowBuySuceessEffect);
        EventMgr.Ins.RegisterEvent(EventConfig.BLANK_CLICK, CloseListEffect);
        EventMgr.Ins.RegisterEvent(EventConfig.APPEARANCE_REBACK, CheckSave);
        EventMgr.Ins.RegisterEvent(EventConfig.REFRESH_APPEARANCE_LIST, GetList);
        EventMgr.Ins.RegisterEvent(EventConfig.SHOW_BUY_SKIN_EFFECT, ShowBuySuceessEffect);
    }

    public override void InitData()
    {
        base.InitData();
        selectIndex = 0;
        selectList.selectedIndex = 0;
        selectList.onClickItem.Set(OnClickSelect);
        ResetIndex();
        GetList();
        OpenListEffect();

        saveBtnPosX = saveBtn.position.x;
        //if (GameData.isOpenGuider)
        //{
        //    StoryCacheMgr.storyCacheMgr.GetStoryGameSave("Newbie", GameGuideConfig.TYPE_APPEARANCE, (storyGameSave) =>
        //    {
        //        if (storyGameSave.IsDefault)
        //        {
        //            GameData.guiderCurrent = GuiderInfoLinked.GetCurrGuiderInfo(GameGuideConfig.TYPE_APPEARANCE, 1);
        //            UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);
        //            StoryCacheMgr.storyCacheMgr.SaveProgress("Newbie", "1", GameGuideConfig.TYPE_APPEARANCE);
        //        }
        //    });
        //}
    }

    void ResetIndex()
    {
        lastId[0] = ownSkins[0];
        lastId[1] = ownBackgrounds[0];
    }


    void RefreshList()
    {
        selectList.GetChildAt(1).asCom.GetController("c1").selectedIndex = RedpointMgr.Ins.BackgroundHaveRedpoint() ? 1 : 0;
        if (selectList.selectedIndex == 0)
            currentList = currentSkinList;
        else
            currentList = currentBackGroundList;
        appearanceList.SetVirtual();
        appearanceList.itemRenderer = RenderListItem;
        appearanceList.numItems = currentList.Count;
        //appearanceList.onClickItem.Set(OnClickItem); 
        SetGoodsItemEffect();
    }

    void GetList()
    {
        List<GameActorSkinConfig> skins = JsonConfig.GameActorSkinConfigs.FindAll(a => a.actor_id == playerActor.actor_id && a.status == 1 &&
             (a.type == GameActorSkinConfig.SKINS_TYPE || a.type == GameActorSkinConfig.DEFAULT_SKINS_TYPE));
        GetCurrentList(ownSkins, currentSkinList, skins);
        List<GameActorSkinConfig> backgrounds = JsonConfig.GameActorSkinConfigs.FindAll(a => a.status == 1 &&
           (a.type == GameActorSkinConfig.BACKGROUND_TYPE || a.type == GameActorSkinConfig.HUMAN_BACKGROUND_TYPE
           || a.type == GameActorSkinConfig.FAIRY_BACKGROUND_TYPE));
        GetCurrentList(ownBackgrounds, currentBackGroundList, backgrounds, true);
        RefreshList();
    }
    /// <summary>
    /// 获取皮肤或背景列表并分类
    /// </summary>
    void GetCurrentList(List<int> ownList, List<GameActorSkinConfig> current, List<GameActorSkinConfig> skins, bool isBackground = false)
    {
        current.Clear();
        List<GameActorSkinConfig> haveSkins = new List<GameActorSkinConfig>();
        List<GameActorSkinConfig> redPoint = new List<GameActorSkinConfig>();
        if (skins != null)
        {
            for (int i = skins.Count - 1; i >= 0; i--)
            {
                if (ownList.Contains(skins[i].id))
                {
                    haveSkins.Add(skins[i]);
                    skins.Remove(skins[i]);
                }
                else if (isBackground)
                {

                    GamePropFragmentConfig fragmentConfig = JsonConfig.GamePropFragmentConfigs.Find(a => a.prop_id == skins[i].id);
                    if (fragmentConfig != null)
                    {
                        TinyItem tiny = ItemUtil.GetTinyItem(fragmentConfig.fragment);
                        if (RedpointMgr.Ins.backgroundRedpoint.Contains(tiny.id))
                        {
                            redPoint.Add(skins[i]);
                            skins.Remove(skins[i]);
                        }
                    }
                }
            }
            haveSkins.Sort(SortSkins);
            redPoint.Sort(SortSkins);
            skins.Sort(SortSkins);
            if (haveSkins.Count > 1)
            {
                //将当前外观放到第一
                GameActorSkinConfig skin = haveSkins.Find(a => a.id == ownList[0]);
                if (skin != null)
                {
                    haveSkins.Remove(skin);
                    haveSkins.Insert(0, skin);
                }
            }
            current.AddRange(haveSkins);
            current.AddRange(redPoint);
            current.AddRange(skins);
        }
    }


    //根据id排序
    private int SortSkins(GameActorSkinConfig tr1, GameActorSkinConfig tr2)
    {
        return tr1.id.CompareTo(tr2.id);
    }

    /// <summary>
    /// 增加列表铺开效果
    /// </summary>
    void SetGoodsItemEffect()
    {
        Vector3 pos = new Vector3();
        for (int i = 0; i < appearanceList.numChildren; i++)
        {
            GObject item = appearanceList.GetChildAt(i);
            item.alpha = 0;

            pos = item.position;
            if (pos == Vector3.zero)
                pos.y = i * 354f;

            item.SetPosition(pos.x, pos.y + 100, pos.z);

            float time = i * 0.1f;

            item.TweenMoveY(pos.y, (time + 0.2f)).SetEase(EaseType.CubicOut).OnStart(() =>
            {
                item.TweenFade(1, (time + 0.3f)).SetEase(EaseType.QuadOut);
            });
        }
    }


    void RenderListItem(int index, GObject obj)
    {
        GComponent item = obj.asCom;
        Controller c1 = item.GetController("c1");
        Controller c2 = item.GetController("c2");
        Controller c3 = item.GetController("c3");
        Controller redpoint = item.GetController("c4");

        int c1Index;
        int c2Index;
        int c3Index;
        //Debug.LogError("******************* index  " + index + "    count   " + currentSkinList.Count);
        List<int> own;
        if (selectList.selectedIndex == 0)
        {
            own = ownSkins;
            c3Index = 0;
        }
        else
        {
            own = ownBackgrounds;
            c3Index = 1;
        }

        TinyItem tinyItem = ItemUtil.GetTinyItem(currentList[index].price);
        item.GetChild("n24").asLoader.url = CommonUrlConfig.GetConsumeItemUrl((TypeConfig.Consume)tinyItem.type);
        item.GetChild("n9").asTextField.text = tinyItem.num.ToString();
        item.GetChild("n1").asLoader.url = UrlUtil.GetActorIconUrl(currentList[index].id);
        item.GetChild("n3").asTextField.text = currentList[index].name_cn;

        item.GetChild("n11").onClick.Set(() => { OnClickItem(index); });
        if (currentList[index].id == own[0])
        {
            c1Index = 1;
            item.GetChild("n2").asTextField.text = selectList.selectedIndex == 0 ? "当前时装" : "当前背景";
        }
        else if (own.Contains(currentList[index].id))
        {
            c1Index = 2;
        }
        else
        {
            item.GetChild("n6").onClick.Set(() => { OnClickBuy(currentList[index].id, index); });
            c1Index = 0;
        }

        if (currentList[index].id == lastId[selectIndex])
            c2Index = 0;
        else
            c2Index = 1;

        c1.selectedIndex = c1Index;
        c2.selectedIndex = c2Index;
        c3.selectedIndex = c3Index;
        redpoint.selectedIndex = 0;
        if (selectList.selectedIndex == 1)
        {
            GamePropFragmentConfig fragmentConfig = JsonConfig.GamePropFragmentConfigs.Find(a => a.prop_id == currentList[index].id);
            if (fragmentConfig != null)
            {
                TinyItem tiny = ItemUtil.GetTinyItem(fragmentConfig.fragment);
                redpoint.selectedIndex = RedpointMgr.Ins.backgroundRedpoint.Contains(tiny.id) ? 1 : 0;
            }
        }
    }

    void OnClickSelect(EventContext context)
    {
        OpenListEffect();
        if (selectIndex != selectList.selectedIndex)
        {
            selectIndex = selectList.selectedIndex;
            RefreshList();
        }
    }

    void OnClickItem(int realIndex)
    {
        AudioClip audioClip = Resources.Load(SoundConfig.INTERACTIVE_AUDIO_EFFECT_URL + (int)SoundConfig.InteractiveAudioId.SkinClick) as AudioClip;
        GRoot.inst.PlayEffectSound(audioClip);
        ChangeSelect(realIndex);
    }

    void ChangeSelect(int realIndex)
    {
        //当前可视item中的index
        int index = appearanceList.ItemIndexToChildIndex(realIndex);

        for (int i = 0; i < appearanceList.numChildren; i++)
        {
            appearanceList.GetChildAt(i).asCom.GetController("c2").selectedIndex = 1;
        }
        appearanceList.GetChildAt(index).asCom.GetController("c2").selectedIndex = 0;
        lastId[selectIndex] = currentList[realIndex].id;


        if (selectIndex == 0)
        {
            Debug.LogError("------------------" + currentList[realIndex].id);
            GameActorSkinConfig skinConfig = DataUtil.GetGameActorSkinConfig(currentList[realIndex].id);
            EventMgr.Ins.DispachEvent(EventConfig.CHANGE_SPINE, skinConfig);
            if (ownSkins.Contains(skinConfig.id)&&int.Parse(GameData.Player.homeActor.skin)!= skinConfig.id)
            {
                OnClickSave();
            }

        }
        else
        {
            int bgId = currentList[realIndex].id;
            if (ownBackgrounds.Contains(bgId))
            {
                if (bgId != InteractiveMainMoudle.ins.currentBg)
                {
                    EventMgr.Ins.DispachEvent(EventConfig.CHANGE_BACKGROUND, currentBackGroundList[realIndex].id);
                    FXMgr.CreateEffectWithScale(spineGcom, new Vector2(700, 792), "UI_changebg", 1, 1.5f, 0);
                OnClickSave();
                }

            }
            else
            {
                ShowBuyView(bgId);
            }

        }
    }


    Extrand commonTipsInfo;
    void CheckSave()
    {
        if ((ownSkins.Contains(lastId[0]) && lastId[0] != ownSkins[0]) ||
            (ownBackgrounds.Contains(lastId[1]) && lastId[1] != ownBackgrounds[0]))
        {
            if (commonTipsInfo == null)
            {
                commonTipsInfo = new Extrand();
                commonTipsInfo.key = "提示";
                commonTipsInfo.msg = MsgException.APPEARANCE_CHANGE;
                commonTipsInfo.callBack = GotoInteractive;
            }
            UIMgr.Ins.showNextPopupView<PopupDialogView, Extrand>(commonTipsInfo);
        }
        else
        {
            GotoInteractive();
        }

    }

    void GotoInteractive()
    {
        EventMgr.Ins.DispachEvent(EventConfig.CHANGE_BACKGROUND, ownBackgrounds[0]);
        EventMgr.Ins.DispachEvent(EventConfig.HIDE_TIPS_PANEL);
        CloseListEffect();
        baseView.GoToMoudle<InteractiveMainMoudle>((int)InteractiveView.MoudleType.INTERACTIVE);
    }

    void ShowBuyView(int id)
    {
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("propId", id);
        InteractiveDataMgr.ins.BackgroundId = id;
        GameMonoBehaviour.Ins.RequestInfoPost(NetHeaderConfig.FRAGMENT, wWWForm, (ActorFragmentRespo fragmentRespo) =>
        {
            UIMgr.Ins.showNextPopupView<BuyBackgroundView, ActorFragmentRespo>(fragmentRespo);
        });
    }


    Extrand extrand;
    void OnClickBuy(int id, int realIndex)
    {
        ChangeSelect(realIndex);
        if (selectList.selectedIndex == 0)
        {
            if (extrand == null)
            {
                extrand = new Extrand();
                extrand.key = "提示";
                extrand.msg = "确认购买该时装？";
                extrand.callBack = RequestSkinBuy;
            }
            UIMgr.Ins.showNextPopupView<PopupDialogView, Extrand>(extrand);
        }
        else
        {
            if (ownBackgrounds.Contains(id) && id != ownBackgrounds[0])
            {
                EventMgr.Ins.DispachEvent(EventConfig.CHANGE_BACKGROUND, id);
                FXMgr.CreateEffectWithScale(spineGcom, new Vector2(700, 792), "UI_changebg", 1, 1.5f, 0);
            }
            else
                ShowBuyView(id);
        }
    }

    void RequestSkinBuy()
    {
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("actorId", playerActor.actor_id);
        wWWForm.AddField("skinId", lastId[0]);
        GameMonoBehaviour.Ins.RequestInfoPost<ActorProperty>(NetHeaderConfig.ACTOR_SKIN_BUY, wWWForm, RequestSkinBuySuccess);
    }

    void RequestSkinBuySuccess(ActorProperty actor)
    {
        EventMgr.Ins.DispachEvent(EventConfig.MUSIC_COMMON_EFFECT, normalInfo);
        TinyItem tinyItem = new TinyItem();
        tinyItem.type = (int)TypeConfig.Consume.Item;
        tinyItem.id = actor.playerActor.actor_id;
        tinyItem.num = lastId[0];
        ShowBuySuceessEffect();
        GetList();
    }


    void OnClickSave()
    {
        if (!ownSkins.Contains(lastId[0]))
        {
            UIMgr.Ins.showErrorMsgWindow(MsgException.NO_HAVE_SKIN);
            return;
        }
        if (!ownBackgrounds.Contains(lastId[1]))
        {
            UIMgr.Ins.showErrorMsgWindow(MsgException.NO_HAVE_BACKGROUND);
            return;
        }

        WWWForm wWWFormSkin = new WWWForm();
        wWWFormSkin.AddField("actorId", playerActor.actor_id);
        wWWFormSkin.AddField("skinId", lastId[0]);
        wWWFormSkin.AddField("backgroundId", lastId[1]);
        GameMonoBehaviour.Ins.RequestInfoPost<PlayerActor>(NetHeaderConfig.ACTOR_SET, wWWFormSkin, RequestSkinSetSuccess);
    }

    void RequestSkinSetSuccess()
    {
        UIMgr.Ins.showErrorMsgWindow(MsgException.SAVE_SUCCESSFULLY);
        GetList();
    }

    #region 动画
    int offset = 0;
    void OpenListEffect()
    {
        listGroup.TweenMoveX(468, 0.5f);
        saveBtn.TweenMoveX(-468, 0.5f);
        shareBtn.TweenMoveX(-468, 0.5f);
        spineGraph.TweenMoveX(-120, 0.5f);
        offset = -120;
    }

    void CloseListEffect()
    {
        listGroup.TweenMoveX(688, 0.5f);
        saveBtn.TweenMoveX(saveBtnPosX, 0.5f);
        shareBtn.TweenMoveX(saveBtnPosX, 0.5f);
        spineGraph.TweenMoveX(0, 0.5f);
        offset = 0;

    }
    void ShowBuySuceessEffect()
    {
        baseView.StartCoroutine(BuySuceessEffect());
    }

    GGraph gGraph;
    GGraph redGraph;
    GGraph blueGraph;
    List<Renderer> renderers;
    IEnumerator BuySuceessEffect()
    {
        GameInitCardsConfig cardsConfig = JsonConfig.GameInitCardsConfigs.Find(a => a.card_id == InteractiveDataMgr.ins.CurrentPlayerActor.actor_id);
        if (gGraph == null)
        {
            foreach (string name in textrueName)
            {
                Texture texture = Resources.Load(UrlUtil.GetTextureUrl(name)) as Texture;
                textures.Add(texture);
            }
            renderers = new List<Renderer>();
            gGraph = FXMgr.CreateEffectWithScale(spineGcom, new Vector2(624 + offset, 1082), "UI_changesuit1", 280, -1);
            for (int i = 1; i <= 4; i++)
            {
                Renderer bookRenderer = gGraph.displayObject.gameObject.transform.Find("UI_changesuit1(Clone)").Find("Path.0" + i).GetComponent<Renderer>();
                renderers.Add(bookRenderer);
            }
        }
        GGraph graph;
        if (cardsConfig.gender == 0)
        {
            if (redGraph == null)
            {
                redGraph = FXMgr.CreateEffectWithScale(spineGcom, new Vector2(624 + offset, 1082), "UI_changesuit2", 3.2f, -1);
            }
            graph = redGraph;
        }
        else
        {
            if (blueGraph == null)
            {
                blueGraph = FXMgr.CreateEffectWithScale(spineGcom, new Vector2(624 + offset, 1082), "UI_changesuit3", 3.2f, -1);
            }
            graph = blueGraph;
        }
        gGraph.displayObject.gameObject.SetActive(false);
        gGraph.displayObject.gameObject.SetActive(true);
        graph.displayObject.gameObject.SetActive(false);
        graph.displayObject.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        foreach (Renderer renderer in renderers)
            renderer.material.mainTexture = textures[cardsConfig.gender];

        yield return new WaitForSeconds(2.5f);
        graph.displayObject.gameObject.SetActive(false);
        gGraph.displayObject.gameObject.SetActive(false);
    }
    #endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using Spine;
using Spine.Unity;
using System;

public class GemFindMoudle : BaseMoudle
{
    /// <summary>
    /// 已获得剑碎片
    /// </summary>
    List<int> gettedSwprdList = new List<int>();
    /// <summary>
    /// 已获得宝石
    /// </summary>
    List<int> gettedGemsList = new List<int>();
    /// <summary>
    /// 碎片回到对应位置x,y,旋转
    /// </summary>
    readonly List<string> swordPosPairs = new List<string>()
    {
        "238,1210,-57",
        "364,1228,0",
        "426,1244,-17",
        "512,1213,-18",
        "92,1259,0",
        "297,1242,0",
    };

    string NewbieKey = "newbie";

    GComponent tbTip0;
    GComponent tbTip1;
    GGraph lightGraph;
    Controller controller;
    GLoader bgLoader;
    GImage shadow;
    GGraph getSwordGraph;
    //狮头闪光
    GGraph lionGraph;
    //黄宝石闪光
    GGraph yellowGemGraph;
    GObject gridGroup;

    List<GImage> gridList = new List<GImage>();
    List<GComponent> swordList = new List<GComponent>();
    List<GComponent> gemsList = new List<GComponent>();
    List<GImage> tbList = new List<GImage>();
    //藤蔓砍位置
    List<GComponent> tbDropList = new List<GComponent>();

    AudioClip getItemAudio;
    AudioClip swordCutAudio;

    //剑动画
    SkeletonAnimation swordAni;
    GComponent sword;
    GObject swordBg;

    GComponent swordAnime;

    string gemsSave = "";
    string swordsSave = "";
    string tbSave = "";

    //宝石与grid位置偏移
    float gemPosWidth;
    float gemPosHeight;

    bool isNewbie;

    public StoryGameInfo storyGameInfo;
    public override void InitMoudle(GComponent gComponent, int controllerIndex)
    {
        base.InitMoudle(gComponent, controllerIndex);
        InitUI();
        InitEvent();
    }

    public override void InitUI()
    {
        base.InitUI();
        tbTip0 = SearchChild("n23").asCom;
        tbTip1 = SearchChild("n24").asCom;
        tbTip0.visible = false;
        tbTip1.visible = false;
        controller = ui.GetController("c1");
        shadow = SearchChild("n15").asImage;
        bgLoader = SearchChild("n0").asLoader;
        sword = SearchChild("n37").asCom;
        swordBg = SearchChild("n1");
        gridGroup = SearchChild("n8");
        getSwordGraph = SearchChild("n35").asCom.GetChild("n5").asGraph;
        lionGraph = SearchChild("n47").asGraph;
        yellowGemGraph = SearchChild("n48").asGraph;
        lightGraph = SearchChild("n63").asGraph;
        swordAnime = SearchChild("n64").asCom;
        swordAnime.visible = false;
        GetGridList(gridList);
        GetSwordList();
        GetGemsList(gemsList);
        GetTBList();

        gemPosWidth = (gridList[0].width - gemsList[0].width) / 2;
        gemPosHeight = (gridList[0].height - gemsList[0].height) / 2;

    }

    #region  初始化
    void GetGridList(List<GImage> gList)
    {
        for (int i = 0; i < 6; i++)
        {
            GImage gImage = SearchChild("n" + (i + 2)).asImage;
            gList.Add(gImage);
        }
    }

    void GetSwordList()
    {
        for (int i = 0; i < 6; i++)
        {
            int index = i;
            GComponent gImage = SearchChild("n" + (i + 16)).asCom;
            gImage.onClick.Set(() => { OnClickSwordDebris(index); });
            swordList.Add(gImage);
        }
    }
    /// <summary>
    /// 按照赤橙黄青蓝紫顺序获得宝石组件
    /// </summary>
    void GetGemsList(List<GComponent> gList)
    {
        for (int i = 0; i < 6; i++)
        {
            int index = i;
            GComponent gImage = SearchChild("n" + (i + 9)).asCom;
            gImage.onDrop.Set(SwordBGOnDrop);
            gImage.onClick.Set(() => { OnClickGems(index); });
            gList.Add(gImage);
        }
    }

    void GetTBList()
    {
        for (int i = 0; i < 6; i++)
        {
            int index = i;
            GImage gImage = SearchChild("n" + (i + 49)).asImage;
            tbList.Add(gImage);
            GComponent gCom = SearchChild("n" + (i + 56)).asCom;
            gCom.onClick.Set(() => { OnClickTreeBranch(index); });
            gCom.onDrop.Set(() => { TBOnDrop(index); });
            tbDropList.Add(gCom);
        }
    }


    #endregion

    public override void InitEvent()
    {
        base.InitEvent();


        //拿起剑
        SearchChild("n35").asCom.GetChild("n4").onClick.Set(() =>
        {
            controller.selectedIndex = 5;
            sword.position = new Vector2(510, 623);
            sword.touchable = false;
            sword.TweenMove(new Vector2(510, 1248), 1f).OnComplete(() =>
            {
                sword.touchable = true;
            });
        });
        SearchChild("n40").asCom.onDrop.Set(SwordBGOnDrop);
        baseView.SearchChild("n26").onClick.Set(OnClickTips);

    }

    public override void InitData<D>(D data)
    {
        base.InitData(data);
        storyGameInfo = data as StoryGameInfo;

        bgLoader.url = UrlUtil.GetGameBGUrl(12);
        FXMgr.CreateEffectWithGGraph(lightGraph, new Vector3(0, 0), "UI_game1_lightline", 162);
        sword.draggable = true;
        sword.onDragStart.Set(OnDragStart);
        sword.onDragEnd.Set(OnDragEnd);
        GetGemsSave();
        GetSwordsSave();
        GetTbSave();
        if (gettedSwprdList.Count >= 6)
            controller.selectedIndex = 5;
        else
            controller.selectedIndex = 0;

       StoryGameSave save = storyGameInfo.gameSaves.Find(a => a.ckey == NewbieKey);
        if (save == null)
        {
            isNewbie = true;
        }

    }

    /// <summary>
    /// 获取宝石拾取信息
    /// </summary>
    void GetGemsSave()
    {
        FXMgr.CreateEffectWithGGraph(yellowGemGraph, new Vector3(348, 1035), "Game1_star", 162);
        StoryGameSave storyGameSave = storyGameInfo.gameSaves.Find(a => a.ckey == GameFindGemsView.GEMS_KEY);
        if (storyGameSave != null)
        {
            gemsSave = storyGameSave.value;
            string[] gems = storyGameSave.value.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries); ;
            foreach (string g in gems)
            {
                int index = int.Parse(g);
                Vector2 pos = new Vector2(gridList[gettedGemsList.Count].x + gemPosWidth, gridList[gettedGemsList.Count].y + gemPosHeight);
                gemsList[index].xy = pos;
                gemsList[index].rotation = 0;
                gettedGemsList.Add(index);
                gemsList[index].onClick.Clear();
                if (index == 2)
                    yellowGemGraph.displayObject.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 获取剑碎片拾取信息
    /// </summary>
    void GetSwordsSave()
    {
        swordAni = FXMgr.LoadSpineEffect("Sword", sword.GetChild("n37").asGraph, new Vector2(393, 20), 100);
        FXMgr.CreateEffectWithGGraph(lionGraph, new Vector3(365, 769), "Game1_star", 162);
        StoryGameSave storyGameSave = storyGameInfo.gameSaves.Find(a => a.ckey == GameFindGemsView.SWORD_KEY);
        if (storyGameSave != null)
        {
            swordsSave = storyGameSave.value;
            string[] swords = storyGameSave.value.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string g in swords)
            {
                int index = int.Parse(g);
                string[] pos = swordPosPairs[index].Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                gettedSwprdList.Add(index);
                if (index == 0)
                {
                    shadow.visible = false;
                    shadow.displayObject.gameObject.SetActive(false);
                }
                if (index == 5)
                {
                    lionGraph.displayObject.gameObject.SetActive(false);
                }
                swordList[index].onClick.Clear();
                swordList[index].xy = new Vector2(int.Parse(pos[0]), int.Parse(pos[1]));
                swordList[index].rotation = int.Parse(pos[2]);
            }
        }
    }

    /// <summary>
    /// 获取藤蔓被砍信息
    /// </summary>
    void GetTbSave()
    {
        StoryGameSave storyGameSave = storyGameInfo.gameSaves.Find(a => a.ckey == GameFindGemsView.TB_KEY);

        if (storyGameSave != null)
        {
            tbSave = storyGameSave.value;
            string[] tb = storyGameSave.value.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (tb.Length > 6)
            {
                tbSave = "";
                return;
            }
            foreach (string g in tb)
            {
                int index = int.Parse(g);
                tbList[index].alpha = 0;
                tbDropList[index].touchable = false;
            }
        }
    }

    /// <summary>
    /// 触碰宝石
    /// </summary>
    void OnClickGems(int index)
    {
        Vector2 pos = new Vector2(gridList[gettedGemsList.Count].x + gemPosWidth, gridList[gettedGemsList.Count].y + gemPosHeight);
        GetEffect(gemsList[index], pos, 0);
        gettedGemsList.Add(index);
        gemsSave += index + ",";
        GameTool.SaveGameInfo(GameFindGemsView.GEMS_KEY, gemsSave, storyGameInfo.gamePointConfig.id);
        gemsList[index].onClick.Clear();
        if (index == 2)
            yellowGemGraph.displayObject.gameObject.SetActive(false);
        PlayGetAudio();
        if (gettedGemsList.Count >= 6)
            baseView.StartCoroutine(ScenceChange());
    }

    /// <summary>
    /// 触碰剑碎片
    /// </summary>
    void OnClickSwordDebris(int index)
    {
        string[] pos = swordPosPairs[index].Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        gettedSwprdList.Add(index);
        swordsSave += index + ",";
        GameTool.SaveGameInfo(GameFindGemsView.SWORD_KEY, swordsSave, storyGameInfo.gamePointConfig.id);

        if (index == 1)
        {
            shadow.visible = false;
            shadow.displayObject.gameObject.SetActive(false);
        }
        if (index == 5)
        {
            lionGraph.displayObject.gameObject.SetActive(false);
        }
        swordList[index].onClick.Clear();
        PlayGetAudio();
        GetEffect(swordList[index], new Vector2(int.Parse(pos[0]), int.Parse(pos[1])), int.Parse(pos[2]));
        if (gettedSwprdList.Count >= 6)
            baseView.StartCoroutine(GetSwordEffect());
    }

    IEnumerator GetSwordEffect()
    {
        yield return new WaitForSeconds(1.8f);
        controller.selectedIndex = 4;
        FXMgr.CreateEffectWithGGraph(getSwordGraph, new Vector3(230, 0, 0), "Game1_swordframe", 162);
            
        if(isNewbie == true)
        {
            swordAnime.visible = true;
            isNewbie = false;
            GameTool.SaveGameInfo(NewbieKey, "1", storyGameInfo.gamePointConfig.id);

        }

    }

    /// <summary>
    /// 触碰树枝
    /// </summary>
    /// <param name="index"></param>
    void OnClickTreeBranch(int index)
    {
        GComponent select = null;
        switch (index)
        {
            case 0:
            case 1:
                select = tbTip0;
                break;
            case 2:
                select = tbTip1;
                break;
        }
        if (select != null)
            baseView.StartCoroutine(ShowTreeBaranchTips(select));
    }

    /// <summary>
    /// 点击树枝提示
    /// </summary>
    IEnumerator ShowTreeBaranchTips(GComponent com)
    {
        com.visible = true;
        com.displayObject.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        com.visible = false;
        com.displayObject.gameObject.SetActive(false);
    }

    #region 拖拽剑
    void OnDragStart(EventContext evt)
    {
        TrackEntry trackEntry = swordAni.AnimationState.SetAnimation(0, "animation", true);
        trackEntry.Loop = false;
        trackEntry.AnimationStart = 0f;
        trackEntry.AnimationEnd = 1f;
        swordAni.timeScale = 1;

        Vector2 pos = GameTool.MousePosToUI(Input.mousePosition, ui);
        pos = new Vector2(pos.x + 80, pos.y + 80);
        sword.touchable = false;
        sword.TweenMove(pos, 0.1f);
    }

    void OnDragEnd()
    {
        GObject obj = GRoot.inst.touchTarget;
        while (obj != null)
        {
            if (obj.hasEventListeners("onDrop"))
            {
                obj.RequestFocus();
                obj.DispatchEvent("onDrop");
                return;
            }
            obj = obj.parent;
        }
    }

    void TBOnDrop(int index)
    {
        tbList[index].alpha = 1;
        tbSave += index + ",";
        if (swordCutAudio == null)
            swordCutAudio = Resources.Load(SoundConfig.GAME_AUDIO_EFFECT_URL + (int)SoundConfig.GameAudioId.Sword) as AudioClip;
        GRoot.inst.PlayEffectSound(swordCutAudio);
        GameTool.SaveGameInfo(GameFindGemsView.TB_KEY, tbSave, storyGameInfo.gamePointConfig.id);
        baseView.StartCoroutine(SwordCutTreesEffect(index));
        swordAnime.visible = false;

    }

    IEnumerator SwordCutTreesEffect(int index)
    {
        TrackEntry trackEntry = swordAni.AnimationState.SetAnimation(0, "animation", true);
        trackEntry.Loop = false;
        trackEntry.AnimationLast = 1f;
        trackEntry.AnimationStart = 1f;
        trackEntry.AnimationEnd = 160f / 60f;
        yield return new WaitForSeconds(95f / 60f - 1f);
        tbList[index].alpha = 1;
        tbList[index].TweenFade(0, 1f);
        tbDropList[index].touchable = false;
        yield return new WaitForSeconds(160f / 60f - 126f / 60f);
        sword.TweenMove(new Vector2(510, 1248), 1f).OnComplete(() =>
        {
            sword.touchable = true;
        });
    }

    void SwordBGOnDrop()
    {
        baseView.StartCoroutine(SwordRebackEffect());
        UIMgr.Ins.showErrorMsgWindow("这里没有什么，换个地方试试吧");
    }

    IEnumerator SwordRebackEffect()
    {
        TrackEntry trackEntry = swordAni.AnimationState.SetAnimation(0, "animation", true);
        trackEntry.Loop = false;
        trackEntry.AnimationLast = 162f / 60f;
        trackEntry.AnimationStart = 162f / 60f;
        trackEntry.AnimationEnd = 220f / 60f;
        yield return new WaitForSeconds(200f / 60f - 162f / 60f);
        sword.TweenMove(new Vector2(510, 1248), 1f).OnComplete(() =>
        {
            sword.touchable = true;
        });
    }
    #endregion

    /// <summary>
    /// 拾取动画
    /// </summary>
    void GetEffect(GObject gObject, Vector2 pos, int angle = 0)
    {
        gObject.TweenMove(new Vector2(375, 812), 0.6f).OnComplete(() =>
        {
            gObject.TweenRotate(angle, 1f);
            gObject.TweenMove(pos, 1f);
        });
    }


    /// <summary>
    /// 场景变换
    /// </summary>
    IEnumerator ScenceChange()
    {

        StoryGameSave gameSave = storyGameInfo.gameSaves.Find(a => a.ckey == GameFindGemsView.GEMS_KEY);
        if (gameSave != null)
        {
            gameSave.value = gemsSave;
        }
        else
        {
            gameSave = new StoryGameSave()
            {
                ckey = GameFindGemsView.GEMS_KEY,
                value = gemsSave,
            };
            storyGameInfo.gameSaves.Add(gameSave);
        }
        yield return new WaitForSeconds(2f);
        sword.visible = false;
        gridGroup.visible = false;
        swordBg.visible = false;
        UIMgr.Ins.showNextPopupView<GameScenceChangeView>();
        ui.pivot = new Vector2(0.5f, 0.5f);
        ui.TweenScale(new Vector2(1.82f, 1.82f), 1f);
        yield return new WaitForSeconds(1f);
        baseView.GoToMoudle<GemPutMoudle, StoryGameInfo>((int)GameFindGemsView.MoudleType.TYPE_PUT, storyGameInfo);
    }

    Extrand extrand;
    void OnClickTips()
    {
        DoTips();
        //if (extrand == null)
        //{
        //    extrand = new Extrand();
        //    extrand.type = 1;
        //    extrand.key = "提示";
        //    List<GameNodeConsumeConfig> list = JsonConfig.GameNodeConsumeConfigs.FindAll(a => a.node_id == storyGameInfo.gamePointConfig.id && a.type == storyGameInfo.gamePointConfig.type);
        //    TinyItem item = ItemUtil.GetTinyItem(list[0].pay);
        //    extrand.item = item;
        //    extrand.msg = "获得提示";
        //    extrand.callBack = DoTips;
        //}
        //UIMgr.Ins.showNextPopupView<PopupDialogView, Extrand>(extrand);

    }

    void DoTips()
    {
        if (gettedSwprdList.Count < 6)
        {
            for(int i = 0; i < swordList.Count; i++)
            {
                if (!gettedSwprdList.Contains(i))
                {
                    OnClickSwordDebris(i);
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < gemsList.Count; i++)
            {
                if (!gettedGemsList.Contains(i))
                {
                    switch (i)
                    {
                        case 0:
                            TBOnDrop(2);
                            break;
                        case 3:
                            TBOnDrop(1);
                            break;
                        case 4:
                            TBOnDrop(3);
                            break;
                    }
                    OnClickGems(i);
                    break;
                }
            }
        }
    }

    void PlayGetAudio()
    {
        if (getItemAudio == null)
            getItemAudio = Resources.Load(SoundConfig.GAME_AUDIO_EFFECT_URL + (int)SoundConfig.GameAudioId.GetItems) as AudioClip;
        GRoot.inst.PlayEffectSound(getItemAudio);
    }
}

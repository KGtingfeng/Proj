using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using System;
using System.Linq;

public class GemPutMoudle : BaseMoudle
{
    GameFindGemsView view;

    AudioClip putDownAudio;

    Controller putController;
    GLoader putBg;
    GTextField putTipText;
    GImage tipImage; 
    GGraph baoshiGraph;
    List<GComponent> putGemsList = new List<GComponent>();
    List<GImage> putGridList = new List<GImage>();
    List<GComponent> putHoleList = new List<GComponent>();
    /// <summary>
    /// 宝石放在洞的位置
    /// </summary>
    Dictionary<int, int> placeIds = new Dictionary<int, int>();
    /// <summary>
    /// 已获得宝石
    /// </summary>
    List<int> gettedGemsList = new List<int>();
    //宝石与grid位置偏移
    float gemPosWidth;
    float gemPosHeight;
    //宝石与洞位置偏移
    float holeWidth;
    float holeHeight;

    GComponent tap;
    GComponent tap1;

    GComponent find;

    string NewbieKey = "newbiePut";
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
        putController = ui.GetController("c1");
        putBg = SearchChild("n0").asLoader;
        putTipText = SearchChild("n23").asTextField;
        tipImage = SearchChild("n24").asImage;
        baoshiGraph = SearchChild("n39").asGraph; 
        tap = SearchChild("n46").asCom;
        tap1 = SearchChild("n47").asCom;

        tap.visible = false;
        tap1.visible = false;
        GetGemsList(putGemsList);
        GetGridList(putGridList);
        GetHoleList();
        holeWidth = (putHoleList[0].width - putGemsList[0].width) / 2;
        holeHeight = (putHoleList[0].height - putGemsList[0].height) / 2;
        gemPosWidth = (putGridList[0].width - putGemsList[0].width) / 2;
        gemPosHeight = (putGridList[0].height - putGemsList[0].height) / 2;

        find = SearchChild("n38").asCom;
        find.visible = false;
    }

    #region  初始化
    void GetGridList(List<GImage> gList)
    {
        for (int i = 0; i < 6; i++)
        {
            GImage gImage = SearchChild("n" + (i + 1)).asImage;
            gList.Add(gImage);
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
            GComponent gImage = SearchChild("n" + (i + 8)).asCom;
            gImage.draggable = false;
            gImage.onClick.Set((EventContext evt) => { OnGemClick(index); });
            gList.Add(gImage);
        }
    }

    void GetHoleList()
    {
        for (int i = 0; i < 6; i++)
        {
            int index = i;
            GComponent gcom = SearchChild("n" + (i + 17)).asCom;
            gcom.onClick.Set((EventContext evt) => { OnHoleClick(index); });
            putHoleList.Add(gcom);
        }
    }
    #endregion


    public override void InitEvent()
    {
        base.InitEvent();
        EventMgr.Ins.RegisterEvent(EventConfig.PUT_GEMS_SAVE, SaveGameInfo);
        baseView.SearchChild("n26").onClick.Set(OnClickTips);
    }

    public override void InitData<D>(D data)
    {
        base.InitData(data);
        storyGameInfo = data as StoryGameInfo;
        view = baseView as GameFindGemsView;
        putBg.url = UrlUtil.GetGameBGUrl(13);
        putTipText.text = storyGameInfo.gamePointConfig.content1;
        GetGemsSave();
        for (int i = 0; i < gettedGemsList.Count; i++)
        {
            int index = gettedGemsList.IndexOf(i);
            putGemsList[i].xy = new Vector2(putGridList[index].x + gemPosWidth, putGridList[index].y + gemPosHeight);
        }
        GetPutGemsSave();
        StoryGameSave save = storyGameInfo.gameSaves.Find(a => a.ckey == NewbieKey);
        if (save == null)
        {
            isNewbie = true;
            tap1.visible = true;
            tap1.position = new Vector2(putGemsList[0].x + 40, putGemsList[0].y + 40);
            for (int i = 1; i < 6; i++)
            {
                putGemsList[i].touchable = false;
                putHoleList[i].touchable = false;
            }
        }

    }

    //IEnumerator ScencePutTips()
    //{
    //    tipImage.scale = Vector2.one * 1.2f;
    //    putTipText.scale = Vector2.one * 1.2f;
    //    tipImage.position = new Vector2(375, 720);
    //    yield return new WaitForSeconds(1f);
    //    tipImage.TweenScale(Vector2.one, 2);
    //    putTipText.TweenScale(Vector2.one, 2);

    //    tipImage.TweenMove(new Vector2(375, 1316), 2).OnComplete(() =>
    //    {
    //        tipMask.visible = false;
    //        tipMask.displayObject.gameObject.SetActive(false);
    //    });
    //}

    /// <summary>
    /// 获取宝石拾取信息
    /// </summary>
    void GetGemsSave()
    {
        string gemsSave = storyGameInfo.gameSaves.Find(a => a.ckey == GameFindGemsView.GEMS_KEY).value;
        string[] gems = gemsSave.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string g in gems)
        {
            int index = int.Parse(g);
            gettedGemsList.Add(index);
        }
    }

    /// <summary>
    /// 获取宝石放入信息
    /// </summary>
    void GetPutGemsSave()
    {
        if (storyGameInfo.gameSaves.Find(a => a.ckey == GameFindGemsView.HOLE_KEY) != null)
        {
            string holeSave = storyGameInfo.gameSaves.Find(a => a.ckey == GameFindGemsView.HOLE_KEY).value;
            string[] tb = holeSave.Split(new char[1] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (tb.Length >= 6)
            {
                view.OnComplete();
                return;
            }
            foreach (string g in tb)
            {
                string[] str = g.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                placeIds.Add(int.Parse(str[0]), int.Parse(str[1])); 
                Vector2 p = new Vector2(putHoleList[int.Parse(str[0])].x + holeWidth, putHoleList[int.Parse(str[0])].y + holeHeight);
                float r = putHoleList[int.Parse(str[0])].rotation;
                putGemsList[int.Parse(str[1])].TweenRotate(r, 0.2f);

                putGemsList[int.Parse(str[1])].xy = p;
                putGemsList[int.Parse(str[1])].onClick.Set(() =>
                {
                    OnClickDigGems(int.Parse(str[1]));
                });
                
            }
        }
    }



    #region  宝石装入
    int selectedGemIndex = -1;
    GGraph selectedGemFx = new GGraph();
    void DestroySelectedGemFx()
    {
        if (selectedGemIndex == -1) return;
        putGemsList[selectedGemIndex].RemoveChild(selectedGemFx);
        GameObject.Destroy(selectedGemFx.displayObject.gameObject);
        ChangeGemBrightness(putGemsList[selectedGemIndex], 0f);
    }
    void OnHoleClick(int index)
    {
        DestroySelectedGemFx();
        if (selectedGemIndex == -1) return; //没有选中宝石，直接退出
        //Debug.Log("第" + index + "个洞被点击被点击" + putGemsList[selectedGemIndex].touchable);
        if (isNewbie && index == 0 && selectedGemIndex == 0)
        {
            isNewbie = false;
            tap.visible = false;
            tap1.visible = false;
            for (int i = 1; i < 6; i++)
            {
                putGemsList[i].touchable = true;
                putHoleList[i].touchable = true;
            }
            tipImage.TweenScale(Vector2.one * 1.5f, 1f).OnComplete(() =>
            {
                tipImage.TweenScale(Vector2.one, 0.5f);

            });
            putTipText.TweenScale(Vector2.one * 1.5f, 1f).OnComplete(() =>
            {
                putTipText.TweenScale(Vector2.one, 0.5f);
            });
        }
            int nowDrag = selectedGemIndex;
        selectedGemIndex = -1;
        if (!placeIds.ContainsKey(index))//如果洞里没有宝石
        {
            if (putDownAudio == null)
                putDownAudio = Resources.Load(SoundConfig.GAME_AUDIO_EFFECT_URL + (int)SoundConfig.GameAudioId.PutDown) as AudioClip;
            GRoot.inst.PlayEffectSound(putDownAudio);
            placeIds.Add(index, nowDrag);
            Vector2 p = new Vector2(putHoleList[index].x + holeWidth, putHoleList[index].y + holeHeight);
            putGemsList[nowDrag].TweenMove(p, 0.2f);//当前宝石进洞
            float r = putHoleList[index].rotation;
            putGemsList[nowDrag].TweenRotate(r, 0.2f);
            //putGemsList[nowDrag].draggable = false;
            putGemsList[nowDrag].onClick.Set(() =>
            {
                OnClickDigGems(nowDrag);//给当前宝石绑定挖宝石事件
            });
            if (placeIds.Count >= 6)
            {
                CheckPlace();
            }
        }
        else//如果洞里有宝石，
        {
            GImage gImage = putGridList[gettedGemsList.IndexOf(nowDrag)];
            Vector2 pos = new Vector2(gImage.x + gemPosWidth, gImage.y + gemPosHeight);
            putGemsList[nowDrag].TweenMove(pos, 0.5f);
        }
         

    }

    //选中效果；改变宝石图片的亮度与对比度
    void ChangeGemBrightness(GObject gem, float bright)
    {
        if (gem.filter is ColorFilter filter)
        {
            filter.Reset();
            filter.AdjustBrightness(bright);
            filter.AdjustContrast(bright);
        }
    }

    void OnGemClick(int index)
    {
        DestroySelectedGemFx();
        if (isNewbie)
        {
            tap.visible = true;
            tap1.visible = false;
        }
        if (selectedGemIndex == index)
        {
            selectedGemIndex = -1;
            ChangeGemBrightness(putGemsList[index], 0f);
        }
        else
        {
            selectedGemIndex = index;
            Debug.Log("OnGemClick:" + index);
            selectedGemFx = FXMgr.CreateEffectWithScale(putGemsList[index], new Vector3(29,40,0), "Game1_star", 200, -1);
            ChangeGemBrightness(putGemsList[index], 0.5f);
            selectedGemFx.displayObject.layer = 5;
        }
        if (selectedGemIndex == -1) return;
        Debug.Log("第" + selectedGemIndex + "个宝石被点击:" + putGemsList[selectedGemIndex].touchable);
    }
    /// <summary>
    /// 挖出宝石的效果
    /// </summary>
    /// <param name="index">index:所挖出的宝石ID</param>
    void OnClickDigGems(int index)
    {
        DestroySelectedGemFx();
        selectedGemIndex = -1;
        Debug.Log("挖出宝石：" + index);
        int key = placeIds.Where(a => a.Value == index).FirstOrDefault().Key;
        placeIds.Remove(key);
        Vector2 pos = new Vector2(putGridList[gettedGemsList.IndexOf(index)].x + gemPosWidth, putGridList[gettedGemsList[index]].y + gemPosHeight);
        putGemsList[index].TweenMove(pos, 1f).OnComplete(() =>
        {
            putGemsList[index].onClick.Clear();
            //挖出宝石后，重新绑定点击宝石事件
            putGemsList[index].onClick.Set(() => { OnGemClick(index); });
            //putGemsList[index].draggable = true;
        });
        float r = putGridList[gettedGemsList.IndexOf(index)].rotation;
        putGemsList[index].TweenRotate(r, 1f);
    }

    #region 原本拖拽的逻辑
    //void OnGemDragStart(int index)
    //{
    //    selectedGemIndex = index;
    //    putGemsList[index].touchable = false;
    //}

    //void OnGemDragEnd()
    //{
    //    GObject obj = GRoot.inst.touchTarget;
    //    putGemsList[selectedGemIndex].touchable = true;
    //    while (obj != null)
    //    {
    //        if (obj.hasEventListeners("onDrop"))
    //        {
    //            obj.RequestFocus();
    //            obj.DispatchEvent("onDrop");
    //            return;
    //        }
    //        obj = obj.parent;
    //    }
    //}

    //void HoleOnDrop(int index)
    //{
    //    if (selectedGemIndex == -1) return;
    //    int nowDrag = selectedGemIndex;

    //    if (!placeIds.ContainsKey(index))
    //    {
    //        if (putDownAudio == null)
    //            putDownAudio = Resources.Load(SoundConfig.GAME_AUDIO_EFFECT_URL + (int)SoundConfig.GameAudioId.PutDown) as AudioClip;
    //        GRoot.inst.PlayEffectSound(putDownAudio);
    //        placeIds.Add(index, nowDrag);
    //        Vector2 p = new Vector2(putHoleList[index].x + holeWidth, putHoleList[index].y + holeHeight);
    //        putGemsList[nowDrag].TweenMove(p, 0.2f);
    //        putGemsList[nowDrag].draggable = false;
    //        putGemsList[nowDrag].onClick.Set(() =>
    //        {
    //            OnClickDigGems(nowDrag);
    //        });
    //        if (placeIds.Count >= 6)
    //        {
    //            CheckPlace();
    //        }
    //    }
    //    else
    //    {
    //        GImage gImage = putGridList[gettedGemsList.IndexOf(nowDrag)];
    //        Vector2 pos = new Vector2(gImage.x + gemPosWidth, gImage.y + gemPosHeight);
    //        putGemsList[nowDrag].TweenMove(pos, 1f);
    //    }
    //}

    /// <summary>
    /// 宝石没有放到对应位置
    /// </summary>
    //void BGOnDrop(EventContext eventContext)
    //{
    //    GImage gImage = putGridList[gettedGemsList.IndexOf(selectedGemIndex)];
    //    Vector2 pos = new Vector2(gImage.x + gemPosWidth, gImage.y + gemPosHeight);
    //    putGemsList[selectedGemIndex].TweenMove(pos, 1);
    //}
    #endregion

    void CheckPlace()
    {
        for (int i = 0; i < placeIds.Count; i++)
        {
            if (placeIds[i] != i)
            {
                for (int j = 0; j < gettedGemsList.Count; j++)
                {
                    Vector2 pos = new Vector2(putGridList[gettedGemsList.IndexOf(j)].x + gemPosWidth, putGridList[gettedGemsList[j]].y + gemPosHeight);
                    float rotation = putGridList[gettedGemsList.IndexOf(j)].rotation;
                    GemRebackGrid(putGemsList[j], pos, rotation, j);
                }
                placeIds.Clear();
                UIMgr.Ins.showErrorMsgWindow("宝石位置放错了，再试试吧！");
                return;
            }
        }
        GemsOnRightHole();
    }

    void RequestGetTips()
    {
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("nodeId", storyGameInfo.gamePointConfig.id);
        wWWForm.AddField("type", storyGameInfo.gamePointConfig.type);
        wWWForm.AddField("index", 2);
        GameMonoBehaviour.Ins.RequestInfoPost<PlayerProperty>(NetHeaderConfig.STROY_STEP_CONSUME, wWWForm, GetTip);
    }

    void GetTip()
    {
        for (int i = 5; i >= 0; i--)
        {
            if (placeIds.ContainsKey(i) && placeIds[i] != i)
            {
                int temp = placeIds[i];
                Vector2 pos = new Vector2(putGridList[gettedGemsList.IndexOf(placeIds[i])].x + gemPosWidth, putGridList[gettedGemsList[placeIds[i]]].y + gemPosHeight);
                float rotation = putGridList[gettedGemsList.IndexOf(placeIds[i])].rotation;
                GemRebackGrid(putGemsList[temp], pos, rotation, temp);
                placeIds.Remove(i);
            }
        }
        for (int j = 0; j < 6; j++)
        {
            if (!placeIds.ContainsKey(j))
            { 
                selectedGemIndex = j;
                OnHoleClick(j);
                break;
            }
        }
    }

    void GemRebackGrid(GComponent gem, Vector2 pos,float rotation,int index)
    {
        selectedGemIndex = -1;
        DestroySelectedGemFx();
        gem.TweenMove(pos, 1f).OnComplete(() =>
        {
            //gem.draggable = true;
        });
        gem.TweenRotate(rotation, 1f);
        gem.onClick.Clear();
        gem.onClick.Set((EventContext evt) => { OnGemClick(index); });
    }

    void GemsOnRightHole()
    {
        putController.selectedIndex = 1;
        foreach (var o in putGemsList)
        {
            o.onClick.Clear();
        }
        //FXMgr.CreateEffectWithGGraph(baoshiGraph, new Vector3(378, 459), "UI_game1_baoshiguang", 162); 
        FXMgr.CreateEffectWithGGraph(baoshiGraph, new Vector3(378, 805), "UI_Game1_final", 162);
        baseView.StartCoroutine(WaitEffect());
    }

    IEnumerator WaitEffect()
    {
        yield return new WaitForSeconds(7f);
        find.visible = true;
        find.GetTransition("t0").Play();
        SearchChild("n36").onClick.Set(() =>
        {
            view.OnComplete();
        });
    }
    #endregion


    /// <summary>
    /// 保存宝石放在哪个洞，不会实时保存，只有返回才保存
    /// </summary>
    void SaveGameInfo()
    {
        if (placeIds.Count == 0)
            return;
        string holeSave = "";
        for (int i = 0; i < 6; i++)
        {
            if (placeIds.ContainsKey(i))
                holeSave += i + "," + placeIds[i] + ";";
        }
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("nodeId", storyGameInfo.gamePointConfig.id);
        wWWForm.AddField("key", GameFindGemsView.HOLE_KEY);
        wWWForm.AddField("value", holeSave);
        GameMonoBehaviour.Ins.RequestInfoPost<StoryGameSave>(NetHeaderConfig.STROY_SAVE_GAME, wWWForm, null, false);
    }

    Extrand extrand;
    void OnClickTips()
    {
        GetTip();
        //if (extrand == null)
        //{
        //    extrand = new Extrand();
        //    extrand.type = 1;
        //    extrand.key = "提示";
        //    List<GameNodeConsumeConfig> list = JsonConfig.GameNodeConsumeConfigs.FindAll(a => a.node_id == storyGameInfo.gamePointConfig.id && a.type == storyGameInfo.gamePointConfig.type);
        //    TinyItem item = ItemUtil.GetTinyItem(list[2].pay);
        //    extrand.item = item;
        //    extrand.msg = "获得提示";
        //    extrand.callBack = RequestGetTips;
        //}
        //UIMgr.Ins.showNextPopupView<PopupDialogView, Extrand>(extrand);

    }
}

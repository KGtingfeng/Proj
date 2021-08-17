using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using System.Text;

[ViewAttr("Game/UI/Y_Game23", "Y_Game23", "Game23")]
public class GameMakeDollsView : BaseView
{
    GLoader bgLoader;

    GComponent gComponent;
    GComponent dollCom;
    GTextField dollComText;


    GTextField dollText;
    GTextField finishDollText;
    GTextField makeDollText;
    Controller talkController;
    GComponent wishDollCom;
    GComponent tipDollCom;
    GComponent tipBigDollCom;
    GComponent finishDollCom;
    GComponent finishDoll;
    GComponent finishDollBoxCom;
    GComponent actorDoll;
    GameXinlingConfig config;
    int dollNum;
    GComponent finishShowCom;
    GList btnList;
    GList fagList;

    Vector2 finishDollPos;
    Vector2 finishDollScale;


    AudioClip audioClip;

    List<GObject> children = new List<GObject>();
    List<Vector2> childernPos = new List<Vector2>();

    List<GameXinlingZzwwDollConfig> dollConfigs = new List<GameXinlingZzwwDollConfig>();

    List<GameXinlingZzwwFrgConfig> current;
    List<GameXinlingZzwwFrgConfig> hair = new List<GameXinlingZzwwFrgConfig>();
    List<GameXinlingZzwwFrgConfig> headwear = new List<GameXinlingZzwwFrgConfig>();
    List<GameXinlingZzwwFrgConfig> necklace = new List<GameXinlingZzwwFrgConfig>();
    List<GameXinlingZzwwFrgConfig> skirt = new List<GameXinlingZzwwFrgConfig>();
    List<GameXinlingZzwwFrgConfig> shoes = new List<GameXinlingZzwwFrgConfig>();
    List<List<int>> dolls = new List<List<int>>();


    //需要渲染的娃娃
    List<GLoader> itemLoaders = new List<GLoader>();
    List<GLoader> wishItemLoaders = new List<GLoader>();
    List<GLoader> tipItemLoaders = new List<GLoader>();
    List<GLoader> tipBigItemLoaders = new List<GLoader>();
    List<GLoader> finishDollLoaders = new List<GLoader>();
    List<GLoader> flyLoaders = new List<GLoader>();
    List<GLoader> finishDollBoxLoaders = new List<GLoader>();

    int[] doll = new int[5];

    int finishCount;
    public override void InitUI()
    {
        base.InitUI();
        gComponent = SearchChild("n1").asCom;
        bgLoader = SearchChild("n2").asLoader;
        //dollText = gComponent.GetChild("n9").asCom.GetChild("n13").asCom.GetChild("n8").asTextField;
        dollCom = gComponent.GetChild("n10").asCom;
        //dollComText = dollCom.GetChild("n8").asTextField;
        tipDollCom = gComponent.GetChild("n31").asCom;
        tipBigDollCom = gComponent.GetChild("n20").asCom.GetChild("n7").asCom;
        finishDoll = gComponent.GetChild("n23").asCom;
        finishDollBoxCom = finishDoll.GetChild("n24").asCom;
        finishShowCom = gComponent.GetChild("n19").asCom;
        finishDollCom = finishShowCom.GetChild("n19").asCom.GetChild("n24").asCom;
        //finishDollText = finishDoll.GetChild("n24").asTextField;
        talkController = gComponent.GetChild("n9").asCom.GetController("c1");
        wishDollCom = gComponent.GetChild("n9").asCom.GetChild("n13").asCom;
        controller = gComponent.GetController("c1");
        btnList = gComponent.GetChild("n16").asList;
        fagList = gComponent.GetChild("n24").asList;
        //makeDollText = gComponent.GetChild("n27").asTextField;
        finishDollPos = finishDoll.position;
        finishDollScale = finishDoll.scale;
        actorDoll = gComponent.GetChild("n32").asCom;
        children.Clear();
        for (int i = 3; i > 0; i--)
        {
            GObject o = gComponent.GetChild("n" + i);
            childernPos.Add(o.position);
            children.Add(o);
        }

        InitEvent();
    }


    public override void InitEvent()
    {
        base.InitEvent();

        SearchChild("n0").onClick.Set(OnClickClose);
        gComponent.GetChild("n4").onClick.Set(OnClickMake);
        gComponent.GetChild("n15").onClick.Set(OnClickMakeFinish);
        gComponent.GetChild("n14").onClick.Set(OnClickTip);
        //finishDoll.draggable = true;
        finishDoll.onDragStart.Set(OnDragStart);
        finishDoll.onDragEnd.Set(OnDragEnd);
        finishDoll.onClick.Set(OnClickMake);
        gComponent.GetChild("n26").asCom.onDrop.Set(CheckDoll);
        gComponent.GetChild("n10").onClick.Set(OnClickImage);
        gComponent.GetChild("n31").onClick.Set(OnClickImage);
        gComponent.GetChild("n20").onClick.Set(OnClickMask);
       
        btnList.onClickItem.Set(OnClickBtn);
        EventMgr.Ins.RegisterEvent(EventConfig.GAME_MAKEDOLL_EXIT, OnHideAnimation);

    }

    public override void InitData()
    {
        OnShowAnimation();
        base.InitData();
        controller.selectedIndex = 0;
        if (GameData.Player.level > 50)
        {
            config = JsonConfig.GameXinlingConfigs.Find(a => a.type == GameXinlingConfig.TYPE_MAKEDOLLS && a.ckey == "3");
        }
        else if (GameData.Player.level > 20)
        {
            config = JsonConfig.GameXinlingConfigs.Find(a => a.type == GameXinlingConfig.TYPE_MAKEDOLLS && a.ckey == "2");
        }
        else
        {
            config = JsonConfig.GameXinlingConfigs.Find(a => a.type == GameXinlingConfig.TYPE_MAKEDOLLS && a.ckey == "1");
        }
        //config = JsonConfig.GameXinlingConfigs.Find(a => a.type == GameXinlingConfig.TYPE_MAKEDOLLS && a.ckey == "3");
        bgLoader.url = UrlUtil.GetStoryBgUrl(44);
        string[] val = config.cval.Split(new char[1] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
        dollNum = int.Parse(val[0]);
        if (audioClip == null)
            audioClip = Resources.Load(SoundConfig.GetGameAudioUrl(SoundConfig.GameAudioId.FINISH_DOLL)) as AudioClip;

        GetFrgList();


        GetLoader(wishItemLoaders, wishDollCom);
        GetLoader(tipItemLoaders, tipDollCom);
        GetLoader(itemLoaders,actorDoll);
        GetLoader(tipBigItemLoaders, tipBigDollCom);
        GetLoader(finishDollLoaders, finishDollCom);
        GetLoader(finishDollBoxLoaders, finishDollBoxCom);

        dollConfigs.Clear();
        dolls.Clear();
        for (int i = 0; i < dollNum; i++)
        {
            dollConfigs.Add(JsonConfig.GameXinlingZzwwDollConfigs[Random.Range(0, JsonConfig.GameXinlingZzwwDollConfigs.Count)]);
            List<int> doll = new List<int>();
            string[] d = dollConfigs[i].cval.Split(new char[1] { ';' }, System.StringSplitOptions.RemoveEmptyEntries);
            foreach (var s in d)
            {
                doll.Add(int.Parse(s));
            }
            dolls.Add(doll);
        }
        RefrashNextDoll();
        finishCount = 0;
        ShowDollInfo();
        finishDoll.draggable = false;
        finishDoll.GetController("c1").selectedIndex = 0;

        for(int i = 0; i < 3; i++)
        {
            children[i].position = childernPos[i];
        }

        AudioClip bg = ResourceUtil.LoadBackGroundMusic(SoundConfig.CommonMusicId.MakeDolls);
        GRoot.inst.PlayBgSound(bg);
    }

    void GetLoader(List<GLoader> loaders, GComponent gComponent)
    {
        for (int i = 0; i < 5; i++)
        {
            loaders.Add(gComponent.GetChild("n" + (20 + i)).asLoader);
        }
    }
    void RefrashNextDoll()
    {
        GetDoll(wishItemLoaders);
        GetDoll(tipItemLoaders);
        GetDoll(tipBigItemLoaders);
    }
    void GetDoll(List<GLoader> loaders)
    {
        
        for (int i = 0; i < 5; i++)
        {
            loaders[i].url = UrlUtil.GetDollPart(dolls[finishCount][i]);
        }
    }
    private void OnClickMake()
    {
        
        if (changeFinishGW != null)
        {
            changeFinishGW.gameObject.SetActive(false);
        }
        if (controller.selectedIndex == 0)
        {
            controller.selectedIndex = 1;
            btnList.selectedIndex = 0;
            OnClickBtn();
            doll[0] = hair[0].id;
            doll[1] = headwear[0].id;
            doll[2] = necklace[0].id;
            doll[3] = skirt[0].id;
            doll[4] = shoes[0].id;
            MakeDollText();
            if (changeGW != null)
            {
                changeGW.gameObject.SetActive(false);
            }
            for (int i = 0; i < 5; i++)
            {
                string url = "";
                switch (i) {
                    //默认衣服
                    case 0:
                        url = UrlUtil.GetDollPart(51);
                        break;
                    case 3:
                        url = UrlUtil.GetDollPart(52);
                        break;
                }
                itemLoaders[i].url = url;
            }
            return;
        }
        UIMgr.Ins.showErrorMsgWindow("请现把制作完成的娃娃给予客人!");
    }

    private void OnClickBtn()
    {
        switch (btnList.selectedIndex)
        {
            case 0:
                current = hair;
                break;
            case 1:
                current = headwear;
                break;
            case 2:
                current = necklace;
                break;
            case 3:
                current = skirt;
                break;
            case 4:
                current = shoes;
                break;
        }

        fagList.SetVirtual();
        fagList.onClickItem.Set(OnClickFagItem);
        fagList.itemRenderer = ItemRenderer;
        fagList.numItems = current.Count;
    }

    private void ItemRenderer(int index, GObject o)
    {
        GButton btn = o.asButton;
        btn.title ="";
        btn.GetChild("n3").asLoader.url = UrlUtil.GetDollIcon(current[index].id);
    }
    GoWrapper changeGW;
    GoWrapper changeFinishGW;
    private void OnClickFagItem(EventContext context)
    {
        int index = fagList.GetChildIndex((GObject)context.data); 
        //真实index
        int realIndex = fagList.ChildIndexToItemIndex(index);
        ChangeGoWrapper();
        doll[btnList.selectedIndex] = current[realIndex].id;
        MakeDollText();
        
        itemLoaders[btnList.selectedIndex].url = UrlUtil.GetDollPart(current[realIndex].id);
    }


  
    private void GetFrgList()
    {
        hair.Clear();
        headwear.Clear();
        necklace.Clear();
        skirt.Clear();
        shoes.Clear();

        hair = JsonConfig.GameXinlingZzwwFrgConfigs.FindAll(a => a.type == GameXinlingZzwwFrgConfig.TYPE_HAIR);
        headwear = JsonConfig.GameXinlingZzwwFrgConfigs.FindAll(a => a.type == GameXinlingZzwwFrgConfig.TYPE_HEADWEAR);
        necklace = JsonConfig.GameXinlingZzwwFrgConfigs.FindAll(a => a.type == GameXinlingZzwwFrgConfig.TYPE_NECKLACE);
        skirt = JsonConfig.GameXinlingZzwwFrgConfigs.FindAll(a => a.type == GameXinlingZzwwFrgConfig.TYPE_SKIRT);
        shoes = JsonConfig.GameXinlingZzwwFrgConfigs.FindAll(a => a.type == GameXinlingZzwwFrgConfig.TYPE_SHOES);

        ListRandom(hair);
        ListRandom(headwear);
        ListRandom(necklace);
        ListRandom(skirt);
        ListRandom(shoes);
    }
   
    private void OnClickImage()
    {
        controller.selectedIndex = 3;
    }

    private void OnClickMask()
    {
        if (controller.selectedIndex == 3)
        {
            controller.selectedIndex = 1;
        }
    }

    private void OnClickTip()
    {
        for (int i = 0; i < doll.Length; i++)
        {
            doll[i] = dolls[finishCount][i];
        }
        GetDoll(itemLoaders);
        MakeDollText();
        OnClickMakeFinish();

    }

    private void ListRandom(List<GameXinlingZzwwFrgConfig> myList)
    {

        int index = 0;
        GameXinlingZzwwFrgConfig temp = null;
        for (int i = 0; i < myList.Count; i++)
        {
            index = UnityEngine.Random.Range(0, myList.Count - 1);
            if (index != i)
            {
                temp = myList[i];
                myList[i] = myList[index];
                myList[index] = temp;
            }
        }
    }

    private void MakeDollText()
    {
        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < doll.Length; i++)
        {
            builder.Append(doll[i] + "\n");
        }
        //makeDollText.text = builder.ToString();
    }

    private void OnClickMakeFinish()
    {
        
        for (int i = 0; i < itemLoaders.Count; i++)
        {
            if (itemLoaders[i].url == "")
            {
                UIMgr.Ins.showErrorMsgWindow("还有部位没有装扮");
                return;
            }
        }
        for (int i = 0; i < 5; i++)
        {
            switch (i)
            {
                //默认衣服
                case 0:
                    finishDollLoaders[i].url = UrlUtil.GetDollPart(51);
                    break;
                case 3:
                    finishDollLoaders[i].url = UrlUtil.GetDollPart(52);
                    break;
                default:
                    finishDollLoaders[i].visible = false;
                    finishDollLoaders[i].url = itemLoaders[i].url;
                    
                    break;
            }
            finishDollBoxLoaders[i].url = itemLoaders[i].url;
        }
        finishShowCom.GetTransition("t1").Play(1, 0, 0, 0,null);
        GComponent com = finishShowCom.GetChild("n19").asCom.GetChild("n23").asCom;
        com.GetTransition("t0").Play(1, 0, 0, 0, null);
        finishShowCom.visible = true;
        //GetController("c1").selectedIndex = 1;
        controller.selectedIndex = 2;

        

        finishShowCom.GetTransition("t0").Play(()=> {
            GRoot.inst.PlayEffectSound(audioClip);
            FlyToFinishDoll();
        });

    }

    int itemCount = 0;
    int maxCount = 5;
    void FlyToFinishDoll()
    {
        
        if (itemCount < maxCount)
        {
            GLoader gLoader;
            if (flyLoaders.Count< maxCount)
            {
                gLoader = new GLoader();
                gLoader.pivot = finishDollLoaders[itemCount].pivot;
                finishDollCom.AddChild(gLoader);
                flyLoaders.Add(gLoader);
            }
            else
            {
                gLoader = flyLoaders[itemCount];
            }
            gLoader.SetPosition(gLoader.position.x + 101, gLoader.position.y + 962, gLoader.position.z);
            gLoader.url = itemLoaders[itemCount].url;
            gLoader.visible = true ;
            //StartCoroutine(FlyWay(gLoader));
            gLoader.TweenMoveX(itemLoaders[itemCount].position.x - Random.Range(100, 200), 0.4f).SetEase(EaseType.CircOut).OnComplete(() =>
            {

                gLoader.TweenMove(itemLoaders[itemCount].position, 0.3f).SetEase(EaseType.CircIn).OnComplete(() =>
                {

                    gLoader.visible = false;
                   
                    switch (itemCount)
                    {
                        //默认衣服
                        case 0:
                        case 3:
                            finishDollLoaders[itemCount].url = itemLoaders[itemCount].url;
                            break;
                        default:
                            finishDollLoaders[itemCount].visible = true;
                            break;
                    }
                    if (itemCount == maxCount-2)
                    {
                        StartCoroutine(PathGW());
                    }
                    itemCount++;
                    ChangeFinishGoWrapper();
                    FlyToFinishDoll();
                    
                }); ;
            });
            gLoader.TweenMoveY(itemLoaders[itemCount].position.y - Random.Range(40,90), 0.4f).SetEase(EaseType.CircOut);

        }
        else
        {
            itemCount = 0;
          
        }
       
    }
   
    

    void MakeFinish()
    {
        
        controller.selectedIndex = 4;
        finishDoll.draggable = true;
        finishDoll.visible = true;

        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < doll.Length; i++)
        {
            builder.Append(doll[i] + "\n");
        }
        //finishDollText.text = builder.ToString();
    }

    private void ShowDollInfo()
    {

        for (int i = 0; i < children.Count; i++)
        {
            children[i].visible = false;
        }
        for (int i = 0; i < dollNum; i++)
        {
            children[i].visible = true;
        }
        for (int i = 0; i < finishCount; i++)
        {
            children[i].visible = false;
        }
        if (finishCount>0)
        {
            for (int i = finishCount; i < children.Count; i++)
            {
                children[i ].TweenMove(children[i-1].position, 0.5f);
            }
        }
        
        talkController.selectedIndex = 0;
        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < dolls[finishCount].Count; i++)
        {
            builder.Append(dolls[finishCount][i] + "\n");
        }
        //dollText.text = builder.ToString();
        //dollComText.text = builder.ToString();
    }

    private void CheckDoll()
    {
        for (int i = 0; i < dolls[finishCount].Count; i++)
        {
            if (doll[i] != dolls[finishCount][i])
            {
                DollError();
                return;
            }
        }
        finishCount++;

        DollTrue();
    }

    private void DollTrue()
    {
        controller.selectedIndex = 0;
        finishDoll.draggable = true;
        finishDoll.visible = true;
        finishDoll.GetController("c1").selectedIndex = 0;
        OnDrop();
        if (finishCount >= dollNum)
        {
            talkController.selectedIndex = 1;
            GameFinish();
            return;
        }
        StartCoroutine(DollTrueTalk());

    }

    private void DollError()
    {
        controller.selectedIndex = 0;
        finishDoll.draggable = true;
        finishDoll.visible = true;
        finishDoll.GetController("c1").selectedIndex = 0;
        StartCoroutine(DollErrorTalk());
        OnDrop();

    }

    IEnumerator DollErrorTalk()
    {
        talkController.selectedIndex = 2;
        yield return new WaitForSeconds(1);
        talkController.selectedIndex = 0;

    }

    IEnumerator DollTrueTalk()
    {
        talkController.selectedIndex = 1;
        yield return new WaitForSeconds(1);
        talkController.selectedIndex = 0;
        RefrashNextDoll();
        ShowDollInfo();

    }

    void OnDragStart()
    {
        finishDoll.touchable = false;
    }

    void OnDragEnd()
    {

        GObject obj = GRoot.inst.touchTarget;
        finishDoll.touchable = true;

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
        OnDrop();
    }

    private void OnDrop()
    {
        finishDoll.TweenMove(finishDollPos, 1);
    }

    private void GameFinish()
    {

        UIMgr.Ins.showNextPopupView<XinlingFinishView, GameXinlingConfig>(config);

    }

    void ChangeGoWrapper()
    {
        if (changeGW == null)
        {
            GGraph g = gComponent.GetChild("n33").asGraph;
            changeGW = FXMgr.CreateEffectWithGGraph(g, "UI_game23_xingkuosan", new Vector3(1, 1, 1), -1);
        }
        else
        {
            changeGW.gameObject.SetActive(false);
            changeGW.gameObject.SetActive(true);
        }
    }
    void ChangeFinishGoWrapper()
    {
        if (changeFinishGW == null)
        {
            GGraph g = finishShowCom.GetChild("n19").asCom.GetChild("n27").asGraph;
            changeFinishGW = FXMgr.CreateEffectWithGGraph(g, "UI_game23_xingkuosan", new Vector3(1, 1, 1), -1);
        }
        else
        {
            changeFinishGW.gameObject.SetActive(false);
            changeFinishGW.gameObject.SetActive(true);
        }
    }
    GoWrapper pathGW1;
    GoWrapper pathGW2;
    GoWrapper pathGW3;
    IEnumerator PathGW()
    {
        GComponent com = finishShowCom.GetChild("n19").asCom.GetChild("n23").asCom;
        com.GetTransition("t0").Play();
        if (pathGW1 == null&& pathGW2 == null&& pathGW3 == null)
        {
            GGraph g1 = com.GetChild("n12").asGraph;
            GGraph g2 = com.GetChild("n13").asGraph;
            GGraph g3 = com.GetChild("n14").asGraph;
            pathGW1 = FXMgr.CreateEffectWithGGraph(g1, "UI_game23_path", new Vector3(1, 1, 1), -1);
            pathGW2 = FXMgr.CreateEffectWithGGraph(g2, "UI_game23_path", new Vector3(1, 1, 1), -1);
            pathGW3 = FXMgr.CreateEffectWithGGraph(g3, "UI_game23_path2", new Vector3(1, 1, 1), -1);
        }
        pathGW1.gameObject.SetActive(true);
        pathGW2.gameObject.SetActive(false);
        pathGW3.gameObject.SetActive(false);
        
        yield return new WaitForSeconds(0.8f);
        showFinishGW();
        yield return new WaitForSeconds(0.1f);
        pathGW2.gameObject.SetActive(true);
       
        yield return new WaitForSeconds(1.1f);
        pathGW3.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        Transition tran = finishShowCom.GetTransition("t1");
        MakeFinish();
        tran.Play(() => {
            finishDoll.GetController("c1").selectedIndex = 1;
            finishShowCom.visible = false;
        });
        yield return new WaitForSeconds(1f);
        pathGW1.gameObject.SetActive(false);
        pathGW2.gameObject.SetActive(false);
        pathGW3.gameObject.SetActive(false);

       
        showFinishDiGW.gameObject.SetActive(false);
        showFinishDingGW.gameObject.SetActive(false);
    }
    GoWrapper showFinishDiGW;
    GoWrapper showFinishDingGW;
    void showFinishGW()
    {
        if (showFinishDiGW == null && showFinishDingGW == null)
        {
            GGraph g1 = finishShowCom.GetChild("n19").asCom.GetChild("n25").asGraph;
            GGraph g2 = finishShowCom.GetChild("n19").asCom.GetChild("n26").asGraph;
            showFinishDiGW = FXMgr.CreateEffectWithGGraph(g1, "UI_game23_finish_di", new Vector3(1, 1, 1), -1);
            showFinishDiGW.scale = Vector3.one * 162;
            showFinishDiGW.position += new Vector3(-376, -760, 0);
            showFinishDingGW = FXMgr.CreateEffectWithGGraph(g2, "UI_game23_finish_ding", new Vector3(1, 1, 1), -1);
            showFinishDingGW.scale = Vector3.one * 162;
            showFinishDingGW.position += new Vector3(-378, -276, 0);
        }
        else
        {
            showFinishDiGW.gameObject.SetActive(true);
            showFinishDingGW.gameObject.SetActive(true);
        }
    }

    Extrand extrand;
    void OnClickClose()
    {
        if (extrand == null)
        {
            extrand = new Extrand();
            extrand.key = "提示";
            extrand.msg = "任务还未完成，现在退出将不会保存游戏进度；确认退出吗？";
            extrand.callBack = onHide;
        }
        UIMgr.Ins.showNextPopupView<GameTipsView, Extrand>(extrand);
    }

    public override void onHide()
    { 
        TouchScreenView.Ins.PlayChangeEffect(() => {
            UIMgr.Ins.HideView<GameMakeDollsView>();
            AudioClip audioClip = ResourceUtil.LoadBackGroundMusic(SoundConfig.CommonMusicId.BgId);
            GRoot.inst.PlayBgSound(audioClip);
        });
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

public class StoryCombienWatchMoudle : BaseMoudle
{

    readonly int Normal_INDEX = 0;
    readonly int TIPS_INDEX = 1;

    GLoader backGroundLoader;
    GTextField tipText;
    GList gList;
    Controller controller;
    AudioClip putDownAudio;

    //string path = "Game/Bg/Story/";

    List<int> combineIds = new List<int>()
    {
        1,2,3,4,5,6,7
    };
    //glist item数据
    List<int> pools = new List<int>();
    //正确pool数据
    List<int> swapPools = new List<int>();
    List<GGraph> gGraphs = new List<GGraph>();
    List<int> placedIds = new List<int>();
    int rightIndex = 0;

    GamePointConfig gamePointConfig;
    CommonBigTipsInfo commonBigTipsInfo;

    public override void InitMoudle(GComponent gComponent, int controllerIndex)
    {
        base.InitMoudle(gComponent, controllerIndex);
        controller = ui.GetController("c1");
        backGroundLoader = SearchChild("n1").asLoader;
        gList = SearchChild("n6").asList;
        tipText = SearchChild("n4").asTextField;
        ui.GetChild("n36").asCom.onDrop.Add(OuterOnDrop);
        gList.onDrop.Set(ItemOnDrop);
        SearchChild("n44").asCom.onDrop.Set(ItemOnDrop);
        InitEvent();
        //UIConfig.verticalScrollBar = "ui://6f05wxg5et56q4x";
        //UIConfig.defaultScrollBarDisplay = ScrollBarDisplayType.Visible;

    }


    public override void InitData<D>(D data)
    {
        //UIConfig.defaultScrollBarDisplay =  ScrollBarDisplayType.Visible;
        base.InitData(data);
        gamePointConfig = data as GamePointConfig;
        if (gamePointConfig != null)
        {
            commonBigTipsInfo = new CommonBigTipsInfo();
            commonBigTipsInfo.isShowText = false;
            commonBigTipsInfo.url = UrlUtil.GetCombineWatchUrl("game_watch");
            tipText.text = gamePointConfig.content1;

            backGroundLoader.url = UrlUtil.GetStoryBgUrl(gamePointConfig.background_id);
            InitBaseInfo();
            InitList();
        }
        //commonBigTipsInfo = new CommonBigTipsInfo();
        //commonBigTipsInfo.isShowText = false;
        //commonBigTipsInfo.url = path + "game_watch";
    }
    
    public override void InitEvent()
    {
        base.InitEvent();
        //tips
        SearchChild("n2").onClick.Add(() =>
        {
            //UIMgr.Ins.showWindow<CommonBigTipsWindows, CommonBigTipsInfo>(commonBigTipsInfo);
            OnClickTips();
        });
        SearchChild("n0").onClick.Add(() =>
        {
            //Clear();
            EventMgr.Ins.DispachEvent(EventConfig.STORY_BREACK_STORY);
        });
        //查看原图
        SearchChild("n45").onClick.Add(() =>
        {
            UIMgr.Ins.showNextPopupView<GameImageView, string>(UrlUtil.GetLookImageUrl("1"));
        });
        //skip
        SearchChild("n3").onClick.Set(() =>
        {
            NormalInfo normalInfo = new NormalInfo();
            normalInfo.index = gamePointConfig.id;
            normalInfo.type = GameConsumeConfig.STORY_PASS_NODE_TYPE;
            EventMgr.Ins.DispachEvent(EventConfig.STORY_SKIP_NODE, normalInfo);
        });

    }

    void InitBaseInfo()
    {
        rightIndex = 0;
        placedIds.Clear();
        swapPools.Clear();

        pools.Clear();
        Clear();
        gGraphs.Clear();

        GGroup gGroup = ui.GetChild("n37").asGroup;
        int cnt = ui.numChildren;
        for (int i = 0; i < cnt; i++)
        {
            GObject gObject = ui.GetChildAt(i);
            if (gObject.group == gGroup)
            {
                gGraphs.Add(gObject.asGraph);
            }
        }


        swapPools.AddRange(combineIds);
        //放置底图
        PlacePartToWhole(0, 1);
        swapPools.RemoveAt(0);
        //gGraphs.RemoveAt(0);

        pools.AddRange(swapPools);
        MathUtil.Shuffle(pools);
    }


    void InitList()
    {
        Refresh();

        LongPressGesture gesture = new LongPressGesture(gList);
        gesture.once = true;
        gesture.trigger = 0f;
        gesture.onAction.Add(OnLongPress);
    }

    void Refresh()
    {
        gList.itemRenderer = RenderListItem;
        gList.numItems = pools.Count;
    }

    string prefix = "game_watch";
    void RenderListItem(int index, GObject obj)
    {

        GComponent item = obj.asCom;
        GLoader itemLoader = item.GetChild("n0").asLoader;
        //int id = index + 1;
        itemLoader.url = UrlUtil.GetCombineWatchUrl(prefix + pools[index]);
        //Debug.Log(itemLoader.url);
        string extral = index + "," + pools[index];
        itemLoader.data = extral;
        item.onDrop.Add(ItemOnDrop);
        item.data = extral;
        itemLoader.color = Color.white;

    }

    void ItemOnDrop(EventContext context)
    {
        string extral = (string)context.data;
        string[] arry = extral.Split(',');
        int index = int.Parse(arry[0]);
        int id = int.Parse(arry[1]);

        Debug.Log(gList.GetChildAt(index));
        GComponent gComponent = gList.GetChildAt(index).asCom;
        Debug.Log(gComponent.GetChild("n0"));
        GLoader gLoader = gComponent.GetChild("n0").asLoader;
        gLoader.color = Color.white;

    }
    void OuterOnDrop(EventContext context)
    {
        string extral = (string)context.data;
        string[] arry = extral.Split(',');
        int index = int.Parse(arry[0]);
        int id = int.Parse(arry[1]);

        if (putDownAudio == null)
            putDownAudio = Resources.Load(SoundConfig.GAME_AUDIO_EFFECT_URL + (int)SoundConfig.GameAudioId.PutDown) as AudioClip;
        GRoot.inst.PlayEffectSound(putDownAudio);
        PlacePartToWhole(index, id);

        rightIndex++;
        if (rightIndex == swapPools.Count)
        {
            CombineOver();
        }


    }


    void OnLongPress(EventContext context)
    {
        //find out which item is under finger
        //逐层往上知道查到点击了那个item
        GObject obj = GRoot.inst.touchTarget;
        GObject p = obj.parent;
        //Debug.Log("obj: " + obj + "  parent: " + p);
        while (p != null)
        {
            if (p == gList)
                break;

            p = p.parent;
        }

        if (p == null)
            return;

        string extral = (string)obj.data;
        Debug.Log(extral);
        string[] arry = extral.Split(',');
      
        int index = int.Parse(arry[0]);
        Debug.Log(index);
        int id = int.Parse(arry[1]);
        Debug.Log(id);
        //该对象已经放置好了就不需要 处理了
        if (placedIds.Contains(id))
            return;

        DragDropManager.inst.StartDrag(obj, obj.icon + "_1", obj.data);
        GLoader itemLoader = obj.asLoader;
        if (itemLoader != null)
            itemLoader.color = Color.gray;

    }

    List<Image> cacheImages = new List<Image>();
    void PlacePartToWhole(int index, int id)
    {
        Debug.Log("id: " + id);
        //第一章默认图片id为1
        string url = UrlUtil.GetCombineWatchUrl(prefix + "1");
        if (id != 1)
            url = UrlUtil.GetCombineWatchUrl(prefix + pools[index]) + "_1";
        Debug.Log("url: " + url);
        //Texture2D texture2D = Resources.Load<Texture>("Game/Combine/Watch/game_watch1_" + id) as Texture2D;
        Texture2D texture2D = Resources.Load<Texture>(url) as Texture2D;
        NTexture texture = new NTexture(texture2D);

        Image image = new Image();

        image.texture = texture;
        int rightPos = GetOrder(id);
        gGraphs[rightPos].SetNativeObject(image);
        gGraphs[rightPos].sortingOrder = placedIds.Count;


        Debug.Log("index " + index + "  " + rightPos + " order: " + placedIds.Count);
        //image.renderingOrder = placedIds.Count + 1;
        placedIds.Add(id);

        cacheImages.Add(image);
    }

    int GetOrder(int id)
    {
        return combineIds.FindIndex(a => a == id);
    }

    void CombineOver()
    {
        bool isSuccess = true;
        int c_id = 0, p_id = 0;
        while (c_id < combineIds.Count || p_id < placedIds.Count)
        {
            //编号为5的零件不用考虑层级关系，遇到则直接跳过判断。
            if (c_id < combineIds.Count && combineIds[c_id] == 5)
            {
                c_id++;
                continue;
            }
            if (p_id < placedIds.Count && placedIds[p_id] == 5)
            {
                p_id++;
                continue;
            }
            if (c_id < combineIds.Count && p_id < placedIds.Count && combineIds[c_id] != placedIds[p_id])
            {
                UIMgr.Ins.showErrorMsgWindow(MsgException.COMINE_FAIL);
                InitBaseInfo();
                InitList();
                isSuccess = false;
                break;
            }
            c_id++;
            p_id++;
        }
        //for (int index = 0; index < placedIds.Count; index++)
        //{
        //    if (combineIds[index] != placedIds[index])
        //    {
        //        UIMgr.Ins.showErrorMsgWindow(MsgException.COMINE_FAIL);
        //        InitBaseInfo();
        //        InitList();
        //        isSuccess = false;
        //        break;
        //    }
        //}

        if (isSuccess)
        {
            CombineSuccess();
        }
    }



    void CombineSuccess()
    {

        NormalInfo normalInfo = new NormalInfo();
        normalInfo.index = gamePointConfig.point1;
        EventMgr.Ins.DispachEvent(EventConfig.STORY_RECORD_SELECT_NODE, normalInfo);

    }


    void Clear()
    {
        for (int i = 0; i < cacheImages.Count; i++)
        {
            cacheImages[i].Dispose();
            cacheImages[i] = null;
        }
        cacheImages.Clear();

        for (int i = 0; i < gGraphs.Count; i++)
        {
            gGraphs[i].sortingOrder = i;
        }
    }

    void DoTips()
    {
        if (rightIndex < swapPools.Count)
        {
            int c_id = 0, p_id = 0;
            while (p_id < placedIds.Count)
            {
                //编号为5的零件不用考虑层级关系，遇到则直接跳过判断。
                if (c_id < combineIds.Count && combineIds[c_id] == 5)
                {
                    c_id++;
                    continue;
                }
                if (p_id < placedIds.Count && placedIds[p_id] == 5)
                {
                    p_id++;
                    continue;
                }
                if (c_id < combineIds.Count && p_id < placedIds.Count && combineIds[c_id] != placedIds[p_id])
                {
                    ReturnToRight(p_id);
                    rightIndex--;
                    continue;
                }
                c_id++;
                p_id++;
            }

            bool isPut5=false;
            for(int i = 0; i < placedIds.Count; i++)
            {
                if (placedIds[i] == 5)
                {
                    isPut5 = true;
                    break;
                }
            }
            GObject o;
            if (isPut5)
            {
                if (combineIds[rightIndex] >= 5)
                {
                    o = gList.GetChildAt(pools.IndexOf(combineIds[rightIndex + 1]));
                    CombienEffect(pools.IndexOf(combineIds[rightIndex + 1]), combineIds[rightIndex + 1], o);
                }
                else
                {
                     o = gList.GetChildAt(pools.IndexOf(combineIds[rightIndex]));
                    CombienEffect(pools.IndexOf(combineIds[rightIndex]), combineIds[rightIndex], o);
                }
            }
            else
            {
                if (combineIds[rightIndex] >= 5)
                {
                    o = gList.GetChildAt(pools.IndexOf(combineIds[rightIndex]));
                    CombienEffect(pools.IndexOf(combineIds[rightIndex]), combineIds[rightIndex], o);
                }
                else
                {
                    o = gList.GetChildAt(pools.IndexOf(combineIds[rightIndex + 1]));
                    CombienEffect(pools.IndexOf(combineIds[rightIndex + 1]), combineIds[rightIndex + 1], o);
                }
            }
            GLoader itemLoader = o.asCom.GetChild("n0").asLoader;
            if (itemLoader != null)
                itemLoader.color = Color.gray;
            rightIndex++;
            if (rightIndex == swapPools.Count)
            {
                CombineOver();
            }
        }
    }

    void CombienEffect(int index,int id,GObject o)
    {
        GLoader _agent = (GLoader)UIObjectFactory.NewObject(ObjectType.Loader);
        _agent.position = new Vector2(355, 1300);
        _agent.url = UrlUtil.GetCombineWatchUrl(prefix + pools[index]);
        ui.AddChild(_agent);
        _agent.sortingOrder = 1;
        _agent.TweenMove(new Vector2(355,690), 1f).OnComplete(()=> { 
        PlacePartToWhole(index, id);
            _agent.Dispose();
        });
    }

    void ReturnToRight(int id)
    {
        Debug.Log(id);
        //for (int i = cacheImages.Count-1 ; i >= id-1; i--)
        //{
            cacheImages[id].Dispose();
            cacheImages[id] = null;
            cacheImages.RemoveAt(id);
        GObject o = gList.GetChildAt(pools.IndexOf(placedIds[id]));
        GLoader itemLoader= o.asCom.GetChild("n0").asLoader;
        if (itemLoader != null)
            itemLoader.color = Color.white;
        placedIds.RemoveAt(id);
        //}

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
        //    List<GameNodeConsumeConfig> list = JsonConfig.GameNodeConsumeConfigs.FindAll(a => a.node_id == gamePointConfig.id && a.type == gamePointConfig.type);
        //    TinyItem item = ItemUtil.GetTinyItem(list[0].pay);
        //    extrand.item = item;
        //    extrand.msg = "获得提示";
        //    extrand.callBack = DoTips;
        //}
        //UIMgr.Ins.showNextPopupView<PopupDialogView, Extrand>(extrand);
    }
}

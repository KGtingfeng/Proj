using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

[ViewAttr("Game/UI/Y_Game13", "Y_Game13", "Game2")]
public class GamePuzzleBellTowerView : BaseGameView
{
    GLoader bgLoader;
    GComponent sceneCom;
    GComponent subCom;
    GComponent bottomSubCom;
    GGraph frameGgraph;
    ScrollPane scrollPane;
    List<GComponent> suipianList;
    List<GComponent> bottomList = new List<GComponent>();
    Dictionary<int, Vector2> defauldPos = new Dictionary<int, Vector2>();
    Dictionary<int, Vector2> defauldScale = new Dictionary<int, Vector2>();

    AudioClip putDownAudio;
    List<int> putDownList = new List<int>();

    List<GComponent> dropList = new List<GComponent>();

    public override void InitUI()
    {
        base.InitUI();
        sceneCom = SearchChild("n8").asCom;
        subCom = sceneCom.GetChild("n1").asCom;
        bgLoader = sceneCom.GetChild("n11").asCom.GetChild("n13").asLoader;
        controller = sceneCom.GetController("c1");
        frameGgraph = sceneCom.GetChild("n15").asGraph;
        bottomSubCom = sceneCom.GetChild("n9").asCom.GetChild("n18").asCom;
        scrollPane = bottomSubCom.scrollPane;
        InitSuipian();
        InitDrop();
        FXMgr.CreateEffectWithGGraph(frameGgraph, new Vector2(377, 745), "G13_frame");
        InitEvent();
    }

    void InitSuipian()
    {
        suipianList = new List<GComponent>();
        for (int i = 0; i < 21; i++)
        {
            GComponent gCom = subCom.GetChild("n" + (11 + i)).asCom;
            int index = i;
            gCom.visible = false;
            suipianList.Add(gCom);

            GComponent gComponent = bottomSubCom.GetChild("n" + (1 + i)).asCom;
            gComponent.draggable = true;
            gComponent.onClick.Set(() => { Debug.LogError(index); });
            gComponent.GetController("c1").selectedIndex = 1;
            bottomList.Add(gComponent);
            defauldPos.Add(i, gComponent.position);
            defauldScale.Add(i, gComponent.scale);
            gComponent.onDragStart.Set(() => { OnDragStart(index); });
            gComponent.onDragEnd.Set(() => { OnDragEnd(index); });
        }
    }

    void InitDrop()
    {
        for (int i = 0; i < 6; i++)
        {
            GComponent gCom = sceneCom.GetChild("n" + (16 + i)).asCom;
            int index = i;
            dropList.Add(gCom);
            gCom.onDrop.Set((EventContext context) => { ShipOnDrop(index, context); });

        }
    }

    public override void InitEvent()
    {
        base.InitEvent();
        //返回
        SearchChild("n5").onClick.Set(() =>
        {
            EventMgr.Ins.DispachEvent(EventConfig.STORY_BREACK_STORY);
        });
        //skip
        SearchChild("n7").onClick.Set(SkipGame);
        //tips
        SearchChild("n6").onClick.Set(OnClickTips);

        SearchChild("n9").onClick.Set(()=> { 
            UIMgr.Ins.showNextPopupView<GameImageView, string>(UrlUtil.GetLookImageUrl("27"));
        });

        SearchChild("n5").asCom.onDrop.Set(BGOnDrop);
        SearchChild("n6").asCom.onDrop.Set(BGOnDrop);
        SearchChild("n7").asCom.onDrop.Set(BGOnDrop);
        sceneCom.onDrop.Set(BGOnDrop);

        sceneCom.GetChild("n23").onClick.Set(OnComplete);

        EventMgr.Ins.ReplaceEvent(EventConfig.STORY_GAME_QUIT, Destroy<GamePuzzleBellTowerView>);
        EventMgr.Ins.ReplaceEvent(EventConfig.STORY_DELETE_GAME_INFO, DeleteGameInfo);

    }


    public override void InitData<D>(D data)
    {
        base.InitData(data);
        storyGameInfo = data as StoryGameInfo;
        bgLoader.url = UrlUtil.GetGameBGUrl(30);


    }


    public override void InitData()
    {
        base.InitData();
        bgLoader.url = UrlUtil.GetGameBGUrl(30);
    }


    void OnDragStart(int index)
    {

        bottomSubCom.RemoveChild(bottomList[index]);
        sceneCom.AddChild(bottomList[index]);
        bottomList[index].TweenScale(Vector2.one, 0.5f);
        bottomList[index].touchable = false;
        bottomList[index].sortingOrder = 2;
        Vector2 pos = GameTool.MousePosToUI(Input.mousePosition, sceneCom);
        bottomList[index].position = pos;
    }

    void OnDragEnd(int index)
    {
        GObject obj = GRoot.inst.touchTarget;
        bottomList[index].touchable = true;

        while (obj != null)
        {
            if (obj.hasEventListeners("onDrop"))
            {
                obj.RequestFocus();
                obj.DispatchEvent("onDrop", index);
                return;
            }
            obj = obj.parent;
        }

    }


    List<int> drops0 = new List<int>() { 4, 5, 6, 11 };
    List<int> drops1 = new List<int>() { 7, 8, 9, 16, 0 };
    List<int> drops2 = new List<int>() { 10, 12, 17 };
    List<int> drops3 = new List<int>() { 0, 20, 19, 13 };
    List<int> drops4 = new List<int>() { 18, 15, 1 };
    List<int> drops5 = new List<int>() { 2, 3, 14 };
    void ShipOnDrop(int index, EventContext context)
    {
        int dragIndex = int.Parse(context.data.ToString());
        switch (index)
        {
            case 0:
                if (drops0.Contains(dragIndex))
                    DropOnTrue(dragIndex);
                else
                    DropOnError(dragIndex);
                break;
            case 1:
                if (drops1.Contains(dragIndex))
                    DropOnTrue(dragIndex);
                else
                    DropOnError(dragIndex);
                break;
            case 2:
                if (drops2.Contains(dragIndex))
                    DropOnTrue(dragIndex);
                else
                    DropOnError(dragIndex);
                break;
            case 3:
                if (drops3.Contains(dragIndex))
                    DropOnTrue(dragIndex);
                else
                    DropOnError(dragIndex);
                break;
            case 4:
                if (drops4.Contains(dragIndex))
                    DropOnTrue(dragIndex);
                else
                    DropOnError(dragIndex);
                break;

            case 5:
                if (drops5.Contains(dragIndex))
                    DropOnTrue(dragIndex);
                else
                    DropOnError(dragIndex);
                break;

        }

    }

    void BGOnDrop(EventContext context)
    {
        int dragIndex = int.Parse(context.data.ToString());
        DropOnError(dragIndex);
    }

    void DropOnTrue(int index)
    {
        StopCoroutine("ShowTips");
        bottomList[index].visible = false;
        suipianList[index].GetController("c1").selectedIndex = 2;
        suipianList[index].visible = true;
        putDownList.Add(index);
        if (putDownAudio == null)
            putDownAudio = Resources.Load(SoundConfig.GAME_AUDIO_EFFECT_URL + (int)SoundConfig.GameAudioId.PutDown) as AudioClip;
        GRoot.inst.PlayEffectSound(putDownAudio);
        if (putDownList.Count == 21)
        {

            controller.selectedIndex = 1;
            FXMgr.CreateEffectWithScale(sceneCom.GetChild("n18").asCom, new Vector2(-114, -819), "G13_finish", 162, -1);

        }
    }

    void DropOnError(int index)
    {
        subCom.RemoveChild(bottomList[index]);
        bottomSubCom.AddChild(bottomList[index]);
        bottomList[index].sortingOrder = 0;


        Vector2 pos = GameTool.MousePosToUI(Input.mousePosition, bottomSubCom);
        pos.y = -pos.y;
        bottomList[index].position = pos;
        bottomList[index].TweenMove(defauldPos[index], 1f).OnComplete(() =>
        {
            bottomList[index].touchable = true;
        });
        bottomList[index].TweenScale(defauldScale[index], 1f);
    }


    Extrand extrand;
    void OnClickTips()
    {
        ShowTips();
        //if (extrand == null)
        //{
        //    extrand = new Extrand();
        //    extrand.type = 1;
        //    extrand.key = "提示";
        //    List<GameNodeConsumeConfig> list = JsonConfig.GameNodeConsumeConfigs.FindAll(a => a.node_id == storyGameInfo.gamePointConfig.id && a.type == storyGameInfo.gamePointConfig.type);
        //    TinyItem item = ItemUtil.GetTinyItem(list[0].pay);
        //    extrand.item = item;
        //    extrand.msg = "获得提示";
        //    extrand.callBack = RequestGetTips;
        //}
        //UIMgr.Ins.showNextPopupView<PopupDialogView, Extrand>(extrand);
    }

    void RequestGetTips()
    {
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("nodeId", storyGameInfo.gamePointConfig.id);
        wWWForm.AddField("type", storyGameInfo.gamePointConfig.type);
        wWWForm.AddField("index", 0);
        GameMonoBehaviour.Ins.RequestInfoPost<PlayerProperty>(NetHeaderConfig.STROY_STEP_CONSUME, wWWForm, () => { StartCoroutine("ShowTips"); });
    }


    void ShowTips()
    {
        //int index = 0;
        for (int i = 0; i < 21; i++)
        {
            if (!putDownList.Contains(i))
            {
                //index = i;
                bottomSubCom.RemoveChild(bottomList[i]);
                sceneCom.AddChild(bottomList[i]);
                bottomList[i].TweenScale(Vector2.one, 0.5f);
                bottomList[i].touchable = false;
                bottomList[i].sortingOrder = 2;
                DropOnTrue(i);
            }
        }

        //scrollPane.ScrollToView(bottomList[index]);
        //bottomList[index].GetController("c1").selectedIndex = 4;
        //suipianList[index].GetController("c1").selectedIndex = 3;
        //suipianList[index].visible = true;
        //yield return new WaitForSeconds(7f);
        //bottomList[index].GetController("c1").selectedIndex = 1;
        //suipianList[index].visible = false;
        //suipianList[index].GetController("c1").selectedIndex = 2;

    }
}

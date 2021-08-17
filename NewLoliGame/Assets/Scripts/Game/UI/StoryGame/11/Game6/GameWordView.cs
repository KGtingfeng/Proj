using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using System;

[ViewAttr("Game/UI/Y_Game6", "Y_Game6", "Game")]
public class GameWordView : BaseGameView
{
    /// <summary>
    /// 文字位置
    /// </summary>
    readonly List<Vector3> orderPos = new List<Vector3>()
    {
        new Vector3(572,340,0),
        new Vector3(486,385,0),
        new Vector3(481,475,0),
        new Vector3(458,520,0),
        new Vector3(98,609,0),
        new Vector3(567,790,0),
    };
    /// <summary>
    /// 底部位置
    /// </summary>
    readonly List<Vector2> defauldPos = new List<Vector2>()
    {
        new Vector2(141,1062),
        new Vector2(379,1062),
        new Vector2(617,1062),
        new Vector2(141,1202),
        new Vector2(379,1202),
        new Vector2(617,1202),
    };
    GLoader bgLoader;
    GComponent scene;

    List<int> putDownList = new List<int>() { -1, -1, -1, -1, -1, -1 };
    List<GComponent> wordList = new List<GComponent>();
    List<GComponent> btnList = new List<GComponent>();
    List<GComponent> dropList;
    AudioClip putDownAudio;
    AudioClip error;
    public override void InitUI()
    {
        base.InitUI();
        bgLoader = SearchChild("n0").asLoader;
        scene = SearchChild("n4").asCom;
        controller = scene.GetController("c1");

        GetList(scene, 4, btnList, false);
        GetList(scene, 10, wordList, true);
        GetDropList();
        FXMgr.CreateEffectWithGGraph(SearchChild("n5").asGraph, new Vector3(0, 0, 0), "UI_game6_buttonbg", 162);
        InitEvent();
    }

    void GetList(GComponent gCom, int indexr, List<GComponent> gList, bool isWord)
    {
        for (int i = 0; i < 6; i++)
        {
            int index = i;
            GComponent com = gCom.GetChild("n" + (i + indexr)).asCom;
            if (!isWord)
            {
                com.touchable = true;
                com.draggable = true;
                com.onDragStart.Set(() =>
                {
                    OnDragStart(index);
                });
                com.onDragEnd.Set(() =>
                {
                    OnDragEnd(index);
                });
                com.onDrop.Set(BGOnDrop);

            }
            else
            {
                com.visible = false;
            }
            gList.Add(com);

        }
    }

    void GetDropList()
    {
        dropList = new List<GComponent>();
        for (int i = 0; i < 6; i++)
        {
            int index = i;
            GComponent com = scene.GetChild("n" + (i + 21)).asCom;
            com.onDrop.Set((EventContext context) =>
            {
                OnDrop(index, context);
            });
            com.onClick.Set(() => { OnClickWord(index); });
            dropList.Add(com);
        }
    }

    public override void InitEvent()
    {
        base.InitEvent();
        //返回
        SearchChild("n1").onClick.Set(() =>
        {
            EventMgr.Ins.DispachEvent(EventConfig.STORY_BREACK_STORY);
        });
        //skip
        SearchChild("n3").onClick.Set(SkipGame);
        //tips
        SearchChild("n2").onClick.Set(OnClickTips);
        scene.onDrop.Set(BGOnDrop);
        SearchChild("n1").asCom.onDrop.Set(BGOnDrop);
        SearchChild("n2").asCom.onDrop.Set(BGOnDrop);
        SearchChild("n3").asCom.onDrop.Set(BGOnDrop);
        EventMgr.Ins.ReplaceEvent(EventConfig.STORY_GAME_QUIT, Destroy<GameWordView>);
        EventMgr.Ins.ReplaceEvent(EventConfig.STORY_DELETE_GAME_INFO, DeleteGameInfo);
    }

    public override void InitData<D>(D data)
    {
        base.InitData(data);
        storyGameInfo = data as StoryGameInfo;
        bgLoader.url = UrlUtil.GetGameBGUrl(21);


    }

    public override void InitData()
    {
        base.InitData();
        bgLoader.url = UrlUtil.GetGameBGUrl(21);
    }


    void OnDragStart(int index)
    {

        btnList[index].touchable = false;

        Vector2 pos = GameTool.MousePosToUI(Input.mousePosition, ui);
        btnList[index].TweenMove(pos, 0.1f);
    }

    void OnDragEnd(int index)
    {
        GObject obj = GRoot.inst.touchTarget;

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

    void OnDrop(int index, EventContext context)
    {

        int dragIndex = int.Parse(context.data.ToString());
        if (putDownList[index] == -1)
        {
            count++;

            DropOnTrue(dragIndex, index);
        }
        else
        {
            wordList[putDownList[index]].visible = false;
            btnList[putDownList[index]].visible = true;
            DropOnError(putDownList[index]);
            DropOnTrue(dragIndex, index);

        }
    }

    void BGOnDrop(EventContext context)
    {
        int dragIndex = int.Parse(context.data.ToString());

        DropOnError(dragIndex);
    }

    int count = 0;
    void DropOnTrue(int index, int wordIndex)
    {
        wordList[index].xy = orderPos[wordIndex];
        wordList[index].visible = true;
        wordList[index].GetTransition("t0").Play();
        FXMgr.CreateEffectWithScale(wordList[index], new Vector3(67, 20, 0), "UI_game6_tianru");
        btnList[index].visible = false;
        putDownList[wordIndex] = index;
        if (putDownAudio == null)
            putDownAudio = Resources.Load(SoundConfig.GAME_AUDIO_EFFECT_URL + (int)SoundConfig.GameAudioId.PutDown) as AudioClip;
        GRoot.inst.PlayEffectSound(putDownAudio);
        if (count == 6)
        {
            CheckOrder();
        }
    }

    void DropOnError(int index)
    {
        btnList[index].TweenMove(defauldPos[index], 1f).OnComplete(() =>
        {
            btnList[index].touchable = true;
        });
    }

    List<int> order = new List<int>() { 0, 5, 1, 4, 2, 3 };
    /// <summary>
    /// 检查顺序
    /// </summary>
    void CheckOrder()
    {
        for (int i = 0; i < putDownList.Count; i++)
        {
            if (putDownList[i] != order[i])
            {
                OrderError();
                return;
            }
        }
        OrderTrue();
    }

    void OrderTrue()
    {
        controller.selectedIndex = 1;
        SearchChild("n4").onClick.Set(OnComplete);
    }

    void OrderError()
    {
        count = 0;
        UIMgr.Ins.showErrorMsgWindow("填词有误，再试试吧！");
        if (error == null)
            error = Resources.Load(SoundConfig.GAME_AUDIO_EFFECT_URL + (int)SoundConfig.GameAudioId.Error) as AudioClip;
        GRoot.inst.PlayEffectSound(error);
        for (int i = 0; i < 6; i++)
        {
            wordList[i].visible = false;
            btnList[i].visible = true;
            DropOnError(i);
            putDownList[i] = -1;
        }
    }

    void OnClickWord(int index)
    {
        if (putDownList[index] != -1)
        {
            wordList[putDownList[index]].visible = false;
            btnList[putDownList[index]].visible = true;
            DropOnError(putDownList[index]);
            putDownList[index] = -1;
            count--;
        }
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
        //    GameNodeConsumeConfig list = JsonConfig.GameNodeConsumeConfigs.Find(a => a.node_id == storyGameInfo.gamePointConfig.id && a.type == storyGameInfo.gamePointConfig.type);
        //    TinyItem item = ItemUtil.GetTinyItem(list.pay);
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
        GameMonoBehaviour.Ins.RequestInfoPost<PlayerProperty>(NetHeaderConfig.STROY_STEP_CONSUME, wWWForm, GetTip);
    }

    void GetTip()
    {
        for (int i = 0; i < 6; i++)
        {
            if (putDownList[i] != -1)
            {
                if (putDownList[i] != order[i])
                {
                    wordList[putDownList[i]].visible = false;
                    btnList[putDownList[i]].visible = true;
                    DropOnError(putDownList[i]);
                    putDownList[i] = -1;
                    if (putDownList.Contains(order[i]))
                    {
                        int index = putDownList.IndexOf(order[i]);
                        wordList[putDownList[index]].visible = false;
                        btnList[putDownList[index]].visible = true;
                        DropOnError(putDownList[index]);
                        putDownList[index] = -1;
                        count--;
                    }
                    //btnList[order[i]].GetTransition("t0").Play(3, 0, null);
                    //FXMgr.CreateEffectWithScale(dropList[i], new Vector3(67, 54, 0), "UI_game6_tips");
                    count++;
                    btnList[order[i]].touchable = false;
                    DropOnTrue(order[i], i);
                    return;
                }

            }
            else
            {

                if (putDownList.Contains(order[i]))
                {
                    int index = putDownList.IndexOf(order[i]);
                    wordList[putDownList[index]].visible = false;
                    btnList[putDownList[index]].visible = true;
                    DropOnError(putDownList[index]);
                    putDownList[index] = -1;
                    count--;
                }
                //btnList[order[i]].GetTransition("t0").Play(3, 0, null);
                //FXMgr.CreateEffectWithScale(dropList[i], new Vector3(67, 54, 0), "UI_game6_tips");

                count++;
                btnList[order[i]].touchable = false;
                DropOnTrue(order[i], i);
                return;
            }
        }

    }

}

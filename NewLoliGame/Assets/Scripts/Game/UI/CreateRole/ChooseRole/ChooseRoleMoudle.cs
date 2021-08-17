using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

public class ChooseRoleMoudle : BaseMoudle
{
    /// <summary>
    /// 智慧
    /// </summary>
    GTextField wisdomText;
    /// <summary>
    /// 魅力
    /// </summary>
    GTextField charmText;

    GLoader attrLoaderA;
    GLoader attrLoaderB;

    GTextField wisdomNameText;
    GTextField charmNameText;


    GTextField nameText;
    GTextField contextText;

    GLoader topInfoLoader;
    GGraph bodyGraph;

    GList loopList;

    GGraph fxGraph;

    Transition t0;
    Transition t1;
    GGraph changefx;

    List<GameInitCardsConfig> dolls;
    GameInitCardsConfig doll;

    public static readonly Dictionary<int, string> itemBgUrl = new Dictionary<int, string>() {
        {1,"btn_choosedoll_yeluoli"},
        {2,"btn_choosedoll_lankongque"},
        {3,"btn_choosedoll_liangcai"},
        {4,"btn_choosedoll_moli"},
        {5,"btn_choosedoll_feiling"},
        {6,"btn_choosedoll_baiguangying"},
        {7,"btn_choosedoll_heixiangling"}
    };

    public static readonly Dictionary<int, string> itemTextColor = new Dictionary<int, string>() {
        {1,"[color=#C077E2]{0}[/color]"},
        {2,"[color=#1DBD77]{0}[/color]"},
        {3,"[color=#F4AB2D]{0}[/color]"},
        {4,"[color=#F0699C]{0}[/color]"},
        {5,"[color=#F35979]{0}[/color]"},
        {6,"[color=#A2A2F4]{0}[/color]"},
        {7,"[color=#7D7D7D]{0}[/color]"}
    };

    public override void InitMoudle<T>(GComponent gComponent, int controllerIndex, T data)
    {
        base.InitMoudle(gComponent, controllerIndex, data);
        InitUI();
    }

    public override void InitUI()
    {
        dolls = DataUtil.GetBindDolls();

        GLoader gLoader = SearchChild("n23").asLoader;
        gLoader.url = UrlUtil.GetChooseDollBgUrl("BG_choosedoll_yeluoli");

        topInfoLoader = SearchChild("n22").asLoader;
        bodyGraph = SearchChild("n3").asGraph;

        attrLoaderA = SearchChild("n7").asLoader;
        attrLoaderB = SearchChild("n8").asLoader;

        wisdomNameText = SearchChild("n9").asTextField;
        charmNameText = SearchChild("n10").asTextField;

        t0 = ui.GetTransition("t0");
        t1 = ui.GetTransition("t2");
        changefx = SearchChild("n31").asGraph;
        changefx.visible = false;

        nameText = SearchChild("n28").asTextField;
        contextText = SearchChild("n29").asTextField;

        wisdomText = SearchChild("n11").asTextField;
        charmText = SearchChild("n12").asTextField;

        loopList = SearchChild("n20").asList;
        fxGraph = SearchChild("n27").asGraph;
        InitEvent();
    }

    public override void InitData<D>(D data)
    {
        base.InitData(data);
        loopList.itemRenderer = RenderListItem;
        loopList.numItems = dolls.Count;
        loopList.selectedIndex = 0;
        doll = dolls[0];

        FXMgr.CreateEffectWithGGraph(fxGraph, new Vector3(-215, -30, 0), "UI_xuanzewawa", 162);
        AudioClip audioClip = ResourceUtil.LoadBackGroundMusic(SoundConfig.CommonMusicId.ChooseRole);
        GRoot.inst.PlayBgSound(audioClip);
        RefreshInfo();
        t0.Play();
    }


    public override void InitEvent()
    {
        //点击契约
        SearchChild("n24").onClick.Set(CreatePlayerRole);
        //故事
        SearchChild("n6").onClick.Set(() =>
        {
            UIMgr.Ins.showNextPopupView<DollStoryView, GameInitCardsConfig>(doll);
        });

    }

    void CreatePlayerRole()
    {
        //WWWForm wWWForm = new WWWForm();
        //wWWForm.AddField("initCard", doll.card_id);
        //GameMonoBehaviour.Ins.RequestInfoPost<Player>(NetHeaderConfig.CONTRACT, wWWForm, CreatePlayerRoleSuccess);
        //UIMgr.Ins.showNextView<CreateRoleView, GameInitCardsConfig>(doll);
        t1.Play(() =>
        {
            AudioClip audioClip = ResourceUtil.LoadBackGroundMusic(SoundConfig.CommonMusicId.BgId);
            GRoot.inst.PlayBgSound(audioClip);
            GameData.guiderCurrent = GameData.guiderCurrent.Next;
            baseView.GoToMoudle<XinlingTalkMoudle, GameInitCardsConfig>((int)ChooseRoleView.MoudleType.Talk, doll);
        });
        changefx.visible = true;
        fxGraph.visible = false;
    }

    void CreatePlayerRoleSuccess()
    {
        baseView.GoToMoudle<GetRoleMoudle, GameInitCardsConfig>((int)ChooseRoleView.MoudleType.GetDoll, doll);

    }


    void RenderListItem(int index, GObject obj)
    {
        GButton item = (GButton)obj;
        string tmpName = dolls[index].name_cn;
        int length = tmpName.Length;
        string formatName = "";
        for (int i = 0; i < length; i++)
        {
            formatName += tmpName[i];
            if (i < length - 1)
                formatName += "\n";
        }
        int cardId = dolls[index].card_id;
        item.titleFontSize = 40;
        item.titleColor = Color.white;
        item.title = string.Format(itemTextColor[cardId], formatName);
        //item.title = formatName;//dolls[index].name_cn;

        if (itemBgUrl.ContainsKey(cardId))
        {
            GLoader loader = item.GetChild("n0").asLoader;
            loader.url = "ui://X_Choose doll/" + itemBgUrl[cardId];
        }

        item.onClick.Set(() =>
        {
            loopList.selectedIndex = index;
            doll = dolls[index];
            RefreshInfo();
        });


    }

    void RefreshInfo()
    {
        //gameInitCardsConfig = DataUtil.GetGameInitCard(doll.card_id);
        //if (gameInitCardsConfig == null)
        //return;
        List<TinyItem> tinyItems = ItemUtil.GetTinyItemForDollConfig(doll);
        if (tinyItems.Count >= 2)
        {
            attrLoaderA.url = tinyItems[0].url;
            wisdomNameText.text = tinyItems[0].name;
            wisdomText.text = "+" + tinyItems[0].num;

            attrLoaderB.url = tinyItems[1].url;
            charmNameText.text = tinyItems[1].name;
            charmText.text = "+" + tinyItems[1].num;
        }
        if (ChooseRoleView.topUrl.ContainsKey(doll.card_id))
        {
            nameText.text = doll.name_cn;
            contextText.text = ChooseRoleView.topText[doll.card_id];
        }
        //topInfoLoader.url = "ui://X_Choose doll/" + ChooseRoleView.topUrl[doll.card_id];
        SetRoleSpine();
    }

    GoWrapper goWrapper;
    void SetRoleSpine()
    {
        GameObject go = FXMgr.CreateRoleSpine(doll.card_id+27, 0);
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
        bodyGraph.SetNativeObject(goWrapper);
        bodyGraph.position = new Vector2(790, 1734);
        goWrapper.scale = Vector2.one * 108;

    }

    void RequestCreateRole()
    {

    }

}

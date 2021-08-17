using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

[ViewAttr("Game/UI/J_Story", "J_Story", "Frame_storyreview")]
public class StoryPlayRecordView : BaseView
{
    /// <summary>
    /// 角色item
    /// </summary>
    string item_role = "ui://6f05wxg5dd7oq26";
    /// <summary>
    /// 旁白item
    /// </summary>
    string item_narraor = "ui://6f05wxg5dd7oq25";
    /// <summary>
    /// 玩家自己
    /// </summary>
    string item_self = "ui://6f05wxg5dd7oq27";
    //string path = "Game/Bg/Story/";


    GLoader backgroundLoader;
    GLoader childBgLoader;
    GList loopList;
    int itemCount = 0;

    GComponent gComponent;

    int backGoundId = -2;
    public override void InitUI()
    {
        //this.displayObject.gameObject.transform.localPosition = Vector3.zero;

        backgroundLoader = SearchChild("n1").asLoader;
        gComponent = SearchChild("n4").asCom;
        loopList = gComponent.GetChild("n3").asList;
        childBgLoader = gComponent.GetChild("n4").asLoader;


        childBgLoader.url = UrlUtil.GetStoryBgUrl(backGoundId);
        InitEvent();
    }

    NormalInfo normalInfo;
    List<int> nodes = new List<int>();

    public override void InitData<D>(D data)
    {

        base.InitData(data);
        nodes.Clear();
        normalInfo = data as NormalInfo;

        if (normalInfo != null)
        {
            nodes.AddRange(StoryDataMgr.ins.playerChapterInfo.GetPointsQueue);
            if (!StoryDataMgr.ins.StoryInfo.isReRead)
                nodes.Add(normalInfo.index);


            loopList.RemoveCacheItem();
            InitBaseInfo();
            //PrintDialog();

            //bigCommonBg
            GamePointConfig gamePointConfig = DataUtil.GetPointConfig(normalInfo.index);
            if (gamePointConfig != null)
                backgroundLoader.url = UrlUtil.GetStoryBgUrl(gamePointConfig.background_id);
        }

    }




    List<StoryRecordInfo> storyRecords = new List<StoryRecordInfo>();
    void InitBaseInfo()
    {
        storyRecords.Clear();
        GameChapterConfig gameChapterConfig = DataUtil.GetChapter(StoryDataMgr.ins.playerChapterInfo.actor_id, StoryDataMgr.ins.playerChapterInfo.chapter_id);
        if (gameChapterConfig == null)
            return;
        List<int> gamepointNodes = DataUtil.GetChapterPassNodesForDialog(gameChapterConfig.startPoint, nodes);
        itemCount = gamepointNodes.Count;
        //storyRecords = new List<StoryRecordInfo>();
        foreach (var nodeId in gamepointNodes)
        {
            GamePointConfig gamePointConfig = DataUtil.GetPointConfig(nodeId);
            //Debug.Log(gamePointConfig.type + "  _   " + gamePointConfig.content1);
            StoryRecordInfo storyRecordInfo = new StoryRecordInfo();
            storyRecordInfo.type = gamePointConfig.type;
            storyRecordInfo.context = gamePointConfig.content1;
            storyRecordInfo.name = gamePointConfig.title;
            switch (gamePointConfig.type)
            {

                case (int)TypeConfig.StoyType.TYPE_ROLE:
                    storyRecordInfo.itemUrl = item_role;
                    break;
                case (int)TypeConfig.StoyType.TYPE_SELF:
                    storyRecordInfo.itemUrl = item_self;
                    break;
                case (int)TypeConfig.StoyType.TYPE_ASIDE:
                case (int)TypeConfig.StoyType.TYPE_TRANSITION:
                    storyRecordInfo.itemUrl = item_narraor;
                    break;
            }
            storyRecords.Add(storyRecordInfo);

        }

        for (int i = 0; i < storyRecords.Count; i++)
        {
            GObject gObject = loopList.AddItemFromPool(storyRecords[i].itemUrl);
            GComponent item = gObject.asCom;
            switch (storyRecords[i].type)
            {

                case (int)TypeConfig.StoyType.TYPE_ROLE:
                    GTextField rNameText = item.GetChild("n1").asTextField;
                    GTextField rContenText = item.GetChild("n0").asTextField;
                    rNameText.text = storyRecords[i].name;
                    rContenText.text = DataUtil.ReplaceCharacterWithStarts(storyRecords[i].context);

                    //DoTypingEffect(rNameText);
                    //DoTypingEffect(rContenText);


                    break;
                case (int)TypeConfig.StoyType.TYPE_SELF:
                    GTextField sNameText = item.GetChild("n1").asTextField;
                    GTextField sContenText = item.GetChild("n0").asTextField;
                    sNameText.text = storyRecords[i].name;
                    sContenText.text = DataUtil.ReplaceCharacterWithStarts(storyRecords[i].context);
                    //DoTypingEffect(sNameText);
                    //DoTypingEffect(sContenText);

                    break;
                case (int)TypeConfig.StoyType.TYPE_ASIDE:
                case (int)TypeConfig.StoyType.TYPE_TRANSITION:
                    GTextField nContenText = item.GetChild("n0").asTextField;
                    nContenText.text = DataUtil.ReplaceCharacterWithStarts(storyRecords[i].context);
                    //DoTypingEffect(nContenText);

                    break;
            }
        }
        if (storyRecords.Count > 0)
            loopList.ScrollToView(0);
        //PrintTex(); 

    }

    public List<TypingEffect> effects = new List<TypingEffect>();
    void AddSignleItemToScrollView(int index)
    {
        effects.Clear();
        GObject gObject = loopList.AddItemFromPool(storyRecords[index].itemUrl);
        GComponent item = gObject.asCom;
        switch (storyRecords[index].type)
        {

            case (int)TypeConfig.StoyType.TYPE_ROLE:
                GTextField rNameText = item.GetChild("n1").asTextField;
                GTextField rContenText = item.GetChild("n0").asTextField;
                rNameText.text = storyRecords[index].name;
                rContenText.text = DataUtil.ReplaceCharacterWithStarts(storyRecords[index].context);

                DoTypingEffect(rNameText);
                DoTypingEffect(rContenText);


                break;
            case (int)TypeConfig.StoyType.TYPE_SELF:
                GTextField sNameText = item.GetChild("n1").asTextField;
                GTextField sContenText = item.GetChild("n0").asTextField;


                sNameText.text = storyRecords[index].name;
                sContenText.text = DataUtil.ReplaceCharacterWithStarts(storyRecords[index].context);
                DoTypingEffect(sNameText);
                DoTypingEffect(sContenText);

                break;
            case (int)TypeConfig.StoyType.TYPE_ASIDE:
            case (int)TypeConfig.StoyType.TYPE_TRANSITION:
                GTextField nContenText = item.GetChild("n0").asTextField;
                nContenText.text = DataUtil.ReplaceCharacterWithStarts(storyRecords[index].context);
                DoTypingEffect(nContenText);

                break;
        }
    }





    private void DoTypingEffect(GTextField textField)
    {
        TypingEffect typingEffect = new TypingEffect(textField);
        typingEffect._textField.visible = false;

        //typingEffect.Start();
        //typingEffects.Add(typingEffect);
        effects.Add(typingEffect);

    }

    public override void InitEvent()
    {
        base.InitEvent();
        //atuo
        gComponent.GetChild("n5").onClick.Set(BackToStory);

        //back
        SearchChild("n2").onClick.Set(BackToStory);


    }

    private void BackToStory()
    {
        gameObject.SetActive(false);
        ui.visible = false;
    }

    //public override void PrintTex()
    //{
    //    base.PrintTex();
    //    MoveItemToMiddle();


    //}



}

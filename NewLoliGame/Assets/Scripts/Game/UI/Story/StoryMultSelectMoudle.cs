using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using System;

public class StoryMultSelectMoudle : BaseMoudle
{

    static StoryMultSelectMoudle ins;
    public static StoryMultSelectMoudle Ins
    {
        get
        {
            return ins;
        }
    }

    /// <summary>
    /// 剧情选择
    /// </summary>
    readonly int PLOT_SELECT = 0;
    /// <summary>
    /// 属性选择
    /// </summary>
    //readonly int ATTRIBUTE_SELECT = 1;
    GLoader backGroundLoader;
    GamePointConfig gamePointConfig;
    Controller controller;

    GComponent attrComponment;
    GTextField charmText;
    GTextField intellText;
    GTextField envText;
    GTextField magicText;
    GTextField skipText;
    GLoader skipIcon;
    GButton skipBtn;

    GTextField titleText;

    GObject answer;

    GComponent attrCom;

    GObject attrBg;
    //string path = "Game/Bg/Story/";
    GList _list;


    bool canClick;
    List<string> defaultItemUrl = new List<string>()
    {
        "ui://6f05wxg5g6pvz",
        "ui://6f05wxg5dd7oq20",

    };


    CallBackList callBackList;

    public override void InitMoudle(GComponent gComponent, int controllerIndex)
    {
        base.InitMoudle(gComponent, controllerIndex);
        controller = ui.GetController("c1");
        _list = SearchChild("n10").asList;

        backGroundLoader = SearchChild("n14").asLoader;
        titleText = SearchChild("n1").asTextField;
        attrComponment = SearchChild("n26").asCom;
        charmText = attrComponment.GetChild("n44").asTextField;
        intellText = attrComponment.GetChild("n46").asTextField;
        envText = attrComponment.GetChild("n45").asTextField;
        magicText = attrComponment.GetChild("n47").asTextField;

        //skip
        GComponent skipCom = SearchChild("n28").asCom;
        skipText = skipCom.GetChild("n24").asTextField;
        skipIcon = skipCom.GetChild("n23").asLoader;
        skipBtn = skipCom.GetChild("n25").asButton;

        attrCom = SearchChild("n26").asCom;
        attrBg = SearchChild("n21");
        answer = SearchChild("n0");


        //answer.alpha = 0;
        skipCom.alpha = 0;
        attrCom.alpha = 0;
        attrBg.alpha = 0;

        //answer.displayObject.gameObject.AddComponent<UITweenFade>().SetTweenFade(0);
        _list.displayObject.gameObject.AddComponent<UITweenMove>().SetTweenMove(new Vector2(_list.x + 1500, _list.y));
        skipCom.displayObject.gameObject.AddComponent<UITweenFade>().SetTweenFade(1f);
        attrCom.displayObject.gameObject.AddComponent<UITweenFade>().SetTweenFade(1f);
        attrBg.displayObject.gameObject.AddComponent<UITweenFade>().SetTweenFade(1f);

        callBackList = new CallBackList();
        callBackList.callBack1 = ChangeBg;
        callBackList.callBack2 = ShowDialogDetailInfo;

        InitEvent();
        ins = this;
    }

    List<StoryConiditionItem> storyConiditionItems;
    public override void InitData<D>(D data)
    {

        gamePointConfig = data as GamePointConfig;
        if (gamePointConfig != null)
        {
            storyConiditionItems = gamePointConfig.storyConiditionItems;
            titleText.text = "";
            answer.visible = false;
            CompareBackGroundId(gamePointConfig.background_id);

            gameConsumeConfig = DataUtil.GetConsumeConfig(GameConsumeConfig.STORY_SKIP_NODE_TYPE);
            //tmp
            if (gamePointConfig != null)
            {
                TinyItem tinyItem = ItemUtil.GetTinyItem(gameConsumeConfig.pay);
                skipText.text = tinyItem.num + ""; ;
                skipIcon.url = CommonUrlConfig.GetConsumeItemUrl((TypeConfig.Consume)tinyItem.type); ;
            }
            SearchChild("n13").visible = true;
            canClick = true;
            if (GameData.isGuider)
            {
                if (gamePointConfig.id == 510)
                {
                    GameData.guiderCurrent = GuiderInfoLinked.GetCurrGuiderInfo(1, 3);
                    UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);
                }
                else if (gamePointConfig.id == 540)
                {
                    GameData.guiderCurrent = GuiderInfoLinked.GetCurrGuiderInfo(1, 4);
                    UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);
                }
                else if (NewbieGuideView.ins != null)
                {
                    NewbieGuideView.ins.onHide();
                }
                SearchChild("n13").visible = false;
            }
        }
    }

    void CompareBackGroundId(int bgId)
    {

        if (StoryDataMgr.ins.lastBgId != bgId && StoryDataMgr.ins.lastBgId > 0)
        {
            StoryDataMgr.ins.lastBgId = bgId;
            EventMgr.Ins.DispachEvent(EventConfig.STORY_ANIMATION_VIEW, callBackList);
            _list.visible = false;
        }
        else
        {
            StoryDataMgr.ins.lastBgId = bgId;
            ChangeBg();
            ShowDialogDetailInfo();

        }
    }


    void ChangeBg()
    {
        backGroundLoader.url = UrlUtil.GetStoryBgUrl(StoryDataMgr.ins.lastBgId);
    }

    void ShowDialogDetailInfo()
    {
        int selectIndex = 0;
        defaultSecond = 0;
        if (gamePointConfig.type == (int)TypeConfig.StoyType.TYPE_ATTRIBUTE)
        {
            selectIndex = 1;
            InitBaseInfo();
        }
        if (controller.selectedIndex != selectIndex)
            controller.selectedIndex = selectIndex;
        haveSpicItem = false;
        InitList();
    }
    bool haveSpicItem;
    #region 现在将好感度过关，剧情过关融合
    void InitList()
    {
        titleText.text = DataUtil.ReplaceCharacterWithStarts(gamePointConfig.title);
        answer.visible = true;

        _list.visible = true;
        _list.RemoveCacheItem();
        int size = storyConiditionItems.Count;
        for (int i = 0; i < size; i++)
        {
            string condition = storyConiditionItems[i].condition;
            string[] conditionArr = condition.Split(',');
            if (conditionArr.Length >= 3)
            {
                haveSpicItem = true;
                break;
            }
        }
        for (int i = 0; i < size; i++)
        {
            RenderListItem(i);
        }
    }


    void RenderListItem(int index)
    {
        defaultSecond = 0;
        StoryConiditionItem storyConditionItem = storyConiditionItems[index];
        string condition = storyConditionItem.condition;
        string[] conditionArr = condition.Split(',');

        if (conditionArr.Length != 2)
        {
            if (conditionArr.Length < 3)//剧情过关界面
            {
                //defaultItemUrl[0];这边的索引0是剧情过关默认图标
                //按钮组件在FGUI中J_Story/Button_dialog_choose
                GComponent item = _list.AddItemFromPool(defaultItemUrl[0]).asCom;
                //float targetX = item.position.x;
                //item.x = Screen.width;
                GButton gButton = item.asButton;
                item.alpha = 1;
                gButton.title = storyConiditionItems[index].content;
                gButton.GetChild("title").asTextField.align = haveSpicItem ? AlignType.Center : AlignType.Left;
                gButton.onClick.Set(() =>
                {
                    if (IsLegalClick())
                    {
                        SelectEffect(index, () =>
                       {
                           OnClckSelectBtn(index);
                       });
                    }

                });

                //item.TweenMoveX(targetX, 0.5f);
            }
            else//condition三个数值，好感度过关的情况
            {
                GComponent item = _list.AddItemFromPool(defaultItemUrl[1]).asCom;
                item.alpha = 1;

                GButton gButton = item.asButton;
                gButton.title = storyConiditionItems[index].content;
                GLoader gLoader = gButton.GetChild("n4").asLoader;
                GTextField gTextField = gButton.GetChild("n5").asTextField;
                GTextField needDescText = gButton.GetChild("n6").asTextField;
                TinyItem tinyItem = ItemUtil.GetTinyItem(storyConiditionItems[index].condition);
                gLoader.url = UrlUtil.GetItemIconUrl(102);
                gTextField.text = tinyItem.num + "开启";
                needDescText.text = "需要" + tinyItem.name;
                gButton.onClick.Set(() =>
                {
                    if (IsLegalClick())
                        OnClickSelectFriendlyBtn(tinyItem, storyConiditionItems[index], index);


                });
            }
        }
        else//属性过关界面
        {
            AttributeRenderItems(index);
        }
    }

    void AttributeRenderItems(int index)//渲染属性过关按钮
    {
        //defaultItemUrl[1];这边的索引1是属性过关默认图标
        GComponent item = _list.AddItemFromPool(defaultItemUrl[1]).asCom;
        item.alpha = 1;

        GButton gButton = item.asButton;
        gButton.title = storyConiditionItems[index].content;
        GLoader gLoader = gButton.GetChild("n4").asLoader;
        GTextField gTextField = gButton.GetChild("n5").asTextField;
        GTextField needDescText = gButton.GetChild("n6").asTextField;
        TinyItem tinyItem = ItemUtil.GetTinyItemTwoForAttribute(storyConiditionItems[index].condition);
        gLoader.url = tinyItem.url;
        gTextField.text = tinyItem.num + "";
        needDescText.text = "需要" + tinyItem.name;

        gButton.onClick.Set(() =>
        {
            if (IsLegalClick())
                OnClickSelectAttributeBtn(tinyItem, storyConiditionItems[index], index);


        });
    }

    int defaultSecond;
    bool IsLegalClick(int duration = 2)
    {

        int nowMillsecon = DateTime.Now.Second;
        if (Math.Abs(nowMillsecon - defaultSecond) >= duration)
        {
            defaultSecond = DateTime.Now.Second;
            return true;
        }
        return false;
    }

    #endregion


    #region 原本属性过关的逻辑
    //void InitList()
    //{
    //    _list.RemoveCacheItem();
    //    int size = storyConiditionItems.Count;
    //    if (storyConiditionItems != null && size > 0)
    //    {
    //        if (controller.selectedIndex == 0)
    //            _list.defaultItem = defaultItemUrl[0];
    //        else
    //            _list.defaultItem = defaultItemUrl[1];


    //        _list.itemRenderer = RenderListItem;
    //        _list.numItems = size;
    //    }


    //}


    //void RenderListItem(int index, GObject obj)
    //{
    //    GButton gButton = obj.asButton;
    //    gButton.title = storyConiditionItems[index].content;

    //    if (controller.selectedIndex == PLOT_SELECT)
    //    {
    //        gButton.onClick.Set(() =>
    //        {
    //            OnClckSelectBtn(index);
    //        });
    //    }
    //    else
    //    {
    //        GLoader gLoader = gButton.GetChild("n4").asLoader;
    //        GTextField gTextField = gButton.GetChild("n5").asTextField;
    //        GTextField needDescText = gButton.GetChild("n6").asTextField;
    //        TinyItem tinyItem = ItemUtil.GetTinyItemTwoForAttribute(storyConiditionItems[index].condition);
    //        gLoader.url = tinyItem.url;
    //        gTextField.text = tinyItem.num + "";
    //        needDescText.text = "需要" + tinyItem.name;

    //        gButton.onClick.Set(() =>
    //        {
    //            OnClickSelectAttributeBtn(tinyItem, storyConiditionItems[index]);
    //        });

    //    }

    //}
    #endregion


    GameConsumeConfig gameConsumeConfig;
    void InitBaseInfo()
    {

        charmText.text = GameData.Player.attribute.charm + "";
        intellText.text = GameData.Player.attribute.intell + "";
        envText.text = GameData.Player.attribute.evn + "";
        magicText.text = GameData.Player.attribute.mana + "";

        //for (int i = 0; i < storyConiditionItems.Count; i++)
        //{
        //    TinyItem tinyItem = ItemUtil.GetTinyItemTwoForAttribute(storyConiditionItems[i].condition);
        //    if (tinyItem != null&&!DataUtil.GetEnoughAttributeResult(tinyItem.type, tinyItem.num))
        //        return 1;
        //}
        //return 0;

    }

    void SelectEffect(int index, Action callback)
    {
        if (!canClick)
            return;
        canClick = false;
        List<GObject> children = _list._children;
        for (int i = 0; i < children.Count; i++)
        {
            if (index == i)
            {
                children[index].TweenScale(Vector2.one * 1.15f, 0.5f).OnComplete(() =>
                {
                    children[index].TweenScale(Vector2.one, 0.5f).OnComplete(() =>
                    {
                        callback();

                    });
                });
            }
            else
            {
                children[i].TweenFade(0, 1f);
            }
        }

    }

    void OnClckSelectBtn(int index)
    {
        int point_id = 0;
        switch (index)
        {
            case 0:
                point_id = gamePointConfig.point1;
                break;
            case 1:
                point_id = gamePointConfig.point2;
                break;
            case 2:
                point_id = gamePointConfig.point3;
                break;
            default: break;
        }

        NormalInfo normalInfo = new NormalInfo();
        normalInfo.index = point_id;
        EventMgr.Ins.DispachEvent(EventConfig.STORY_RECORD_SELECT_NODE, normalInfo);

        //Debug.LogError("<color=#13FF00>刷新节点： OnClckSelectBtn</color>");
    }

    void OnClickSelectFriendlyBtn(TinyItem tinyItem, StoryConiditionItem storyConiditionItem, int index)
    {
        int favor = int.Parse(StoryDataMgr.ins.QueryStoryInfo(tinyItem.id).extra.favour);//角色好感度
        if (favor > tinyItem.num)
        {


            SelectEffect(index, () =>
            {
                if (StoryDataMgr.ins.roleStoryInfo.node_id < StoryDataMgr.ins.StoryInfo.node_id)
                {
                    StoryView.view.RequestNodePassResult(gamePointConfig, () =>
                    {
                        TriggerNextNode(storyConiditionItem);
                    });
                }
                else
                {
                    TriggerNextNode(storyConiditionItem);
                }
            });
        }
        else
        {
            Debug.Log("好感度不足");
            StoryAttributeWindowInfo storyAttributeWindowInfo = new StoryAttributeWindowInfo();
            storyAttributeWindowInfo.storyInfo = new StoryInfo();
            storyAttributeWindowInfo.storyInfo.node_id = storyConiditionItem.point;
            storyAttributeWindowInfo.type = StoryAttributeWindowInfo.FAVOUR_TYPE;
            storyAttributeWindowInfo.actorName = JsonConfig.GameInitCardsConfigs.Find(a => a.card_id == tinyItem.id).name_cn;
            GameConsumeConfig attrConsume = DataUtil.GetConsumeConfig(GameConsumeConfig.STORY_PASS_NODE_TYPE);
            if (attrConsume != null)
            {
                storyAttributeWindowInfo.tinyItem = ItemUtil.GetTinyItem(attrConsume.pay);
                EventMgr.Ins.DispachEvent(EventConfig.STORY_GOTO_ATTRIBUTE, storyAttributeWindowInfo);
            }
        }
    }


    void OnClickSelectAttributeBtn(TinyItem tinyItem, StoryConiditionItem storyConiditionItem, int index)
    {
        //正式版
        if (DataUtil.GetEnoughAttributeResult(tinyItem.type, tinyItem.num))
        //单机版
        //if (false)
        {
            //单机版
            //TriggerNextNode(storyConiditionItem);


            SelectEffect(index, () =>
            {
                //正式版
                if (StoryDataMgr.ins.roleStoryInfo.node_id < StoryDataMgr.ins.StoryInfo.node_id)
                {
                    StoryView.view.RequestNodePassResult(gamePointConfig, () =>
                    {
                        TriggerNextNode(storyConiditionItem);
                    });
                }
                else
                {
                    TriggerNextNode(storyConiditionItem);
                }
            });

        }
        else
        {

            StoryAttributeWindowInfo storyAttributeWindowInfo = new StoryAttributeWindowInfo();
            storyAttributeWindowInfo.storyInfo = new StoryInfo();
            storyAttributeWindowInfo.storyInfo.node_id = storyConiditionItem.point;
            storyAttributeWindowInfo.type = StoryAttributeWindowInfo.ATTRIBUTE_TYPE;

            GameConsumeConfig attrConsume = DataUtil.GetConsumeConfig(GameConsumeConfig.STORY_PASS_NODE_TYPE);
            if (attrConsume != null)
            {
                storyAttributeWindowInfo.tinyItem = ItemUtil.GetTinyItem(attrConsume.pay);
                EventMgr.Ins.DispachEvent(EventConfig.STORY_GOTO_ATTRIBUTE, storyAttributeWindowInfo);
            }

        }
    }
    void TriggerNextNode(StoryConiditionItem storyConiditionItem)
    {
        NormalInfo normalInfo = new NormalInfo();
        normalInfo.index = storyConiditionItem.point;
        EventMgr.Ins.DispachEvent(EventConfig.STORY_RECORD_SELECT_NODE, normalInfo);

    }

    public override void InitEvent()
    {
        base.InitEvent();
        SearchChild("n13").onClick.Set(() =>
        {
            if (GameData.isGuider)
            {

            }
            else
            {
                EventMgr.Ins.DispachEvent(EventConfig.STORY_BREACK_STORY);

            }
        });

        ////回顾剧情
        //SearchChild("n12").onClick.Set(
        //() =>
        //{



        //});

        //跳过
        skipBtn.onClick.Set(() =>
        {
            if (gamePointConfig != null)
            {
                NormalInfo normalInfo = new NormalInfo();
                normalInfo.index = gamePointConfig.id;
                normalInfo.type = GameConsumeConfig.STORY_PASS_NODE_TYPE;
                EventMgr.Ins.DispachEvent(EventConfig.STORY_SKIP_NODE, normalInfo);
            }


        });


    }






}

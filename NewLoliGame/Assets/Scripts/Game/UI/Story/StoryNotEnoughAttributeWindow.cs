using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

[ViewAttr("Game/UI/J_Story", "J_Story", "Frame_attributes_lackfail")]
public class StoryNotEnoughAttributeWindow : BaseView
{

    static StoryNotEnoughAttributeWindow ins;
    public static StoryNotEnoughAttributeWindow Ins
    {
        get
        {
            return ins;
        }
    }
    GLoader imgLoader;
    GTextField numText;
    GTextField tittleText;
    GButton attributeBtn;
    public override void InitUI()
    {
        controller = ui.GetController("c1");
        tittleText = SearchChild("n2").asTextField;
        imgLoader = SearchChild("n4").asLoader;
        numText = SearchChild("n5").asTextField;
        attributeBtn = SearchChild("n3").asButton;

        ins = this;
        InitEvent();
    }
    StoryAttributeWindowInfo storyAttributeWindowInfo;

    public override void InitData<D>(D data)
    {
        base.InitData(data);

        storyAttributeWindowInfo = data as StoryAttributeWindowInfo;
        if (storyAttributeWindowInfo == null)
        {
            Debug.LogError("参数为空:storyAttributeWindowInfo");
            return;
        }

        controller.selectedIndex = storyAttributeWindowInfo.type;
        imgLoader.url = CommonUrlConfig.GetConsumeItemUrl((TypeConfig.Consume)storyAttributeWindowInfo.tinyItem.type); ;
        numText.text = storyAttributeWindowInfo.tinyItem.num + "";
        attributeBtn.title = storyAttributeWindowInfo.tinyItem.name;


        if (controller.selectedIndex == StoryAttributeWindowInfo.FAVOUR_TYPE)//如果是好感度不足界面
        {
            tittleText.text = ReplaceForStr(tittleText.text, "X", storyAttributeWindowInfo.actorName);
        }

        if (GameData.isGuider)
        {
            //GameData.guiderCurrent = GuiderInfoLinked.GetCurrGuiderInfo();
            //UIMgr.Ins.showNextPopupView<NewbieGuideView>();

        }
    }


    string ReplaceForStr(string text, string oldStr, string newStr)
    {
        int index = text.IndexOf(oldStr);
        return text.Substring(0, index) + newStr + text.Substring(index + 1);
    }

    public override void InitData()
    {
        base.InitData();
    }

    public override void InitEvent()
    {
        base.InitEvent();
        //close
        SearchChild("n14").onClick.Set(onHide);

        //consume
        attributeBtn.onClick.Set(() =>
        {
            onHide();
            NormalInfo normalInfo = new NormalInfo();
            normalInfo.noIndex = storyAttributeWindowInfo.storyInfo.node_id;
            normalInfo.type = GameConsumeConfig.STORY_PASS_NODE_TYPE;
            EventMgr.Ins.DispachEvent(EventConfig.STORY_SKIP_NODE, normalInfo);
        });


        //attribute

        SearchChild("n6").onClick.Set(() =>
        {
            UpgradeAttribute();
        });
        //restart
        SearchChild("n7").onClick.Set(() =>
        {

        });

    }

    public void UpgradeAttribute()
    {
        onHide();
        StoryView.view.OnHideAnimation();
        NormalInfo normalInfo = new NormalInfo();
        normalInfo.index = 0;
        AudioClip audioClip = ResourceUtil.LoadBackGroundMusic(SoundConfig.CommonMusicId.BgId);
        GRoot.inst.PlayBgSound(audioClip);
        if (controller.selectedIndex == StoryAttributeWindowInfo.ATTRIBUTE_TYPE)
        {
            //进入成长界面

            EventMgr.Ins.DispachEvent(EventConfig.GOTO_VIEW_ROLE_GROUP, normalInfo);
        }
        else if (controller.selectedIndex == StoryAttributeWindowInfo.FAVOUR_TYPE)
        {
            //进入角色互动界面
            UIMgr.Ins.showNextPopupView<InteractiveView, NormalInfo>(normalInfo);
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

[ViewAttr("Game/UI/T_Common", "T_Common", "Frame_story", true)]
public class DollStoryView : BaseView
{
    GTextField contexText;
    GameInitCardsConfig doll;
    GLoader storyBgLoader;
    public override void InitUI()
    {
        base.InitUI();
        GComponent gComponent = SearchChild("n11").asCom;
        contexText = gComponent.GetChild("n2").asTextField;

        InitEvent();
    }

    public override void InitData<D>(D data)
    {
        OnShowAnimation();
        base.InitData(data);
        doll = data as GameInitCardsConfig;
        if (doll != null)
            contexText.text = doll.story;
    }

    public override void InitEvent()
    {
        SearchChild("n16").onClick.Set(Delete);
    }

    public void Delete()
    {
        onDeleteAnimation<DollStoryView>();
    }
}

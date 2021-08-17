using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

[ViewAttr("Game/UI/J_Story", "J_Story", "Frame_imageselect")]
public class StoryImageSelectView : BaseView
{
    GamePointConfig gamePointConfig;
    GLoader bgLoader;
    GComponent gComponent;

    public override void InitUI()
    {
        base.InitUI();
        bgLoader = SearchChild("n2").asLoader;
        gComponent = SearchChild("n3").asCom;

        InitEvent();

    }

    public override void InitEvent()
    {
        base.InitEvent();
        SearchChild("n0").onClick.Set(() =>
        {
            EventMgr.Ins.DispachEvent(EventConfig.STORY_BREACK_STORY);
        });

        EventMgr.Ins.ReplaceEvent(EventConfig.STORY_GAME_QUIT, () =>
        {
            EventMgr.Ins.RemoveEvent(EventConfig.STORY_GAME_QUIT);
            UIMgr.Ins.HideView<StoryImageSelectView>();
        });

    }

    public override void InitData<D>(D data)
    {
        base.InitData(data);
        gamePointConfig = data as GamePointConfig;
        bgLoader.url = UrlUtil.GetStoryBgUrl(gamePointConfig.background_id);

        string[] content1 = gamePointConfig.content1.Split(new char[1] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
        GComponent gCom1 = UIPackage.CreateObject("T_Common", "kong").asCom;
        gCom1.SetSize(float.Parse(content1[0]), float.Parse(content1[1]));
        gCom1.xy = new Vector2(float.Parse(content1[2]), float.Parse(content1[3]));
        gCom1.onClick.Set(() =>
        {
            GotoNextPoint(gamePointConfig.point1);
        });
        gComponent.AddChild(gCom1);
        FXMgr.CreateEffectWithScale(gComponent, new Vector2(float.Parse(content1[5]), float.Parse(content1[6])), content1[4], 162, -1);

        string[] content2 = gamePointConfig.content2.Split(new char[1] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
        GComponent gCom2 = UIPackage.CreateObject("T_Common", "kong").asCom;
        gCom2.SetSize(float.Parse(content2[0]), float.Parse(content2[1]));
        gCom2.xy = new Vector2(float.Parse(content2[2]), float.Parse(content2[3]));
        gCom2.onClick.Set(() =>
        {
            GotoNextPoint(gamePointConfig.point2);
        });
        gComponent.AddChild(gCom2);
        FXMgr.CreateEffectWithScale(gComponent, new Vector2(float.Parse(content2[5]), float.Parse(content2[6])), content2[4], 162, -1);

        string[] content3 = gamePointConfig.content3.Split(new char[1] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
        GComponent gCom3 = UIPackage.CreateObject("T_Common", "kong").asCom;
        gCom3.SetSize(float.Parse(content3[0]), float.Parse(content3[1]));
        gCom3.xy = new Vector2(float.Parse(content3[2]), float.Parse(content3[3]));
        gCom3.onClick.Set(() =>
        {
            GotoNextPoint(gamePointConfig.point3);
        });
        gComponent.AddChild(gCom3);
        FXMgr.CreateEffectWithScale(gComponent, new Vector2(float.Parse(content3[5]), float.Parse(content3[6])), content3[4], 162, -1);

    }

    public override void InitData()
    {
        base.InitData();
        //bgLoader.url = UrlUtil.GetGameBGUrl("Game10", "bg");

        string[] content1 = "200,200,274,645,G10_p2,308,601".Split(new char[1] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
        GComponent gCom1 = UIPackage.CreateObject("T_Common", "kong").asCom;
        gCom1.SetSize(float.Parse(content1[0]), float.Parse(content1[1]));
        gCom1.xy = new Vector2(float.Parse(content1[2]), float.Parse(content1[3]));
        gCom1.onClick.Set(() => { Debug.LogError("point1"); });
        gComponent.AddChild(gCom1);
        FXMgr.CreateEffectWithScale(gComponent, new Vector2(float.Parse(content1[5]), float.Parse(content1[6])), content1[4], 162, -1);

        string[] content2 = "200,200,-11,751,G10_p1,128,886".Split(new char[1] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
        GComponent gCom2 = UIPackage.CreateObject("T_Common", "kong").asCom;
        gCom2.SetSize(float.Parse(content2[0]), float.Parse(content2[1]));
        gCom2.xy = new Vector2(float.Parse(content2[2]), float.Parse(content2[3]));
        gCom2.onClick.Set(() => { Debug.LogError("point2"); });
        gComponent.AddChild(gCom2);
        FXMgr.CreateEffectWithScale(gComponent, new Vector2(float.Parse(content2[5]), float.Parse(content2[6])), content2[4], 162, -1);

        string[] content3 = "200,200,586,923,G10_p3,694,1074".Split(new char[1] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
        GComponent gCom3 = UIPackage.CreateObject("T_Common", "kong").asCom;
        gCom3.SetSize(float.Parse(content3[0]), float.Parse(content3[1]));
        gCom3.xy = new Vector2(float.Parse(content3[2]), float.Parse(content3[3]));
        gCom3.onClick.Set(() => { Debug.LogError("point3"); });
        gComponent.AddChild(gCom3);
        FXMgr.CreateEffectWithScale(gComponent, new Vector2(float.Parse(content3[5]), float.Parse(content3[6])), content3[4], 162, -1);
    }


    void GotoNextPoint(int pointId)
    {
        NormalInfo normalInfo = new NormalInfo();
        normalInfo.index = pointId;
        EventMgr.Ins.DispachEvent(EventConfig.STORY_RECORD_SELECT_NODE, normalInfo);

    }



}

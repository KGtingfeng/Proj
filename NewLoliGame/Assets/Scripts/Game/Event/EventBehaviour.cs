using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;


/// <summary>
/// 主要用于事件注册
/// </summary>
public class EventBehaviour : MonoBehaviour
{



    private void Awake()
    {

        //注册进入成长界面事件
        EventMgr.Ins.RegisterEvent<NormalInfo>(EventConfig.GOTO_VIEW_ROLE_GROUP, RegisetGoToRoleGroupView);
        //注册玩家信息同步数据 这里是同步玩家的所有信息
        EventMgr.Ins.RegisterEvent(EventConfig.SYCHRONIZED_PLAYER_INFO, SychronizedPlayerInfo);
        //注册跳转头像界面
        EventMgr.Ins.RegisterEvent(EventConfig.GOTO_VIEW_PLAYER_HEAD, ShowPlayerHeadView);
        //手动触发点击声音
        EventMgr.Ins.RegisterEvent(EventConfig.SOUND_CLICK_BTN, PlayClickSound);

    }

    void SwitchMainViewController()
    {
        EventMgr.Ins.DispachEvent(EventConfig.MUSIC_MAIN_TIPS);
        //MainView.ins.SwitchController((int)MainView.MoudleType.Top);
    }

    /// <summary>
    /// 注册进入成长界面事件
    /// </summary>
    /// <param name="normalInfo">Normal info.</param>
    void RegisetGoToRoleGroupView(NormalInfo normalInfo)
    {
        SwitchMainViewController();
        UIMgr.Ins.showNextPopupView<RoleGropView, NormalInfo>(normalInfo);
    }

   


    /// <summary>
    /// 按需同步玩家数据
    /// </summary>
    void SychronizedPlayerInfo()
    {
        Action callBack = () => { };
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("playerId", GameData.playerId);
        wWWForm.AddField("userId", GameData.User.id);

        GameMonoBehaviour.Ins.RequestInfoPost<Player>(NetHeaderConfig.GET_PLAYR_INFO, wWWForm, callBack, false);
    }


    void ShowPlayerHeadView()
    {
        SwitchMainViewController();
        UIMgr.Ins.showNextPopupView<PlayerHeadView>();
    }


    void PlayClickSound()
    { 
        Stage.inst.PlayOneShotSound(UIConfig.buttonSound.nativeClip, 0.5f);
    }

}

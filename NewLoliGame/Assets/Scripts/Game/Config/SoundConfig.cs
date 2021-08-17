using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sound config.声音相关配置
/// </summary>
public class SoundConfig
{
    /// <summary>
    /// 通用背景音效地址
    /// </summary>
    public static string COMMON_AUDIO_EFFECT_URL = "Audio/Common/Effect/";
    /// <summary>
    /// 通用背景音乐
    /// </summary>
    public static string COMMON_AUDIO_MUSIC_URL = "Audio/Common/Music/";
    /// <summary>
    /// 主界面点击角色配置url
    /// </summary>
    public static string COMMON_MAIN_ROLE_URL = "Audio/Main/";



    /// <summary>
    /// 剧情音效通用地址前缀
    /// </summary>
    public static string STORY_AUDIO_EFFECT_URL = "Audio/Story/Effect/";
    /// <summary>
    /// 剧情背景声音前缀
    /// </summary>
    public static string STORY_AUDIO_MUSIC_URL = "Audio/Story/Music/";
    /// <summary>
    /// 剧情对白地址前缀
    /// </summary>
    public static string STORY_AUDIO_SOUND_URL = "Audio/Story/Sound/";

    /// <summary>
    /// 音效id定义
    /// </summary>
    public enum CommonEffectId
    {
        //点击按钮
        ClickBtn = 1,
        //打开界面
        OpenView = 2,
        //获得奖励
        GetAward = 3,
        //角色升级
        RoleUpgrade = 4,
        //切换界面
        SwitchView = 5,
        //属性升级
        AttribuevUpgrade = 6,
        //娃娃升级
        DollUpgrade = 7,
        //获得娃娃
        GETDOLL = 8,
        //击掌
        Highfive=9,
        //属性升级特效
        AttribuevUpgradeFx = 10,
        //娃娃升级特效
        DollUpgradeFx = 11,
        //角色升级特效
        RoleUpgradeFx = 12,
        //角色升级特效
        ChooseDollFx = 13,
    }

    /// <summary>
    /// 通用背景音乐
    /// </summary>
    public enum CommonMusicId
    {
        //背景音乐
        BgId = 1,
        //制作娃娃音乐
        MakeDolls = 2,
        //钓垃圾音乐
        PickUp = 3,
        //垃圾分类音乐
        Rubbish = 4,
        //找茬音乐
        FindDifference = 5,
        //选择娃娃音乐
        ChooseRole = 6,
    }

    /// <summary>
    /// 互动音效
    /// </summary>
    public static string INTERACTIVE_AUDIO_EFFECT_URL = "Audio/Interactive/";

    /// <summary>
    /// 互动音效ID定义
    /// </summary>
    public enum InteractiveAudioId
    {
        //切换时装或背景
        SkinClick = 0,
        //获得好感度
        GetFavor,
        //好感度下降
        FavorDown,
    }

    /// <summary>
    /// 手机音效
    /// </summary>
    public static string PHONE_AUDIO_EFFECT_URL = "Audio/Call/";

    /// <summary>
    /// 手机音效ID定义
    /// </summary>
    public enum PhoneAudioId
    {
        //短信
        Message = 1,
        //电话
        Call,
    }
    public static string GetGameAudioUrl(GameAudioId id)
    {
        return GAME_AUDIO_EFFECT_URL + (int)id;
    }
    /// <summary>
    /// 小游戏音效
    /// </summary>
    public static string GAME_AUDIO_EFFECT_URL = "Audio/Game/";
    /// <summary>
    /// 音乐
    /// </summary>
    public static string GAME_MUSIC_URL = "Audio/Music/";
    /// <summary>
    /// 小游戏音效ID定义
    /// </summary>
    public enum GameAudioId
    {
        //砍东西
        Sword = 0,
        //放下物品
        PutDown = 1,
        //获得物品
        GetItems = 2,
        //跳水
        Diving = 3,
        //跳
        Jump = 4,
        //倒计时
        Countdown = 5,
        //点击失败
        ClickFail = 6,
        //点击成功
        ClickSuccess = 7,
        //筋脉线出现
        ConnectJinmai = 8,
        //通用错误
        Error = 9,
        //打开机关
        OpenGear = 10,
        //点击表盘成功
        ClickClock = 11,
        //打捞成功
        Pickup = 12,
        //曼朵拉攻击音效
        MDL_Attact = 13,
        //防御曼朵拉点击屏幕
        MDL_Click = 14,
        //魔法阵修复音效
        MDL_Fix = 15,
        //抓垃圾抓到障碍
        PICK_ROCK = 16,
        //抓垃圾绳子
        PICK_ROPE = 17,
        //完成娃娃
        FINISH_DOLL = 18,
        //鋼琴點擊
        PIANO = 19,
        //成功
        SUCCESS = 20,
        //旋转
        SPIN = 21,
        //通用成功
        COMMOND_SUCCESS = 22,
        //电击
        ELECTRIC = 23,
        //碎
        SUI = 24,
    }

    /// <summary>
    /// 起床铃音效
    /// </summary>
    public static string WAKEUP_BELL_AUDIO_EFFECT_URL = "Audio/AlarmClock/";

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventConfig
{

    /// <summary>
    /// 刷新成长模块
    /// </summary>
    public static readonly string REFRESH_ROLE_INFO_MOUDEL = "refreshRoleMoudelKey";
    /// <summary>
    /// 玩家升级
    /// </summary>
    public static readonly string PLAYER_UPGRADE_LEVEL = "playerUpgradeLevel";
    /// <summary>
    /// 娃娃升级
    /// </summary>
    public static readonly string DOLL_UPGRADE_LEVEL = "dollUpgradeLevel";
    /// <summary>
    /// 娃娃属性升级
    /// </summary>
    public static readonly string DOLL_ATTRIBUTE_UPGRADE = "dollAttributeUpgrade";
    /**********刷新顶部信息*************/
    /// <summary>
    /// 刷新用户顶部信息
    /// </summary>
    public static readonly string REFRESH_USER_TOP_INFO = "refreshPlayerTopInfo";
    /// <summary>
    /// 刷新互动顶部信息
    /// </summary>
    public static readonly string REFRESH_INTERACTIVE_TOP_INFO = "refreshInteractiveTopInfo";
    /// <summary>
    /// 刷新商城顶部信息
    /// </summary>
    public static readonly string REFRESH_SHOPMALL_TOP_INFO = "refreshShopmallTopTnfo";
    /// <summary>
    /// 刷新好友系统顶部信息
    /// </summary>
    public static readonly string REFRESH_FRIEND_TOP_INFO = "refreshFriendTopInfo";
    /// <summary>
    /// 刷新福利系统顶部信息
    /// </summary>
    public static readonly string REFRESH_WELFARE_TOP_INFO = "refreshWelfareTopInfo";
    /// <summary>
    /// 刷新属性升级顶部信息
    /// </summary>
    public static readonly string REFRESH_ATTRIBUITE_TOP_INFO = "refreshAttribuiteTopTnfo";
    /// <summary>
    /// 刷新制作工坊顶部信息
    /// </summary>
    public static readonly string REFRESH_MAKEGIFTS_TOP_INFO = "refreshMakeGiftsTopTnfo";

    /// <summary>
    /// 刷新成就顶部信息
    /// </summary>
    public static readonly string REFRESH_ACHIEVEMENT_TOP_INFO = "refreshAchievementTopTnfo";
    /// <summary>
    /// 刷新角色顶部信息
    /// </summary>
    public static readonly string REFRESH_ROLE_TOP_INFO = "refreshAchievementTopTnfo";
    /// <summary>
    /// 刷新剧情顶部信息
    /// </summary>
    public static readonly string REFRESH_STORY_TOP_INFO = "refreshStoryTopTnfo";
    /**************************************/

    /// <summary>
    /// 刷新用户详细信息
    /// </summary>
    public static readonly string REFRESH_PLAYER_BASE_INFO = "refreshPlayerBaseInfo";
    /// <summary>
    /// 获得娃娃
    /// </summary>
    public static readonly string GET_DOLL = "userGetDoll";

    /// <summary>
    /// 请求心跳
    /// </summary>
    public static readonly string GET_HEARTBEAT = "getHeartbeat";
    /**********剧情事件*************/

    public static readonly string STORY_INTO_CHAPTER = "storyIntoChapter";
    public static readonly string STORY_REFRESH_HEAD_INFO = "storyRefreshHeadInfo";
    public static readonly string STORY_TRIGGER_LOOP = "storyTriggerLoop";
    public static readonly string STORY_CHAPTER_STARTING = "storyChapterStaring";
    public static readonly string STORY_GOTO_TRANSITION = "storyGoToTransition";
    public static readonly string STORY_TRIGGER_NEXT_NODE = "storyTriggerNextNode";
    public static readonly string STORY_ANIMATION_VIEW = "storyAnimationView"; 
    public static readonly string STORY_BREACK_STORY = "storyBreakStory";
    public static readonly string STORY_READ_RECORD = "storyReadRecord";
    public static readonly string STORY_REFRESH_NODE = "storyRefreshNode";
    public static readonly string STORY_GOTO_ATTRIBUTE = "storyGoToAttribue";
    public static readonly string STORY_RECORD_SELECT_NODE = "storyRecordSelectNode";
    public static readonly string STORY_PLAY_RECORD = "storyPlayRecords";
    public static readonly string STORY_SKIP_NODE = "storySkipNodes";
    public static readonly string STORY_GOTO_SELECT_STORY = "storySelectStories";

    public static readonly string STORY_BREAKE_MAIN = "storyBreakToMain";
    /// <summary>
    /// 剧情中角色升级或好感提升特效
    /// </summary>
    public static readonly string STORY_SHOW_GET_EFFECT = "storyShowGetEffect";
    /// <summary>
    /// 剧情游戏退出，无论是进入下一剧情还是返回剧情选择
    /// </summary>
    public static readonly string STORY_GAME_QUIT = "storyGameQuit";
    /// <summary>
    /// 删除游戏保存信息，当跳过时调用
    /// </summary>
    public static readonly string STORY_DELETE_GAME_INFO = "storyDeleteGameInfo";


    /**********获取奖励*************/
    public static readonly string AWARD_GET_SINGLE = "awardGetSingle";





    /**********登录事件*************/
    public static readonly string LOGIN_GOTO_LOGIN = "loginGotoLogin";
    public static readonly string LOGIN_ANNOUCEMENT = "loginAnnouncement";

    public static readonly string LOGIN_SERVERLIST = "loginServerList";
    public static readonly string LOGIN_SERVICEAGREEMENT = "loginServiceAgreement";


    /**********Dispose*************/
    public static readonly string STORY_DIALOG_DISPOSE = "storyDialogDispose";



    /**********Music*************/
    public static readonly string MUSIC_COMMON_BG_MUSIC = "commonBgMusic";
    public static readonly string MUSIC_STORY_BG_MUSIC = "storyBgMusic";
    public static readonly string MUSIC_STORY_EFFECT = "storyEffect";
    public static readonly string MUSIC_MAIN_TIPS = "mainTipsSound";
    public static readonly string MUSIC_COMMON_EFFECT = "commonEffect";
    public static readonly string MUSIC_DIALOG_PLAY_OVER = "playOverDialogMusic";

    /**********view跳转事件*************/
    public static readonly string GOTO_VIEW_ROLE_GROUP = "gotoViewRoleGroup";
    public static readonly string GOTO_VIEW_PLAYER_HEAD = "gotoViewPlayerHead";


    /**********数据同步事件*************/
    public static readonly string SYCHRONIZED_PLAYER_INFO = "sychronizedPlayerInfo";
    //同步角色部分属性(例如货币)
    public static readonly string REFRESH_PLAYER_PROPERTY_INFO = "refreshPlayerPropertyInfo";

    /**********点击事件*************/
    public static readonly string SOUND_CLICK_BTN = "soundClickBtn";

    /**********view跳转事件*************/
    public static readonly string GOTO_VIEW_MAIN_EFFECT = "gotoViewMainEffect";
    /// <summary>
    /// 前往剧情
    /// </summary>
    public static readonly string GOTO_VIEW_STORY = "gotoViewStory";

    /**********互动界面事件*************/
    /// <summary>
    /// 点击空白处
    /// </summary>
    public static readonly string BLANK_CLICK = "blankClick";
    /// <summary>
    /// 修改SPINE
    /// </summary>
    public static readonly string CHANGE_SPINE = "changeSpine";
    public static readonly string CHANGE_BACKGROUND = "changeBackground";
    /// <summary>
    /// 从外观界面返回
    /// </summary>
    public static readonly string APPEARANCE_REBACK = "appearanceReback";
    /// <summary>
    /// 刷新制作工坊列表
    /// </summary>
    public static readonly string REFRESH_ALL_GIFT_LIST = "refreshAllGiftList";
    /// <summary>
    /// 刷新拥有礼物列表
    /// </summary>
    public static readonly string REFRESH_OWN_GIFT_LIST = "refreshOwnGiftList";
    /// <summary>
    /// 好感动改变
    /// </summary>
    public static readonly string FAVOR_CHANGE = "favorChange";
    /// <summary>
    /// 赠送成功特效
    /// </summary>
    public static readonly string PRESENT_SUCCESS_EFFECT = "presentSuccessEffect";
    /// <summary>
    /// 关闭送礼后说话
    /// </summary>
    public static readonly string CLOSE_PRESENT_TALK = "closePresentTalk";
    /// <summary>
    /// 刷新外观列表
    /// </summary>
    public static readonly string REFRESH_APPEARANCE_LIST = "refreshAppearanceList";
    /// <summary>
    /// 倒计时刷新
    /// </summary>
    public static readonly string REFRSH_COUNTDOWN = "refreshCountdown";
    /// <summary>
    /// 互动主界面动画
    /// </summary>
    public static readonly string INTERACTIVE_MAIN_EFFECT = "interactiveMainEffect";
    /// <summary>
    /// 关闭互动主界面说话
    /// </summary>
    public static readonly string CLOSE_INTERACTIVE_TALK = "closeInteractiveTalk";
    /// <summary>
    /// 刷新礼物
    /// </summary>
    public static readonly string REFRESH_PROPS = "refreshProps";
    /// <summary>
    /// 刷新制作工坊
    /// </summary>
    public static readonly string REFRESH_MAKE_GIFTS = "refreshMakeGifts";
    /// <summary>
    /// 刷新今日心愿
    /// </summary>
    public static readonly string REFRESH_TODAY_WISH = "refreshTodayWish";
    /// <summary>
    /// 购买服装背景特效
    /// </summary>
    public static readonly string SHOW_BUY_SKIN_EFFECT = "showBuySkinEffect";
    /********************隐藏提示框事件*****************/
    public static readonly string HIDE_TIPS_PANEL = "hideTipsPanel";
    /// <summary>
    /// 主界面ICON动画
    /// </summary>
    public static readonly string MAIN_OPEN_EFFECT = "mainOpenEffect";
    /// <summary>
    /// 星星飞出特效
    /// </summary>
    public static readonly string STAR_FLY_EFFECT = "starFlyEffect";
    /// <summary>
    /// 钻石飞出特效
    /// </summary>
    public static readonly string DIAMOND_FLY_EFFECT = "diamondFlyEffect";


    /*******************手机事件************************/
    /// <summary>
    /// 离开聊天界面
    /// </summary>
    public static readonly string SMS_MOUDLE_LEAVE = "smsMoudleLeave";
    /// <summary>
    /// 手机主界面信息增加
    /// </summary>
    public static readonly string SMS_ADD_INFO = "smsAddInfo";
    /// <summary>
    /// 刷新红包节点
    /// </summary>
    public static readonly string SMS_REFRESH_REDBAO = "smsRefreshRedBao";
    /// <summary>
    /// 回复朋友圈
    /// </summary>
    public static readonly string MOMENTS_REPLY = "momentsReply";
    /// <summary>
    /// 发布朋友圈
    /// </summary>
    public static readonly string MOMENTS_RELEASE = "momentsRelease";
    /// <summary>
    /// 电话或视频开始主界面刷新
    /// </summary>
    public static readonly string SMS_CALL_START_MAIN = "smsCallStartMain";
    /// <summary>
    /// 电话或视频结束主界面刷新
    /// </summary>
    public static readonly string SMS_CALL_FINISH_MAIN = "smsCallFinishMain";
    /// <summary>
    /// 电话或视频开始电话记录刷新
    /// </summary>
    public static readonly string SMS_CALL_START_CALL_RECORD = "smsCallStartCallRecord";
    /// <summary>
    /// 电话或视频结束电话记录刷新
    /// </summary>
    public static readonly string SMS_CALL_FINISH_CALL_RECORD = "smsCallFinishCallRecord";
    /// <summary>
    /// 短信推送刷新聊天
    /// </summary>
    public static readonly string SMS_PUSH_REFRESH = "smsPushRefresh";
    /// <summary>
    /// 电话推送刷新聊天
    /// </summary>
    public static readonly string SMS_CALL_PUSH_REFRESH = "smsCallPushRefresh";
    /// <summary>
    /// 拒绝接听来电
    /// </summary>
    public static readonly string REJECT_THE_CALL = "rejectTheCall";
    /// <summary>
    /// 更多朋友圈回退
    /// </summary>
    public static readonly string MORE_MOMENTS_REBACK = "moreMomentsReback";
    /// <summary>
    /// 前往个人朋友圈
    /// </summary>
    public static readonly string GOTO_PERSONAL_MPMENT = "gotoPersonalMoment";
    /*******************房间事件************************/
    /// <summary>
    /// 更改铃声
    /// </summary>
    public static readonly string CHANGE_RING = "changeRing";
    /// <summary>
    /// 刷新房间顶部信息
    /// </summary>
    public static readonly string REFRESH_ROOM_TOP_INFO = "refreshRoomTopInfo";

    /*******************邮件事件************************/
    /// <summary>
    /// 读邮件
    /// </summary>
    public static readonly string READ_MAIL = "readMail";
    /// <summary>
    /// 领取邮件
    /// </summary>
    public static readonly string GET_MAIL = "getMail";

    /*******************好友事件************************/
    /// <summary>
    /// 根据对方ID，跳转到其详细信息页面
    /// </summary>
    public static readonly string GET_FRIEND_DETAIL = "getFriendDetail";
    /// <summary>
    /// 排行榜和好友列表中删除好友后，更新列表
    /// </summary>
    public static readonly string REFRESH_AFTER_DELETE = "refreshAfterDelete";
    /// <summary>
    /// 确认删除好友后，好友信息界面关闭
    /// </summary>
    public static readonly string HIDE_PLAYER_INFO = "hidePlayerInfo";
    /*******************小游戏事件************************/
    /// <summary>
    /// 装入宝石保存
    /// </summary>
    public static readonly string PUT_GEMS_SAVE = "putGemsSave";


    /*******************排行榜事件************************/
    /// <summary>
    /// 设置前三
    /// </summary>
    public static readonly string SET_TOP_THREE = "setTopThree";

    /*******************头像框事件************************/
    /// <summary>
    /// 刷新主界面头像框
    /// </summary>
    public static readonly string REFRESH_AVATAR_FRAME = "refreshAvatarFrame";
    /// <summary>
    /// 刷新玩家信息称号
    /// </summary>
    public static readonly string REFRESH_PLAYER_TITLE = "refreshPlayerTitle";
    /*******************任务事件************************/
    /// <summary>
    /// 购买星星时刷新任务
    /// </summary>
    public static readonly string BUY_STAR_REFRESH = "buyStarRefresh";
    /// <summary>
    /// 购买星星时刷新成就任务
    /// </summary>
    public static readonly string REFRESH_ACHIEVEMENT_TASK = "refreshAchievementTask";

    /*******************辛灵游戏退出事件************************/
    /// <summary>
    /// 垃圾分类任务退出
    /// </summary>
    public static readonly string GAME_CLASSIFICATION_EXIT = "gameClassificationExit";
    /// <summary>
    /// 找茬任务退出
    /// </summary>
    public static readonly string GAME_FIND_EXIT = "gameFindExit";
    /// <summary>
    /// 捡垃圾任务退出
    /// </summary>
    public static readonly string GAME_PICKUP_EXIT = "gamePickupExit";
    /// <summary>
    /// 制作娃娃任务退出
    /// </summary>
    public static readonly string GAME_MAKEDOLL_EXIT = "gameMakedollExit";
    /// <summary>
    /// 制作娃娃任务退出
    /// </summary>
    public static readonly string GAME_THROW_BACK = "gameThrowBack"; 
    /// <summary>
    /// 制作娃娃任务退出
    /// </summary>
    public static readonly string GAME_THROW_GET = "gameThrowGet";
    /// <summary>
    /// 制作娃娃任务退出
    /// </summary>
    public static readonly string GAME_THROW = "gameThrow";
    /// <summary>
    /// 完成任务
    /// </summary>
    public static readonly string GAME_FINISH_TASk = "gameFinishTask";


    /*******************红点事件************************/

    /// <summary>
    /// 福利tab小红点
    /// </summary>
    public static readonly string WELFARE_TAB_RED_POINT = "welfareTabRedPoint";

    /// <summary>
    /// 好友tab小红点
    /// </summary>
    public static readonly string FRIEND_TAB_RED_POINT = "friendTabRedPoint";




    /// <summary>
    /// 获得红点
    /// </summary>
    public static readonly string GET_RED_POINT = "getRedPoint";
    /// <summary>
    /// 刷新主界面红点
    /// </summary>
    public static readonly string REFRESH_MAIN_RED_POINT = "refreshMainRedPoint";
    /// <summary>
    /// 刷新角色界面红点
    /// </summary>
    public static readonly string REFRESH_ROLEGROUP_RED_POINT = "refreshRolegroupRedPoint";
    /// <summary>
    /// 刷新属性升级红点
    /// </summary>
    public static readonly string REFRESH_ATTRIBUITE_RED_POINT = "refreshAttributeRedPoint";



    /*******************其他************************/
    /// <summary>
    /// 获得主界面按钮位置
    /// </summary>
    public static readonly string GET_MAIN_VIEW_BUTTON_POS = "getMainViewButtonPos";

    /// <summary>
    /// 手动控制闪白消失
    /// </summary>
    public static readonly string SHANBAI_HIDE = "shanbaiHide";
    /// <summary>
    /// 手动控制闪白消失
    /// </summary>
    public static readonly string LOGIN_HIDE = "loginHide";

    /*******************选择角色新手************************/
    /// <summary>
    ///  检查语音
    /// </summary>
    public static readonly string CHECK_SOUND = "checkSound";


}

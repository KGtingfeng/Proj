
public class NetHeaderConfig
{
    #region 
    //外网
    //public static readonly string IP = "http://134.175.224.89:8082/api/";
    //本地
    //public static readonly string IP = "http://192.168.4.20:8082/api/";
    //luo
    //public static readonly string IP = "http://192.168.4.23:8082/api/";
    //public static readonly string IP = "http://134.175.224.89:8082/api/";
    public static string IP = IPConfig.IP_CONFIG;
    #endregion


    #region methodName
    /// <summary>
    /// 注册.
    /// </summary>
    public const string SIGNUP = "login/signup";

    /// <summary>
    /// 登录
    /// </summary>
    public const string SIGNIN = "login/signin";
    /// <summary>
    /// 服务器返回时间
    /// </summary>
    public const string SERVER_HEARTBEAT = "server/heartbeat";
    /// <summary>
    /// 公告
    /// </summary>
    public const string QUERY_ANNOUNCEMENT = "server/allnotice";
    /// <summary>
    ///服务器协议
    /// </summary>
    public const string QUERY_SERVICEAGREEMENT = "server/contract";
    /// <summary>
    /// 服务器协议列表
    /// </summary>
    public const string QUERY_SERVICELIST = "server/list";
    /// <summary>
    /// 创建玩家
    /// </summary>
    public const string CREATE_PLAYER = "player/create/player";
    /// <summary>
    /// 创建玩家和契约娃娃合成一个接口
    /// </summary>
    public const string CREATE_PLAYER_NEW = "player/creates";
    /// <summary>
    /// 获得全部默认娃娃数据
    /// </summary>
    public const string GET_DEFAULT_DOLLS = "player/all/doll";
    /// <summary>
    /// 角色契约
    /// </summary>
    public const string CONTRACT = "player/create/concluded";
    /// <summary>
    /// 获取玩家数据
    /// </summary>
    public const string GET_PLAYR_INFO = "player/player/info";
    /// <summary>
    /// 获取所有娃娃数据
    /// </summary>
    public const string GET_ALL_DOLLS = "player/const/doll";
    /// <summary>
    /// 玩家等级提升
    /// </summary>
    public const string UPGRADE_PLAYER_LEVEL = "player/player/upgrade";
    /// <summary>
    /// 玩家属性升级
    /// </summary>
    public const string UPGRADE_PLAYER_ATTRIBUTE = "player/attribute/upgrade";
    /// <summary>
    /// 娃娃升级
    /// </summary>
    public const string UPGRADE_DOLL_LEVEL = "player/card/upgrade";
    /// <summary>
    /// 娃娃购买
    /// </summary>
    public const string DOLL_BUY = "player/card/buy";
    /// <summary>
    /// 娃娃皮肤购买
    /// </summary>
    public const string DOLL_SKIN_BUY = "doll/buy/skin";
    /// <summary>
    /// 娃娃皮肤设置
    /// </summary>
    public const string DOLL_SKIN_SET = "doll/set/skin";
    /// <summary>
    /// 娃娃碎片合成
    /// </summary>
    public const string DOLL_COMPOSE = "player/card/compose";
    /// <summary>
    ///修改玩家信息 
    /// </summary>
    public const string MODIFY_PLAYERINFO = "player/edit";
    /// <summary>
    ///获得玩家头像
    /// </summary>
    public const string GET_PLAYER_AVATAR = "player/avatar";
    /// <summary>
    /// 获得玩家属性(货币)信息
    /// </summary>
    public const string GET_PLAYER_PPROPERTY = "player/property";
    /// <summary>
    /// 道具信息
    /// </summary>
    public const string PLAYER_PROP_INFO = "player/prop/info";
    /// <summary>
    /// 获取玩家章节信息
    /// </summary>
    public const string STORY_GET_SOTRY_INFO = "story/get/story";
    /// <summary>
    /// 记录节点
    /// </summary>
    public const string STORY_RECORD_NODE = "story/set/chapter";
    /// <summary>
    /// 判断节点是否能过
    /// </summary>
    public const string STORY_JUDGE_NODE = "story/can/pass";
    /// <summary>
    /// 获取章节信息
    /// </summary>
    public const string STORY_CHAPTER_INFO = "story/get/playerpassed";
    /// <summary>
    /// 花费道具通关
    /// </summary>
    public const string STROY_SKIP_NODE = "story/passbystar";
    /// <summary>
    /// 重置故事信息
    /// </summary>
    public const string STROY_RESET = "story/reset/story";
    /// <summary>
    /// 小游戏进度保存
    /// </summary>
    public const string STROY_SAVE_GAME = "story/game/save";
    /// <summary>
    /// 小游戏进度加载
    /// </summary>
    public const string STROY_LOAD_GAME = "story/game/load";
    /// <summary>
    /// 小游戏进度删除
    /// </summary>
    public const string STROY_DELETE_GAME = "story/game/delete";
    /// <summary>
    /// 小游戏进度删除
    /// </summary>
    public const string STROY_DELETE_BY_KEY = "game/delbykey";
    /// <summary>
    /// 剧情步骤消耗
    /// </summary>
    public const string STROY_STEP_CONSUME = "story/step/consume";

    /// <summary>
    /// 渠道有关开关
    /// </summary>
    public const string CONFIG_GAME_ALL = "config/game/all";
    /**************************角色互动*********************************/
    /// <summary>
    /// 修改角色昵称
    /// </summary>
    public const string ACTOR_EDIT = "actor/edit";
    /// <summary>
    /// 获得角色信息
    /// </summary>
    public const string ACTOR_LIST = "actor/list";
    /// <summary>
    /// 购买角色
    /// </summary>
    public const string ACTOR_BUY = "actor/buy";
    /// <summary>
    /// 角色道具列表
    /// </summary>
    public const string ACTOR_PROP_LIST = "actor/prop/list";
    /// <summary>
    /// 赠送道具给角色
    /// </summary>
    public const string PRESENT_PROP = "actor/prop/present";
    /// <summary>
    /// 学习制作道具
    /// </summary>
    public const string PROP_STUDY = "actor/prop/study";
    /// <summary>
    /// 制作道具
    /// </summary>
    public const string PROP_MAKE = "actor/prop/make";
    /// <summary>
    /// 角色皮肤列表
    /// </summary>
    public const string ACTOR_SKIN_LIST = "actor/skin/list";
    /// <summary>
    /// 角色皮肤购买
    /// </summary>
    public const string ACTOR_SKIN_BUY = "actor/skin/buy";
    /// <summary>
    /// 角色皮肤设置
    /// </summary>
    public const string ACTOR_SET = "actor/set/current";
    /// <summary>
    /// 角色背景列表
    /// </summary>
    public const string ACTOR_BACKGROUND_LIST = "actor/background/list";
    /// <summary>
    /// 角色背景合成
    /// </summary>
    public const string ACTOR_BACKGROUND_COMPOSE = "actor/background/compose";
    /// <summary>
    /// 角色背景购买
    /// </summary>
    public const string ACTOR_BACKGROUND_BUY = "actor/background/buy";
    /// <summary>
    /// 设置到主页
    /// </summary>
    public const string SET_HOMEACTOR = "actor/set/homeactor";
    /// <summary>
    /// 碎片查询
    /// </summary>
    public const string FRAGMENT = "actor/fragment";
    /// <summary>
    /// 今日心愿
    /// </summary>
    public const string TODAY_WISH = "actor/task/wishgift";
    /**************************购买*********************************/
    /// <summary>
    /// 商品购买
    /// </summary>
    public const string MALL_BUY = "mall/buy";
    /// <summary>
    /// 获得玩家限购等信息
    /// </summary>
    public const string MALL_INFO = "mall/info";
    /// <summary>
    /// 添加玩家某种资源
    /// </summary>
    public const string PLAYER_ADD_RESOURCES = "player/add/resource";
    /// <summary>
    /// 直接购买100个星星
    /// </summary>
    public const string PLAYER_BUY_STAR = "player/buystar";
    /// <summary>
    /// 月卡信息
    /// </summary>
    public const string PLAYER_CARD_INFO = "player/card/info";

    /**************************实名*********************************/
    /// <summary>
    /// 实名
    /// </summary>
    public const string REAL_NAME = "user/verify";
    /// <summary>
    /// user
    /// </summary>
    public const string USER_INFO = "user/current";
    /// <summary>
    /// 开关配置
    /// </summary>
    public const string SWITCH_CONFIG = "config/all";

    /**************************手机*********************************/
    /// <summary>
    ///消息推送
    /// </summary>
    public const string CELL_CHECK_MESSAGE = "cell/check/message";
    /// <summary>
    /// 手机首页
    /// </summary>
    public const string CELL_LIST_INDEX = "cell/list/index";
    /// <summary>
    /// 历史剧情
    /// </summary>
    public const string CELL_LIST_PACK = "cell/list/pack";
    /// <summary>
    /// 角色聊天记录
    /// </summary>
    public const string CELL_QUERY_MESSAGE = "cell/query/message";
    /// <summary>
    /// 保存当前阶段
    /// </summary>
    public const string CELL_SET_STEP = "cell/set/step";
    /// <summary>
    /// 买一个包
    /// </summary>
    public const string CELL_MESSAGE_BUY = "cell/message/buy";
    /// <summary>
    /// 历史通话剧情
    /// </summary>
    public const string CELL_QUERY_VOICE = "cell/query/voice";
    /// <summary>
    /// 查询朋友圈
    /// </summary>
    public const string CELL_QUERY_MOMENT = "cell/query/moment";
    /// <summary>
    /// set朋友圈
    /// </summary>
    public const string CELL_SET_MOMENT = "cell/set/moment";
    /// <summary>
    /// 查询可发送朋友圈
    /// </summary>
    public const string CELL_SEND_MOMENT = "cell/send/moment";
    /// <summary>
    /// 查询角色朋友圈
    /// </summary>
    public const string CELL_LIST_MOMENT = "cell/list/moment";
    /// <summary>
    /// 短信重置
    /// </summary>
    public const string CELL_SMS_RESET = "cell/reset/message";
    /*************************  背景  *************************/
    /// <summary>
    /// 获得时刻图
    /// </summary>
    public const string PLAYER_MOMENT = "player/moment";
    /// <summary>
    /// 保存背景
    /// </summary>
    public const string STORY_SAVE = "story/save";
    /// <summary>
    /// 加载背景
    /// </summary>
    public const string STORY_LOAD = "story/load";
    /// <summary>
    /// 删除背景
    /// </summary>
    public const string STORY_DELETE = "story/delete";
    /*************************  房间  *************************/
    /// <summary>
    /// 设置闹钟
    /// </summary>
    public const string SAVE_ALARM = "actor/save/alarm";
    /// <summary>
    ///击掌
    /// </summary>
    public const string ACTOR_HIGHFIVE = "actor/highfive";
    /// <summary>
    ///击掌状态
    /// </summary>
    public const string HIGHFIVE_STATUS = "actor/status";
    /// <summary>
    ///闹钟配置
    /// </summary>
    public const string QUERY_ALARM = "actor/query/alarm";
    /// <summary>
    /// 查询时刻
    /// </summary>
    public const string QUERY_TIME = "player/moment";

    /*************************  邮件  *************************/
    /// <summary>
    ///未读邮件数量
    /// </summary>
    public const string UNREAD_MAILS = "player/unread/mails";
    /// <summary>
    ///获取邮件，分页
    /// </summary>
    public const string MAILS_LIST = "player/list/mails";
    /// <summary>
    ///邮件详情
    /// </summary>
    public const string QUERY_MAIL = "player/query/mail";
    /// <summary>
    ///领取邮件
    /// </summary>
    public const string GET_MAILS_AWARD = "player/award/mails";
    /// <summary>
    ///读邮件
    /// </summary>
    public const string READ_MAIL = "player/read/mail";
    /// <summary>
    ///删除邮件
    /// </summary>
    public const string DELETE_MAILS = "player/delete/mails";
    /*************************  排行榜  *************************/
    /// <summary>
    ///属性排行
    /// </summary>
    public const string RANK_ATTR_ALL = "rank/attr/all";
    /// <summary>
    ///属性单榜
    /// </summary>
    public const string RANK_ATTR_TYPE = "rank/attr/type";
    /// <summary>
    ///好感排行
    /// </summary>
    public const string RANK_FAVOR_ALL = "rank/favor/all";
    /// <summary>
    ///好感单榜
    /// </summary>
    public const string RANK_FAVOR_ID = "rank/favor/id";
    /// <summary>
    ///好感排行
    /// </summary>
    public const string RANK_CARD_ALL = "rank/card/all";
    /// <summary>
    ///好感单榜
    /// </summary>
    public const string RANK_CARD_ID = "rank/card/id";
    /// <summary>
    ///角色信息
    /// </summary>
    public const string RANK_PLAYER_ID = "rank/player/id";
    /*************************  称号  *************************/
    /// <summary>
    /// 设置称号
    /// </summary>
    public const string SET_TITLE = "player/set/title";
    /// <summary>
    /// 设置头像框
    /// </summary>
    public const string SET_FRAME = "player/set/frame";
    /// <summary>
    /// 查询称号
    /// </summary>
    public const string PLAYER_TITLE = "player/title";
    /// <summary>
    /// 查询头像框
    /// </summary>
    public const string PLAYER_FRAME = "player/frame";
    /*************************  任务  *************************/
    /// <summary>
    /// 每日任务
    /// </summary>
    public const string MISSION_LIST = "mission/list";
    /// <summary>
    /// 获得任务奖励
    /// </summary>
    public const string MISSION_GET_AWARD = "mission/finish/mission";
    /// <summary>
    /// 获得宝箱奖励
    /// </summary>
    public const string MISSION_GET_CHESTS = "mission/get/chests";



    /// <summary>
    /// 红点
    /// </summary>
    public const string RED_POINT = "player/get/redpoint";
    /************************   好友  *************************/
    /// <summary>
    /// 好友列表
    /// </summary>
    public const string FRIEND_LIST = "friends/list";
    ///// <summary>
    ///// 我的收礼情况
    ///// </summary>
    //public const string FRIEND_RECEVIED = "friends/received";
    ///// <summary>
    ///// 我的送礼情况
    ///// </summary>
    //public const string FRIEND_SENT = "friends/sent";
    /// <summary>
    /// 领取好友礼物
    /// </summary>
    public const string FRIEND_GIFT_RECEIVE = "friends/award";
    /// <summary>
    /// 赠送好友礼物
    /// </summary>
    public const string FRIEND_GIFT_SEND = "friends/present";
    /// <summary>
    /// 好友推荐列表
    /// </summary>
    public const string FRIEND_RECOMMEND = "friends/rcmd";
    /// <summary>
    /// 按昵称查找好友
    /// </summary>
    public const string FRIEND_SEARCH = "friends/search";
    /// <summary>
    /// 好友申请列表
    /// </summary>
    public const string FRIEND_APLLY_LIST = "friends/applications";
    /// <summary>
    /// 申请添加好友
    /// </summary>
    public const string FRIEND_APLLY = "friends/apply";
    /// <summary>
    /// 同意申请
    /// </summary>
    public const string FRIEND_AGREE = "friends/agree";

    public const string FRIEND_DELETE = "friends/del";
    /// <summary>
    /// 忽略申请
    /// </summary>
    public const string FRIEND_IGNORE = "friends/ignore";

    /************************   福利  *************************/
    /// <summary>
    /// 每日签到列表
    /// </summary>
    public const string WELFARE_DAILY_LIST = "welfare/daily/list";
    /// <summary>
    /// 每日签到的签到动作
    /// </summary>
    public const string WELFARE_DAILY_CHECK = "welfare/daily/check";
    /// <summary>
    /// 大礼包签到
    /// </summary>
    public const string WELFARE_DAILY_AWARD = "welfare/daily/award";
    /// <summary>
    /// 七日签到
    /// </summary>
    public const string WELFARE_SEVEN_LIST = "welfare/seven/list";
    /// <summary>
    /// 七日签到领取奖励
    /// </summary>
    public const string WELFARE_SEVEN_CHECK = "welfare/seven/check";
    /// <summary>
    /// 每日福利
    /// </summary>
    public const string WELFARE_AD_LIST = "welfare/ad/list";
    /// <summary>
    /// 每日福利广告签到
    /// </summary>
    public const string WELFARE_AD_CHECK = "welfare/ad/check";
    /// <summary>
    /// 激活码
    /// </summary>
    public const string CODE_AWARD = "code/award";
    /// <summary>
    /// 辛灵任务列表
    /// </summary>
    public const string XINLING_LIST = "xinling/list";
    /// <summary>
    /// 辛灵任务完成
    /// </summary>
    public const string XINLING_CHECK = "xinling/check";
    #endregion


}




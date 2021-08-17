using System;
using System.Collections.Generic;

/// <summary>
/// Response infor. 数据传输返回结构
/// </summary>

public class SerializeJsonConf<T>
{
    public List<T> entity;

}



public class ResponseInfor<T>
{
    public Status status;
    /// <summary>
    /// 数据实体
    /// </summary>
    public T entity;

}

public class ResponseInforList<T>
{
    public Status status;
    /// <summary>
    /// 数据实体
    /// </summary>
    public List<T> entity;

}


/// <summary>
/// 状态
/// </summary>
///
[Serializable]
public class Status
{
    public static readonly bool OK = true;
    public static readonly bool ERROR = false;

    public int code;
    public string msg;
    public bool success;
    public long timeStamp;

}


[Serializable]
public class SignUpInfo
{
    public int playerId;
    public string token;
    public User user;

}

[Serializable]
public class User
{
    public int id;
    public string userName;
    public string email;
    public string nickName;
    public string avatar;
    //type 0 可以进入游戏,type 1 不能进入游戏，22-8点内,type 2 超过当天游戏时长
    public int type;
    //0:未实名 1:已shiming
    public int status;
    public List<StringList> roles;
    public int sex;
    public int age;
    public string limitHour;
}


/// <summary>
/// 玩家信息
/// </summary>
[Serializable]
public class Player
{
    public int id;
    public int user_id;
    public string name;
    public int type;
    public string user_name;
    public string nickname;
    public string avatar;
    public int server_id;
    public string birthday;
    public int character;
    public int energy;
    public int exp;
    public int level;
    public string description;

    public AvatarFrame title;
    /// <summary>
    /// 爱心
    /// </summary>
    public int love;
    /// <summary>
    /// 钻石
    /// </summary>
    public int diamond;
    public int gender;
    /// <summary>
    /// 背包大小
    /// </summary>
    public int bag_size;
    /// <summary>
    /// 卡牌库大小
    /// </summary>
    public int card_bag_size;
    public string prop;
    /// <summary>
    /// 卡牌列表
    /// </summary>
    public List<GameInitCardsConfig> card;
    public int vip_level;
    public int honor;
    public PlayerAttribute attribute;
    public PlayerAttrLevel attrLevel;

    public HomeActor homeActor;
    /// <summary>
    /// 闹钟状态，二进制 000->表示未领取、未开启、没有响铃
    /// </summary>
    public int alarm;
    //头像框
    public AvatarFrame avatar_frame;
    //充值
    public int charge_prop;
}


[Serializable]
public class HomeActor
{
    public int actor_id;
    public string skin;
    public string background;
}

[Serializable]
public class PlayerProperty
{
    /// <summary>
    /// 爱心
    /// </summary>
    public int love;
    /// <summary>
    /// 钻石
    /// </summary>
    public int diamond;

}

[Serializable]
public class PlayerAttribute
{

    /// <summary>
    /// 玩家等级经验值开始
    /// </summary>
    public int addMidNum;
    /// <summary>
    /// 玩家等级经验值结束
    /// </summary>
    public int outMidNum;
    /// <summary>
    /// 魅力
    /// </summary>
    public int charm;
    /// <summary>
    /// 环保
    /// </summary>
    public int evn;
    /// <summary>
    /// 智慧
    /// </summary>
    public int intell;
    /// <summary>
    /// 魔法
    /// </summary>
    public int mana;
}

[Serializable]
public class PlayerAttrLevel
{
    /// <summary>
    /// 魅力等级
    /// </summary>
    public int charmLv;
    /// <summary>
    /// 环保等级
    /// </summary>
    public int envLv;
    /// <summary>
    /// 智慧等级
    /// </summary>
    public int intellLv;
    /// <summary>
    /// 魔法等级
    /// </summary>
    public int manaLv;
}


/// <summary>
/// 占位数据
/// </summary>
public class HolderData
{

}

[Serializable]
public class IntList
{
    public int info;
}

[Serializable]
public class StringList
{
    public string info;
}

/// <summary>
/// 服务器列表
/// </summary>
[Serializable]
public class ServerList
{
    public int id;
    //服务区
    public string zone;
    public string server_name;
    public string server_address;
    /// <summary>
    /// 
    /// </summary>
    public string type;
    /// <summary>
    /// 1: 新服 2: 流畅 3: 火爆 4: 测试 5: 维护
    /// </summary>
    public int status;

}


/// <summary>
/// 公告
/// </summary>
[Serializable]
public class Announcement
{
    //类型 1公告  2服务协议
    public int xtype;
    public string title;
    public string body;
}

/// <summary>
/// 头像
/// </summary>
[Serializable]
public class Avatar
{
    /// <summary>
    /// 娃娃头像
    /// </summary>
    public static readonly int TYPE_DOLL = 1;
    /// <summary>
    ///角色头像
    /// </summary>
    public static readonly int TYPE_ROLE = 2;
    /// <summary>
    /// 时刻头像
    /// </summary>
    public static readonly int TYPE_MOMENT = 3;

    public int id;
    public int type;
    //0 未获得 1 获得
    public int isOwn;
}

/// <summary>
/// 角色列表
/// </summary>
[Serializable]
public class Role
{
    public int id;
    public string type;
    //0 未获得 1 获得
    public int own;
    public int actorFavorite;
    public string name;
}


/// <summary>
///  角色外观信息
/// </summary>
[Serializable]
public class PlayerActor
{
    public int id;
    /// <summary>
    /// Player ID
    /// </summary>
    public long player_id;

    public int actor_id;

    /// <summary>
    ///  好感度
    /// </summary>
    public string favour;

    /// <summary>
    ///  获得的皮肤ID 逗号分隔
    /// </summary>
    public string skin;

    /// <summary>
    ///  背景ID 逗号分隔
    /// </summary>
    public string background;
    /// <summary>
    /// 解锁制作物品 逗号分隔
    /// </summary>
    public string unlock_prop;

    public List<PlayerProp> playerProp;
    public float Favour
    {
        get
        {
            return float.Parse(favour);
        }
    }

}

/// <summary>
/// 道具信息
/// </summary>
[Serializable]
public class PlayerProp
{

    public long pack_id;

    public long player_id;

    public int prop_id;

    public int prop_type;

    public int prop_count;

    public int prop_lock;

    public int day;
}

/// <summary>
/// 人物信息和消耗
/// </summary>
[Serializable]
public class ActorProperty
{
    public PlayerProperty property;
    public PlayerActor playerActor;
}

/// <summary>
/// 礼物信息和消耗
/// </summary>
[Serializable]
public class PropMake
{
    public int charge_prop;
    public int love;
    public int diamond;
    public List<PlayerProp> playerProp;
    public PlayerMall playerMall;

}


[Serializable]
public class ServerTime
{
    public long currentTime;
    //type 0 可以进入游戏,type 1 不能进入游戏，22-8点内,type 2 超过当天游戏时长
    public int type;
    public string limitHour;
}

[Serializable]
public class ActorFragmentRespo
{

    /// <summary>
    /// 道具ID
    /// </summary>
    public int propId;

    /// <summary>
    /// 需要的碎片数量
    /// </summary>
    public int need;

    /// <summary>
    /// 玩家有的碎片数量
    /// </summary>
    public int own;

}

[Serializable]
public class ActorTask
{
    public bool isFinish;
    public Task task;
    public long end_time;
}

[Serializable]
public class Task
{
    public int task_id;
    public PlayerProp playerProp;
}

[Serializable]
public class RealName
{
    public string Result;
    public string Description;
    public User user;
}

[Serializable]
public class SwitchConfig
{
    public static readonly string KEY_REALNAME = "shimingrenzheng";
    public static readonly string KEY_RECHARGE = "chongzhixianzhi";
    public static readonly string KEY_TIME = "shijianxianzhi";
    public static readonly string KEY_FRIEND_LIMIT = "haoyoushuliang";

    public string key;
    public string value;
    public string name;
    public string description;
}

[Serializable]
public class ChannelSwitchConfig
{
    public static readonly string KEY_SEVEN = "seven";
    public static readonly string KEY_DAILY = "daily";
    public static readonly string KEY_AD = "ad";
    public static readonly string KEY_GUID = "guid";
    /// <summary>
    /// 礼包签到天数
    /// </summary>
    public static readonly string KEY_PACK = "qiandaoleiji";

    public string key;
    public int value;
    public string name;
    public string channel;
}

[Serializable]
public class PlayerStoryInfoExtra
{
    public string favour;
    public string level;
}

[Serializable]
public class StoryGameSave
{
    public static string DEFAULT = "default";
    public int nodeId;
    public string ckey;
    public string value;


    public StoryGameSave() { }

    public StoryGameSave(int nodeId, string ckey, string value)
    {
        this.nodeId = nodeId;
        this.ckey = ckey;
        this.value = value;
    }

    public bool IsDefault
    {
        get { return value.Equals(DEFAULT); }
    }
}

/// <summary>
/// 短信信息
/// </summary>
[Serializable]
public class SmsListIndex
{
    /// <summary>
    /// 有联系方式，但是没有开始聊天
    /// </summary>
    public const int TYPE_NOT_START = -1;
    /// <summary>
    /// 有没有新消息，保存上一条最新
    /// </summary>
    public const int TYPE_READED = 0;
    /// <summary>
    /// 有未读
    /// </summary>
    public const int TYPE_UNREAD = 1;
    /// <summary>
    /// 有已读，未完成
    /// </summary>
    public const int TYPE_READED_UNDONE = 2;
    /// <summary>
    /// 完成
    /// </summary>
    public const int TYPE_HAVE_DONE = 3;

    public int actor_id;
    public int sms_id;
    public int node_id;
    public int story_status;
    public string cell_node;
    public string pass_node;
    public string award_node;
    public string award;
    public string name;
    public string favour;
    public string update_time;
    public PlayerStoryInfoExtra extra;

}
/// <summary>
/// sms保存回调
/// </summary>
[Serializable]
public class SmsSave
{
    public int actorId;
    public int smsId;
    public int nodeId;
    public int storyStatus;
    public string cellNode;
    public string passNode;
    public string awardNode;
    public string award;
    public PlayerStoryInfoExtra extra;
}

/// <summary>
/// 历史记录返回
/// </summary>
[Serializable]
public class CellListPack
{
    public int actorId;
    public int smsId;
    public int storyStatus;
    public string pass_node;
}

/// <summary>
/// 通话背景记录
/// </summary>
[Serializable]
public class CellVoiceBgSaveInfo
{
    public static readonly string Key = "CellVoiceBg_";
    public int id;
    public int player_id;
    public int node_id;
    public string key;
    public string value;
}

/// <summary>
/// 购买包后返回
/// </summary>
[Serializable]
public class MessageBuy
{
    public int actor_id;
    public string call_list;
    public string message_list;
}

/// <summary>
/// 朋友圈回调
/// </summary>
[Serializable]
public class PlayerTimeline
{
    public int id;
    public int timeline_id;
    public string nodes;
    public string awards;
    public string name;
    public PlayerStoryInfoExtra extra;
    public string update_time;

}

/// <summary>
/// 推送回调    
/// </summary>
[Serializable]
public class PushInfo
{
    public List<SmsListIndex> message;
    public List<int> moment;
}

/// <summary>
/// 闹钟信息  
/// </summary>
[Serializable]
public class AlarmClockInfo
{
    public int actor_id;
    public string time_settings;
    public string time_settings_new;
    public int alarm;
    public HomeActor actor;
}

[Serializable]
public class Mail
{
    public MailPage page;
    public List<MailInfo> results;
    public int total;
}


[Serializable]
public class MailInfo
{
    public int mailId;
    public string from;
    public string title;
    public string content;
    public string quoteContent;
    public int type;
    public string detailUrl;
    public string extraData;
    public int status;
    public long createTime;
}

[Serializable]
public class MailPage
{
    public int page;
    public int limit;
    public int total;
}

//*************好友系统**************Begin
[Serializable]
public class Friend
{
    public int totalPages;
    public int totalFriends;
    public List<FriendInfo> list;
    /// <summary>
    /// 送礼情况表
    /// </summary>
    public List<GiftCondition> SentList;
    /// <summary>
    /// 收礼情况表
    /// </summary>
    public List<GiftCondition> ReceivedList;
}

[Serializable]
public class FriendInfo
{
    public int id;
    public int level;
    public string name;
    public string title;
    public string avatar;
    public int frame;
    public int presented;
    public int presentedMe;
    /// <summary>
    /// 0：未申请///1：已申请
    /// </summary>
    public bool applied;
    public int giftId;
}
[Serializable]
public class GiftCondition
{
    public int id;
    public int fromId;
    public int playerId;
    public string content;
    public string quoteContent;
    public int type;
    public string extraData;
    public int status;
    public long createTime;
}


//*************好友系统**************End



//*************福利系统**************Begin
[Serializable]
public class WelfareInfo
{
    public int curTimes;
    public int signedToday;

    public string bigAwardData;
    public string awardData;
    public string signed_list;
    /// <summary>
    /// 七日签到的礼物列表
    /// </summary>
    public List<string> awardDataList;
}

[Serializable]
public class SevenInfo
{
    public int loginTime;
    public string signedToday;
}

//*************福利系统**************End

[Serializable]
public class TimeChart
{
    public int moment_id;
}

[Serializable]
public class RoleTime
{
    public int actorId;
    public List<GameMomentConfig> configs;
}

[Serializable]
public class RankAll
{
    public List<PlayerRankInfo> top_three;
    public List<PlayerRankInfo> list;

}

[Serializable]
public class RankSingle
{
    public PlayerRankInfo player;
    public List<PlayerRankInfo> list;

}

[Serializable]
public class PlayerRankInfo
{
    public string attr;
    public int id;
    public int val;
    public string name;
    public string title;
    public string avatar;
    public int frame;
    public bool applied;
}

public class RankTopThree
{
    public RankType type;
    public List<PlayerRankInfo> top_three;

}


[Serializable]
public class PlayerInfo
{
    public OtherPlayer player;

    public List<PlayerRankInfo> attr;
    public List<PlayerRankInfo> favor;
    public List<PlayerRankInfo> moment;

}

[Serializable]
public class OtherPlayer
{
    public string name;
    public string avatar;
    public int level;

    public int frame;
    public int title;
    public int isFriend;
    public int isApply;
    public int playerId;
}

[Serializable]
public class AvatarFrame
{
    public int current;
    public List<int> own;
    public PlayerAttribute playerAttr;
}

[Serializable]
public class DailyTask
{
    public List<TaskInfo> dailyTasks;
    public int vitality;
    public string vitalityChests;
    public List<TaskInfo> tasksValue;
    public string doneTasks;
}

[Serializable]
public class TaskInfo
{
    public int missionId;
    public int progress;
    public int state;
}

[Serializable]
public class TaskAward
{
    public List<PlayerProp> playerProp;
    public int love;
    public int diamond;
    public string level;
    public int exp;

}
[Serializable]
public class TaskAwardInfo
{
    public DailyTask mission;
    public List<TaskAward> extra;
}

/// <summary>
///  红点
/// </summary>
[Serializable]
public class PlayerRedpoint
{

    /// <summary>
    /// 序号
    /// </summary>
    public int player_id;

    /// <summary>
    /// 可以升级的属性 0 charm 1 evn 2intell 3 mana
    /// </summary>
    public string attr_upgrade;

    /// <summary>
    /// 可以升级的娃娃
    /// </summary>
    public string doll_upgrade;

    /// <summary>
    /// 新称号
    /// </summary>
    public string title;

    /// <summary>
    /// 新框
    /// </summary>
    public string frame;

    /// <summary>
    /// 可以合成的娃娃
    /// </summary>
    public string doll_combine;

    /// <summary>
    /// 可以合成的背景
    /// </summary>
    public string background_combine;

    /// <summary>
    /// 可以有心愿礼物送的角色
    /// </summary>
    public string wish_gift;

    /// <summary>
    /// 新的邮件
    /// </summary>
    public int email;

    /// <summary>
    /// 新的成就任务
    /// </summary>
    public int d_mission;

    /// <summary>
    /// 新的每日任务
    /// </summary>
    public int a_mission;

    /// <summary>
    /// 新的消息，包括短信和语音
    /// </summary>
    public string message;

    /// <summary>
    /// 角色剧情
    /// </summary>
    public string story;

    /// <summary>
    /// 新的朋友圈
    /// </summary>
    public string timeline_public;
    /// <summary>
    /// 好友送东西
    /// </summary>
    public string friend_gift;
    /// <summary>
    /// 好友申请
    /// </summary>
    public string friend_applied;
    /// <summary>
    /// 福利
    /// </summary>
    public string welfare;
    /// <summary>
    /// 辛灵任务
    /// </summary>
    public string xinling;
    /// <summary>
    /// 任务未完成
    /// </summary>
    public string d_mission_un;

}
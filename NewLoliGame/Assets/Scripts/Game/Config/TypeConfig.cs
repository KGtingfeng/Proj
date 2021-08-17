using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏所有通用类型枚举定义
/// </summary>

public class TypeConfig
{

    public enum CIEM
    {
        /// <summary>
        /// 魅力
        /// </summary>
        CHARM = 1,
        /// <summary>
        /// 智力
        /// </summary>
        INTELL,
        /// <summary>
        /// 环保
        /// </summary>
        ENV,
        /// <summary>
        /// 魔法
        /// </summary>
        MAGIC
    }

    /// <summary>
    /// 消耗类型
    /// </summary>
    public enum Consume
    {
        /// <summary>
        /// 道具
        /// </summary>
        Item = 0,
        /// <summary>
        /// 钻石
        /// </summary>
        Diamond = 1,
        /// <summary>
        /// 星星
        /// </summary>
        Star = 2,
        /// <summary>
        /// 好感度
        /// </summary>
        Friendly = 3,
        /// <summary>
        /// 娃娃
        /// </summary>
        Doll = 4,
        /// <summary>
        ///角色
        /// </summary>
        Role = 5,
        /// <summary>
        /// 时刻
        /// </summary>
        Time = 6,
        /// <summary>
        /// 短信故事
        /// </summary>
        Message = 7,
        /// <summary>
        /// 朋友圈故事
        /// </summary>
        Story = 8,
        /// <summary>
        /// 手机故事
        /// </summary>
        Mobile = 9,
        /// <summary>
        /// 经验值
        /// </summary>
        EXP = 10,
        /// <summary>
        /// 视频
        /// </summary>
        Video = 11,
        /// <summary>
        /// 活跃
        /// </summary>
        Active = 12,
        /// <summary>
        /// 头像框
        /// </summary>
        AvatarFrame = 13,
        /// <summary>
        /// 称号
        /// </summary>
        Title = 14,
    }

    /// <summary>
    /// 剧情类别类型
    /// </summary>
    public enum StoyType
    {
        NONE = 0,
        /// <summary>
        /// 角色
        /// </summary>
        TYPE_ROLE = 1,
        /// <summary>
        /// 玩家
        /// </summary>
        TYPE_SELF = 2,
        /// <summary>
        /// 旁白
        /// </summary>
        TYPE_ASIDE = 3,
        /// <summary>
        /// 过场
        /// </summary>
        TYPE_TRANSITION = 4,
        /// <summary>
        /// 剧情
        /// </summary>
        TYPE_PLOT = 5,
        /// <summary>
        /// 属性
        /// </summary>
        TYPE_ATTRIBUTE = 6,
        /// <summary>
        /// 拼图
        /// </summary>
        TYPE_COMBINE = 7,

        /// <summary>
        /// 探索
        /// </summary>
        TYPE_SEARCH = 8,

        /// <summary>
        /// 找东西游戏
        /// </summary>
        TYPE_FIND = 9,
        /// <summary>
        /// 拼船游戏
        /// </summary>
        TYPE_PUZZLE = 10,
        /// <summary>
        /// 划船游戏
        /// </summary>
        TYPE_JUMP_SHIP = 11,
        /// <summary>
        /// 拼怀表游戏
        /// </summary>
        TYPE_PUZZLE_WATCH = 12,
        /// <summary>
        /// 播放视频
        /// </summary>
        TYPE_VIDEO = 13,
        /// <summary>
        /// 调怀表游戏
        /// </summary>
        TYPE_SELECT_TIME = 14,
        /// <summary>
        /// 拼四时钟游戏
        /// </summary>
        TYPE_PUZZLE_FOUR_CLOCK = 15,
        /// <summary>
        /// 填词游戏
        /// </summary>
        TYPE_WORD = 16,
        /// <summary>
        /// 接筋脉游戏
        /// </summary>
        TYPE_CONNECT_JINMAI = 18,
        /// <summary>
        /// 答题游戏
        /// </summary>
        TYPE_ANSWER = 17,
        /// <summary>
        /// 希腊填宝石游戏
        /// </summary>
        TYPE_GEM_FILLING = 19,
        /// <summary>
        /// 图片选择
        /// </summary>
        TYPE_IMAGE_SELECT = 20,
        /// <summary>
        /// 点击时间转盘
        /// </summary>
        TYPE_ClICK_CLOCK = 21,
        /// <summary>
        /// 找钟楼碎片
        /// </summary>
        TYPE_BELL_TOWER_FIND = 22,
        /// <summary>
        /// 角色背景放大
        /// </summary>
        TYPE_ROLE_BG_ENLARGE = 23,
        /// <summary>
        /// 玩家背景放大
        /// </summary>
        TYPE_SELF_BG_ENLARGE = 24,
        /// <summary>
        /// 旁白背景放大
        /// </summary>
        TYPE_ASIDE_BG_ENLARGE = 25,
        /// <summary>
        /// 过场背景放大
        /// </summary>
        TYPE_TRANSITION_BG_ENLARGE = 26,
        /// <summary>
        /// 拼钟楼碎片
        /// </summary>
        TYPE_BELL_TOWER_PUZZLE = 27,
        /// <summary>
        /// 废墟找碎片
        /// </summary>
        TYPE_RUINS_FIND = 28,
        /// <summary>
        /// 躲避蓝色光线
        /// </summary>
        TYPE_AVIOD_BLUE = 29,
        /// <summary>
        /// 穿越大楼
        /// </summary>
        TYPE_ACROSS_BUILDING = 30,
        /// <summary>
        /// 寻找大门碎片
        /// </summary>
        TYPE_FIND_DOOR_SP = 31,
        /// <summary>
        /// 防御曼朵拉
        /// </summary>
        TYPE_DEFEND_MDL = 32,
        /// <summary>
        /// 抵御曼朵拉
        /// </summary>
        TYPE_ATTACT_MDL = 33,
        /// <summary>
        /// 拼大门碎片
        /// </summary>
        TYPE_PUZZLE_DOOR = 34,
        /// <summary>
        /// 思思弹钢琴
        /// </summary>
        TYPE_SISI_PIANO = 35,
        /// <summary>
        /// 庞尊雷电
        /// </summary>
        TYPE_Thunder = 36,
    }


    public enum SMSType
    {
        /// <summary>
        /// 对话
        /// </summary>
        TYPE_Dialogue = 1,
        /// <summary>
        /// 选择
        /// </summary>
        TYPE_Select,
        /// <summary>
        /// 照片
        /// </summary>
        TYPE_Image,
        /// <summary>
        /// 红包
        /// </summary>
        TYPE_Hongbao,
        /// <summary>
        /// 红包开奖节点
        /// </summary>
        TYPE_AWARD = 6,

    }

    public enum PropType
    {
        //不显示
        NOT_SHOW = -1,
        //可使用
        CAN_USE = 1,
        //碎片
        FRAGMENT = 2,
        //礼物
        GIFT = 3,
        //娃娃
        DOLL = 5,
        //礼包
        PACK = 6,
        //背景
        BACKGROUND = 7,
        //时刻
        TIME = 8,
    }

}

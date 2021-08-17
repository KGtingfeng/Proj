using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstInfo
{
}

/// <summary>
/// 修改玩家信息
/// </summary>
public class ModifyPlayerInfo
{
    //姓名、昵称与消耗类型一致
    //修改玩家姓名
    public const int MODIFY_NAME = 4;
    //修改玩家昵称
    public const int MODIFY_NICKNAME = 5;
    //修改玩家生日
    public const int MODIFY_BIRTHDAY = 6;
    //修改玩家身份
    public const int MODIFY_CHARACTER = 7;
    //修改通讯录角色名
    public const int MODIFY_ADDRESSBOOK = 8;
}

/// <summary>
/// Player数据类型信息，便于修改刷新数据
/// </summary>
public class PlayerDataTypeInfo
{
    /// <summary>
    /// 货币
    /// </summary>
    public const int MONEY = 0;
    /// <summary>
    /// 修改信息
    /// </summary>
    public const int CAN_MODIFY= 1;
    /// <summary>
    /// 属性等级
    /// </summary>
    public const int ATTR_LEVEL = 2;
    /// <summary>
    /// 属性
    /// </summary>
    public const int ATTR_ATTRIBUTE= 3;
    /// <summary>
    /// 玩家信息（等级）
    /// </summary>
    public const int PLAYER_INFO = 4;
   /// <summary>
   /// 其它
   /// </summary>
    public const int OTHER = 5;
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class GameData
{
    public static long ServerTime;

    /// <summary>
    /// 时间差：本地与服务器上的
    /// </summary>
    public static long Delta_T;

    private static User _user;
    public static User User
    {
        get { return _user; }
        set { _user = value; }
    }

    private static Player player;
    public static Player Player
    {
        get { return player; }
        set { player = value; }
    }

    private static List<Avatar> avatars;
    public static List<Avatar> Avatars
    {
        get { return avatars; }
        set { avatars = value; }
    }

    private static List<SwitchConfig> switchConfigs;
    public static List<SwitchConfig> SwitchConfigs
    {
        get { return switchConfigs; }
        set { switchConfigs = value; }
    }

    public static string token = "";
    public static int playerId;

    public static GuiderInfoLinked guiderCurrent;

    public static bool isOpenGuider;
    public static bool isGuider;

    public static List<ChannelSwitchConfig> Configs;


    private static List<GameInitCardsConfig> _doll = new List<GameInitCardsConfig>();
    public static List<GameInitCardsConfig> Dolls
    {
        get
        {
            if (_doll.Count == 0)
                RefreshDolls();
            return _doll;
        }
        set { _doll = value; }
    }

    public static void RefreshDolls()
    {
        List<GameInitCardsConfig> gameInitCardsConfigs = new List<GameInitCardsConfig>();
        gameInitCardsConfigs.AddRange(DataUtil.GetDisplayDolls());
        //Debug.Log("before count:" + gameInitCardsConfigs.Count);
        //这里排序 把已经拥有的排序在前面 
        List<GameInitCardsConfig> tmp = new List<GameInitCardsConfig>();
        if (Player != null)
        {
            foreach (var doll in Player.card)
            {
                int index = gameInitCardsConfigs.FindIndex(a => a.card_id == doll.card_id);
                if (index >= 0)
                {
                    tmp.Add(doll);
                    gameInitCardsConfigs.RemoveAt(index);
                }
            }
            _doll.Clear();
            if (tmp.Count > 0)
                _doll.AddRange(tmp);
            if (gameInitCardsConfigs.Count > 0)
            {
                _doll.AddRange(gameInitCardsConfigs);
            }
            //处理角色界面，让拥有的角色中间显示
            if (Player.card != null && Player.card.Count < 2)
            {
                var element = _doll[0];
                _doll[0] = _doll[1];
                _doll[1] = element;
            }
        }
        else
        {
            _doll.AddRange(gameInitCardsConfigs);
        }
    }

    #region 按好感度排序的角色
    //角色获得
    private static List<GameInitCardsConfig> roles;
    public static List<GameInitCardsConfig> Roles
    {
        get { return roles; }
        set { roles = value; }
    }

    //保存当前已拥有数据
    static List<Role> _ownRoleList;
    public static List<Role> OwnRoleList
    {
        get
        {
            return _ownRoleList;
        }
        set { _ownRoleList = value; }
    }

    /// <summary>
    /// 刷新RoleList数据
    /// </summary>
    /// <param name="roleList">仅包含已拥有数据</param>
    public static void RefreshRoleListSort(List<Role> roleList)
    {
        //List<GameInitCardsConfig> allRoleList = DataUtil.GetGameInitCardsConfigs();
        /*************  临时  ****************/
        List<GameInitCardsConfig> allRoleList = JsonConfig.GameInitCardsConfigs.Where(a => a.card_id == 9 || a.card_id == 11 
        || a.card_id == 28 || a.card_id == 10 || a.card_id == 22 || a.card_id == 18).ToList();


        List<GameInitCardsConfig> tmpOwnRoleList = new List<GameInitCardsConfig>();
        roleList.Sort(delegate (Role roleA, Role roleB)
        {
            return roleA.id.CompareTo(roleB.id);
        });

        OwnRoleList = roleList;
        List<GameInitCardsConfig> ownCards = new List<GameInitCardsConfig>();
        foreach (Role r in OwnRoleList)
        {
            GameInitCardsConfig cardConfig = allRoleList.Find(a => a.card_id == r.id);
            if (cardConfig != null)
            {
                ownCards.Add(cardConfig);
            }
        }
        //GameInitCardsConfig gameInitCardsConfig = null;
        //foreach (var role in roleList)
        //{
        //    if (role.id > 27)
        //        continue;

        //    gameInitCardsConfig = DataUtil.GetGameInitCard(role.id);
        //    if (gameInitCardsConfig != null)
        //    {
        //        tmpOwnRoleList.Add(gameInitCardsConfig);
        //        allRoleList.Remove(gameInitCardsConfig);
        //    }
        //}

        Roles = ownCards;
    }
    #endregion

}

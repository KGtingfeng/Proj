using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using FairyGUI;

public class InteractiveDataMgr
{

    private static InteractiveDataMgr _interactiveDataMgr;
    public static InteractiveDataMgr ins
    {
        get
        {
            if (_interactiveDataMgr == null)
                _interactiveDataMgr = new InteractiveDataMgr();
            return _interactiveDataMgr;
        }
    }
    /// <summary>
    /// 选中角色（当前展示角色）
    /// </summary>
    GameInitCardsConfig selectRoleCardConfig;
    public GameInitCardsConfig SelectRoleCardConfig
    {
        get { return selectRoleCardConfig; }
        set { selectRoleCardConfig = value; }
    }

    private InteractiveDataMgr() { }

    PlayerActor _currentPlayerActor;
    /// <summary>
    /// 当前互动角色
    /// </summary>
    public PlayerActor CurrentPlayerActor
    {
        get
        {
            if (_currentPlayerActor == null)
                Debug.LogError("CurrentPlayerActor is NULL");
            return _currentPlayerActor;
        }
        set { _currentPlayerActor = value; }
    }

    /// <summary>
    /// 拥有道具列表
    /// </summary>
    List<PlayerProp> playerProps;
    public List<PlayerProp> PlayerProps
    {
        get
        {
            return playerProps;
        }
        set { playerProps = value; }
    }



    public List<int> ownSkins = new List<int>();
    public List<int> ownBackgrounds = new List<int>();
    public void RefreshSkin()
    {
        ownSkins.Clear();
        ownBackgrounds.Clear();
        string[] skinStr = _currentPlayerActor.skin.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        if (skinStr.Length > 0)
        {
            foreach (string skin in skinStr)
            {
                ownSkins.Add(int.Parse(skin));
            }
        }
        skinStr = _currentPlayerActor.background.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        if (skinStr.Length > 0)
        {
            foreach (string skin in skinStr)
            {
                ownBackgrounds.Add(int.Parse(skin));
            }
        }
    }

    int _backgroundId;
    public int BackgroundId { get; set; }


    public List<Role> OwnRoleList
    {
        get { return GameData.OwnRoleList; }
        set { GameData.OwnRoleList = value; }
    }

    public void RefreshRoleList(Role role)
    {
        if (OwnRoleList == null)
            OwnRoleList = new List<Role>();
        if (role == null)
            return;
        Role tmpRole = OwnRoleList.Where(a => a.id == role.id).FirstOrDefault();
        if (tmpRole != null)
            OwnRoleList.Remove(tmpRole);
        OwnRoleList.Add(role);
        GameData.RefreshRoleListSort(GameData.OwnRoleList);
    }
   
    /// <summary>
    /// 赠送礼物ID
    /// </summary>
    public int presentProp;

    //赠送礼物说的话
    public string GetTalkText()
    {
        GameActorReactConfig gameActor = JsonConfig.GameActorReactConfigs.Find(a => a.actor_id == CurrentPlayerActor.actor_id);
        return DataUtil.ReplaceCharacterWithStarts(gameActor.GetCondition(GetFavor()));
    }

    //该物品的好感
    public int GetFavor()
    {
        int favor;
        GameFavourPropConfig gameFavourProp = JsonConfig.GameFavourPropConfigs.Find(a => a.prop_id == presentProp);
        gameFavourProp.FavorDic.TryGetValue(CurrentPlayerActor.actor_id, out favor);
        return favor;
    }

}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class DataUtil
{
    /// <summary>
    /// 是否拥有该卡牌
    /// </summary>
    /// <returns><c>true</c>, if own doll was ised, <c>false</c> otherwise.</returns>
    /// <param name="id">Identifier.</param>
    public static bool IsOwnDoll(int id)
    {
        if (GameData.Player == null || GameData.Player.card == null || GameData.Player.card.Count == 0)
            return false;

        GameInitCardsConfig doll = GameData.Player.card.Where(a => a.card_id == id).FirstOrDefault();
        if (doll == null)
            return false;
        return true;
    }

    /// <summary>
    /// 找到doll卡牌
    /// </summary>
    /// <returns>The doll body image.</returns>
    /// <param name="id">Identifier.</param>
    public static string GetDollBodyImg(int id)
    {
        string url = "";
        if (!RoleConfig.bodyUrl.TryGetValue(id, out url))
        {
            url = "cha_yeluoli";
            UnityEngine.Debug.Log("not find body img " + id);

        }
        return url;
    }


    /// <summary>
    /// 找到卡牌的位置
    /// </summary>
    /// <returns>The doll index.</returns>
    /// <param name="cardId">Card identifier.</param>
    public static int FindDollIndex(int cardId)
    {
        int index = GameData.Dolls.FindIndex(a => a.card_id == cardId);
        return index;
    }

    /// <summary>
    /// 查找职业
    /// </summary>
    /// <returns>The role profession.</returns>
    /// <param name="index">Index.</param>
    public static string FindRoleProfession(int index)
    {
        string profession = "";
        if (!RoleConfig.roleProfessons.TryGetValue(index, out profession))
        {
            profession = "其他";
            UnityEngine.Debug.Log("not profession " + profession);

        }

        return profession;
    }

    /// <summary>
    /// 查找玩家属性信息
    /// </summary>
    /// <returns>The player attr level config.</returns>
    /// <param name="level">Level.</param>
    public static PlayerAttrLevelConfig GetPlayerAttrLevelConfig(int level)
    {
        return JsonConfig.PlayerAttrLevelConfigs.FirstOrDefault(a => a.level_id == level);

    }

    public static PlayerLevelConfig GetPlayerLevelConfig()
    {
        return JsonConfig.PlayerLevelConfigs.Where(a => a.level_id == GameData.Player.level).FirstOrDefault();
    }

    /// <summary>
    /// 获得可显示剧情角色
    /// </summary>
    /// <returns></returns>
    public static List<GameInitCardsConfig> GetGameInitCardsConfigs()
    {
        return JsonConfig.GameInitCardsConfigs.Where(a => a.type == GameInitCardsConfig.TYPE_ROLE).ToList();
    }


    /// <summary>
    /// 根据类型获取可以绑定的娃娃
    /// </summary>
    /// <returns>The bind dolls.</returns>
    public static List<GameInitCardsConfig> GetBindDolls()
    {
        return JsonConfig.GameInitCardsConfigs.Where(a => a.type == GameInitCardsConfig.TYPE_DOLL).ToList();
    }

    /// <summary>
    /// 获取可显示娃娃
    /// </summary>
    /// <returns>The display dolls.</returns>
    public static List<GameInitCardsConfig> GetDisplayDolls()
    {
        return JsonConfig.GameInitCardsConfigs.Where(a => a.status == true && a.type == GameInitCardsConfig.TYPE_DOLL).ToList();
    }

    /// <summary>
    /// 找到娃娃基础配置
    /// </summary>
    /// <returns>The game init cards.</returns>
    /// <param name="cardId">Card identifier.</param>
    public static GameInitCardsConfig GetGameInitCard(int cardId)
    {
        return JsonConfig.GameInitCardsConfigs.Where(a => a.card_id == cardId).FirstOrDefault();
    }

    /// <summary>
    /// 找到角色等级配置
    /// </summary>
    /// <returns>The card level config.</returns>
    /// <param name="cardId">Card identifier.</param>
    /// <param name="level">Level.</param>
    public static GameCardLevelConfig GetCardLevelConfig(int cardId, int level)
    {
        return JsonConfig.GameCardLevelConfigs.Where(a => a.level == level).FirstOrDefault();
    }


    /// <summary>
    /// 获取章节列表
    /// </summary>
    /// <returns>The chapters.</returns>
    /// <param name="actor_id">Actor identifier.</param>
    public static List<GameChapterConfig> GetChapters(int actor_id)
    {
        return JsonConfig.GameChapterConfigs.Where(a => a.actor_id == actor_id).ToList();
    }

    /// <summary>
    /// 查找具体章节
    /// </summary>
    /// <returns>The chapter.</returns>
    /// <param name="actor_id">Actor identifier.</param>
    /// <param name="chapterId">Chapter identifier.</param>
    public static GameChapterConfig GetChapter(int actor_id, int chapterId)
    {
        return JsonConfig.GameChapterConfigs.FirstOrDefault(a => a.actor_id == actor_id && a.id == chapterId);
    }

    /// <summary>
    /// 获取剧情节点
    /// </summary>
    /// <returns>The node config.</returns>
    /// <param name="chapterId">Chapter identifier.</param>
    /// <param name="id">Identifier.</param>
    public static GameNodeConfig GetNodeConfig(int chapterId, int id)
    {
        GameNodeConfig gameNode = JsonConfig.GameNodeConfigs.FirstOrDefault(a => a.chapter_id == chapterId && a.id == id);
        if (gameNode == null)
            return null;
        //避免引用问题
        GameNodeConfig gameNodeConfig = new GameNodeConfig();
        gameNodeConfig.id = gameNode.id;
        gameNodeConfig.awards = gameNode.awards;
        gameNodeConfig.chapter_id = gameNode.chapter_id;
        gameNodeConfig.isPass = gameNode.isPass;
        gameNodeConfig.point_id = gameNode.point_id;
        return gameNodeConfig;
    }

    /// <summary>
    /// 获取剧情剧情节点id
    /// </summary>
    /// <returns>The point config.</returns>
    /// <param name="id">Identifier.</param>
    public static GamePointConfig GetPointConfig(int id)
    {
        return JsonConfig.GamePointConfigs.Where(a => a.id == id).FirstOrDefault();
    }


    /// <summary>
    /// 获取玩家是否满足属性值
    /// </summary>
    /// <returns><c>true</c>, if attribute was enoughed, <c>false</c> otherwise.</returns>
    /// <param name="type">Type.</param>
    /// <param name="num">Number.</param>
    public static bool GetEnoughAttributeResult(int type, int num)
    {
        bool isEnough = false;
        switch (type)
        {
            //魅力
            case (int)TypeConfig.CIEM.CHARM:
                isEnough = num <= GameData.Player.attribute.charm;
                break;
            //智力
            case (int)TypeConfig.CIEM.INTELL:
                isEnough = num <= GameData.Player.attribute.intell;
                break;
            //环保
            case (int)TypeConfig.CIEM.ENV:
                isEnough = num <= GameData.Player.attribute.evn;
                break;
            //魔法
            case (int)TypeConfig.CIEM.MAGIC:
                isEnough = num <= GameData.Player.attribute.mana;

                break;
            default:
                break;
        }


        return isEnough;
    }


    /// <summary>
    ///  该接口就是获取玩家所有的对白节点 纯对白
    /// </summary>
    /// <returns>The chapter pass nodes for dialog.</returns>
    /// <param name="startNode">Start node.</param>
    /// <param name="nodes">Nodes.</param>
    public static List<int> GetChapterPassNodesForDialog(int startNode, List<int> nodes)
    {
        //当前的回放的终点
        int currentNode = nodes[nodes.Count - 1];
        return GetAllPoints(startNode, currentNode, nodes);

    }

    static List<int> GetAllPoints(int startNode, int endNode, List<int> orders)
    {
        //Debug.Log("start  form " + startNode + "  to: " + endNode);
        List<int> allNode = new List<int>();
        //还没有到结束带你
        if (startNode > 0 && startNode <= endNode)
        {
            GamePointConfig gamePointConfig = GetPointConfig(startNode);
            switch (gamePointConfig.type)
            {
                case (int)TypeConfig.StoyType.TYPE_ROLE:
                    allNode.Add(startNode);
                    allNode.AddRange(GetAllPoints(gamePointConfig.point1, endNode, orders));
                    break;
                case (int)TypeConfig.StoyType.TYPE_SELF:
                    allNode.Add(startNode);
                    allNode.AddRange(GetAllPoints(gamePointConfig.point1, endNode, orders));
                    break;
                case (int)TypeConfig.StoyType.TYPE_ASIDE:
                    allNode.Add(startNode);
                    allNode.AddRange(GetAllPoints(gamePointConfig.point1, endNode, orders));
                    break;
                case (int)TypeConfig.StoyType.TYPE_TRANSITION:
                    allNode.Add(startNode);
                    allNode.AddRange(GetAllPoints(gamePointConfig.point1, endNode, orders));
                    break;
                case (int)TypeConfig.StoyType.TYPE_ROLE_BG_ENLARGE:
                    allNode.Add(startNode);
                    allNode.AddRange(GetAllPoints(gamePointConfig.point1, endNode, orders));
                    break;
                case (int)TypeConfig.StoyType.TYPE_SELF_BG_ENLARGE:
                    allNode.Add(startNode);
                    allNode.AddRange(GetAllPoints(gamePointConfig.point1, endNode, orders));
                    break;
                case (int)TypeConfig.StoyType.TYPE_ASIDE_BG_ENLARGE:
                    allNode.Add(startNode);
                    allNode.AddRange(GetAllPoints(gamePointConfig.point1, endNode, orders));
                    break;
                case (int)TypeConfig.StoyType.TYPE_TRANSITION_BG_ENLARGE:
                    allNode.Add(startNode);
                    allNode.AddRange(GetAllPoints(gamePointConfig.point1, endNode, orders));
                    break;

                //需要特殊处理
                case (int)TypeConfig.StoyType.TYPE_PLOT:
                    {
                        DoSelectPoint(endNode, orders, allNode, gamePointConfig);
                    }
                    break;
                //需要特殊处理
                case (int)TypeConfig.StoyType.TYPE_ATTRIBUTE:
                    DoSelectPoint(endNode, orders, allNode, gamePointConfig);
                    break;
                //需要特殊处理
                case (int)TypeConfig.StoyType.TYPE_VIDEO:

                    DoSelectPoint(endNode, orders, allNode, gamePointConfig);
                    break;
                //需要特殊处理
                case (int)TypeConfig.StoyType.TYPE_SEARCH:
                    DoSelectPoint(endNode, orders, allNode, gamePointConfig);
                    break;
                    //小游戏
                default:
                    DoSelectPoint(endNode, orders, allNode, gamePointConfig);
                    break;
            }

        }
        //结束
        else
        {
            return allNode;
        }

        return allNode;

    }

    private static void DoSelectPoint(int endNode, List<int> orders, List<int> allNode, GamePointConfig gamePointConfig)
    {
        List<int> tempNodes = new List<int>();
        tempNodes.Add(gamePointConfig.point1);
        if (!string.IsNullOrEmpty(gamePointConfig.content2))
        {
            tempNodes.Add(gamePointConfig.point2);
        }
        if (!string.IsNullOrEmpty(gamePointConfig.content3))
        {
            tempNodes.Add(gamePointConfig.point3);
        }
        //该剧情的总共选择点 从order中找到玩家选择的点
        foreach (var selectNode in tempNodes)
        {
            //UnityEngine.Debug.Log("find " + selectNode);
            //如果包括这个点 表示选择的就是这个点
            if (orders.Contains(selectNode))
            {
                //UnityEngine.Debug.Log("find before " + selectNode +  " count: " + allNode.Count);
                //allNode.AddRange(GetAllPoints(gamePointConfig.point1, endNode, orders));
                allNode.AddRange(GetAllPoints(selectNode, endNode, orders));
                //UnityEngine.Debug.Log("find after " + selectNode + " count: " + allNode.Count);
                break;
            }
        }
    }


    /// <summary>
    /// ****替换为用户昵称
    /// </summary>
    /// <returns>The character with starts.</returns>
    /// <param name="context">Context.</param>
    public static string ReplaceCharacterWithStarts(string context)
    {
        return context.Replace("****", GameData.Player.nickname);
    }


    /// <summary>
    /// 获取时刻
    /// </summary>
    /// <returns>The game moment config.</returns>
    /// <param name="roleId">Role identifier.</param>
    public static GameMomentConfig GetGameMomentConfig(int roleId)
    {
        return JsonConfig.GameMomentConfigs.FirstOrDefault(a => a.moment_id == roleId);
    }

    /// <summary>
    /// 查找消耗类型
    /// </summary>
    /// <returns>The consume config.</returns>
    /// <param name="type">Type.</param>
    public static GameConsumeConfig GetConsumeConfig(int type)
    {
        return JsonConfig.GameConsumeConfigs.FirstOrDefault(a => a.id == type);

    }

    /// <summary>
    /// 获取主界面角色声音 表情 文字
    /// </summary>
    /// <returns>The role main tips config.</returns>
    /// <param name="id">Identifier.</param>
    public static GameRoleMainTipsConfig GetRoleMainTipsConfig(int id)
    {
        return JsonConfig.GameRoleMainTipsConfigs.FirstOrDefault(a => a.id == id);
    }


    public static string GetRandomName()
    {
        string tmpName = "";
        if (SurnnameList != null && NameList != null)
        {
            int index = Random.Range(0, SurnnameList.Count);
            tmpName = SurnnameList[index].content;
            index = Random.Range(0, NameList.Count);
            tmpName += NameList[index].content;
        }
        return tmpName;
    }

    ////姓氏surnnameList
    static List<GameRandomConfig> SurnnameList
    {
        get { return JsonConfig.GameRandomConfigs.FindAll(a => a.type == GameRandomConfig.SURNNAME); }
    }
    ////名字 nameList
    static List<GameRandomConfig> NameList
    {
        get { return JsonConfig.GameRandomConfigs.FindAll(a => a.type == GameRandomConfig.NAME); }
    }


    /// <summary>
    /// 根据类型判断
    /// </summary>
    /// <returns>false 付费更改  true </returns>
    public static bool isCanModify(int consumeType)
    {
        string[] consumeStatus = GameData.Player.description.Split(',');
        // 名字、昵称、生日、头像 顺序对应
        //consumeStatus[index]为0时，免费修改，否则有消耗
        if (consumeStatus.Length < 4)
            return false;
        switch (consumeType)
        {
            case ModifyPlayerInfo.MODIFY_NAME:
                return 0 == int.Parse(consumeStatus[0]);
            case ModifyPlayerInfo.MODIFY_NICKNAME:
                return 0 == int.Parse(consumeStatus[1]);
            case ModifyPlayerInfo.MODIFY_BIRTHDAY:
                return 0 == int.Parse(consumeStatus[2]);
            case ModifyPlayerInfo.MODIFY_CHARACTER:
                return 0 == int.Parse(consumeStatus[3]);
        }
        return true;
    }

    /// <summary>
    /// 获得商品配置信息 
    /// </summary>
    /// <param name="prop_id"></param>
    /// <returns></returns>
    public static GameMallConfig GetGameMallConfig(int mall_id)
    {
        return JsonConfig.GameMallConfigs.FirstOrDefault(a => a.mall_id == mall_id);
    }
    /// <summary>
    /// 获得道具信息
    /// </summary>
    /// <param name="prop_id"></param>
    /// <returns></returns>
    public static GamePropConfig GetGamePropConfig(int prop_id)
    {
        return JsonConfig.GamePropConfigs.FirstOrDefault(a => a.prop_id == prop_id);
    }

    /// <summary>
    /// 获得皮肤配置信息
    /// </summary>
    /// <param name="skinId"></param>
    /// <returns></returns>
    public static GameActorSkinConfig GetGameActorSkinConfig(int skinId)
    {
        return JsonConfig.GameActorSkinConfigs.FirstOrDefault(a => a.id == skinId);
    }

    #region  通话
    //获取节点奖励
    public static GameSmsNodeConfig GetGameSmsNodeConfig(int id)
    {
        GameSmsNodeConfig gameSmsNode = JsonConfig.GameSmsNodeConfigs.FirstOrDefault(a => a.id == id);
        if (gameSmsNode == null)
        {
            Debug.Log("GameSmsNodeConfig Node id " + id + "  is  null!!  ");
            return null;
        }
        //避免引用问题
        GameSmsNodeConfig n = new GameSmsNodeConfig();
        n.id = gameSmsNode.id;
        n.sms_id = gameSmsNode.sms_id;
        n.awards = gameSmsNode.awards;
        n.point_id = gameSmsNode.point_id;

        return gameSmsNode;
    }

    /// <summary>
    /// 获取剧情节点id
    /// </summary>
    /// <returns>The point config.</returns>
    /// <param name="id">Identifier.</param>
    public static GameSmsPointConfig GetGameSmsPointConfig(int id)
    {
        GameSmsPointConfig gameSmsPoint = JsonConfig.GameSmsPointConfigs.FirstOrDefault(a => a.id == id);
        if (gameSmsPoint == null)
        {
            Debug.Log("GameSmsPointConfig Point id " + id + "  is  null!!  ");
            return null;
        }
        //避免引用问题
        GameSmsPointConfig gameSmsPointConfig = new GameSmsPointConfig();
        gameSmsPointConfig.id = gameSmsPoint.id;
        gameSmsPointConfig.type = gameSmsPoint.type;
        gameSmsPointConfig.title = gameSmsPoint.title;
        gameSmsPointConfig.card_id = gameSmsPoint.card_id;
        gameSmsPointConfig.content1 = gameSmsPoint.content1;
        gameSmsPointConfig.content2 = gameSmsPoint.content2;
        gameSmsPointConfig.content3 = gameSmsPoint.content3;
        gameSmsPointConfig.point1 = gameSmsPoint.point1;
        gameSmsPointConfig.point2 = gameSmsPoint.point2;
        gameSmsPointConfig.point3 = gameSmsPoint.point3;
        return gameSmsPointConfig;

    }

    public static GameCellSmsConfig GetGameCellSmsConfig(int smsId)
    {
        return JsonConfig.GameCellSmsConfigs.Where(a => a.id == smsId).FirstOrDefault();
    }
    #endregion

    public static GameTimelineNodeConfig GetGameTimelineNodeConfig(int id)
    {
        GameTimelineNodeConfig gameNode = JsonConfig.GameTimelineNodeConfigs.FirstOrDefault(a => a.id == id);
        if (gameNode == null)
        {
            Debug.Log("GameTimelineNodeConfig Node id " + id + "  is  null!!  ");
            return null;
        }
        //避免引用问题
        GameTimelineNodeConfig node = new GameTimelineNodeConfig();
        node.id = gameNode.id;
        node.awards = gameNode.awards;
        node.point_id = gameNode.point_id;
        node.sms_id = gameNode.sms_id;
        return node;
    }

    public static GameTimelinePointConfig GetGameTimelinePointConfig(int id)
    {
        GameTimelinePointConfig gameSmsPoint = JsonConfig.GameTimelinePointConfigs.FirstOrDefault(a => a.id == id);
        if (gameSmsPoint == null)
        {
            Debug.Log("GameTimelinePointConfig Point id " + id + "  is  null!!  ");
            return null;
        }
        //避免引用问题
        GameTimelinePointConfig gamePoint = new GameTimelinePointConfig();
        gamePoint.id = gameSmsPoint.id;
        gamePoint.type = gameSmsPoint.type;
        gamePoint.title = gameSmsPoint.title;
        gamePoint.card_id = gameSmsPoint.card_id;
        gamePoint.content1 = gameSmsPoint.content1;
        gamePoint.content2 = gameSmsPoint.content2;
        gamePoint.content3 = gameSmsPoint.content3;
        gamePoint.point1 = gameSmsPoint.point1;
        gamePoint.point2 = gameSmsPoint.point2;
        gamePoint.point3 = gameSmsPoint.point3;
        return gamePoint;
    }

    public static GameCellTimelineConfig GetGameCellTimelineConfig(int smsId)
    {
        return JsonConfig.GameCellTimelineConfigs.Where(a => a.id == smsId).FirstOrDefault();
    }
}

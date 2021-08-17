using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Json config. json配置文件信息
/// </summary>

public class JsonConfig
{

    private static List<PlayerAttrLevelConfig> playerAttrLevelConfigs;
    public static List<PlayerAttrLevelConfig> PlayerAttrLevelConfigs
    {
        get
        {
            if (playerAttrLevelConfigs == null)
            {
                playerAttrLevelConfigs = ResourceUtil.LoadJsonFile<PlayerAttrLevelConfig>();
            }
            return playerAttrLevelConfigs;
        }
    }

    private static List<GameCardLevelConfig> gameCardLevelConfigs;
    public static List<GameCardLevelConfig> GameCardLevelConfigs
    {
        get
        {
            if (gameCardLevelConfigs == null)
            {
                gameCardLevelConfigs = ResourceUtil.LoadJsonFile<GameCardLevelConfig>();
            }
            return gameCardLevelConfigs;
        }
    }

    private static List<GameConsumeConfig> gameConsumes;
    public static List<GameConsumeConfig> GameConsumeConfigs
    {
        get
        {
            if (gameConsumes == null)
            {
                gameConsumes = ResourceUtil.LoadJsonFile<GameConsumeConfig>();
            }
            return gameConsumes;
        }
    }

    private static List<GameChapterConfig> gameChapterConfigs;
    public static List<GameChapterConfig> GameChapterConfigs
    {
        get
        {
            if (gameChapterConfigs == null)
            {
                gameChapterConfigs = ResourceUtil.LoadJsonFile<GameChapterConfig>();
            }
            return gameChapterConfigs;
        }
    }

    private static List<GameInitCardsConfig> gameInitCardsConfigs;
    public static List<GameInitCardsConfig> GameInitCardsConfigs
    {
        get
        {
            if (gameInitCardsConfigs == null)
            {
                gameInitCardsConfigs = ResourceUtil.LoadJsonFile<GameInitCardsConfig>();
            }
            return gameInitCardsConfigs;
        }
    }

    private static List<GameCardsSkinConfig> gameCardsSkinConfigs;
    public static List<GameCardsSkinConfig> GameCardsSkinConfigs
    {
        get
        {
            if (gameCardsSkinConfigs == null)
            {
                gameCardsSkinConfigs = ResourceUtil.LoadJsonFile<GameCardsSkinConfig>();
            }
            return gameCardsSkinConfigs;
        }
    }

    private static List<GameActorReactConfig> gameActorReactConfigs;
    public static List<GameActorReactConfig> GameActorReactConfigs
    {
        get
        {
            if (gameActorReactConfigs == null)
            {
                gameActorReactConfigs = ResourceUtil.LoadJsonFile<GameActorReactConfig>();
            }
            return gameActorReactConfigs;
        }
    }

    private static List<GameActorSkinConfig> gameActorSkinConfigs;
    public static List<GameActorSkinConfig> GameActorSkinConfigs
    {
        get
        {
            if (gameActorSkinConfigs == null)
            {
                gameActorSkinConfigs = ResourceUtil.LoadJsonFile<GameActorSkinConfig>();
            }
            return gameActorSkinConfigs;
        }
    }

    private static List<GamePropFragmentConfig> gamePropFragmentConfigs;
    public static List<GamePropFragmentConfig> GamePropFragmentConfigs
    {
        get
        {
            if (gamePropFragmentConfigs == null)
            {
                gamePropFragmentConfigs = ResourceUtil.LoadJsonFile<GamePropFragmentConfig>();
            }
            return gamePropFragmentConfigs;
        }
    }

    private static List<GameFavourPropConfig> gameFavourPropConfigs;
    public static List<GameFavourPropConfig> GameFavourPropConfigs
    {
        get
        {
            if (gameFavourPropConfigs == null)
            {
                gameFavourPropConfigs = ResourceUtil.LoadJsonFile<GameFavourPropConfig>();
            }
            return gameFavourPropConfigs;
        }
    }

    private static List<GameFavourTitleConfig> gameFavourTitleConfigs;
    public static List<GameFavourTitleConfig> GameFavourTitleConfigs
    {
        get
        {
            if (gameFavourTitleConfigs == null)
            {
                gameFavourTitleConfigs = ResourceUtil.LoadJsonFile<GameFavourTitleConfig>();
            }
            return gameFavourTitleConfigs;
        }
    }

    private static List<GamePropConfig> gamePropConfigs;
    public static List<GamePropConfig> GamePropConfigs
    {
        get
        {
            if (gamePropConfigs == null)
            {
                gamePropConfigs = ResourceUtil.LoadJsonFile<GamePropConfig>();
            }
            return gamePropConfigs;
        }
    }

    private static List<PlayerLevelConfig> playerLevelConfigs;
    public static List<PlayerLevelConfig> PlayerLevelConfigs
    {
        get
        {
            if (playerLevelConfigs == null)
            {
                playerLevelConfigs = ResourceUtil.LoadJsonFile<PlayerLevelConfig>();
            }
            return playerLevelConfigs;
        }
    }

    private static List<GameMomentConfig> gameMoments;
    public static List<GameMomentConfig> GameMomentConfigs
    {
        get
        {
            if (gameMoments == null)
            {
                gameMoments = ResourceUtil.LoadJsonFile<GameMomentConfig>();
            }
            return gameMoments;
        }
    }

    private static List<GameNodeConfig> gameNodeConfigs;
    public static List<GameNodeConfig> GameNodeConfigs
    {
        get
        {
            if (gameNodeConfigs == null)
            {
                gameNodeConfigs = ResourceUtil.LoadJsonFile<GameNodeConfig>();
            }
            return gameNodeConfigs;
        }
    }

    private static List<GamePointConfig> gamePointConfigs;
    public static List<GamePointConfig> GamePointConfigs
    {
        get
        {
            if (gamePointConfigs == null)
            {
                gamePointConfigs = ResourceUtil.LoadJsonFile<GamePointConfig>();
            }
            return gamePointConfigs;
        }
    }

    private static List<GameRandomConfig> gameRandomConfigs;
    public static List<GameRandomConfig> GameRandomConfigs
    {
        get
        {
            if (gameRandomConfigs == null)
            {
                gameRandomConfigs = ResourceUtil.LoadJsonFile<GameRandomConfig>();
            }
            return gameRandomConfigs;
        }
    }


    private static List<GameStoryConfig> gameStoryConfigs;
    public static List<GameStoryConfig> GameStoryConfigs
    {
        get
        {
            if (gameStoryConfigs == null)
            {
                gameStoryConfigs = ResourceUtil.LoadJsonFile<GameStoryConfig>();
            }
            return gameStoryConfigs;
        }

    }


    private static List<GameRoleMainTipsConfig> gameRoleMainTips;
    public static List<GameRoleMainTipsConfig> GameRoleMainTipsConfigs
    {
        get
        {
            if (gameRoleMainTips == null)
            {
                gameRoleMainTips = ResourceUtil.LoadJsonFile<GameRoleMainTipsConfig>();
            }
            return gameRoleMainTips;
        }
    }

    private static List<GameMallConfig> gameMallConfigs;
    public static List<GameMallConfig> GameMallConfigs
    {
        get
        {
            if (gameMallConfigs == null)
            {
                gameMallConfigs = ResourceUtil.LoadJsonFile<GameMallConfig>();
            }
            return gameMallConfigs;
        }
    }

    private static List<GamePropConfig> gameProps;
    public static List<GamePropConfig> GameProps
    {
        get
        {
            if (gameProps == null)
            {
                gameProps = ResourceUtil.LoadJsonFile<GamePropConfig>();
            }
            return gameProps;
        }
    }

    private static List<GameTaskConfig> gameTaskConfig;
    public static List<GameTaskConfig> GameTaskConfigs
    {
        get
        {
            if (gameTaskConfig == null)
            {
                gameTaskConfig = ResourceUtil.LoadJsonFile<GameTaskConfig>();
            }
            return gameTaskConfig;
        }
    }


    private static List<GameNodeConsumeConfig> gameNodeConsumeConfigs;
    public static List<GameNodeConsumeConfig> GameNodeConsumeConfigs
    {
        get
        {
            if (gameNodeConsumeConfigs == null)
            {
                gameNodeConsumeConfigs = ResourceUtil.LoadJsonFile<GameNodeConsumeConfig>();
            }
            return gameNodeConsumeConfigs;
        }
    }

    private static List<GameSmsNodeConfig> gameSmsNodeConfigs;
    public static List<GameSmsNodeConfig> GameSmsNodeConfigs
    {
        get
        {
            if (gameSmsNodeConfigs == null)
            {
                gameSmsNodeConfigs = ResourceUtil.LoadJsonFile<GameSmsNodeConfig>();
            }
            return gameSmsNodeConfigs;
        }
    }

    private static List<GameSmsPointConfig> gameSmsPointConfigs;
    public static List<GameSmsPointConfig> GameSmsPointConfigs
    {
        get
        {
            if (gameSmsPointConfigs == null)
            {
                gameSmsPointConfigs = ResourceUtil.LoadJsonFile<GameSmsPointConfig>();
            }
            return gameSmsPointConfigs;
        }
    }

    private static List<GameCellSmsConfig> gameCellSmsConfigs;
    public static List<GameCellSmsConfig> GameCellSmsConfigs
    {
        get
        {
            if (gameCellSmsConfigs == null)
            {
                gameCellSmsConfigs = ResourceUtil.LoadJsonFile<GameCellSmsConfig>();
            }
            return gameCellSmsConfigs;
        }
    }

    private static List<GameTimelineNodeConfig> gameTimelineNodeConfigs;
    public static List<GameTimelineNodeConfig> GameTimelineNodeConfigs
    {
        get
        {
            if (gameTimelineNodeConfigs == null)
            {
                gameTimelineNodeConfigs = ResourceUtil.LoadJsonFile<GameTimelineNodeConfig>();
            }
            return gameTimelineNodeConfigs;
        }
    }

    private static List<GameTimelinePointConfig> gameTimelinePointConfigs;
    public static List<GameTimelinePointConfig> GameTimelinePointConfigs
    {
        get
        {
            if (gameTimelinePointConfigs == null)
            {
                gameTimelinePointConfigs = ResourceUtil.LoadJsonFile<GameTimelinePointConfig>();
            }
            return gameTimelinePointConfigs;
        }
    }

    private static List<GameCellTimelineConfig> gameCellTimelineConfigs;
    public static List<GameCellTimelineConfig> GameCellTimelineConfigs
    {
        get
        {
            if (gameCellTimelineConfigs == null)
            {
                gameCellTimelineConfigs = ResourceUtil.LoadJsonFile<GameCellTimelineConfig>();
            }
            return gameCellTimelineConfigs;
        }
    }

    private static List<GameCellVoiceBackgroundConfig> gameCellVoiceBackgroundConfigs;
    public static List<GameCellVoiceBackgroundConfig> GameCellVoiceBackgroundConfigs
    {
        get
        {
            if (gameCellVoiceBackgroundConfigs == null)
            {
                gameCellVoiceBackgroundConfigs = ResourceUtil.LoadJsonFile<GameCellVoiceBackgroundConfig>();
            }
            return gameCellVoiceBackgroundConfigs;
        }
    }

    private static List<GameExceptionServiceConfig> gameExceptionServiceConfigs;
    public static List<GameExceptionServiceConfig> GameExceptionServiceConfigs
    {
        get
        {
            if (gameExceptionServiceConfigs == null)
            {
                gameExceptionServiceConfigs = ResourceUtil.LoadJsonFile<GameExceptionServiceConfig>();
            }
            return gameExceptionServiceConfigs;
        }
    }

    private static List<SpecialWordsConfig> specialWordsConfigs;
    public static List<SpecialWordsConfig> SpecialWordsConfigs
    {
        get
        {
            if (specialWordsConfigs == null)
            {
                specialWordsConfigs = ResourceUtil.LoadJsonFile<SpecialWordsConfig>();
            }
            return specialWordsConfigs;
        }
    }

    private static List<GameTitleConfig> gameTitleConfigs;
    public static List<GameTitleConfig> GameTitleConfigs
    {
        get
        {
            if (gameTitleConfigs == null)
            {
                gameTitleConfigs = ResourceUtil.LoadJsonFile<GameTitleConfig>();
            }
            return gameTitleConfigs;
        }
    }

    private static List<GameMissionConfig> gameMissionConfigs;
    public static List<GameMissionConfig> GameMissionConfigs
    {
        get
        {
            if (gameMissionConfigs == null)
            {
                gameMissionConfigs = ResourceUtil.LoadJsonFile<GameMissionConfig>();
            }
            return gameMissionConfigs;
        }
    }

    private static List<GameAvatarFrameConfig> gameAvatarFrameConfigs;
    public static List<GameAvatarFrameConfig> GameAvatarFrameConfigs
    {
        get
        {
            if (gameAvatarFrameConfigs == null)
            {
                gameAvatarFrameConfigs = ResourceUtil.LoadJsonFile<GameAvatarFrameConfig>();
            }
            return gameAvatarFrameConfigs;
        }
    }

    private static List<GameChestsConfig> gameChestsConfigs;
    public static List<GameChestsConfig> GameChestsConfigs
    {
        get
        {
            if (gameChestsConfigs == null)
            {
                gameChestsConfigs = ResourceUtil.LoadJsonFile<GameChestsConfig>();
            }
            return gameChestsConfigs;
        }
    }

    private static List<GameFromConfig> gameFromConfigs;
    public static List<GameFromConfig> GameFromConfigs
    {
        get
        {
            if (gameFromConfigs == null)
            {
                gameFromConfigs = ResourceUtil.LoadJsonFile<GameFromConfig>();
            }
            return gameFromConfigs;
        }
    }

    private static List<GameFuncStepConfig> gameFuncStepConfigs;
    public static List<GameFuncStepConfig> GameFuncStepConfigs
    {
        get
        {
            if (gameFuncStepConfigs == null)
            {
                gameFuncStepConfigs = ResourceUtil.LoadJsonFile<GameFuncStepConfig>();
            }
            return gameFuncStepConfigs;
        }
    }

    private static List<GameSevenAdConfig> gameSevenAdConfigs;
    public static List<GameSevenAdConfig> GameSevenAdConfigs
    {
        get
        {
            if (gameSevenAdConfigs == null)
            {
                gameSevenAdConfigs = ResourceUtil.LoadJsonFile<GameSevenAdConfig>();
            }
            return gameSevenAdConfigs;
        }
    }

    private static List<GameXinlingConfig> gameXinlingConfigs;
    public static List<GameXinlingConfig> GameXinlingConfigs
    {
        get
        {
            if (gameXinlingConfigs == null)
            {
                gameXinlingConfigs = ResourceUtil.LoadJsonFile<GameXinlingConfig>();
            }
            return gameXinlingConfigs;
        }
    }

    private static List<GameXinlingLjflConfig> gameXinlingLjflConfigs;
    public static List<GameXinlingLjflConfig> GameXinlingLjflConfigs
    {
        get
        {
            if (gameXinlingLjflConfigs == null)
            {
                gameXinlingLjflConfigs = ResourceUtil.LoadJsonFile<GameXinlingLjflConfig>();
            }
            return gameXinlingLjflConfigs;
        }
    }

    private static List<GameXinlingYqzcConfig> gameXinlingYqzcConfigs;
    public static List<GameXinlingYqzcConfig> GameXinlingYqzcConfigs
    {
        get
        {
            if (gameXinlingYqzcConfigs == null)
            {
                gameXinlingYqzcConfigs = ResourceUtil.LoadJsonFile<GameXinlingYqzcConfig>();
            }
            return gameXinlingYqzcConfigs;
        }
    }

    private static List<GameXinlingZzwwDollConfig> gameXinlingZzwwDollConfigs;
    public static List<GameXinlingZzwwDollConfig> GameXinlingZzwwDollConfigs
    {
        get
        {
            if (gameXinlingZzwwDollConfigs == null)
            {
                gameXinlingZzwwDollConfigs = ResourceUtil.LoadJsonFile<GameXinlingZzwwDollConfig>();
            }
            return gameXinlingZzwwDollConfigs;
        }
    }

    private static List<GameXinlingZzwwFrgConfig> gameXinlingZzwwFrgConfigs;
    public static List<GameXinlingZzwwFrgConfig> GameXinlingZzwwFrgConfigs
    {
        get
        {
            if (gameXinlingZzwwFrgConfigs == null)
            {
                gameXinlingZzwwFrgConfigs = ResourceUtil.LoadJsonFile<GameXinlingZzwwFrgConfig>();
            }
            return gameXinlingZzwwFrgConfigs;
        }
    }

    private static List<GameXinglingDlllConfig> gameXinglingDlllConfigs;
    public static List<GameXinglingDlllConfig> GameXinglingDlllConfigs
    {
        get
        {
            if (gameXinglingDlllConfigs == null)
            {
                gameXinglingDlllConfigs = ResourceUtil.LoadJsonFile<GameXinglingDlllConfig>();
            }
            return gameXinglingDlllConfigs;
        }
    }

    private static List<GameDetailConfig> gameDetailConfigs;
    public static List<GameDetailConfig> GameDetailConfigs
    {
        get
        {
            if (gameDetailConfigs == null)
            {
                gameDetailConfigs = ResourceUtil.LoadJsonFile<GameDetailConfig>();
            }
            return gameDetailConfigs;
        }
    }

    private static List<GameMusicTimelineConfig> gameMusicTimelineConfigs;
    public static List<GameMusicTimelineConfig> GameMusicTimelineConfigs
    {
        get
        {
            if (gameMusicTimelineConfigs == null)
            {
                gameMusicTimelineConfigs = ResourceUtil.LoadJsonFile<GameMusicTimelineConfig>();
            }

            return gameMusicTimelineConfigs;
        }
    }

    private static List<GuiderInfoLinked> guiderLinkedListInfo;
    public static List<GuiderInfoLinked> GuiderLinkedListInfo
    {
        get
        {
            if (guiderLinkedListInfo == null)
            {
                List<GameGuideConfig> guiderInfos = ResourceUtil.LoadJsonFile<GameGuideConfig>();
                guiderLinkedListInfo = CreateLinkedList(guiderInfos);

            }

            return guiderLinkedListInfo;
        }
    }


    private static Dictionary<string, GuiderInfoLinked> guiderDict = new Dictionary<string, GuiderInfoLinked>();

    public static List<GuiderInfoLinked> CreateLinkedList(List<GameGuideConfig> list)
    {
        List<GuiderInfoLinked> relist = new List<GuiderInfoLinked>();


        guiderDict.Clear();
        for (int i = 0; i < list.Count; i++)
        {


            GameGuideConfig gi = list[i];
            GuiderInfoLinked gil = new GuiderInfoLinked(gi, null, i);
            relist.Add(gil);
            string key = gi.id.ToString();
            if (guiderDict.ContainsKey(key))
            {
            }
            guiderDict[key] = gil;
        }

        for (int i = 0; i < relist.Count; i++)
        {
            string str = relist[i].guiderInfo.next_to;
            GuiderInfoLinked next = null;
            if (str != null && str != "")
            {
                guiderDict.TryGetValue(str, out next);
            }
            relist[i].Next = next;
        }
        return relist;
    }
}

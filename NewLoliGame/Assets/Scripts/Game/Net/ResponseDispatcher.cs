using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResponseDispatcher
{
    public static void Dispatcher<T>(string method, T entity, Status status)
    {
        
        switch (method)
        { 
            case NetHeaderConfig.SIGNUP:
            case NetHeaderConfig.SIGNIN:
                SignUpInfo signUpInfo = entity as SignUpInfo;
                if (signUpInfo != null)
                {
                    GameData.User = signUpInfo.user;
                    GameData.token = signUpInfo.token;
                    GameData.playerId = signUpInfo.playerId;
                }
                break;
            case NetHeaderConfig.CONTRACT:
            case NetHeaderConfig.CREATE_PLAYER_NEW:
            case NetHeaderConfig.GET_PLAYR_INFO:
                GameData.Player = entity as Player;
                GameData.playerId = GameData.Player.id;
                //刷新数据
                GameData.Dolls.Clear();
                break;
            case NetHeaderConfig.GET_PLAYER_PPROPERTY:
            case NetHeaderConfig.PLAYER_BUY_STAR:
                PlayerProperty playerProperty = entity as PlayerProperty;
                if (playerProperty != null)
                    ResponseCallBack.RefreshPlayerMoneyInfo(playerProperty.love, playerProperty.diamond);
                break;
            case NetHeaderConfig.UPGRADE_PLAYER_LEVEL:
                {
                    Player player = entity as Player;
                    if (player != null)
                    {
                        ResponseCallBack.UpgradePlayerLevel(player);
                        int[] info = { PlayerDataTypeInfo.PLAYER_INFO, PlayerDataTypeInfo.ATTR_ATTRIBUTE };
                        ResponseCallBack.RefreshPlayerInfo(info, player);
                    }
                }
                break;
            case NetHeaderConfig.UPGRADE_PLAYER_ATTRIBUTE:

                Player attrPlayer = entity as Player;
                if (attrPlayer != null)
                {
                    ResponseCallBack.UpgradePlayerAttribute(attrPlayer);

                    int[] info = { PlayerDataTypeInfo.MONEY, PlayerDataTypeInfo.ATTR_LEVEL, PlayerDataTypeInfo.ATTR_ATTRIBUTE };
                    ResponseCallBack.RefreshPlayerInfo(info, attrPlayer);
                }

                break;
            case NetHeaderConfig.UPGRADE_DOLL_LEVEL:
                Player attrPlayerA = entity as Player;
                if (attrPlayerA != null)
                {
                    int[] info = { PlayerDataTypeInfo.MONEY, PlayerDataTypeInfo.ATTR_ATTRIBUTE };
                    ResponseCallBack.RefreshPlayerInfo(info, attrPlayerA);

                    ResponseCallBack.UpgradeDollLevel(attrPlayerA);
                }
                break;
            case NetHeaderConfig.DOLL_BUY:
            case NetHeaderConfig.DOLL_COMPOSE:
                Player attrPlayerB = entity as Player;
                if (attrPlayerB != null)
                {
                    int[] info = { PlayerDataTypeInfo.MONEY, PlayerDataTypeInfo.ATTR_ATTRIBUTE };
                    ResponseCallBack.RefreshPlayerInfo(info, attrPlayerB);

                    ResponseCallBack.DollBuy(attrPlayerB);
                }
                break;
            case NetHeaderConfig.DOLL_SKIN_SET:
            case NetHeaderConfig.DOLL_SKIN_BUY:
                Player attrPlayerC = entity as Player;
                ResponseCallBack.DollSkinBuy(attrPlayerC);
                break;
            case NetHeaderConfig.STORY_RECORD_NODE:
            case NetHeaderConfig.STROY_SKIP_NODE:
                PlayerStoryInfo playerStory = entity as PlayerStoryInfo;
                if (playerStory != null)
                    EventMgr.Ins.DispachEvent(EventConfig.STORY_REFRESH_NODE, playerStory);
                break;
            case NetHeaderConfig.STORY_CHAPTER_INFO:
                PlayerChapterInfo playerChapterInfo = entity as PlayerChapterInfo;
                if (playerChapterInfo != null)
                    StoryDataMgr.ins.playerChapterInfo = playerChapterInfo;
                break;
            case NetHeaderConfig.PLAYER_ADD_RESOURCES:
                Player playerA = entity as Player;
                if (playerA != null)
                {
                    int[] info = { PlayerDataTypeInfo.MONEY };
                    ResponseCallBack.RefreshPlayerInfo(info, playerA);
                }
                break;
            case NetHeaderConfig.MODIFY_PLAYERINFO:
                Player playerDes = entity as Player;
                if (playerDes != null)
                {
                    int[] info = { PlayerDataTypeInfo.MONEY, PlayerDataTypeInfo.CAN_MODIFY };
                    ResponseCallBack.RefreshPlayerInfo(info, playerDes);
                }
                EventMgr.Ins.DispachEvent(EventConfig.REFRESH_PLAYER_BASE_INFO);
                break;
            case NetHeaderConfig.ACTOR_SKIN_LIST:
                PlayerActor playerActor = entity as PlayerActor;
                if (playerActor != null)
                {
                    InteractiveDataMgr.ins.CurrentPlayerActor = playerActor;
                    InteractiveDataMgr.ins.PlayerProps = playerActor.playerProp;
                    InteractiveDataMgr.ins.RefreshSkin();
                }
                break;
            case NetHeaderConfig.PRESENT_PROP:
                PlayerActor playerActorA = entity as PlayerActor;
                ResponseCallBack.PresentProp(playerActorA);

                break;
            case NetHeaderConfig.PROP_STUDY:
                ActorProperty propStudy = entity as ActorProperty;
                ResponseCallBack.RefreshPlayerMoneyInfo(propStudy.property.love, propStudy.property.diamond);
                InteractiveDataMgr.ins.CurrentPlayerActor.unlock_prop = propStudy.playerActor.unlock_prop;
                EventMgr.Ins.DispachEvent(EventConfig.REFRESH_ALL_GIFT_LIST);
                break;
            case NetHeaderConfig.PROP_MAKE:
                PropMake propMake = entity as PropMake;

                ResponseCallBack.RefreshPlayerMoneyInfo(propMake.love, propMake.diamond);
                InteractiveDataMgr.ins.PlayerProps = propMake.playerProp;
                EventMgr.Ins.DispachEvent(EventConfig.REFRESH_OWN_GIFT_LIST);
                break;
            case NetHeaderConfig.MALL_BUY:
                PropMake propMallMake = entity as PropMake;
                GameData.Player.charge_prop = propMallMake.charge_prop;


                ResponseCallBack.RefreshPlayerMoneyInfo(propMallMake.love, propMallMake.diamond);
                ShopMallDataMgr.ins.RefreshQureyMallCallBackInfo(propMallMake);
                break;

            case NetHeaderConfig.CODE_AWARD:
            case NetHeaderConfig.FRIEND_GIFT_RECEIVE:
            case NetHeaderConfig.WELFARE_SEVEN_CHECK:
            case NetHeaderConfig.WELFARE_AD_CHECK:
            case NetHeaderConfig.WELFARE_DAILY_CHECK:
                PropMake dailyPropMake = entity as PropMake;
                ResponseCallBack.PlayerPropMoney(dailyPropMake);
                ResponseCallBack.RefreshPlayerMoneyInfo(dailyPropMake.love, dailyPropMake.diamond);
                break;
            case NetHeaderConfig.PLAYER_PROP_INFO:
                PlayerProp playerPropInfos = entity as PlayerProp;
                ShopMallDataMgr.ins.CurrentPropInfo = playerPropInfos;
                break;
            case NetHeaderConfig.ACTOR_EDIT:
                Role role = entity as Role;
                SMSDataMgr.Ins.RefreshRoleName(role);
                break;
            case NetHeaderConfig.ACTOR_BACKGROUND_BUY:
            case NetHeaderConfig.ACTOR_BACKGROUND_COMPOSE:
            case NetHeaderConfig.ACTOR_SKIN_BUY:
                ActorProperty propStudyA = entity as ActorProperty;

                ResponseCallBack.RefreshPlayerMoneyInfo(propStudyA.property.love, propStudyA.property.diamond);
                InteractiveDataMgr.ins.CurrentPlayerActor = propStudyA.playerActor;
                InteractiveDataMgr.ins.RefreshSkin();
                break;
            case NetHeaderConfig.ACTOR_SET:
                PlayerActor playerActorB = entity as PlayerActor;
                ResponseCallBack.ActorSet(playerActorB);
                break;
            case NetHeaderConfig.SET_HOMEACTOR:
                HomeActor homeActor = entity as HomeActor;
                GameData.Player.homeActor = homeActor;
                break;
            case NetHeaderConfig.USER_INFO:
                User user = entity as User;
                GameData.User = user;
                break;
            case NetHeaderConfig.CELL_CHECK_MESSAGE:
                PushInfo pushInfo = entity as PushInfo;
                ResponseCallBack.GetCallInfo(pushInfo);
                break;
            case NetHeaderConfig.SAVE_ALARM:
                AlarmClockInfo alarmClockInfo = entity as AlarmClockInfo;
                if (GameData.Player.homeActor.actor_id != alarmClockInfo.actor_id)
                {
                    string[] skins = alarmClockInfo.actor.skin.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    string[] backgrounds = alarmClockInfo.actor.background.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    GameData.Player.homeActor.skin = skins[0];
                    GameData.Player.homeActor.background = backgrounds[0];
                    GameData.Player.homeActor.actor_id = alarmClockInfo.actor.actor_id;

                    WWWForm wWWForm = new WWWForm();
                    wWWForm.AddField("actorId", alarmClockInfo.actor_id);
                    GameMonoBehaviour.Ins.RequestInfoPost<HomeActor>(NetHeaderConfig.SET_HOMEACTOR, wWWForm, null);
                }
                break;
            case NetHeaderConfig.RED_POINT:
                PlayerRedpoint redpoint = entity as PlayerRedpoint;
                RedpointMgr.Ins.RefreshRedpoint(redpoint);
                break;


            default:
                break;
        }
        RefreshTop();
        GetTime(status.timeStamp);
    }


    public static void Dispatcher<T>(string method, List<T> entity ,Status status)
    {
        switch (method)
        {
            case NetHeaderConfig.QUERY_ANNOUNCEMENT:
                List<Announcement> announcements = entity as List<Announcement>;
                EventMgr.Ins.DispachEvent(EventConfig.LOGIN_ANNOUCEMENT, announcements);
                break;
            case NetHeaderConfig.QUERY_SERVICEAGREEMENT:
                List<Announcement> serviceAgreement = entity as List<Announcement>;
                EventMgr.Ins.DispachEvent(EventConfig.LOGIN_SERVICEAGREEMENT, serviceAgreement);
                break;
            case NetHeaderConfig.QUERY_SERVICELIST:
                List<ServerList> serverLists = entity as List<ServerList>;
                EventMgr.Ins.DispachEvent(EventConfig.LOGIN_SERVERLIST, serverLists);
                break;
            case NetHeaderConfig.GET_PLAYER_AVATAR:
                List<Avatar> avatars = entity as List<Avatar>;
                if (avatars != null && avatars.Count > 0)
                {
                    GameData.Avatars = avatars;
                }
                break;
            case NetHeaderConfig.GET_DEFAULT_DOLLS:
            case NetHeaderConfig.GET_ALL_DOLLS:
                List<GameInitCardsConfig> dolls = entity as List<GameInitCardsConfig>;
                ResponseCallBack.GetAllDolls(dolls);

                break;
            case NetHeaderConfig.STORY_GET_SOTRY_INFO:
                List<PlayerStoryInfo> playerStoryInfos = entity as List<PlayerStoryInfo>;
                if (playerStoryInfos != null && playerStoryInfos.Count > 0)
                    StoryDataMgr.ins.storyInfos = playerStoryInfos;
                break;
            case NetHeaderConfig.STROY_RESET:
                List<PlayerStoryInfo> playerStoryInfosA = entity as List<PlayerStoryInfo>;
                if (playerStoryInfosA != null && playerStoryInfosA.Count > 0)
                {
                    StoryDataMgr.ins.ResetRoleStoryInfo(playerStoryInfosA);
                }
                break;
            case NetHeaderConfig.ACTOR_LIST:
                List<Role> roleLists = entity as List<Role>;
                //InteractiveDataMgr.ins.RefreshRoleListSort(roleLists);
                GameData.RefreshRoleListSort(roleLists);
                break;
            case NetHeaderConfig.ACTOR_BUY:
                List<Role> newRoles = entity as List<Role>;
                if (newRoles != null && newRoles.Count > 0)
                {
                    GameInitCardsConfig gameInitCards = JsonConfig.GameInitCardsConfigs.Find(a => a.card_id == newRoles[0].id);
                    newRoles[0].name = gameInitCards.name_cn;
                    InteractiveDataMgr.ins.RefreshRoleList(newRoles[0]);
                    EventMgr.Ins.DispachEvent(EventConfig.REFRESH_PLAYER_PROPERTY_INFO);
                    TinyItem tinyItem = new TinyItem();
                    tinyItem.id = newRoles[0].id;
                    UIMgr.Ins.showNextPopupView<GetRoleView, TinyItem>(tinyItem);

                }
                break;
            case NetHeaderConfig.ACTOR_PROP_LIST:
                List<PlayerProp> playerProps = entity as List<PlayerProp>;
                InteractiveDataMgr.ins.PlayerProps = playerProps;
                break;
            case NetHeaderConfig.MALL_INFO:
                List<PlayerMall> playerMalls = entity as List<PlayerMall>;
                ShopMallDataMgr.ins.PlayerMallList = playerMalls;
                break;

            case NetHeaderConfig.SWITCH_CONFIG:
                List<SwitchConfig> switchConfig = entity as List<SwitchConfig>;
                GameData.SwitchConfigs = switchConfig;
                break;

            case NetHeaderConfig.PLAYER_MOMENT:
                List<GameMomentConfig> gameMoments = entity as List<GameMomentConfig>;
                SMSDataMgr.Ins.GameMomentBgConfigs = gameMoments;
                break;
            default:
                break;
        }
        RefreshTop();
        GetTime(status.timeStamp);
    }

    static void GetTime(long time)
    {
        ServerTime serverTime = new ServerTime()
        {
            currentTime = time
        };
        if (ServiceObject.ins!=null)
            ServiceObject.ins.Callback(serverTime);
    }
    static void RefreshTop()
    {
        EventMgr.Ins.DispachEvent(EventConfig.REFRESH_USER_TOP_INFO);
        EventMgr.Ins.DispachEvent(EventConfig.REFRESH_INTERACTIVE_TOP_INFO);
        EventMgr.Ins.DispachEvent(EventConfig.REFRESH_SHOPMALL_TOP_INFO);
        EventMgr.Ins.DispachEvent(EventConfig.REFRESH_ATTRIBUITE_TOP_INFO);
        EventMgr.Ins.DispachEvent(EventConfig.REFRESH_MAKEGIFTS_TOP_INFO);
        EventMgr.Ins.DispachEvent(EventConfig.REFRESH_ROOM_TOP_INFO);
        EventMgr.Ins.DispachEvent(EventConfig.REFRESH_ACHIEVEMENT_TOP_INFO);
        EventMgr.Ins.DispachEvent(EventConfig.REFRESH_FRIEND_TOP_INFO);
        EventMgr.Ins.DispachEvent(EventConfig.REFRESH_WELFARE_TOP_INFO);
        EventMgr.Ins.DispachEvent(EventConfig.REFRESH_STORY_TOP_INFO);
        EventMgr.Ins.DispachEvent(EventConfig.REFRESH_ROLE_TOP_INFO);
    }

}

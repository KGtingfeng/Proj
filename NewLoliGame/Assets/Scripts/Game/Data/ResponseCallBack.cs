using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

/// <summary>
/// 请求返回数据，刷新数据方法
/// </summary>
public class ResponseCallBack
{
    /// <summary>
    /// 刷新货币信息
    /// </summary>
    /// <param name="love"></param>
    /// <param name="diamond"></param>
    public static void RefreshPlayerMoneyInfo(int love, int diamond)
    {
        GameData.Player.love = love;
        GameData.Player.diamond = diamond;
    }

    /// <summary>
    /// 需要手动将货币相关获得放入PlayerProp
    /// </summary>
    public static void PlayerPropMoney(PropMake propMake)
    {
        if (propMake.love > GameData.Player.love)
        {
            PlayerProp prop = new PlayerProp()
            {
                prop_id = 2,
                prop_type = 2,
                prop_count = propMake.love - GameData.Player.love,
            };
            if (propMake.playerProp == null)
            {
                propMake.playerProp = new List<PlayerProp>();
            }
            propMake.playerProp.Add(prop);
        }
        if (propMake.diamond > GameData.Player.diamond)
        {
            PlayerProp prop = new PlayerProp()
            {
                prop_id = 1,
                prop_type = 1,
                prop_count = propMake.diamond - GameData.Player.diamond,
            };
            if (propMake.playerProp == null)
            {
                propMake.playerProp = new List<PlayerProp>();
            }
            propMake.playerProp.Add(prop);
        }

    }


    /// <summary>
    /// 刷新玩家信息
    /// </summary>
    /// <param name="type"></param>
    /// <param name="tmpPlayer"></param>
    public static void RefreshPlayerInfo(int[] type, Player tmpPlayer)
    {

        for (int i = 0; i < type.Length; i++)
        {
            switch (type[i])
            {
                case PlayerDataTypeInfo.MONEY:
                    RefreshPlayerMoneyInfo(tmpPlayer.love, tmpPlayer.diamond);
                    break;
                case PlayerDataTypeInfo.CAN_MODIFY:
                    {
                        GameData.Player.description = tmpPlayer.description;
                        GameData.Player.name = tmpPlayer.name;
                        GameData.Player.nickname = tmpPlayer.nickname;
                        GameData.Player.birthday = tmpPlayer.birthday;
                        GameData.Player.avatar = tmpPlayer.avatar;
                        GameData.Player.character = tmpPlayer.character;
                    }
                    break;
                case PlayerDataTypeInfo.PLAYER_INFO:
                    {
                        GameData.Player.exp = tmpPlayer.exp;
                        GameData.Player.level = tmpPlayer.level;
                    }
                    break;
                case PlayerDataTypeInfo.ATTR_LEVEL:
                    {
                        if (tmpPlayer.attrLevel != null)
                            GameData.Player.attrLevel = tmpPlayer.attrLevel;
                    }
                    break;
                case PlayerDataTypeInfo.ATTR_ATTRIBUTE:
                    {
                        if (tmpPlayer.attribute != null)
                            GameData.Player.attribute = tmpPlayer.attribute;
                        EventMgr.Ins.DispachEvent(EventConfig.REFRESH_ROLE_INFO_MOUDEL);
                    }
                    break;
            }
        }
    }


    /// <summary>
    /// 角色升级
    /// </summary>
    /// <param name="player"></param>
    public static void UpgradePlayerLevel(Player player)
    {
        PlayerLevelConfig levelConfig = JsonConfig.PlayerLevelConfigs.Find(a => a.level_id == player.level);
        PlayerLevelConfig oldLevelConfig = JsonConfig.PlayerLevelConfigs.Find(a => a.level_id == GameData.Player.level);

        List<PlayerLevelConfig> playerLevelConfigs = new List<PlayerLevelConfig>();
        playerLevelConfigs.Add(oldLevelConfig);
        playerLevelConfigs.Add(levelConfig);
        EventMgr.Ins.DispachEvent(EventConfig.PLAYER_UPGRADE_LEVEL, playerLevelConfigs);
    }

    /// <summary>
    /// 角色升级属性
    /// </summary>
    /// <param name="attrPlayer"></param>
    public static void UpgradePlayerAttribute(Player attrPlayer)
    {
        PlayerAttrLevel tmpAttributeLvA = GameData.Player.attrLevel;

        TinyItem tinyItem = new TinyItem();
        //compare
        if (attrPlayer.attrLevel.charmLv > tmpAttributeLvA.charmLv)
        {
            tinyItem.num = DataUtil.GetPlayerAttrLevelConfig(attrPlayer.attrLevel.charmLv).charm;
            tinyItem.url = CommonUrlConfig.GetCharmUrl();
            tinyItem.type = 0;
        }

        else if (attrPlayer.attrLevel.intellLv > tmpAttributeLvA.intellLv)
        {
            tinyItem.num = DataUtil.GetPlayerAttrLevelConfig(attrPlayer.attrLevel.intellLv).intell;
            tinyItem.url = CommonUrlConfig.GetWisdomUrl();
            tinyItem.type = 1;
        }

        else if (attrPlayer.attrLevel.envLv > tmpAttributeLvA.envLv)
        {
            tinyItem.num = DataUtil.GetPlayerAttrLevelConfig(attrPlayer.attrLevel.envLv).evn;
            tinyItem.url = CommonUrlConfig.GetEnvUrl();
            tinyItem.type = 2;
        }

        else if (attrPlayer.attrLevel.manaLv > tmpAttributeLvA.manaLv)
        {
            tinyItem.num = DataUtil.GetPlayerAttrLevelConfig(attrPlayer.attrLevel.manaLv).mana;
            tinyItem.url = CommonUrlConfig.GetMagicUrl();
            tinyItem.type = 3;
        }
        if (tinyItem.num > 0)
            EventMgr.Ins.DispachEvent(EventConfig.DOLL_ATTRIBUTE_UPGRADE, tinyItem);

    }

    /// <summary>
    /// 娃娃升级
    /// </summary>
    public static void UpgradeDollLevel(Player player)
    {
        int index = GameData.Player.card.FindIndex(a => a.card_id == player.card[0].card_id);
        if (index >= 0)
        {
            GameInitCardsConfig from = GameData.Player.card[index];
            GameData.Player.card[index] = player.card[0];
            GameData.RefreshDolls();

            List<TinyItem> tinyItems = ItemUtil.GetTinyItemsForDollUpgrade(from, player.card[0]);

            tinyItems[0].voiceId = (int)SoundConfig.CommonEffectId.DollUpgrade;
            EventMgr.Ins.DispachEvent(EventConfig.DOLL_UPGRADE_LEVEL, tinyItems);
        }
    }

    /// <summary>
    /// 娃娃购买
    /// </summary>
    /// <param name="player"></param>
    public static void DollBuy(Player player)
    {
        //卡片单独操作
        GameData.Player.card.Add(player.card[0]);
        GameData.RefreshDolls();
        TinyItem tinyItem = new TinyItem();
        tinyItem.name = player.card[0].name_cn;
        tinyItem.url = "ui://J_Cha/" + DataUtil.GetDollBodyImg(player.card[0].card_id);
        tinyItem.id = player.card[0].card_id;
        EventMgr.Ins.DispachEvent(EventConfig.GET_DOLL, tinyItem);
    }

    /// <summary>
    /// 娃娃皮肤购买
    /// </summary>
    /// <param name="attrPlayerC"></param>
    public static void DollSkinBuy(Player attrPlayerC)
    {
        if (attrPlayerC != null)
        {
            int index = GameData.Player.card.FindIndex(a => a.card_id == attrPlayerC.card[0].card_id);
            int[] info = { PlayerDataTypeInfo.MONEY, PlayerDataTypeInfo.ATTR_ATTRIBUTE };
            RefreshPlayerInfo(info, attrPlayerC);
            GameData.Player.card[index] = attrPlayerC.card[0];
            GameData.RefreshDolls();
        }
    }




    /// <summary>
    /// 赠送礼物
    /// </summary>
    /// <param name="playerActor"></param>
    public static void PresentProp(PlayerActor playerActor)
    {
        FavorItem item = new FavorItem();
        item.favor = (int)(playerActor.Favour - InteractiveDataMgr.ins.CurrentPlayerActor.Favour);
        item.oldFavor = GameTool.FavorLevel(InteractiveDataMgr.ins.CurrentPlayerActor.Favour);
        item.newFavor = GameTool.FavorLevel(playerActor.Favour);
        InteractiveDataMgr.ins.CurrentPlayerActor = playerActor;
        InteractiveDataMgr.ins.PlayerProps = playerActor.playerProp;
        Role role = GameData.OwnRoleList.Find(a => a.id == playerActor.actor_id);
        role.actorFavorite = int.Parse(playerActor.favour);

        EventMgr.Ins.DispachEvent(EventConfig.FAVOR_CHANGE);
        EventMgr.Ins.DispachEvent(EventConfig.REFRESH_OWN_GIFT_LIST);
        EventMgr.Ins.DispachEvent(EventConfig.PRESENT_SUCCESS_EFFECT, item);

    }

    /// <summary>
    /// 角色皮肤设置
    /// </summary>
    public static void ActorSet(PlayerActor playerActorB)
    {
        if (playerActorB != null)
        {
            InteractiveDataMgr.ins.CurrentPlayerActor = playerActorB;
            InteractiveDataMgr.ins.RefreshSkin();

            if (GameData.Player.homeActor.actor_id == playerActorB.actor_id)
            {
                GameData.Player.homeActor.skin = playerActorB.skin.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries)[0];
                GameData.Player.homeActor.background = playerActorB.background.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries)[0];
            }
        }
    }

    /// <summary>
    ///获得所有玩家数据
    /// </summary>
    /// <param name="gameInitCardsConfigs"></param>
    public static void GetAllDolls(List<GameInitCardsConfig> dolls)
    {
        if (dolls != null)
        {
            //这里排序 把已经拥有的排序在前面 
            List<GameInitCardsConfig> tmp = new List<GameInitCardsConfig>();
            if (GameData.Player != null)
            {
                foreach (var doll in GameData.Player.card)
                {
                    int index = dolls.FindIndex(a => a.card_id == doll.card_id);
                    if (index >= 0)
                    {
                        tmp.Add(doll);
                        dolls.RemoveAt(index);
                    }
                }
            }
            GameData.Dolls.Clear();
            if (tmp.Count > 0)
                GameData.Dolls.AddRange(tmp);
            if (dolls.Count > 0)
                GameData.Dolls.AddRange(dolls);
        }
    }

    /// <summary>
    /// 获得电话推送 
    /// </summary>
    public static void GetCallInfo(PushInfo pushInfo)
    {
        if (!UIMgr.Ins.HaveView<MainView>() || pushInfo == null)
        {
            return;
        }
        if (pushInfo.message != null && pushInfo.message.Count > 0)
        {
            SmsListIndex smsListIndex = GetCurrentSmsListIndex(pushInfo.message);
            GameCellSmsConfig gameCellSms = DataUtil.GetGameCellSmsConfig(smsListIndex.sms_id);
            foreach (SmsListIndex smsList in pushInfo.message)
            {
                //非弹出的也要处理
                if (smsListIndex.sms_id != smsList.sms_id)
                {
                    GameCellSmsConfig smsConfig = DataUtil.GetGameCellSmsConfig(smsList.sms_id);
                    GameSmsNodeConfig gameSmsNode = DataUtil.GetGameSmsNodeConfig(smsConfig.startPoint);
                    GameSmsPointConfig gameSmsPoint = DataUtil.GetGameSmsPointConfig(gameSmsNode.point_id);
                    SMSInfo info = new SMSInfo(gameSmsPoint, gameSmsNode, smsConfig);
                    if (SMSDataMgr.Ins.IsOnSms && SMSDataMgr.Ins.CurrentRole == smsConfig.actor_id)
                    {
                        if (smsConfig.message_type == (int)TypeConfig.Consume.Message)
                            EventMgr.Ins.DispachEvent(EventConfig.SMS_PUSH_REFRESH, info);
                        else
                            EventMgr.Ins.DispachEvent(EventConfig.SMS_CALL_PUSH_REFRESH, info);
                    }
                    else if (SMSDataMgr.Ins.IsOnPhone)
                    {
                        EventMgr.Ins.DispachEvent(EventConfig.SMS_ADD_INFO, info);
                    }

                    if (smsConfig.message_type == (int)TypeConfig.Consume.Message)
                    {
                        AudioClip audioClip = Resources.Load(SoundConfig.PHONE_AUDIO_EFFECT_URL + (int)SoundConfig.PhoneAudioId.Message) as AudioClip;
                        GRoot.inst.PlayEffectSound(audioClip);
                    }
                }
            }
            switch (gameCellSms.message_type)
            {
                case (int)TypeConfig.Consume.Mobile:
                    ViewCallInterface(gameCellSms, smsListIndex);
                    break;
                case (int)TypeConfig.Consume.Message:
                    GameSmsNodeConfig nodeConfig = DataUtil.GetGameSmsNodeConfig(gameCellSms.startPoint);
                    GameSmsPointConfig pointConfig = DataUtil.GetGameSmsPointConfig(nodeConfig.point_id);
                    ShowPopMessage(pointConfig, smsListIndex);
                    break;
            }
        }
        else if (pushInfo.moment != null && pushInfo.moment.Count > 0)
        {

            PushPopInfo popInfo = new PushPopInfo()
            {
                type = 1,
                nodeId = GetMomentNode(pushInfo.moment),
            };
            UIMgr.Ins.showNextPopupView<SMSPopView, PushPopInfo>(popInfo);
        }
    }

    /// <summary>
    /// 展示通话界面信息
    /// </summary>
    /// <param name="gameCellSms"></param>
    static void ViewCallInterface(GameCellSmsConfig gameCellSms, SmsListIndex sms)
    {
        //通话中
        if (CallDataMgr.Ins.isCalling || sms == null)
            return;

        GameSmsNodeConfig gameSmsNode = DataUtil.GetGameSmsNodeConfig(gameCellSms.startPoint);
        GameSmsPointConfig gameSmsPoint = DataUtil.GetGameSmsPointConfig(gameSmsNode.point_id);
        SMSInfo info = new SMSInfo(gameSmsPoint, gameSmsNode, gameCellSms);
        if (SMSDataMgr.Ins.IsOnSms && SMSDataMgr.Ins.CurrentRole == gameCellSms.actor_id)
            EventMgr.Ins.DispachEvent(EventConfig.SMS_CALL_PUSH_REFRESH, info);
        else if (SMSDataMgr.Ins.IsOnPhone)
            EventMgr.Ins.DispachEvent(EventConfig.SMS_ADD_INFO, info);

        if (CallDataMgr.Ins.cellSmsConfig != null && (CallDataMgr.Ins.cellSmsConfig.prior / 1000) > (gameCellSms.prior / 1000))
            EventMgr.Ins.DispachEvent(EventConfig.REJECT_THE_CALL);

        if (SMSView.view == null || !SMSDataMgr.Ins.IsOnSms || SMSDataMgr.Ins.CurrentRole == 0 || SMSDataMgr.Ins.CurrentRole != gameCellSms.actor_id)
        {
            PushPopInfo popInfo = new PushPopInfo()
            {
                type = 0,
                smsListIndex = sms,
            };
            UIMgr.Ins.showNextPopupView<SMSPopView, PushPopInfo>(popInfo);
        }
        else
        {
            CallDataMgr.Ins.isViewSms = false;
            UIMgr.Ins.showNextPopupView<CallInterfaceView, SmsListIndex>(sms);
        }
    }
    /// <summary>
    /// 获得当前需要播放的信息
    /// 电话优先级高于
    /// </summary>
    static SmsListIndex GetCurrentSmsListIndex(List<SmsListIndex> smsListIndices)
    {
        List<SmsListIndex> callConfig = new List<SmsListIndex>();
        List<SmsListIndex> smsConfig = new List<SmsListIndex>();
        for (int i = 0; i < smsListIndices.Count; i++)
        {
            GameCellSmsConfig gameCell = DataUtil.GetGameCellSmsConfig(smsListIndices[i].sms_id);
            if (gameCell.message_type == (int)TypeConfig.Consume.Mobile)
                callConfig.Add(smsListIndices[i]);
            else if (gameCell.message_type == (int)TypeConfig.Consume.Message)
                smsConfig.Add(smsListIndices[i]);
        }
        if (callConfig.Count != 0)
            return callConfig[0];
        return smsConfig[0];
    }

    /// <summary>
    /// 短信推送调用
    /// </summary>
    static void ShowPopMessage(GameSmsPointConfig pointConfig, SmsListIndex sms)
    {
        GameSmsNodeConfig nodeConfig = JsonConfig.GameSmsNodeConfigs.Find(a => a.id == pointConfig.id);
        GameCellSmsConfig cellSmsConfig = JsonConfig.GameCellSmsConfigs.Find(a => a.id == nodeConfig.sms_id);
        SMSInfo info = new SMSInfo(pointConfig, nodeConfig, cellSmsConfig);
        if (SMSDataMgr.Ins.IsOnSms && SMSDataMgr.Ins.CurrentRole == cellSmsConfig.actor_id)
        {
            EventMgr.Ins.DispachEvent(EventConfig.SMS_PUSH_REFRESH, info);
        }
        else
        {
            if (SMSDataMgr.Ins.IsOnPhone)
            {
                EventMgr.Ins.DispachEvent(EventConfig.SMS_ADD_INFO, info);
            }
            else
            {
                PushPopInfo popInfo = new PushPopInfo()
                {
                    type = 0,
                    smsListIndex = sms,
                };
                UIMgr.Ins.showNextPopupView<SMSPopView, PushPopInfo>(popInfo);
            }
        }
        if (cellSmsConfig.message_type == (int)TypeConfig.Consume.Message)
        {
            AudioClip audioClip = Resources.Load(SoundConfig.PHONE_AUDIO_EFFECT_URL + (int)SoundConfig.PhoneAudioId.Message) as AudioClip;
            GRoot.inst.PlayEffectSound(audioClip);
        }
    }

    static int GetMomentNode(List<int> nodes)
    {

        for (int i = 0; i < nodes.Count; i++)
        {
            GameTimelineNodeConfig n = DataUtil.GetGameTimelineNodeConfig(nodes[i]);
            GameCellTimelineConfig cell = DataUtil.GetGameCellTimelineConfig(n.sms_id);
            GameTimelinePointConfig p = DataUtil.GetGameTimelinePointConfig(n.point_id);
            if (p.type == 6)
                return nodes[i];
            else
            {
                if (cell.actor_id == 0)
                    return nodes[i];
                else if (cell.startPoint == nodes[i])
                    return nodes[i];
            }

        }
        return nodes[0];
    }
}

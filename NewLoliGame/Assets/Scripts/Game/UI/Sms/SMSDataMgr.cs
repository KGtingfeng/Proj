using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using System;
using System.Text;

public class SMSDataMgr
{
    private static SMSDataMgr _instance;
    public static SMSDataMgr Ins
    {
        get
        {
            if (_instance == null)
                _instance = new SMSDataMgr();
            return _instance;
        }
    }

    //最后一次聊天对象
    private int currentRole = 0;
    public int CurrentRole
    {
        get { return currentRole; }
        set { currentRole = value; }
    }

    //聊天背景
    private List<GameMomentConfig> gameMomentBgConfigs;
    public List<GameMomentConfig> GameMomentBgConfigs
    {
        get
        {
            if (gameMomentBgConfigs == null)
                gameMomentBgConfigs = new List<GameMomentConfig>();
            return gameMomentBgConfigs;
        }
        set { gameMomentBgConfigs = value; }
    }

    /// <summary>
    /// 是否在聊天
    /// </summary>
    public bool IsOnSms { get; set; }

    /// <summary>
    /// 是否在手机
    /// </summary>
    public bool IsOnPhone { get; set; }

    /// <summary>
    /// 主界面信息
    /// </summary>
    public List<SmsListIndex> SmsIndexList { get; set; }

    //朋友圈信息,记录刚发的或刚回复的内容
    List<PlayerTimeline> momentList = new List<PlayerTimeline>();
    public List<PlayerTimeline> MomentList
    {
        get { return momentList; }
        set { momentList = value; }
    }

    public int ComeForm { get; set; }

    //记录个人详情需要返回的的模块
    public SMSView.MoudleType Moudle;


    public void SetProgressInfo(GProgressBar bar, GComponent favorCom, Role role)
    {
        int actorFavorite = role != null ? role.actorFavorite : 1;
        int level = GameTool.FavorLevel(actorFavorite);
        GameFavourTitleConfig titleConfig = JsonConfig.GameFavourTitleConfigs.Find(a => a.level_id == level);
        if (titleConfig != null)
        {
            bar.max = titleConfig.level + 1;
            bar.value = actorFavorite;
        }
        GameTool.SetLevelProgressBar(favorCom.GetChild("n23").asCom.GetChild("n23"), level);

        GTextField levelText = favorCom.GetChild("n21").asTextField;
        levelText.text = level + "";
    }

    public void RefreshRoleName(Role role)
    {
        if (role == null)
            return;
        foreach (var item in GameData.OwnRoleList)
        {
            if (item.id == role.id)
                item.name = role.name;
        }

    }


    public void RefreshFavor(SmsSave info)
    {
        if (SmsIndexList != null && info.extra != null && !string.IsNullOrEmpty(info.extra.favour))
        {
            string[] favor = info.extra.favour.Split(new char[1] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            SmsListIndex smsList = SmsIndexList.Find(a => a.actor_id == info.actorId);
            if (smsList != null)
                smsList.favour = favor[1];
            Role role = GameData.OwnRoleList.Find(a => a.id == info.actorId);
            if (role != null)
                role.actorFavorite = int.Parse(favor[1]);
        }
    }

    /// <summary>
    /// 获得回复语句
    /// </summary>
    public static string GetReply(string[] moment, GameCellTimelineConfig gameCell)
    {
        if (moment.Length > 1)
        {
            StringBuilder text = new StringBuilder();
            for (int i = 1; i < moment.Length; i++)
            {
                GameTimelineNodeConfig gameNode = DataUtil.GetGameTimelineNodeConfig(int.Parse(moment[i]));
                GameTimelinePointConfig gamePoint = DataUtil.GetGameTimelinePointConfig(gameNode.point_id);
                if (gamePoint != null)
                {
                    string contentText = DataUtil.ReplaceCharacterWithStarts(gamePoint.content1);
                    contentText = GameTool.Conversion(contentText);
                    //角色回复我回复的那一句
                    if (gamePoint.type == 6)
                    {
                        string actorName = GameTool.GetActorName(gameCell.actor_id);
                        text.Append("[color=#806375][b]" + actorName + "[/color][/b]回复了[color=#806375][b]" + GameData.Player.name + ":[/color][/b]" + contentText);
                    }
                    else
                    {
                        if (gamePoint.title == "0")
                            text.Append("[color=#806375][b]" + GameData.Player.name + ":[/color][/b][color=#86748c]" + contentText + "[/color]");
                        else
                        {
                            string actorName = GameTool.GetActorName(int.Parse(gamePoint.title));
                            text.Append("[color=#806375][b]" + actorName + ":[/color][/b][color=#86748c]" + contentText + "[/color]");
                        }
                    }
                }
                if (i != moment.Length - 1)
                    text.Append("\n");
            }
            return text.ToString();
        }
        else
            return "";
    }
}

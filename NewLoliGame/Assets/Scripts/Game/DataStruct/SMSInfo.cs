using System;


public class SMSInfo
{
    public bool isRead;
    public GameSmsPointConfig gamePointConfig;
    public GameSmsNodeConfig gameNodeConfig;
    public GameCellSmsConfig gameCellConfig;

    public SMSInfo(GameSmsPointConfig gamePointConfig, GameSmsNodeConfig smsNodeConfig, GameCellSmsConfig smsConfig, bool isRead = false)
    {
        this.gamePointConfig = gamePointConfig;
        this.gameNodeConfig = smsNodeConfig;
        this.gameCellConfig = smsConfig;
        this.isRead = isRead;
    }
}

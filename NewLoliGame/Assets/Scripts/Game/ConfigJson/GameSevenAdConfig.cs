using System;

[Serializable]
public class GameSevenAdConfig
{
    //七天登陆
    public const int SEVEN_TYPE = 1;
    //每日广告福利
    public const int AD_TYPE = 2;
    public int id;
    public int type;
    public string award;

}

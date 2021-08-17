using System;
[Serializable]
public class GameSmsNodeConfig
{
    public int id;
    /// <summary>
    /// 短信包id
    /// </summary>
    public int sms_id;
    /// <summary>
    /// 对话id
    /// </summary>
    public int point_id;
    /// <summary>
    /// 是否跳过
    /// </summary>
    public int isPass;
    public string awards;
}

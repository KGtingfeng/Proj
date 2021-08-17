using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefsUtil
{

    static string userNameKey = "userNameKey";
    static string pwdKey = "pwdKey";

    static string serverKey = "serverKey_";

    static void CacheString(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
    }

    public static void CacheUserLoginInfo(string userName,string pwd)
    {
        CacheString(userNameKey, userName);
        CacheString(pwdKey, pwd);
    }

    public static string GetUserName()
    {
        return PlayerPrefs.GetString(userNameKey);
    }

    public static string GetUserPwd()
    {
        return PlayerPrefs.GetString(pwdKey);
    }

    public static void SerServerInfo(int userId,string serverId)
    {
        CacheString((serverKey+ userId), serverId);
    }

    public static string GetServerId(int userId)
    {
        return PlayerPrefs.GetString((serverKey + userId));
    }
}

using UnityEngine;
using System.Collections;
using System;

public class TimeUtil {
    
    /// <summary>
	/// 得到当前时间戳
	/// </summary>
	/// <returns></returns>
	public static long TimeStamp()
    {
        long epoch = (System.DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
        return epoch;
    }



    /// <summary>
    /// 获得服务器时间
    /// </summary>
    /// <param name="time">毫秒</param>
    /// <returns></returns>
    public static DateTime getServerTime()
    {
        long timeStamp = GameData.Delta_T + TimeStamp() * 1000;
        DateTime dateTime = getTime(timeStamp);
        return dateTime;
    }

    public static DateTime getTime(long _time)
    {
        long timeStamp = _time;

        DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        long lTime = long.Parse(timeStamp + "0000");

        TimeSpan toNow = new TimeSpan(lTime);
        DateTime dtResult = dtStart.Add(toNow);
        return dtResult;
    }


    /// <summary>
    /// 时间转换 例如吧20:30这样的字符串转换为时间
    /// </summary>
    /// <returns>The to date time.</returns>
    /// <param name="strTime">String time.</param>
    public static DateTime convertToDateTime(string strTime){
		DateTime time = Convert.ToDateTime (strTime);
		return time;
	}
    
}

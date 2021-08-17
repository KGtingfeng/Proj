using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonUrlConfig
{


    /// <summary>
    /// 获取魅力图片
    /// </summary>
    /// <returns>The charm URL.</returns>
    public static string GetCharmUrl()
    {
        return "ui://T_Common/icon_common_meili";
    }

    /// <summary>
    /// 获取魔法图片
    /// </summary>
    /// <returns>The magic URL.</returns>
    public static string GetMagicUrl()
    {
        return "ui://T_Common/icon_common_mofa";
    }


    /// <summary>
    /// 获取智慧图片
    /// </summary>
    /// <returns>The charm URL.</returns>
    public static string GetWisdomUrl()
    {
        return "ui://T_Common/icon_common_zhihui";
    }


    /// <summary>
    /// 获取环保图片
    /// </summary>
    /// <returns>The env URL.</returns>
    public static string GetEnvUrl()
    {
        return "ui://T_Common/icon_common_huanbao";
    }

    /// <summary>
    /// 获取消耗类型图片地址
    /// </summary>
    /// <returns>The consume item URL.</returns>
    /// <param name="consume">Consume.</param>
    public static string GetConsumeItemUrl(TypeConfig.Consume consume)
    {
        //默认是爱心
        string url = "ui://T_Common/icon_common_aixin";
        switch (consume)
        {
            case TypeConfig.Consume.Diamond:
                url = "ui://T_Common/icon_common_zuanshi";
                break;
            case TypeConfig.Consume.Time:
                url = "Game/Time/";
                break;
            case TypeConfig.Consume.Item:
                url = "ui://T_Common/icon_common_jiage";
                break;
            case TypeConfig.Consume.EXP:
                url = "ui://T_Common/icon_exp";
                break;
            case TypeConfig.Consume.Active:
                url = "ui://1g573qgkh4v22";
                break;
        }

        return url;
    }





}

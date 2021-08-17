using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEngine;

public class MathUtil
{

    static List<string> ZH_INDEX = new List<string>()
    {
        "一",
        "二",
        "三",
        "四",
        "五",
        "六",
        "七",
        "八",
        "九",
        "十"
    };

    /// <summary>
    /// Shuffle the specified sources. 混淆数组
    /// </summary>
    /// <param name="sources">Sources.</param>
    /// <typeparam name="T">The 1st type parameter.</typeparam>
    public static void Shuffle<T>(List<T> sources)
    {
        Random random = new Random();
        T[] arry = sources.ToArray();
        for (int i = sources.Count; i > 1; i--)
        {
            int newIndex = random.Next(0, i);
            int sourceIndex = i - 1;

            T data = arry[sourceIndex];
            arry[sourceIndex] = arry[newIndex];
            arry[newIndex] = data;
        }

        for (int i = 0; i < arry.Length; i++)
        {
            sources[i] = arry[i];
        }


    }


    public static string GetZHINDEX(int count)
    {
        if (count < 10)
            return ZH_INDEX[count - 1];


        string str = "";

        if (count < 100)
        {
            int prefix = count / 10;
            int suffix = count % 10;
            if (suffix == 0)
                str = ZH_INDEX[prefix - 1] + ZH_INDEX[9];
            else
                str = ZH_INDEX[prefix - 1] + ZH_INDEX[9] + ZH_INDEX[suffix - 1];

        }

        return str;
    }


    /// <summary>
    /// Strings to int list. 采用分隔符','
    /// </summary>
    /// <returns>The to int list.</returns>
    /// <param name="context">Context.</param>
    public static List<int> StringToIntList(string context)
    {
        List<int> list = new List<int>();
        if(!string.IsNullOrEmpty(context))
        {
            string[] arry = context.Split(',');
            if(arry.Length > 0)
            {
               for(int i = 0; i < arry.Length; i++)
                {
                    int obj = int.Parse( arry[i]);
                    list.Add(obj);
                }
            }
        }


        return list;
    }




}

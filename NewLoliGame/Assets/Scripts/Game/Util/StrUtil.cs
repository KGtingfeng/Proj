using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 字符串常用操作工具类
/// </summary>
public class StrUtil
{
    /// <summary>
    /// 将字符串的第一个字符替换为另一个字符并返回
    /// </summary>
    /// <param name="str"></param>
    /// <param name="oldStr"></param>
    /// <param name="newStr"></param>
    /// <returns>如果没有就返回原本的字符串</returns>
    public static string ReplaceFirst(string str, string oldStr, string newStr)
    {
        int index = str.IndexOf(oldStr);
        if (index <= 0 || index >= str.Length) { return str; }

        return str.Substring(0, index) + newStr + str.Substring(index + 1);
    }
}

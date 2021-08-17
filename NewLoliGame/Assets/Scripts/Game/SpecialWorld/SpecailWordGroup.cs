using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 具有相同首字符的词组集合
/// </summary>
public class SpecailWordGroup
{
    /// <summary>
    /// 集合
    /// </summary>
    private List<string> wordGroups;

    public SpecailWordGroup()
    {
        wordGroups = new List<string>();
    }

    /// <summary>
    /// 添加词
    /// </summary>
    /// <param name="word"></param>
    public void Add(string word)
    {
        wordGroups.Add(word);
    }

    /// <summary>
    /// 获取总数
    /// </summary>
    /// <returns></returns>
    public int Count()
    {
        return wordGroups.Count;
    }

    /// <summary>
    /// 根据下标获取词
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public string GetWord(int index)
    {
        return wordGroups[index];
    }
}
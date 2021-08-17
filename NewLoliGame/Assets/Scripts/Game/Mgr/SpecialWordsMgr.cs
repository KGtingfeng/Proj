//// ========================================================
//// 描述：
//// 作者：Simon 
//// 创建时间：2020-09-19 22:28:47
//// 版 本：1.0
//// 版权:Guys go
//// 工作室: Dream Dog Studio 
//// ========================================================

//using System.Collections;
//using System.Collections.Generic;
//using System.Text;
//using System.Text.RegularExpressions;
//using UnityEngine;

//public class SpecialWordsMgr
//{
//    static SpecialWordsMgr ins;
//    public static SpecialWordsMgr instance
//    {
//        get
//        {
//            if (ins == null)
//                ins = new SpecialWordsMgr();

//            return ins;
//        }
//    }

//    List<string> specialWorlds = new List<string>()
//    {
//        //"WEB战牌",
//        //"yin毛",
//        //"一丝不挂",
//        "一中一台",
//        "一边一国",
//        //"一贯道",
//        //"一夜情"

//    };

//    public void Init()
//    {
//        TouchScreenView.Ins.StartCoroutine(InitHashTableContext());
//        //Debug.Log("DDS 结束时间： : " + System.DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss fff"));
//    }

//    Hashtable hashtable;

//    IEnumerator InitHashTableContext()
//    {
//        yield return new WaitForEndOfFrame(); ;
//        hashtable = new Hashtable(JsonConfig.SpecialWordsConfigs.Count);
//        //Debug.Log("DDS 开始时间： : " + System.DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss fff"));
//        foreach (var worlds in JsonConfig.SpecialWordsConfigs)
//        {
//            Hashtable tmpHasTable = hashtable;
//            for (int i = 0; i < worlds.content.Length; i++)
//            {
//                //Debug.Log(worlds[i]);
//                char signleChar = worlds.content[i];
//                if (tmpHasTable.ContainsKey(signleChar))
//                {
//                    tmpHasTable = (Hashtable)tmpHasTable[signleChar];
//                }
//                else
//                {
//                    Hashtable newHashTable = new Hashtable();
//                    newHashTable.Add("isEnd", false);
//                    tmpHasTable.Add(signleChar, newHashTable);
//                    tmpHasTable = newHashTable;

//                }

//                //如果上最后一个
//                if (i == worlds.content.Length - 1)
//                {
//                    if (!tmpHasTable.ContainsKey("isEnd"))
//                    {
//                        tmpHasTable.Add("isEnd", true);
//                    }
//                    else
//                    {
//                        tmpHasTable["isEnd"] = true;
//                    }
//                }

//            }
//        }
//    }

//    /// <summary>
//    /// 从指定文本中查找敏感字
//    /// </summary>
//    /// <param name="txt"></param>
//    /// <returns></returns>
//    public string SearchFilterWolrdAndReplace(string txt)
//    {
//        string tmpContent = Regex.Replace(txt, "[^\u4e00-\u9fa5a-zA-Z0-9]", "");
//        int i = 0;
//        StringBuilder stringBuilder = new StringBuilder(tmpContent);
//        while (i < tmpContent.Length)
//        {
//            int len = CheckFilterWorld(tmpContent, i);
//            if (len > 0)
//            {
//                for (int j = 0; j < len; j++)
//                {
//                    stringBuilder[i + j] = '*';
//                }
//                i += len;
//            }
//            else
//                ++i;

//        }
//        return stringBuilder.ToString();


//    }

//    public bool IsSpecialWord(string txt)
//    {
//        string tmpContent = Regex.Replace(txt, "[^\u4e00-\u9fa5a-zA-Z0-9]", "");
//        bool isSpecial = false;
//        int i = 0;
//        while (i < tmpContent.Length)
//        {
//            int len = CheckFilterWorld(tmpContent, i);
//            if (len > 0)
//            {
//                isSpecial = true;
//                break;
//            }
//            ++i;
//        }
//        return isSpecial;
//    }

//    /// <summary>
//    /// 从指定索引查找
//    /// </summary>
//    /// <param name="txt"></param>
//    /// <param name="beginIndex"></param>
//    /// <returns></returns>
//    int CheckFilterWorld(string txt, int beginIndex)
//    {
//        bool flag = false;
//        int len = 0;
//        Hashtable curHashTable = hashtable;
//        for (int i = beginIndex; i < txt.Length; i++)
//        {
//            char c = txt[i];
//            Hashtable tmpHashTable = (Hashtable)curHashTable[c];
//            if (tmpHashTable != null)
//            {
//                if ((bool)tmpHashTable["isEnd"])
//                    flag = true;
//                else
//                    curHashTable = tmpHashTable;
//                len++;

//            }
//            else
//            {
//                break;
//            }
//        }

//        if (!flag)
//            len = 0;
//        return len;
//    }

//}

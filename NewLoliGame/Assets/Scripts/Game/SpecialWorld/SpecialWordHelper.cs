using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Microsoft.VisualBasic;
using System;

using System.Text;
using UnityEditor;
using System.IO;
using UnityEngine.Networking;

public class SpecialWordHelper
{
    static SpecialWordHelper helper;

    public static SpecialWordHelper Ins
    {
        get
        {
            if (helper == null)
            {
                helper = new SpecialWordHelper();

            }
            return helper;
        }
    }

    /// <summary>
    /// 检测源游标
    /// </summary>
    int cursor = 0;

    /// <summary>
    /// 匹配成功后偏移量
    /// </summary>
    int wordlenght = 0;

    /// <summary>
    /// 检测词游标
    /// </summary>
    int nextCursor = 0;


    //[MenuItem("HotKey/快捷测试")]
    public static void Test()
    {

        //SpecialWordHelper specialWordHelper = new SpecialWordHelper();
        //bool isRight = specialWordHelper.IsSpecialWord("simon");
        //Debug.Log("is Right?" + isRight);

        //List<StoryGameSave> storyGameSave = new List<StoryGameSave>();
        //storyGameSave.Add(new StoryGameSave());
        //storyGameSave.Add(new StoryGameSave());



        Alarm alarm = new Alarm();
        alarm.weeks = new List<int>();
        alarm.weeks.Add(1);
        alarm.weeks.Add(2);
        alarm.weeks.Add(3);

        //string str = JsonUtility.ToJson(new Serialization<StoryGameSave>(storyGameSave));
        Debug.Log(JsonUtility.ToJson(alarm));

    }


    public SpecialWordHelper()
    {
        LoadWordLibrary();
    }
        

    /// <summary>
    /// 内存词典
    /// </summary>
    private SpecailWordGroup[] wordGroups = new SpecailWordGroup[(int)char.MaxValue];

    private string sourctText = string.Empty;
    /// <summary>
    /// 检测源
    /// </summary>
    public string SourctText
    {
        get { return sourctText; }
        set { sourctText = value; }
    }



    private List<string> illegalWords = new List<string>();

    /// <summary>
    /// 检测到的非法词集
    /// </summary>
    public List<string> IllegalWords
    {
        get { return illegalWords; }
    }

    /// <summary>
    /// 判断是否是中文
    /// </summary>
    /// <param name="character"></param>
    /// <returns></returns>
    private bool isCHS(char character)
    {
        //  中文表意字符的范围 4E00-9FA5
        int charVal = (int)character;
        return (charVal >= 0x4e00 && charVal <= 0x9fa5);
    }

    /// <summary>
    /// 判断是否是数字
    /// </summary>
    /// <param name="character"></param>
    /// <returns></returns>
    private bool isNum(char character)
    {
        int charVal = (int)character;
        return (charVal >= 48 && charVal <= 57);
    }

    /// <summary>
    /// 判断是否是字母
    /// </summary>
    /// <param name="character"></param>
    /// <returns></returns>
    private bool isAlphabet(char character)
    {
        int charVal = (int)character;
        return ((charVal >= 97 && charVal <= 122) || (charVal >= 65 && charVal <= 90));
    }


    /// <summary>
    /// 转半角小写的函数(DBC case)
    /// </summary>
    /// <param name="input">任意字符串</param>
    /// <returns>半角字符串</returns>
    ///<remarks>
    ///全角空格为12288，半角空格为32
    ///其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248
    ///</remarks>
    private string ToDBC(string input)
    {
        char[] c = input.ToCharArray();
        for (int i = 0; i < c.Length; i++)
        {
            if (c[i] == 12288)
            {
                c[i] = (char)32;
                continue;
            }
            if (c[i] > 65280 && c[i] < 65375)
                c[i] = (char)(c[i] - 65248);
        }
        return new string(c).ToLower();
    }


    private void LoadWordLibrary()
    {

        List<string> wordList = new List<string>();
        Array.Clear(wordGroups, 0, wordGroups.Length);
        string path = Application.persistentDataPath + "/t_special_words.txt";
#if UNITY_EDITOR
        path = Application.streamingAssetsPath + "/t_special_words.txt";
#endif
        //Debug.Log("before:" + System.DateTime.Now);
        string[] words = System.IO.File.ReadAllLines(path, System.Text.Encoding.Default);
        //Debug.Log("over:" + System.DateTime.Now);

        foreach (string word in words)
        {
            string key = this.ToDBC(word);
            wordList.Add(key);
        }
        Comparison<string> cmp = delegate (string key1, string key2)
        {
            return key1.CompareTo(key2);
        };
        wordList.Sort(cmp);
        for (int i = wordList.Count - 1; i > 0; i--)
        {
            if (wordList[i].ToString() == wordList[i - 1].ToString())
            {
                wordList.RemoveAt(i);
            }
        }
        foreach (var word in wordList)
        {
            if (word.Length > 0)
            {
                SpecailWordGroup group = wordGroups[(int)word[0]];
                if (group == null)
                {
                    group = new SpecailWordGroup();
                    wordGroups[(int)word[0]] = group;

                }
                string s = word.Substring(1);
                group.Add(word.Substring(1));
            }
        }

    }

    /// <summary>
    /// 检测
    /// </summary>
    /// <param name="blackWord"></param>
    /// <returns></returns>
    private bool Check(string blackWord)
    {

        wordlenght = 0;
        //检测源下一位游标
        nextCursor = cursor + 1;
        bool found = false;
        //遍历词的每一位做匹配
        for (int i = 0; i < blackWord.Length; i++)
        {
            //特殊字符偏移游标
            int offset = 0;
            if (nextCursor >= sourctText.Length)
            {
                break;
            }
            else
            {
                //检测下位字符如果不是汉字 数字 字符 偏移量加1
                for (int y = nextCursor; y < sourctText.Length; y++)
                {

                    if (!isCHS(sourctText[y]) && !isNum(sourctText[y]) && !isAlphabet(sourctText[y]))
                    {
                        offset++;
                        //避让特殊字符，下位游标如果>=字符串长度 跳出
                        if (nextCursor + offset >= sourctText.Length)
                            break;
                        wordlenght++;

                    }
                    else
                        break;
                }

                if ((int)blackWord[i] == (int)sourctText[nextCursor + offset])
                {
                    found = true;
                }
                else
                {
                    found = false;
                    break;
                }


            }
            nextCursor = nextCursor + 1 + offset;
            wordlenght++;


        }
        return found;
    }

    /// <summary>
    /// 查找并替换
    /// </summary>
    /// <param name="replaceChar"></param>
    public bool IsSpecialWord(string text)
    {
        //Debug.LogError(text);
        char replaceChar = '*';
        cursor = 0;
        sourctText = text;
        illegalWords.Clear();
        if (sourctText != string.Empty)
        {
            char[] tempString = sourctText.ToCharArray();
            for (int i = 0; i < SourctText.Length; i++)
            {
                //查询以该字为首字符的词组
                SpecailWordGroup group = wordGroups[(int)ToDBC(SourctText)[i]];
                if (group != null)
                {
                    for (int z = 0; z < group.Count(); z++)
                    {
                        string word = group.GetWord(z);
                        if (word.Length == 0 || Check(word))
                        {
                            string blackword = string.Empty;
                            for (int pos = 0; pos < wordlenght + 1; pos++)
                            {
                                blackword += tempString[pos + cursor].ToString();
                                tempString[pos + cursor] = replaceChar;
                            }
                            illegalWords.Add(blackword);
                            cursor = cursor + wordlenght;
                            i = i + wordlenght;
                        }
                    }
                }
                cursor++;
            }
            foreach (var item in illegalWords)
            {
                Debug.LogError(item);
            }
            return illegalWords.Count != 0;
        }
        return true;
    }
}




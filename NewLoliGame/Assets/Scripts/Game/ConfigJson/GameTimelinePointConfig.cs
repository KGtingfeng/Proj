using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class GameTimelinePointConfig
{
    public int id;
    public int type;
    public int temp_id;
    public string title;
    public int card_id;
    public int background_id;
    public int frame_id;
    /// <summary>
    /// 音效
    /// </summary>
    public int voice_id;
    /// <summary>
    /// 背景音乐
    /// </summary>
    public int music_id;
    /// <summary>
    /// 对白音乐
    /// </summary>
    public int sound_id;
    public int face_id;
    public string content1;
    public string content2;
    public string content3;
    public string condition1;
    public string condition2;
    public string condition3;
    public int point1;
    public int point2;
    public int point3;

    List<StoryConiditionItem> contents;
    public List<StoryConiditionItem> Contents
    {
        get
        {
            if (contents == null)
            {
                contents = new List<StoryConiditionItem>();
                if (!string.IsNullOrEmpty(content1))
                {
                    StoryConiditionItem item = new StoryConiditionItem();
                    item.content = content1;
                    item.point = point1;
                    item.index = 1;
                    contents.Add(item);
                }
                if (!string.IsNullOrEmpty(content1))
                {
                    StoryConiditionItem item = new StoryConiditionItem();
                    item.content = content2;
                    item.point = point2;
                    item.index = 2;
                    contents.Add(item);
                }
                if (!string.IsNullOrEmpty(content1))
                {
                    StoryConiditionItem item = new StoryConiditionItem();
                    item.content = content3;
                    item.point = point3;
                    item.index = 3;
                    contents.Add(item);
                }
            }

            return contents;
        }
    }
}

﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 具体剧情配置
/// </summary>
[Serializable]
public class GamePointConfig
{


    public int id;
    public int type;
    public int temp_id;
    public int face_id;
    public string title;
    public int sound_ids;
    public int music_ids;
    public int voice_ids;
    public int background_ids;
    public int card_ids;
    public string content1;
    public string content2;
    public string content3;
    public string condition1;
    public string condition2;
    public string condition3;
    public int point1;
    public int point2;
    public int point3;
    public int card_id;
    public int background_id;
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
    /// <summary>
    /// 对话框id
    /// </summary>
    public int frame_background_id;
    /// <summary>
    /// 特效id
    /// </summary>
    public int effect_id;

    List<StoryConiditionItem> _storyConiditionItems;
    public List<StoryConiditionItem> storyConiditionItems
    {
        get
        {
            if (_storyConiditionItems == null)
            {
                _storyConiditionItems = new List<StoryConiditionItem>();
                if (!content1.Equals(""))
                {
                    StoryConiditionItem storyConiditionItem = new StoryConiditionItem();
                    storyConiditionItem.content = content1;
                    storyConiditionItem.condition = condition1;
                    storyConiditionItem.point = point1;
                    storyConiditionItem.index = 1;
                    _storyConiditionItems.Add(storyConiditionItem);
                }

                if (!content2.Equals(""))
                {
                    StoryConiditionItem storyConiditionItem = new StoryConiditionItem();
                    storyConiditionItem.content = content2;
                    storyConiditionItem.condition = condition2;
                    storyConiditionItem.point = point2;
                    storyConiditionItem.index = 2;
                    _storyConiditionItems.Add(storyConiditionItem);
                }



                if (!content3.Equals(""))
                {
                    StoryConiditionItem storyConiditionItem = new StoryConiditionItem();
                    storyConiditionItem.content = content3;
                    storyConiditionItem.condition = condition3;
                    storyConiditionItem.point = point3;
                    storyConiditionItem.index = 3;
                    _storyConiditionItems.Add(storyConiditionItem);
                }



            }

            return _storyConiditionItems;

        }
    }



}

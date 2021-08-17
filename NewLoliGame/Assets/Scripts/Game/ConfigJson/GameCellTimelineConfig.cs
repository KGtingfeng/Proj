using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class GameCellTimelineConfig
{
    public int id;
    public int actor_id;
    public int message_type;
    public int content_type;
    public int prior;
    public string title;
    public string all_node;
    public string long_tree;

    public string[] ranges;
    public int startPoint
    {
        get
        {
            if (ranges == null)
            {
                ranges = long_tree.Split(',');
            }
            return int.Parse(ranges[0]);
        }
    }


    public int endPoint
    {
        get
        {
            if (ranges == null)
            {
                ranges = long_tree.Split(',');
            }
            return int.Parse(ranges[1]);
        }
    }
}

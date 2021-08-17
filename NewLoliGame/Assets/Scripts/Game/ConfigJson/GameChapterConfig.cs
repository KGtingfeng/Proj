using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameChapterConfig
{
    public int actor_id;
    public int id;
    public string name;
    public string title;
    public string long_tree;
    public string all_node;
    public string background_id;


    public string[] ranges;


    public int startPoint
    {
        get
        {
            if(ranges == null)
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

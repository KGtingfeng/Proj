using System;

[Serializable]
public class GameCellSmsConfig
{
    //生日
    public const int TYPE_BIRTHDAY = 1;
    //节日
    public const int TYPE_FESTIVAL = 2;
    //回归
    public const int TYPE_RETURN = 3;
    //剧情
    public const int TYPE_STORY = 4;
    //时刻
    public const int TYPE_TIME = 5;
    //日常
    public const int TYPE_DAILY = 6;


    public int id;
    public int actor_id;
    public int message_type;
    public int content_type;
    public int prior;
    public string title;
    public string all_node;
    public string long_tree;
    public string reset_cost;
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

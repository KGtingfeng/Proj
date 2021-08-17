using System;
using System.Collections.Generic;

/// <summary>
/// 玩家章节信息
/// </summary>
[Serializable]
public class PlayerChapterInfo
{
    /// <summary>
    /// 章节节点队列
    /// </summary>
    List<int> pointsQueue;
    /// <summary>
    /// 章节已经领取的奖励节点
    /// </summary>
    List<int> awardPoints;

    public int actor_id;
    public int chapter_id;
    /// <summary>
    /// 通关节点
    /// </summary>
    public string pass_node;
    /// <summary>
    /// 奖励节点
    /// </summary>
    public string award_node;
    public int story_status;


    public List<int> GetPointsQueue
    {
        get
        {
            pointsQueue = new List<int>();
            {
                pointsQueue = MathUtil.StringToIntList(pass_node);

            }
            return pointsQueue;
        }
    }


    public List<int> GetAwardPoints
    {
        get
        {
            awardPoints = new List<int>();
            {
                awardPoints = MathUtil.StringToIntList(award_node);

            }
            return awardPoints;
        }
    }



    void RefreshPointsQueue(string points)
    {
        if(pointsQueue == null)
        {
            pointsQueue = new List<int>();
        }
        pointsQueue.Clear();
        pass_node = points;
        pointsQueue = MathUtil.StringToIntList(pass_node);

    }


    void RefreshAwardPoints(string awrds)
    {
        if (awardPoints == null)
            awardPoints = new List<int>();
        awardPoints.Clear();
        award_node = awrds;
        awardPoints = MathUtil.StringToIntList(award_node);
    }


    public void SychronizedPoints(string passNode, string awards)
    {
        RefreshPointsQueue(passNode);
        RefreshAwardPoints(awards);
    }


}

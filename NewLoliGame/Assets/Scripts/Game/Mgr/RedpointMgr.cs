using System.Collections;
using System.Collections.Generic;

public class RedpointMgr
{
    static RedpointMgr ins;
    public static RedpointMgr Ins
    {
        get
        {
            if (ins == null)
            {
                ins = new RedpointMgr();
            }
            return ins;
        }
    }

    PlayerRedpoint redpoint;
    //剧情红点
    public List<int> storyRedpoint = new List<int>();
    //娃娃升级
    public List<int> dollUpgradeRedpoint = new List<int>();
    //娃娃合成
    public List<int> dollCombineRedpoint = new List<int>();
    //属性升级
    public List<int> attributeRedpoint = new List<int>();
    //心愿礼物
    public List<int> wishRedpoint = new List<int>();
    //背景合成
    public List<int> backgroundRedpoint = new List<int>();
    //朋友圈
    public List<int> timelineRedpoint = new List<int>();
    //短信
    public List<int> smsRedpoint = new List<int>();
    //语音
    public List<int> callRedpoint = new List<int>();
    //福利
    public List<int> welfareRedpoint = new List<int>();
    //好友
    public List<int> friendRedPoint = new List<int>();
    //辛灵任务
    public List<int> xinlingRedPoint = new List<int>();


    bool alreadyCacheRedPoint;

    public void RefreshRedpoint(PlayerRedpoint playerRedpoint)
    {
        redpoint = playerRedpoint;

        storyRedpoint.Clear();
        string[] storyStr = playerRedpoint.story.Split(new char[1] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < storyStr.Length; i++)
        {
            storyRedpoint.Add(int.Parse(storyStr[i]));
        }

        dollUpgradeRedpoint.Clear();
        storyStr = playerRedpoint.doll_upgrade.Split(new char[1] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < storyStr.Length; i++)
        {
            dollUpgradeRedpoint.Add(int.Parse(storyStr[i]));
        }

        dollCombineRedpoint.Clear();
        storyStr = playerRedpoint.doll_combine.Split(new char[1] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < storyStr.Length; i++)
        {
            dollCombineRedpoint.Add(int.Parse(storyStr[i]));
        }

        attributeRedpoint.Clear();
        storyStr = playerRedpoint.attr_upgrade.Split(new char[1] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < storyStr.Length; i++)
        {
            attributeRedpoint.Add(int.Parse(storyStr[i]));
        }

        wishRedpoint.Clear();
        storyStr = playerRedpoint.wish_gift.Split(new char[1] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < storyStr.Length; i++)
        {
            wishRedpoint.Add(int.Parse(storyStr[i]));
        }

        backgroundRedpoint.Clear();
        storyStr = playerRedpoint.background_combine.Split(new char[1] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < storyStr.Length; i++)
        {
            backgroundRedpoint.Add(int.Parse(storyStr[i]));
        }

        timelineRedpoint.Clear();
        storyStr = playerRedpoint.timeline_public.Split(new char[1] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < storyStr.Length; i++)
        {
            timelineRedpoint.Add(int.Parse(storyStr[i]));
        }


        welfareRedpoint.Clear();
        storyStr = playerRedpoint.welfare.Split(new char[1] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < storyStr.Length; i++)
        {
            welfareRedpoint.Add(int.Parse(storyStr[i]));
        }

        friendRedPoint.Clear();
        friendRedPoint.Add(int.Parse(playerRedpoint.friend_gift));
        friendRedPoint.Add(0);
        friendRedPoint.Add(int.Parse(playerRedpoint.friend_applied));




        smsRedpoint.Clear();
        callRedpoint.Clear();
        storyStr = playerRedpoint.message.Split(new char[1] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < storyStr.Length; i++)
        {
            GameCellSmsConfig cellConfig = JsonConfig.GameCellSmsConfigs.Find(a => a.id == int.Parse(storyStr[i]));

            if (cellConfig.message_type == (int)TypeConfig.Consume.Message)
            {
                if (!smsRedpoint.Contains(cellConfig.actor_id))
                {
                    smsRedpoint.Add(cellConfig.actor_id);
                }
            }
            else
            {
                if (!callRedpoint.Contains(cellConfig.actor_id))
                {
                    callRedpoint.Add(cellConfig.actor_id);
                }
            }

        }



        xinlingRedPoint.Clear();
        storyStr = playerRedpoint.xinling.Split(new char[1] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < storyStr.Length; i++)
        {
            xinlingRedPoint.Add(int.Parse(storyStr[i]));
        }
        alreadyCacheRedPoint = true;
    }

    public bool PlayerHeadHaveRedpoint()
    {
        return TitleHaveRedpoint() || FrameHaveRedpoint();
    }

    public bool TitleHaveRedpoint()
    {
        return !string.IsNullOrEmpty(redpoint.title);
    }

    public List<int> GetTitleRedpoint()
    {
        List<int> frame = new List<int>();
        string[] frameStr = redpoint.title.Split(new char[1] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < frameStr.Length; i++)
        {
            frame.Add(int.Parse(frameStr[i]));
        }

        return frame;
    }

    public void ClearTitle()
    {
        redpoint.title = "";
    }

    public bool FrameHaveRedpoint()
    {
        return !string.IsNullOrEmpty(redpoint.frame);
    }

    public List<int> GetFrameRedpoint()
    {
        List<int> frame = new List<int>();
        string[] frameStr = redpoint.frame.Split(new char[1] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < frameStr.Length; i++)
        {
            frame.Add(int.Parse(frameStr[i]));
        }

        return frame;
    }

    public void ClearFrame()
    {
        redpoint.frame = "";
    }

    public bool RoleGroupHaveRedpoint()
    {
        return !string.IsNullOrEmpty(redpoint.doll_combine) || !string.IsNullOrEmpty(redpoint.doll_upgrade) || AttributeHaveRedpoint();
    }

    public bool AttributeHaveRedpoint()
    {
        for (int i = 0; i < attributeRedpoint.Count; i++)
        {
            if (attributeRedpoint[i] > 0)
                return true;
        }
        return false;
    }

    public bool StoryHaveRedpoint()
    {
        return !string.IsNullOrEmpty(redpoint.story);
    }

    public bool InteractiveHaveRedpoint()
    {
        return !string.IsNullOrEmpty(redpoint.wish_gift);
    }

    public bool BackgroundHaveRedpoint()
    {
        return !string.IsNullOrEmpty(redpoint.background_combine);
    }

    public bool PhoneHaveRedpoint()
    {
        return !string.IsNullOrEmpty(redpoint.message) || !string.IsNullOrEmpty(redpoint.timeline_public);
    }

    public bool PostTimelineHaveRedpoint()
    {
        for (int i = 0; i < timelineRedpoint.Count; i++)
        {
            GameCellTimelineConfig timelineConfig = JsonConfig.GameCellTimelineConfigs.Find(a => a.id == timelineRedpoint[i]);
            if (timelineConfig != null && timelineConfig.actor_id == 0)
            {
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// 辛灵任务有未完成
    /// </summary>
    public bool XinlingHaveTip()
    {
        for (int i = 0; i < xinlingRedPoint.Count; i++)
        {
            if (xinlingRedPoint[i] == 0)
            {
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// 任务有未完成
    /// </summary>
    public bool TaskHaveTip()
    {
        if (redpoint.d_mission_un == "1")
        {
            return true;
        }
        return false;
    }

    public void RemoveTimelineRedpoint()
    {
        timelineRedpoint.Clear();
        redpoint.timeline_public = "";
    }

    public int MailHaveRedpoint()
    {
        return redpoint.email;
    }

    public int TaskHaveRedpoint()
    {
        return redpoint.d_mission;
    }

    public int AchievementHaveRedpoint()
    {
        return redpoint.a_mission;
    }


    public int WelfareHaveRedpoint()
    {

        foreach (var item in welfareRedpoint)
        {
            if (item == 1)
            {
                return 1;
            }
        }
        return 0;
    }

    public int FriendHaveRedpoint()
    {
        foreach (var item in friendRedPoint)
        {
            if (item == 1)
            {
                return 1;
            }
        }
        return 0;
    }

    /// <summary>
    /// 是否已经请求过小红点数据了
    /// </summary>
    /// <returns></returns>
    public bool CanRefreshRedPoint
    {
        get { return alreadyCacheRedPoint; }
    }
}

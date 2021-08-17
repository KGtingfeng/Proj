using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MomentsInfo
{
    public static readonly int TYPE_REPLY = 0;
    public static readonly int TYPE_POST = 1;
    //0回复，1发布
    public int type;
    public GameTimelinePointConfig PointConfig;
    public List<PlayerTimeline> sms;
}

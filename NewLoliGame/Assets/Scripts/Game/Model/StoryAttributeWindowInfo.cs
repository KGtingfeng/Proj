
using System;
/// <summary>
/// 主要用于剧情和属性选择选择失败的参数传递
/// </summary>
public class StoryAttributeWindowInfo 
{
    /// <summary>
    /// 属性
    /// </summary>
    public static int ATTRIBUTE_TYPE = 0;
    /// <summary>
    /// 好感度
    /// </summary>
    public static int FAVOUR_TYPE = 1;

    public TinyItem tinyItem;
    public StoryInfo storyInfo;
    public Action callback;
    public int type;
    public string actorName;
}



public class TinyItem
{
    /// <summary>
    /// 道具类型
    /// </summary>
    public int type;
    /// <summary>
    /// 道具数量
    /// </summary>
    public int num;
    /// <summary>
    /// 图片地址
    /// </summary>
    public string url;
    /// <summary>
    /// 名字
    /// </summary>
    public string name;
    /// <summary>
    /// id
    /// </summary>
    public int id;

    /// <summary>
    /// 声音id 用于扩展参数
    /// </summary>
    public int voiceId;

    public TinyItem()
    {

    }

    public TinyItem(string name, string url, int num, int type)
    {
        this.name = name;
        this.url = url;
        this.num = num;
        this.type = type;
    }

    public TinyItem(TinyItem item)
    {
        this.id = item.id;
        this.name = item.name;
        this.url = item.url;
        this.num = item.num;
        this.type = item.type;
    }

}

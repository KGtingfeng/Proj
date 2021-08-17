using System;


/// <summary>
/// 封装普通类型为对象 进行参数传递
/// </summary>
public class NormalInfo
{
    public int index = -1;
    /// <summary>
    /// 用于参数传递的排斥情况，如果index小于0,那么可以用下面这个参数 ，用于参数扩展
    /// </summary>
    public int noIndex;

    public int type;

}


public class NormalInfoCallBack : NormalInfo
{
    public Action callback;
}


/// <summary>
/// 用于扩展参数传递
/// </summary>
public class Extrand
{
    public int type = 0;
    public string msg;
    public string key;
    public TinyItem item;
    public Action callBack;
    public Action extrand;

}


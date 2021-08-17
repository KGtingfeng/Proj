using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EventMgr
{

    public delegate void eventNotify<T>(T context);

    //Dictionary<string, Action> eventKeyPairsNoParam = new Dictionary<string, Action>();
    Dictionary<string, Delegate> eventKeyPairs = new Dictionary<string, Delegate>();

    private static EventMgr eventMgr;
    public static EventMgr Ins
    {
        get
        {
            if (eventMgr == null)
                eventMgr = new EventMgr();
            return eventMgr;
        }
    }

    private EventMgr()
    {

    }


    /// <summary>
    /// 注册无参数事件
    /// </summary>
    /// <param name="key">Key.</param>
    /// <param name="action">Action.</param>
    public void RegisterEvent(string key, Action action)
    {
        if (!eventKeyPairs.ContainsKey(key))
        {
            eventKeyPairs.Add(key, action);
        }
    }

    public void RegisterEvent<T>(string key, Action<T> action)
    {
        if (!eventKeyPairs.ContainsKey(key))
        {
            eventKeyPairs.Add(key, action);
        }

    }

    public void ReplaceEvent<T>(string key, Action<T> action)
    {
        if (!eventKeyPairs.ContainsKey(key))
        {
            eventKeyPairs.Add(key, action);
        }
        else
        {
            eventKeyPairs[key] = action;
        }

    }

    public void ReplaceEvent(string key, Action action)
    {
        if (!eventKeyPairs.ContainsKey(key))
        {
            eventKeyPairs.Add(key, action);
        }
        else
        {
            eventKeyPairs[key] = action;
        }

    }

    public void RemoveEvent(string key)
    {
        if (eventKeyPairs.ContainsKey(key))
        {
            eventKeyPairs.Remove(key);
        }
    }

    public void DispachEvent(string key)
    {
        Delegate dg = null;
        if (eventKeyPairs.TryGetValue(key, out dg))
        {
            Action action = dg as Action;
            action();
        }

    }

    public void DispachEvent<T>(string key, T data)
    {
        Delegate dg = null;
        if (eventKeyPairs.TryGetValue(key, out dg))
        {
            Action<T> action = dg as Action<T>;
            action(data);
        }

    }

    public void Dispose()
    {
        eventKeyPairs.Clear();
        eventMgr = null;
    }





}




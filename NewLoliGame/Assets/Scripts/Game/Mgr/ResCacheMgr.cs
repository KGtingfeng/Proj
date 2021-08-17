using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

/// <summary>
/// 资源缓存单例
/// </summary>
public class ResCacheMgr  
{


    private static ResCacheMgr instance;
    public static ResCacheMgr Ins
    {
        get
        {
            if (instance == null)
                instance = new ResCacheMgr();

            return instance;
        }
    }

    private ResCacheMgr() { }


    /// <summary>
    /// 公用声音资源缓存
    /// </summary>
    Dictionary<string, AudioClip> commonAudioCaches = new Dictionary<string, AudioClip>();



    public void CacheCommonAudioClip(string name, AudioClip audioClip)
    {
        if (!commonAudioCaches.ContainsKey(name))
        {
            commonAudioCaches.Add(name, audioClip);

        }  
    }


    public AudioClip GetCommonAudioClip(string name)
    {
        if (commonAudioCaches.ContainsKey(name))
        {
            return commonAudioCaches[name];
        }
        return null;
    }

    /// <summary>
    /// Spine资源缓存
    /// </summary>
    Dictionary<string, UnityEngine.Object> commonSpineCaches = new Dictionary<string, UnityEngine.Object>();

    public void CacheCommonSpine(string name, UnityEngine.Object prefab)
    {
        if (!commonSpineCaches.ContainsKey(name))
        {
            commonSpineCaches.Add(name, prefab);

        }
    }


    public UnityEngine.Object GetCommonSpine(string name)
    {
        if (commonSpineCaches.ContainsKey(name))
        {
            return commonSpineCaches[name];
        }
        return null;
    }



}

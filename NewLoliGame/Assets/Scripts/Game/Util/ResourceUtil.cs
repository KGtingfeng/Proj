
using System;
using System.Collections.Generic;
using UnityEngine;

public class ResourceUtil
{
    static readonly string JSON_PATH = "Game/Json/";


    public static List<T> LoadJsonFile<T>()
    {
        string name = typeof(T).Name;
        string url = JSON_PATH + name;
        TextAsset textAsset = Resources.Load<TextAsset>(url);
        if (textAsset != null)
        {
            //Debug.LogError("url    " + url);

            //Debug.LogError("txt   " + textAsset.text);
            SerializeJsonConf<T> data = JsonUtility.FromJson<SerializeJsonConf<T>>(textAsset.text);
            return data.entity;
        }

        return default;


    }


    /// <summary>
    /// 获取通用背景音乐
    /// </summary>
    /// <returns>The back ground music.</returns>
    /// <param name="commonMusicId">Common music identifier.</param> 
    public static AudioClip LoadBackGroundMusic(SoundConfig.CommonMusicId commonMusicId)
    {
        string url = SoundConfig.COMMON_AUDIO_MUSIC_URL + (int)commonMusicId;
        return LoadAudio(url);
    }


    /// <summary>
    /// 获取通用音效
    /// </summary>
    /// <returns>The effect.</returns>
    /// <param name="commonEffectId">Common effect identifier.</param>
    public static AudioClip LoadEffect(SoundConfig.CommonEffectId commonEffectId)
    {
        string url = SoundConfig.COMMON_AUDIO_EFFECT_URL + (int)commonEffectId;
        return LoadAudio(url);

    }

    /// <summary>
    /// 剧情背景音乐
    /// </summary>
    /// <returns>The story background music.</returns>
    /// <param name="musicId">Music identifier.</param>
    public static AudioClip LoadStoryBgMusic(int musicId)
    {
        string url = SoundConfig.STORY_AUDIO_MUSIC_URL + (int)musicId;
        return LoadAudio(url);
    }

    /// <summary>
    /// 加载对白音乐
    /// </summary>
    /// <returns>The story dialog sound.</returns>
    /// <param name="soundId">Sound identifier.</param>
    public static AudioClip LoadStoryDialogSound(int soundId)
    {
        string url = SoundConfig.STORY_AUDIO_SOUND_URL + soundId;
        return LoadAudio(url);
    }

    /// <summary>
    /// 获取剧情音效
    /// </summary>
    /// <returns>The story effect sound.</returns>
    /// <param name="effectId">Effect identifier.</param>
    public static AudioClip LoadStoryEffectSound(int effectId)
    {
        string url = SoundConfig.STORY_AUDIO_EFFECT_URL + effectId;
        return LoadAudio(url);
    }


    /// <summary>
    /// 获取主界面点击角色对应音效
    /// </summary>
    /// <returns>The main role tips music.</returns>
    /// <param name="suffix">Suffix.</param>
    public static AudioClip LoadMainRoleTipsMusic(string suffix)
    {
        string url = SoundConfig.COMMON_MAIN_ROLE_URL + suffix;
        return LoadAudio(url);
    }

    public static AudioClip LoadAudio(string path)
    {
        AudioClip audioClip = ResCacheMgr.Ins.GetCommonAudioClip(path);
        if (audioClip != null)
            return audioClip;
        audioClip = Resources.Load<AudioClip>(path);
        if (audioClip != null)
            ResCacheMgr.Ins.CacheCommonAudioClip(path, audioClip);
        return audioClip;
    }

    public static UnityEngine.Object LoadSpineByName(string spineName)
    {
        string url = UrlUtil.GetSpineUrl(spineName);
        return LoadSpine(url);
    }
    public static UnityEngine.Object LoadSpine(string path)
    {
        UnityEngine.Object o = ResCacheMgr.Ins.GetCommonSpine(path);
        if (o != null)
            return o;
        o = Resources.Load(path);
        if (o != null)
            ResCacheMgr.Ins.CacheCommonSpine(path, o);
        return o;
    }
}

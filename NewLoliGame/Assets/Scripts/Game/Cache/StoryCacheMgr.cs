using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class StoryCacheMgr
{

    List<StoryGameSave> storyGameSaves = new List<StoryGameSave>();
    private static StoryCacheMgr _storyCacheMgr;


    public static StoryCacheMgr storyCacheMgr
    {
        get
        {
            if (_storyCacheMgr == null)
            {
                _storyCacheMgr = new StoryCacheMgr();

            }
            return _storyCacheMgr;
        }
    }



    string FilePath
    {
        get
        {
            return Application.persistentDataPath + "/" + GameData.playerId + "_StoryGameSave";
        }
    }

    public void Init()
    {
        storyGameSaves.Clear();
        GameMonoBehaviour.Ins.StartCoroutine(LoadSotryGameSaveFile());
    }


    IEnumerator LoadSotryGameSaveFile()
    {

        if (File.Exists(FilePath))
        {
            string context = File.ReadAllText(FilePath);
            storyGameSaves = JsonUtility.FromJson<Serialization<StoryGameSave>>(context).ToList();
            Debug.Log("LoadSotryGameSaveFile count: " + storyGameSaves.Count);
        }

        yield return null;
    }



    public void Delete(int nodeId)
    {
        int count = storyGameSaves.RemoveAll(a => a.nodeId == nodeId);
        //int index = storyGameSaves.FindIndex(a => a.nodeId == nodeId);
        if (count >= 0)
        {
            Debug.Log("移除个数");
            Save();
        }
    }

    public void Delete(string key, int nodeId)
    {

        int index = storyGameSaves.FindIndex(a => a.nodeId == nodeId && a.ckey == key);
        if (index >= 0)
        {
            storyGameSaves.RemoveAt(index);
            Save();
        }
    }


    public void SaveProgress(string key, string value, int nodeId)
    {
        Debug.Log("save: " + key + "  value:" + value + " nodeId: " + nodeId);
        CacheStoryGameSave(key, value, nodeId);
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("nodeId", nodeId);
        wWWForm.AddField("key", key);
        wWWForm.AddField("value", value);
        GameMonoBehaviour.Ins.RequestInfoPost<StoryGameSave>(NetHeaderConfig.STROY_SAVE_GAME, wWWForm, null, false);


    }


    public void GetStoryGameSave(string key, int nodeId, Action<StoryGameSave> callback)
    {
        Debug.Log("coming call: " + key + " nodid: " + nodeId);
        int index = storyGameSaves.FindIndex(a => a.ckey == key && a.nodeId == nodeId);
        if (index < 0)
        {
            WWWForm wWWForm = new WWWForm();
            wWWForm.AddField("nodeId", nodeId);
            wWWForm.AddField("key", key);
            GameMonoBehaviour.Ins.RequestInfoPostHaveData(NetHeaderConfig.STROY_LOAD_GAME, wWWForm, (List<StoryGameSave> storyGameSaves) =>
            {
                ServertSychronizedToLocal(key, nodeId, storyGameSaves);
                //递归调用一次即可
                GetStoryGameSave(key, nodeId, callback);

            }, false);
        }
        else
        {
            callback(storyGameSaves[index]);

        }
    }

    public void GetStoryGameSaves(int nodeId, Action<List<StoryGameSave>> callback)
    {
        List<StoryGameSave> list = storyGameSaves.Where(a => a.nodeId == nodeId).ToList();
        if (list.Count == 0)
        {
            WWWForm wWWForm = new WWWForm();
            wWWForm.AddField("nodeId", nodeId);
            GameMonoBehaviour.Ins.RequestInfoPost(NetHeaderConfig.STROY_LOAD_GAME, wWWForm,
                (List<StoryGameSave> storyGameSave) =>
                {
                    ServerSychronizedMulties(storyGameSave);
                    callback(storyGameSave);
                });
        }
        else
        {
            callback(list);
        }
    }





    /// <summary>
    /// 服务器同步到本地
    /// </summary>
    /// <param name="key"></param>
    /// <param name="nodeId"></param>
    /// <param name="_storyGameSaves"></param>
    public void ServertSychronizedToLocal(string key, int nodeId, List<StoryGameSave> _storyGameSaves)
    {
        if (_storyGameSaves.Count == 0)
        {

            RefresLocalCache(new StoryGameSave(nodeId, key, StoryGameSave.DEFAULT));
        }
        else
        {
            foreach (var item in _storyGameSaves)
            {
                //这里进行对比
                RefresLocalCache(item);
            }
            if (_storyGameSaves.Find(a => a.nodeId == nodeId) == null)
            {
                RefresLocalCache(new StoryGameSave(nodeId, key, StoryGameSave.DEFAULT));
            }
        }

        Save();
    }

    public void ServerSychronizedMulties(List<StoryGameSave> _storyGameSaves)
    {
        foreach (var item in _storyGameSaves)
        {
            //这里进行对比
            RefresLocalCache(item);
        }

        string context = JsonUtility.ToJson(new Serialization<StoryGameSave>(storyGameSaves));
        File.WriteAllText(FilePath, context);
    }


    void RefresLocalCache(StoryGameSave storyGameSave)
    {
        int index = storyGameSaves.FindIndex(a => a.nodeId == storyGameSave.nodeId && a.ckey == storyGameSave.ckey);
        if (index >= 0)
            storyGameSaves[index] = storyGameSave;
        else
            storyGameSaves.Add(storyGameSave);

    }

    void CacheStoryGameSave(string key, string value, int nodeId)
    {
        int index = storyGameSaves.FindIndex(a => a.nodeId == nodeId && a.ckey == key);
        if (index >= 0)
        {
            storyGameSaves[index].value = value;
        }
        else
            storyGameSaves.Add(new StoryGameSave(nodeId, key, value));

        Save();
    }


    void Save()
    {
        string context = JsonUtility.ToJson(new Serialization<StoryGameSave>(storyGameSaves));
        File.WriteAllText(FilePath, context);
    }

}

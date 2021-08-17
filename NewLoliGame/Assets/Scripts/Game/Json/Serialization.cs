using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using System;

/**
 
 string str = JsonUtility.ToJson(new Serialization<Enemy>(enemies));
  
List<Enemy> enemies = JsonUtility.FromJson<Serialization<Enemy>>(str).ToList();
 */
// List<T>
[Serializable]
public class Serialization<T>
{
    [SerializeField]
    List<T> data;
    public List<T> ToList() { return data; }

    public Serialization(List<T> data)
    {
        this.data = data;
    }
}


/*
  
string str = JsonUtility.ToJson(new Serialization<int, Enemy>(enemies));
 JsonUtility.FromJson<Serialization<int, Enemy>>(str).ToDictionary();
 
 */
// Dictionary<TKey, TValue>
[Serializable]
public class Serialization<TKey, TValue> : ISerializationCallbackReceiver
{
    [SerializeField]
    List<TKey> keys;
    [SerializeField]
    List<TValue> values;

    Dictionary<TKey, TValue> target;
    public Dictionary<TKey, TValue> ToDictionary() { return target; }

    public Serialization(Dictionary<TKey, TValue> target)
    {
        this.target = target;
    }

    public void OnBeforeSerialize()
    {
        keys = new List<TKey>(target.Keys);
        values = new List<TValue>(target.Values);
    }

    public void OnAfterDeserialize()
    {
        var count = Math.Min(keys.Count, values.Count);
        target = new Dictionary<TKey, TValue>(count);
        for (var i = 0; i < count; ++i)
        {
            target.Add(keys[i], values[i]);
        }
    }


}

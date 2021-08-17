
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 赠送礼物说的话
/// </summary>
[Serializable]
public class GameActorReactConfig
{ 
	
	public int actor_id;
	
	public string condition_overjoyed;
	
	public string condition_happy;
	
	public string condition_notbad;
	
	public string condition_unlike;

    List<string> overjoyed;
    List<string> happy;
    List<string> notbad;
    List<string> unlike;

    string GetOverjoyed()
    {
        if(overjoyed == null)
        {
            overjoyed = new List<string>();
            string[] condition = condition_overjoyed.Split(new char[1] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in condition)
            {
                overjoyed.Add(s);
            }
        }
        if (overjoyed.Count == 1)
            return overjoyed[0];
        int index = UnityEngine.Random.Range(0, overjoyed.Count);
        return overjoyed[index];
    }

    string GetHappy()
    {
        if (happy == null)
        {
            happy = new List<string>();
            string[] condition = condition_happy.Split(new char[1] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in condition)
            {
                happy.Add(s);
            }
        }
        if (happy.Count == 1)
            return happy[0];
        int index = UnityEngine.Random.Range(0, happy.Count);
        return happy[index];
    }

    string GetNotbad()
    {
        if (notbad == null)
        {
            notbad = new List<string>();
            string[] condition = condition_notbad.Split(new char[1] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in condition)
            {
                notbad.Add(s);
            }
        }
        if (notbad.Count == 1)
            return notbad[0];
        int index = UnityEngine.Random.Range(0, notbad.Count);
        return notbad[index];
    }

    string GetUnlike()
    {
        if (unlike == null)
        {
            unlike = new List<string>();
            string[] condition = condition_unlike.Split(new char[1] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in condition)
            {
                unlike.Add(s);
            }
        }
        if (unlike.Count == 1)
            return unlike[0];
        int index = UnityEngine.Random.Range(0, unlike.Count);
        return unlike[index];
    }

    public string GetCondition(int favor)
    {
        if (favor >= -10 && favor <= 0)
            return GetUnlike();
        if (favor > 0 && favor <= 30)
            return GetNotbad();
        if (favor > 30 && favor <= 40)
            return GetHappy();
        return GetOverjoyed();
    }
}

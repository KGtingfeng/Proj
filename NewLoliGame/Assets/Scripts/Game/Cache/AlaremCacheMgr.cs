using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlaremCacheMgr
{
    static AlaremCacheMgr _alaremCacheMgr;
    public static AlaremCacheMgr alaremCacheMgr
    {
        get
        {
            if (_alaremCacheMgr == null)
            {
                _alaremCacheMgr = new AlaremCacheMgr();
            }

            return _alaremCacheMgr;
        }
    }

    string FilePath
    {
        get
        {
            return Application.persistentDataPath + "/" + GameData.playerId + "_AlaremCache";
        }
    }

    public void Init()
    {

    }


    public void Refresh(Alarm alarm)
    {

    }


}




[Serializable]
public class Alarm
{
    public int status;
    public int hour;
    public int min;
    public List<int> weeks;
}
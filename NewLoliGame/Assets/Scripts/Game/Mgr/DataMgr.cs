using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataMgr 
{
    private static DataMgr instance;
    public static DataMgr Ins
    {
        get
        {
            if (instance == null)
                instance = new DataMgr();

            return instance;
        }
    }

    private DataMgr() { }




}

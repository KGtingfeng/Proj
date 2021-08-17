using UnityEngine;
using System;
using System.Collections;

public class WaitUnity : MonoBehaviour
{
    public static void Wait()
    {
        DateTime dt1 = DateTime.Now;
        while ((DateTime.Now - dt1).TotalMilliseconds < 10000)
        {
            continue;
        };
    }

}

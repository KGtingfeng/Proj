using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using FairyGUI;

public class TimeCountdown : MonoBehaviour
{
    Action<TimeSpan> onChange;
    Action onOver;
    GameObject timeObject;

    public void initBase(Action<TimeSpan> onChange, Action onOver, GObject timeObj)
    {
        this.onChange = onChange;
        this.onOver = onOver;
        timeObject = timeObj.displayObject.gameObject;
    }

    public void countdown(int totalSeconds)
    {
        TimerCountDown tiemCountDown = timeObject.GetComponent<TimerCountDown>();
        if (tiemCountDown == null)
        {
            tiemCountDown = timeObject.AddComponent<TimerCountDown>();
        }
        tiemCountDown.totalSeconds = totalSeconds;
        tiemCountDown.OnChange = delegate (TimeSpan t, int total)
        {
            onChange(t);
        };

        tiemCountDown.OnOver = delegate ()
        {
            onOver();
        };
    }

}

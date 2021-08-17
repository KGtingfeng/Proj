using UnityEngine;
using System;

public class TimerCountDown : MonoBehaviour
{
    public delegate void EventHander();
    public delegate void EventHander1(TimeSpan ts, int timeSecCurr);

    public EventHander OnOver;

    public EventHander1 OnChange;

    //public
    public int totalSeconds = 10;//总时间
    public TimeSpan s;

    //private
    private float tempTime = 1.0f;
    private bool running = true;


    // Use this for initialization
    void Start()
    {
        s = new TimeSpan(0, 0, totalSeconds);
    }

    public void Pause()
    {
        running = false;
    }

    public void Resume()
    {
        running = true;
    }

    public void TotalSeconds(int totalSecond)
    {
        totalSeconds = totalSecond;
        s = new TimeSpan(0, 0, totalSeconds);
    }


    // Update is called once per frame
    void Update()
    {
        if (!running) { return; }

        tempTime -= Time.deltaTime;
        if (tempTime < 0)
        {
            if (totalSeconds > 0)
            {
                totalSeconds--;
                s = new TimeSpan(0, 0, totalSeconds);
                if (OnChange != null)
                {
                    OnChange(s, totalSeconds);
                }

                tempTime = 1.0f;
            }
            else
            {
                if (OnOver != null)
                {
                    OnOver();
                }
                Destroy(this);
            }
        }


    }
}

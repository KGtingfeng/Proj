using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

public class TweenMgr
{
    public static void SetTween(string baseData, GComponent gComponent)
    {
        if (!string.IsNullOrEmpty(baseData))
        {
            string[] effect = baseData.Split(new char[1] { ';' }, System.StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in effect)
            {
                string[] info = item.Split(new char[1] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
                //Debug.LogError("  sps  " + info[0] + "   type   " + info[1]);
                switch (info[1])
                {
                    case "0":
                        AddTweenFade(info, gComponent);
                        break;
                    case "1":
                    case "2":
                        AddTweenMove(info, gComponent);
                        break;
                    case "3":
                        SetTween(gComponent.GetChild(info[0]).asCom.baseUserData, gComponent.GetChild(info[0]).asCom);
                        break;
                }
            }

        }
    }

    static void AddTweenMove(string[] info, GComponent gComponent)
    {
        GObject o = gComponent.GetChild(info[0]);
        Vector2 pos = o.position;

        switch (info[1])
        {
            case "1":
                pos.x += int.Parse(info[2]);
                break;
            case "2":
                pos.y += int.Parse(info[2]);
                break;
        }
        float delay = 0;
        if (info.Length > 3)
            delay = float.Parse(info[3]);
        if (o is GGroup)
        {
            GObject oo = null;
            int cnt = gComponent.numChildren;
            for (int i = 0; i < cnt; i++)
            {
                if (gComponent.GetChildAt(i).group == o)
                {
                    oo = gComponent.GetChildAt(i);
                }
            }
            if (oo != null)
            {
                oo.displayObject.gameObject.AddComponent<UITweenMove>().SetTweenGroup(o.asGroup, pos, 1, delay);
            }
        }
        else
        {
            o.displayObject.gameObject.AddComponent<UITweenMove>().SetTweenMove(pos, 1, delay);
        }
    }

    static void AddTweenFade(string[] info, GComponent gComponent)
    {
        GObject o = gComponent.GetChild(info[0]);
        float delay = 0;
        if (info.Length > 3)
            delay = float.Parse(info[3]);
        if (o is GGroup)
        {
            GObject oo = null;
            int cnt = gComponent.numChildren;
            for (int i = 0; i < cnt; i++)
            {
                if (gComponent.GetChildAt(i).group == o)
                {
                    oo = gComponent.GetChildAt(i);
                }
            }
            if (oo != null)
            {
                oo.displayObject.gameObject.AddComponent<UITweenFade>().SetFadeGroup(o.asGroup, 1, delay);
            }
        }
        else
        {
            o.displayObject.gameObject.AddComponent<UITweenFade>().SetTweenFade(1, delay);
        }
    }
}

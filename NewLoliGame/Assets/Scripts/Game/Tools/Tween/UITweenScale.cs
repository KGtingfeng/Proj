using FairyGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITweenScale : UITweenFade
{
    float scaleTime = 0.5f;
    Vector2 from;
    Vector2 to;

    public void SetTweenScale(Vector2 from, float scaleTime = 1f, float delay = 0f)
    {
        DisplayObjectInfo info = gameObject.GetComponent<DisplayObjectInfo>();
        gObject = GRoot.inst.DisplayObjectToGObject(info.displayObject);
        gObject.alpha = 0;
        to = gObject.scale;
        this.delay = delay;
        this.scaleTime = scaleTime;
        this.from = from;
        gObject.scale = from;
         isFirst = false;
        if (gameObject.activeInHierarchy == true)
        {
            StartCoroutine(ScaleIn());
            StartCoroutine(FadeIn());
        }

    }
    private void OnEnable()
    {
        //Debug.LogError("OnEnable");
        if (gObject != null && !isFirst)
        {
            StartCoroutine(FadeIn());
            StartCoroutine(ScaleIn());
        }

    }

    private void OnDisable()
    {
        //Debug.LogError("OnDisable");
        ScaleOut();
        FadeOut();
    }

    public IEnumerator ScaleIn()
    {
        gObject.scale = from;
        yield return new WaitForSeconds(delay);
        gObject.TweenScale(to, scaleTime);
    }

    public void ScaleOut()
    {
        gObject.scale = to;
        gObject.TweenScale(from, scaleTime);
    }
}

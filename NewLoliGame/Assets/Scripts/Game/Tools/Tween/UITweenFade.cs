using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

public class UITweenFade : MonoBehaviour
{

    public float delay=0f;
    public float fadeTime=1f;
    public GObject gObject;
    public bool isFirst = true;
 

    public void SetTweenFade(float fadeTime = 1f,float delay = 0f)
    {
        DisplayObjectInfo info = gameObject.GetComponent<DisplayObjectInfo>();
        gObject = GRoot.inst.DisplayObjectToGObject(info.displayObject);
        gObject.alpha = 0;
        this.delay = delay;
        this.fadeTime = fadeTime;
        isFirst = false;
        if(gameObject.activeInHierarchy==true)
        StartCoroutine(FadeIn());

    }

    public void SetFadeGroup(GGroup gGroup, float fadeTime = 1f, float delay = 0f)
    {
        gObject = gGroup;
        gObject.alpha = 0;
        this.delay = delay;
        this.fadeTime = fadeTime;
        isFirst = false;
        if (gameObject.activeInHierarchy == true)
            StartCoroutine(FadeIn());
    }

    private void OnEnable()
    {
        //Debug.LogError("OnEnable");
        if (gObject!=null&&!isFirst)
        StartCoroutine( FadeIn());

    }

    private void OnDisable()
    {
        //Debug.LogError("OnDisable");
        FadeOut();
    }

   public  IEnumerator FadeIn()
    {
        gObject.alpha = 0;
        yield return new WaitForSeconds(delay);
        //Debug.LogError("FadeIn");
        gObject.TweenFade(1, fadeTime);
    }

    public void FadeOut()
    {
        gObject.alpha = 1;
        //Debug.LogError("FadeOut");
        gObject.TweenFade(0, fadeTime);
    }

}

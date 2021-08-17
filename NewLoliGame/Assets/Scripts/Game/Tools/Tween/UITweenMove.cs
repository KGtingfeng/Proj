using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

public class UITweenMove : UITweenFade
{
    EaseType easeType = EaseType.QuartOut;
    public float moveTime=1f;
    public Vector2 from;
    Vector2 to;

 
    public void SetTweenGroup(GGroup gGroup, Vector2 f, float moveTime = 1f, float delay = 0)
    {
        gObject = gGroup;
        to = gObject.position;
        from = f;
        gObject.position = from;
        this.delay = delay;
        this.moveTime = moveTime;
        isFirst = false;
        if (gameObject.activeInHierarchy == true)
        {
            StartCoroutine(FadeIn());
            StartCoroutine(MoveIn());
        }
    }

    public void SetTweenMove(Vector2 f,float moveTime=1f, float delay = 0)
    {
        //Debug.LogError("SetTweenMove   ");
        DisplayObjectInfo info = gameObject.GetComponent<DisplayObjectInfo>();
        gObject = GRoot.inst.DisplayObjectToGObject(info.displayObject);
        to = gObject.position;
        from = f;
        gObject.position = from;
        this.delay = delay;
        this.moveTime = moveTime;
        isFirst = false;
        if (gameObject.activeInHierarchy == true)
        {
            StartCoroutine(FadeIn());
            StartCoroutine(MoveIn());
        }
            
    }

    public void SetTweenMove(Vector2 f, EaseType ease, float moveTime = 1f, float delay = 0)
    {
        easeType = ease;
        SetTweenMove(f, moveTime, delay);
    }

    private void OnEnable()
    {
        //Debug.LogError("UITweenMove     OnEnable");
        if (gObject != null && !isFirst)
        {
            StartCoroutine(FadeIn());
            StartCoroutine(MoveIn());
        }
           
    }

    private void OnDisable()
    {
        //Debug.LogError("UITweenMove     OnDisable");

        FadeOut();
        MoveOut();
    }

    IEnumerator MoveIn()
    {
        gObject.position = from;
        yield return new WaitForSeconds(delay);

        //Debug.LogError("MoveIn"); 
        gObject.TweenMove(to, moveTime).SetEase(easeType);
    }

    void MoveOut()
    {
        gObject.position = to;
        //Debug.LogError("MoveOut");
        gObject.TweenMove(from, moveTime).SetEase(easeType);
    }
}

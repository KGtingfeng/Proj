using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;




public class BaseMoudle
{
    public BaseView baseView;
    public GComponent ui;
    public int controllerIndex;
    public List<TypingEffect> typingEffects = new List<TypingEffect>();
    public TypingEffect currentTypingEffect;
    public Action onComplete;
    public Action onTypingComplete;
    public float speed = 1.0f;
    public virtual void InitMoudle(GComponent gComponent, int controllerIndex)
    {
        ui = gComponent;
        this.controllerIndex = controllerIndex;
    }

    public virtual void InitMoudle<T>(GComponent gComponent, int controllerIndex, T data)
    {
        InitMoudle(gComponent, controllerIndex);
    }

    public virtual void InitEvent()
    {

    }

    public virtual void InitUI()
    {
       TweenMgr.SetTween(ui.baseUserData, ui);
    }

    

    /// <summary>
    /// 重新进入该界面时调用
    /// </summary>
    /// <param name="data">Data.</param>
    /// <typeparam name="D">The 1st type parameter.</typeparam>
    public virtual void InitData<D>(D data)
    {

    }


    public virtual void InitData()
    {

    }



    public virtual void Reback()
    {
        baseView.SwitchController(controllerIndex - 1);
    }

    public GObject SearchChild(string name)
    {

        return ui.GetChild(name);
    }


    public virtual void InitTypeEffect()
    {
        typingEffects.Clear();
    }

    public void PrintTex()
    {

        currentTypingEffect = null;
        if (typingEffects.Count > 0)
        {
            //Debug.LogError("PrintTex*********************typingEffects.Count  " + typingEffects.Count);
            currentTypingEffect = typingEffects[0];
            currentTypingEffect.speed = speed;
            currentTypingEffect.printTime = 0.08f;

            typingEffects.RemoveAt(0);
            Timers.inst.StartCoroutine(currentTypingEffect.OutPut(PrintTex));
        }
        else
        {
            //Debug.LogError("PrintTex###################typingEffects size: " + typingEffects.Count);
            printAllAction?.Invoke();
            onComplete?.Invoke();
            onTypingComplete?.Invoke();
        }
    }



    public Action printAllAction;

    /// <summary>
    /// 加速打印
    /// </summary>
    /// <returns><c>true</c>, if print was sped, <c>false</c> otherwise.</returns>
    public bool SpeedPrint()
    {
        //Debug.Log("speed test SpeedPrint 开始");

        if (currentTypingEffect != null)
        {
            //Debug.Log("speed test SpeedPrint 中间");
            currentTypingEffect.ShowAllRightNow();
            //currentTypingEffect.Cancel();
            return false;
        }
        //Debug.Log("speed test SpeedPrint 结束");

        return true;
    }


    public virtual void Dispose()
    {

    }


    #region  ************动画部分开始**********


    Vector2 pivot = new Vector2(0.5f, 0.5f);
    Vector2 scaleInit = new Vector2(0.98f, 0.98f);
    Vector2 scaleToBig = new Vector2(1.03f, 1.03f);
    Vector2 scaleToSmall = new Vector2(0.5f, 0.5f);

    float scaleTime = 0.1f;
    float startAlpha;
    float endAlpha = 1;


    public virtual void SmallToBig()
    {
        ui.pivot = pivot;
        ui.alpha = startAlpha;

        ui.scale = scaleInit;
        ui.TweenScale(scaleToBig, scaleTime).SetEase(EaseType.QuadIn).OnComplete(() =>
        {
            ui.TweenScale(Vector2.one, 0.05f).SetEase(EaseType.QuadIn);
        });

        ui.TweenFade(1, scaleTime).SetEase(EaseType.QuadIn);
    }

    public Action scaleToSmallAction;
    public virtual void BigToSmall()
    {

        ui.TweenScale(scaleToBig, scaleTime).SetEase(EaseType.QuadOut).OnComplete(() =>
        {
            ui.TweenScale(Vector2.one, scaleTime).SetEase(EaseType.QuadOut).OnComplete(() =>
          {
              scaleToSmallAction?.Invoke();
          });
            ui.TweenFade(0, scaleTime).SetEase(EaseType.QuadOut);
        });


    }

    #endregion ************动画部分结束**********
}

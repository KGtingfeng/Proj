using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

public class BaseWindow : Window
{
    public List<TypingEffect> typingEffects = new List<TypingEffect>();
    public TypingEffect currentTypingEffect;
    public IEnumerator coroutine;
    public GComponent wParent;
    public IEnumerator iEcoroutine;
    public float duration = 1.0f;
    protected static BaseWindow _window;

    public static BaseWindow window
    {
        get
        {
            return _window;
        }
    }

    public Message message;

    protected override void OnInit()
    {
        _window = this;
        InitUI();
    }

    /// <summary>
    /// 如果没有动画 必须重写该方法
    /// </summary>
    protected override void OnShown()
    {
        _window = this;

        iEcoroutine = CloseIEnumerator();
        GameMonoBehaviour.Ins.StartCoroutine(iEcoroutine);
    }

    public virtual void InitUI()
    {

    }
    public virtual void InitData()
    {
        _window = this;
        if (wParent.scaleX < 1 || wParent.scaleY < 1)
        {
            wParent.scale = Vector2.one;
            wParent.scaleX = 1;
            wParent.scaleY = 1;
        }

    }

    public virtual void InitEvent()
    {
        Debug.LogError("_window : " + _window);
    }

    public void ShowWindow()
    {
        GRoot.inst.ShowPopup(this);
    }

    public virtual void ShowWindow<D>(D data)
    {
        ShowWindow();
    }

    public virtual void HideWindow()
    {
        GRoot.inst.HidePopup(this);

    }




    public virtual void onPopupClosed()
    {
        //Debug.Log( this +    "  onPopupClosed");
    }

    protected void CreateWindow<T>()
    {
        contentPane = UIMgr.Ins.CreateWindow<T>();
        Center();
        modal = true;
        UIMgr.Ins.InsertWindow(_window);
        contentPane.onRemovedFromStage.Add(onPopupClosed);
        wParent = contentPane.parent;
    }




    protected GObject SearchChild(string name)
    {
        GObject gObject = contentPane.GetChild(name);
        if (gObject == null)
        {
            Debug.Log("cant't find child " + name);
            return null;
        }

        return gObject;
    }


    public virtual void InitTypeEffect()
    {
        typingEffects.Clear();
    }


    public virtual void PrintTex()
    {
        currentTypingEffect = null;
        if (typingEffects.Count > 0)
        {
            currentTypingEffect = typingEffects[0];
            currentTypingEffect._textField.visible = true;
            currentTypingEffect.Start();
            currentTypingEffect.printTime = 0.07f;

            typingEffects.RemoveAt(0);
            coroutine = currentTypingEffect.OutPut(PrintTex);
            Timers.inst.StartCoroutine(coroutine);
        }
    }




    /// <summary>
    /// 加速打印
    /// </summary>
    /// <returns><c>true</c>, if print was sped, <c>false</c> otherwise.</returns>
    public bool SpeedPrint()
    {
        if (typingEffects.Count > 0 || currentTypingEffect != null)
        {
            currentTypingEffect.printTime = 0;
            return false;
        }
        return true;
    }

    public void StopTypeEffect()
    {
        Timers.inst.StopCoroutine(coroutine);
    }

    public virtual IEnumerator CloseIEnumerator()
    {
        yield return new WaitForSeconds(duration);
        BigToSmall();
    }

    public void StopCoroutine()
    {
        if (iEcoroutine != null)
            GameMonoBehaviour.Ins.StopCoroutine(iEcoroutine);
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
        contentPane.pivot = pivot;
        contentPane.alpha = startAlpha;
        contentPane.scale = scaleInit;
        contentPane.TweenScale(scaleToBig, scaleTime).SetEase(EaseType.QuadIn).OnComplete(() =>
        {
            contentPane.TweenScale(Vector2.one, 0.05f).SetEase(EaseType.QuadIn).OnComplete(OnShown);
        });

        contentPane.TweenFade(1, scaleTime).SetEase(EaseType.QuadIn);
    }

    public Action scaleToSmallAction;
    public virtual void BigToSmall()
    {

        contentPane.TweenScale(scaleToBig, scaleTime).SetEase(EaseType.QuadOut).OnComplete(() =>
        {
            contentPane.TweenScale(Vector2.one, scaleTime).SetEase(EaseType.QuadOut).OnComplete(HideImmediately);
            contentPane.TweenFade(0, scaleTime).SetEase(EaseType.QuadOut);
        });
    }


    override protected void DoShowAnimation()
    {
        //Debug.Log("DoShowAnimation");
        InitData();
        SmallToBig();
    }

    override protected void DoHideAnimation()
    {
        //Debug.Log("DoHideAnimation");
        StopCoroutine();
        //this.TweenScale(new Vector2(0.1f, 0.1f), 0.3f).OnComplete(this.HideImmediately);
        BigToSmall();
    }

    #endregion ************动画部分结束**********
}

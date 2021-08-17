using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;


/// <summary>
/// 主要负责ui初始化相关
/// </summary>
public class BaseView : MonoBehaviour
{
    public GComponent ui;
    public Controller controller;
    public List<BaseMoudle> baseMoudles = new List<BaseMoudle>();
    public bool isAnimationView;

    public List<TypingEffect> typingEffects = new List<TypingEffect>();
    public TypingEffect currentTypingEffect;
    public Action onComplete;
    public float speed = 1.0f;
    private void Awake()
    {
        //_view = this;

    }


    public virtual void InitUI()
    {
       TweenMgr.SetTween(ui.baseUserData, ui);
    }


    public virtual void InitData()
    {
    }

    public virtual void InitData<D>(D data) { }

    public virtual void InitEvent() { }

    public virtual void onShow() { }
    /// <summary>
    /// 需要窗体动画
    /// </summary>
    public virtual void OnShowAnimation()
    {
        SmallToBig();
    }

    public virtual void OnHideAnimation()
    {
        BigToSmall();
    }
    public virtual void onHide()
    {
        ui.TweenFade(0, 1f).OnComplete(() =>
        {
            ui.visible = false;
            gameObject.SetActive(false);
        });
       
    }

    public void onDeleteAnimation<T>()
    {
        ui.TweenScale(scaleToBig, scaleTime).SetEase(EaseType.QuadOut).OnComplete(() =>
        {
            ui.TweenScale(Vector2.one, scaleTime).SetEase(EaseType.QuadOut).OnComplete(() =>
            {
                UIMgr.Ins.HideView<T>();
            });
            ui.TweenFade(startAlpha, scaleTime).SetEase(EaseType.QuadOut);
        });
    }

    public virtual void SwitchController(int index)
    {
        //通过索引设置控制器的活动页面
        controller.selectedIndex = index;
    }



    public virtual void GoToMoudle<T>(int index) { }

    public virtual void GoToMoudle<T, D>(int index, D data) { }

    public GObject SearchChild(string name)
    {
        return ui.GetChild(name);
    }


    public virtual BaseMoudle FindMoudle<T>()
    {
        foreach (var moudle in baseMoudles)
        {
            if (moudle.GetType() == typeof(T))
                return moudle;
        }
        return null;
    }

    #region  字体自动播放
    public virtual void InitTypeEffect()
    {
        typingEffects.Clear();
    }

    public void PrintTex()
    {
        currentTypingEffect = null;
        if (typingEffects.Count > 0)
        {
            currentTypingEffect = typingEffects[0];
            currentTypingEffect.speed = speed;
            currentTypingEffect.printTime = 0.1f;

            typingEffects.RemoveAt(0);
            Timers.inst.StartCoroutine(currentTypingEffect.OutPut(PrintTex));
        }
        else
        {
            onComplete?.Invoke();
        }
    }

    /// <summary>
    /// 加速打印
    /// </summary>
    /// <returns><c>true</c>, if print was sped, <c>false</c> otherwise.</returns>
    public bool SpeedPrint()
    {
        if (currentTypingEffect != null)
        {
            currentTypingEffect.ShowAllRightNow();
            return false;
        }
        return true;
    }

    #endregion


    #region  ************动画部分开始**********


    public Vector2 pivot = new Vector2(0.5f, 0.5f);
    Vector2 scaleInit = new Vector2(0.90f, 0.90f);
    Vector2 scaleToBig = new Vector2(1.08f, 1.08f);
    Vector2 scaleToSmall = new Vector2(0.5f, 0.5f);

    public float scaleTime = 0.1f;
    float startAlpha;
    float endAlpha = 1;


    void SmallToBig()
    {
        ui.pivot = pivot;
        ui.alpha = startAlpha;

        ui.scale = scaleInit;
        ui.TweenScale(scaleToBig, scaleTime).SetEase(EaseType.QuadIn).OnComplete(() =>
        {
            ui.TweenScale(Vector2.one, 0.05f).SetEase(EaseType.QuadIn);
        });

        ui.TweenFade(endAlpha, scaleTime).SetEase(EaseType.QuadIn);
    }

    public Action scaleToSmallAction;
    void BigToSmall()
    {

        ui.TweenScale(scaleToBig, scaleTime).SetEase(EaseType.QuadOut).OnComplete(() =>
        {
            ui.TweenScale(Vector2.one, scaleTime).SetEase(EaseType.QuadOut).OnComplete(() =>
            {
                ui.visible = false;
                gameObject.SetActive(false);
            });
            //ui.TweenFade(startAlpha, scaleTime).SetEase(EaseType.QuadOut);
        });


    }

    #endregion ************动画部分结束**********

}

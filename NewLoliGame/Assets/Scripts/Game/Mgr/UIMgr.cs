using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

public class UIMgr
{

    #region 单例部分
    private static UIMgr instance;
    public static UIMgr Ins
    {
        get
        {
            if (instance == null)
                instance = new UIMgr();

            return instance;
        }
    }

    private UIMgr() { }
    #endregion


    /// <summary>
    /// 用于存储ui列表对象
    /// </summary>
    List<BaseView> baseViews = new List<BaseView>();
    List<BaseWindow> baseWindows = new List<BaseWindow>();
    public Transform uiParent;
    /// <summary>
    /// 常驻
    /// </summary>
    List<Type> ForeverTypes = new List<Type>
    {
        typeof(TouchScreenView),
        typeof(LoginWaitimeView),
        typeof(ShanbaiWaittimeView),
        typeof(SplahView),
    };

    public int GetRenderOrder()
    {
        return baseViews.Count + 1;
    }



    #region

    public bool HaveView<T>()
    {
        BaseView baseView = findView<T>();
        if (baseView == null)
        {
            return false;
        }
        return true;
    }

    BaseView findView<T>()
    {
        foreach (var view in baseViews)
        {
            if (view.GetType() == typeof(T))
            {
                return view;
            }
        }

        return null;

    }

    public void AddView(BaseView baseView)
    {
        if (baseViews.Contains(baseView))
        {
            Debug.Log("Repeat add view, Please check it!!");
            return;
        }
        baseViews.Add(baseView);
    }


    BaseWindow FindWindow<T>()
    {
        foreach (var window in baseWindows)
        {
            if (window.GetType() == typeof(T))
            {
                return window;
            }
        }

        return null;

    }

    void addWindow(BaseWindow baseWindow)
    {

        if (baseWindows.Contains(baseWindow))
        {
            Debug.Log("Repeat add window, Please check it!!");
            return;
        }
        baseWindows.Add(baseWindow);

    }

    #endregion




    #region showView

    private BaseView getView<T>()
    {
        BaseView baseView = findView<T>();
        if (baseView == null)
        {
            baseView = ResourceMgr.Ins.CreateView<T>();
            baseViews.Add(baseView);
            baseView.InitUI();
        }
        baseView.GetComponent<UIPanel>().SetSortingOrder(currentMaxOrder(baseView), true);
        return baseView;
    }

    private int currentMaxOrder(BaseView baseView)
    {
        int order = baseView.GetComponent<UIPanel>().sortingOrder;
        int maxOrder = 0;

        //baseViews 包括自身
        foreach (var view in baseViews)
        {
            //if (view.GetType() == typeof(TouchScreenView) || view.GetType() == typeof(SplahView) 
            //    || view.GetType() == typeof(ShanbaiWaittimeView) || view.GetType() == typeof(LoginWaitimeView))
            if(ForeverTypes.Contains(view.GetType()))
            {
                continue;
            }

            if (!view.ui.visible || view.GetType() == baseView.GetType())
                continue;
            if (order <= view.GetComponent<UIPanel>().sortingOrder)
                maxOrder = view.GetComponent<UIPanel>().sortingOrder;
        }
        order = order <= maxOrder ? (maxOrder + 1) : order;
        return order;
    }

    public BaseView ShowToastView<T, D>(D data)
    {
        BaseView baseView = ResourceMgr.Ins.CreateView<T>();
        baseView.InitUI();
        SetToastView(baseView, data);
        return baseView;
    }

    public void SetToastView<D>(BaseView baseView, D data)
    {
        baseView.GetComponent<UIPanel>().SetSortingOrder(currentMaxOrder(baseViews[baseViews.Count - 1]) + 1, true);
        baseView.ui.visible = true;
        baseView.gameObject.SetActive(true);
        baseView.InitData(data);
    }

    /// <summary>
    /// 直接使用类名显示下一个View
    /// </summary>
    /// <param name="baseView">Base view.</param>
    public BaseView showNextView<T>()
    {
        BaseView baseView = getView<T>();
        if (baseViews.Count >= 2)
        {
            for (int i = 0; i < baseViews.Count; i++)
            {
                //if (baseViews[i].GetType() != typeof(TouchScreenView) && baseViews[i].GetType() != typeof(ShanbaiWaittimeView)
                //    && baseViews[i].GetType() != typeof(LoginWaitimeView)&& baseViews[i].GetType() != typeof(T))
                if(!ForeverTypes.Contains(baseViews[i].GetType()) && baseViews[i].GetType() != typeof(T))
                {
                    baseViews[i].ui.visible = false;
                    baseViews[i].gameObject.SetActive(false);
                }
            }
        }
        GRoot.inst.StopEffectSound();
        baseView.ui.visible = true;
        baseView.gameObject.SetActive(true);
        baseView.InitData();
        return baseView;
    }

    public BaseView showNextView<T, D>(D data)
    {
        BaseView baseView = getView<T>();
        if (baseViews.Count >= 2)
        {
            for (int i = 0; i < baseViews.Count; i++)
            {
                //if (baseViews[i].GetType() != typeof(TouchScreenView)&& baseViews[i].GetType() != typeof(ShanbaiWaittimeView)
                //    && baseViews[i].GetType() != typeof(LoginWaitimeView))
                if (!ForeverTypes.Contains(baseViews[i].GetType()) && baseViews[i].GetType() != typeof(T))
                {
                    baseViews[i].ui.visible = false;
                    baseViews[i].gameObject.SetActive(false);
                }
            }

        }
        GRoot.inst.StopEffectSound();

        baseView.ui.visible = true;
        baseView.gameObject.SetActive(true);
        baseView.InitData(data);
        return baseView;
    }

    public BaseView showNextPopupView<T>()
    {
        BaseView baseView = getView<T>();
        baseView.ui.visible = true;
        baseView.gameObject.SetActive(true);
        baseView.InitData();
        return baseView;
    }


    public BaseView showNextPopupView<T, D>(D data)
    {
        BaseView baseView = getView<T>();
        baseView.ui.visible = true;
        baseView.gameObject.SetActive(true);
        baseView.InitData(data);
        return baseView;
    }

    /// <summary>
    /// 进入主界面之前release
    /// </summary>
    /// <returns>The main view from login.</returns>
    /// <typeparam name="T">The 1st type parameter.</typeparam>
    public BaseView showViewWithReleaseOthers<T>()
    {
        for (int index = baseViews.Count - 1; index >= 0; index--)
        {
            //if (baseViews[index].GetType() != typeof(TouchScreenView)&& baseViews[index].GetType() != typeof(ShanbaiWaittimeView)
            //    && baseViews[index].GetType() != typeof(LoginWaitimeView))
            if (!ForeverTypes.Contains(baseViews[index].GetType()) && baseViews[index].GetType() != typeof(T))
            {
                baseViews[index].ui.visible = false;
                baseViews[index].gameObject.SetActive(false);
                UnityEngine.Object.Destroy(baseViews[index].gameObject);
                baseViews.RemoveAt(index);
            }

        }
        return showNextView<T>();
    }

    public void showBeforeView<T, V>()
    {
        for (int index = baseViews.Count - 1; index >= 0; index--)
        {
            //source
            if (baseViews[index].GetType() == typeof(V))
            {
                Debug.Log("move view  " + typeof(V));
                //baseViews[index].scaleToSmallAction = () =>
                //{
                //    Debug.Log("over");
                //    baseViews[index].ui.visible = false;
                //    baseViews[index].gameObject.SetActive(false);
                //};
                if (baseViews[index].isAnimationView)
                {
                    baseViews[index].OnHideAnimation();
                }
                else
                {
                    baseViews[index].ui.visible = false;
                    baseViews[index].gameObject.SetActive(false);
                }

                continue;
            }
            //target
            if (baseViews[index].GetType() == typeof(T))
            {
                baseViews[index].gameObject.SetActive(true);
                baseViews[index].ui.visible = true;
                baseViews[index].onShow();
            }

        }

    }


    public void ShowViewWithoutHideBefore<T, D>(D data)
    {
        BaseView baseView = getView<T>();
        baseView.InitData(data);

    }

    public void ShowViewWithoutHideBefore<T>()
    {
        BaseView baseView = getView<T>();
        baseView.InitData();
    }


    public void HideView<T>()
    {
        for (int index = baseViews.Count - 1; index >= 0; index--)
        {
            //source
            if (baseViews[index].GetType() == typeof(T))
            {
                baseViews[index].ui.visible = false;
                baseViews[index].gameObject.SetActive(false);
                UnityEngine.Object.Destroy(baseViews[index].gameObject);
                baseViews.RemoveAt(index);
                break;

            }
        }

    }

    public void OnHideView<T>()
    {
        for (int index = baseViews.Count - 1; index >= 0; index--)
        {
            //source
            if (baseViews[index].GetType() == typeof(T))
            {
                baseViews[index].onHide();
                break;

            }
        }

    }

    public void showMainView<V>()
    {
        showBeforeView<MainView, V>();
    }

    public GComponent CreateWindow<T>()
    {
        BaseWindow baseWindow = FindWindow<T>();
        if (baseWindow == null)
        {
            GComponent gComponent = ResourceMgr.Ins.CreateWindow<T>();
            return gComponent;
        }

        return baseWindow.asCom;
    }


    public void showWindow<T, D>(D data)
    {
        BaseWindow baseWindow = FindWindow<T>();
        if (baseWindow == null)
        {
            baseWindow = (BaseWindow)Activator.CreateInstance(typeof(T));
        }
        baseWindow.ShowWindow<D>(data);
        baseWindow.InitData();
    }


    public void showWindow<T>()
    {
        BaseWindow baseWindow = FindWindow<T>();
        if (baseWindow == null)
        {
            baseWindow = (BaseWindow)Activator.CreateInstance(typeof(T));
        }
        baseWindow.ShowWindow();
        baseWindow.InitData();
    }

    public void showWindow<T>(Message message)
    {
        BaseWindow baseWindow = FindWindow<T>();
        if (baseWindow == null)
        {
            baseWindow = (BaseWindow)Activator.CreateInstance(typeof(T));
        }
        baseWindow.message = message;
        baseWindow.ShowWindow();
        baseWindow.InitData();
    }


    public void ShowNetLoadinWindow()
    {
        showWindow<NetLoadingWindow>();
    }


    public void HideNetLoadinWindow()
    {
        if (NetLoadingWindow._window != null)
            NetLoadingWindow._window.HideWindow();
    }

    public void showErrorMsgWindow(string msg)
    {
        ErrorMsg errorMsg = new ErrorMsg(msg);
        showWindow<TipsWindow>(errorMsg);
    }


    public void InsertWindow(BaseWindow baseWindow)
    {
        addWindow(baseWindow);
    }


    public void DisposeWindows()
    {
        for (int i = baseWindows.Count - 1; i >= 0; i--)
        {
            baseWindows[i] = null;
            baseWindows.RemoveAt(i);

        }
    }

    #endregion 


}

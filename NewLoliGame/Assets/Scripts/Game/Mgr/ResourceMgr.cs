using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

public class ResourceMgr
{
    private static ResourceMgr instance;
    public static ResourceMgr Ins
    {
        get
        {
            if (instance == null)
                instance = new ResourceMgr();

            return instance;
        }
    }

    private ResourceMgr() { }



 


    public BaseView CreateView<T>()
    {
        ViewAttr viewAttr = (ViewAttr)System.Attribute.GetCustomAttribute(typeof(T), typeof(ViewAttr));
        if (viewAttr == null)
        {
            Debug.Log("BaseView viewAttr 配置异常");
            return null;
        }
        GameObject view = new GameObject();
        view.transform.parent = UIMgr.Ins.uiParent;


        //Debug.Log(uIPanel);
        UIPackage.AddPackage(viewAttr.PackagePath);
        UIPanel panel = view.AddComponent<UIPanel>();
        panel.packageName = viewAttr.PackageName;
        panel.componentName = viewAttr.ComponmentName;
        //设置renderMode的方式
        panel.container.renderMode = RenderMode.ScreenSpaceOverlay;


        //设置sortingOrder的方式
        int order = UIMgr.Ins.GetRenderOrder();
        panel.SetSortingOrder(order, true);
        if (typeof(T) == typeof(TouchScreenView))
        {
            panel.SetSortingOrder(1000, true);
        }
        if (typeof(T) == typeof(SplahView))
        {
            panel.SetSortingOrder(500, true);
        }
        if (typeof(T) == typeof(ShanbaiWaittimeView))
        {
            panel.SetSortingOrder(900, true);
        }
        if (typeof(T) == typeof(LoginWaitimeView))
        {
            panel.SetSortingOrder(800, true);
        }
        //设置hitTestMode的方式
        panel.SetHitTestMode(HitTestMode.Default);
        panel.gameObject.layer = 5;
        panel.fitScreen = FitScreen.FitSize;
        //panel.gameObject.SetActive(false);
        //最后，创建出UI
        panel.CreateUI();

        BaseView baseView = view.AddComponent(typeof(T)) as BaseView;
        view.transform.name = baseView.GetType().Name;
        baseView.ui = panel.ui;
        baseView.isAnimationView = viewAttr.AnimationView;
        panel.container.fairyBatching = true;
    return baseView;
    }

    public GComponent CreateWindow<T>()
    {
        ViewAttr viewAttr = (ViewAttr)System.Attribute.GetCustomAttribute(typeof(T), typeof(ViewAttr));
        if (viewAttr == null)
        {
            Debug.Log("BaseWindow viewAttr 配置异常");
            return null;
        }
        GObject gObject = UIPackage.CreateObject(viewAttr.PackageName, viewAttr.ComponmentName);
        if (gObject == null)
        {
            Debug.Log("BaseWindow create fail !!");
            return null;
        }
 
        return gObject.asCom;
    }
}

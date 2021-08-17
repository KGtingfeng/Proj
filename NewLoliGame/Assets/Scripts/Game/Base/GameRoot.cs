using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

/// <summary>
/// Game root. 游戏的根节点
/// </summary>
public class GameRoot : MonoBehaviour
{
    public GComponent ui;



    List<BaseView> baseViews = new List<BaseView>();



    private static GameRoot gameRoot;
    public static GameRoot GetSigleIns
    {
        get
        {
            return gameRoot;

        }
        private set
        {
            gameRoot = value;
        }
    }


    void Awake()
    {
        GetSigleIns = this;
        Application.targetFrameRate = 60;
        //DontDestroyOnLoad(gameObject);

    }


    // Start is called before the first frame update
    void Start()
    {

        initBaseConfig();

    }


    void initBaseConfig()
    {
        ui = FindObjectOfType<UIPanel>().ui;
        ui.fairyBatching = true;

        AddFirstPackageAndView();
    }


    /// <summary>
    /// 启动添加包 和第一个页面相关属性
    /// </summary>
    void AddFirstPackageAndView()
    {
        UIPackage.AddPackage(UINameConfig.LOLI_GAME_UI_PACKAGE_URL);
        EnterView<LoginView>(UINameConfig.LOGING_COM_NAME);

    }

    public void EnterView<T>(string viewName)
    {


        //BaseView view = FindBaseView<T>(viewName);
        ////create view
        //if(view == null)
        //{
            GObject gObject = UIPackage.CreateObject(UINameConfig.MAIN_PACKAGENAME, viewName);
            if (gObject != null)
            {
                //ui.RemoveChildren();
                //GComponent gComponent = gObject.asCom; 
                
                //gComponent.SetXY(Screen.width * 0.5f, Screen.height * 0.5f);
                //BaseView baseView = gComponent.displayObject.gameObject.AddComponent(typeof(T)) as BaseView;
                //baseView.gComponent = gComponent;
                //baseView.viewName = viewName;
                //ui.AddChild(gComponent);
                //baseView.InitUI(); 
                //InsertView(baseView);  
            }

        //}
        //else
        //{

        //}



            

         
    }



    //BaseView FindBaseView<T>(string viewName)
    //{
    //    foreach (var view in baseViews)
    //    //{
    //    //    if(view.GetType()== typeof(T) && view.viewName == viewName)
    //    //    {
    //    //        return view;
    //    //    }
    //    //}

    //    return null;

    //}


    void InsertView(BaseView baseView)
    {
        //if(baseViews.Count > 0)
        //{
        //    baseView.paretnNode = baseViews[baseViews.Count - 1];
        //    baseViews[baseViews.Count - 1].childNode = baseView;
        //    baseViews[baseViews.Count - 1].gComponent.visible = false;
        //    baseView.gComponent.visible = true;
        //}
        //else
        //{
        //    baseViews.Add(baseView);
        //    baseView.gComponent.visible = true;
                 
        //}
    }

    /// <summary>
    /// 新界面离开
    /// </summary>
    /// <param name="baseView">Base view.</param>
    public void ExitView(BaseView baseView)
    {

    }

    /// <summary>
    /// 回到上一个View
    /// </summary>
    /// <param name="baseView">Base view.</param>
    public void BackToFrontView(BaseView baseView)
    {

    }









}

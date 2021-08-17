using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
  loading弹出管理
 
 */
public class SmallLoadingMgr : MonoBehaviour
{

    public static SmallLoadingMgr smallLoadingMgr;
    enum LoadingType
    {
        None,
        /// <summary>
        /// 等待间隔时间
        /// </summary>
        Waiting,
        /// <summary>
        /// 显示中
        /// </summary>
        Show,

    }

    Dictionary<string, bool> keyValuePairs = new Dictionary<string, bool>();


    private void Awake()
    {
        smallLoadingMgr = this;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    public void Push(string key)
    {
        if (keyValuePairs.ContainsKey(key))
            keyValuePairs[key] = true;
        else
            keyValuePairs.Add(key, true);

        if (loadingType == LoadingType.None)
            StartCoroutine(DelayShowLoading(key));
    }


    public void Popup(string key)
    {
        if (keyValuePairs.ContainsKey(key))
            keyValuePairs[key] = false;
        //Debug.Log("LoadingTest 关闭加载......" + key);
        if (loadingType == LoadingType.Show)
        {
            UIMgr.Ins.HideNetLoadinWindow();
            loadingType = LoadingType.None;
           

        }

    }


    LoadingType loadingType = LoadingType.None;


    IEnumerator DelayShowLoading(string key)
    {
        //Debug.Log("LoadingTest开始加载倒计时......" + key);
        loadingType = LoadingType.Waiting;
        yield return new WaitForSeconds(0.5f);
        bool isShow;
        if (keyValuePairs.TryGetValue(key, out isShow) && isShow)
        {
            loadingType = LoadingType.Show;
            UIMgr.Ins.ShowNetLoadinWindow();
            //Debug.Log("LoadingTest需要显示加载......" + key);
        }
        else
        {
            loadingType = LoadingType.None;
        }
    }
}

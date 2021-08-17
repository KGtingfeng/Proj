using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using System;

[ViewAttr("Game/UI/D_Login", "D_Login", "TouchScreen")]
public class TouchScreenView : BaseView
{
    private static TouchScreenView ins;
    public static TouchScreenView Ins
    {
        get { return ins; }
    }
    GameObject prefab;

    GGraph holderGraph;

    GameObject fx;

    public override void InitUI()
    {
        base.InitUI();
        holderGraph = SearchChild("n0").asGraph;

        prefab = Resources.Load("Game/GFx/Prefabs/common_touch") as GameObject;
        ins = this;
        now = DateTime.Now;

        LoadTmpEffect();

    }


    private void LateUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GGraph holder = new GGraph();
            holder.SetSize(100, 100);
            //holder.draw
            ui.AddChild(holder);
            GameObject go = Instantiate(prefab);
            GoWrapper goWrapper = new GoWrapper(go);
            holder.SetNativeObject(goWrapper);

            Vector2 pos = Input.mousePosition;

            //转换为FairyGUI的屏幕坐标
            pos.y = Screen.height - pos.y;
            go.SetActive(true);

            Vector2 localPos = holder.GlobalToLocal(pos);
            holder.SetPosition(localPos.x, localPos.y, 0);

            AutoDestroy autoDestroy = holder.displayObject.gameObject.AddComponent<AutoDestroy>();
            autoDestroy.goWrapper = goWrapper;
        }

    }

    #region 界面过渡特效

    public void LoadTmpEffect()
    {
        GameObject tmp = Resources.Load(UrlUtil.GetGFXUrl("guochuangguang")) as GameObject;
        fx = Instantiate(tmp);
        GoWrapper goWrapper = new GoWrapper(fx);
        fx.gameObject.SetActive(false);
        holderGraph.SetNativeObject(goWrapper);
        holderGraph.displayObject.gameObject.transform.localScale = (Vector3.one * 162);

        holderGraph.SetPosition(336, 240, 0);
    }


    DateTime now;
    public void PlayTmpEffect()
    {
        if ((DateTime.Now - now).Seconds > 0.5f)
        {
            fx.gameObject.SetActive(false);
            fx.gameObject.SetActive(true);
            now = DateTime.Now;
        }

    }

    public void PlayChangeEffect(Action action)
    {
        StartCoroutine(ChangeEffect(action));
        
    }

    IEnumerator ChangeEffect(Action action)
    {
        PlayTmpEffect();
        yield return new WaitForSeconds(0.8f);
        action?.Invoke();
    }

    #endregion


    #region 金币飞出特效
    readonly int range = 100;
    readonly int maxNum = 20;
    readonly int minNum = 15;
    readonly float minScale = 1.5f;
    readonly float maxScale = 0.5f;

    WaitForSeconds sortWait = new WaitForSeconds(0.03f);

    public void CreateEffect(Vector2 start, Vector2 finish, bool isStar)
    {
        StartCoroutine(GoldEffect(start, finish, isStar));
    }

    IEnumerator GoldEffect(Vector2 start, Vector2 finish, bool isStar)
    {
        string name = isStar ? "UI_icon_xingxing" : "UI_icon_zuanshi";

        int count = UnityEngine.Random.Range(minNum, maxNum);
        for (int i = 0; i < count; i++)
        {
            string finalName = "";
            int fxRange = UnityEngine.Random.Range(1, 4);
            if (fxRange != 1)
                finalName = fxRange.ToString();

            float scale = UnityEngine.Random.Range(minScale, maxScale);
            GGraph gGraph = FXMgr.CreateEffectWithScale(ui, start, name + finalName, 162, 1.6f);
            gGraph.displayObject.gameObject.transform.localScale *= scale;
            int x = UnityEngine.Random.Range(-range, range);
            int y = UnityEngine.Random.Range(-range, range);
            Vector2 pos = new Vector2(start.x + x, start.y - y);
            yield return sortWait;
            gGraph.TweenMove(pos, 0.4f).OnComplete(() =>
            {
                gGraph.TweenMove(finish, 1f).SetEase(EaseType.CubicIn);
            });
        }
    }

    #endregion

    #region  道具toast
    WaitForSeconds longWait = new WaitForSeconds(0.6f);
    List<BaseView> toastList = new List<BaseView>();

    public void ShowPropsTost(List<PlayerProp> playerProps)
    {
        StartCoroutine(ShowGetPropsEffect(playerProps));
    }

    IEnumerator ShowGetPropsEffect(List<PlayerProp> playerProps)
    {
        foreach (PlayerProp prop in playerProps)
        {
            string url;
            if (prop.prop_id >= 100)
            {
                switch (prop.prop_type)
                {
                    case (int)TypeConfig.Consume.AvatarFrame:
                    case (int)TypeConfig.Consume.Title:
                        TinyItem tinyItem = new TinyItem()
                        {
                            id = prop.prop_id,
                            type = prop.prop_type,
                            num = prop.prop_count,
                        };
                        url = UrlUtil.GetPropsIconUrl(tinyItem);
                        break;
                    default:
                        TinyItem item = new TinyItem()
                        {
                            id = prop.prop_id,
                            type = 0,
                            num = prop.prop_count,
                        };
                        url = UrlUtil.GetPropsIconUrl(item);
                        break;
                }
            }
            else
            {
                url = CommonUrlConfig.GetConsumeItemUrl((TypeConfig.Consume)prop.prop_id);
            }

            TinyItem tiny = new TinyItem
            {
                type = prop.prop_id,
                url = url,
                num = prop.prop_count,
            };
            CreateFromPool(tiny);
            yield return longWait;
        }

    }

    void CreateFromPool(TinyItem prop)
    {
        if (toastList.Count > 0)
        {
            BaseView baseView = toastList[0];
            toastList.RemoveAt(0);
            UIMgr.Ins.SetToastView<TinyItem>(baseView, prop);
        }
        else
        {
            UIMgr.Ins.ShowToastView<CommonGetPropsToastView, TinyItem>(prop);
        }
    }

    public void RemoveToPool(BaseView baseView)
    {
        toastList.Add(baseView);
    }

    #endregion
}

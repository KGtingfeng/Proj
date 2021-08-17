using FairyGUI;
using Spine.Unity;
using UnityEngine;

/// <summary>
/// 特效工具，用来生成特效
/// </summary>
public class FXMgr
{
    /// <summary>
    /// 创建一个特效
    /// </summary>
    public static GGraph CreateEffectWithScale(GComponent ui, Vector3 pos, string fxName, float scale = 162, float time = 3f, int index = -1)
    {
        GGraph gGraph = new GGraph();
        if (index < 0)
            ui.AddChild(gGraph);
        else
            ui.AddChildAt(gGraph, index);
        GameObject fx = CreateObject(UrlUtil.GetGFXUrl(fxName));
        GoWrapper goWrapper = new GoWrapper(fx);
        gGraph.SetNativeObject(goWrapper);
        gGraph.displayObject.gameObject.transform.localScale = (Vector3.one * scale);

        gGraph.position = pos;
        if (time > 0)
        {
            AutoDestroy autoDestroy = gGraph.displayObject.gameObject.AddComponent<AutoDestroy>();
            autoDestroy.duration = time;
            autoDestroy.goWrapper = goWrapper;
        }
        return gGraph;
    }


    /// <summary>
    /// 在指定图形下创建一个特效
    /// </summary>
    /// /// <param name="gGraph"></param>
    /// <param name="pos">位置</param>
    /// <param name="FxName">特效名</param>
    /// <param name="scale">缩放</param>
    /// <param name="time">删除时间，-1不删除</param>
    /// <param name="index">层级，-1默认层级</param>
    public static GoWrapper CreateEffectWithGGraph(GGraph gGraph, Vector3 pos, string FxName, int scale = 1, float time = -1f)
    {
        GameObject fx = CreateObject(UrlUtil.GetGFXUrl(FxName));
        GoWrapper goWrapper = new GoWrapper(fx);
        gGraph.SetNativeObject(goWrapper);
        gGraph.displayObject.gameObject.transform.localScale = (Vector3.one * scale);

        gGraph.position = pos;
        if (time > 0)
        {
            AutoDestroy autoDestroy = gGraph.displayObject.gameObject.AddComponent<AutoDestroy>();
            autoDestroy.duration = time;
            autoDestroy.goWrapper = goWrapper;
        }

        return goWrapper;
    }

    /// <summary>
    /// 在指定图形下创建一个特效
    /// </summary>
    /// /// <param name="gGraph"></param>
    /// <param name="FxName">特效名</param>
    /// <param name="scale">缩放</param>
    /// <param name="time">删除时间，-1不删除</param>
    /// <param name="index">层级，-1默认层级</param>
    public static GoWrapper CreateEffectWithGGraph(GGraph gGraph, string FxName, Vector3 scale, float time = -1f)
    {
        GameObject fx = CreateObject(UrlUtil.GetGFXUrl(FxName));
        GoWrapper goWrapper = new GoWrapper(fx);
        gGraph.SetNativeObject(goWrapper);
        fx.transform.localScale = scale;
        if (time > 0)
        {
            AutoDestroy autoDestroy = gGraph.displayObject.gameObject.AddComponent<AutoDestroy>();
            autoDestroy.duration = time;
            autoDestroy.goWrapper = goWrapper;
        }

        return goWrapper;
    }
    /// <summary>
    /// 创建一个spine动画
    /// </summary>
    public static SkeletonAnimation LoadSpineEffect(string name, GGraph gGraph, Vector2 pos, int scale = 162, string animationName = "animation")
    {
        GameObject prefab = Resources.Load(UrlUtil.GetFxSpineUrl(name)) as GameObject;
        GameObject go = Object.Instantiate(prefab);
        SkeletonAnimation ani = null;
        Component[] components = go.GetComponentsInChildren(typeof(SkeletonAnimation));
        if (components != null && components.Length > 0)
        {
            ani = components[0] as SkeletonAnimation;
            ani.AnimationState.SetAnimation(0, animationName, true);
            ani.timeScale = 0;
        }
        GoWrapper goWrapper = new GoWrapper(go);
        goWrapper.wrapTarget = go;
        gGraph.SetNativeObject(goWrapper);
        goWrapper.scale = Vector3.one * scale;
        goWrapper.position = pos;
        return ani;
    }

    /// <summary>
    /// 设置头像框
    /// </summary>
    public static void SetFrame(GGraph gGraph, GLoader frameLoader, int frameId, float scale, Vector3 pos)
    {
        if (frameId > 2000)
        {
            gGraph.visible = true;
            frameLoader.visible = false;
            if (string.IsNullOrEmpty(gGraph.url))
            {
                gGraph.url = frameId.ToString();
                GameObject go = CreateObject(UrlUtil.GetAvatarFrame(frameId));
                GoWrapper goWrapper = new GoWrapper();
                goWrapper.SetWrapTarget(go, true);
                gGraph.SetNativeObject(goWrapper);

            }
            else if (gGraph.url != frameId.ToString())
            {
                GoWrapper wrapper = gGraph.displayObject as GoWrapper;
                gGraph.url = frameId.ToString();
                GameObject go = CreateObject(UrlUtil.GetAvatarFrame(frameId));
                Object.Destroy(wrapper.wrapTarget);
                wrapper.SetWrapTarget(go, true);

            }
            gGraph.displayObject.gameObject.transform.localScale = (Vector3.one * scale);
            gGraph.position = pos;
        }
        else
        {
            gGraph.visible = false;
            frameLoader.visible = true;
            frameLoader.url = UrlUtil.GetAvatarFrame(frameId);
        }
    }

    /// <summary>
    /// 设置称号
    /// </summary>
    public static void SetTitle(GGraph gGraph, GLoader gLoader, int level, float scale, Vector3 pos)
    {
        if (level > 2)
        {
            gGraph.visible = true;
            gLoader.visible = false;
            if (string.IsNullOrEmpty(gGraph.url))
            {
                gGraph.url = level.ToString();
                GameObject go = CreateObject(UrlUtil.GetTitleBg(level));
                GoWrapper goWrapper = new GoWrapper();
                goWrapper.SetWrapTarget(go, true);
                gGraph.SetNativeObject(goWrapper);
            }
            else if (gGraph.url != level.ToString())
            {
                GoWrapper wrapper = gGraph.displayObject as GoWrapper;
                gGraph.url = level.ToString();
                GameObject go = CreateObject(UrlUtil.GetTitleBg(level));
                Object.Destroy(wrapper.wrapTarget);
                wrapper.SetWrapTarget(go, true);

            }
            gGraph.displayObject.gameObject.transform.localScale = (Vector3.one * scale);
            gGraph.position = pos;
        }
        else
        {
            gGraph.visible = false;
            gLoader.visible = true;
            gLoader.url = UrlUtil.GetTitleBg(level);
        }
    }
    /// <summary>
    /// 从指定地址创建特效
    /// </summary>
    /// <param name="gGraph"></param>
    /// <param name="url"></param>
    /// <param name="scale"></param>
    /// <param name="pos"></param>
    public static GameObject CreateObject(string url)
    {
        GameObject prefab = Resources.Load(url) as GameObject;
        GameObject fx = Object.Instantiate(prefab);
        return fx;
    }

    public static GameObject CreateRoleSpine(int actorId, int skinId)
    {
        string path = skinId != 0 ? (actorId + "_" + skinId) : actorId.ToString();
        SkeletonAnimation skeletonAnimation = null;
        UnityEngine.Object prefab = ResourceUtil.LoadSpineByName(path);
        if (prefab == null)
            prefab = ResourceUtil.LoadSpineByName(actorId.ToString());

        GameObject go = (GameObject)GameObject.Instantiate(prefab);
        //go.transform.localPosition = new Vector3(30, -980, 1000);
        go.transform.localScale = Vector3.one;//新模型较大
        Component[] components = go.GetComponentsInChildren(typeof(SkeletonAnimation));
        if (components != null && components.Length > 0)
        {
            skeletonAnimation = components[0] as SkeletonAnimation;
        }

        SpineCtr.SetIdleAnimation(skeletonAnimation);
        return go;
    }


    /// <summary>
    /// 创建具有alpha效果的spine 角色
    /// </summary>
    /// <param name="actorId"></param>
    /// <param name="skinId"></param>
    public static void CreateRoleSpineForAlpha(int actorId, int skinId)
    {
        string path = skinId != 0 ? (actorId + "_" + skinId) : actorId.ToString();
        UnityEngine.Object prefab = ResourceUtil.LoadSpineByName(path);
        if (prefab == null)
            prefab = ResourceUtil.LoadSpineByName(actorId.ToString());
        RoleMgr.roleMgr.CreateRole(prefab);
        SpineCtr.SetIdleAnimation(RoleMgr.roleMgr.GetSkeletonAnimation);
    }

}

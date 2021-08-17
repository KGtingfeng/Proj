using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using System.IO;
using System;
using UnityEngine.Networking;

[ViewAttr("Game/UI/T_Common", "T_Common", "Frame_loading")]
public class LoadingView : BaseView
{
    GProgressBar progressBar;
    GGraph gGraph;
    GObject icon;
    float startX = 15;
    float finishX = 616;

    float iconstartX = 10;
    float iconfinishX = 584;
    List<string> AudioUrl = new List<string> {
        SoundConfig.STORY_AUDIO_MUSIC_URL,
        SoundConfig.STORY_AUDIO_EFFECT_URL,
        SoundConfig.STORY_AUDIO_SOUND_URL,
        SoundConfig.COMMON_AUDIO_EFFECT_URL,
        SoundConfig.COMMON_AUDIO_MUSIC_URL,
        SoundConfig.COMMON_MAIN_ROLE_URL
    };

    List<Type> Types = new List<Type>
    {
        typeof(InteractiveView),
        typeof(MainView),
        typeof(ShopMallView),
        typeof(StoryView),
        typeof(ShopMallView),
        typeof(SMSView),
        typeof(GameDetailView),
    };
    int doneCount;
    GLoader bgLoader;
    public override void InitUI()
    {
        base.InitUI();
        bgLoader = SearchChild("n5").asLoader;
        progressBar = SearchChild("n3").asProgress;
        gGraph = SearchChild("n3").asCom.GetChild("n3").asGraph;
        icon = SearchChild("n3").asCom.GetChild("n4");
    }

    public override void InitData()
    {
        base.InitData();
        bgLoader.url = UrlUtil.GetLoadingBgUrl();
        progressBar.max = 100;
        progressBar.value = 0;
        FXMgr.CreateEffectWithGGraph(gGraph, gGraph.xy, "loadstar");
        doneCount = 0;
        type = 0;
        StartCoroutine(LoadRes());
        UIMgr.Ins.HideView<SplahView>();
    }

    IEnumerator LoadRes()
    {
        doneCount += 10;
        yield return new WaitForEndOfFrame();

#if !UNITY_EDITOR
  string filePath = Application.persistentDataPath + "/t_special_words.txt";
        if (!File.Exists(filePath))
        {
            StartCoroutine(LoadSpeicail());
        }  
#endif     
        doneCount += 20;        
        StartCoroutine(LoadAllMusic());   
        doneCount += 40;
        StartCoroutine(LoadPackage());
        doneCount = 100;

    }


    IEnumerator LoadSpeicail()
    {
        string path = "jar:file://" + Application.dataPath + "!/assets/t_special_words.txt";
#if UNITY_IOS
      path = Application.dataPath + "/Raw /t_special_words.txt";
#endif
        using (UnityWebRequest www = UnityWebRequest.Get(path))
        {
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                //Debug.Log(www.downloadHandler.text);
                File.WriteAllText(Application.persistentDataPath + "/t_special_words.txt", www.downloadHandler.text);

            }
        }
    }

    /// <summary>
    /// 加载音乐
    /// </summary>
    IEnumerator LoadAllMusic()
    {

        foreach (string url in AudioUrl)
        {
            //Debug.LogError("家族 :" + url);
            yield return StartCoroutine(LoadMusic(url));
        }
        type++;
        yield return null;
    }

    IEnumerator LoadMusic(string url)
    {
        DirectoryInfo directory = new DirectoryInfo("Assets/Resources/" + url);
        FileInfo[] files = directory.GetFiles("*.mp3");
        foreach (FileInfo info in files)
        {
            ResourceRequest audioClip = Resources.LoadAsync<AudioClip>(url + info.Name);
            string path = url + info.Name;
            //Debug.LogError("*********************path  " + path);
            yield return audioClip;
            ResCacheMgr.Ins.CacheCommonAudioClip(path, audioClip.asset as AudioClip);
        }

        yield return new WaitForEndOfFrame();
    }


    /// <summary>
    /// 加载部分FairyGUI资源，否则场景过渡特效会很卡
    /// </summary>
    IEnumerator LoadPackage()
    {
        foreach (Type type in Types)
        {
            ViewAttr viewAttr = (ViewAttr)System.Attribute.GetCustomAttribute(type, typeof(ViewAttr));
            UIPackage package = UIPackage.AddPackage(viewAttr.PackagePath);
            package.LoadAllAssets();
            yield return new WaitForEndOfFrame();
        }
        type++;
        yield return null;
    }

    //IEnumerator LoadSpecialWords()
    //{
    //    SpecialWordsMgr.instance.Init();
    //    doneCount += 40;
    //    yield return null;
    //}

    int currentCount = 0;
    int type;
    private void Update()
    {
        if (currentCount < doneCount)
        {
            if (type == 2)
            {
                currentCount += 10;
            }
            else
            {
                currentCount += 2;
            }
            progressBar.value = currentCount;
            float currentX = startX + (finishX - startX) / 100 * currentCount;
            gGraph.x = currentX;
            float iconcurrentX = iconstartX + (iconfinishX - iconstartX) / 100 * currentCount;
            icon.x = iconcurrentX;
            if (currentCount >= 100)
                //StartCoroutine(LoadingFinish());
                UIMgr.Ins.showViewWithReleaseOthers<LoginView>();

        }
    }

    IEnumerator LoadingFinish()
    {
        yield return new WaitForSeconds(0.6f);
        UIMgr.Ins.showViewWithReleaseOthers<LoginView>();
    }

}

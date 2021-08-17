using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using FairyGUI;

[ViewAttr("Game/UI/J_Story", "J_Story", "frame_video")]
public class StoryVideoView : BaseView
{

    VideoPlayer videoPlayer;
    RenderTexture renderTexture;
    GLoader videoLoader;

    GamePointConfig gamePointConfig;
    public override void InitUI()
    {
        base.InitUI();
        renderTexture = Resources.Load(UrlUtil.GetVideoUrl("0")) as RenderTexture;
        videoLoader = SearchChild("n1").asLoader;

    }


    public override void InitData<D>(D data)
    {
        base.InitData(data);
        gamePointConfig = data as GamePointConfig;
        videoLoader.visible = false;
        StartCoroutine(PlayVideo());
        GRoot.inst.StopBgSound();

    }

    IEnumerator PlayVideo()
    {
        Object o = Resources.Load(UrlUtil.GetVideoUrl(gamePointConfig.card_id.ToString()));
        //Debug.LogError(" play  video  " + gamePointConfig.card_id + " " + o);
        GameObject go = GameObject.Instantiate(o) as GameObject;
        //Debug.Log("go=" + go);
        videoPlayer = go.GetComponent<VideoPlayer>();
        videoPlayer.time = 0;
        videoPlayer.isLooping = false;
        videoPlayer.playOnAwake = false;
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.targetTexture = renderTexture;
        videoPlayer.aspectRatio = VideoAspectRatio.NoScaling;
        videoPlayer.Prepare();
        yield return new WaitUntil(() => { return videoPlayer.isPrepared; });
        videoPlayer.Play();
        yield return new WaitUntil(() => { return videoPlayer.frame > 1; });
        videoLoader.texture = new NTexture(videoPlayer.targetTexture);
        StartCoroutine(CompletVideo((float)videoPlayer.length));
        videoLoader.visible = true;
    }

    IEnumerator CompletVideo(float time)
    {
        yield return new WaitForSeconds(time);
        CallBackList callBackList = new CallBackList();
        callBackList.callBack1 = () =>
        {
            videoPlayer.Stop();
            AudioClip audioClip = ResourceUtil.LoadBackGroundMusic(SoundConfig.CommonMusicId.BgId);
            GRoot.inst.PlayBgSound(audioClip);
            NormalInfo normalInfo = new NormalInfo();
            normalInfo.index = gamePointConfig.point1;
            EventMgr.Ins.DispachEvent(EventConfig.STORY_RECORD_SELECT_NODE, normalInfo);
            onHide();
        };

        UIMgr.Ins.showNextPopupView<StoryAnimationView, CallBackList>(callBackList);

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using FairyGUI;
public class VideoCallMoudle : BaseMoudle
{
 
    Controller controller;
    GGraph gGraph;

    public override void InitUI()
    {
        controller = ui.GetController("c1");
        gGraph = SearchChild("n2").asCom.GetChild("n17").asGraph;
        InitEvent();
    }

    public override void InitData()
    {
        controller.selectedIndex = 2;
        initVideoClip();
    }

    void initVideoClip()
    {
        GameObject camera = GameObject.Find("Stage Camera");
        var videoClip = Resources.Load<VideoClip>("Video/OP");
        if (videoClip == null)
            return;

        GameObject go = new GameObject("Audio");

        GoWrapper goWrapper = new GoWrapper(go);
        gGraph.SetNativeObject(goWrapper);

        var videoPlayer = go.AddComponent<VideoPlayer>();
        //var videoPlayer = gGraph.displayObject.gameObject.AddComponent<VideoPlayer>();
        videoPlayer.playOnAwake = false;
        videoPlayer.clip = videoClip;
        videoPlayer.renderMode = VideoRenderMode.CameraFarPlane;
        videoPlayer.targetCamera = camera.GetComponent<Camera>();
        videoPlayer.targetCameraAlpha = 1f;
        videoPlayer.Play();



    }


    public override void InitEvent()
    {

    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;


[ViewAttr("Game/UI/X_Beginner_guidance", "X_Beginner_guidance", "frame_newbie")]
public class NewbieXinlingView : BaseView
{

    GTextField content;
    GuiderInfoLinked info;
    public override void InitUI()
    {
        base.InitUI();
        content = SearchChild("n3").asCom.GetChild("n7").asTextField;
        InitEvent();
    }


    public override void InitEvent()
    {
        base.InitEvent();
        SearchChild("n2").onClick.Set(onHide);
    }

    public override void InitData<D>(D data)
    {
        base.InitData(data);
        info = data as GuiderInfoLinked;
        content.text = info.guiderInfo.contents;
        GRoot.inst.StopDialogSound();
        if (info.guiderInfo.actor_voice != "0")
        {
            Debug.LogError("voice " + info.guiderInfo.flow + "_" + info.guiderInfo.step);
            AudioClip audioClip = Resources.Load<AudioClip>(UrlUtil.GetNewbieBgmUrl(info.guiderInfo.actor_voice));
            GRoot.inst.PlayDialogSound(audioClip);
        }

    }

    public override void onHide()
    {
        if (!Stage.inst._audioDialog.isPlaying)
        {
            info.callback?.Invoke();
            base.onHide();
        }
    }
}

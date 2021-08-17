using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

[ViewAttr("Game/UI/Fx_Ani", "Fx_Ani", "Ani_zhuanchangwithfit1")]
public class StoryAnimationView : BaseView
{
    CallBackList callBackList;
    Transition transition;
    GComponent maskCom;
    public override void InitUI()
    {

        base.InitUI();
        GComponent gComponent = SearchChild("n0").asCom;
        maskCom = SearchChild("n0").asCom;
        maskCom.x = -maskCom.width;

    }

    public override void InitData<D>(D data)
    {
        base.InitData(data);
        callBackList = data as CallBackList;
        isTrigger = true;
        maskCom.TweenMoveX(Screen.width, 1f).SetEase(EaseType.Linear).OnComplete(() =>
      {
          callBackList.callBack2?.Invoke();
          UIMgr.Ins.HideView<StoryAnimationView>();

      });

    }

    bool isTrigger;
    float tmpTime = 0;
    private void Update()
    {
        if (isTrigger)
        {
            tmpTime += Time.deltaTime;
            if (tmpTime >= 0.5f)
            {
                isTrigger = false;
                callBackList.callBack1?.Invoke();
            }
        }
    }



}

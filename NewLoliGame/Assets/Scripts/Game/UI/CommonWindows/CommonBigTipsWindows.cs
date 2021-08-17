using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using FairyGUI;


[ViewAttr("Game/UI/T_Common", "T_Common", "Frame_tips")]
public class CommonBigTipsWindows : BaseWindow
{
    Controller controller;
    GLoader imgLoader;
    CommonBigTipsInfo commonBigTipsInfo;
    GTextField contextText;

    int TIPS_TEXT = 0;
    int TIPS_IMG = 1;

    public override void InitUI()
    {
        CreateWindow<CommonBigTipsWindows>();
        controller = contentPane.GetController("c1");
        imgLoader = SearchChild("n10").asLoader;
        contextText = SearchChild("n9").asTextField;
        InitEvent();
    }

    public override void ShowWindow<D>(D data)
    {
        base.ShowWindow(data);
        commonBigTipsInfo = data as CommonBigTipsInfo;


    }


    public override void InitEvent()
    {
        SearchChild("n7").onClick.Set(Close);
        //SearchChild("n11").onClick.Add(Close);
    }



    public override void InitData()
    {
        base.InitData();
        if (commonBigTipsInfo != null)
        {
            int index = commonBigTipsInfo.isShowText ? TIPS_TEXT : TIPS_IMG;
            controller.selectedIndex = index;

            if (commonBigTipsInfo.isShowText)
            {
                contextText.text = commonBigTipsInfo.context;
            }
            else
            {
                imgLoader.url = commonBigTipsInfo.url;
            }

        }

    }

    public override void onPopupClosed()
    {
        if (commonBigTipsInfo != null && commonBigTipsInfo.callBack != null)
        {
            commonBigTipsInfo.callBack();
        }
    }

    void Close()
    {
        Hide();
        if (commonBigTipsInfo != null && commonBigTipsInfo.callBack != null)
        {
            commonBigTipsInfo.callBack();
        }
    }

    protected override void OnShown()
    {

    }
}

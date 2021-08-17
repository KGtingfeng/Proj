using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

public class SMSKongMoudle : BaseMoudle
{
    public enum loaderType
    {
        //聊天记录
        SMS,
        //通话录音
        CALL,
        //朋友圈
        MOMENTS,
    }
    readonly Dictionary<loaderType, string> loaderList = new Dictionary<loaderType, string>
    {
        {loaderType.SMS, "ui://srheglvmjhz31q"},
        {loaderType.CALL, "ui://srheglvmjhz31u"},
        { loaderType.MOMENTS,"ui://srheglvmh8ad2a"},
    };

    GTextField gTitle;
    GTextField contentText;
    public override void InitMoudle(GComponent gComponent, int controllerIndex)
    {
        base.InitMoudle(gComponent, controllerIndex);
        InitUI();
        InitEvent();
    }

    public override void InitUI()
    {
        base.InitUI();
        gTitle = SearchChild("n407").asTextField;
        contentText = SearchChild("n58").asTextField;
    }

    public override void InitData<D>(D data)
    {
        base.InitData(data);
        string str = data as string;
        int type = int.Parse(str);
        switch (type)
        {
            case (int)loaderType.SMS:
                gTitle.text = "聊天记录";
                contentText.text = "暂时没有聊天记录，快去和他聊一聊吧！";
                break;
            case (int)loaderType.CALL:
                gTitle.text = "通话记录";
                contentText.text = "暂时没有通话记录，快去和他聊一聊吧！";
                break;
            case (int)loaderType.MOMENTS:
                gTitle.text = "朋友圈";
                contentText.text = "暂时没有朋友圈，快去和他聊一聊吧！";
                break;
        }
    }
}

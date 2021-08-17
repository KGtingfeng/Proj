using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallDataMgr
{
    private CallDataMgr() { }

    private static CallDataMgr ins;
    public static CallDataMgr Ins
    {
        get
        {
            if (ins == null)
                ins = new CallDataMgr();
            return ins;
        }
    }

    //是否直接展示消息
    public bool isViewSms = false;

    /// <summary>
    /// 标记未正在接听电话，用于推送判断是否调用MessagecallView
    /// </summary>
    bool _isCalling;
    public bool isCalling {
        get { return _isCalling; }
        set { _isCalling = value; }
    }

    //当前信息包
    public GameCellSmsConfig cellSmsConfig;
    public GameInitCardsConfig cardsConfig;
    /// <summary>
    /// 预设一个临时数据
    /// </summary>
    public void RefreshSMSInfo(int sms_id)
    {
        cellSmsConfig = DataUtil.GetGameCellSmsConfig(sms_id);
        if(cellSmsConfig!=null)
            cardsConfig = DataUtil.GetGameInitCard(cellSmsConfig.actor_id);

    }
    
    public void Dispose()
    {
        cellSmsConfig = null;
        cardsConfig = null;
        isViewSms = false;
        isCalling = false;
    }
    
}

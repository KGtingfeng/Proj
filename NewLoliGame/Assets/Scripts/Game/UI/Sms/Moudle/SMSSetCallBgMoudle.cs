using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

public class SMSSetCallBgMoudle : BaseMoudle
{
    //拥有的时刻图
    List<GameMomentConfig> GameMomentBgs
    {
        get { return SMSDataMgr.Ins.GameMomentBgConfigs; }
    }
    /// <summary>
    /// 当前角色拥有的全部背景（包括默认）
    /// </summary>
    List<GameCellVoiceBackgroundConfig> ownCellVoiceBgConfigs;

    int selectIndex = 0;
    GameCellVoiceBackgroundConfig selectBgNameConfig;

    CellVoiceBgSaveInfo cellVoiceBgSaveInfo;
    GList bgList;
    GLoader bgLoader;
    public override void InitMoudle(GComponent gComponent, int controllerIndex)
    {
        base.InitMoudle(gComponent, controllerIndex);
        InitUI();
        InitEvent();
    }

    public override void InitUI()
    {
        base.InitUI();
        bgList = SearchChild("n69").asList;
        bgLoader = SearchChild("n65").asCom.GetChild("n64").asLoader;
    }

    public override void InitEvent()
    {
        base.InitEvent();
        SearchChild("n67").onClick.Set(OnClickComfirm);
    }

    public override void InitData<D>(D data)
    {
        base.InitData(data);
        GetCurrentBg();
    }

    void InitCurrentViewInfo()
    {
        bgList.SetVirtual();
        bgList.itemRenderer = RendererItem;
        bgList.numItems = ownCellVoiceBgConfigs.Count;
        bgList.onClickItem.Set(OnClickItem);

        bgList.selectedIndex = selectIndex;
    }

    void GetAllVoiceBgs()
    {
        ownCellVoiceBgConfigs = new List<GameCellVoiceBackgroundConfig>();
        ownCellVoiceBgConfigs = JsonConfig.GameCellVoiceBackgroundConfigs.FindAll(a => a.limit.Equals("") && a.actor_id == SMSDataMgr.Ins.CurrentRole && a.type == 1);

        for (int i = 0; i < GameMomentBgs.Count; i++)
        {
            GameCellVoiceBackgroundConfig gameCellVoice = GetCellBgConfig(GameMomentBgs[i].moment_id);
            if (gameCellVoice != null)
                ownCellVoiceBgConfigs.Add(gameCellVoice);
        }
        selectIndex = 0;
        selectBgNameConfig = ownCellVoiceBgConfigs[0];
    }

    void RendererItem(int index, GObject gObject)
    {
        GComponent gCom = gObject.asCom;
        GLoader bgLoader = gCom.GetChild("n2").asCom.GetChild("n1").asLoader;
        string url = ownCellVoiceBgConfigs[index].assets;
        bgLoader.url = UrlUtil.GetCallBgUrl(url);

        if (cellVoiceBgSaveInfo != null && cellVoiceBgSaveInfo.value != null && cellVoiceBgSaveInfo.value.Equals(url))
        {
            selectIndex = index;
            selectBgNameConfig = ownCellVoiceBgConfigs[index];
        }
    }

    private void OnClickItem(EventContext context)
    {
        int selectedIndex = bgList.GetChildIndex((GObject)context.data);
        int itemIndex = bgList.ChildIndexToItemIndex(selectedIndex);

        selectBgNameConfig = ownCellVoiceBgConfigs[itemIndex];
        bgLoader.url = UrlUtil.GetCallBgUrl(selectBgNameConfig.assets);

    }

    void OnClickComfirm()
    {
        if (selectBgNameConfig.assets.Equals(""))
            return;
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("key", CellVoiceBgSaveInfo.Key + SMSDataMgr.Ins.CurrentRole);
        wWWForm.AddField("value", selectBgNameConfig.assets);
        GameMonoBehaviour.Ins.RequestInfoPost<CellVoiceBgSaveInfo>(NetHeaderConfig.STORY_SAVE, wWWForm, ()=> {
            UIMgr.Ins.showErrorMsgWindow("修改成功！");
        });
    }
    void GetCurrentBg()
    {
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("key", CellVoiceBgSaveInfo.Key + SMSDataMgr.Ins.CurrentRole);
        Debug.LogError(CellVoiceBgSaveInfo.Key + SMSDataMgr.Ins.CurrentRole);
        GameMonoBehaviour.Ins.RequestInfoPost(NetHeaderConfig.STORY_LOAD, wWWForm, (CellVoiceBgSaveInfo cellVoiceBgSaveInfo) =>
        {
            if (cellVoiceBgSaveInfo != null)
            {
                this.cellVoiceBgSaveInfo = cellVoiceBgSaveInfo;
                GetAllVoiceBgs();
                InitCurrentViewInfo();
                bgLoader.url = UrlUtil.GetCallBgUrl(selectBgNameConfig.assets);
                bgLoader.alpha = 0.2f;
            }

        });
    }

    public GameCellVoiceBackgroundConfig GetCellBgConfig(int moment_id)
    {
        return JsonConfig.GameCellVoiceBackgroundConfigs.Find(a => a.tinyItem != null && a.tinyItem.id == moment_id && a.actor_id == SMSDataMgr.Ins.CurrentRole);
    }
}

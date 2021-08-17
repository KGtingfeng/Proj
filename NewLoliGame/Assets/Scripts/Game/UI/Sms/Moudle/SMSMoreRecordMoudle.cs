using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

public class SMSMoreRecordMoudle : BaseMoudle
{
    GList recordList;
    List<SmsListIndex> cellLists;
    public override void InitMoudle(GComponent gComponent, int controllerIndex)
    {
        base.InitMoudle(gComponent, controllerIndex);
        InitUI();
        InitEvent();
    }

    public override void InitUI()
    {
        base.InitUI();
        recordList = SearchChild("n56").asList;
    }


    public override void InitData<D>(D data)
    {
        base.InitData(data);
        cellLists = data as List<SmsListIndex>;
        Refresh();
    }

    public override void InitData()
    {
        base.InitData();
        Refresh();
    }

    void Refresh()
    {
        recordList.SetVirtual();
        recordList.itemRenderer = RendererItem;
        recordList.numItems = cellLists.Count;
        recordList.onClickItem.Set(OnClickItem);
        recordList.RefreshVirtualList();
    }

    void RendererItem(int index, GObject gObject)
    {
        GButton gButton = gObject.asButton;
        GameCellSmsConfig smsConfig = JsonConfig.GameCellSmsConfigs.Find(a => a.id == cellLists[index].sms_id);

        if (smsConfig != null)
        {
            gButton.title = smsConfig.title;
            if (smsConfig.content_type == GameCellSmsConfig.TYPE_DAILY)
                gButton.GetChild("n0").asCom.GetController("c1").selectedIndex = 1;
            else
                gButton.GetChild("n0").asCom.GetController("c1").selectedIndex = 0;
        }
    }


    void OnClickItem(EventContext context)
    {
        int selectedIndex = recordList.GetChildIndex((GObject)context.data);
        int itemIndex = recordList.ChildIndexToItemIndex(selectedIndex);
        baseView.GoToMoudle<SMSRecordMoudle, SmsListIndex>((int)SMSView.MoudleType.TYPE_RECORD, cellLists[itemIndex]);
    }

}

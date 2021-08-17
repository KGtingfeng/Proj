using System;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;
using System.Text;

[ViewAttr("Game/UI/F_Room", "F_Room", "Frame_clock", true)]
public class AlarmClockView : BaseView
{
    public static AlarmClockView ins;

    GList hourList;
    GList secondList;
    GList weekList;
    GButton openBtn;
    GButton nameBtn;
    GTextField nameText;
    int actorId;
    string alarm;

    GObject[] weeks;
    AlarmClockInfo info;
    public override void InitUI()
    {
        base.InitUI();
        hourList = SearchChild("n15").asList;
        secondList = SearchChild("n16").asList;
        weekList = SearchChild("n7").asList;
        openBtn = SearchChild("n5").asButton;
        nameBtn = SearchChild("n21").asButton;
        weeks = weekList.GetChildren();
        nameText = nameBtn.GetChild("n2").asTextField;
        InitEvent();
        ins = this;
    }

    public override void InitEvent()
    {
        base.InitEvent();
        nameBtn.onClick.Set(() =>
        {
            ChooseSound();
        });
        SearchChild("n1").onClick.Set(() =>
        {
            SaveAlarm();
            OnHideAnimation();
        });
        EventMgr.Ins.RegisterEvent<int>(EventConfig.CHANGE_RING, RefreshName);
    }

    public override void InitData<D>(D data)
    {
        OnShowAnimation();
        base.InitData(data);
        info = data as AlarmClockInfo;
        hourList.scrollPane.snapToItem = true;
        hourList.SetVirtualAndLoop();
        hourList.itemRenderer = RenderItem;
        hourList.numItems = 24;
        secondList.scrollPane.snapToItem = true;
        secondList.SetVirtualAndLoop();
        secondList.itemRenderer = RenderItem;
        secondList.numItems = 60;
        weekList.onClickItem.Set(OnClickItem);
        InitAlarm();

        //if (GameData.isOpenGuider)
        //{
        //    StoryCacheMgr.storyCacheMgr.GetStoryGameSave("Newbie", GameGuideConfig.TYPE_ALARM, (storyGameSave) =>
        //    {
        //        if (storyGameSave.IsDefault)
        //        {
        //            GameData.guiderCurrent = GuiderInfoLinked.GetCurrGuiderInfo(GameGuideConfig.TYPE_ALARM, 1);
        //            UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);
        //            StoryCacheMgr.storyCacheMgr.SaveProgress("Newbie", "1", GameGuideConfig.TYPE_ALARM);
        //        }
        //    });
        //}
    }

    void InitAlarm()
    {
        alarm = Convert.ToString(info.alarm, 2).PadLeft(3, '0');
        openBtn.selected = alarm[1] == '1';
        string[] time = info.time_settings_new.Split(' ');
        hourList.ScrollToView(int.Parse(time[2]));
        secondList.ScrollToView(int.Parse(time[1]));
        string[] week = time[5].Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (GObject gObject in weeks)
        {
            gObject.asButton.selected = false;
        }
        foreach (string num in week)
        {
            weeks[int.Parse(num) - 1].asButton.selected = true;
        }
        actorId = info.actor_id;
        RefreshName(info.actor_id);
    }

    void RenderItem(int index, GObject obj)
    {
        GComponent itemCom = obj.asCom;
        itemCom.GetChild("n0").asTextField.text = index.ToString();
    }

    readonly float snap = 40;
    int GetHour()
    {
        int hourIndex = (int)(hourList.scrollPane.scrollingPosY / snap);
        int mod = hourIndex % 24;

        return mod;
    }

    int GetSecond()
    {
        int secondIndex = (int)(secondList.scrollPane.scrollingPosY / snap);
        int mod = secondIndex % 60;
        return mod;
    }

    void RefreshName(int actorId)
    {
        this.actorId = actorId;
        GameInitCardsConfig gameInitCards = JsonConfig.GameInitCardsConfigs.Find(a => a.card_id == actorId);
        nameText.text = gameInitCards.name_cn;
    }

    void OnClickItem(EventContext context)
    {
        GButton gButton = (GButton)context.data;
        bool haveUp = false;
        foreach (GObject gObject in weeks)
        {
            if (gObject.asButton.selected)
            {
                haveUp = true;
                break;
            }
        }
        if (!haveUp)
        {
            UIMgr.Ins.showErrorMsgWindow("闹钟至少选择一天!");
            gButton.selected = true;
        }
    }

    public void ChooseSound()
    {
        NormalInfo normalInfo = new NormalInfo();
        normalInfo.index = actorId;
        UIMgr.Ins.showNextPopupView<ChooseSoundView, NormalInfo>(normalInfo);
    }

    /// <summary>
    /// 设置时间
    /// 秒、分、时、天、月、星期、年
    /// time=0 15 10 * ? 1,2,5,6 *   表示星期天，周一，周四，周五上午10点15
    /// </summary>
    void SaveAlarm()
    {
        StringBuilder weekStr = new StringBuilder();
        for (int i = 0; i < weeks.Length; i++)
        {
            if (weeks[i].asButton.selected)
            {
                weekStr.Append((i + 1) + ",");
            }
        }
        if (weekStr.Length > 1)
            weekStr.Remove(weekStr.Length - 1, 1);
        string time = "0 " + GetSecond() + " " + GetHour() + " ? * " + weekStr.ToString() + " *";
        int status = openBtn.selected ? 2 : 1;

        if (NeedSave(time, status))
        {
            WWWForm wWWForm = new WWWForm();
            wWWForm.AddField("status", status);
            wWWForm.AddField("actorId", actorId);
            wWWForm.AddField("time", time);
            GameMonoBehaviour.Ins.RequestInfoPost<AlarmClockInfo>(NetHeaderConfig.SAVE_ALARM, wWWForm, null);
            char[] alarms = alarm.ToCharArray();
            alarms[1] = char.Parse((status - 1).ToString());
            alarm = new string(alarms);
            int i = Convert.ToInt32(alarm, 2);
            GameData.Player.alarm = i;
        }

    }

    bool NeedSave(string time, int status)
    {
        string oldStatus = alarm[1].ToString();
        if (oldStatus != (status - 1).ToString())
            return true;
        if (time != info.time_settings_new)
            return true;
        if (actorId != info.actor_id)
            return true;
        return false;
    }

    public void NewbieOpen()
    {
        openBtn.selected = true;
    }
}

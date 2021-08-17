using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;


[ViewAttr("Game/UI/T_Common", "T_Common", "Frame_Scrollselection", true)]
public class SelectCalendarView : BaseView
{
    //组
    GGroup gGroup;
    GObject tips;

    GList yearList;
    GList monthList;
    GList dayList;
    //开始年份
    int startYear;
    Color32 color32 = new Color32(236, 119, 147, 255);
    int year;
    int month;
    int day;

    bool isModify;
    public override void InitUI()
    {
        base.InitUI();
        yearList = SearchChild("n11").asList;
        monthList = SearchChild("n14").asList;
        dayList = SearchChild("n15").asList;
        gGroup = SearchChild("n33").asGroup;
        tips = SearchChild("n34");
        InitEvent();
    }



    public override void InitData()
    {
        isModify = PlayerHeadView.ins != null;
        setCurrentPosition();
        OnShowAnimation();
        base.InitData();

        if (isModify)
            tips.text = "(一年只能修改一次）";
        else
            tips.text = "                    ";

        InitYearsList();
        InitMonthsList();
        int monthDay = DateTime.DaysInMonth(2000, 1);
        InitDaysList(monthDay);

        ChangeYearTextColor();
        ChangeMonthTextColor();
        ChangeDayTextColor();
    }


    public override void InitEvent()
    {
        base.InitEvent();
        SearchChild("n10").onClick.Set(() =>
        {
            year = GetYear();
            month = GetMonth();
            day = GetDay();
            DateTime nowTime = DateTime.Now;
            //Debug.Log(nowTime.ToString());
            if (year > nowTime.Year ||
             (nowTime.Year == year && month > nowTime.Month) ||
              (nowTime.Year == year && nowTime.Month == month && day > nowTime.Day))
            {
                UIMgr.Ins.showErrorMsgWindow(MsgException.BIRTHDAY_ERROR);
            }
            else
            {
                SychronizedBirthdayInfo();
                OnHideAnimation();
            }

            //Debug.Log(year + "年" + month + "月" + day + "日");

        });
        //close
        SearchChild("n9").onClick.Set(OnHideAnimation);
    }

    Vector2 midPivot = new Vector2(0.5f, 0.5f);
    Vector2 downPivot = new Vector2(0.5f, 0.75f);

    void setCurrentPosition()
    {
        Vector3 vector = gGroup.position;
        float pos = isModify ? vector.y : 808;
        pivot = isModify ? midPivot : downPivot;
        gGroup.SetPosition(vector.x, pos, vector.z);

    }

    /// <summary>
    /// 同步生日信息
    /// </summary>
    void SychronizedBirthdayInfo()
    {
        if (isModify)
        {
            PlayerHeadView.ins.ReqEditPlayerBirthday(year, month, day);
        }
        else
        {
            CreateRoleView.view.SychronizedBirthday(year, month, day);
        }
    }

    void InitYearsList()
    {
        startYear = 1900;
        yearList.scrollPane.snapToItem = true;
        yearList.SetVirtualAndLoop();
        yearList.itemRenderer = YearsListItem;
        yearList.numItems = 200;
        yearList.ScrollToView(99);
        yearList.scrollPane.onScrollEnd.Set(onScrollEndYearList);

    }


    private void onScrollEndYearList(EventContext context)
    {
        ChangeYearTextColor();
    }

    private void ChangeYearTextColor()
    {
        int years = GetYear();
        ChangeTextColor(years + "年", yearList);
    }

    private void ChangeTextColor(string param, GList gList)
    {
        for (int i = 0; i < 3; i++)
        {
            GComponent itemCom = gList.GetChildAt(i).asCom;
            GTextField gTextField = itemCom.GetChild("n0").asTextField;
            if (gTextField.text.Equals(param))
            {

                gTextField.color = color32;
            }
            else
            {
                gTextField.color = Color.black;
            }

        }
    }

    void YearsListItem(int index, GObject obj)
    {
        GComponent itemCom = obj.asCom;
        itemCom.GetChild("n0").asTextField.text = startYear + index + "年";
        //itemCom.GetChild("n0").asTextField.color = Color.black;
    }


    void InitMonthsList()
    {

        monthList.scrollPane.snapToItem = true;
        monthList.SetVirtualAndLoop();
        monthList.itemRenderer = MonthsListItem;
        monthList.numItems = 12;
        monthList.ScrollToView(11);
        monthList.scrollPane.onScrollEnd.Set(onScrollEndMonthList);
    }

    private void onScrollEndMonthList(EventContext context)
    {
        ChangeMonthTextColor();
    }

    private void ChangeMonthTextColor()
    {
        string tmp = GetMonth() + "月";
        ChangeTextColor(tmp, monthList);
    }

    void MonthsListItem(int index, GObject obj)
    {
        GComponent itemCom = obj.asCom;
        itemCom.GetChild("n0").asTextField.text = 1 + index + "月";

    }



    void InitDaysList(int monthDay)
    {
        dayList.scrollPane.snapToItem = true;
        dayList.SetVirtualAndLoop();
        dayList.itemRenderer = DaysListItem;
        dayList.numItems = monthDay;

        dayList.onTouchBegin.Set(() =>
        {
            int tmpYear = GetYear();
            int tmpMonth = GetMonth();
            if (tmpYear != year || tmpMonth != month)
            {
                year = tmpYear;
                month = tmpMonth;

                int tmpDay = DateTime.DaysInMonth(year, month);
                if (tmpDay != dayList.numItems)
                {
                    InitDaysList(tmpDay);
                }
            }

        });
        dayList.ScrollToView(monthDay - 1);
        dayList.scrollPane.onScrollEnd.Set(onScrollEndDayList);
    }

    private void onScrollEndDayList(EventContext context)
    {
        ChangeDayTextColor();
    }

    private void ChangeDayTextColor()
    {
        ChangeTextColor(GetDay() + "日", dayList);
    }

    float snap = 126;
    int GetMonth()
    {
        float monthIndex = monthList.scrollPane.scrollingPosY / snap;
        //Debug.LogError("*******************monthIndex " + monthIndex + "    posY  " + monthList.scrollPane.scrollingPosY);
        float mod = monthIndex % 12;
        if (mod == 11)
            return 1;
        return Mathf.FloorToInt(monthIndex % 12.0f) + 2;
    }

    int GetYear()
    {
        float index = yearList.scrollPane.scrollingPosY / snap;
        return Mathf.FloorToInt((index + 1) % 200) + startYear;
        //return Mathf.FloorToInt(index % 200) + 1 + startYear;
    }

    int GetDay()
    {
        float daysIndex = dayList.scrollPane.scrollingPosY / snap;
        float mod = daysIndex % dayList.numItems;
        if ((mod + 1) == dayList.numItems)
        {
            return 1;
        }
        return Mathf.FloorToInt(daysIndex % dayList.numItems) + 2;
    }

    void DaysListItem(int index, GObject obj)
    {
        GComponent itemCom = obj.asCom;
        itemCom.GetChild("n0").asTextField.text = (1 + index) + "日";

    }
}

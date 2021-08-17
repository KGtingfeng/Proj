using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

[ViewAttr("Game/UI/X_Choose doll", "X_Choose doll", "Choose")]
public class ChooseRoleView : BaseView
{

    public enum MoudleType
    {
        ChooseDoll = 0,
        GetDoll,
        Talk,
    };


    Dictionary<MoudleType, string> moudleNamePairs = new Dictionary<MoudleType, string>()
    {
        {MoudleType.ChooseDoll,"n0"},
        {MoudleType.GetDoll,"n1"},
        {MoudleType.Talk,"n2"},
    };



    public static readonly Dictionary<int, string> bodyUrl = new Dictionary<int, string>() {
        {1,"cha_yeluoli"},
        {2,"cha_lankongque"},
        {3,"cha_liangcai"},
        {4,"cha_moli"},
        {5,"cha_feiling"},
        {6,"cha_baiguangying"},
        {7,"cha_heixiangling"}
    };

    public static readonly Dictionary<int, string> topUrl = new Dictionary<int, string>() {
        {1,"ptn_name_yeluoli"},
        {2,"ptn_name_lankongque"},
        {3,"ptn_name_liangcai"},
        {4,"ptn_name_moli"},
        {5,"ptn_name_feiling"},
        {6,"ptn_name_baiguangying"},
        {7,"ptn_name_heixiangling"}
    };
    public static readonly Dictionary<int, string> topText = new Dictionary<int, string>() {
        {1,"温柔可亲，善解人意"},
        {2,"婀娜多姿，善良重情"},
        {3,"热情开朗，活泼可爱"},
        {4,"温柔可爱，善解人意"},
        {5,"强势自信，争强好胜"},
        {6,"向往自由，高傲任性"},
        {7,"命运坎坷，坚强善良"}
    };


    public override void InitUI()
    {
        base.InitUI();
        controller = ui.GetController("c1");

        
        //GoToMoudle<ChooseRoleMoudle, Card>((int)MoudleType.ChooseDoll, null);

        InitEvent();
    }

    public override void InitData()
    {
        ui.visible = true;
        gameObject.SetActive(true);
        base.InitData();

        GameData.guiderCurrent = GuiderInfoLinked.GetCurrGuiderInfo(0, 1);

        GoToMoudle<XinlingTalkMoudle, GameInitCardsConfig>((int)MoudleType.Talk, null);
    }
    public override void InitData<T>(T data)
    {
        base.InitData(data);
        int step = int.Parse(data as String) ;
        ui.visible = true;
        gameObject.SetActive(true);
        GameData.guiderCurrent = GuiderInfoLinked.GetCurrGuiderInfo(0, step);

        GoToMoudle<ChooseRoleMoudle, Card>((int)ChooseRoleView.MoudleType.ChooseDoll, null);
    }
    public override void GoToMoudle<T, D>(int index, D data)
    {
        MoudleType moudleType = (MoudleType)index;
        BaseMoudle baseMoudle = FindMoudle<T>();
        if (baseMoudle == null)
        {
            baseMoudle = (BaseMoudle)Activator.CreateInstance(typeof(T));
            baseMoudles.Add(baseMoudle);
            GComponent gComponent = null;

            string componmentName;
            if (moudleNamePairs.TryGetValue(moudleType, out componmentName))
            {
                GObject gObject = ui.GetChild(componmentName);
                if (gObject == null)
                {
                    Debug.Log(componmentName + " not find, please check it");
                    return;
                }
                gComponent = gObject.asCom;
            }

            baseMoudle.baseView = this;

            baseMoudle.InitMoudle<D>(gComponent, index, data);
        }

        baseMoudle.InitData<D>(data);
        SwitchController(index);
    }

    private void Update()
    {
        EventMgr.Ins.DispachEvent(EventConfig.CHECK_SOUND);
    }

}

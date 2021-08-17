using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

[ViewAttr("Game/UI/Y_Game8", "Y_Game8", "Tips")]
public class GameAnswerTipsView : BaseView
{
    private GTextField timeText;
    GComponent time;
    GComponent error; 
    public override void InitUI()
    {
        base.InitUI();
        controller = ui.GetController("c1");
        timeText = SearchChild("n11").asCom.GetChild("n4").asTextField;
        time = SearchChild("n9").asCom;
        error = SearchChild("n11").asCom;
    }

    public override void InitData<D>(D data)
    {
        base.InitData(data);
        Extrand extrand = data as Extrand;
        controller.selectedIndex = extrand.type;

        switch (extrand.type)
        {
            case 0:
                SearchChild("n5").onClick.Set(extrand.callBack.Invoke);
                break;
            case 1:
                SearchChild("n10").onClick.Set(extrand.callBack.Invoke);
                SearchChild("n9").onClick.Set(extrand.extrand.Invoke);
                time.GetChild("title").text = extrand.item.num.ToString();
                //time.GetChild("title").text = extrand.item.num.ToString();
                break;
            case 2:
                SearchChild("n10").onClick.Set(extrand.callBack.Invoke);
                SearchChild("n11").onClick.Set(() =>
                {
                    Timers.inst.Remove(CountDown);
                    extrand.extrand.Invoke();
                });
                error.GetChild("title").text = extrand.item.num.ToString();

                StartRotate();
                break;
        }

    }


    int totalTime;
    void CountDown(object param)
    {
        if (totalTime == 0)
        {
            Timers.inst.Remove(CountDown);
            SearchChild("n11").onClick.Clear();
            SearchChild("n11").asCom.GetController("c1").selectedIndex = 1;
            timeText.text = "(" + totalTime + "s)重试";
            return;
        }
        timeText.text = "(" + totalTime + "s)重试";
        totalTime--;
    }

    void StartRotate()
    {
        totalTime = 10;
        timeText.text = "(" + totalTime + "s)重试";
        totalTime--;
        Timers.inst.Add(1f, 0, CountDown);
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

//帮助辛灵
[ViewAttr("Game/UI/B_Help_xinling", "B_Help_xinling", "Help_xinling")]
public class HelpTaskView : BaseView
{
    GTextField content;

    GButton makeDoll;
    UITweenMove makedollTween;
    GButton findDifference;
    GButton pickupTrash;
    GButton rubbishClassification;

    GComponent topCom;
    GLoader bgLoader;

    GComponent spineCom;

    GGraph role;
    string taskInfo;
    string[] info;
    public override void InitUI()
    {
        base.InitUI();
        makeDoll = SearchChild("n8").asButton;
        findDifference = SearchChild("n7").asButton;
        pickupTrash = SearchChild("n10").asButton;
        rubbishClassification = SearchChild("n9").asButton;
        //content = SearchChild("n4").asTextField;
        bgLoader = SearchChild("n2").asLoader;
        topCom = SearchChild("n18").asCom;
        controller = ui.GetController("c1");
        spineCom = SearchChild("n16").asCom;


        SetRoleSpine();

        makeDoll.displayObject.gameObject.AddComponent<UITweenMove>().SetTweenMove(new Vector2(makeDoll.x-700, makeDoll.y),0.5f);
        findDifference.displayObject.gameObject.AddComponent<UITweenMove>().SetTweenMove(new Vector2(findDifference.x + 700, findDifference.y), 0.5f);
        pickupTrash.displayObject.gameObject.AddComponent<UITweenMove>().SetTweenMove(new Vector2(pickupTrash.x - 700, pickupTrash.y), 0.5f);
        rubbishClassification.displayObject.gameObject.AddComponent<UITweenMove>().SetTweenMove(new Vector2(rubbishClassification.x + 700, rubbishClassification.y), 0.5f);


        InitEvent();
    }

    GoWrapper goWrapper;
    void SetRoleSpine()
    {
        role = new GGraph();
        spineCom.AddChild(role);

        GameObject go = FXMgr.CreateRoleSpine(18, 0);
        if (goWrapper == null)
        {
            goWrapper = new GoWrapper(go);
        }
        else
        {
            GameObject gobj = goWrapper.wrapTarget;
            goWrapper.wrapTarget = go;
            GameObject.Destroy(gobj);
        }
        role.SetNativeObject(goWrapper);
        goWrapper.scale = Vector2.one * 80;
        role.position = new Vector2(640, 1539);
    }

    public override void InitEvent()
    {
        base.InitEvent();

        //SearchChild("n1").onClick.Set(() =>
        //{
        //    controller.selectedIndex = 0;
        //});

        SearchChild("n0").onClick.Set(() =>
        {
            TouchScreenView.Ins.PlayChangeEffect(() => { 
            UIMgr.Ins.showNextView<MainView>();

            });

        });
        //click loveBtn星星
        topCom.GetChild("n13").onClick.Set(() =>
        {
            UIMgr.Ins.showNextPopupView<BuyStarsView>();
        });
        //click diamondBtn 钻石
        topCom.GetChild("n14").onClick.Set(() =>
        {
            UIMgr.Ins.showNextPopupView<ShopMallView>();
        });
        makeDoll.onClick.Set(OnClickMakeDolls);
        findDifference.onClick.Set(OnClickFindDifference);
        pickupTrash.onClick.Set(OnClickPickupTrash);
        rubbishClassification.onClick.Set(OnClickRubbishClassification);

        EventMgr.Ins.RegisterEvent<int>(EventConfig.GAME_FINISH_TASk, FinishTask);
    }

    public override void InitData<D>(D data)
    {
        base.InitData(data);
        taskInfo = data as string;
        info = taskInfo.Split(new char[1] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
        bgLoader.url = UrlUtil.GetBgUrl("Xinling", "0");
        GetTopText();
        Refresh();
    }
    void GetTopText()
    {
        topCom.GetChild("n15").asTextField.text = GameData.Player.love + "";
        topCom.GetChild("n16").asTextField.text = GameData.Player.diamond + "";
    }
    private void FinishTask(int index)
    {
        info[index] = "1";
        Refresh();
    }

    private void Refresh()
    {

        makeDoll.GetController("c1").selectedIndex = info[0] == "0" ? 0 : 1;
        pickupTrash.GetController("c1").selectedIndex = info[1] == "0" ? 0 : 1;
        rubbishClassification.GetController("c1").selectedIndex = info[2] == "0" ? 0 : 1;
        findDifference.GetController("c1").selectedIndex = info[3] == "0" ? 0 : 1;
        bool isFinish = true;
        for (int i = 0; i < info.Length; i++)
        {
            if (info[i]=="0")
            {
                isFinish = false;
                break;
            }
        }
        if (isFinish)
        {
            controller.selectedIndex = 1;
        }
    }

    private void OnClickFindDifference()
    {
        if (info[3] == "0")
        {
            NormalInfo normalInfo = new NormalInfo()
            {
                index = GameXinlingConfig.TYPE_FIND,
            };
            UIMgr.Ins.showNextPopupView<GameInfoView, NormalInfo>(normalInfo);
        }
        else
        {
            UIMgr.Ins.showErrorMsgWindow("该任务已完成!");
        }
        //NormalInfo normalInfo = new NormalInfo()
        //{
        //    index = GameXinlingConfig.TYPE_FIND,
        //};
        //UIMgr.Ins.showNextPopupView<GameInfoView, NormalInfo>(normalInfo);
    }

    private void OnClickMakeDolls()
    {
        if (info[0] == "0")
        {
            NormalInfo normalInfo = new NormalInfo()
            {
                index = GameXinlingConfig.TYPE_MAKEDOLLS,
            };
            UIMgr.Ins.showNextPopupView<GameInfoView, NormalInfo>(normalInfo);
        }
        else
        {
            UIMgr.Ins.showErrorMsgWindow("该任务已完成!");
        }
    }

    private void OnClickPickupTrash()
    {
        if (info[1] == "0")
        {
            NormalInfo normalInfo = new NormalInfo()
            {
                index = GameXinlingConfig.TYPE_PICKUP,
            };
            UIMgr.Ins.showNextPopupView<GameInfoView, NormalInfo>(normalInfo);
        }
        else
        {
            UIMgr.Ins.showErrorMsgWindow("该任务已完成!");
        }
    }

    private void OnClickRubbishClassification()
    {
        if (info[2] == "0")
        {
            NormalInfo normalInfo = new NormalInfo()
            {
                index = GameXinlingConfig.TYPE_CLASSIFICATION,
            };
            UIMgr.Ins.showNextPopupView<GameInfoView, NormalInfo>(normalInfo);
        }
        else
        {
            UIMgr.Ins.showErrorMsgWindow("该任务已完成!");
        }
    }
}

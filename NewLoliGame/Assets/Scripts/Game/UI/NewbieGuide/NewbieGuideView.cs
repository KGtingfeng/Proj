using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using System.Reflection;
using System;

[ViewAttr("Game/UI/X_Beginner_guidance", "X_Beginner_guidance", "talk_with_xinling")]
public class NewbieGuideView : BaseView
{
    public static NewbieGuideView ins;
    GComponent talkCom;
    GComponent contentCom;
    GTextField content;

    //GComponent popTalkCom;
    //GComponent popTalk;
    //GTextField popTalkText;

    public GComponent tapCom;
    GComponent tap;

    GComponent bubbleCom;
    GComponent bubble;
    GTextField bubbleText;

    GComponent maskCom;
    GObject maskBai;
    Controller maskControoler;


    GGraph role;
    GObject contentImage;

    GGraph mask;

    DateTime clickTime;
    public static GuiderInfoLinked current
    {
        get
        {
            return GameData.guiderCurrent;
        }
        set
        {
            GameData.guiderCurrent = value;
        }
    }

    public bool isHavePos;
    public Vector2 cPos;

    bool isTest;
    int type;
    /// <summary>
    /// 点击事件：可以自己监听
    /// </summary>
    public Action OnClick;


    public bool isClick;
    public override void InitUI()
    {
        base.InitUI();
        clickTime = DateTime.Now;
        talkCom = SearchChild("n0").asCom;
        contentCom = talkCom.GetChild("n1").asCom;
        content = contentCom.GetChild("n2").asTextField;


        role = talkCom.GetChild("n0").asGraph;
        controller = ui.GetController("c1");
        contentImage = contentCom.GetChild("n1");
        tapCom = SearchChild("n4").asCom;
        tap = tapCom.GetChild("n0").asCom;

        //popTalkCom = SearchChild("n0").asCom;
        //popTalk = popTalkCom.GetChild("n1").asCom;
        //popTalkText = popTalk.GetChild("n2").asTextField;

        mask = SearchChild("n5").asGraph;
        maskCom = SearchChild("n3").asCom;

        bubbleCom = SearchChild("n8").asCom;
        bubble = bubbleCom.GetChild("n6").asCom;
        bubbleText = bubble.GetChild("n7").asTextField;

        maskBai = maskCom.GetChild("n13");
        maskControoler = maskCom.GetController("c1");


        SetRoleSpine();
        InitEvent();
        ins = this;
    }

    GoWrapper goWrapper;
    void SetRoleSpine()
    {
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
        goWrapper.scale = Vector2.one * 108;

    }

    public override void InitEvent()
    {
        base.InitEvent();

    }

    public override void InitData()
    {
        base.InitData();
        isTest = true;
        controller.selectedIndex = 7;
        type = 0;
        maskControoler.selectedIndex = 1;
    }

    public override void InitData<D>(D data)
    {
        base.InitData(data);
        TapGoWrapper();
        isTest = false;
        isClick = false;
        if (current == null)
        {
            onHide();
            GameData.isGuider = false;
            return;
        }
        Debug.Log(current.guiderInfo.flow + "            " + current.guiderInfo.step);
        switch (current.guiderInfo.flow)
        {
            case 1:
                if (current.guiderInfo.type == 7)
                {
                    tapCom.onClick.Set(() =>
                    {
                        steps[current.guiderInfo.flow].inovke(current);
                    });
                    mask.onClick.Set(() =>
                    {
                        steps[current.guiderInfo.flow].inovke(current);
                    });
                    SwitchController(current.guiderInfo.type - 1);

                }
                else
                {
                    Open();
                }
                break;

            default:
                Open();
                break;
        }

    }

    private void Update()
    {
        if (isTest)
        {
            if (Input.GetMouseButtonDown(0))
            {
                switch (type)
                {
                    case 0:
                        Vector2 pos = Input.mousePosition;
                        pos = GameTool.MousePosToUI(pos, tapCom);
                        Debug.LogError(pos);
                        tap.position = pos;
                        break;
                    case 1:
                        Vector2 pos1 = Input.mousePosition;
                        pos = GameTool.MousePosToUI(pos1, talkCom);
                        Debug.LogError(pos);
                        contentCom.position = pos;
                        break;
                    case 2:
                        Vector2 pos2 = Input.mousePosition;
                        pos = GameTool.MousePosToUI(pos2, talkCom);
                        Debug.LogError(pos);
                        role.position = pos;
                        break;
                    case 3:
                        //Vector2 pos3 = Input.mousePosition;
                        //pos = GameTool.MousePosToUI(pos3, popTalkCom);
                        //Debug.LogError(pos);
                        //popTalk.position = pos;
                        break;
                    case 4:
                        Vector2 pos4 = Input.mousePosition;
                        pos = GameTool.MousePosToUI(pos4, bubbleCom);
                        Debug.LogError(pos);
                        bubble.position = pos;
                        break;
                    case 5:
                        Vector2 pos5 = Input.mousePosition;
                        pos = GameTool.MousePosToUI(pos5, maskCom);
                        Debug.LogError(pos);
                        //pos = talkCom.TransformPoint(pos, ui);
                        maskBai.position = pos;

                        break;

                }
            }
            else if (Input.GetKeyDown(KeyCode.Q))
            {
                type = 0;
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                type = 1;
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                type = 2;
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                type = 3;
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                type = 4;
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                type = 5;
            }
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (isTest)
            {
                isTest = false;
                controller.selectedIndex = current.guiderInfo.type - 1;
                //tap.visible = current.guiderInfo.type != 4;
            }
            else
            {
                tap.visible = true;
                isTest = true;
                controller.selectedIndex = 5;
                maskControoler.selectedIndex = 1;
            }
        }
    }

    GoWrapper tapGoWrapper;
    void TapGoWrapper()
    {
        if (tapGoWrapper == null)
        {
            UnityEngine.Object prefab = Resources.Load("Game/GFX/Prefabs/UI_guid");
            GGraph effect = tap.GetChild("n5").asGraph;
            if (prefab != null)
            {
                GameObject go = (GameObject)Instantiate(prefab);
                tapGoWrapper = new GoWrapper(go);
                effect.SetNativeObject(tapGoWrapper);
            }
        }
        else
        {
            tapGoWrapper.gameObject.SetActive(true);
        }
    }



    /// <summary>
    /// 静态记录所有的大步
    /// </summary>
    public static NewbieBigStep[] steps = {
        new NewbieStep0 (),
        new NewbieStep1 (),
        new NewbieStep2 (),
        new NewbieStep3 (),
        new NewbieStep4 (),
        new NewbieStep5 (),
        new NewbieStep6 (),
        new NewbieStep7 (),
        new NewbieStep8 (),
        new NewbieStep9 (),
        new NewbieStep10 (),
        new NewbieStep11 (),
        new NewbieStep12 (),

    };


    /// <summary>
    /// 静态记录所有的大步
    /// </summary>
    public static NewbieComPos[] comPos = {
        new NewbieCom1 (),
        new NewbieCom2 (),
        new NewbieCom3 (),
        new NewbieCom4 (),
        new NewbieCom5 (),
        new NewbieCom6 (),
        new NewbieCom7 (),
        new NewbieCom8 (),
        new NewbieCom9 (),

    };

    List<int> dialogList = new List<int>() { 1, 2, 4, 7, 8, 9 };
    /// <summary>
    /// 下一步
    /// </summary>
    public void InvokeNext()
    {
        if (dialogList.Contains(controller.selectedIndex))
        {
            if (!Stage.inst._audioDialog.isPlaying)
            {
                GuiderInfoLinked gi = current;
                if (gi == null)
                {
                    return;
                }
                NewbieBigStep bgs = steps[gi.guiderInfo.flow];
                bgs.invokeNext();
            }
        }
        else
        {
            GRoot.inst.StopDialogSound();
            GuiderInfoLinked gi = current;
            if (gi == null)
            {
                return;
            }
            NewbieBigStep bgs = steps[gi.guiderInfo.flow];
            bgs.invokeNext();
        }

    }


    /// <summary>
    /// 打开新手引导
    /// </summary>
    public void Open()
    {
        maskCom.onClick.Set(ExecuteNext);
        tapCom.onClick.Set(ExecuteNext);
        talkCom.onClick.Set(ExecuteNext);
        bubbleCom.onClick.Set(ExecuteNext);
        mask.onClick.Set(ExecuteNext);
        SetGuiderInfo();
    }



    /// <summary>
    /// 点击事件：上一步完成时
    /// </summary>
    public void ExecuteNext()
    {
        //if ((DateTime.Now - clickTime).Seconds < 0.5f)
        //    return;
        //clickTime = DateTime.Now;
        if (isTest)
            return;
        if (isClick)
            return;
        InvokeNext();
        //SetGuiderInfo();
    }


    /// <summary>
    /// 设置文字人物位置
    /// </summary>
    public void SetGuiderInfo()
    {
        //设置文字和点击位置
        if (current != null && current.guiderInfo != null)
        {
            GuiderInfoLinked gi = current;
            if (gi.guiderInfo.flow - 1 < comPos.Length)
            {
                NewbieComPos bgs = comPos[gi.guiderInfo.flow - 1];
                bgs.invokeNext();
            }
            else
            {
                isHavePos = false;
            }
            Debug.LogError("SetGuiderInfo " + current.guiderInfo.flow + "_" + current.guiderInfo.step);

            if (current.guiderInfo.actor_voice != "0")
            {
                Debug.LogError("voice " + current.guiderInfo.flow + "_" + current.guiderInfo.step);
                AudioClip audioClip = Resources.Load<AudioClip>(UrlUtil.GetNewbieBgmUrl(current.guiderInfo.actor_voice));
                GRoot.inst.PlayDialogSound(audioClip);
            }
            SwitchController(current.guiderInfo.type - 1);
            if (current.guiderInfo.type == 8 || current.guiderInfo.type == 9 || current.guiderInfo.type == 11)
            {
                maskControoler.selectedIndex = 1;
            }
            else
            {
                maskControoler.selectedIndex = 0;
            }
            //tap.visible = current.guiderInfo.type != 4;
            string[] cursorPos = current.guiderInfo.cursor_axis.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            string[] actorPos = current.guiderInfo.actor_axis.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            string[] contentPos = current.guiderInfo.contents_axis.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            if (isHavePos)
            {
                cPos.x += 48;
                //cPos.y += 36;
                tap.position = cPos;
            }
            else
            {
                if (cursorPos.Length < 2)
                {
                    tap.position = new Vector2(601, 1343);
                }
                else
                {
                    tap.position = new Vector2(int.Parse(cursorPos[0]), int.Parse(cursorPos[1]));
                }
            }

            if (contentPos.Length < 2)
            {
                contentCom.position = new Vector2(87, 545);
                bubble.position = new Vector2(87, 545);
            }
            else
            {
                contentCom.position = new Vector2(int.Parse(contentPos[0]), int.Parse(contentPos[1]));
                if (contentPos.Length > 2 && contentPos[2] == "1")
                {
                    contentImage.asImage.flip = FlipType.Horizontal;
                }
                else
                {
                    contentImage.asImage.flip = FlipType.None;
                }
                bubble.position = new Vector2(int.Parse(contentPos[0]), int.Parse(contentPos[1]));

            }
            if (actorPos.Length < 2)
            {
                role.position = new Vector2(377, 1776);
            }
            else
            {
                role.position = new Vector2(int.Parse(actorPos[0]), int.Parse(actorPos[1]));
                if (actorPos.Length >= 4)
                {
                    //Vector2 pos = talkCom.TransformPoint(role.position, ui);
                    maskBai.position = role.position;
                    maskBai.width = int.Parse(actorPos[2]);
                    maskBai.height = int.Parse(actorPos[3]);
                }
            }

            content.text = current.guiderInfo.contents;
            bubbleText.text = current.guiderInfo.contents;
            //popTalkText.text = current.guiderInfo.contents;
        }


    }
}


public class NewbieBigStep
{

    public void invokeNext()
    {
        GuiderInfoLinked next = NewbieGuideView.current.Next;
        GuiderInfoLinked current = NewbieGuideView.current;
        NewbieGuideView.current = next;
        NewbieGuideView.ins.isClick = true;
                    
        NewbieGuideView.ins.SwitchController(11);
        inovke(current);

    }


    public void inovke(GuiderInfoLinked current)
    {
        MethodInfo mi = this.GetType().GetMethod("Step_" + current.guiderInfo.flow + "_" + current.guiderInfo.step);
        Debug.LogError("执行：class:" + this.GetType().Name + "     method:" + "Step_" + current.guiderInfo.flow + "_" + current.guiderInfo.step);

        mi.Invoke(this, new object[0]);
        if (current.guiderInfo.need_save == 1)
        {
            GameTool.SaveNewbie(current.guiderInfo.flow, current.guiderInfo.step);
        }

    }


}


/// <summary>
/// 新手引导链表
/// </summary>
public class GuiderInfoLinked
{
    public GameGuideConfig guiderInfo;
    public GuiderInfoLinked Next;
    public int index;
    public Action callback;

    public GuiderInfoLinked(GameGuideConfig curr, GuiderInfoLinked next, int index)
    {
        this.guiderInfo = curr;
        this.Next = next;
        this.index = index;

    }

    public static GuiderInfoLinked GetCurrGuiderInfo(int currBigStep, int smallStep)
    {
        List<GuiderInfoLinked> relist = JsonConfig.GuiderLinkedListInfo;

        for (int i = 0; i < relist.Count; i++)
        {
            if (relist[i].guiderInfo.flow == currBigStep &&
                relist[i].guiderInfo.step == smallStep)
            {
                return relist[i];
            }
        }
        return null;
    }
}
/// <summary>
/// 注册
/// </summary>
public class NewbieStep0 : NewbieBigStep
{
}
/// <summary>
/// 剧情
/// </summary>
public class NewbieStep1 : NewbieBigStep
{
    public void Step_1_0()
    {
    }

    public void Step_1_1()
    {
        StoryChapterStaringMoudle.Ins.OnClick();
    }

    public void Step_1_2()
    {
        StoryDialogMoudle.Ins.GoToNextNode();
    }

    public void Step_1_3()
    {
        NewbieGuideView.ins.onHide();

    }

    public void Step_1_4()
    {
        UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);
    }

    public void Step_1_5()
    {
        UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);

    }

    public void Step_1_6()
    {
        NewbieGuideView.ins.onHide();


    }

    public void Step_1_7()
    {
        UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);

    }

    public void Step_1_8()
    {
        GetTimeView.ins.OnNewbieClose();

    }


    public void Step_1_9()
    {
        FindDoorSPGameView.ins.OnClickCom(0);
        UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);

    }

    public void Step_1_10()
    {
        FindDoorSPGameView.ins.GetTip();
        NewbieGuideView.ins.onHide();
    }

    public void Step_1_11()
    {
        GamePuzzleDoorView.ins.ShowNewTipsCom();
        NewbieGuideView.ins.onHide();

    }

    public void Step_1_12()
    {
        StoryTransitionMoudle.Ins.NewbieOnClickNext();
    }

}



/// <summary>
/// 互动
/// </summary>
public class NewbieStep2 : NewbieBigStep
{
    public void Step_2_0()
    {
        MainView.ins.RequestRoleListInfo();
    }


    public void Step_2_3()
    {
        MainView.ins.RequestRoleListInfo();
    }

    public void Step_2_4()
    {
        InteractiveRoleListMoudle.ins.RequestActorBuy(11);
    }

    public void Step_2_5()
    {
        ContactTipsView.ins.Confirm();
    }

    public void Step_2_6()
    {
        GetRoleView.ins.onHide();
        UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);

    }

    public void Step_2_7()
    {
        InteractiveRoleListMoudle.ins.NewbieRequest();

    }

    public void Step_2_8()
    {
        InteractiveView.ins.ShowOnMain();

    }

    public void Step_2_9()
    {
        WWWForm wWWForm = new WWWForm();
        GameMonoBehaviour.Ins.RequestInfoPost<List<SmsListIndex>>(NetHeaderConfig.CELL_LIST_INDEX, wWWForm, (List<SmsListIndex> smsLists) =>
        {
            TouchScreenView.Ins.PlayChangeEffect(() =>
            {
                UIMgr.Ins.showNextPopupView<SMSView, List<SmsListIndex>>(smsLists);
            });
        });

    }



}

/// <summary>
/// 手机
/// </summary>
public class NewbieStep3 : NewbieBigStep
{

    public void Step_3_0()
    {
        WWWForm wWWForm = new WWWForm();
        GameMonoBehaviour.Ins.RequestInfoPost<List<SmsListIndex>>(NetHeaderConfig.CELL_LIST_INDEX, wWWForm, (List<SmsListIndex> smsLists) =>
        {

            TouchScreenView.Ins.PlayChangeEffect(() =>
            {
                UIMgr.Ins.showNextPopupView<SMSView, List<SmsListIndex>>(smsLists);
            });
        });
    }

    public void Step_3_1()
    {
        WWWForm wWWForm = new WWWForm();
        GameMonoBehaviour.Ins.RequestInfoPost<List<SmsListIndex>>(NetHeaderConfig.CELL_LIST_INDEX, wWWForm, (List<SmsListIndex> smsLists) =>
        {

            TouchScreenView.Ins.PlayChangeEffect(() =>
            {
                UIMgr.Ins.showNextPopupView<SMSView, List<SmsListIndex>>(smsLists);
            });
        });

    }

    public void Step_3_2()
    {
        SMSMainMoudle.Ins.NewbieGotoSms();
    }

    public void Step_3_3()
    {
        SMSMoudle.Ins.OnClickBottom();
        UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);

    }

    public void Step_3_4()
    {
        //SMSMoudle.Ins.NewbieChoose();
        UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);

    }

    public void Step_3_5()
    {
        SMSMoudle.Ins.NewbieChoose();
        UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);

    }

    public void Step_3_6()
    {
        SMSMoudle.Ins.OnClickSend();
    }

    public void Step_3_7()
    {
        UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);
    }

    public void Step_3_8()
    {
        UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);

    }

    public void Step_3_9()
    {
        UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);

    }

    public void Step_3_10()
    {
        UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);

    }

    public void Step_3_11()
    {
        UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);

    }


}
/// <summary>
/// 成长界面
/// </summary>
public class NewbieStep4 : NewbieBigStep
{
    public void Step_4_1()
    {
        UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);

    }

    public void Step_4_2()
    {
        UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);

    }

    public void Step_4_3()
    {
        UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);

    }
}


/// <summary>
/// 角色升级
/// </summary>
public class NewbieStep5 : NewbieBigStep
{

    public void Step_5_1()
    {
        UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);
    }

}
/// <summary>
/// 属性升级
/// </summary>
public class NewbieStep6 : NewbieBigStep
{
    public void Step_6_1()
    {
        UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);
    }


}
/// <summary>
/// 娃娃升级
/// </summary>
public class NewbieStep7 : NewbieBigStep
{

    public void Step_7_1()
    {
        UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);
    }
}

/// <summary>
/// 角色外观
/// </summary>
public class NewbieStep8 : NewbieBigStep
{
    public void Step_8_1()
    {
        UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);
    }
}

/// <summary>
/// 赠送礼物
/// </summary>
public class NewbieStep9 : NewbieBigStep
{

    public void Step_9_1()
    {
        UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);
    }

}

/// <summary>
/// 闹钟
/// </summary>
public class NewbieStep10 : NewbieBigStep
{

    public void Step_10_1()
    {
        UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);
    }

    public void Step_10_2()
    {
        UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);
    }

    public void Step_10_3()
    {
        UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);

    }

}
/// <summary>
/// 朋友圈
/// </summary>
public class NewbieStep11 : NewbieBigStep
{

    public void Step_11_1()
    {
        UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);

    }

    public void Step_11_2()
    {
        UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);
    }


}
/// <summary>
/// 通讯录
/// </summary>
public class NewbieStep12 : NewbieBigStep
{
    public void Step_12_1()
    {
        UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);

    }

    public void Step_12_2()
    {
        UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);
    }

}


public class NewbieComPos
{
    public void invokeNext()
    {
        inovke(NewbieGuideView.current.guiderInfo.flow, NewbieGuideView.current.guiderInfo.step);

    }

    public void inovke(int big, int small)
    {
        MethodInfo mi = this.GetType().GetMethod("Step_" + big + "_" + small);
        if (mi == null)
        {
            NewbieGuideView.ins.isHavePos = false;
        }
        else
        {
            Debug.LogError("Getpos   " + this.GetType().Name + "     method:" + "Step_" + big + "_" + small);
            mi.Invoke(this, new object[0]);
            NewbieGuideView.ins.isHavePos = true;
        }

    }
}

public class NewbieCom1 : NewbieComPos
{
    public void Step_1_8()
    {
        NewbieGuideView.ins.cPos = GetTimeView.ins.ui.TransformPoint(GetTimeView.ins.SearchChild("n1").position, NewbieGuideView.ins.tapCom);
        NewbieGuideView.ins.cPos.y += 30;
    }

    public void Step_1_10()
    {
        NewbieGuideView.ins.cPos = FindDoorSPGameView.ins.ui.TransformPoint(FindDoorSPGameView.ins.SearchChild("n2").position, NewbieGuideView.ins.tapCom);
        NewbieGuideView.ins.cPos.y += 30;
    }
}

public class NewbieCom2 : NewbieComPos
{
    public void Step_2_0()
    {
        NewbieGuideView.ins.cPos = MainView.ins.ui.TransformPoint(MainView.ins.SearchChild("n28").position, NewbieGuideView.ins.tapCom);
    }

    public void Step_2_3()
    {
        NewbieGuideView.ins.cPos = MainView.ins.ui.TransformPoint(MainView.ins.SearchChild("n28").position, NewbieGuideView.ins.tapCom);
    }

    public void Step_2_8()
    {
        NewbieGuideView.ins.cPos = InteractiveView.ins.ui.TransformPoint(InteractiveView.ins.showMainPos, NewbieGuideView.ins.tapCom);
        NewbieGuideView.ins.cPos.y += 30;
    }
}

public class NewbieCom3 : NewbieComPos
{
    public void Step_3_0()
    {
        NewbieGuideView.ins.cPos = MainView.ins.ui.TransformPoint(MainView.ins.SearchChild("n27").position, NewbieGuideView.ins.tapCom);
    }

    public void Step_3_1()
    {
        NewbieGuideView.ins.cPos = InteractiveView.ins.ui.TransformPoint(InteractiveView.ins.SearchChild("n27").position, NewbieGuideView.ins.tapCom);
        NewbieGuideView.ins.cPos.y += 30;
    }

    public void Step_3_6()
    {
        NewbieGuideView.ins.cPos = SMSMoudle.Ins.ui.TransformPoint(SMSMoudle.Ins.SearchChild("n34").position, NewbieGuideView.ins.tapCom);
        NewbieGuideView.ins.cPos.y += 30;
    }

}

public class NewbieCom4 : NewbieComPos
{

}

public class NewbieCom5 : NewbieComPos
{

}

public class NewbieCom6 : NewbieComPos
{

}

public class NewbieCom7 : NewbieComPos
{

}

public class NewbieCom8 : NewbieComPos
{

}

public class NewbieCom9 : NewbieComPos
{



}
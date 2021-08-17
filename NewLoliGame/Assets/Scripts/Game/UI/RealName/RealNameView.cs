using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using System;

[ViewAttr("Game/UI/T_Common", "T_Common", "Frame_shiming")]
public class RealNameView : BaseView
{

    GTextInput nameText;
    GTextInput numberText;
    GTextField context;
    GTextField antiText;
    GButton gButton;
    Extrand extrand;
    public override void InitUI()
    {
        base.InitUI();
        nameText = SearchChild("n13").asTextInput;
        numberText = SearchChild("n14").asTextInput;
        controller = ui.GetController("c1");
        gButton = SearchChild("n11").asButton;
        context = SearchChild("n4").asTextField;
        antiText = SearchChild("n19").asTextField;
        InitEvent();
    }

    public override void InitEvent()
    {
        base.InitEvent();
        gButton.onClick.Set(OnClickSubmit);

    }

    public override void InitData()
    {
        OnShowAnimation();
        base.InitData();
        nameText.text = "";
        numberText.text = "";
        SwitchConfig config = GameData.SwitchConfigs.Find(a => a.key == SwitchConfig.KEY_REALNAME);
        context.text = config.description;
        SearchChild("n7").onClick.Set(() =>
        {
            EventMgr.Ins.DispachEvent(EventConfig.LOGIN_GOTO_LOGIN);
            UIMgr.Ins.HideView<RealNameView>();
        });
    }

    public override void InitData<D>(D data)
    {
        base.InitData(data);
        extrand = data as Extrand;
        if (extrand.type == 0)
        {

            nameText.text = "";
            numberText.text = "";
            SwitchConfig config = GameData.SwitchConfigs.Find(a => a.key == SwitchConfig.KEY_REALNAME);
            context.text = config.description;
            SearchChild("n7").onClick.Set(() =>
            {
                EventMgr.Ins.DispachEvent(EventConfig.LOGIN_GOTO_LOGIN);
                UIMgr.Ins.HideView<RealNameView>();
            });
        }
        else if (extrand.type == 1)
        {
            controller.selectedIndex = 2;
            antiText.text = extrand.msg;
            gButton.title = extrand.key;
            gButton.onClick.Set(() =>
            {
                extrand.callBack?.Invoke();
                UIMgr.Ins.HideView<RealNameView>();

            });
        }



    }

    void OnClickSubmit()
    {
        if (nameText.text.Trim() == "")
        {
            UIMgr.Ins.showErrorMsgWindow("名字不能为空");

            return;
        }
        if (numberText.text.Trim() == "")
        {
            UIMgr.Ins.showErrorMsgWindow("身份证号码不能为空");
            return;
        }
        if (numberText.text.Length != 18 && numberText.text.Length != 15)
        {
            UIMgr.Ins.showErrorMsgWindow("请输入正确的身份证号码");
            return;
        }

        RequestRealName();
    }

    void RequestRealName()
    {
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("userId", GameData.User.id);
        wWWForm.AddField("name", nameText.text);
        wWWForm.AddField("id", GameTool.Encrypt(numberText.text));
        GameMonoBehaviour.Ins.RequestInfoPost<RealName>(NetHeaderConfig.REAL_NAME, wWWForm, RequestRealNameSuccess);
    }

    void RequestRealNameSuccess(RealName realName)
    {
        if (realName.Result != "0")
        {
            UIMgr.Ins.showErrorMsgWindow(realName.Description);
        }
        else
        {

            GameMonoBehaviour.Ins.RequestInfoGet<User>(NetHeaderConfig.USER_INFO, null);
            UIMgr.Ins.showErrorMsgWindow(realName.Description);
            controller.selectedIndex = 1;
            gButton.title = "确定";
            gButton.onClick.Set(() => { OnClick(); });
            SearchChild("n7").onClick.Set(() => { OnClick(); });
        }
    }

    void OnClick()
    {
        if (GameData.User.age < 18)
        {
            Extrand extrand = new Extrand
            {
                key = "知道了",
                callBack = SelectServiceMoudle.moudle.QueryAnnouncement,
                type = 1
            };
            extrand.msg = "您的账号已纳入防沉迷保护系统，每日在线总时长不得超过1.5小时，每天22点至次日8点将被限制登录游戏；同时，如果您是未满8周岁的用户，不可进行游戏充值；年满8周岁至15周岁的用户，单次充值不可超过50元，月累计充值不可超过200元；年满16至17周岁，单次充值不可超过100元，月累计充值不可超过400元。祝您游戏愉快，我们将一直守护您！";
            UIMgr.Ins.showNextPopupView<RealNameView, Extrand>(extrand);

        }
        else
        {
            SelectServiceMoudle.moudle.QueryAnnouncement();
        }

        UIMgr.Ins.HideView<RealNameView>();
    }


}

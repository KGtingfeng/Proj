using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;


public class LoginMoudle : BaseMoudle
{
    GLoader accountBgLoader;
    GLoader pwdBgLoader;

    GTextInput accountTextInput;
    GTextInput pwdTextInput;

    string accountName = "";
    string pwd = "";

    string defaultUrl = "ui://t0kqvihpzw9n1z";
    string inputUrl = "ui://t0kqvihpzw9n20";

    public override void InitMoudle(GComponent gComponent, int controllerIndex)
    {
        //SDKController.Instance.OnEnterLoginMoudle();
        base.InitMoudle(gComponent, controllerIndex);
        InitUI();
        InitEvent();
    }


    public override void InitUI()
    {
        accountBgLoader = SearchChild("n21").asLoader;
        pwdBgLoader = SearchChild("n23").asLoader;

        accountTextInput = SearchChild("n15").asTextInput;
        pwdTextInput = SearchChild("n16").asTextInput;

        accountName = PlayerPrefsUtil.GetUserName();
        pwd = PlayerPrefsUtil.GetUserPwd();
        if (accountName != "" && pwd != "")
        {
            accountTextInput.text = accountName;
            pwdTextInput.text = pwd;
        }

        scaleToSmallAction = () =>
        {
            baseView.SwitchController((int)LoginView.MoudleType.First);
        };
        GButton cancelBtn = ui.GetChild("n12").asButton;


    }


    public override void InitEvent()
    {
        accountTextInput.onFocusIn.Set(() =>
        {
            accountBgLoader.url = inputUrl;
        });

        accountTextInput.onFocusOut.Set(() =>
        {
            accountBgLoader.url = defaultUrl;
            accountName = accountTextInput.text.Trim();
        });

        pwdTextInput.onFocusIn.Set(() =>
        {
            pwdBgLoader.url = inputUrl;
        });
        pwdTextInput.onFocusOut.Set(() =>
        {
            pwdBgLoader.url = defaultUrl;
            pwd = pwdTextInput.text.Trim();
        });
        //cancel btn
        ui.GetChild("n12").onClick.Set(BigToSmall);
        //login
        ui.GetChild("n13").onClick.Set(() =>
        {
            if (accountName == "" || pwd == "")
            {
                UIMgr.Ins.showErrorMsgWindow(MsgException.INPUT_ERROR);
                return;
            }
            WWWForm wWWForm = new WWWForm();
            wWWForm.AddField("userName", accountName);
            wWWForm.AddField("password", pwd);

            GameMonoBehaviour.Ins.RequestInfoPost<SignUpInfo>(NetHeaderConfig.SIGNIN, wWWForm, LoginSuccess);

        });
        ////ModeifyPwd
        //ui.GetChild("n18").onClick.Set(() =>
        //{
        //    baseView.GoToMoudle<ModeifyPwdMoudle>((int)LoginView.MoudleType.ModeifyPwd);
        //});

    }


    void LoginSuccess()
    {

        PlayerPrefsUtil.CacheUserLoginInfo(accountName, pwd);
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("userId", GameData.User.id);
        AsyncRequestMgr.asyncRequestMgr.AsyRequestInfo(AsyncRequestMgr.GetNetType.FunctionOff);
        if (GameData.playerId != 0)
        {
            AsyncRequestMgr.asyncRequestMgr.AsyRequestInfo(AsyncRequestMgr.GetNetType.RedPoint);
        }
        AsyncRequestMgr.asyncRequestMgr.AsyRequestInfo(AsyncRequestMgr.GetNetType.StoryGameSave);
        StoryCacheMgr.storyCacheMgr.Init();
        GameMonoBehaviour.Ins.RequestInfoPostList<SwitchConfig>(NetHeaderConfig.SWITCH_CONFIG, wWWForm, RequestSuccess);
        SDKController.Instance.RegisterTPush();
    }

    void RequestSuccess()
    {

        PlayerPrefsUtil.CacheUserLoginInfo(accountName, pwd);
        baseView.GoToMoudle<SelectServiceMoudle>((int)LoginView.MoudleType.SelectServer);
    }


    public override void InitData()
    {
        base.InitData();
        SmallToBig();

    }


}





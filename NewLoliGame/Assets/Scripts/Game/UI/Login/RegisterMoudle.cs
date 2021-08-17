using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

public class RegisterMoudle : BaseMoudle
{
    GTextInput accountTextInput;
    GTextInput firstPwdTextInput;
    GTextInput seconPwdTextInput;

    GLoader accountBgLoader;
    GLoader firstPwdBgLoader;
    GLoader secondPwdBgLoader;


    string accountName = "";
    string firstPwd = "";
    string secondPwd = "";

    string defaultUrl = "ui://t0kqvihpzw9n1z";
    string inputUrl = "ui://t0kqvihpzw9n20";

    public override void InitMoudle(GComponent gComponent, int controllerIndex)
    {
        base.InitMoudle(gComponent, controllerIndex);



        InitUI();
        InitEvent();
    }

    public override void InitUI()
    {
        accountTextInput = SearchChild("n8").asTextInput;
        firstPwdTextInput = SearchChild("n15").asTextInput;
        seconPwdTextInput = SearchChild("n16").asTextInput;

        accountBgLoader = SearchChild("n28").asLoader;
        firstPwdBgLoader = SearchChild("n29").asLoader;
        secondPwdBgLoader = SearchChild("n30").asLoader;

        scaleToSmallAction = () =>
        {
            baseView.SwitchController((int)LoginView.MoudleType.First);
        };
    }

    public override void InitData()
    {
        base.InitData();
        SmallToBig();


    }

    public override void InitEvent()
    {

        accountTextInput.onFocusOut.Set(() =>
        {
            accountName = accountTextInput.text.Trim();
            accountBgLoader.url = defaultUrl;
        });

        accountTextInput.onFocusIn.Set(() =>
        {
            accountBgLoader.url = inputUrl;
        });

        firstPwdTextInput.onFocusOut.Set(() =>
        {
            firstPwd = firstPwdTextInput.text.Trim();
            firstPwdBgLoader.url = defaultUrl;
        });

        firstPwdTextInput.onFocusIn.Set(() =>
        {
            firstPwdBgLoader.url = inputUrl;
        });
        seconPwdTextInput.onFocusOut.Set(() =>
        {
            secondPwd = seconPwdTextInput.text.Trim();
            secondPwdBgLoader.url = defaultUrl;
        }
        );

        seconPwdTextInput.onFocusIn.Set(() =>
        {
            secondPwdBgLoader.url = inputUrl;
        });

        GComponent gComponent = SearchChild("n21").asCom;
        //registBtn
        SearchChild("n20").onClick.Set(() =>
        {
            ////GTween.Shake(gComponent.displayObject.gameObject.transform.localPosition, 12.5f, 1.5f).SetTarget(gComponent.displayObject.gameObject.transform);
            //GTween.Shake(Vector3.zero,1, 1)
            //.SetDelay(0.5f)
            //.SetTimeScale(1)
            //.SetIgnoreEngineTimeScale(true)
            //.SetTarget(gComponent) ;


            //GTween.Shake(gComponent.displayObject.gameObject.transform.localPosition, 0.5f, 0.5f).SetTarget(gComponent.displayObject.gameObject).OnUpdate(
            //(GTweener tweener) =>
            //{
            //    Debug.Log("ss");
            //     gComponent.displayObject.gameObject.transform.localPosition = new Vector3(tweener.value.x, tweener.value.y, gComponent.displayObject.gameObject.transform.localPosition.z);
            //});

            if (GetAccountLegalResult() && GetPwdLegalResult())
            {
                WWWForm wWWForm = new WWWForm();
                wWWForm.AddField("userName", accountName);
                wWWForm.AddField("password", firstPwd);

                GameMonoBehaviour.Ins.RequestInfoPost<SignUpInfo>(NetHeaderConfig.SIGNUP, wWWForm, RegisterSuccess);

            }

        });

        //back
        SearchChild("n21").onClick.Set(BigToSmall);

    }


    public void RegisterSuccess()
    {
        //记录缓存数据
        PlayerPrefsUtil.CacheUserLoginInfo(accountName, firstPwd);
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("userId", GameData.User.id);
        GameMonoBehaviour.Ins.RequestInfoPostList<SwitchConfig>(NetHeaderConfig.SWITCH_CONFIG, wWWForm, RequestSuccess);


    }

    void RequestSuccess()
    {
        baseView.GoToMoudle<SelectServiceMoudle>((int)LoginView.MoudleType.SelectServer);
    }

    public bool GetAccountLegalResult()
    {
        if (accountName.Equals(""))
        {
            UIMgr.Ins.showErrorMsgWindow(MsgException.USERNAME_LENGTH_NOT_NULL);
            return false;
        }

        return true;
    }

    public bool GetPwdLegalResult()
    {

        if (firstPwd.Equals("") || secondPwd.Equals(""))
        {
            UIMgr.Ins.showErrorMsgWindow(MsgException.PASSWORD_LENGTH_NOT_NULL);
            return false;
        }

        if (!firstPwd.Equals(secondPwd))
        {
            UIMgr.Ins.showErrorMsgWindow(MsgException.PASSWORD_NOT_SAME);
            return false;
        }

        return true;
    }



}

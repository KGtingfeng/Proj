using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using FairyGUI;
[ViewAttr("Game/UI/Z_Main", "Z_Main", "Frame_playerinfo", true)]
public class PlayerHeadView : BaseView
{
    public static PlayerHeadView ins;
    GTextField idText;
    GTextField nameText;
    GTextField level;
    GTextField nickNameText;
    GTextField ageNameText;
    GTextField birthdayNameText;
    GTextField idTypeText;
    GComponent headUI;
    GLoader iconGLoader;
    GTextField titleText;

    GButton modifyBirthdayBtn;
    GLoader titleLoader;
    GGraph titleGraph;

    GComponent titleCom;

    GLoader frameLoader;
    GGraph frameGraph;
    GComponent gComponent;

    GList btnList;
    Player player
    {
        get { return GameData.Player; }
    }


    public override void InitUI()
    {
        base.InitUI();
        ins = this;
        level = SearchChild("n37").asTextField;
        nameText = SearchChild("n24").asTextField;
        idText = SearchChild("n26").asTextField;
        nickNameText = SearchChild("n31").asTextField;
        ageNameText = SearchChild("n32").asTextField;
        birthdayNameText = SearchChild("n33").asTextField;
        idTypeText = SearchChild("n34").asTextField;
        modifyBirthdayBtn = SearchChild("n14").asButton;

        headUI = SearchChild("n1").asCom;
        iconGLoader = headUI.GetChild("n16").asLoader;
        titleCom = SearchChild("n38").asCom;
        titleLoader = titleCom.GetChild("n39").asLoader;
        titleGraph = titleCom.GetChild("n40").asGraph;
        titleText = SearchChild("n25").asTextField;
        frameLoader = SearchChild("n1").asCom.GetChild("n18").asLoader;
        frameGraph = SearchChild("n1").asCom.GetChild("n19").asGraph;

        btnList = SearchChild("n17").asList;

        InitEvent();
    }


    public override void InitData()
    {
        OnShowAnimation();

        base.InitData();

        InitInfos();
    }

    int GetAge()
    {
        int age = 1;
        string[] ages = player.birthday.Split('-');
        if (ages.Length > 0)
        {
            age = 2020 - Convert.ToInt32(ages[0]);
            if (age <= 0)
                age = 1;
        }
        return age;
    }



    public override void InitEvent()
    {
        base.InitEvent();
        EventMgr.Ins.RegisterEvent(EventConfig.REFRESH_PLAYER_BASE_INFO, InitInfos);
        //change head btn
        SearchChild("n1").onClick.Set(getAvatarInfos);

        SearchChild("n12").onClick.Set(() =>
        {
            Extrand extrand = new Extrand();
            extrand.type = ModifyPlayerInfo.MODIFY_NAME;
            UIMgr.Ins.showNextPopupView<ChangePlayerNameView, Extrand>(extrand);
        });
        SearchChild("n13").onClick.Set(() =>
        {
            Extrand extrand = new Extrand();
            extrand.type = ModifyPlayerInfo.MODIFY_NICKNAME;
            UIMgr.Ins.showNextPopupView<ChangePlayerNameView, Extrand>(extrand);
        });
        //修改生日
        modifyBirthdayBtn.onClick.Set(() =>
        {
            UIMgr.Ins.showNextPopupView<SelectCalendarView>();
        });
        //修改身份
        SearchChild("n15").onClick.Set(() =>
        {
            UIMgr.Ins.showNextPopupView<SelectProfessionView>();
        });

        //back to login
        SearchChild("n20").onClick.Set(() =>
        {
            CallBackList callBackList = new CallBackList();
            callBackList.callBack1 = () =>
            {

                GRoot.inst.HidePopup();
                EventMgr.Ins.Dispose();
                UIMgr.Ins.showViewWithReleaseOthers<LoginView>();
                SDKController.Instance.StopService();

                ServiceObject service = GameObject.FindObjectOfType<ServiceObject>();
                Destroy(service.gameObject);
                ServiceObject.ins = null;
            };
            UIMgr.Ins.showNextPopupView<LoginAnimationView, CallBackList>(callBackList);
            
        });

        SearchChild("n54").onClick.Set(() =>
        {
            OnHideAnimation();
            EventMgr.Ins.DispachEvent(EventConfig.REFRESH_MAIN_RED_POINT);
        });

        //修改称号
        SearchChild("n39").onClick.Set(() =>
        {
            WWWForm wWWForm = new WWWForm();
            GameMonoBehaviour.Ins.RequestInfoPost<AvatarFrame>(NetHeaderConfig.PLAYER_TITLE, wWWForm, (AvatarFrame frame) =>
            {
                UIMgr.Ins.showNextPopupView<TitleView, AvatarFrame>(frame);
                titleCom.GetController("c1").selectedIndex = 0;
            });

        });
        EventMgr.Ins.RegisterEvent(EventConfig.REFRESH_PLAYER_TITLE, ChangeTitle);

        //激活码按钮
        btnList.GetChildAt(0).onClick.Set(() =>
        {
            UIMgr.Ins.showNextPopupView<ActiveCodeView>();
        });

    }

    void getAvatarInfos()
    {
        //获得全部头像信息
        WWWForm wWWForm = new WWWForm();
        GameMonoBehaviour.Ins.RequestInfoPostList<Avatar>(NetHeaderConfig.GET_PLAYER_AVATAR, wWWForm, () =>
        {
            base.OnHideAnimation();
            UIMgr.Ins.showNextPopupView<ChangeHeadView>();
            headUI.GetController("c1").selectedIndex = 0;
        });
    }

    void InitInfos()
    {
        idText.text = player.id + "";
        level.text = player.level.ToString();
        ageNameText.text = GetAge() + "";
        nameText.text = player.name;
        nickNameText.text = player.nickname;
        birthdayNameText.text = player.birthday;
        idTypeText.text = DataUtil.FindRoleProfession(player.character);
        iconGLoader.url = UrlUtil.GetRoleHeadIconUrl(player.avatar);
        modifyBirthdayBtn.visible = DataUtil.isCanModify(ModifyPlayerInfo.MODIFY_BIRTHDAY);

        GameAvatarFrameConfig frameConfig = JsonConfig.GameAvatarFrameConfigs.Find(a => a.id == player.avatar_frame.current);
        FXMgr.SetFrame(frameGraph, frameLoader, frameConfig.source_id, 100, new Vector3(135, 136, 1000));
        ChangeTitle();

        titleCom.GetController("c1").selectedIndex = RedpointMgr.Ins.TitleHaveRedpoint() ? 1 : 0;
        RefreshHeadRedpoint();
    }

    public void ReqEditPlayerBirthday(int year, int month, int day)
    {
        string birthday = year + "-" + month + "-" + day;

        Action callBack = () => { UIMgr.Ins.showErrorMsgWindow(MsgException.MODIFY_SUCCESSFULLY); };
        //获得全部头像信息
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("birthday", birthday);
        GameMonoBehaviour.Ins.RequestInfoPost<Player>(NetHeaderConfig.MODIFY_PLAYERINFO, wWWForm, callBack);
    }

    public override void OnHideAnimation()
    {
        base.OnHideAnimation();
        EventMgr.Ins.DispachEvent(EventConfig.MAIN_OPEN_EFFECT);
    }

    void ChangeTitle()
    {
        if (player.title.current != 0)
        {
            GameTitleConfig titleConfig = JsonConfig.GameTitleConfigs.Find(a => a.id == player.title.current);
            FXMgr.SetTitle(titleGraph, titleLoader, titleConfig.level, 80, new Vector3(127, 22, 1000));
            titleText.text = titleConfig.name_cn;
        }
        else
        {
            titleText.text = "暂无";
            FXMgr.SetTitle(titleGraph, titleLoader, 1, 80, new Vector3(127, 22, 1000));
        }
    }

    void RefreshHeadRedpoint()
    {
        headUI.GetController("c1").selectedIndex = RedpointMgr.Ins.FrameHaveRedpoint() ? 1 : 0;
    }
}

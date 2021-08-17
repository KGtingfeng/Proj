using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using System.Text.RegularExpressions;
using System;

[ViewAttr("Game/UI/C_Creat role", "C_Creat role", "Creat role")]
public class CreateRoleView : BaseView
{
    public static CreateRoleView view;
    public GComponent createRoleUI;

    public enum MoudleType
    {
        GetDoll = 3,
    };


    Dictionary<MoudleType, string> moudleNamePairs = new Dictionary<MoudleType, string>()
    {
        {MoudleType.GetDoll,"n8"},
    };

    GTextInput nameTextInput;
    GTextInput nickNameTextInput;
    GTextField birthdayText;
    GTextField professionText;
    GButton reBackButton;

    GTextField dollName;
    GTextField dollBlood;
    GLoader dollLoader;

    string userName = "";
    string nickName = "";
    string birthday = "";
    int selectIndex = 0;

    int PAGE_SELECT_PROFESSIO_INDEX = 1;
    int PAGE_DEFAULT_INDEX = 0;

    GameInitCardsConfig doll;
    GComponent root;

    DateTime time;
    public override void InitUI()
    {
        base.InitUI();
        view = this;
        createRoleUI = SearchChild("n3").asCom;
        GLoader gLoader = SearchChild("n4").asLoader;
        gLoader.url = UrlUtil.GetCreatRoleBgUrl();
        controller = ui.GetController("c1");
        //选择职业
        uiSelectProfessionCom = SearchChild("n5").asCom;
        professionList = uiSelectProfessionCom.GetChild("n9").asList;
        reBackButton = SearchChild("n7").asButton;
        root = ui;
        ui = createRoleUI;
        nameTextInput = SearchChild("n15").asTextInput;
        nickNameTextInput = SearchChild("n16").asTextInput;
        birthdayText = SearchChild("n17").asTextField;

        professionText = SearchChild("n32").asTextField;
        dollName = createRoleUI.GetChild("n37").asTextField;
        dollBlood = createRoleUI.GetChild("n38").asTextField;
        dollLoader = createRoleUI.GetChild("n36").asLoader;
        InitEvent();
        time = DateTime.Now;
    }

    public override void InitData<D>(D data)
    {
        base.InitData(data);
        doll = data as GameInitCardsConfig;

        ui.visible = true;
        gameObject.SetActive(true);
        controller.selectedIndex = PAGE_DEFAULT_INDEX;
        GetRandomName();
        //SychronizedBirthday(2001, 02, 02);
        //职业
        SetProfessionText();
        //birthdayText.text = "";
        SychronizedBirthday(2000, 1, 1);
        dollLoader.url = UrlUtil.GetStoryHeadIconUrl(doll.card_id + 27);
        dollName.text = doll.name_cn;
        string[] str = doll.info.Split(new char[1] { '|' });
        if (str.Length > 1)
        {
            dollBlood.text = str[1];
        }

    }

    public override void InitData()
    {

        ui.visible = true;
        gameObject.SetActive(true);
        controller.selectedIndex = PAGE_DEFAULT_INDEX;
        GetRandomName();
        //SychronizedBirthday(2001, 02, 02);
        //职业
        SetProfessionText();
    }

    public override void InitEvent()
    {
        //随机名
        SearchChild("n24").onClick.Set(() => { GetRandomName(); });

        nameTextInput.onFocusOut.Set(() =>
        {
            userName = nameTextInput.text.Trim();
            getNickName();
        });

        nickNameTextInput.onFocusOut.Set(() =>
        {
            nickName = nickNameTextInput.text.Trim();
        });

        professionText.onClick.Set(() =>
        {
            //UIMgr.Ins.showNextPopupView<SelectProfessionView>();
            controller.selectedIndex = PAGE_SELECT_PROFESSIO_INDEX;
        });

        SearchChild("n14").onClick.Set(() =>
        {
            // UIMgr.Ins.showNextPopupView<SelectProfessionView>();
            controller.selectedIndex = PAGE_SELECT_PROFESSIO_INDEX;
        });

        reBackButton.onClick.Set(() =>
        {
            //UIMgr.Ins.showBeforeView<LoginView, CreateRoleView>();
            //onHide();
            UIMgr.Ins.showNextView<ChooseRoleView, string>("6");
        });

        //create roleBtn
        createRoleUI.GetChild("n12").onClick.Set(onClickCreateBtn);

        //select birthday
        birthdayText.onClick.Set(() =>
        {
            UIMgr.Ins.showNextPopupView<SelectCalendarView>();
        });

        createRoleUI.GetChild("n34").onClick.Set(() =>
        {
            UIMgr.Ins.showNextPopupView<SelectCalendarView>();
        });

        //职业
        professionList.onClickItem.Set(OnClickItem);
    }

    void onClickCreateBtn()
    {
        if (CheckBaseInfo())
        {
            CreatePlayer();
        }
    }

    void CreatePlayer()
    {
        if ((DateTime.Now - time).Seconds <= 0.5f)
            return;

        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("playerName", userName);
        wWWForm.AddField("nickName", nickName);
        wWWForm.AddField("chrType", selectIndex);
        wWWForm.AddField("birthday", birthday);
        wWWForm.AddField("initCard", doll.card_id);

        //GameMonoBehaviour.Ins.RequestInfoPost<SignUpInfo>(NetHeaderConfig.CREATE_PLAYER, wWWForm, CreatePlayerSuccess);
        GameMonoBehaviour.Ins.RequestInfoPost<Player>(NetHeaderConfig.CREATE_PLAYER_NEW, wWWForm, CreatePlayerSuccess);

    }


    void CreatePlayerSuccess()
    {
        RequestPlayerRoles();
    }


    void RequestPlayerRoles()
    {
        if (GameData.Dolls == null || GameData.Dolls.Count == 0)
        {
            UIMgr.Ins.showErrorMsgWindow(MsgException.RESPONSE_ENTITY_IS_NULL);
            return;
        }



        WWWForm wWForm = new WWWForm();
        GameMonoBehaviour.Ins.RequestInfoPost(NetHeaderConfig.CONFIG_GAME_ALL, wWForm,
            (List<ChannelSwitchConfig> configs) =>
        {
            GameData.Configs = configs;
            ChannelSwitchConfig guid = configs.Find(a => a.key == ChannelSwitchConfig.KEY_GUID);
            if (guid != null)
            {
                if (guid.value == 1)
                {
                    GameData.isOpenGuider = true;
                }
                else
                {
                    GameData.isOpenGuider = false;
                }
            }
            else
            {
                GameData.isOpenGuider = false;
            }
            if (GameData.isOpenGuider)
            {
                StoryCacheMgr.storyCacheMgr.SaveProgress("Newbie", "1,1", 0);
            }

        });

        //WWWForm wWWForm = new WWWForm();
        //wWWForm.AddField("initCard", doll.card_id);
        //GameMonoBehaviour.Ins.RequestInfoPost<Player>(NetHeaderConfig.CONTRACT, wWWForm, () =>
        //{
        //    GoToMoudle<GetRoleMoudle, GameInitCardsConfig>((int)MoudleType.GetDoll, doll);
        //});

        GoToMoudle<GetRoleMoudle, GameInitCardsConfig>((int)MoudleType.GetDoll, doll);

        //UIMgr.Ins.showNextView<ChooseRoleView>();
        //GameMonoBehaviour.Ins.RequestInfoGet<GameInitCardsConfig>(NetHeaderConfig.GET_DEFAULT_DOLLS, RequestPlayerRolesSuccess);
    }



    IEnumerator CreateRoleEffect()
    {
        TouchScreenView.Ins.PlayTmpEffect();
        SDKController.Instance.StartService();

        yield return new WaitForSeconds(0.8f);
        //UIMgr.Ins.showNextView<ChooseRoleView>();
        UIMgr.Ins.showViewWithReleaseOthers<MainView>();



    }

    void RequestPlayerRolesSuccess()
    {
        if (GameData.Dolls == null || GameData.Dolls.Count == 0)
        {
            UIMgr.Ins.showErrorMsgWindow(MsgException.RESPONSE_ENTITY_IS_NULL);
            return;
        }
        StartCoroutine(CreateRoleEffect());
        //UIMgr.Ins.showNextView<ChooseRoleView>();
    }


    public bool CheckBaseInfo()
    {
        if (userName.Equals("") || nickName.Equals("") || birthday.Equals(""))
        {
            UIMgr.Ins.showErrorMsgWindow(MsgException.BASE_INFO_NOT_ENOUGH);
            return false;
        }

        if (SpecialWordHelper.Ins.IsSpecialWord(userName) || SpecialWordHelper.Ins.IsSpecialWord(nickName))
        {
            UIMgr.Ins.showErrorMsgWindow(MsgException.HAS_SPECIAL_WORDS);
            return false;
        }
        return true;
    }

    public bool CheckBirthdayFormat()
    {
        return Regex.IsMatch(birthday, @"^((((1[6-9]|[2-9]\d)\d{2})-(0?[13578]|1[02])-(0?[1-9]|[12]\d|3[01]))|(((1[6-9]|[2-9]\d)\d{2})-(0?[13456789]|1[012])-(0?[1-9]|[12]\d|30))|(((1[6-9]|[2-9]\d)\d{2})-0?2-(0?[1-9]|1\d|2[0-9]))|(((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-))$");
    }

    public void GetRandomName()
    {
        string tmpName = DataUtil.GetRandomName();
        nameTextInput.text = tmpName;
        userName = tmpName;
        getNickName();
    }

    /// <summary>
    /// 获得昵称
    /// </summary>
    private void getNickName()
    {
        string tmpName = nameTextInput.text.Trim();
        if (tmpName.Equals(""))
        {
            nickNameTextInput.text = "";
            nickName = "";
            return;
        }
        nickNameTextInput.text = tmpName[tmpName.Length - 1] + "儿";
        nickName = nickNameTextInput.text;
    }

    public void SychronizedBirthday(int year, int month, int day)
    {
        birthday = year + "-" + month + "-" + day;
        birthdayText.text = birthday;
    }


    #region ***********处理选择职业************
    GComponent uiSelectProfessionCom;
    GList professionList;


    private void OnClickItem(EventContext context)
    {
        int selectedIndex = professionList.GetChildIndex((GObject)context.data);
        selectIndex = professionList.ChildIndexToItemIndex(selectedIndex);
        controller.selectedIndex = PAGE_DEFAULT_INDEX;
        SetProfessionText();
    }


    void SetProfessionText()
    {
        professionText.text = RoleConfig.roleProfessons[selectIndex];
    }

    #endregion***********处理选择职业************

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
                GObject gObject = root.GetChild(componmentName);
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

        baseMoudle.InitData();
        SwitchController(index);
    }
}

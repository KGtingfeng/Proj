using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using System.Text.RegularExpressions;
using System;

[ViewAttr("Game/UI/Z_Main", "Z_Main", "Frame_changename_father", true)]
public class ChangePlayerNameView : BaseView
{
    bool isModifyName;

    int FREE = 0;
    int CONSUME = 1;

    GComponent changeNameUI;
    Controller consumeController;

    GTextField titleText;
    GObject consumeText;
    GTextInput nameTextInput;
    //修改名称消费类型
    GLoader consumeGLoader;
    GTextField consumeTextField;

    GameConsumeConfig gameConsumeConfig;

    Extrand extrand;

    string name = "";
    public override void InitUI()
    {

        changeNameUI = SearchChild("n0").asCom;
        consumeController = changeNameUI.GetController("c1");

        titleText = changeNameUI.GetChild("n47").asTextField;
        consumeText = changeNameUI.GetChild("n48");

        consumeGLoader = changeNameUI.GetChild("n20").asLoader;
        consumeTextField = changeNameUI.GetChild("n53").asTextField;
        nameTextInput = changeNameUI.GetChild("n52").asTextInput;

        InitEvent();
    }


    public override void InitData<D>(D data)
    {
        OnShowAnimation();
        base.InitData(data);

        extrand = data as Extrand;

        int modifyType = extrand.type;

        isModifyName = modifyType != ModifyPlayerInfo.MODIFY_NICKNAME;
        titleText.text = isModifyName ? "修改名字" : "修改小名";

        gameConsumeConfig = DataUtil.GetConsumeConfig(modifyType);
        if (gameConsumeConfig != null)
        {
            TinyItem tinyItem = ItemUtil.GetTinyItem(gameConsumeConfig.pay);

            consumeTextField.text = "" + tinyItem.num;
            //consumeGLoader.url = tinyItem.url;
            consumeGLoader.url = CommonUrlConfig.GetConsumeItemUrl((TypeConfig.Consume)tinyItem.type);
            consumeController.selectedIndex = DataUtil.isCanModify(modifyType) ? FREE : CONSUME;
            return;
        }
        consumeText.visible = false;
    }

    public override void InitEvent()
    {
        nameTextInput.onFocusOut.Set(() =>
        {
            name = nameTextInput.text.Trim();
        });
        changeNameUI.GetChild("n49").onClick.Set(() =>
        {
            nameTextInput.text = "";
            name = "";
            OnHideAnimation();
        });

        changeNameUI.GetChild("n50").onClick.Set(onClickChangeBtn);
    }

    void onClickChangeBtn()
    {
        if (!isChange())
            return;
        switch (extrand.type)
        {
            case ModifyPlayerInfo.MODIFY_NAME:
                QueryModifyPlayerName("name");
                break;
            case ModifyPlayerInfo.MODIFY_NICKNAME:
                QueryModifyPlayerName("nickName");
                break;
            case ModifyPlayerInfo.MODIFY_ADDRESSBOOK:
                QueryEditActorName(int.Parse(extrand.msg));
                break;
        }
    }

    void QueryModifyPlayerName(string type)
    {

        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField(type, name);
        wWWForm.AddField("consumeId", gameConsumeConfig.id);
        GameMonoBehaviour.Ins.RequestInfoPost<Player>(NetHeaderConfig.MODIFY_PLAYERINFO, wWWForm, ModifySuccess);
    }

    void QueryEditActorName(int actorId)
    {
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("actorId", actorId);
        wWWForm.AddField("nickname", name);
        GameMonoBehaviour.Ins.RequestInfoPost<Role>(NetHeaderConfig.ACTOR_EDIT, wWWForm, () =>
        {
            ModifySuccess();
            extrand.callBack?.Invoke();
        });
    }

    void ModifySuccess()
    {
        nameTextInput.text = "";
        name = "";
        OnHideAnimation();
    }

    /// <summary>
    /// 判断是否可进行替换
    /// </summary>
    /// <returns></returns>
    private bool isChange()
    {
        if (name.Equals(""))
        {
            UIMgr.Ins.showErrorMsgWindow(MsgException.NICKNAME_LENGTH_NOT_NULL);
            return false;
        }
        if (name.Length < 2)
        {
            UIMgr.Ins.showErrorMsgWindow(MsgException.NICKNAME_LENGTH_NOT_ENOUGH);
            return false;
        }
        if (name.Length > 6)
            return false;

        if (SpecialWordHelper.Ins.IsSpecialWord(name))
        {
            UIMgr.Ins.showErrorMsgWindow(MsgException.HAS_SPECIAL_WORDS);
            return false;
        }
        return true;
    }
}

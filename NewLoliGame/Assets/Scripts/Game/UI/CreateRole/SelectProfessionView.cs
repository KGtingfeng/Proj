using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using System;

[ViewAttr("Game/UI/T_Common", "T_Common", "Frame_chooseshenfen", true)]
public class SelectProfessionView : BaseView
{
    int selectIndex = 0;
    GList professionList;
    GGroup gGroup;
    bool isModify;

    public override void InitUI()
    {
        base.InitUI();
        gGroup = SearchChild("n19").asGroup;
        professionList = SearchChild("n9").asList;
        InitEvent();
    }
    public override void InitData()
    {
        isModify = PlayerHeadView.ins != null;
        setCurrentPosition();
        OnShowAnimation();
        base.InitData();
    }

    public override void InitEvent()
    {
        //职业
        professionList.onClickItem.Set(OnClickItem);
        SearchChild("n20").onClick.Set(OnHideAnimation);
    }

    private void OnClickItem(EventContext context)
    {
        int selectedIndex = professionList.GetChildIndex((GObject)context.data);
        selectIndex = professionList.ChildIndexToItemIndex(selectedIndex);
        SetProfessionText();
    }

    Vector2 midPivot = new Vector2(0.5f, 0.5f);
    Vector2 downPivot = new Vector2(0.5f, 0.75f);
    void setCurrentPosition()
    {
        Vector3 vector = gGroup.position;
        float pos = isModify ? vector.y : 808;
        pivot = isModify ? midPivot : downPivot;
        gGroup.SetPosition(vector.x, pos, vector.z);

    }
    void SetProfessionText()
    {
        Action callBack = () =>
        {
            OnHideAnimation();
            UIMgr.Ins.showErrorMsgWindow(MsgException.MODIFY_SUCCESSFULLY);
        };
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("character", selectIndex.ToString());
        GameMonoBehaviour.Ins.RequestInfoPost<Player>(NetHeaderConfig.MODIFY_PLAYERINFO, wWWForm, callBack);
    }

}

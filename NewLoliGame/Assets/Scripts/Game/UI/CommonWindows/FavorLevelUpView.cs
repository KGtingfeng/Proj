using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

[ViewAttr("Game/UI/H_Interactive", "H_Interactive", "Frame_haogandushengjizi")]
public class FavorLevelUpView : BaseView
{
    GTextField oldLevelText;
    GTextField newLevelText;

    GComponent levelCom;

    GGraph gGraph;

    public override void InitUI()
    {
        base.InitUI();
        levelCom = SearchChild("n1").asCom;
        oldLevelText = levelCom.GetChild("n1").asTextField;
        newLevelText = levelCom.GetChild("n3").asTextField;

    }

    public override void InitData<D>(D data)
    {
        OnShowAnimation();
        base.InitData(data);
        levelCom.alpha = 0;
        FavorItem item = data as FavorItem;

        oldLevelText.text = item.oldFavor.ToString() + "级";
        newLevelText.text = item.newFavor.ToString() + "级";
        string fxName = "UI_haogandu_shengji";
        if (item.newFavor - item.oldFavor < 0)
        {
            fxName = "UI_haogandujiangji";
        }
        gGraph = FXMgr.CreateEffectWithScale(SearchChild("n1").asCom, new Vector3(-240, -805), fxName);
        StartCoroutine(ShowLevel());
    }

    IEnumerator ShowLevel()
    {
        yield return new WaitForSeconds(3f);
        levelCom.TweenFade(1, 0.5f);
    }

    public override void OnShowAnimation()
    {
        base.OnShowAnimation();
        ui.visible = true;
        gameObject.SetActive(true);
        StartCoroutine(CloseView());
    }

    IEnumerator CloseView()
    {
        yield return new WaitForSeconds(5.1f);
        UIMgr.Ins.HideView<FavorLevelUpView>();
    }

}

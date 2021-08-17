using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;


[ViewAttr("Game/UI/T_Common", "T_Common", "frame_common_levelup_com")]
public class RoleUpgradeSuccessWindow : BaseWindow
{
    GList _list;
    GComponent fxCom;

    List<TinyItem> tinyItems;
    NormalInfo normalInfo;
    public override void InitUI()
    {
        CreateWindow<RoleUpgradeSuccessWindow>();

        _list = SearchChild("n18").asList;
        fxCom = SearchChild("n17").asCom;

        normalInfo = new NormalInfo();
    }

    public override void ShowWindow<D>(D data)
    {
        base.ShowWindow(data);
        tinyItems = data as List<TinyItem>;
        if (tinyItems != null)
        {
            InitScrollList();
            if (tinyItems[0].voiceId != 0)
            {
                normalInfo.index = tinyItems[0].voiceId;
                EventMgr.Ins.DispachEvent(EventConfig.MUSIC_COMMON_EFFECT, normalInfo);
            }
        }
        FXMgr.CreateEffectWithScale(fxCom, new Vector2(600f, 800f), "UI_framestar_small", 1);

    }



    void InitScrollList()
    {
        _list.itemRenderer = RenderListItem;
        _list.numItems = tinyItems.Count;
    }

    void RenderListItem(int index, GObject obj)
    {
        GComponent gComponent = obj.asCom;
        gComponent.GetChild("n6").asLoader.url = tinyItems[index].url;
        gComponent.GetChild("n10").text = tinyItems[index].name;
        Controller controller = gComponent.GetController("c1");
        controller.selectedIndex = tinyItems[index].num - tinyItems[index].type > 0 ? 0 : 1;
        SetRoleItemEffect(index, gComponent);

        //EventMgr.Ins.DispachEvent(EventConfig.PLAYER_UPGRADE_LEVEL_EFFECT, TotalTextAnima(gComponent.GetChild("n14").asTextField, tinyItems[index].num, tinyItems[index].type));
    }

    void SetRoleItemEffect(int index, GComponent gCom)
    {
        GTextField textField = gCom.GetChild("n14").asTextField;
        textField.text = "+" + tinyItems[index].type;

        Vector3 pos = new Vector3();
        pos = gCom.position;
        if (pos == Vector3.zero)
            pos.y = index * 96f;

        gCom.SetPosition(-464f, pos.y, pos.z);
        float time = index * 0.25f;
        gCom.TweenMoveX(0, (time + 0.5f)).SetEase(EaseType.CubicOut).OnComplete(() =>
        {
            GameMonoBehaviour.Ins.StartCoroutine(TotalTextAnima(textField, tinyItems[index].num, tinyItems[index].type));
        });
    }


    //属性提示效果
    private IEnumerator TotalTextAnima(GTextField gText, int totalPower, int oldPower)
    {
        int times = 10;
        int partNum = (totalPower - oldPower) / times;
        int tempTotal = oldPower;
        gText.text = "+" + tempTotal.ToString();
        for (var i = 0; i < times; i++)
        {
            yield return new WaitForSeconds(0.05f);
            tempTotal += partNum;
            gText.text = "+" + tempTotal.ToString();
            if (i == times - 1)
            {
                gText.text = "+" + totalPower;
            }
        }

    }


    override protected void OnShown()
    {
        window.duration = 2.5f;
        base.OnShown();
        GameMonoBehaviour.Ins.StartCoroutine(GuiderShow());

    }
    IEnumerator GuiderShow()
    {
        yield return new WaitForSeconds(2.5f);
        if (GameData.isGuider)
        {
            if (GameData.guiderCurrent.guiderInfo.flow == 2)
                GameData.guiderCurrent = GuiderInfoLinked.GetCurrGuiderInfo(2, 6);
            else if (GameData.guiderCurrent.guiderInfo.flow == 6)
                GameData.guiderCurrent = GuiderInfoLinked.GetCurrGuiderInfo(6,6);
            else if (GameData.guiderCurrent.guiderInfo.flow == 5)
                GameData.guiderCurrent = GuiderInfoLinked.GetCurrGuiderInfo(5, 7);
            UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);
        }
           
    }



}

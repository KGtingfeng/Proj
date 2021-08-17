using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

[ViewAttr("Game/UI/T_Common", "T_Common", "Frame_common_obtainattribute")]
public class CommonGetPropsToastView : BaseView
{
    GTextField numText;
    GLoader imgLoader;
    TinyItem tinyItem;
    NormalInfo normalInfo;
    public override void InitUI()
    {
        numText = SearchChild("n23").asTextField;
        imgLoader = SearchChild("n24").asLoader;
        normalInfo = new NormalInfo();
        normalInfo.index = (int)SoundConfig.CommonEffectId.AttribuevUpgrade;
    }


    public override void InitData<D>(D data)
    {
        onShow();
        base.InitData(data);
        tinyItem = data as TinyItem;
        EventMgr.Ins.DispachEvent(EventConfig.MUSIC_COMMON_EFFECT, normalInfo);
        if (tinyItem != null)
        {
            numText.text = "+" + tinyItem.num;
            imgLoader.url = tinyItem.url;
            if (StoryView.view.ui.visible == true)
                return;
            Vector2 pos = new Vector2((imgLoader.x + imgLoader.width / 2), (imgLoader.y + imgLoader.height / 2)); ;
            switch (tinyItem.type)
            {
                case (int)TypeConfig.Consume.Diamond:
                    EventMgr.Ins.DispachEvent(EventConfig.DIAMOND_FLY_EFFECT, pos);
                    imgLoader.url = CommonUrlConfig.GetConsumeItemUrl((TypeConfig.Consume)tinyItem.type);
                    break;
                case (int)TypeConfig.Consume.Star:
                    EventMgr.Ins.DispachEvent(EventConfig.STAR_FLY_EFFECT, pos);
                    imgLoader.url = CommonUrlConfig.GetConsumeItemUrl((TypeConfig.Consume)tinyItem.type);
                    break;
            }
        }
    }

    public override void onShow()
    {
        base.onShow();
        ui.alpha = 1;
        ui.y = 0;
        ui.TweenFade(0.2f, 1.5f);
        ui.TweenMoveY(ui.y - 100, 1.5f);
        StartCoroutine(CloseView());
    }

    IEnumerator CloseView()
    {
        yield return new WaitForSeconds(1.6f);
        onHide();
        TouchScreenView.Ins.RemoveToPool(this);
    }
}

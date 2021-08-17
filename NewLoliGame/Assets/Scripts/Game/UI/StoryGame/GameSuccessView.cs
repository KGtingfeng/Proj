using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

[ViewAttr("Game/UI/Y_Game_common", "Y_Game_common", "Frame_Game_success", true)]
public class GameSuccessView : BaseView
{
    GameTipsInfo info;
    GoWrapper topGW;
    AudioClip success;

    public override void InitUI()
    {
        base.InitUI();
        success = Resources.Load(SoundConfig.GAME_AUDIO_EFFECT_URL + (int)SoundConfig.GameAudioId.COMMOND_SUCCESS) as AudioClip;

        InitEvent();
    }
    public override void InitData<D>(D data)
    {
        base.InitData(data);
        info = data as GameTipsInfo;
        SearchChild("n3").asTextField.text = info.context;
        if (topGW == null)
        {
            GGraph g = SearchChild("n10").asGraph;
            topGW = FXMgr.CreateEffectWithGGraph(g, "UI_gamesucced", new Vector3(1, 1, 162), -1);
        }
        else
        {
            topGW.gameObject.SetActive(false);
            topGW.gameObject.SetActive(true);
        }

        GRoot.inst.PlayEffectSound(success);

    }
    public override void InitEvent()
    {
        base.InitEvent();
        SearchChild("n4").asButton.onClick.Set(() =>
        {
            onHide();
            info.callBack();
        });
    }
}
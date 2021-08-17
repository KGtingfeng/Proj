using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;


[ViewAttr("Game/UI/Y_Game_common", "Y_Game_common", "Frame_xinling_thank")]
public class XinlingFinishView : BaseView
{
    GComponent propCom1;
    GComponent propCom2;
    GTextField propName1;
    GTextField propName2;
    
    GameXinlingConfig config;
    public override void InitUI()
    {
        base.InitUI();
        propName1 = SearchChild("n32").asTextField;
        propName2 = SearchChild("n33").asTextField;
        propCom1 = SearchChild("n30").asCom;
        propCom2 = SearchChild("n31").asCom;

        InitEvent();
    }

    public override void InitEvent()
    {
        base.InitEvent();
        SearchChild("n28").onClick.Set(OnClickFinish);
    }

    public override void InitData<D>(D data)
    {
        OnShowAnimation();
        base.InitData(data);
        config = data as GameXinlingConfig;

        List<TinyItem> tinyItems = ItemUtil.GetTinyItmeList(config.award);
        propCom1.GetChild("n64").asTextField.text = tinyItems[0].num.ToString();
        propCom1.GetChild("n62").asLoader.url = tinyItems[0].url;
        GamePropConfig propConfig1 = JsonConfig.GamePropConfigs.Find(a => a.prop_id == tinyItems[0].id);
        propName1.text = propConfig1.prop_name;

        propCom2.GetChild("n64").asTextField.text = tinyItems[1].num.ToString();
        propCom2.GetChild("n62").asLoader.url = tinyItems[1].url;
        GamePropConfig propConfig2 = JsonConfig.GamePropConfigs.Find(a => a.prop_id == tinyItems[1].id);
        propName2.text = propConfig2.prop_name;
        for (int i = 0; i < tinyItems.Count; i++)
        {
            UIMgr.Ins.showNextPopupView<CommonGetPropsView, TinyItem>(tinyItems[i]);
        }
    
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("type", config.type);
        wWWForm.AddField("level", config.ckey);
        GameMonoBehaviour.Ins.RequestInfoPost<TaskAward>(NetHeaderConfig.XINLING_CHECK, wWWForm, (TaskAward propMake) =>
        {
            if (!string.IsNullOrEmpty(propMake.level))
            {
                GameTool.ShowLevelUpEffect(propMake.level);
            }
            EventMgr.Ins.DispachEvent(EventConfig.GAME_FINISH_TASk, config.type - 1);
            TouchScreenView.Ins.ShowPropsTost(propMake.playerProp);
        });
        
        
    }
 
    private void OnClickFinish()
    {
        
            TouchScreenView.Ins.PlayChangeEffect(() => {
            UIMgr.Ins.showNextView<HelpTaskView>();
                AudioClip audioClip = ResourceUtil.LoadBackGroundMusic(SoundConfig.CommonMusicId.BgId);
                GRoot.inst.PlayBgSound(audioClip);
            });
    }
}

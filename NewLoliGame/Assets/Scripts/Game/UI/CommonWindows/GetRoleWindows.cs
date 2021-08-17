using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using FairyGUI;
[ViewAttr("Game/UI/T_Common", "T_Common", "Choose_doll_final")]
public class GetRoleWindows : BaseWindow
{

    GLoader bgLoader;
    GLoader bodyLoader;
    GLoader descLoader; 
    GTextField descText;

    NormalInfo normalInfo = new NormalInfo();
    TinyItem tinyItem;
   


    //GameInitCardsConfig gameInitCardsConfig;
    public override void InitUI()
    {
        CreateWindow<GetRoleWindows>();

        bgLoader = SearchChild("n9").asLoader;
        bodyLoader = SearchChild("n1").asLoader; 
        descLoader = SearchChild("n6").asLoader;
        normalInfo.index = (int)SoundConfig.CommonEffectId.GETDOLL;
        InitEvent();
    }

    public override void ShowWindow<D>(D data)
    {

        base.ShowWindow(data);
        tinyItem = data as TinyItem;
       


    }




    public override void InitEvent()
    {
        //share
        SearchChild("n2").onClick.Add(() =>
        {

        });


        //close
        SearchChild("n3").onClick.Add(HideWindow);
    }

    public override void InitData()
    {
        base.InitData();

        bgLoader.url = UrlUtil.GetChooseDollBgUrl("BG_choosedoll_final");
        if (tinyItem != null)
        {
            bodyLoader.url = tinyItem.url;
            string url = " ";
            switch (tinyItem.type)
            {
                case (int)TypeConfig.Consume.Time:
                    {
                        GameMomentConfig gameMomentConfig = DataUtil.GetGameMomentConfig(tinyItem.id); 
                        if (gameMomentConfig != null )
                        {
                            url = UrlUtil.GetGameMomentUrl(gameMomentConfig.moment_id);
                        }
                    }
                    break;
                default:
                    url =UrlUtil.GeTinyItemUrl(tinyItem.id);
                    break;
            }
            descLoader.url = url; 
        }
        PlaySound();
    }


    void PlaySound()
    {
        EventMgr.Ins.DispachEvent(EventConfig.MUSIC_COMMON_EFFECT, normalInfo);
    }

    protected override void OnShown()
    {

    }
}

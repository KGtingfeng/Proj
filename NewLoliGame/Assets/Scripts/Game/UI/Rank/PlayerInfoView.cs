using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

[ViewAttr("Game/UI/T_Common", "T_Common", "frame_playerinfo")]
public class PlayerInfoView : BaseView
{


    GLoader iconLoader;
    GTextField nameText;
    GTextField titleText;
    GTextField levelText;

    GTextField totalAttributeText;

    Dictionary<string, GTextField> attrList = new Dictionary<string, GTextField>();

    GTextField totalFavorText;

    GTextField totalTimeText;

    GList roleList;

    PlayerInfo playerInfo;

    GLoader titleLoader;
    GGraph titleGraph;

    GLoader frameLoader;
    GGraph frameGraph;

    public override void InitUI()
    {
        base.InitUI();
        iconLoader = SearchChild("n3").asCom.GetChild("n16").asLoader;
        nameText = SearchChild("n4").asTextField;
        levelText = SearchChild("n7").asTextField;
        titleText = SearchChild("n8").asTextField;

        totalAttributeText = SearchChild("n9").asTextField;
        GTextField charmText = SearchChild("n10").asCom.GetChild("n44").asTextField;
        GTextField eveText = SearchChild("n10").asCom.GetChild("n45").asTextField;
        GTextField intellText = SearchChild("n10").asCom.GetChild("n46").asTextField;
        GTextField manaText = SearchChild("n10").asCom.GetChild("n47").asTextField;
        attrList.Add("Charm", charmText);
        attrList.Add("Intell", eveText);
        attrList.Add("Evn", intellText);
        attrList.Add("Mana", manaText);

        totalFavorText = SearchChild("n13").asTextField;

        totalTimeText = SearchChild("n15").asTextField;

        roleList = SearchChild("n18").asList;
        frameLoader = SearchChild("n3").asCom.GetChild("n18").asLoader;
        frameGraph = SearchChild("n3").asCom.GetChild("n19").asGraph;
        titleLoader = SearchChild("n5").asCom.GetChild("n39").asLoader;
        titleGraph = SearchChild("n5").asCom.GetChild("n40").asGraph;

        controller = ui.GetController("c1");
        InitEvent();
    }

    public override void InitEvent()
    {
        base.InitEvent();
        SearchChild("n0").onClick.Set(OnHideAnimation);
        SearchChild("n28").asGraph.onClick.Set(() =>    //申请好友
        {
            WWWForm wWWForm = new WWWForm();
            wWWForm.AddField("ids", playerInfo.player.playerId.ToString());
            GameMonoBehaviour.Ins.RequestInfoPost<HolderData>(NetHeaderConfig.FRIEND_APLLY, wWWForm, (HolderData hd) =>
            {
                controller.selectedIndex = 3;
                UIMgr.Ins.showErrorMsgWindow("好友申请已提交");
            });
        });

        SearchChild("n30").asGraph.onClick.Set(() =>//删除确认
        {
            UIMgr.Ins.showNextPopupView<DelFriendConfirmView, OtherPlayer>(playerInfo.player);

            EventMgr.Ins.RegisterEvent(EventConfig.HIDE_PLAYER_INFO, OnHideAnimation);
        });
    }

    public override void InitData<D>(D data)
    {
        OnShowAnimation();
        base.InitData(data);
        playerInfo = data as PlayerInfo;
        Refresh();
        roleList.SetVirtual();
        roleList.itemRenderer = RenderItem;
        roleList.numItems = playerInfo.favor.Count;


    }

    void Refresh()
    {

        if (playerInfo.player.playerId == GameData.playerId)
        {
            controller.selectedIndex = 0;//是自己，不显示按钮
        }
        else
        {
            if (playerInfo.player.isFriend == 1)
            {
                controller.selectedIndex = 1;//是好友，显示删除
            }
            else
            {
                if (playerInfo.player.isApply == 1)
                {
                    controller.selectedIndex = 3;
                }
                else
                {
                    controller.selectedIndex = 2;//不是好友，显示添加
                }
            }
        }

        iconLoader.url = UrlUtil.GetRoleHeadIconUrl(playerInfo.player.avatar);
        nameText.text = playerInfo.player.name;
        levelText.text = playerInfo.player.level.ToString();
        int totalAttr = 0;
        foreach (var rankInfo in playerInfo.attr)
        {
            attrList[rankInfo.attr].text = rankInfo.val + "";
            totalAttr += rankInfo.val;
        }
        totalAttributeText.text = "总属性:" + totalAttr;

        int totalFavor = 0;
        foreach (var rankInfo in playerInfo.favor)
        {
            totalFavor += rankInfo.val;
        }
        totalFavorText.text = "总角色好感度:" + totalFavor;
        totalTimeText.text = "总收集时刻:" + playerInfo.moment.Count;

        GameAvatarFrameConfig frameConfig = JsonConfig.GameAvatarFrameConfigs.Find(a => a.id == playerInfo.player.frame);
        FXMgr.SetFrame(frameGraph, frameLoader, frameConfig.source_id, 100, new Vector3(135, 134, 1000));


        if (playerInfo.player.title != 0)
        {
            GameTitleConfig titleConfig = JsonConfig.GameTitleConfigs.Find(a => a.id == playerInfo.player.title);
            titleText.text = titleConfig.name_cn;
            FXMgr.SetTitle(titleGraph, titleLoader, titleConfig.level, 75, new Vector3(122, 19, 1000));
        }
        else
        {
            titleText.text = "暂无";
            FXMgr.SetTitle(titleGraph, titleLoader, 1, 75, new Vector3(122, 19, 1000));
        }

    }


    void RenderItem(int index, GObject gObject)
    {
        GComponent gCom = gObject.asCom;
        GLoader roleIconLoader = gCom.GetChild("n19").asLoader;
        GComponent favorCom = gCom.GetChild("n16").asCom;
        GImage loveImage = favorCom.GetChild("n23").asCom.GetChild("n23").asImage;
        GTextField favorText = favorCom.GetChild("n21").asTextField;
        PlayerRankInfo rankInfo = playerInfo.favor[index];
        roleIconLoader.url = UrlUtil.GetStoryHeadIconUrl(int.Parse(rankInfo.attr.Substring(5)));
        int level = GameTool.FavorLevel(rankInfo.val);
        GameTool.SetLevelProgressBar(loveImage, level);
        favorText.text = level + "";

    }


}

using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

public class RoleInfoMoudle : BaseMoudle
{
    static RoleInfoMoudle ins;
    public static RoleInfoMoudle Ins
    {
        get
        {
            return ins;
        }
    }


    GProgressBar progressBar;
    GTextField nameText;
    GTextField lvText;
    GTextField idText;

    /// <summary>
    /// 魅力
    /// </summary>
    GTextField charmText;
    /// <summary>
    /// 环保
    /// </summary>
    GTextField evnText;
    /// <summary>
    /// 智慧
    /// </summary>
    GTextField intellText;
    /// <summary>
    /// 魔法
    /// </summary>
    GTextField manaText;

    //头像
    GComponent headUI;
    GLoader icon;

    GTextField headlvText;

    GList _list;

    GComponent attribuiteCom;
    List<GameInitCardsConfig> Dolls
    {
        get
        {
            return GameData.Dolls;
        }
    }

    PlayerAttribute playerAttribute
    {
        get
        {
            return GameData.Player.attribute;
        }
    }

    Player player { get { return GameData.Player; } }

    public override void InitMoudle<T>(GComponent gComponent, int controllerIndex, T data)
    {
        ins = this;
        base.InitMoudle(gComponent, controllerIndex, data);
        InitUI();
        InitEvent();
        Refresh();
        EventMgr.Ins.RegisterEvent(EventConfig.REFRESH_ROLE_INFO_MOUDEL, Refresh);
        EventMgr.Ins.RegisterEvent(EventConfig.REFRESH_ROLEGROUP_RED_POINT, RefreshRedpoint);
    }

    public override void InitUI()
    {
        base.InitUI();
        GComponent baseCom = SearchChild("n40").asCom;

        nameText = SearchChild("n8").asTextField;
        idText = SearchChild("n47").asTextField;
        progressBar = SearchChild("n14").asProgress;
        lvText = SearchChild("n16").asTextField;
        charmText = baseCom.GetChild("n44").asTextField;
        evnText = baseCom.GetChild("n45").asTextField;
        intellText = baseCom.GetChild("n46").asTextField;
        manaText = baseCom.GetChild("n47").asTextField;
        
         headUI = SearchChild("n4").asCom;
        icon = headUI.GetChild("n16").asLoader;
        headlvText = SearchChild("n45").asTextField;

        _list = SearchChild("n38").asList;
        attribuiteCom = SearchChild("n35").asCom;


    }

    


    public override void InitData<D>(D data)
    {
        base.InitData(data);
        NormalInfo info = data as NormalInfo;
        Refresh();

        if (info != null)
            return;
        if (GameData.isOpenGuider)
        {
            StoryCacheMgr.storyCacheMgr.GetStoryGameSave("Newbie", GameGuideConfig.TYPE_ROLE_GROUP, (storyGameSave) =>
            {
                if (storyGameSave.IsDefault)
                {
                    GameData.guiderCurrent = GuiderInfoLinked.GetCurrGuiderInfo(4, 1);
                    UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);
                    StoryCacheMgr.storyCacheMgr.SaveProgress("Newbie", "1", 4);
                }
            });
        }
    }

   
    void InitScrollList()
    {

        _list.SetVirtualAndLoop();
        _list.itemRenderer = RenderListItem;
        _list.numItems = Dolls.Count;

        _list.scrollPane.onScroll.Add(DoSpecialEffect);
        _list.onClickItem.Set(OnClickItem);
        DoSpecialEffect();
        //_list.apexIndex = 0;
        //_list.EnsureBoundsCorrect();
        //_list.ScrollToView(0);
        _list.ScrollToView(Dolls.Count);
        //_list.ScrollToViewMiddle(Dolls.Count);
        //_list.ScrollToViewMiddle(0);

    }

    void Refresh()
    {
        int level = player.level;
        nameText.text = player.name;
        EventMgr.Ins.DispachEvent(EventConfig.REFRESH_ROLE_TOP_INFO);
        idText.text = player.id + "";
        lvText.text = level.ToString() + "级";
        icon.url = UrlUtil.GetRoleHeadIconUrl(player.avatar);
        headlvText.text = level.ToString();
        PlayerLevelConfig playerLevelConfig = DataUtil.GetPlayerLevelConfig();
        if (playerLevelConfig != null)
        {
            //float max = playerLevelConfig.out_mid_num - playerLevelConfig.add_mid_num;
            //float currentValue = player.exp - playerLevelConfig.add_mid_num;

            //progressBar.max = max;
            ////progressBar.value = currentValue;
            //progressBar.value = 50;
            //progressBar.text = currentValue + "/" + max;
            progressBar.max = playerLevelConfig.out_mid_num;
            progressBar.min = playerLevelConfig.add_mid_num;
            progressBar.value = player.exp;
        }
        charmText.text = playerAttribute.charm + "";
        evnText.text = playerAttribute.evn + "";
        intellText.text = playerAttribute.intell + "";
        manaText.text = playerAttribute.mana + "";

        InitScrollList();
        RefreshRedpoint();
    }

    public override void InitEvent()
    {
        //upgrade role
        SearchChild("n15").onClick.Set(() =>
        {
            RoleUpgrade();
        });

        //upgrade attr
        attribuiteCom.onClick.Set(() =>
        {
            GotoUpgradeAttribuite();
        });

       
        
    }




    void DoSpecialEffect()
    {
        float midX = _list.scrollPane.posX + _list.viewWidth / 2;
        int cnt = _list.numChildren;
        for (int i = 0; i < cnt; i++)
        {
            GObject obj = _list.GetChildAt(i);
            float dist = Mathf.Abs(midX - obj.x - obj.width / 2);
            //Debug.LogError(" ************************* i  " + i +"  dist  "+dist);
            if (dist > obj.width / 4) //no intersection{
            {
                float ss = 1 - (1 - obj.width / 4 / dist) * 0.4f;
                obj.SetScale(ss, ss);
            }
            else
            {
                float ss = 1 + (1 - dist / obj.width) * 0.1f;
                obj.SetScale(1, 1);
            }
        }

    }

    int scrollIndex = 0;
    void RenderListItem(int index, GObject obj)
    {
        obj.SetPivot(0.5f, 0.5f);
        GComponent itemCom = obj.asCom;
        GComponent iconCom = itemCom.GetChild("n6").asCom;
        GLoader gLoader = iconCom.GetChild("n6").asLoader;

        int id = Dolls[index].card_id;

        //gLoader.url = "ui://J_Cha/" + DataUtil.GetDollBodyImg(id);
        Controller controller = itemCom.GetController("c1");
        Controller redController = itemCom.GetController("c2");
        string level = "0";
        int cIndex = 1;
        redController.selectedIndex = 0;

        if (DataUtil.IsOwnDoll(Dolls[index].card_id))
        {
            level = Dolls[index].init_level.ToString();
            scrollIndex = index;
            cIndex = 0;
            gLoader.url = UrlUtil.GetDollSkinIconUrl(id, Dolls[index].skin_id);
            if (RedpointMgr.Ins.dollUpgradeRedpoint.Contains(Dolls[index].card_id))
            {
                redController.selectedIndex = 1;
            }
        }
        else
        {
            gLoader.url = UrlUtil.GetDollSkinIconUrl(id, 0);
            //无法使碎片对应到娃娃，临时处理
            if (RedpointMgr.Ins.dollCombineRedpoint.Contains(Dolls[index].card_id + 3009))
            {
                redController.selectedIndex = 1;
            }
        }

        itemCom.GetChild("n8").text = Dolls[index].name_cn;
        itemCom.GetChild("n9").text = "Lv."+level;

        List<TinyItem> tinyItems = ItemUtil.GetTinyItemForDollConfig(Dolls[index]);
        if (tinyItems.Count == 2)
        {
            itemCom.GetChild("n4").asLoader.url = tinyItems[0].url;
            itemCom.GetChild("n5").asLoader.url = tinyItems[1].url;
        }
        controller.selectedIndex = cIndex;


    }



    private void OnClickItem(EventContext context)
    {
        int selectedIndex = _list.GetChildIndex((GObject)context.data);
        int itemIndex = _list.ChildIndexToItemIndex(selectedIndex);
        baseView.GoToMoudle<DollUpgradeMoudle, GameInitCardsConfig>((int)RoleGropView.MoudleType.DollUpgrade, Dolls[itemIndex]);
    }

    public void NewbieDollUpgrade()
    {
        baseView.GoToMoudle<DollUpgradeMoudle, GameInitCardsConfig>((int)RoleGropView.MoudleType.DollUpgrade, Dolls[1]);

    }

    public void GotoUpgradeAttribuite()
    {
        baseView.StartCoroutine(GotoAttribuite());
    }

    IEnumerator GotoAttribuite()
    {
        TouchScreenView.Ins.PlayTmpEffect();
        yield return new WaitForSeconds(0.8f);
        //baseView.GoToMoudle<RoleAttribuiteUpgradeMoudle, HolderData>((int)RoleGropView.MoudleType.AttributeUpgrade, null);
        UIMgr.Ins.showNextPopupView<RoleAttribuiteUpgradeView>();

    }

    public void RoleUpgrade()
    {
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("propId", "2010,2011,2012");
        GameMonoBehaviour.Ins.RequestInfoPost<List<PlayerProp>>(NetHeaderConfig.PLAYER_PROP_INFO, wWWForm, RequestProps);
    }

    void RequestProps(List<PlayerProp> props)
    {
        UIMgr.Ins.showNextPopupView<RoleUpgradeView, List<PlayerProp>>(props);

    }

    void RefreshRedpoint()
    {
        attribuiteCom.GetController("c1").selectedIndex = RedpointMgr.Ins.AttributeHaveRedpoint() ? 1 : 0;
        _list.RefreshVirtualList();
    }
}

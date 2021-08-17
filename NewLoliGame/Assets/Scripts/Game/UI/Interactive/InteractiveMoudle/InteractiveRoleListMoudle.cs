using FairyGUI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class InteractiveRoleListMoudle : BaseMoudle
{
    public static InteractiveRoleListMoudle ins;
    enum TYPE
    {
        ALL = 0,
        HUMAN,
        FAIRY,
    }
    GLoader bgLoader;
    GList roleList;
    GList btnList;

    GComponent role;
    GLoader roleIcon;
    GTextField consumeNum;
    GTextField roleName;
    GComponent favorBarCom;

    GGroup leftGroup;
    GGroup rightGroup;
    GGroup upGroup;

    //默认点击所有
    int selectType = 0;
    public List<GameInitCardsConfig> allRoles;

    public List<Role> ownRoles
    {
        get { return GameData.OwnRoleList; }
    }
    //仙子、人类
    List<GameInitCardsConfig> fairyGameInitCardsConfigs;
    List<GameInitCardsConfig> humanGameInitCardsConfigs;
    //获取当前展示的角色信息
    List<GameInitCardsConfig> currentRoleList;
    public override void InitMoudle(GComponent gComponent, int controllerIndex)
    {
        base.InitMoudle(gComponent, controllerIndex);
        InitUI();
        InitEvent();
    }

    public override void InitUI()
    {
        base.InitUI();
        roleList = SearchChild("n41").asList;
        btnList = SearchChild("n40").asList;
        bgLoader = SearchChild("n43").asLoader;

        leftGroup = SearchChild("n97").asGroup;
        rightGroup = SearchChild("n98").asGroup;
        upGroup = SearchChild("n33").asGroup;
        ins = this;
    }

    public override void InitData()
    {
        bgLoader.url = UrlUtil.GetInteractiveBgUrl(1);
        if (ownRoles == null || ownRoles.Count <= 0)
        {
            RequestRoleListInfo();
            return;
        }
        btnList.selectedIndex = 0;
        GetCategoricalData(0);
        RefreshItems();
        if (GameData.isGuider)
        {
            UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);
        }
    }

    public override void InitEvent()
    {
        btnList.onClickItem.Set(() =>
        {
            if (selectType != btnList.selectedIndex)
            {
                selectType = btnList.selectedIndex;
                GetCategoricalData(selectType);
                RefreshItems();
            }
        });

    }


    //数据为null时使用
    void RequestRoleListInfo()
    {
        WWWForm wWWForm = new WWWForm();
        GameMonoBehaviour.Ins.RequestInfoPostList<Role>(NetHeaderConfig.ACTOR_LIST, wWWForm, () =>
        {
            btnList.selectedIndex = 0;
            GetCategoricalData(0);
            RefreshItems();
            if (GameData.isGuider)
            {
                UIMgr.Ins.showNextPopupView<NewbieGuideView, NormalInfo>(null);
            }
        });
    }

    /// <summary>
    /// 获得分类数据
    /// </summary>
    void GetCategoricalData(int selectIndex)
    {
        allRoles = JsonConfig.GameInitCardsConfigs.Where(a => a.type!=0 && a.type != 1).ToList();
        allRoles.Sort(delegate (GameInitCardsConfig configA, GameInitCardsConfig configB)
        {
            return configA.card_id.CompareTo(configA.card_id);
        });
        List<GameInitCardsConfig> haveRoles = new List<GameInitCardsConfig>();
        List<GameInitCardsConfig> notHaveRoles = new List<GameInitCardsConfig>();

        foreach (var role in allRoles)
        {
            if (ownRoles.Find(a => a.id == role.card_id) != null)
                haveRoles.Add(role);
            else
                notHaveRoles.Add(role);
        }
        allRoles = haveRoles;
        allRoles.AddRange(notHaveRoles);
        fairyGameInitCardsConfigs = new List<GameInitCardsConfig>();
        humanGameInitCardsConfigs = new List<GameInitCardsConfig>();

        foreach (var item in allRoles)
        {
            if (item.status)
            {
                fairyGameInitCardsConfigs.Add(item);
            }
            else
            {
                humanGameInitCardsConfigs.Add(item);
            }
        }
        switch (selectIndex)
        {
            case (int)TYPE.ALL:
                currentRoleList = allRoles;
                break;
            case (int)TYPE.HUMAN:
                currentRoleList = humanGameInitCardsConfigs;
                break;
            case (int)TYPE.FAIRY:
                currentRoleList = fairyGameInitCardsConfigs;
                break;
        }

    }

    void RefreshItems()
    {
        for (int i = roleList.numItems-1; i >= 0; i--)
        {
            GObject gObject = roleList.RemoveChildAt(i,true);
        }
        roleList.itemRenderer = RenderListItem;
        roleList.numItems = currentRoleList.Count;
        if (currentRoleList.Count > 0)
            roleList.ScrollToView(0);
    }


    void RenderListItem(int index, GObject obj)
    {
        obj.SetPivot(0.5f, 0.5f);
        obj.alpha = 0;
        GComponent itemCom = obj.asCom;
        role = itemCom.GetChild("n6").asCom;
        role.GetChild("n6").asLoader.url = UrlUtil.GetDollSkinIconUrl(currentRoleList[index].card_id, 0);
        favorBarCom = itemCom.GetChild("n46").asCom;
        GTextField favorLevel = favorBarCom.GetChild("n21").asTextField;

        Controller controller = itemCom.GetController("c1");
        Role tmpRole = ownRoles.Where(a => a.id == currentRoleList[index].card_id).FirstOrDefault();
        if (tmpRole == null)
        {
            controller.selectedIndex = 1;
            favorLevel.text = "0";
            GameTool.SetLevelProgressBar(favorBarCom.GetChild("n23").asCom.GetChild("n23").asImage, 0);
            TinyItem tinyItem = ItemUtil.GetTinyItem(currentRoleList[index].price);
            consumeNum = itemCom.GetChild("n42").asTextField;
            consumeNum.text = "" + tinyItem.num;
        }
        else
        {
            int level = GameTool.FavorLevel(tmpRole.actorFavorite);
            favorLevel.text = "" + level;
            GameTool.SetLevelProgressBar(favorBarCom.GetChild("n23").asCom.GetChild("n23").asImage, level);
            roleName = itemCom.GetChild("n44").asTextField;
            roleName.text = currentRoleList[index].name_cn;
            controller.selectedIndex = 0;
        }

        itemCom.onClick.Set(() => { ClickItem(tmpRole != null, currentRoleList[index]); });

        Controller redpointController = itemCom.GetController("c2");
        redpointController.selectedIndex = RedpointMgr.Ins.wishRedpoint.Contains(currentRoleList[index].card_id) ? 1 : 0;
        if (RedpointMgr.Ins.wishRedpoint.Contains(currentRoleList[index].card_id))
        {
            redpointController.selectedIndex = 1;
        }
        SetGoodsItemEffect(index, obj);
    }

    void SetGoodsItemEffect(int i, GObject item)
    {
        Vector3 pos = new Vector3();
        item.alpha = 0;
        
        pos = GetItemPos(i, item);
        item.SetPosition(pos.x, pos.y + 200, pos.z);
        float time = i * 0.1f;

        item.TweenMoveY(pos.y, (time + 0.1f)).SetEase(EaseType.CubicOut).OnStart(() =>
        {
            item.TweenFade(1, (time + 0.15f)).SetEase(EaseType.QuadOut);
        });
    }

    Vector3 GetItemPos(int index, GObject gObject)
    {
        Vector3 pos = gObject.position;
        if (pos == Vector3.zero)
        {
            pos.x = index % 2 == 0 ? 0f : 373f;
            pos.y = index / 2 * 455f;
        }
        return pos;
    }


    void ClickItem(bool isOwn, GameInitCardsConfig cardsConfig)
    {
        AudioClip audioClip = ResourceUtil.LoadEffect(SoundConfig.CommonEffectId.ClickBtn);
        GRoot.inst.PlayClickSound(audioClip);
        if (isOwn)
        {
            //InteractiveDataMgr.ins.SelectRoleCardConfig = cardsConfig;
            RequestSkinList(cardsConfig.card_id);
        }
        else
            RequestActorBuy(cardsConfig.card_id);
    }

    public void NewbieRequest()
    {
        RequestSkinList(11);
    }

    void RequestSkinList(int id)
    {
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("actorId", id);
        GameMonoBehaviour.Ins.RequestInfoPost<PlayerActor>(NetHeaderConfig.ACTOR_SKIN_LIST, wWWForm, RequestSkinListSuccess);
    }


    void RequestSkinListSuccess()
    {
        upGroup.alpha = 0;
        upGroup.y = 260;
        baseView.GoToMoudle<InteractiveMainMoudle>((int)InteractiveView.MoudleType.INTERACTIVE);
    }

    /// <summary>
    /// 购买角色
    /// </summary>
    /// <param name="cardId"></param>
    public void RequestActorBuy(int cardId)
    {
        //确认支付后
        System.Action questCallback = () =>
        {
            if (cardId >= 28 && cardId <= 34)
            {
                WWWForm wWWForm = new WWWForm();
                wWWForm.AddField("cardId", cardId - 27);
                GameMonoBehaviour.Ins.RequestInfoPost<Player>(NetHeaderConfig.DOLL_BUY, wWWForm, RequestRoleListInfo);
            }
            else
            {
                WWWForm wWWForm = new WWWForm();
                wWWForm.AddField("actorId", cardId);
                GameMonoBehaviour.Ins.RequestInfoPostList<Role>(NetHeaderConfig.ACTOR_BUY, wWWForm, ActorBuyCallback);
            }
        };
        Extrand extrand = new Extrand();
        extrand.key = cardId.ToString();
        extrand.callBack = questCallback;

        UIMgr.Ins.showNextPopupView<ContactTipsView, Extrand>(extrand);
    }

    void ActorBuyCallback()
    {
        GetCategoricalData(btnList.selectedIndex);
        RefreshItems();
        WWWForm wWWForm = new WWWForm();
        GameMonoBehaviour.Ins.RequestInfoPost<PlayerRedpoint>(NetHeaderConfig.RED_POINT, wWWForm, () =>
        {
            RefreshItems();
        }, false);
    }

}

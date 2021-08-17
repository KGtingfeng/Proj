using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;
using System;

public class DollSuitMoudle : BaseMoudle
{

    GameInitCardsConfig doll;
    GTextField nameText;
    GTextField levelText;
    GLoader itemIconLoader;
    GTextField itemNameText;
    GTextField itemNumText;
    GLoader bodyLoader;
    GLoader bgLoader;
    GLoader frameLoader;
    GList _list;

    List<GameCardsSkinConfig> skinConfigs = new List<GameCardsSkinConfig>();
    List<GameCardsSkinConfig> haveSkins = new List<GameCardsSkinConfig>();
    int cardIndex = 0;

    public override void InitMoudle<T>(GComponent gComponent, int controllerIndex, T data)
    {
        base.InitMoudle(gComponent, controllerIndex, data);
        doll = data as GameInitCardsConfig;

        InitUI();
        InitEvent();
        Refresh();
    }


    public override void InitUI()
    {
        base.InitUI();
        bgLoader = SearchChild("n42").asLoader;
        frameLoader = SearchChild("n39").asLoader;
        nameText = SearchChild("n20").asTextField;
        levelText = SearchChild("n21").asTextField;
        itemIconLoader = SearchChild("n29").asLoader;
        itemNameText = SearchChild("n30").asTextField;
        itemNumText = SearchChild("n31").asTextField;
        bodyLoader = SearchChild("n27").asLoader;
        _list = SearchChild("n33").asList;

    }

    public override void InitEvent()
    {
        base.InitEvent();
        //close
        SearchChild("n34").onClick.Add(Reback);

        //chuchuang
        SearchChild("n19").onClick.Add(() =>
        {

        });

        //left btn 
        SearchChild("n6").onClick.Add(() =>
        {
            if (cardIndex == 0)
                cardIndex = GameData.Dolls.Count;
            cardIndex--;

            doll = GameData.Dolls[cardIndex];
            Refresh();
        });
        //right
        SearchChild("n7").onClick.Add(() =>
        {
            if (cardIndex == (GameData.Dolls.Count - 1))
                cardIndex = -1;
            cardIndex++;

            doll = GameData.Dolls[cardIndex];
            Refresh();
        });


    }

    public override void InitData<D>(D data)
    {
        base.InitData(data);
        doll = data as GameInitCardsConfig;
        bgLoader.url = UrlUtil.GetCommonBgUrl("roleshow2");
        frameLoader.url = UrlUtil.GetCommonBgUrl("roleshow1");
        Refresh();
    }

    //最后选择的type
    int lastType;
    void Refresh()
    {
        cardIndex = DataUtil.FindDollIndex(doll.card_id);
        bool isOwnDoll = DataUtil.IsOwnDoll(doll.card_id);
        nameText.text = doll.name_cn;
        levelText.text = isOwnDoll ? doll.init_level.ToString() : "0";
        TinyItem tinyItem;
        //已拥有娃娃
        if (isOwnDoll)
        {
            bodyLoader.url = UrlUtil.GetDollSkinIconUrl(doll.card_id, doll.skin_id);
            GetSkins();
            tinyItem = ItemUtil.GetTinyItemForSkinConfig(skinConfigs.Find(a => a.type == doll.skin_id));
            lastType = doll.skin_id;
        }
        else
        {
            bodyLoader.url = UrlUtil.GetDollSkinIconUrl(doll.card_id, 0);
            skinConfigs.Clear();
            skinConfigs = JsonConfig.GameCardsSkinConfigs.FindAll(a => a.card_id == doll.card_id);
            tinyItem = ItemUtil.GetTinyItemForSkinConfig(skinConfigs.Find(a => a.type == 0));
            lastType = 0;
        }

        if (tinyItem != null)
        {
            itemIconLoader.url = tinyItem.url;
            itemNameText.text = tinyItem.name;
            itemNumText.text = tinyItem.num + "";
        }

        _list.SetVirtual();
        //只有一个皮肤
        if (skinConfigs.Count == 1)
        {
            _list.itemRenderer = RenderListItemOne;
            _list.numItems = 2;
        }
        else
        {
            _list.itemRenderer = RenderListItem;
            _list.numItems = skinConfigs.Count;
            _list.onClickItem.Set(OnClickItem);
        }
    }

    void GetSkins()
    {
        skinConfigs.Clear();
        haveSkins.Clear();
        //Debug.LogError("***************** ownSKins   " + doll.OwnSkins);
        List<int> ownSkinsId = new List<int>();
        if (DataUtil.IsOwnDoll(doll.card_id))
        {
            string[] ownSkins = doll.OwnSkins.Split(new Char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string type in ownSkins)
            {
                ownSkinsId.Add(int.Parse(type));
            }
        }

        List<GameCardsSkinConfig> skins = JsonConfig.GameCardsSkinConfigs.FindAll(a => a.card_id == doll.card_id);
        if (skins != null)
        {
            for (int i = skins.Count - 1; i >= 0; i--)
            {
                if (ownSkinsId.Contains(skins[i].type))
                {
                    haveSkins.Add(skins[i]);
                    skins.Remove(skins[i]);
                }
            }
            haveSkins.Sort(SortSkins);
            skins.Sort(SortSkins);
            if (haveSkins.Count > 1)
            {
                GameCardsSkinConfig skin = haveSkins.Find(a => a.type == doll.skin_id);
                haveSkins.Remove(skin);
                haveSkins.Insert(0, skin);
            }
            skinConfigs.AddRange(haveSkins);
            skinConfigs.AddRange(skins);
        }
    }

    private int SortSkins(GameCardsSkinConfig tr1, GameCardsSkinConfig tr2)
    {
        return tr1.type.CompareTo(tr2.type);
    }


    //只有一个皮肤
    void RenderListItemOne(int index, GObject obj)
    {
        obj.SetPivot(0.5f, 0.5f);
        GComponent itemCom = obj.asCom;
        itemCom.GetChild("n50").asButton.changeStateOnClick = false;
        GLoader gLoader = itemCom.GetChild("n36").asCom.GetChild("n36").asLoader;
        itemCom.GetChild("n33").text = skinConfigs[0].name_cn;
        gLoader.url = UrlUtil.GetDollSkinIconUrl(skinConfigs[0].card_id, skinConfigs[0].type);


        if (lastType == index)
        {
            _list.GetChildAt(index).asCom.GetChild("n50").asButton.selected = true;
        }
        Controller controller = itemCom.GetController("c1");


        if (index != 0)
            controller.selectedIndex = 3;

    }

    void RenderListItem(int index, GObject obj)
    {
        obj.SetPivot(0.5f, 0.5f);
        GComponent itemCom = obj.asCom;
        itemCom.GetChild("n50").asButton.changeStateOnClick = false;
        GLoader gLoader = itemCom.GetChild("n36").asCom.GetChild("n36").asLoader;
        itemCom.GetChild("n33").text = skinConfigs[index].name_cn;
        gLoader.url = UrlUtil.GetDollSkinIconUrl(skinConfigs[index].card_id, skinConfigs[index].type);
        if (lastType == skinConfigs[index].type)
            itemCom.GetChild("n50").asButton.selected = true;
        else
            itemCom.GetChild("n50").asButton.selected = false;

        Controller controller = itemCom.GetController("c1");
        int cIndex = 2;
        //已拥有娃娃
        if (DataUtil.IsOwnDoll(doll.card_id))
        {
            if (skinConfigs[index].type == doll.skin_id)
                cIndex = 0;
            else if (haveSkins.Find(a => a.type == skinConfigs[index].type) != null)
            {
                cIndex = 1;
                itemCom.GetChild("n39").onClick.Set(() => SetDollSkin(skinConfigs[index].type));
            }
            else
            {
                TinyItem item = ItemUtil.GetTinyItem(skinConfigs[index].price);
                itemCom.GetChild("n43").asTextField.text = item.num + "";
                itemCom.GetChild("n42").asLoader.url = item.url;
                itemCom.GetChild("n40").onClick.Set(() => BuyDollSkin(skinConfigs[index].type));
            }
        }
        else
        {
            if (index == 0)
                cIndex = 0;
            else
            {
                TinyItem item = ItemUtil.GetTinyItem(skinConfigs[index].price);
                itemCom.GetChild("n43").asTextField.text = item.num + "";
                itemCom.GetChild("n42").asLoader.url = item.url;
                itemCom.GetChild("n40").onClick.Set(() =>
                {
                    UIMgr.Ins.showErrorMsgWindow(MsgException.NO_HAVE_DOLL);
                });
            }

        }

        controller.selectedIndex = cIndex;

    }


    private void OnClickItem(EventContext context)
    {
        //在列表里的索引
        int itemIndex = _list.GetChildIndex((GObject)context.data);
        //真实索引
        int realIndex = _list.ChildIndexToItemIndex(itemIndex);
        //Debug.LogError("*********** realIndex   " + realIndex + "  itemIndex   " + itemIndex);
        if (lastType == skinConfigs[realIndex].type)
            return;
        if (skinConfigs.Count > 1)
        {
            //单选控制
            for (int i = 0; i < _list.numChildren; i++)
                _list.GetChildAt(i).asCom.GetChild("n50").asButton.selected = false;
            _list.GetChildAt(itemIndex).asCom.GetChild("n50").asButton.selected = true;
            lastType = skinConfigs[realIndex].type;
            TinyItem tinyItem = ItemUtil.GetTinyItemForSkinConfig(skinConfigs.Find(a => a.type == skinConfigs[realIndex].type));
            if (tinyItem != null)
            {
                itemIconLoader.url = tinyItem.url;
                itemNameText.text = tinyItem.name;
                itemNumText.text = tinyItem.num + "";
            }

            bodyLoader.url = UrlUtil.GetDollSkinIconUrl(skinConfigs[realIndex].card_id, skinConfigs[realIndex].type);
        }

        //if (DataUtil.IsOwnDoll(Dolls[itemIndex].card_id))
        //{
        //    BaseView.view.GoToMoudle<DollUpgradeMoudle, Doll>((int)RoleGropView.MoudleType.DollUpgrade, Dolls[itemIndex]);
        //}
        //else
        //{
        //    BaseView.view.GoToMoudle< DollNotHaveMoudle, Doll >((int)RoleGropView.MoudleType.Doll_NotHave, Dolls[itemIndex]);
        //}
        //BaseView.view.GoToMoudle<DollUpgradeMoudle, Doll>((int)RoleGropView.MoudleType.DollUpgrade, Dolls[itemIndex]);

    }

    void SetDollSkin(int type)
    {
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("cardId", doll.card_id);
        wWWForm.AddField("skinId", type);
        GameMonoBehaviour.Ins.RequestInfoPost<Player>(NetHeaderConfig.DOLL_SKIN_SET, wWWForm, BuyDollSkinSuccess);
    }

    void BuyDollSkin(int type)
    {
        WWWForm wWWForm = new WWWForm();
        wWWForm.AddField("cardId", doll.card_id);
        wWWForm.AddField("skinId", type);
        GameMonoBehaviour.Ins.RequestInfoPost<Player>(NetHeaderConfig.DOLL_SKIN_BUY, wWWForm, BuyDollSkinSuccess);
    }

    void BuyDollSkinSuccess()
    {
        doll = GameData.Dolls.Find(a => a.card_id == doll.card_id);
        Refresh();
    }

    public override void Reback()
    {
        baseView.GoToMoudle<DollUpgradeMoudle, GameInitCardsConfig>((int)RoleGropView.MoudleType.DollUpgrade, doll);
    }
}

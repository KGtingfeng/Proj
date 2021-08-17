using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemUtil
{

    /// <summary>
    /// Gets the tiny item.
    /// </summary>
    /// <param name="contex">用,分离，包括道具类型和消耗数量Contex.</param>
    public static TinyItem GetTinyItemTwo(string contex)
    {

        string[] arry = contex.Split(',');
        if (arry.Length != 2)
            return null;

        TinyItem tinyItem = new TinyItem();
        tinyItem.type = int.Parse(arry[0]);
        tinyItem.url = CommonUrlConfig.GetConsumeItemUrl((TypeConfig.Consume)tinyItem.type);
        tinyItem.num = int.Parse(arry[1]);
        return tinyItem;

    }

    /// <summary>
    /// 通用类型
    /// </summary>
    /// <returns>The tiny item.</returns>
    /// <param name="contex">Contex.</param>
    public static TinyItem GetTinyItem(string contex)
    {
        string[] arry = contex.Split(',');
        if (arry.Length < 3)
            return null;
        TinyItem tinyItem = new TinyItem();
        tinyItem.type = int.Parse(arry[0]);
        tinyItem.url = CommonUrlConfig.GetConsumeItemUrl((TypeConfig.Consume)tinyItem.type);
        if (int.Parse(arry[1]) == (int)TypeConfig.PropType.PACK)
        {
            tinyItem.type = int.Parse(arry[1]);
            tinyItem.id = int.Parse(arry[0]);
            tinyItem.num = int.Parse(arry[2]);

            tinyItem.url = "Game/Props/" + tinyItem.id;
            return tinyItem;
        }
        switch (tinyItem.type)
        {
            case (int)TypeConfig.Consume.Time:
                tinyItem.id = int.Parse(arry[2]);
                tinyItem.url += tinyItem.id;
                break;
            case (int)TypeConfig.Consume.Star:
                tinyItem.id = 100;
                tinyItem.url = UrlUtil.GetPropsIconUrl(tinyItem);

                break;
            case (int)TypeConfig.Consume.Diamond:
                tinyItem.id = 1;
                tinyItem.url = UrlUtil.GetPropsIconUrl(tinyItem);

                break;
            case (int)TypeConfig.Consume.Item:
            case (int)TypeConfig.Consume.AvatarFrame:
            case (int)TypeConfig.Consume.Title:
            case (int)TypeConfig.Consume.Friendly:
                tinyItem.id = int.Parse(arry[1]);
                tinyItem.url = UrlUtil.GetPropsIconUrl(tinyItem);
                break;
            default:
                tinyItem.id = int.Parse(arry[1]);
                break;
        }
        tinyItem.num = int.Parse(arry[2]);
        tinyItem.name = GetConsumeName((TypeConfig.Consume)tinyItem.type);
        return tinyItem;

    }

    public static List<TinyItem> GetTinyItmeList(string context)
    {
        List<TinyItem> tinyItems = new List<TinyItem>();
        string[] arry = context.Split(';');
        if (arry.Length == 0)
            return null;
        TinyItem tinyItem;
        foreach (var item in arry)
        {
            if (item.Length < 3)
                continue;
            tinyItem = GetTinyItem(item);
            tinyItems.Add(tinyItem);
        }
        return tinyItems;
    }

    /// <summary>
    /// 获取道具
    /// </summary>
    /// <param name="contex"></param>
    /// <returns></returns>
    public static TinyItem GetPlayerPropItem(string contex)
    {
        string[] arry = contex.Split(',');
        if (arry.Length < 3)
            return null;
        TinyItem tinyItem = new TinyItem();
        tinyItem.id = int.Parse(arry[0]);
        tinyItem.type = int.Parse(arry[1]);
        tinyItem.num = int.Parse(arry[2]);
        return tinyItem;

    }

    public static TinyItem GetTinyItemTwoForAttribute(string contex)
    {

        string[] arry = contex.Split(',');
        if (arry.Length != 2)
            return null;

        TinyItem tinyItem = new TinyItem();
        tinyItem.type = int.Parse(arry[0]);
        tinyItem.url = GetAttributeUrl(tinyItem.type);
        tinyItem.num = int.Parse(arry[1]);
        tinyItem.name = GetProperty(tinyItem.type);
        return tinyItem;

    }

    public static string GetAttributeUrl(int type)
    {
        string url = "";
        switch (type)
        {
            //魅力
            case (int)TypeConfig.CIEM.CHARM:
                url = CommonUrlConfig.GetCharmUrl();
                break;
            //智力
            case (int)TypeConfig.CIEM.INTELL:
                url = CommonUrlConfig.GetWisdomUrl();
                break;
            //环保
            case (int)TypeConfig.CIEM.ENV:
                url = CommonUrlConfig.GetEnvUrl();
                break;
            //魔法
            case (int)TypeConfig.CIEM.MAGIC:
                url = CommonUrlConfig.GetMagicUrl();

                break;
            default:
                break;
        }

        return url;
    }



    /// <summary>
    /// 用于计算出娃娃显示的属性
    /// </summary>
    /// <returns>The tiny item for doll config.</returns>
    /// <param name="contex">Contex.</param>
    public static List<TinyItem> GetTinyItemForDollConfig(GameInitCardsConfig gameInitCardsConfig)
    {
        List<TinyItem> tinyItems = new List<TinyItem>();
        if (gameInitCardsConfig != null)
        {
            //魅力
            if (gameInitCardsConfig.charm != 0)
            {
                TinyItem t1 = new TinyItem();
                t1.name = "魅力";
                t1.num = gameInitCardsConfig.charm;
                t1.url = CommonUrlConfig.GetCharmUrl();
                tinyItems.Add(t1);

            }

            //智力
            if (gameInitCardsConfig.intell != 0)
            {
                TinyItem t2 = new TinyItem();
                t2.name = "智慧";
                t2.num = gameInitCardsConfig.intell;
                t2.url = CommonUrlConfig.GetWisdomUrl();
                tinyItems.Add(t2);

            }

            //环保
            if (gameInitCardsConfig.evn != 0)
            {
                TinyItem t3 = new TinyItem();
                t3.name = "环保";
                t3.num = gameInitCardsConfig.evn;
                t3.url = CommonUrlConfig.GetEnvUrl();
                tinyItems.Add(t3);

            }


            //魔法
            if (gameInitCardsConfig.mana != 0)
            {
                TinyItem t4 = new TinyItem();
                t4.name = "魔法";
                t4.num = gameInitCardsConfig.mana;
                t4.url = CommonUrlConfig.GetMagicUrl();
                tinyItems.Add(t4);

            }


        }

        return tinyItems;

    }

    /// <summary>
    /// 获取皮肤加成
    /// </summary>
    /// <param name="gameInitCardsConfig"></param>
    /// <returns></returns>
    public static TinyItem GetTinyItemForSkinConfig(GameCardsSkinConfig gameCardsSkinConfig)
    {
        TinyItem tinyItem = new TinyItem();
        if (gameCardsSkinConfig != null)
        {
            //魅力
            if (gameCardsSkinConfig.charm != 0)
            {
                tinyItem.name = "魅力";
                tinyItem.num = gameCardsSkinConfig.charm;
                tinyItem.url = CommonUrlConfig.GetCharmUrl();
                return tinyItem;
            }

            //智力
            if (gameCardsSkinConfig.intell != 0)
            {
                tinyItem.name = "智慧";
                tinyItem.num = gameCardsSkinConfig.intell;
                tinyItem.url = CommonUrlConfig.GetWisdomUrl();
                return tinyItem;
            }

            //环保
            if (gameCardsSkinConfig.evn != 0)
            {
                tinyItem.name = "环保";
                tinyItem.num = gameCardsSkinConfig.evn;
                tinyItem.url = CommonUrlConfig.GetEnvUrl();
                return tinyItem;
            }


            //魔法
            if (gameCardsSkinConfig.mana != 0)
            {
                tinyItem.name = "魔法";
                tinyItem.num = gameCardsSkinConfig.mana;
                tinyItem.url = CommonUrlConfig.GetMagicUrl();
                return tinyItem;
            }


        }
        return null;

    }

    public static List<TinyItem> GetTinyItemsForDollUpgrade(GameInitCardsConfig from, GameInitCardsConfig to)
    {
        List<TinyItem> tinyItems = new List<TinyItem>();
        if (to != null)
        {
            //魅力
            if (to.charm > from.charm)
            {
                TinyItem t1 = new TinyItem();
                t1.name = "魅力";
                t1.num = to.charm;// - from.charm;
                t1.url = CommonUrlConfig.GetCharmUrl();
                tinyItems.Add(t1);

            }

            //智力
            if (to.intell > from.intell)
            {
                TinyItem t2 = new TinyItem();
                t2.name = "智慧";
                t2.num = to.intell;// - from.intell;
                t2.url = CommonUrlConfig.GetWisdomUrl();
                tinyItems.Add(t2);

            }

            //环保
            if (to.evn > from.evn)
            {
                TinyItem t3 = new TinyItem();
                t3.name = "环保";
                t3.num = to.evn;// - from.evn;
                t3.url = CommonUrlConfig.GetEnvUrl();
                tinyItems.Add(t3);

            }


            //魔法
            if (to.mana > from.mana)
            {
                TinyItem t4 = new TinyItem();
                t4.name = "魔法";
                t4.num = to.mana;// - from.mana;
                t4.url = CommonUrlConfig.GetMagicUrl();
                tinyItems.Add(t4);

            }


        }

        return tinyItems;

    }

    /// <summary>
    /// 获取商品列表
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    public static List<TinyItem> GetMallItemList(string content)
    {
        List<TinyItem> tinyItems = new List<TinyItem>();
        string[] arry = content.Split(';');
        if (arry.Length == 0)
            return null;
        TinyItem tinyItem;
        foreach (var item in arry)
        {
            if (item.Length < 3)
                continue;
            tinyItem = GetPlayerPropItem(item);
            tinyItems.Add(tinyItem);
        }
        return tinyItems;

    }


    /// <summary>
    /// 获取4个基本属性
    /// </summary>
    /// <returns>The property.</returns>
    /// <param name="type">Type.</param>
    public static string GetProperty(int type)
    {
        return RoleConfig.fourPropertiesKeyPairs[type];
    }


    /// <summary>
    /// 获取道具大类名字
    /// </summary>
    /// <returns>The consume name.</returns>
    /// <param name="consume">Consume.</param>
    public static string GetConsumeName(TypeConfig.Consume consume)
    {
        //默认是爱心
        string name = "道具";
        switch (consume)
        {
            case TypeConfig.Consume.Diamond:
                name = "钻石";
                break;
            case TypeConfig.Consume.Time:
                name = "时刻";
                break;
            case TypeConfig.Consume.Star:
                name = "星星";
                break;
            case TypeConfig.Consume.Friendly:
                name = "好感度";
                break;
        }

        return name;
    }

}

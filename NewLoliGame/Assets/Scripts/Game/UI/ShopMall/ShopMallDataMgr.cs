using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class ShopMallDataMgr
{
    /// <summary>
    /// 根据ID约定,
    ///星星商城 （后端ID约定格式：xxx）
    ///充值商城 （后端ID约定格式：600xxx）
    ///礼包商城 （后端ID约定格式：700xxx）
    ///钻石商城 （后端ID约定格式：900xxx）
    /// </summary>
    public enum MallType
    {
        LOVE = 0,
        RECHARGE = 6,
        GIFT = 7,
        CARD = 8,
        DIAMOND = 9,
    }
    public enum ClickType
    {
        CARD = 0,
        RECHARGE = 1,
        LOVE = 2,
        DIAMOND = 3,
        GIFT = 4,
    }

    static ShopMallDataMgr shopMallData;
    public static ShopMallDataMgr ins
    {
        get
        {
            if (shopMallData == null)
                shopMallData = new ShopMallDataMgr();
            return shopMallData;
        }
    }

    public List<PlayerProp> cardsInfo
    {
        get; set;
    }

    /// <summary>
    /// 服务器时间
    /// </summary>
    DateTime serverDateTime;
    public DateTime ServerDateTime
    {
        get
        {
            serverDateTime = TimeUtil.getServerTime();
            return serverDateTime;
        }
    }

    Dictionary<int, List<GameMallConfig>> gameMallDic;
    public Dictionary<int, List<GameMallConfig>> GameMallDic
    {
        get
        {
            if (gameMallDic == null || gameMallDic.Count < 4)
                RefreshGameMallDic();
            return gameMallDic;
        }
        set
        {
            gameMallDic = value;
        }
    }

    public List<GameMallConfig> GetCurrentGameMalls(int selectIndex)
    {
        List<GameMallConfig> currentGameMalls = new List<GameMallConfig>();

        switch (selectIndex)
        {
            case (int)ClickType.CARD:
                currentGameMalls = GameMallDic[(int)MallType.CARD];
                break;
            case (int)ClickType.RECHARGE:
                currentGameMalls = GameMallDic[(int)MallType.RECHARGE];
                break;
            case (int)ClickType.LOVE:
                currentGameMalls = GameMallDic[(int)MallType.LOVE];
                break;
            case (int)ClickType.DIAMOND:
                currentGameMalls = GameMallDic[(int)MallType.DIAMOND];
                break;
            case (int)ClickType.GIFT:
                currentGameMalls = GameMallDic[(int)MallType.GIFT];
                break;
        }
        SortGameMallList(currentGameMalls);
        return currentGameMalls;
    }

    /// <summary>
    /// 商品分类
    /// </summary>
    public void RefreshGameMallDic()
    {
        gameMallDic = new Dictionary<int, List<GameMallConfig>>();

        List<GameMallConfig> gameMallConfigs = JsonConfig.GameMallConfigs;

        int id = 0;
        foreach (var mall in gameMallConfigs)
        {
            id = mall.mall_id / 100000;
            if (!gameMallDic.ContainsKey(id))
                gameMallDic.Add(id, new List<GameMallConfig>());

            if (gameMallDic[id].Contains(mall) || !isViewGameMall(mall))
                continue;
            gameMallDic[id].Add(mall);
        }
    }

    /// <summary>
    /// 排序
    /// </summary>
    public void SortGameMallList(List<GameMallConfig> gameMallConfigs)
    {
        gameMallConfigs.Sort(delegate (GameMallConfig mallA, GameMallConfig mallB)
            {
                if (mallA.rank == mallB.rank)
                    return mallA.mall_id.CompareTo(mallB.mall_id);
                return mallA.rank.CompareTo(mallB.rank);
            });
    }


    /// <summary>
    /// 判断折扣信息
    /// </summary>
    /// <param name="originalprice">原价</param>
    /// <param name="discount">折扣</param>
    /// <param name="discountPrice">折扣价</param>
    /// <returns></returns>
    public bool isHasDiscount(int originalprice, int discount, out int discountPrice)
    {
        if (discount == 100)
        {
            discountPrice = originalprice;
            return false;
        }
        float disCountNum = originalprice * 0.01f * discount;
        discountPrice = (int)disCountNum;
        return true;
    }

    //记录当前角色的信息
    PlayerProp currentPropInfo = new PlayerProp();
    public PlayerProp CurrentPropInfo
    {
        get { return currentPropInfo; }
        set { currentPropInfo = value; }
    }

    /// <summary>
    /// 展示商品
    /// </summary>
    /// <param name="gameMall"></param>
    /// <returns></returns>
    public bool isViewGameMall(GameMallConfig gameMall)
    {
        if (gameMall.mall_id < 100 || gameMall.type == 999)
            return false;
        if (gameMall.has_limit_time != 0 && isOutOfTime(gameMall))
            return false;
        if (gameMall.limit_num != 0 && isOutOfLimitTime(gameMall))
            return false;
        return true;
    }

    /// <summary>
    /// 判断是否在时间范围内
    /// </summary>
    /// <param name="startTime"></param>
    /// <param name="endTime"></param>
    /// <returns></returns>
    public bool isOutOfTime(GameMallConfig gameMall)
    {
        TimeSpan timeSpan = new TimeSpan();
        timeSpan = ServerDateTime - TimeUtil.convertToDateTime(gameMall.limit_start_time);
        if (timeSpan.TotalSeconds < 0)
            return true;
        timeSpan = ServerDateTime - TimeUtil.convertToDateTime(gameMall.limit_end_time);
        if (timeSpan.TotalSeconds >= 0)
            return true;
        return false;
    }

    /// <summary>
    /// 判断是否在限时时间
    /// </summary>
    /// <param name="serverTime"></param>
    /// <param name="gameMall"></param>
    /// <param name="timeSpan"></param>
    /// <returns></returns>
    public bool isLimitTime(GameMallConfig gameMall, out TimeSpan timeSpan)
    {
        timeSpan = new TimeSpan();
        if (gameMall.has_limit_time == 0)
            return false;
        TimeSpan time = ServerDateTime - TimeUtil.convertToDateTime(gameMall.limit_start_time);
        if (time.TotalSeconds < 0)
            return false;
        time = TimeUtil.convertToDateTime(gameMall.limit_end_time) - ServerDateTime;
        if (time.TotalSeconds <= 0)
            return false;
        timeSpan = time;
        return true;
    }

    /// <summary>
    /// 超出限购数量
    /// </summary>
    /// <param name="gameMall"></param>
    /// <returns></returns>
    public bool isOutOfLimitTime(GameMallConfig gameMall)
    {
        int num = GetLimitMallBoughtNum(gameMall.mall_id);
        //Debug.LogError(num+"      "+( num >= gameMall.limit_num));
        return num >= gameMall.limit_num;
    }

    /// <summary>
    /// 限购购买次数
    /// </summary>
    List<PlayerMall> playerMallList;
    public List<PlayerMall> PlayerMallList
    {
        get { return playerMallList; }
        set { playerMallList = value; }
    }

    /// <summary>
    /// 限购商品已购买数量
    /// </summary>
    /// <returns></returns>
    public int GetLimitMallBoughtNum(int mall_id)
    {
        if (playerMallList == null || PlayerMallList.Count <= 0)
            return 0;
        PlayerMall playerMall = PlayerMallList.Where(a => a.mall_id == mall_id).FirstOrDefault();
        return playerMall == null ? 0 : playerMall.buy_times;
    }

    /// <summary>
    /// 购买完毕后刷新信息
    /// </summary>
    /// <param name="propMake"></param>
    public void RefreshQureyMallCallBackInfo(PropMake propMake)
    {
        if (propMake != null)
        {
            if (propMake.playerProp.Count > 0)
                CurrentPropInfo = propMake.playerProp[0];
            if (propMake.playerMall != null && PlayerMallList != null)
            {
                PlayerMall playerMall = PlayerMallList.Where(a => a.mall_id == propMake.playerMall.mall_id).FirstOrDefault();
                if (playerMall != null)
                    PlayerMallList.Remove(playerMall);
                PlayerMallList.Add(propMake.playerMall);

                RefreshGameMallDic();
            }
        }
        if (ShopMallView.view != null)
        {

            ShopMallView.view.RefreshGoodList();
        }

    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

/***

主要用于防成谜管理
*/
public class AntiAddictionMgr
{

    private static AntiAddictionMgr _ins = null;
    public static AntiAddictionMgr Instance
    {
        get
        {
            if (_ins == null)
            {
                _ins = new AntiAddictionMgr();

            }
            return _ins;
        }
    }

    public bool CanBuy(GameMallConfig gameMallConfig)
    {
        string[] cost = gameMallConfig.cost.Split(',');
        if (cost.Length == 3)
        {
            SwitchConfig realName = GameData.SwitchConfigs.Find(a => a.key == SwitchConfig.KEY_REALNAME);
            SwitchConfig recharge = GameData.SwitchConfigs.Find(a => a.key == SwitchConfig.KEY_RECHARGE);
            if (cost[0] == "0" && realName != null && realName.value == "1" &&
            recharge != null && recharge.value == "1" && GameData.User.status == 1)
            {
                return isConsumable(int.Parse(cost[2]));
            }
            else
                return true;
        }
        return true;

    }


    public bool isConsumable(int cost)
    {
        if (GameData.User.age <= 7)
        {
            UIMgr.Ins.showErrorMsgWindow("根据健康系统限制，您暂时无法进行充值/购买行为，感谢您的理解");
            return false;
        }
        else if (GameData.User.age >= 8 && GameData.User.age <= 15 && cost > 50)
        {
            UIMgr.Ins.showErrorMsgWindow("根据健康系统限制，您本次进行的充值/购买金额已达系统上限，请降低金额或选择更低价格商品");
            return false;
        }
        else if (GameData.User.age >= 16 && GameData.User.age <= 17 && cost > 100)
        {
            UIMgr.Ins.showErrorMsgWindow("根据健康系统限制，您本次进行的充值/购买金额已达系统上限，请降低金额或选择更低价格商品");
            return false;
        }
        return true;
    }

}

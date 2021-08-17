using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;
using System.Text;
using FairyGUI;

public static class GameTool
{
    static readonly int maxY = 18;
    static readonly int minY = -38;
    /// <summary>
    /// 好感等级进度条设置
    /// </summary>
    public static void SetLevelProgressBar(GObject gObject, int level)
    {
        Vector3 pos = gObject.position;
        pos.y = minY + (maxY - minY) / GameFavourTitleConfig.MAX_LEVEL * level;
        gObject.SetPosition(pos.x, -pos.y, pos.z);
    }

    /// <summary>
    /// 好感度等级
    /// </summary>
    /// <param name="favor"></param>
    /// <returns></returns>
    public static int FavorLevel(float favor)
    {
        int favorLevel = 0;
        foreach (var config in JsonConfig.GameFavourTitleConfigs)
        {
            if (config.level > favor)
            {
                favorLevel = config.level_id;
                break;
            }
        }
        if (favorLevel == 0)
            favorLevel = GameFavourTitleConfig.MAX_LEVEL;
        return favorLevel;
    }

    /// <summary>
    /// 将超出长度到字符串裁剪
    /// </summary>
    public static string GetCutText(string content, int lenght)
    {
        if (content.Length > lenght)
        {
            string str = null;
            char[] contents = content.ToCharArray();
            //{**}表示图片，避免裁剪

            for (int i = 1; i < 5; i++)
            {
                if (contents[lenght - i] == '{')
                {
                    for (int j = lenght - i + 1; j < content.Length; j++)
                    {
                        if (contents[j] == '}')
                        {
                            str = content.Substring(0, j + 1);
                            break;
                        }
                    }
                    break;
                }
            }

            if (str == null)
                str = content.Substring(0, lenght);
            return str + "...";
        }
        else
            return content;
    }

    /// <summary>
    /// 角色在剧情或手机玩法中升级/好感动提升
    /// </summary>
    public static IEnumerator ShowEffect(UpgradeInfo info)
    {
        PlayerStoryInfoExtra item = info.extra;
        if (!string.IsNullOrEmpty(item.level)&&item.level!="0")
        {
            ShowLevelUpEffect(item.level);
            yield return new WaitForSeconds(5f);
        }
        if (!string.IsNullOrEmpty(item.favour))
        {
            FavorItem favorItem = new FavorItem();
            string[] favor = item.favour.Split(new char[1] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            favorItem.favor = int.Parse(favor[1]) - int.Parse(favor[0]);
            favorItem.oldFavor = FavorLevel(int.Parse(favor[0]));
            favorItem.newFavor = FavorLevel(int.Parse(favor[1]));
            favorItem.actorId = info.actorId;
            UIMgr.Ins.showNextPopupView<FavorView, FavorItem>(favorItem);
            yield return new WaitForSeconds(2f);
            if (favorItem.newFavor - favorItem.oldFavor != 0)
            {
                if (info.gGraph != null)
                    FXMgr.CreateEffectWithGGraph(info.gGraph, new Vector3(-18, -279), "UI_juesebeijingguang", 162, 2);
                UIMgr.Ins.showNextPopupView<FavorLevelUpView, FavorItem>(favorItem);
            }
        }

    }

    /// <summary>
    /// 升级特效
    /// </summary>
    /// <param name="level"></param>
    public static void ShowLevelUpEffect(string level)
    {
        if (string.IsNullOrEmpty(level) || level == "0")
            return;
        string[] levels = level.Split(new char[1] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        if (string.IsNullOrEmpty(level) || levels[0] == levels[1])
        {
            return;
        }
        List<PlayerLevelConfig> playerLevelConfigs = new List<PlayerLevelConfig>();
        GameData.Player.level = int.Parse(levels[1]);
        if (levels.Length > 0)
        {
            PlayerLevelConfig oldLevelConfig = JsonConfig.PlayerLevelConfigs.Find(a => a.level_id == int.Parse(levels[0]));
            PlayerLevelConfig levelConfig = JsonConfig.PlayerLevelConfigs.Find(a => a.level_id == int.Parse(levels[1]));
            playerLevelConfigs.Add(oldLevelConfig);
            playerLevelConfigs.Add(levelConfig);
            UIMgr.Ins.showWindow<LevelUpWindow, List<PlayerLevelConfig>>(playerLevelConfigs);
        }
    }

    static readonly string secretKey = "wdsgame.com";
    /// <summary>
    /// 与或
    /// </summary>
    public static string Encrypt(string content)
    {

        char[] data = content.ToCharArray();
        char[] key = secretKey.ToCharArray();

        for (int i = 0; i < data.Length; i++)
        {
            data[i] ^= key[i % key.Length];
        }

        string str = new string(data);
        return str;
    }
    /// <summary>
    /// 剧情发奖
    /// </summary>
    /// <param name="info"></param>
    public static void GetAwards(AwardInfo info)
    {
        if (!string.IsNullOrEmpty(info.award) && info.award != "0")
        {
            TinyItem tinyItem = ItemUtil.GetTinyItem(info.award);
            switch (tinyItem.type)
            {
                case (int)TypeConfig.Consume.Time:
                    //EventMgr.Ins.DispachEvent(EventConfig.GET_DOLL, tinyItem);
                    UIMgr.Ins.showNextPopupView<GetTimeView, TinyItem>(tinyItem);
                    break;
                case (int)TypeConfig.Consume.EXP:
                case (int)TypeConfig.Consume.Friendly:
                    UpgradeInfo upgradeInfo = new UpgradeInfo();
                    upgradeInfo.actorId = tinyItem.id;
                    upgradeInfo.extra = info.extra;
                    TouchScreenView.Ins.StartCoroutine(ShowEffect(upgradeInfo));
                    SmsSave save = new SmsSave();
                    save.actorId = tinyItem.id;
                    save.extra = info.extra;
                    SMSDataMgr.Ins.RefreshFavor(save);
                    break;
                case (int)TypeConfig.Consume.Item:
                case (int)TypeConfig.Consume.Diamond:
                case (int)TypeConfig.Consume.Star:
                    UIMgr.Ins.showNextPopupView<CommonGetPropsView, TinyItem>(tinyItem);
                    break;
            }
        }
    }

    /// <summary>
    /// 获得角色昵称
    /// </summary>
    public static string GetActorName(int actorId)
    {
        string actorName;
        if (GameData.OwnRoleList == null)
        {
            return "";
        }
        Role actorNick = GameData.OwnRoleList.Find(a => a.id == actorId);
        if (actorNick != null)
            actorName = actorNick.name;
        else
        {
            GameInitCardsConfig gameInitCards = JsonConfig.GameInitCardsConfigs.Find(a => a.card_id == actorId);
            actorName = gameInitCards.name_cn;
        }
        return actorName;
    }

    /// <summary>
    /// 解析服务器返回时间，格式：2020-08-26T11:37:24.62+08:00
    /// </summary>
    public static DateTime GetDateTime(string str)
    {
        if (!string.IsNullOrEmpty(str))
        {
            string time0 = str.Substring(0, 10);
            string[] strs = time0.Split(new char[1] { '-' }, StringSplitOptions.RemoveEmptyEntries);
            string time1 = str.Substring(11, 8);
            string[] times = time1.Split(new char[1] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            DateTime date = new DateTime(int.Parse(strs[0]), int.Parse(strs[1]), int.Parse(strs[2]), int.Parse(times[0]), int.Parse(times[1]), int.Parse(times[2]));
            return date;
        }
        else
        {
            Debug.Log("DateTime string is null!");
            return DateTime.Now;
        }
    }

    /// <summary>
    /// 保存小游戏信息
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="nodeId"></param>
    public static void SaveGameInfo(string key, string value, int nodeId)
    {
        //WWWForm wWWForm = new WWWForm();
        //wWWForm.AddField("nodeId", nodeId);
        //wWWForm.AddField("key", key);
        //wWWForm.AddField("value", value);
        //GameMonoBehaviour.Ins.RequestInfoPost<StoryGameSave>(NetHeaderConfig.STROY_SAVE_GAME, wWWForm, null, false);

        StoryCacheMgr.storyCacheMgr.SaveProgress(key, value, nodeId);


    }

    /// <summary>
    /// 保存新手引导
    /// </summary>
    public static void SaveNewbie(int bigId, int smallId)
    {
        if (bigId < 8)
        {
            //WWWForm wWWForm = new WWWForm();
            //wWWForm.AddField("nodeId", 0);
            //wWWForm.AddField("key", "Newbie");
            //wWWForm.AddField("value", bigId + "," + smallId);
            //GameMonoBehaviour.Ins.RequestInfoPost<StoryGameSave>(NetHeaderConfig.STROY_SAVE_GAME, wWWForm, null, false);
            StoryCacheMgr.storyCacheMgr.SaveProgress("Newbie", bigId + "," + smallId, 0);
        }
        //半封闭
        else
        {
            //WWWForm wWWForm = new WWWForm();
            //wWWForm.AddField("nodeId", bigId);
            //wWWForm.AddField("key", "Newbie");
            //wWWForm.AddField("value", bigId + "," + smallId);
            //GameMonoBehaviour.Ins.RequestInfoPost<StoryGameSave>(NetHeaderConfig.STROY_SAVE_GAME, wWWForm, null, false);
           
            StoryCacheMgr.storyCacheMgr.SaveProgress("Newbie", bigId + "," + smallId, bigId);
        }

    }



    /// <summary>
    /// 屏幕坐标转组件下坐标
    /// </summary>
    public static Vector2 MousePosToUI(Vector2 mousePos, GComponent gCom)
    {
        //先屏幕坐标转fairyGui坐标，然后gRoot坐标转gCom0坐标
        Vector2 pos = GRoot.inst.TransformPoint(GRoot.inst.GlobalToLocal(mousePos), gCom);
        pos.y = 1624 - pos.y;
        return pos;
    }

    /// <summary>
    /// 屏幕坐标转组件下坐标(适用于定位坐标在UI中的位置)
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static Vector2 MousePosToLocalUI(Vector2 mousePos, GComponent gCom)
    {
        Vector2 pos2 = GRoot.inst.GlobalToLocal(Input.mousePosition);
        pos2.y = 1624 - pos2.y;
        Vector2 pos3 = gCom.RootToLocal(pos2, GRoot.inst);
        return pos3;
    }


    public static Vector2 MousePosToRoot(Vector2 mousePos)
    {
        Vector2 pos = GRoot.inst.GlobalToLocal(Input.mousePosition);
        pos.y = Screen.height - pos.y;
        return pos;
    }


    /// <summary>
    /// 根据类型转换成html加载图片
    /// </summary>
    /// <param name="str"></param>
    public static string Conversion(string str)
    {

        MatchCollection matchs = Regex.Matches(str, "({[0-9]+})");
        foreach (Match m in matchs)
        {
            Match match = Regex.Match(m.ToString(), "[0-9]+");
            //Debug.LogError(match.ToString());
            string image = "<img src='" + UrlUtil.GetEmojiUrl(match.ToString()) + "' width='55' height='55'/>";
            str = str.Replace(m.ToString(), image);
        }

        return str;
    }


    /// <summary>
    /// 发奖
    /// </summary>
    /// <param name="taskAward"></param>
    public static void ShowAwards(TaskAward taskAward)
    {
        List<PlayerProp> props = new List<PlayerProp>();
        if (taskAward != null )
        { 
            if (taskAward.playerProp != null)
            {
                foreach (var prop in taskAward.playerProp)
                {
                    PlayerProp playerProp = props.Find(a => a.prop_id == prop.prop_id);
                    if (playerProp == null)
                    {
                        props.Add(prop);
                    }
                    else
                    {
                        playerProp.prop_count += prop.prop_count;
                    }
                }
            }
            if (!string.IsNullOrEmpty(taskAward.level))
            {
                GameTool.ShowLevelUpEffect(taskAward.level);
            }

            if (taskAward.love > GameData.Player.love)
            {
                PlayerProp playerProp = new PlayerProp()
                {
                    prop_id = (int)TypeConfig.Consume.Star,
                    prop_type = (int)TypeConfig.Consume.Star,
                    prop_count = taskAward.love - GameData.Player.love,
                };
                props.Add(playerProp);
                GameData.Player.love = taskAward.love;
            }
            if (taskAward.diamond > GameData.Player.diamond)
            {
                PlayerProp playerProp = new PlayerProp()
                {
                    prop_id = (int)TypeConfig.Consume.Diamond,
                    prop_type = (int)TypeConfig.Consume.Diamond,
                    prop_count = taskAward.diamond - GameData.Player.diamond,
                };
                props.Add(playerProp);
                GameData.Player.diamond = taskAward.diamond;
            };
            if (taskAward.exp > GameData.Player.exp)
            {
                PlayerProp playerProp = new PlayerProp()
                {
                    prop_id = (int)TypeConfig.Consume.EXP,
                    prop_type = (int)TypeConfig.Consume.EXP,
                    prop_count = taskAward.exp - GameData.Player.exp,
                };
                props.Add(playerProp);
                GameData.Player.exp = taskAward.exp;
            };
        }
        
        TouchScreenView.Ins.ShowPropsTost(props);
    }

    /// <summary>
    /// 设置Item动态效果
    /// </summary>
    /// <param name="index"></param>
    /// <param name="obj"></param>
    public static  void SetItemEffectOne(int index, GObject obj,float pageY)
    {
        Vector3 pos = new Vector3();
        pos = obj.position;
        pos.y = index * pageY;
        obj.SetPosition(pos.x, pos.y + 100, pos.z);
        float time = index * 0.2f;
        obj.TweenMoveY(pos.y, (time + 0.3f)).SetEase(EaseType.CubicOut).OnStart(() =>
        {
            obj.TweenFade(1, (time + 0.3f)).SetEase(EaseType.QuadOut);
        });
    }

    public static void SetListEffectOne(GList gList)
    {
        Vector3 pos = new Vector3();
        for (int i = 0; i < gList.numChildren; i++)
        {
            GObject item = gList.GetChildAt(i);
            item.alpha = 0;
            pos = item.position;
            item.SetPosition(pos.x, pos.y + 100, pos.z);
            float time = i * 0.2f;

            item.TweenMoveY(pos.y, (time + 0.3f)).SetEase(EaseType.CubicOut).OnStart(() =>
            {
                item.TweenFade(1, (time + 0.15f)).SetEase(EaseType.QuadOut);
            });
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 主要用于传输数据 获取相关地址
/// </summary>

public class UrlUtil
{

    /// <summary>
    /// 获得背景皮肤
    /// </summary>
    public static string GetSkinBgUrl(int id)
    {
        return "Game/Bg/BgSkin/" + id;
    }

    /// <summary>
    /// 获得通话背景
    /// </summary>
    public static string GetCallBgUrl(string name)
    {
        return "Game/Bg/Call/" + name;
    }

    /// <summary>
    /// 获得娃娃 背景
    /// </summary>
    public static string GetChooseDollBgUrl(string name)
    {
        return "Game/Bg/ChooseDoll/" + name;
    }

    /// <summary>
    /// 获得通用bg
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string GetCommonBgUrl(string name)
    {
        return "Game/Bg/Common/" + name;
    }

    public static string GetRolegrowupBgUrl(string name)
    {
        return "Game/Bg/Rolegrowup/" + name;
    }

    /// <summary>
    /// 获得创建角色背景
    /// </summary>
    public static string GetCreatRoleBgUrl()
    {
        return "Game/Bg/CreatRole/bg";
    }

    /// <summary>
    /// 获得创建角色背景
    /// </summary>
    public static string GetCreateNewbieBgUrl(string name)
    {
        return "Game/Bg/CreatRole/" + name;
    }

    /// <summary>
    /// 获取小游戏背景  Game1
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string GetGameBGUrl(int id)
    {
        return "Game/Bg/Story/DialogBg/" + id;
    }

    public static string GetInteractiveBgUrl(int id)
    {
        return "Game/Bg/Interactive/Interactive" + id;
    }

    /// <summary>
    ///获得登陆背景
    ///</summary>
    public static string GetLoginBgUrl()
    {
        return "Game/Bg/Login/BG_Login";
    }

    /// <summary>
    ///获得加载背景
    ///</summary>
    public static string GetLoadingBgUrl()
    {
        return "Game/Bg/Login/BG_Login2";
    }

    /// <summary>
    /// 获得主界面背景
    /// </summary>
    public static string GetMainBgUrl()
    {
        return "Game/Bg/Main/BG_main";
    }

    public static string GetShopBgUrl(string name)
    {
        return "Game/Bg/Shop/" + name;
    }

    /// <summary>
    /// 获得主界面背景
    /// </summary>
    public static string GetSplashImageUrl(string splashName)
    {
        return "Game/Bg/Splash/" + splashName;
    }

    /// <summary>
    /// 获得房间背景
    /// </summary>
    public static string GetRoomBgUrl(string name)
    {
        return "Game/Bg/Room/" + name;
    }


    /// <summary>
    /// 获得背景
    /// </summary>
    public static string GetBgUrl(string folder, string name)
    {
        return "Game/Bg/" + folder + "/" + name;
    }


    /// <summary>
    /// 获取每个角色的章节图片
    /// </summary>
    /// <returns>The chapter head icon.</returns>
    /// <param name="actorId">Actor identifier.</param>
    /// <param name="chapter">Chapter.</param>
    public static string GetChapterHeadIconUrl(int chapter)
    {
        return "Game/Bg/Story/Chapter/" + chapter;
    }

    /// <summary>
    /// 获取剧情背景图片 背景图片都走这个方法
    /// </summary>
    /// <returns>The story background URL.</returns>
    /// <param name="id">Identifier.</param>
    public static string GetStoryBgUrl(int id)
    {
        return "Game/Bg/Story/DialogBg/" + id;
    }

    /// <summary>
    /// 获取剧情中other文件下面的地址
    /// </summary>
    /// <returns>The story other URL.</returns>
    /// <param name="id">Identifier.</param>
    public static string GetStoryOtherUrl(int id)
    {
        return "Game/Bg/Story/Other/" + id;
    }

    /// <summary>
    /// 获取小游戏合成图
    /// </summary>
    /// <returns>The combine watch URL.</returns>
    /// <param name="name">Name.</param>
    public static string GetCombineWatchUrl(string name)
    {
        return "Game/Combine/Watch/" + name;
    }

    /// <summary>
    /// 获得剧情背景
    /// </summary>
    public static string GetGameWatchUrl(int index)
    {
        return "Game/Combine/Watch/game_watch1_" + index;
    }

    /// <summary>
    /// 获取特效地址
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string GetGFXUrl(string name)
    {
        return "Game/GFX/Prefabs/" + name;
    }

    /// <summary>
    /// 获取特效pine地址
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string GetFxSpineUrl(string name)
    {
        return "Game/GFX/Spine/Moudle/" + name;
    }

    /// <summary>
    /// 获取材质图
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string GetTextureUrl(string name)
    {
        return "Game/GFX/Textures/" + name;
    }

    public static string GetDialogHeadIconUrl(int id)
    {
        return "Game/Headicon/Dialog/" + id;
    }

    /// <summary>
    /// 获取头像
    /// </summary>
    /// <returns>The head icon.</returns>
    /// <param name="id">Identifier.</param>
    public static string GetRoleHeadIconUrl(string id)
    {
        return "Game/Headicon/Role/" + id;
    }

    public static string GetStorySelfHeadIconUrl(int id)
    {
        return "Game/Headicon/Self/" + id;
    }

    /// <summary>
    /// 获取角色故事头像,手机也用
    /// </summary>
    /// <param name="iconId"></param>
    /// <returns></returns>
    public static string GetStoryHeadIconUrl(int iconId)
    {
        return "Game/HeadiconStory/" + iconId;
    }

    /// <summary>
    /// 获取背包内物品(临时使用，后续更改)
    /// </summary>
    /// <returns>The head icon.</returns>
    /// <param name="id">Identifier.</param>
    public static string GetItemIconUrl(int itemId)
    {
        return "Game/Props/" + itemId;
    }
    /// <summary>
    /// 获取各类资源Icon地址
    /// </summary>
    public static string GetPropsIconUrl(TinyItem item)
    {
        switch (item.type)
        {
            case (int)TypeConfig.Consume.AvatarFrame:
            case (int)TypeConfig.Consume.Item:


                switch (item.id / 1000)
                {
                    case (int)TypeConfig.Consume.Title:
                        GameTitleConfig titleConfigA = JsonConfig.GameTitleConfigs.Find(a => a.id == item.id);
                        return GetTitleIconUrl(titleConfigA.level);
                    case (int)TypeConfig.Consume.Time:
                        return "Game/Props/" + (int)TypeConfig.Consume.Time;
                    default:
                        return "Game/Props/" + item.id;
                }
            case (int)TypeConfig.Consume.Friendly:
                return "ui://T_Common/icon_haogandu";
            case (int)TypeConfig.Consume.Title:
                GameTitleConfig titleConfig = JsonConfig.GameTitleConfigs.Find(a => a.id == item.id);
                return GetTitleIconUrl(titleConfig.level);

            default:
                return "Game/Props/" + item.type;

        }
    }
    /// <summary>
    /// 称号icon地址
    /// </summary>
    public static string GetTitleIconUrl(int level)
    {
        int id = 21;
        switch (level)
        {
            case 2:
                id = 22;
                break;
            case 3:
                id = 23;
                break;
        }

        return "Game/Props/" + id;
    }

    /// <summary>
    /// 获取角色时装、背景icon地址
    /// </summary>
    /// <param name="itemId"></param>
    /// <returns></returns>
    public static string GetActorIconUrl(int itemId)
    {
        return "Game/Props/SkinIcon/" + itemId;
    }

    /// <summary>
    /// 垃圾地址
    /// </summary>
    public static string GetRubbishUrl(int id)
    {
        return "Game/Rubbish/" + id;
    }

    /// <summary>
    /// 垃圾地址
    /// </summary>
    public static string GetFindImageUrl(string id, int i)
    {
        return "Game/FindDifferenceImage/zc_" + id + "_" + i;
    }

    /// <summary>
    /// 获取娃娃皮肤Icon地址
    /// </summary>
    /// <param name="cardId"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string GetDollSkinIconUrl(int cardId, int skinId)
    {
        return "Game/Role/Character/" + cardId + "/" + skinId;
    }


    /// <summary>
    /// 角色名片
    /// </summary>
    /// <param name="tinyItemId"></param>
    /// <returns></returns>
    public static string GeTinyItemUrl(int tinyItemId)
    {
        return "Game/Role/Feature/" + tinyItemId;
    }

    /// <summary>
    /// 获取时刻id
    /// </summary>
    /// <param name="momentId"></param>
    /// <returns></returns>
    public static string GetGameMomentUrl(int momentId)
    {
        return "Game/Role/Time/" + momentId;
    }


    /// <summary>
    /// 获取Spine地址
    /// </summary>
    /// <returns>The spine URL.</returns>
    /// <param name="id">Identifier.</param>
    public static string GetSpineUrl(string name)
    {
        return "Game/Spine/Moudle/" + name;
    }

    /// <summary>
    /// 时刻地址
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static string GetTimeUrl(int id)
    {
        return "Game/Time/" + id;
    }

    /// <summary>
    /// 时刻获得图片地址
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static string GetTimeGainUrl(int id)
    {
        return "Game/Time_gain/" + id;
    }


    public static string GetMessageBg(string name)
    {
        return "Game/Bg/Message/" + name;
    }
    /// <summary>
    /// 短信或朋友圈的图
    /// </summary>
    public static string GetSmsImageURL(int id)
    {
        return "Game/Bg/Sms/" + id;
    }

    /// <summary>
    /// 时刻缩略图
    /// </summary>
    public static string GetMomentTime(int id)
    {
        return "Game/Bg/Room/Time/" + id;
    }

    /// <summary>
    /// 选择角色时刻
    /// </summary>
    public static string GetMomentTimeRole(int id)
    {
        return "Game/Bg/Room/Role/" + id;
    }

    /// <summary>
    /// 称号url
    /// </summary>
    public static string GetTitleBg(int id)
    {
        return "Game/title/" + id;
    }

    /// <summary>
    /// 称号url
    /// </summary>
    public static string GetAvatarFrame(int id)
    {
        return "Game/headFrame/" + id;
    }

    /// <summary>
    /// 制作doll icon
    /// </summary>
    public static string GetDollIcon(int id)
    {
        return "Game/Doll/" + id;
    }
    /// <summary>
    /// 制作doll part
    /// </summary>
    public static string GetDollPart(int id)
    {
        return "Game/Doll/Make/" + id;
    }

    /// <summary>
    /// 表情包URL
    /// </summary>
    public static string GetEmojiUrl(string id)
    {
        return "Game/Look/" + id;
    }

    /// <summary>
    /// 查看大图URL
    /// </summary>
    public static string GetLookImageUrl(string id)
    {
        return "Game/Bg/Story/Game/" + id;
    }

    /// <summary>
    /// 新手语音URL
    /// </summary>
    public static string GetNewbieBgmUrl(string id)
    {
        return "Audio/Newbie/" + id;
    }


    /// <summary> 
    /// 
    /// </summary> 
    public static string GetVideoUrl(string videoId)
    {
        return "Video/Prefabs/" + videoId;
    }

    /// <summary> 
    /// 
    /// </summary> 
    public static string GetGameDetailUrl(int id)
    {
        return "Game/Bg/Story/Gameinstructions/" + id;
    }
}

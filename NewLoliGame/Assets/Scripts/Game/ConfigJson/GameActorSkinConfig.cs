
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// ���
/// </summary>
[Serializable]
public class GameActorSkinConfig
{
    /// <summary>
    /// 仙子默认背景
    /// </summary>
    public static readonly int FAIRY_BACKGROUND_TYPE = 5;
    /// <summary>
    /// 背景
    /// </summary>
    public static readonly int BACKGROUND_TYPE = 4;
    /// <summary>
    /// 皮肤
    /// </summary>
    public static readonly int SKINS_TYPE = 3;
    /// <summary>
    /// 人类默认背景
    /// </summary>
    public static readonly int HUMAN_BACKGROUND_TYPE = 2;
    /// <summary>
    /// 默认皮肤
    /// </summary>
    public static readonly int DEFAULT_SKINS_TYPE = 1;
    public int id;
    public int actor_id;
    public int type;
    public string name_cn;
    public string price;
    public double favour_extra;
    public string fragment;
    public int status;
}

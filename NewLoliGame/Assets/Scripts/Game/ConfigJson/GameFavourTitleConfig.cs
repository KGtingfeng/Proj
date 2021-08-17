
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
///  
/// </summary>
[Serializable]
public class GameFavourTitleConfig
{
    public static readonly int MAX_LEVEL = 6;
    /// <summary>
    /// 
    /// </summary>
    public int level_id;
	
	/// <summary>
	/// 中文名字
	/// </summary>
	public string name_cn;
	
	/// <summary>
	/// 图片ID
	/// </summary>
	public string logo;
	
	/// <summary>
	/// 等级
	/// </summary>
	public int level;
	
}

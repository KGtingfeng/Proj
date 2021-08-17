
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
///  任务配置
/// </summary>
[Serializable]
public class GameTaskConfig
{ 
	
	/// <summary>
	/// 任务ID
	/// </summary>
	public int task_id;
	
	/// <summary>
	/// 任务类型
	/// </summary>
	public int task_type;
	
	/// <summary>
	/// 对应角色ID
	/// </summary>
	public int actor_id;
	
	/// <summary>
	/// 任务需要的道具ID
	/// </summary>
	public string task_condition;
	
	/// <summary>
	/// 任务获得
	/// </summary>
	public string task_award;
	
	/// <summary>
	/// 任务描述
	/// </summary>
	public string task_description;
	
}

using System;

[Serializable]
public class GameCellVoiceBackgroundConfig
{
   //1、默认 2、时刻图 
    public int type;
    public int actor_id;
    public string name;
    public string assets;
    public string limit;
    
    public TinyItem tinyItem {
        get { return ItemUtil.GetTinyItem(limit); }
    }
}

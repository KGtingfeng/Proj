using System;

[Serializable]
public class GameDetailConfig  
{
    public int id;
    public string title;
    public string content;

    public Action callback;
}

using System;

[Serializable]
public class GameTitleConfig
{
    public int id;
    public string name_cn;
    public string logo;
    public bool reg;
    public bool status;
    public int need_lvl;
    public int charm;
    public int evn;
    public int intell;
    public int mana;
    public int level;


    public TinyItem GetAttr()
    {
        TinyItem item = new TinyItem();
        if (charm > 0)
        {
            item.num = charm;
            item.url = CommonUrlConfig.GetCharmUrl();
        }
        else if (evn > 0)
        {
            item.num = evn;
            item.url = CommonUrlConfig.GetEnvUrl();
        }
        else if (intell > 0)
        {
            item.num = intell;
            item.url = CommonUrlConfig.GetWisdomUrl();
        }
        else if (mana > 0)
        {
            item.num = mana;
            item.url = CommonUrlConfig.GetMagicUrl();
        }
        return item;
    }
}

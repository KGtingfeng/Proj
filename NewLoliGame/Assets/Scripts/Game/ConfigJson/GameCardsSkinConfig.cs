using System;
using UnityEngine;

[Serializable]
public class GameCardsSkinConfig
{
    public int id;
    //娃娃id
    public int card_id;
    
    public int type;
    public string name_cn;
    public string price;
    public bool status;
    public int charm;
    public int evn;
    public int intell;
    public int mana;

    public void print()
    {
        Debug.Log(" [ id=" + id + " cardid=" + card_id + " type=" + type + " name_cn=" + name_cn +
            " price=" + price + " status=" + status +  " evn=" + evn +  " mana=" + mana  + " ]");

    }
}

using System;
using UnityEngine;


[Serializable]
public class GameCardLevelConfig
{
    public int card_id;
    public int level;
    public int charm_cff;
    public int evn_cff;
    public int intell_cff;
    public int mana_cff;
    public string consume;



    public void print()
    {
        Debug.Log(" [ cardid=" + card_id + "  level=" + level + " charm_cff=" + charm_cff + " evn_cff=" + evn_cff + 
        " intell_cff=" + intell_cff + " mana_cff=" + mana_cff + " consume=" + consume +" ]");

    }

}




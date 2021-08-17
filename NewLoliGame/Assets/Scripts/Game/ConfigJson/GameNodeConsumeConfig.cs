using System;

[Serializable]
public class GameNodeConsumeConfig
{
    public int node_id;
    public int type;
    public string pay;

    public TinyItem Pay
    {
        get
        {
            return ItemUtil.GetTinyItem(pay);
        }
    }
}

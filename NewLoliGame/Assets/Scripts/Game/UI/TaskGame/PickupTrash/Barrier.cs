using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour
{

    BoxCollider2D boxCollider;
    public GameXinglingDlllConfig config;
    public void Init(GameXinglingDlllConfig dlllConfig)
    {
        config = dlllConfig;
        boxCollider = gameObject.AddComponent<BoxCollider2D>();
        boxCollider.isTrigger = true;
        switch (dlllConfig.ckey)
        {
            case "1":
                boxCollider.size = new Vector2(45, 40);
                boxCollider.offset = new Vector2(23, -20);
                break;
            case "2":
                boxCollider.size = new Vector2(54, 50);
                boxCollider.offset = new Vector2(36, -33);
                break;
            case "3":
                boxCollider.size = new Vector2(78, 70);
                boxCollider.offset = new Vector2(50, -40);
                break;

        }

    }

}

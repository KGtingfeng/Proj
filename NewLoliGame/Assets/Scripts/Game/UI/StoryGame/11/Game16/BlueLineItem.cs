using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueLineItem : MonoBehaviour
{
    BoxCollider2D box;

    private void Awake()
    {
        box = gameObject.AddComponent<BoxCollider2D>();
        Rigidbody2D rigid2D = gameObject.AddComponent<Rigidbody2D>();
        rigid2D.gravityScale = 0;

        box.isTrigger = true;
    }
    public void Init(bool isVertical, int type = 0)
    {
        if (isVertical)
        {
            box.size = new Vector2(10, 500);
            box.offset = new Vector2(18, -400);

        }
        else
        {
            box.size = new Vector2(10, 500);
            box.offset = new Vector2(18, -400);

        }
    }
}

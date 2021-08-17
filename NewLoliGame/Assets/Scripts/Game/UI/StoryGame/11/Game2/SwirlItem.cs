using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
public class SwirlItem : MonoBehaviour
{
    GObject go;
    void Start()
    {
        CircleCollider2D box = gameObject.AddComponent<CircleCollider2D>();
        Rigidbody2D rigid2D = gameObject.AddComponent<Rigidbody2D>();
        rigid2D.gravityScale = 0;
        rigid2D.mass = 10;
        box.radius = 150;
        box.isTrigger = true;
        box.offset = new Vector2(30, 0);
    }

    public void Init(GObject gComponent)
    {
        go = gComponent;
        Move();
    }

    void Move()
    {
        go.TweenMoveY(2000, 4f).SetEase(EaseType.Linear).OnComplete(() =>
        {
            go.y = -200;
            GameJumpShipView.instance.RemoveSwirlToPool(go);
        });
    }
}

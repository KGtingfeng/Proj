using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

public class StoneItem : MonoBehaviour
{
    GObject go;
    void Start()
    {
        CircleCollider2D box = gameObject.AddComponent<CircleCollider2D>();
        Rigidbody2D rigid2D = gameObject.AddComponent<Rigidbody2D>();
        rigid2D.gravityScale = 0;
        box.radius = 65;
        box.offset = new Vector2(0, 0);
    }

    public void Init(GObject gComponent, int random)
    {
        go = gComponent;
        Move(random);
    }

    void Move(int random)
    {
        go.TweenMoveY(2000 + random * 40, 4f).SetEase(EaseType.Linear).OnComplete(() =>
        {
            go.y = -200;
            GameJumpShipView.instance.RemoveToPool(go);
        });
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleItem : MonoBehaviour
{
    void Start()
    {
        CircleCollider2D box = gameObject.AddComponent<CircleCollider2D>();
        Rigidbody2D rigid2D = gameObject.AddComponent<Rigidbody2D>();
        rigid2D.gravityScale = 0;
        rigid2D.mass = 0.1f;
        box.isTrigger = true;
        box.radius = 70;
        box.offset = new Vector2(100, -90);

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameAviodView.instance.GameFail();

    }



}

using UnityEngine;
using FairyGUI;

public class ShipItem : MonoBehaviour
{
    public PolygonCollider2D box;
    Rigidbody2D rigid2D;
    void Start()
    {
        box = gameObject.AddComponent<PolygonCollider2D>();
        rigid2D = gameObject.AddComponent<Rigidbody2D>();
        rigid2D.gravityScale = 0;
        rigid2D.mass = 0.1f;

        Vector2[] points = new Vector2[]
        {
            new Vector2(-0.4373f,114.4576f),
            new Vector2(-22.7666f,47.43665f),
            new Vector2(-25.02055f,-34.89233f),
            new Vector2(-1.1160f,-91.00562f),
            new Vector2(30.6404f,-51.2087f),
            new Vector2(24.82422f,38.04199f),
        };
        box.points = points;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameJumpShipView.instance.OnCollisionStone();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        GameJumpShipView.instance.OnCollisionSwirl();
    }
}

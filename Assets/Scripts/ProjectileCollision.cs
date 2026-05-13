using UnityEngine;

public class ProjectileCollision : MonoBehaviour
{
    public ShooterController shooter;
    public float moveSpeed = 12f;

    private Rigidbody2D rb;
    private bool hasAttached = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (hasAttached)
            return;

        if (rb == null)
            return;

        if (rb.bodyType != RigidbodyType2D.Dynamic)
            return;

        if (rb.velocity.sqrMagnitude > 0.01f)
        {
            rb.velocity = rb.velocity.normalized * moveSpeed;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasAttached)
            return;

        Bubble hitBubble = collision.collider.GetComponent<Bubble>();
        bool hitCeiling = collision.collider.CompareTag("Ceiling");

        /*
         * Nếu chạm tường trái/phải:
         * Không gắn bóng.
         * Không gọi GameManager.
         * Chỉ giữ tốc độ để bóng bật tiếp.
         */
        if (hitBubble == null && !hitCeiling)
        {
            KeepSpeed();
            return;
        }

        hasAttached = true;

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        ContactPoint2D contact = collision.GetContact(0);
        Bubble bubble = GetComponent<Bubble>();

        if (shooter != null)
        {
            shooter.OnProjectileHit(bubble, contact.point);
        }

        Destroy(this);
    }

    private void KeepSpeed()
    {
        if (rb == null)
            return;

        if (rb.velocity.sqrMagnitude <= 0.01f)
            return;

        rb.velocity = rb.velocity.normalized * moveSpeed;
        rb.WakeUp();
    }
}
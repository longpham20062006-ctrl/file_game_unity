using UnityEngine;

public class Bubble : MonoBehaviour
{
    public int colorId;

    public int row = -1;
    public int col = -1;

    public bool isOnGrid = false;

    public void SetGridPosition(int newRow, int newCol)
    {
        row = newRow;
        col = newCol;
        isOnGrid = true;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.bodyType = RigidbodyType2D.Static;
            rb.gravityScale = 0f;
        }
    }

    public void MakeProjectile()
    {
        isOnGrid = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
            rb.drag = 0f;
            rb.angularDrag = 0f;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.WakeUp();
        }
    }

    public void Drop()
    {
        isOnGrid = false;
        row = -1;
        col = -1;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 1.8f;
            rb.freezeRotation = false;
            rb.velocity = new Vector2(Random.Range(-1.2f, 1.2f), -2.5f);
            rb.angularVelocity = Random.Range(-180f, 180f);
        }

        Destroy(gameObject, 3f);
    }
}
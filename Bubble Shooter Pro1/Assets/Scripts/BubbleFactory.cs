using UnityEngine;

public class BubbleFactory : MonoBehaviour
{
    [Header("Bubble Settings")]
    public int spriteSize = 128;
    public float bubbleRadius = 0.32f;

    [Header("Bubble Colors")]
    public Color[] bubbleColors =
    {
        Color.red,
        Color.blue,
        Color.green,
        Color.yellow,
        new Color(0.7f, 0.2f, 1f),
        new Color(1f, 0.45f, 0f)
    };

    private Sprite bubbleSprite;

    private void Awake()
    {
        bubbleSprite = CreateCircleSprite(spriteSize);
    }

    public GameObject CreateBubble(int colorId, Vector3 position)
    {
        GameObject bubble = new GameObject("Bubble");

        bubble.transform.position = position;
        bubble.transform.localScale = Vector3.one * bubbleRadius * 2f;

        SpriteRenderer sr = bubble.AddComponent<SpriteRenderer>();
        sr.sprite = bubbleSprite;
        sr.color = bubbleColors[colorId % bubbleColors.Length];
        sr.sortingOrder = 5;

        CircleCollider2D col = bubble.AddComponent<CircleCollider2D>();
        col.radius = 0.5f;

    Rigidbody2D rb = bubble.AddComponent<Rigidbody2D>();
rb.gravityScale = 0f;
rb.freezeRotation = true;
rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
rb.interpolation = RigidbodyInterpolation2D.Interpolate;
rb.sleepMode = RigidbodySleepMode2D.NeverSleep;
rb.drag = 0f;
rb.angularDrag = 0f;

        Bubble bubbleScript = bubble.AddComponent<Bubble>();
        bubbleScript.colorId = colorId;

        return bubble;
    }

    private Sprite CreateCircleSprite(int size)
    {
        Texture2D tex = new Texture2D(size, size);
        tex.filterMode = FilterMode.Bilinear;

        Vector2 center = new Vector2(size / 2f, size / 2f);
        float radius = size * 0.48f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);

                if (distance <= radius)
                {
                    tex.SetPixel(x, y, Color.white);
                }
                else
                {
                    tex.SetPixel(x, y, Color.clear);
                }
            }
        }

        tex.Apply();

        return Sprite.Create(
            tex,
            new Rect(0, 0, size, size),
            new Vector2(0.5f, 0.5f),
            size
        );
    }
}
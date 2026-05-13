using UnityEngine;

public class ShooterController : MonoBehaviour
{
    [Header("References")]
    public BubbleFactory bubbleFactory;
    public BubbleGrid bubbleGrid;
    public Transform firePoint;

    [Header("Shoot Settings")]
    public float shootSpeed = 12f;
    public float minAngle = 12f;
    public float maxAngle = 168f;

    private GameObject currentBubble;
    private Vector2 aimDirection = Vector2.up;
    private bool canShoot = true;

    private void Start()
    {
        CreateNewBubble();
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
            return;

        Aim();

        if (Input.GetMouseButtonUp(0) && canShoot)
        {
            Shoot();
        }

        if (Input.touchCount > 0 && canShoot)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Ended)
            {
                Shoot();
            }
        }
    }

    private void Aim()
    {
        if (firePoint == null)
            return;

        Vector3 inputPosition;

        if (Input.touchCount > 0)
        {
            inputPosition = Input.GetTouch(0).position;
        }
        else
        {
            inputPosition = Input.mousePosition;
        }

        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(
            new Vector3(inputPosition.x, inputPosition.y, 10f)
        );

        Vector2 direction = worldPosition - firePoint.position;

        if (direction.y <= 0)
            return;

        direction.Normalize();

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        angle = Mathf.Clamp(angle, minAngle, maxAngle);

        aimDirection = new Vector2(
            Mathf.Cos(angle * Mathf.Deg2Rad),
            Mathf.Sin(angle * Mathf.Deg2Rad)
        ).normalized;

        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
    }

    private void Shoot()
    {
        if (currentBubble == null)
            return;

        canShoot = false;

        Bubble bubble = currentBubble.GetComponent<Bubble>();
        bubble.MakeProjectile();

        currentBubble.transform.position = firePoint.position;

        Collider2D col = currentBubble.GetComponent<Collider2D>();
        if (col != null)
            col.enabled = true;

        Rigidbody2D rb = currentBubble.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 0f;
            rb.drag = 0f;
            rb.angularDrag = 0f;
            rb.freezeRotation = true;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.velocity = aimDirection * shootSpeed;
            rb.WakeUp();
        }

        ProjectileCollision projectile = currentBubble.AddComponent<ProjectileCollision>();
        projectile.shooter = this;
        projectile.moveSpeed = shootSpeed;

        currentBubble = null;
    }

    private void CreateNewBubble()
    {
        if (bubbleFactory == null || firePoint == null)
        {
            Debug.LogError("ShooterController thiếu BubbleFactory hoặc FirePoint.");
            return;
        }

        int colorId = Random.Range(0, 4);

        currentBubble = bubbleFactory.CreateBubble(colorId, firePoint.position);

        Bubble bubble = currentBubble.GetComponent<Bubble>();
        if (bubble != null)
            bubble.isOnGrid = false;

        Rigidbody2D rb = currentBubble.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.bodyType = RigidbodyType2D.Static;

        Collider2D col = currentBubble.GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;
    }

    public void OnProjectileHit(Bubble bubble, Vector3 hitPosition)
    {
        if (bubble == null || bubbleGrid == null)
            return;

        bubbleGrid.AttachBubble(bubble, hitPosition);

        int destroyedCount = bubbleGrid.LastDestroyedCount;
        int remainingCount = bubbleGrid.TotalBubbleCount;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnAfterBubbleAttached(destroyedCount, remainingCount);
        }

        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
            return;

        canShoot = true;
        CreateNewBubble();
    }
}
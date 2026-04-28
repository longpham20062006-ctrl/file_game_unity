using UnityEngine;

public class ObstacleMover : MonoBehaviour
{
    public float destroyX = -12f;

    private void Update()
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.isGameOver) return;

        transform.Translate(Vector3.left * GameManager.Instance.obstacleSpeed * Time.deltaTime);

        if (transform.position.x < destroyX)
        {
            Destroy(gameObject);
        }
    }
}
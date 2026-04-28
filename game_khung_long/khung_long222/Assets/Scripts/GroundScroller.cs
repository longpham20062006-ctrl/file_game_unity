using UnityEngine;

public class GroundScroller : MonoBehaviour
{
    public float tileWidth = 20f;

    private void Update()
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.isGameOver) return;

        transform.Translate(Vector3.left * GameManager.Instance.obstacleSpeed * Time.deltaTime);

        if (transform.position.x <= -tileWidth)
        {
            transform.position += Vector3.right * tileWidth * 2f;
        }
    }
}
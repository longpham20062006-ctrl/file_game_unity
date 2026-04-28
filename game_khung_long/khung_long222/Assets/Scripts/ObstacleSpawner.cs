using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject[] obstaclePrefabs;
    public Transform spawnPoint;

    public float minSpawnTime = 1.2f;
    public float maxSpawnTime = 2.2f;

    private float timer;
    private float nextSpawnTime;

    private void Start()
    {
        SetNextSpawnTime();
    }

    private void Update()
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.isGameOver) return;

        timer += Time.deltaTime;

        if (timer >= nextSpawnTime)
        {
            SpawnObstacle();
            timer = 0f;
            SetNextSpawnTime();
        }
    }

    private void SpawnObstacle()
    {
        if (obstaclePrefabs == null || obstaclePrefabs.Length == 0) return;
        if (spawnPoint == null) return;

        int randomIndex = Random.Range(0, obstaclePrefabs.Length);
        GameObject selectedPrefab = obstaclePrefabs[randomIndex];

        Instantiate(selectedPrefab, spawnPoint.position, Quaternion.identity);
    }

    private void SetNextSpawnTime()
    {
        float speed = 6f;

        if (GameManager.Instance != null)
        {
            speed = GameManager.Instance.obstacleSpeed;
        }

        float speedFactor = Mathf.InverseLerp(6f, 12f, speed);

        float currentMin = Mathf.Lerp(minSpawnTime, 0.8f, speedFactor);
        float currentMax = Mathf.Lerp(maxSpawnTime, 1.5f, speedFactor);

        nextSpawnTime = Random.Range(currentMin, currentMax);
    }
}
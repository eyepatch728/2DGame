using UnityEngine;

public class TheAlpsMountainIceBlockSpawner : MonoBehaviour
{
    public GameObject iceBlockPrefab; // Prefab of the ice block
    public Transform[] spawnPoints;   // Three spawn points for the lanes
    public float spawnInterval = 1f;  // Interval at which ice blocks spawn

    private float spawnTimer = 0f;

    void Update()
    {
        if (TheAlpsMountainManager.Instance.IsGameActive())
        {
            spawnTimer += Time.deltaTime;

            if (spawnTimer >= spawnInterval)
            {
                SpawnIceBlock();
                spawnTimer = 0f;
            }
        }
    }

    void SpawnIceBlock()
    {
        int randomIndex = Random.Range(0, spawnPoints.Length);
        Instantiate(iceBlockPrefab, spawnPoints[randomIndex].position, Quaternion.identity);
    }
}

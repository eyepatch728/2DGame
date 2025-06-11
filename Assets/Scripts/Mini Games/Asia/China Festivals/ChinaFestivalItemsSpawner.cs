using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChinaFestivalItemsSpawner : MonoBehaviour
{
    public GameObject itemPrefab;
    public GameObject itemPrefabForIpad;
    public float spawnRate = 2f;
    public int itemsPerSpawn = 3; // Number of items to spawn in one cycle
    public float spawnDelay = 0.2f; // Delay between each item spawn
    [SerializeField] float minX = -8f;
    [SerializeField] float maxX = 8f;
    public float spawnY = 6f;

    private float nextSpawnTime;
    private Queue<ChinaFestivalsManager.FestivalItem> itemQueue =
        new Queue<ChinaFestivalsManager.FestivalItem>();
    private List<ChinaFestivalsManager.FestivalItem> activeItems =
        new List<ChinaFestivalsManager.FestivalItem>();

    private void Start()
    {
        nextSpawnTime = Time.time + spawnRate;
        RefillQueue();
    }

    private void RefillQueue()
    {
        var uncollectedItems = new List<ChinaFestivalsManager.FestivalItem>();

        foreach (var activeItem in activeItems)
        {
            if (!activeItem.isCollected)
            {
                uncollectedItems.Add(activeItem);
            }
        }

        foreach (var item in ChinaFestivalsManager.Instance.festivalItems)
        {
            if (!uncollectedItems.Contains(item))
            {
                uncollectedItems.Add(item); // NEW: Allow all items to be added
            }
        }

        activeItems.Clear();

        for (int i = uncollectedItems.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            var temp = uncollectedItems[i];
            uncollectedItems[i] = uncollectedItems[randomIndex];
            uncollectedItems[randomIndex] = temp;
        }

        foreach (var item in uncollectedItems)
        {
            itemQueue.Enqueue(item);
        }
    }

    private void Update()
    {
        if (DeviceResForELiffelTower.Instance.isIpad)
        {
            maxX = -5f;
            minX = 5f;
        }
        else if (DeviceResForELiffelTower.Instance.isIphone)
        {
            maxX = -8f;
            minX = 8f;
        }
        if (ChinaFestivalsManager.Instance.isGameStart())
        {
            if (ChinaFestivalsManager.Instance.isGamePaused) return;

            if (Time.time >= nextSpawnTime)
            {
                // If queue is empty, refill it
                if (itemQueue.Count == 0)
                {
                    RefillQueue();
                }

                // Only spawn if we have items in the queue
                if (itemQueue.Count > 0)
                {
                    StartCoroutine(SpawnItemsWithDelay(itemsPerSpawn)); // Spawn items with a delay
                    nextSpawnTime = Time.time + spawnRate;
                }
            }

            // Clean up activeItems list by removing destroyed objects
            activeItems.RemoveAll(item => item == null);
            ChinaFestivalsManager.Instance.CheckGameComplete();
        }

    }

    private IEnumerator SpawnItemsWithDelay(int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (itemQueue.Count > 0)
            {
                Vector2 spawnPosition = new Vector2(Random.Range(minX, maxX), spawnY);
                GameObject newItem = Instantiate(itemPrefab, spawnPosition, Quaternion.identity);

                ChinaFestivalsManager.FestivalItem itemToSpawn = itemQueue.Dequeue();
                ChinaFestivalFallingItems fallingItem = newItem.GetComponent<ChinaFestivalFallingItems>();
                fallingItem.SetItem(itemToSpawn);

                // Add to active items
                activeItems.Add(itemToSpawn);

                // Wait for the delay before spawning the next item
                yield return new WaitForSeconds(spawnDelay);
            }
        }
    }
}
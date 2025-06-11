using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FestivalItemsSpawner : MonoBehaviour
{
    public GameObject itemPrefab;
    public float spawnRate = 2f;
    public int itemsPerSpawn = 3; // Number of items to spawn in one cycle
    public float spawnDelay = 0.2f; // Delay between each item spawn
    public float minX;
    public float maxX;
    public float spawnY = 6f;

    private float nextSpawnTime;
    private Queue<BrazillianFestivalManager.FestivalItem> itemQueue =
        new Queue<BrazillianFestivalManager.FestivalItem>();
    private List<BrazillianFestivalManager.FestivalItem> activeItems =
        new List<BrazillianFestivalManager.FestivalItem>();
    public float aspectRatio;
    public bool isIpad;
    public bool isIphone;
    private void Start()
    {
        if (aspectRatio >= 16f / 9f || Screen.width < 768)
        {
            isIpad = false;
            isIphone = true;
        }
        else
        {
            isIpad = true;
            isIphone = false;
        }

        if (isIpad)
        {
            //Debug.Log("Detected an iPad resolution.");
            minX = -4f;
            maxX = 4f;
        }
        else if (isIphone)
        {
            //Debug.Log("Detected an iPhone resolution.");
            minX = -8f;
            maxX = 8f;
        }
        else
        {
            //Debug.Log("Unknown device resolution.");
        }
        nextSpawnTime = Time.time + spawnRate;
        RefillQueue();
    }
    private void RefillQueue()
    {
        var uncollectedItems = new List<BrazillianFestivalManager.FestivalItem>();

        foreach (var activeItem in activeItems)
        {
            if (!activeItem.isCollected)
            {
                uncollectedItems.Add(activeItem);
            }
        }

        foreach (var item in BrazillianFestivalManager.Instance.festivalItems)
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
        if (BrazillianFestivalManager.Instance.isGameStart())
        {
            if (BrazillianFestivalManager.Instance.isGamePaused) return;

            if (Time.time >= nextSpawnTime)
            {
                // If queue is empty, refill it
                if (itemQueue.Count == 0)
                {
                    RefillQueue();
                }

                // Only spawn if we have items in queue
                if (itemQueue.Count > 0)
                {
                    StartCoroutine(SpawnItemsWithDelay(itemsPerSpawn)); // Spawn items with a delay
                                                                        //SpawnItem();
                    nextSpawnTime = Time.time + spawnRate;
                }
            }

            // Clean up activeItems list by removing destroyed objects
            activeItems.RemoveAll(item => item == null);
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

                BrazillianFestivalManager.FestivalItem itemToSpawn = itemQueue.Dequeue();
                FallingItem fallingItem = newItem.GetComponent<FallingItem>();
                fallingItem.SetItem(itemToSpawn);

                // Add to active items
                activeItems.Add(itemToSpawn);

                // Wait for the delay before spawning the next item
                yield return new WaitForSeconds(spawnDelay);
            }
        }
    }
    //private void SpawnItem()
    //{
    //    if (itemQueue.Count > 0)
    //    {
    //        Vector2 spawnPosition = new Vector2(Random.Range(minX, maxX), spawnY);
    //        GameObject newItem = Instantiate(itemPrefab, spawnPosition, Quaternion.identity);
    //        //newItem.transform.localScale = itemScale;

    //        BrazillianFestivalManager.FestivalItem itemToSpawn = itemQueue.Dequeue();
    //        FallingItem fallingItem = newItem.GetComponent<FallingItem>();
    //        fallingItem.SetItem(itemToSpawn);

    //        // Add to active items
    //        activeItems.Add(itemToSpawn);
    //    }
    //}
}

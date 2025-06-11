using System.Collections.Generic;
using UnityEngine;

public class TheAlpsFlagManager : MonoBehaviour
{
    [System.Serializable]
    public class FlagData
    {
        public GameObject flagPrefab; // Prefab of the flag
        public AudioClip voiceClip;   // Audio clip for the flag
        public string countryName;    // Name of the country (optional)
    }

    public FlagData[] flags;          // List of flags
    public Transform[] spawnPoints;  // Three lanes for spawning flags
    public float spawnInterval = 1f; // Time between spawns
    private float spawnTimer = 0f;
    public AudioSource audioSource;  // AudioSource to play voice clips

    private List<GameObject> activeFlags = new List<GameObject>();
    public List<FlagData> collectedFlags = new List<FlagData>();
    public GameObject finishLine;    // Reference to the finish line
    public float finishLineSpeed = 2f; // Speed at which the finish line moves

    private bool isFinishLineActive = false; // To track if the finish line is active

    private void Start()
    {
        if (finishLine != null)
        {
            finishLine.SetActive(false); // Hide the finish line initially
        }
    }

    private void Update()
    {
        if (TheAlpsMountainManager.Instance.IsGameActive())
        {
            spawnTimer += Time.deltaTime;

            if (spawnTimer >= spawnInterval)
            {
                SpawnFlag();
                spawnTimer = 0f;
            }

            // Move the finish line if active
            if (isFinishLineActive && finishLine != null)
            {
                finishLine.transform.position += Vector3.down * finishLineSpeed * Time.deltaTime;
            }
        }
    }

    private void SpawnFlag()
    {
        if (collectedFlags.Count >= flags.Length)
        {
            ActivateFinishLine();
            return;
        }

        FlagData flagToSpawn = null;
        int maxAttempts = 10;
        int attempts = 0;
        int laneToUse = -1;

        while (flagToSpawn == null && attempts < maxAttempts)
        {
            int randomFlagIndex = Random.Range(0, flags.Length);
            int randomLaneIndex = Random.Range(0, spawnPoints.Length);

            if (!collectedFlags.Contains(flags[randomFlagIndex]))
            {
                flagToSpawn = flags[randomFlagIndex];
                laneToUse = randomLaneIndex;
            }
            attempts++;
        }

        if (flagToSpawn == null || laneToUse == -1) return;

        // Spawn the flag
        Vector3 spawnPosition = spawnPoints[laneToUse].position;

        // Adjust position if it overlaps with existing objects
        spawnPosition = AdjustPositionIfOverlapping(spawnPosition);

        GameObject spawnedFlag = Instantiate(flagToSpawn.flagPrefab, spawnPosition, Quaternion.identity);
        spawnedFlag.SetActive(true);

        activeFlags.Add(spawnedFlag);

        TheAlpsFlagBehavior flagBehavior = spawnedFlag.AddComponent<TheAlpsFlagBehavior>();
        flagBehavior.flagManager = this;
        flagBehavior.flagData = flagToSpawn;
        flagBehavior.slideSpeed = 2f;
    }

    private Vector3 AdjustPositionIfOverlapping(Vector3 position)
    {
        float yOffset = 1f; // Minimum distance to avoid overlap
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, 0.5f);

        while (colliders.Length > 0)
        {
            position.y += yOffset; // Move upward to avoid overlap
            colliders = Physics2D.OverlapCircleAll(position, 0.5f);
        }

        return position;
    }


    public void FlagCollected(GameObject flag, FlagData flagData)
    {
        // Play the voice clip
        if (flagData.voiceClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(flagData.voiceClip);
        }

        // Mark the flag as collected
        collectedFlags.Add(flagData);

        // Remove the flag from the active list
        activeFlags.Remove(flag);

        // Destroy the flag game object
        Destroy(flag);

        // Check if all flags are collected
        if (collectedFlags.Count >= flags.Length)
        {
            ActivateFinishLine();
        }
    }

    private void ActivateFinishLine()
    {
        if (finishLine != null && !isFinishLineActive)
        {
            finishLine.SetActive(true); // Show the finish line
            isFinishLineActive = true;
        }
    }

    public void RespawnFlag(FlagData flagData)
    {
        if (collectedFlags.Contains(flagData))
        {
            return; // Skip this flag
        }

        int maxAttempts = 10;
        int attempts = 0;
        Vector3 spawnPosition = Vector3.zero;
        bool positionFound = false;

        while (!positionFound && attempts < maxAttempts)
        {
            int randomLaneIndex = Random.Range(0, spawnPoints.Length);
            spawnPosition = spawnPoints[randomLaneIndex].position;

            // Check for overlap and adjust position if necessary
            spawnPosition = AdjustPositionIfOverlapping(spawnPosition);

            // If no overlap is detected, we consider the position valid
            positionFound = (Physics2D.OverlapCircleAll(spawnPosition, 0.5f).Length == 0);
            attempts++;
        }

        if (!positionFound)
        {
            Debug.LogWarning("Failed to find a suitable position for respawning the flag.");
            return;
        }

        // Spawn the flag
        GameObject spawnedFlag = Instantiate(flagData.flagPrefab, spawnPosition, Quaternion.identity);
        spawnedFlag.SetActive(true);

        // Add the flag to the active list
        activeFlags.Add(spawnedFlag);

        // Attach a flag script to handle collection
        TheAlpsFlagBehavior flagBehavior = spawnedFlag.AddComponent<TheAlpsFlagBehavior>();
        flagBehavior.flagManager = this;
        flagBehavior.flagData = flagData;
    }

}

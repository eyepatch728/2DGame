using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BigBenManager : MonoBehaviour
{
    public static BigBenManager Instance;
    [Header("Scene References")]
    public TowerOfPisaRopeMovement rope; // Reference to the rope already in scene
    public TowerOfPisaCameraFollow cameraFollow;
    [Header("Prefabs")]
    public GameObject floorPrefab7;
    public GameObject floorPrefab6;
    public GameObject floorPrefab5;
    public GameObject floorPrefab4;
    public GameObject floorPrefab3;
    public GameObject floorPrefab2;
    public GameObject floorPrefab1;
    public GameObject floorPrefab0;

    [Header("UI References")]
    public TMP_Text floorCountText;
    public GameObject gameOverPanel;
    public GameObject winPanel;

    public int currentFloor { get; private set; } = 0;
    public float retryDelay = 1.5f;
    private int totalFloors = 8;
    private GameObject currentBlock;
    private bool isGameActive = true;
    private Vector3 lastPlacedPosition;
    private bool canPlace = true;  // Flag to prevent rapid placement
    private void Awake()
    {
        Instance = this;
        if (rope == null)
        {
            Debug.LogError("Rope reference not set in GameManager!");
            return;
        }
    }
    private void Start()
    {
        StartGame();
    }
    private void Update()
    {
        if (!isGameActive || !canPlace) return;

        if (Input.GetMouseButtonDown(0))
        {
            PlaceBlock();
        }

        // Update current block's position to follow rope's end position
        if (currentBlock != null)
        {
            currentBlock.transform.position = rope.GetEndPosition();
        }
    }
    private void StartGame()
    {
        currentFloor = 0;
        lastPlacedPosition = Vector3.zero;
        isGameActive = true;
        canPlace = true;

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);
        rope.UpdateRopeHeight(0);
        SpawnNextBlock();
        UpdateUI();
    }


    private void SpawnNextBlock()
    {
        if (currentFloor >= totalFloors)
        {
            WinGame();
            return;
        }

        Vector3 spawnPosition = rope.GetEndPosition();

        GameObject prefabToSpawn;
        switch (currentFloor)
        {
            case 0:
                prefabToSpawn = floorPrefab7;
                currentBlock = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
                break; 
            case 1:
                prefabToSpawn = floorPrefab6;
                currentBlock = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
                break;
            case 2:
                prefabToSpawn = floorPrefab5;
                currentBlock = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
                break;
            case 3:
                prefabToSpawn = floorPrefab4;
                currentBlock = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
                break;
            case 4:
                prefabToSpawn = floorPrefab3;
                currentBlock = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
                break;
            case 5:
                prefabToSpawn = floorPrefab2;
                currentBlock = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
                break;
            case 6:
                prefabToSpawn = floorPrefab1;
                currentBlock = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
                break;
            case 7:
                prefabToSpawn = floorPrefab0;
                currentBlock = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
                break; 
        }

    }




    public void PlaceBlock()
    {
        if (!isGameActive || currentBlock == null || !canPlace) return;

        canPlace = false;  // Prevent rapid placement
        TowerOfPisaFloor block = currentBlock.GetComponent<TowerOfPisaFloor>();

        // Detach from the rope
        currentBlock.transform.SetParent(null);

        // Check if placement is successful
        block.TryPlace(lastPlacedPosition);
        bool successfulPlacement = true;
        if (successfulPlacement)
        {
            lastPlacedPosition = currentBlock.transform.position;
            currentFloor++;
            UpdateUI();
            rope.UpdateRopeHeight(currentFloor);
            // Move the camera up after placing each block
            if (cameraFollow != null)
            {
                cameraFollow.IncrementYOffset();
            }
            StartCoroutine(SpawnNextBlockAfterDelay(0.5f));  // Short delay for successful placement
        }
        else
        {
            StartCoroutine(HandleUnsuccessfulPlacement());
        }
    }

    private IEnumerator SpawnNextBlockAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (currentFloor < totalFloors)
        {
            SpawnNextBlock();
            canPlace = true;  // Re-enable placement
        }
    }

    private IEnumerator HandleUnsuccessfulPlacement()
    {
        yield return new WaitForSeconds(retryDelay);

        // Only spawn new block if game is still active
        if (isGameActive)
        {
            SpawnNextBlock();
            canPlace = true;  // Re-enable placement
        }
    }
    private void UpdateUI()
    {
        if (floorCountText != null)
            floorCountText.text = $"Floor: {currentFloor}/{totalFloors}";
    }

    private void GameOver()
    {
        isGameActive = false;
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    private void WinGame()
    {
        isGameActive = false;
        if (winPanel != null)
            winPanel.SetActive(true);
    }
    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

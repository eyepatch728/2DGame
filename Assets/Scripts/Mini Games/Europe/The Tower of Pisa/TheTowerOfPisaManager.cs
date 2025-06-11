using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class TheTowerOfPisaManager : MonoBehaviour
{
    public static TheTowerOfPisaManager Instance { get; private set; }

    [Header("Scene References")]
    public TowerOfPisaRopeMovement rope; // Reference to the rope already in scene
    public TowerOfPisaCameraFollow cameraFollow;
    [Header("Prefabs")]
    //public GameObject baseFloorPrefab;
    //public GameObject middleFloorPrefab;
    //public GameObject topFloorPrefab;
    public GameObject[] FloorToSpawnPrefab;
    int flootToSpawnIndex = 0;
    [Header("UI References")]
    public TMP_Text floorCountText;
    //public GameObject gameOverPanel;
    //public GameObject winPanel;

    public int currentFloor = 0;
    public float retryDelay = 0.5f;
    private int totalFloors = 8;
    private GameObject currentBlock;
    private Vector3 lastPlacedPosition;
    public bool isGameWon = false;
    private bool canPlace = true;  // Flag to prevent rapid placement
    public GameObject towerParent;
    public bool isBigBen;


    public bool gameActive = false;
    //[Header("Cat Animation")]
    //public Animator catAnimator; // Animator for the cat
    //public string isTalkingBool = "isTalking";
    //public AudioSource audioSource;
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
        gameActive = false;
    }
    public void StartGame()
    {
        currentFloor = 0;
        lastPlacedPosition = Vector3.zero;
        gameActive = true;
        canPlace = true;

        //if (gameOverPanel != null) gameOverPanel.SetActive(false);
        //if (winPanel != null) winPanel.SetActive(false);
        rope.UpdateRopeHeight(0);
        SpawnNextBlock();
        UpdateUI();
    }
    public void EndGame()
    {
        gameActive = false;
        TowerOfPisaPopups.Instance.ShowEndGamePopup();
    }
    public bool isGameStart()
    {
        return gameActive;
    }

    public float nextPlacementTimer = 0;
    public bool hasBlock = true;
    private void Update()
    {
        if (isGameWon && !isBigBen)
        {
            // Define the target rotation (e.g., rotate around the Z-axis by -10 degrees)
            Quaternion targetRotation = Quaternion.Euler(0, 0, -10f);

            // Smoothly interpolate between the current rotation and the target rotation
            float rotationSpeed = 1f; // Adjust this value to control the speed of rotation
            towerParent.transform.rotation = Quaternion.Slerp(towerParent.transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            return;
        }
        nextPlacementTimer -= Time.deltaTime;
        if (!isGameStart() || !canPlace) return;
        if(nextPlacementTimer <= 0)
        {
            nextPlacementTimer = 0;
        }
        if (Input.GetMouseButtonDown(0) && nextPlacementTimer <= 0 && hasBlock)
        {

            SoundManager.instance.PlaySingleSound(5);
            nextPlacementTimer = 2.4f;
            PlaceBlock();
        }

        // Update current block's position to follow rope's end position
        if (currentBlock != null)
        {
            currentBlock.transform.position = rope.GetEndPosition();
        }
        

    }

    public void BoxSuccessfull()
    {
        currentFloor++;
        UpdateUI();
        rope.UpdateRopeHeight(currentFloor);
        StartCoroutine(SpawnNextBlockAfterDelay(0.1f));
        cameraFollow.IncrementYOffset();
        if (currentFloor == totalFloors)
        {
            //isGameWon = true;

            SoundManager.instance.PlaySingleSound(4);
            WinGame();
        }
       currentBlock.transform.SetParent(towerParent.transform, true);

    }
    SpriteRenderer spriteRenderer;
    int layerNumber =20;
    private void SpawnNextBlock()
    {
        if (currentFloor >= totalFloors)
        {
            WinGame();
            //return;
        }
        hasBlock = true;
        Vector3 spawnPosition = rope.GetEndPosition();

        GameObject prefabToSpawn;
        if (!isBigBen)
        {
            if (currentFloor == 0)
                flootToSpawnIndex = 0;
            else if (currentFloor == totalFloors - 1)
                flootToSpawnIndex = 2;
            else
                flootToSpawnIndex = 1;
        }
        else
        {
            flootToSpawnIndex = currentFloor;
        }
       
        currentBlock = Instantiate(FloorToSpawnPrefab[flootToSpawnIndex], spawnPosition, Quaternion.identity);
        spriteRenderer = currentBlock.GetComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = layerNumber;
        layerNumber--;
    }

    public void PlaceBlock()
    {
        if (!isGameStart() || currentBlock == null || !canPlace) return;
        canPlace = false;  // Prevent rapid placement
        hasBlock = false;
        TowerOfPisaFloor block = currentBlock.GetComponent<TowerOfPisaFloor>();
        block.GetComponent<BoxCollider2D>().enabled = true;
        // Detach from the rope
        currentBlock.transform.SetParent(null);
       
        // Check if placement is successful
        bool successfulPlacement = block.TryPlace(lastPlacedPosition);

        //if (successfulPlacement)
        {
            lastPlacedPosition = currentBlock.transform.position;
            UpdateUI();
            rope.UpdateRopeHeight(currentFloor);
            // Move the camera up after placing each block
            //if (cameraFollow != null)
            //{
            //}
            //StartCoroutine(SpawnNextBlockAfterDelay(3f));  // Short delay for successful placement
        }
        //else
        {
            //StartCoroutine(HandleUnsuccessfulPlacement());
        }
    }

    public IEnumerator SpawnNextBlockAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (currentFloor < totalFloors)
        {
            SpawnNextBlock();
            canPlace = true;  // Re-enable placement
        }
    }

    public IEnumerator HandleUnsuccessfulPlacement()
    {
        yield return new WaitForSeconds(retryDelay);
        print("Handledd");
        // Only spawn new block if game is still active
        if (isGameStart())
        {
            SpawnNextBlock();
            canPlace = true;  // Re-enable placement
        }
    }

    public void UpdateUI()
    {
        if (floorCountText != null)
            //floorCountText.text = $"Floor: {currentFloor}/{totalFloors}";
            floorCountText.text =$"{currentFloor}";
    }

    private void WinGame()
    {
        //isGameActive = false;
        isGameWon = true;

        Invoke(nameof(EndGame) , 1.5f);
        //if (winPanel != null)
        //    winPanel.SetActive(true);
    }

    public void backToMainmenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

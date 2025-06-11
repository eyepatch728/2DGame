using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GrandCanyonManager : MonoBehaviour
{
    public static GrandCanyonManager Instance;
    public Collider2D Player;
    public Renderer playerRenderer;
    public Animator PlayerAnimator;
    //public int Health = 3;
    public float MaxInvicibilityTimer = 1.0f;
    public float RunSpeed = 5.0f;
    public float JumpSpeed = 10.0f;
    public float HitSpeed = -10.0f;
    public float HitImpulse = 10.0f;
    public float RunRecoveryTimeFromHit = 0.2f;
    public float RunRecoverTimeFromJump = 1.0f;
    public float JumpImpulse = 100.0f;
    public float Gravity = 9.8f;

    public float ObstacleMinFrequency = 0.5f;
    public float ObstacleMaxFrequency = 1.0f;
    public float CactusMinFrequency = 3.0f;
    public float CactusMaxFrequency = 5.0f;

    public GameObject[] Obstacles;
    public GameObject[] Cacti;
    public Transform[] Grounds;
    public float MinGroundX = -2.0f;
    public Transform[] Backgrounds;
    public float MinBackgroundX = -30.0f;
    public float BackgroundParallax = 0.5f;
    public TMP_Text CactiCountText; // Assign this in the Unity Inspector

    //public GameObject WinPanel;

    public int TotalCactiToWin = 10; // Number of cacti to collect to win
    private int collectedCacti = 0;  // Track collected cacti

    float obstacleNextDistance;
    float cactusNextDistance;

    List<Collider2D> SpawnedObstacles = new List<Collider2D>();
    List<Collider2D> SpawnedCacti = new List<Collider2D>();

    bool isJumping = false;
    Vector3 velocity = Vector3.zero;
    float scrollSpeed;
    bool jumpHold = false;
    float invicibilityTimer;
    bool isPaused = false;
    float distance = 0f;

    const float FLOOR_HEIGHT = -2.69f;
    const float SPAWN_X = 12.0f;
    const float KILL_X = -12.0f;
    public bool isGameComplete = false; // Initialize as false
    public bool gameActive = false;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        gameActive = false;
        PlayerAnimator.enabled = false;
        UpdateCactiCountText();

    }
    public void StartGame()
    {
        gameActive = true;
        PlayerAnimator.enabled = true;

        obstacleNextDistance = Random.Range(ObstacleMinFrequency, ObstacleMaxFrequency);
        cactusNextDistance = Random.Range(CactusMinFrequency, CactusMaxFrequency);
        scrollSpeed = RunSpeed;

    }
    public void EndGame()
    {
        gameActive = false;
        GrandCanyonPopups.instance.ShowEndGamePopup();
    }
    public bool isGameStart()
    {
        return gameActive;
    }

    void Update()
    {
        if (isGameStart())
        {
            if (isGameComplete)
            {
                //PlayerAnimator.Play("Idle");
                // Stop all game activities
                return;
            }
            if (isPaused)
                return;

            if (distance > obstacleNextDistance)
            {
                SpawnObstacle();
                obstacleNextDistance = distance + Random.Range(ObstacleMinFrequency, ObstacleMaxFrequency);
            }

            if (distance > cactusNextDistance)
            {
                SpawnCactus();
                cactusNextDistance = distance + Random.Range(CactusMinFrequency, CactusMaxFrequency);
            }

            UpdatePlayer();

            List<Collider2D> deleteObjects = new List<Collider2D>();
            foreach (var obstacle in SpawnedObstacles)
            {
                obstacle.transform.position += Vector3.left * (scrollSpeed * Time.deltaTime);
                if (obstacle.transform.position.x < KILL_X)
                {
                    deleteObjects.Add(obstacle);
                    Destroy(obstacle.gameObject);
                }
            }

            List<Collider2D> deleteCacti = new List<Collider2D>();
            foreach (var cactus in SpawnedCacti)
            {
                cactus.transform.position += Vector3.left * (scrollSpeed * Time.deltaTime);
                if (cactus.transform.position.x < KILL_X)
                {
                    deleteCacti.Add(cactus);
                    Destroy(cactus.gameObject);
                }
            }

            UpdateGroundAndBackground();

            distance += scrollSpeed * Time.deltaTime;

            CheckCollisions(deleteObjects, deleteCacti);

            foreach (var del in deleteObjects)
                SpawnedObstacles.Remove(del);
            foreach (var del in deleteCacti)
                SpawnedCacti.Remove(del);

            UpdateScrollSpeed();

            UpdateAnimations();
        }
        
    }

    void SpawnObstacle()
    {
        GameObject go = Instantiate(Obstacles[Random.Range(0, Obstacles.Length)]);
        go.SetActive(true);
        go.transform.position = new Vector3(SPAWN_X, FLOOR_HEIGHT, 0f);
        SpawnedObstacles.Add(go.GetComponent<Collider2D>());
    }

    void SpawnCactus()
    {
        float spawnY = FLOOR_HEIGHT + 5.5f;
        GameObject go = Instantiate(Cacti[Random.Range(0, Cacti.Length)]);
        go.SetActive(true);
        go.transform.position = new Vector3(SPAWN_X, spawnY, 0f);
        SpawnedCacti.Add(go.GetComponent<Collider2D>());
    }

    void UpdatePlayer()
    {
        Player.transform.position += velocity * Time.deltaTime;
        velocity.y -= Gravity * Time.deltaTime;

        if (Player.transform.position.y < FLOOR_HEIGHT)
        {
            velocity.y = 0f;
            isJumping = false;
        }

        if (Input.touchCount > 0)
        {
            SoundManager.instance.PlaySingleSound(7);
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                jumpHold = true;
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(0).phase == TouchPhase.Canceled)
            {
                jumpHold = false;
            }
        }

        if (jumpHold && !isJumping)
        {
            velocity = Vector3.up * JumpImpulse;
            isJumping = true;
            scrollSpeed = JumpSpeed;
            jumpHold = false;
        }
    }

    void UpdateGroundAndBackground()
    {
        foreach (var ground in Grounds)
        {
            ground.position += Vector3.left * (scrollSpeed * Time.deltaTime);
            if (ground.position.x < MinGroundX)
                ground.position += Vector3.right * (ground.GetComponent<SpriteRenderer>().size.x * ground.localScale.x * 2.0f);
        }

        foreach (var background in Backgrounds)
        {
            background.position += Vector3.left * (scrollSpeed * Time.deltaTime * BackgroundParallax);
            if (background.position.x < MinBackgroundX)
                background.position += Vector3.right * (27.8f * 2.0f);
        }
    }

    void CheckCollisions(List<Collider2D> deleteObjects, List<Collider2D> deleteCacti)
    {
        if (invicibilityTimer > 0)
        {
            invicibilityTimer -= Time.deltaTime;
        }
        else
        {
            foreach (var obstacle in SpawnedObstacles)
            {
                if (Player.IsTouching(obstacle))
                {
                    invicibilityTimer = MaxInvicibilityTimer;
                    //Health--;
                    scrollSpeed = HitSpeed;
                    velocity = Vector3.up * HitImpulse;
                    obstacle.enabled = false;

                    SpriteRenderer sr = Player.GetComponentInChildren<SpriteRenderer>();
                    sr.DOColor(new Color(1.0f, 0f, 0f, 0.5f), MaxInvicibilityTimer).SetEase(Ease.Flash, 12.0f);
                    break;
                }
            }
        }

        foreach (var cactus in SpawnedCacti)
        {
            if (Player.IsTouching(cactus))
            {

                SoundManager.instance.PlaySingleSound(3);
                collectedCacti++;
                deleteCacti.Add(cactus);
                Destroy(cactus.gameObject);
                UpdateCactiCountText();
                if (collectedCacti >= TotalCactiToWin)
                {
                    PlayerAnimator.Play("Idle");
                    Invoke(nameof(Delay), 1f);
                    
                }
            }
        }
    }
    void Delay()
    {
        isGameComplete = true;
        // Call a method to handle game completion
        ShowWinUI();
        return;
    }
    void UpdateCactiCountText()
    {
        CactiCountText.text = $"{collectedCacti}/{TotalCactiToWin}";
    }
    void UpdateScrollSpeed()
    {
        if (invicibilityTimer > 0)
            scrollSpeed = Mathf.MoveTowards(scrollSpeed, RunSpeed, Time.deltaTime / RunRecoveryTimeFromHit);
        else
            scrollSpeed = Mathf.MoveTowards(scrollSpeed, RunSpeed, Time.deltaTime / RunRecoverTimeFromJump);
    }

    void UpdateAnimations()
    {
        if (isJumping)
        {
            if (velocity.magnitude < 10.0f)
            {
                PlayerAnimator.Play("InAir");
            }
            else if (velocity.y > 0)
            {
                PlayerAnimator.Play("JumpUp");
            }
            else
            {
                PlayerAnimator.Play("JumpDown");
            }
        }
        else
        {
            if (invicibilityTimer > 0 && !isHitAnimPlaying)
            {
                StartCoroutine(PlayHitAnimation());
                //PlayerAnimator.Play("Hit");
            }
            if (invicibilityTimer <= 0)
                PlayerAnimator.Play("Run");
        }
    }

    bool isHitAnimPlaying = false;
    IEnumerator PlayHitAnimation()
    {
        isHitAnimPlaying = true;
        playerRenderer.material.color = Color.red;
        yield return new WaitForSeconds(0.4f);
        // Flash effect with color changing
        for (int i = 0; i < 2; i++)
        {
            playerRenderer.material.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            playerRenderer.material.color = Color.white;
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(0.2f);
        isHitAnimPlaying = false;
    }

    void ShowWinUI()
    {
        Debug.Log("Number of Cactis Collected.................");
        EndGame();
        //WinPanel.SetActive(true);
        // Additional win logic can be added here
    }
    public void BackToMainMenu()
    {
        SoundManager.instance.PlaySingleSound(1);
        SceneManager.LoadSceneAsync("MainMenu");
    }
}

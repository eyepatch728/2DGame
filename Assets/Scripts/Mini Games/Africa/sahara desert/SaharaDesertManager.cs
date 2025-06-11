using DG.Tweening;
using System.Collections.Generic;
// using UnityEditor.ShaderGraph.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaharaDesertManager : MonoBehaviour
{
    public static SaharaDesertManager Instance;
    public Collider2D Player;
    public Animator PlayerAnimator;
    public Image ProgressBar;
    public GameObject Oasis;

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

    public GameObject[] Obstacles;
    public Transform[] Grounds;
    public float MinGroundX = -2.0f;
    public Transform[] Backgrounds;
    public float MinBackgroundX = -30.0f;
    public float BackgroundParallax = 0.5f;

    float obstacleNextDistance;
    List<Collider2D> SpawnedObstacles = new List<Collider2D>();

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
    public bool isGameComplete = false;

    // Distance at which the oasis becomes visible
    public float OasisAppearDistance = 950.0f;
    // Distance at which the game is considered complete
    public float OasisReachDistance = 1000.0f;

    // Multiplier to control the speed of the progress fill amount
    public float ProgressSpeedMultiplier = 1.0f;

    private SpriteRenderer oasisSpriteRenderer;
    public bool gameActive = false;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        gameActive = false;
        PlayerAnimator.enabled = false;
    }
    public void StartGame()
    {
        gameActive = true;
        PlayerAnimator.enabled = true;

        obstacleNextDistance = Random.Range(ObstacleMinFrequency, ObstacleMaxFrequency);
        scrollSpeed = RunSpeed;
        ProgressBar.fillAmount = 0;

        if (Oasis != null)
        {
            oasisSpriteRenderer = Oasis.GetComponent<SpriteRenderer>();
            SetOasisAlpha(0f); // Start fully transparent
            Oasis.SetActive(false); // Initially hidden
        }

    }
    public void EndGame()
    {
        gameActive = false;
        SaharaDesertPopups.instance.ShowEndGamePopup();
    }
    public bool isGameStart()
    {
        return gameActive;
    }

    void Update()
    {
        if (isGameStart()) {
            if (isGameComplete)
            {
                PlayerAnimator.Play("Idle");
                return;
            }
            if (isPaused)
                return;

            if (distance > obstacleNextDistance)
            {
                SpawnObstacle();
                obstacleNextDistance = distance + Random.Range(ObstacleMinFrequency, ObstacleMaxFrequency);
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

            distance += scrollSpeed * Time.deltaTime;
            CheckCollisions(deleteObjects);
            UpdateProgressBar();

            foreach (var del in deleteObjects)
                SpawnedObstacles.Remove(del);

            UpdateGroundAndBackground();
            UpdateScrollSpeed();
            UpdateAnimations();
        }
        
    }

    void CheckCollisions(List<Collider2D> deleteObjects)
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
                    scrollSpeed = HitSpeed;
                    velocity = Vector3.up * HitImpulse;
                    obstacle.enabled = false;

                    // Visual feedback for hit
                    SpriteRenderer sr = Player.GetComponentInChildren<SpriteRenderer>();
                    sr.DOColor(new Color(1.0f, 0f, 0f, 0.5f), MaxInvicibilityTimer).SetEase(Ease.Flash, 12.0f);

                    // You may add further hit effects or logic here, such as reducing health, etc.
                    break;
                }
            }
        }
    }

    void SpawnObstacle()
    {
        if (distance >= OasisAppearDistance - 50.0f) // Stop spawning obstacles when near the oasis
        {
            return;
        }

        GameObject go = Instantiate(Obstacles[Random.Range(0, Obstacles.Length)]);
        go.SetActive(true);
        go.transform.position = new Vector3(SPAWN_X, FLOOR_HEIGHT, 0f);
        SpawnedObstacles.Add(go.GetComponent<Collider2D>());
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

    void UpdateProgressBar()
    {
        float progress = (distance / OasisReachDistance) * ProgressSpeedMultiplier;
        ProgressBar.fillAmount = Mathf.Clamp01(progress);

        if (progress >= 1.0f && !isGameComplete)
        {
            isGameComplete = true;
            ShowOasis();
        }
        else if (distance >= OasisAppearDistance && !Oasis.activeSelf)
        {
            EnableOasis();
        }

        if (Oasis.activeSelf && !isGameComplete)
        {
            // Calculate the desired position for the oasis based on progress
            float progressToOasis = (distance - OasisAppearDistance) / (OasisReachDistance - OasisAppearDistance);
            float desiredDistance = Mathf.Lerp(10.0f, 5.0f, progressToOasis); // Gradually move closer
            float targetX = Player.transform.position.x + desiredDistance;

            // Smoothly move the oasis to maintain its relative position
            float currentX = Oasis.transform.position.x;
            float newX = Mathf.Lerp(currentX, targetX, Time.deltaTime * 2.0f);
            Oasis.transform.position = new Vector3(newX, FLOOR_HEIGHT, 0f);

            // Update alpha based on progress
            float alphaProgress = Mathf.Clamp01((distance - OasisAppearDistance) / (OasisReachDistance - OasisAppearDistance));
            SetOasisAlpha(alphaProgress);
        }
    }




    void EnableOasis()
    {
        Oasis.SetActive(true);
        // Position the oasis further ahead but not too far
        float oasisDistance = 10.0f; // Reduced from 15.0f to keep oasis closer
        Oasis.transform.position = new Vector3(Player.transform.position.x + oasisDistance, FLOOR_HEIGHT, 0f);
        Debug.Log("Oasis is now visible!");
    }

    void ShowOasis()
    {
        Debug.Log("You've reached the oasis!");

        // Since the oasis is already in a good position, we just need to ensure it stays there
        // Remove the DOTween movement as it's no longer needed
        PlayerAnimator.Play("Idle");
        // Destroy any remaining obstacles
        foreach (var obstacle in SpawnedObstacles)
        {
            Destroy(obstacle.gameObject);
        }
        SpawnedObstacles.Clear();

        EndGame();
    }


    void SetOasisAlpha(float alpha)
    {
        if (oasisSpriteRenderer != null)
        {
            Color color = oasisSpriteRenderer.color;
            color.a = alpha;
            oasisSpriteRenderer.color = color;
        }
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
            if (invicibilityTimer > 0)
                PlayerAnimator.Play("Hit");
            else
                PlayerAnimator.Play("Run");
        }
    }

    public void BackToMainMenu()
    {
        SoundManager.instance.PlaySingleSound(1);
        SceneManager.LoadSceneAsync("MainMenu");
    }
}

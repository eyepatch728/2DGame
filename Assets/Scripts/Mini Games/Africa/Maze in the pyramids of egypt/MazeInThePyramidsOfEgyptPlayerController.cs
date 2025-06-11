using UnityEngine;

public class MazeInThePyramidsOfEgyptPlayerController : MonoBehaviour
{
    public static MazeInThePyramidsOfEgyptPlayerController instance;
    public float moveSpeed = 5f; // Speed of the player
    private Vector2 movement;   // Stores player movement direction
    private Vector2 lastMovement; // Stores the last movement direction
    private Rigidbody2D rb;
    public bool isMoving = false;
    private Animator animator; // Reference to Animator component
    private bool isUsingUIButtons = false; // Track if UI buttons are being used
    SpriteRenderer spriteRenderer;

    private void Awake()
    {
        instance = this;
        spriteRenderer = this.GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0; // Disable gravity for top-down movement
        animator = GetComponent<Animator>(); // Get Animator component
    }

    void Update()
    {
        // Only process keyboard input if UI buttons are not being used
        if (!isUsingUIButtons)
        {
            // Get keyboard input
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");

            // Normalize movement for consistent speed when moving diagonally
            if (movement.magnitude > 1)
                movement.Normalize();

            // Check if the player is moving
            isMoving = movement != Vector2.zero;

            // Update animations
            animator.SetBool("isMoving", isMoving);
            if (isMoving)
            {
                UpdatePlayerDirection(movement);
                lastMovement = movement; // Store the last movement direction
            }
            else
            {
                ResetDirectionAnimations();
            }
        }
    }

    void FixedUpdate()
    {
        if (isMoving)
        {
            // Move the player based on input
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        }
    }

    // Updates animation based on movement direction
    private void SetDirectionAnimations(Vector2 moveDirection)
    {
        animator.SetBool("isMovingLeft", moveDirection.x < 0);
        animator.SetBool("isMovingRight", moveDirection.x > 0);
        animator.SetBool("isMovingUp", moveDirection.y > 0);
        animator.SetBool("isMovingDown", moveDirection.y < 0);
    }

    // Resets all directional movement animations
    private void ResetDirectionAnimations()
    {
        animator.SetBool("isMovingLeft", false);
        animator.SetBool("isMovingRight", false);
        animator.SetBool("isMovingUp", false);
        animator.SetBool("isMovingDown", false);
    }

    void UpdatePlayerDirection(Vector2 moveDirection)
    {
        SetDirectionAnimations(moveDirection);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("collectable-bottle"))
        {
            SoundManager.instance.PlaySingleSound(0);
            print("Collections!!");
            MazeInThePyramidsOfEgyptManager.Instance.ShowFact();
            MazeInThePyramidsOfEgyptManager.Instance.collectedBottles++;
            Destroy(collision.gameObject);
        }
        else if (collision.CompareTag("level-gate"))
        {
            MazeInThePyramidsOfEgyptManager.Instance.CompleteLevel();
        }
    }

    // Movement functions for UI buttons
    public void MoveUp() => SetMoveTarget(Vector2.up);
    public void MoveDown() => SetMoveTarget(Vector2.down);
    public void MoveLeft() => SetMoveTarget(Vector2.left);
    public void MoveRight() => SetMoveTarget(Vector2.right);

    private void SetMoveTarget(Vector2 direction)
    {
        isStopped = false;
        isUsingUIButtons = true; // Disable keyboard input
        movement = direction;
        lastMovement = direction; // Store the last movement direction
        isMoving = true;
        animator.SetBool("isMoving", true);
        UpdatePlayerDirection(direction);
    }

    bool isStopped = false;
    // Call this method when UI button is released
    public void StopUIMovement()
    {
        if (isStopped)
        {
            return;
        }
        isStopped = true;
        isUsingUIButtons = false;
        isMoving = false;
        //print("Last Movement: " + lastMovement);
        //SetLastDirectionIdleSprite(lastMovement); // Use lastMovement instead of movement
        movement = Vector2.zero;
        ResetDirectionAnimations();
    }

    //private void SetLastDirectionIdleSprite(Vector2 moveDirection)
    //{
    //    if (moveDirection.x < 0)
    //    {
    //        spriteRenderer.sprite = MazeInThePyramidsOfEgyptManager.Instance.spriteLeft;
    //    }
    //    else if (moveDirection.x > 0)
    //    {
    //        spriteRenderer.sprite = MazeInThePyramidsOfEgyptManager.Instance.spriteRight;
    //    }
    //    else if (moveDirection.y < 0)
    //    {
    //        spriteRenderer.sprite = MazeInThePyramidsOfEgyptManager.Instance.spriteDown;
    //    }
    //    else if (moveDirection.y > 0)
    //    {
    //        spriteRenderer.sprite = MazeInThePyramidsOfEgyptManager.Instance.spriteUp;
    //    }
    //}
}
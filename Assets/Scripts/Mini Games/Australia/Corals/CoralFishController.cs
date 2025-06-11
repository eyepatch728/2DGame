using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CoralFishController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float steeringSensitivity = 0.5f;
    public float smoothRotation = 5f;  // Added for smoother rotation

    [Header("UI References")]
    public SteeringWheel steeringWheel;
    public Button forwardButton;
    public Button backwardButton;

    [Header("Components")]
    public SpriteRenderer spriteRenderer;  // Reference to sprite renderer
    public Animator animator;              // Reference to animator

    private bool isMovingForward = false;
    private bool isMovingBackward = false;
    private Vector3 currentVelocity;      // For smooth movement
    private float targetRotation;         // For smooth rotation
    private Rigidbody2D rb;
    private float verticalInput = 0f;          // Smoothed input for forward/backward movement
    private float targetSteeringAngle = 0f;
    void Start()
    {
        // Initialize the fish facing right
        transform.rotation = Quaternion.Euler(0, 0, 0);

        // Add button event listeners
        AddButtonListeners(forwardButton, OnForwardButtonPressed, OnForwardButtonReleased);
        AddButtonListeners(backwardButton, OnBackwardButtonPressed, OnBackwardButtonReleased);
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0; // Disable gravity for top-down movement
    }

    private void Update()
    {
        float targetInput = 0f;

        if (isMovingForward)
        {
            targetInput = 1f;
        }
        else if (isMovingBackward)
        {
            targetInput = -1f;
        }
        // Smooth the vertical input to avoid jerks
        verticalInput = Mathf.Lerp(verticalInput, targetInput, Time.deltaTime * 10f);

        // Smooth the steering angle
        if (steeringWheel != null)
        {
            targetSteeringAngle = Mathf.Lerp(targetSteeringAngle, steeringWheel.currentSteeringAngle, Time.deltaTime * 5f);
        }
    }
    void FixedUpdate()
    {
        HandleMovement();
        HandleRotation();
        UpdateAnimation();
    }

    void HandleMovement()
    {
        // Check which movement is active based on button presses
        float targetInput = 0f;
        if (isMovingForward)
        {
            targetInput = 1f;
        }
        else if (isMovingBackward)
        {
            targetInput = -1f;
        }

        // Smooth the input transition
        verticalInput = Mathf.Lerp(verticalInput, targetInput, Time.fixedDeltaTime * 10f);

        // Move the fish forward/backward relative to its facing direction
        Vector2 movement = transform.right * verticalInput * moveSpeed * Time.fixedDeltaTime; // Changed from `transform.up` to `transform.right`
        rb.MovePosition(rb.position + movement);
    }

    void HandleRotation()
    {
        // Get a smoothed steering angle from the SteeringWheel script
        float steeringAngle = Mathf.Lerp(0, steeringWheel.currentSteeringAngle, Time.fixedDeltaTime * 10f);

        // Apply rotation to the Rigidbody2D - removed direction multiplier since steering should be consistent
        float rotationAmount = steeringAngle * steeringSensitivity * Time.fixedDeltaTime;
        rb.MoveRotation(rb.rotation + rotationAmount);
    }


    void UpdateAnimation()
    {
        if (animator != null)
        {
            animator.SetBool("IsSwimming", isMovingForward || isMovingBackward);
        }
    }

    void OnForwardButtonPressed(PointerEventData eventData)
    {
        isMovingForward = true;
        isMovingBackward = false;
    }

    void OnForwardButtonReleased(PointerEventData eventData)
    {
        isMovingForward = false;
    }

    void OnBackwardButtonPressed(PointerEventData eventData)
    {
        isMovingBackward = true;
        isMovingForward = false;
    }

    void OnBackwardButtonReleased(PointerEventData eventData)
    {
        isMovingBackward = false;
    }

    void AddButtonListeners(Button button, UnityEngine.Events.UnityAction<PointerEventData> pressListener, UnityEngine.Events.UnityAction<PointerEventData> releaseListener)
    {
        EventTrigger eventTrigger = button.gameObject.GetComponent<EventTrigger>();
        if (eventTrigger == null)
        {
            eventTrigger = button.gameObject.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry pointerDownEntry = new EventTrigger.Entry();
        pointerDownEntry.eventID = EventTriggerType.PointerDown;
        pointerDownEntry.callback.AddListener((eventData) => pressListener((PointerEventData)eventData));

        EventTrigger.Entry pointerUpEntry = new EventTrigger.Entry();
        pointerUpEntry.eventID = EventTriggerType.PointerUp;
        pointerUpEntry.callback.AddListener((eventData) => releaseListener((PointerEventData)eventData));

        eventTrigger.triggers.Add(pointerDownEntry);
        eventTrigger.triggers.Add(pointerUpEntry);
    }
}

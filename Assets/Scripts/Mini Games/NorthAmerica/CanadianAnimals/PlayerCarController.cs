using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // Add this for handling PointerDown and PointerUp events

public class PlayerCarController : MonoBehaviour
{
    public float moveSpeed = 5f;               // Speed for forward/backward movement
    public SteeringWheel steeringWheel;        // Reference to the SteeringWheel script
    public float steeringSensitivity = 0.5f;   // Sensitivity of the steering effect

    public Button forwardButton;               // Button for forward movement
    public Button backwardButton;              // Button for backward movement

    private bool isMovingForward = false;      // Track whether the forward button is pressed
    private bool isMovingBackward = false;     // Track whether the backward button is pressed

    private Rigidbody2D rb;
    private float verticalInput = 0f;          // Smoothed input for forward/backward movement
    private float targetSteeringAngle = 0f;    // Smoothed steering angle

    [SerializeField] ParticleSystem ps;
    void Start()
    {
        // Add EventTrigger to handle PointerDown and PointerUp
        AddButtonListeners(forwardButton, OnForwardButtonPressed, OnForwardButtonReleased);
        AddButtonListeners(backwardButton, OnBackwardButtonPressed, OnBackwardButtonReleased);

        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0; // Disable gravity for top-down movement
    }

    void Update()
    {
        // Collect and smooth input in Update
        float targetInput = 0f;

        if (isMovingForward)
        {
            targetInput = 1f;
            if(ps != null)
             ps.Play();
        }

        else if (isMovingBackward)
        {
            targetInput = -1f;
        }
        else 
        {
            if (ps != null)
                ps.Stop();
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

    // Smooth the vertical input to avoid jerks
    verticalInput = Mathf.Lerp(verticalInput, targetInput, Time.fixedDeltaTime * 10f);

    // Move the car forward/backward using Rigidbody2D
    Vector2 movement = transform.up * verticalInput * moveSpeed * Time.fixedDeltaTime; // Use `transform.up` for local forward direction
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


    // Event handlers for button presses
    void OnForwardButtonPressed(PointerEventData eventData)
    {
        isMovingForward = true;
        isMovingBackward = false;  // Ensure backward movement is not active
    }

    void OnForwardButtonReleased(PointerEventData eventData)
    {
        isMovingForward = false;   // Stop moving forward when released
    }

    void OnBackwardButtonPressed(PointerEventData eventData)
    {
        isMovingBackward = true;
        isMovingForward = false;   // Ensure forward movement is not active
    }

    void OnBackwardButtonReleased(PointerEventData eventData)
    {
        isMovingBackward = false;  // Stop moving backward when released
    }

    // Helper function to add PointerDown and PointerUp event listeners to buttons
    void AddButtonListeners(Button button, UnityEngine.Events.UnityAction<PointerEventData> pressListener, UnityEngine.Events.UnityAction<PointerEventData> releaseListener)
    {
        // Get or add EventTrigger component
        EventTrigger eventTrigger = button.gameObject.GetComponent<EventTrigger>();
        if (eventTrigger == null)
        {
            eventTrigger = button.gameObject.AddComponent<EventTrigger>();
        }

        // Create new event entry for PointerDown and PointerUp
        EventTrigger.Entry pointerDownEntry = new EventTrigger.Entry();
        pointerDownEntry.eventID = EventTriggerType.PointerDown;
        pointerDownEntry.callback.AddListener((eventData) => pressListener((PointerEventData)eventData));

        EventTrigger.Entry pointerUpEntry = new EventTrigger.Entry();
        pointerUpEntry.eventID = EventTriggerType.PointerUp;
        pointerUpEntry.callback.AddListener((eventData) => releaseListener((PointerEventData)eventData));

        // Add the event entries to the EventTrigger
        eventTrigger.triggers.Add(pointerDownEntry);
        eventTrigger.triggers.Add(pointerUpEntry);
    }
}

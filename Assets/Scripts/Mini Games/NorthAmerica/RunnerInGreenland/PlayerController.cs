using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 10f;      // Speed at which the player moves to the target lane
    private int currentLane = 1;       // Start at the middle lane (0 = left, 1 = middle, 2 = right)
    public Transform[] lanes;          // Array of lane positions

    private Vector3 targetPosition;    // The target position for the player

    private Vector2 touchStartPos;     // Start position of the swipe
    private bool isSwiping = false;    // Tracks if a swipe is in progress

    private bool hasCollided = false;  // Flag to track if the player has already collided with an ice block
    //private int starCount = 3;
    public Image[] stars;
    public Animator animator;
    void Start()
    {
        // Initialize the target position to the starting lane
        targetPosition = transform.position;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (GameManager.Instance.IsGameActive())
        {
            
            MoveToCurrentLane();
            if (!hasCollided)
            {
                HandleSwipeInput();
                animator.SetBool("Hit", false);
            }
        }
    }

    void HandleSwipeInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    touchStartPos = touch.position;
                    isSwiping = true;
                    break;

                case TouchPhase.Ended:
                    if (isSwiping)
                    {
                        Vector2 swipeDelta = touch.position - touchStartPos;
                        isSwiping = false;

                        // Check if the swipe is primarily horizontal
                        if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
                        {
                            if (swipeDelta.x > 0)
                            {
                                MoveRight();
                            }
                            else
                            {
                                MoveLeft();
                            }
                        }
                    }
                    break;
            }
        }
    }

    void MoveLeft()
    {
        if (currentLane > 0)
        {
            currentLane--;
            UpdateTargetPosition();
        }
    }

    void MoveRight()
    {
        if (currentLane < lanes.Length - 1)
        {
            currentLane++;
            UpdateTargetPosition();
        }
    }

    void UpdateTargetPosition()
    {

        SoundManager.instance.PlaySingleSound(9);
        targetPosition = new Vector3(lanes[currentLane].position.x, transform.position.y, transform.position.z);
    }

    void MoveToCurrentLane()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    // This is the important part for detecting collision with ice blocks
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the player is already collided with an ice block
        if (hasCollided)
            return;

        // Print out the name of the collided object for debugging purposes
        

        if (GameManager.Instance.IsGameActive())
        {
            if (other.gameObject.CompareTag("iceblock"))
            {
                Debug.Log("Collided with: iceblock");
                hasCollided = true;
                animator.SetBool("Hit", true);
                //DeductStar();
                // Add any further actions here, such as game over, damage, etc.

                // You can reset hasCollided after a brief delay (e.g., for a few seconds or after the player moves).
                // For example, you can reset the flag after the player moves away from the block:
                StartCoroutine(ResetCollisionFlag());
            }
        }
    }
    //private void DeductStar()
    //{
    //    if (starCount > 0)
    //    {
    //        starCount--;
    //        // Disable the star at the current index
    //        if (starCount >= 0)
    //        {
    //            stars[starCount].enabled = false; // Disable the star image
    //        }
    //    }
    //}
    // Coroutine to reset the collision flag after a brief delay
    private IEnumerator ResetCollisionFlag()
    {
        yield return new WaitForSeconds(1.5f);  // Adjust delay as needed
        hasCollided = false;
        animator.SetBool("Hit", false);
    }
}

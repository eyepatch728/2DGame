using UnityEngine;

public class PlatformController : MonoBehaviour
{
    public float speed = 1f; // Speed of the platform movement
    public bool moveLeft = true; // Determines the direction
    public float leftBound = -10f; // Leftmost position for the platform
    public float rightBound = 10f; // Rightmost position for the platform
    private void Start()
    {
        speed = Random.Range(1.0f, 3.5f); // Randomize speed
    }
    private void Update()
    {
        if (CrossTheNileRiverManager.Instance.isGameStart())
        {
            // Move platforms in the current direction
            Vector3 movement = moveLeft ? Vector3.left : Vector3.right;
            transform.Translate(movement * speed * Time.deltaTime);

            // Reverse direction when reaching bounds
            if (transform.position.x <= leftBound && moveLeft)
            {
                moveLeft = false; // Change direction to right
                AdjustCrocodileDirection();
            }
            else if (transform.position.x >= rightBound && !moveLeft)
            {
                moveLeft = true; // Change direction to left
                AdjustCrocodileDirection();
            }
        }
        
    }
    private void AdjustCrocodileDirection()
    {
        // Iterate through all child objects in the platform row
        foreach (Transform child in transform)
        {
            // Check if the child is a crocodile
            if (child.CompareTag("crocodile"))
            {
                SpriteRenderer sr = child.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    // Flip the crocodile sprite based on movement direction
                    sr.flipX = moveLeft; // flipX is true if moving right, false if moving left
                }
            }
        }
    }
}

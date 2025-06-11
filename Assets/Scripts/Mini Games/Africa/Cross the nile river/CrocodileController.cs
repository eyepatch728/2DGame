using UnityEngine;

public class CrocodileController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public bool movingLeft = true;

    [Header("Boundary Settings")]
    public float leftBoundary = -10f;
    public float rightBoundary = 10f;

    void Update()
    {
        // Move crocodile horizontally
        float moveDirection = movingLeft ? -1 : 1;
        transform.Translate(Vector2.right * moveSpeed * moveDirection * Time.deltaTime);

        // Reverse direction at boundaries
        if (transform.position.x <= leftBoundary)
        {
            movingLeft = false;
            FlipSprite();
        }
        else if (transform.position.x >= rightBoundary)
        {
            movingLeft = true;
            FlipSprite();
        }
    }

    void FlipSprite()
    {
        // Flip sprite based on movement direction
        transform.localScale = new Vector3(
            transform.localScale.x * -1,
            transform.localScale.y,
            transform.localScale.z
        );
    }
}

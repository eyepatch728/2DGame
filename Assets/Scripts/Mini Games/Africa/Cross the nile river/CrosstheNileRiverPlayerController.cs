using System;
using System.Collections;
using UnityEngine;

public class CrosstheNileRiverPlayerController : MonoBehaviour
{
    [Header("Jump Settings")]
    public float jumpForce = 5f;

    private Rigidbody2D rb;
    private Renderer playerRenderer;
    private bool canJump = true;
    private bool finalJumpTriggered = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerRenderer = GetComponent<Renderer>();
    }

    void Update()
    {
        if (CrossTheNileRiverManager.Instance.isGameStart())
        {
            // Check for touch/tap input
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                TryJump();
            }

            // Optional: For testing in Unity Editor
            if (Input.GetMouseButtonDown(0))
            {
                TryJump();
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Reset jump when touching ground
        if (collision.gameObject.CompareTag("Ground"))
        {
            canJump = true;
        }
        else if (collision.gameObject.CompareTag("FinishGround") && !finalJumpTriggered)
        {
            // Prevent multiple triggers
            finalJumpTriggered = true;
            canJump = false;
            gameObject.transform.SetParent(collision.transform);
            rb.constraints = RigidbodyConstraints2D.FreezeAll;

            // Perform the final jump sequence before completing the game
            StartCoroutine(FinalJumpSequence());
        }

        // When landing on a platform, make the player a child of the platform
        if (collision.gameObject.CompareTag("Platform"))
        {
            canJump = true;
            transform.SetParent(collision.transform, true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Handle crocodile collision
        if (collision.CompareTag("crocodile"))
        {
            SoundManager.instance.PlaySingleSound(5);
            StartCoroutine(PlayHitAnimation());
        }
    }

    IEnumerator PlayHitAnimation()
    {
        // Flash effect with color changing
        for (int i = 0; i < 3; i++)
        {
            playerRenderer.material.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            playerRenderer.material.color = Color.white;
            yield return new WaitForSeconds(0.1f);
        }

        playerRenderer.material.color = Color.white;
    }

    void TryJump()
    {
        if (canJump)
        {
            SoundManager.instance.PlaySingleSound(7);
            transform.SetParent(null, true);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            canJump = false;
        }
    }

    // Final jump before the game completes
    private IEnumerator FinalJumpSequence()
    {
        Debug.Log("Performing final jump before game completion...");

        // Unfreeze player and apply jump force
        rb.constraints = RigidbodyConstraints2D.None;
        transform.SetParent(null, true);
        rb.AddForce(Vector2.up * jumpForce * 1.2f, ForceMode2D.Impulse); // Slightly stronger jump

        yield return new WaitForSeconds(2f); // Wait for the jump to finish

        Debug.Log("Player landed! Completing the game...");
        CrossTheNileRiverManager.Instance.GameOver(); // Complete the game after landing
    }
}

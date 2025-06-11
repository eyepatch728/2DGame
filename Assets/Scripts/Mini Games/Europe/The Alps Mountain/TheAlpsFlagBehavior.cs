using UnityEngine;

public class TheAlpsFlagBehavior : MonoBehaviour
{
    public TheAlpsFlagManager.FlagData flagData; // The data for this flag
    public TheAlpsFlagManager flagManager;      // Reference to the FlagManager

    public float slideSpeed = 2f;

    void Update()
    {
        if (TheAlpsMountainManager.Instance.IsGameActive())
        {
            transform.Translate(Vector2.down * slideSpeed * Time.deltaTime);

            // Destroy ice block when it goes off screen
            if (transform.position.y < -5f)
            {
                flagManager.RespawnFlag(flagData);

                // Destroy the current flag
                Destroy(gameObject);
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Notify the manager that the flag was collected
            flagManager.FlagCollected(gameObject, flagData);
        }
    }

    //private void OnBecameInvisible()
    //{
    //    // Respawn the flag if it goes off-screen without being collected
       
    //}
}

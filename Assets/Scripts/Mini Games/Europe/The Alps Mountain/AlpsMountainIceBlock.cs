using UnityEngine;

public class AlpsMountainIceBlock : MonoBehaviour
{
    public float slideSpeed = 5f;

    void Update()
    {
        if (TheAlpsMountainManager.Instance.IsGameActive())
        {
            transform.Translate(Vector2.down * slideSpeed * Time.deltaTime);

            // Destroy ice block when it goes off screen
            if (transform.position.y < -5f)
            {
                Destroy(gameObject);
            }
        }
    }
}

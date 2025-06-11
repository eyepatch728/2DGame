using UnityEngine;

public class IceBlock : MonoBehaviour
{
    public float slideSpeed = 5f;

    void Update()
    {
        if (GameManager.Instance.IsGameActive())
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

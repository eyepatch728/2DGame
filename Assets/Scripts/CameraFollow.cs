using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;            // Reference to the player's transform (the car)
    public Vector3 offset;              // Offset from the player
    public float smoothSpeed = 0.125f;  // Smoothing factor for camera movement

    public SpriteRenderer background;   // Reference to the background sprite

    private float minX, maxX, minY, maxY;
    private float camHalfWidth, camHalfHeight;

    void Start()
    {
        if (offset == Vector3.zero)
        {
            offset = transform.position - player.position;
        }

        if (background != null)
        {
            CalculateCameraBounds();
        }
        else
        {
            Debug.LogWarning("No background assigned! Camera may show empty areas.");
        }
    }

    void LateUpdate()
    {
        Vector3 desiredPosition = player.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Clamp the position to stay within bounds
        transform.position = new Vector3(
            Mathf.Clamp(smoothedPosition.x, minX, maxX),
            Mathf.Clamp(smoothedPosition.y, minY, maxY),
            smoothedPosition.z
        );
    }

    private void CalculateCameraBounds()
    {
        Camera cam = Camera.main;
        camHalfHeight = cam.orthographicSize;
        camHalfWidth = camHalfHeight * cam.aspect;

        Bounds bgBounds = background.bounds;

        // Calculate min and max camera positions to keep background fully visible
        minX = bgBounds.min.x + camHalfWidth;
        maxX = bgBounds.max.x - camHalfWidth;
        minY = bgBounds.min.y + camHalfHeight;
        maxY = bgBounds.max.y - camHalfHeight;
    }
}

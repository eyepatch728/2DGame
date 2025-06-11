using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class TowerOfPisaCameraFollow : MonoBehaviour
{
    [Header("Camera Settings")]
    public float smoothSpeed = 0.125f;    // Smoothness of the camera movement
    public Vector3 initialOffset;         // Initial offset of the camera
    public float heightIncrement = 1f;    // How much to increase the camera's Y position per floor
    public float minHeight = 0f;          // Minimum y position of the camera
    public float maxHeight = 20f;         // Maximum y position of the camera

    private Vector3 currentOffset;

    private void Start()
    {
        currentOffset = initialOffset;
    }

    private void FixedUpdate()
    {
        if(!TheTowerOfPisaManager.Instance.isGameWon)
        {
            // Update the camera's position based on the offset
            Vector3 desiredPosition = new Vector3(transform.position.x, currentOffset.y, transform.position.z);
            desiredPosition.y = Mathf.Clamp(desiredPosition.y, minHeight, maxHeight);  // Clamp within bounds

            // Smoothly move the camera to the new position
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
            return;
        }
        else
        {
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, new Vector3(transform.position.x, 0.52f, transform.position.z), smoothSpeed);
            transform.position = smoothedPosition;
        }
    }

    // Method to increment the camera's Y offset when a new block is placed
    public void IncrementYOffset()
    {
        currentOffset.y += heightIncrement;
    }
}

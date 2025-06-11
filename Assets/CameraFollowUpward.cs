using UnityEngine;

public class CameraFollowUpward : MonoBehaviour
{
    public Transform target; // The player or object to follow
    public float smoothSpeed = 0.125f; // Smoothness of the movement
    public float offsetY = 2f; // Offset in the upward direction
    private float initialX; // Fixed X position of the camera

    void Start()
    {
        // Store the initial X position of the camera
        initialX = transform.position.x;

        // Optional: Ensure the target is assigned
        if (target == null)
        {
            Debug.LogError("CameraFollowUpward: Target not assigned!");
        }
    }

    //void LateUpdate()
    //{
    //    if (target == null) return;

    //    // Calculate the desired position (fixed X, smooth Y)
    //    Vector3 desiredPosition = new Vector3(initialX, target.position.y + offsetY, transform.position.z);

    //    // Ensure the camera doesn't move downward
    //    if (desiredPosition.y < transform.position.y)
    //    {
    //        desiredPosition.y = transform.position.y;
    //    }

    //    // Smoothly move the camera towards the desired position
    //    Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
    //    transform.position = smoothedPosition;
    //}
    void LateUpdate()
    {
        if (target == null) return;

        // Calculate the desired position (fixed X, smooth Y)
        Vector3 desiredPosition = new Vector3(transform.position.x, target.position.y + offsetY, transform.position.z);

        // Smoothly move the camera towards the desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}

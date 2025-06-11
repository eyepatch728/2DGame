using UnityEngine;

public class TowerOfPisaRopeMovement : MonoBehaviour


{
    public TowerOfPisaCameraFollow cameraFollow;

    [Header("Pendulum Settings")]
    public float maxSwingAngle = 45f;    // Maximum swing angle in degrees
    public float swingSpeed = 2f;        // Speed of the swing
    public float ropeLength = 3f;        // Length of the rope
    [Header("Height Settings")]
    public float blockHeight = 1f;       // Height of each block
    public float heightOffset = 2f;      // Extra space above the last placed block

    private Transform ropeEnd;
    private Vector3 initialPosition;
    private void Awake()
    {
        // Create rope end point
        ropeEnd = new GameObject("RopeEnd").transform;
        ropeEnd.parent = transform;
        ropeEnd.localPosition = Vector3.down * ropeLength;
        initialPosition = transform.position;
    }

    private void Update()
    {
        if (TheTowerOfPisaManager.Instance.isGameStart())
        {
            // Calculate the current angle using a sine wave

            float angle = maxSwingAngle * Mathf.Sin(Time.time * swingSpeed);
            if (angle > 37 || angle < -37)
            {
                SoundManager.instance.PlaySingleSound(9);
            }
            // Apply rotation to the rope
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        
    }

    public Vector3 GetEndPosition()
    {
        return ropeEnd.position;
    }
    public void UpdateRopeHeight(int currentFloor)
    {
        // Calculate new height based on current floor
        float newHeight = initialPosition.y + (currentFloor * blockHeight);

        // Update rope position with offset
        transform.position = new Vector3(
            initialPosition.x,
            newHeight + heightOffset,
            initialPosition.z
        );
    }
}

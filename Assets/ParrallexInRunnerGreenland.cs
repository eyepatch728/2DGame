using UnityEngine;

public class ParallaxScrollY : MonoBehaviour
{
    public float scrollSpeed = 0.5f; // Adjust speed in Inspector
    private Vector3 startPosition;
    private float spriteHeight;
    public float resetY = 7f;        // Y position to reset to
    public float minY = -14f;

    public GameObject popUp;
    public GameObject tutorial;
    public GameObject finalPopUp;



    void Start()
    {
        if (!popUp.activeSelf && !tutorial.activeSelf)
        {
            startPosition = transform.position;
            spriteHeight = GetComponent<SpriteRenderer>().bounds.size.y; // Get image height
        }
    }

    void Update()
    {
        if ((popUp == null || !popUp.activeSelf) &&
            (tutorial == null || !tutorial.activeSelf) &&
            (finalPopUp == null || !finalPopUp.activeSelf)) // Added finalPopUp check
        {
            // Move background down
            transform.position += Vector3.down * scrollSpeed * Time.deltaTime;

            // Reset position when reaching minY
            if (transform.position.y <= minY)
            {
                transform.position = new Vector3(transform.position.x, resetY, transform.position.z);
            }
        }
    }

}

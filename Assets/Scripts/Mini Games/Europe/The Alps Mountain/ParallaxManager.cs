using UnityEngine;

public class ParallaxManager : MonoBehaviour
{
    public Transform background1;
    public Transform background2;
    public float scrollSpeed = 0.5f;

    private float bgHeight;

    public GameObject popUp;
    public GameObject tutorial;
    public GameObject finalPopUp;

    void Start()
    {
        bgHeight = background1.GetComponent<SpriteRenderer>().bounds.size.y;
        background2.position = new Vector3(background1.position.x, background1.position.y + bgHeight, background1.position.z);
    }

    void Update()
    {
        if ((popUp == null || !popUp.activeSelf) &&
            (tutorial == null || !tutorial.activeSelf) &&
            (finalPopUp == null || !finalPopUp.activeSelf))
        {
            // Move both backgrounds down
            background1.position += Vector3.down * scrollSpeed * Time.deltaTime;
            background2.position += Vector3.down * scrollSpeed * Time.deltaTime;

            // Check if a background has moved out of view
            if (background1.position.y <= -bgHeight)
            {
                background1.position = new Vector3(background1.position.x, background2.position.y + bgHeight, background1.position.z);
                SwapBackgrounds();
            }
            else if (background2.position.y <= -bgHeight)
            {
                background2.position = new Vector3(background2.position.x, background1.position.y + bgHeight, background2.position.z);
                SwapBackgrounds();
            }
        }
    }

    void SwapBackgrounds()
    {
        // Swap references for proper looping
        Transform temp = background1;
        background1 = background2;
        background2 = temp;
    }
}

using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
public class CatCart : MonoBehaviour
{
    public float moveSpeed = 10f;
    private Vector3 touchOffset; // Offset from the cart's position to touch position
    private bool isDragging = false;

    private void Update()
    {
        if (SceneManager.GetActiveScene().name=="Brazilian Festival")
        {
            if (BrazillianFestivalManager.Instance.isGameStart() && !BrazillianFestivalManager.Instance.isGamePaused)
            {
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);

                    switch (touch.phase)
                    {
                        case TouchPhase.Began:
                            // Check if the touch is on the cart
                            if (IsTouchingCart(touch.position))
                            {
                                isDragging = true;
                                // Calculate offset between touch position and cart position
                                Vector3 worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, Camera.main.nearClipPlane));
                                touchOffset = transform.position - worldPoint;
                            }
                            break;

                        case TouchPhase.Moved:
                            if (isDragging)
                            {
                                // Move the cart based on current touch position
                                Vector3 worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, Camera.main.nearClipPlane));
                                Vector3 newPosition = worldPoint + touchOffset; // Apply offset
                                newPosition.y = transform.position.y; // Keep original y position
                                transform.position = newPosition; // Update cart position
                            }
                            break;

                        case TouchPhase.Ended:
                        case TouchPhase.Canceled:
                            isDragging = false; // Stop dragging when touch ends or is canceled
                            break;
                    }
                }

                ClampPosition();
            }
        } 
        else if (SceneManager.GetActiveScene().name=="China Festival")
        {
            if (ChinaFestivalsManager.Instance.isGameStart() && !ChinaFestivalsManager.Instance.isGamePaused)
            {
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);

                    switch (touch.phase)
                    {
                        case TouchPhase.Began:
                            // Check if the touch is on the cart
                            if (IsTouchingCart(touch.position))
                            {
                                isDragging = true;
                                // Calculate offset between touch position and cart position
                                Vector3 worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, Camera.main.nearClipPlane));
                                touchOffset = transform.position - worldPoint;
                            }
                            break;

                        case TouchPhase.Moved:
                            if (isDragging)
                            {
                                // Move the cart based on current touch position
                                Vector3 worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, Camera.main.nearClipPlane));
                                Vector3 newPosition = worldPoint + touchOffset; // Apply offset
                                newPosition.y = transform.position.y; // Keep original y position
                                transform.position = newPosition; // Update cart position
                            }
                            break;

                        case TouchPhase.Ended:
                        case TouchPhase.Canceled:
                            isDragging = false; // Stop dragging when touch ends or is canceled
                            break;
                    }
                }

                ClampPosition();
            }
        }
       
       
    }

    private bool IsTouchingCart(Vector2 touchPosition)
    {

        // Convert touch position to world point
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(touchPosition.x, touchPosition.y, Camera.main.nearClipPlane));

        // Check if the world point is within the bounds of the cart
        Collider2D collider = GetComponent<Collider2D>(); // Assuming there is a Collider2D on the cart
        return collider != null && collider.OverlapPoint(worldPoint);
    }

    private void ClampPosition()
    {
        Vector3 position = transform.position;
        position.x = Mathf.Clamp(position.x, -8f, 8f); // Adjust based on your camera view
        transform.position = position;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<FallingItem>(out FallingItem fallingItem))
        {
            BrazillianFestivalManager.FestivalItem item = fallingItem.GetItem();

            if (!item.isCollected)
            {
                item.isCollected = true;
                BrazillianFestivalManager.Instance.PauseGame();
                // Check if this will be the last collected item
                bool isLastItem = BrazillianFestivalManager.Instance.festivalItems.All(i => i == item || i.isCollected);
                
                BrazillianFestivalManager.Instance.ShowItemPopup(item, isLastItem);
            }

            Destroy(other.gameObject);
        }
        else if (other.TryGetComponent<ChinaFestivalFallingItems>(out ChinaFestivalFallingItems chinaFallingItem))
        {
            ChinaFestivalsManager.FestivalItem item = chinaFallingItem.GetItem();

            if (!item.isCollected)
            {
                item.isCollected = true;
                ChinaFestivalsManager.Instance.PauseGame();

                // Check if this will be the last collected item
                bool isLastItem = ChinaFestivalsManager.Instance.festivalItems.All(i => i == item || i.isCollected);

                ChinaFestivalsManager.Instance.ShowItemPopup(item, isLastItem);
            }

            Destroy(other.gameObject);
        }

    }
}

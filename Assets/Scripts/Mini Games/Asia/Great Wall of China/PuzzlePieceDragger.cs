using UnityEngine;

public class PuzzlePieceDragger : MonoBehaviour
{
    private Vector3 originalPosition;
    private PuzzleSlot targetSlot;
    private bool isDraggable = true;
    void Start()
    {
        originalPosition = transform.position;
    }

    void OnMouseDown()
    {
        if (GreatWallOfChinaManager.Instance.isGameStart())
        {
            // Only allow dragging if the piece is still draggable
            if (!isDraggable)
                return;

            this.GetComponent<Collider2D>().enabled = false; // Disable collider to allow free movement
        }
        
    }

    void OnMouseDrag()
    {
        if (GreatWallOfChinaManager.Instance.isGameStart())
        {
            // Only allow dragging if the piece is still draggable
            if (!isDraggable)
                return;

            // Move the piece with the mouse
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(mousePosition.x, mousePosition.y, 0);
        }
          
    }

    void OnMouseUp()
    {
        if (GreatWallOfChinaManager.Instance.isGameStart())
        {
            // Check if the piece is over the correct slot
            PuzzleSlot hitSlot = GetSlotUnderMouse();
            if (hitSlot != null && hitSlot.CanPlacePiece(this))
            {
                // Place the piece correctly
                SoundManager.instance.PlaySingleSound(0);
                transform.position = hitSlot.transform.position;
                targetSlot = hitSlot;

                // Mark the piece as no longer draggable
                isDraggable = false;

                // Disable further dragging interactions
                GetComponent<Collider2D>().enabled = false;

                // Inform the GameController to check if the puzzle is complete
                GreatWallOfChinaManager.Instance.CheckPuzzleCompletion();
            }
            else
            {
                // If not correctly placed, return to the original position
                transform.position = originalPosition;

                // Re-enable dragging if returned to original position
                GetComponent<Collider2D>().enabled = true;
            }
        }
           
    }

    private PuzzleSlot GetSlotUnderMouse()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (hit.collider != null)
        {
            return hit.collider.GetComponent<PuzzleSlot>();
        }
        return null;
    }

    public bool IsPlacedCorrectly()
    {
        return targetSlot != null && targetSlot.IsCorrectPiece(this);
    }

}

using UnityEngine;

public class AnimalBabyDraggable : MonoBehaviour
{
    private MatchParentChildSaManager gameManager;
    public MatchParentChildSaManager.AnimalSet animalData;
    private bool isDragging;
    private Vector3 dragOffset;
    private bool canDrag = false;
    public float childScaleFactor = 0.5f; // Adjust in inspector
    private ParentAnimal currentParentAnimal = null;
    private void Start()
    {
        // Reduce sprite size for baby animal
        transform.localScale = Vector3.one * childScaleFactor;
    }
    public void EnableDragging(bool enable)
    {
        canDrag = true;
    }
    public void Initialize(MatchParentChildSaManager.AnimalSet data, MatchParentChildSaManager manager)
    {
        animalData = data;
        gameManager = manager;
        GetComponent<SpriteRenderer>().sprite = data.babySprite;
    }

    private void OnMouseDown()
    {
        if (!canDrag) return;
        isDragging = true;
        dragOffset = transform.position - GetMouseWorldPosition();
    }

    private void OnMouseDrag()
    {
        if (isDragging)
        {
            transform.position = GetMouseWorldPosition() + dragOffset;
        }
    }

    private void OnMouseUp()
    {
        isDragging = false;
        CheckPlacement();
    }

    private void CheckPlacement()
    {
        bool isCorrect = false;

        // Check if the current parent animal is the correct one
        if (currentParentAnimal != null && currentParentAnimal.animalData.animalName == animalData.animalName)
        {
            isCorrect = true; // Correct parent, so placement is correct
        }

        // Notify the game manager about whether the placement is correct
        gameManager.OnBabyPlaced(isCorrect, animalData);
    }

    // Trigger detection for the correct parent animal
    private void OnTriggerEnter2D(Collider2D collider)
    {
        ParentAnimal parentAnimal = collider.GetComponent<ParentAnimal>();

        if (parentAnimal != null)
        {
            // If the baby animal is over the correct parent, track this parent
            currentParentAnimal = parentAnimal;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        // If the baby animal leaves the parent collider, reset the currentParentAnimal
        ParentAnimal parentAnimal = collider.GetComponent<ParentAnimal>();

        if (parentAnimal != null && parentAnimal == currentParentAnimal)
        {
            currentParentAnimal = null;
        }
    }


    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -Camera.main.transform.position.z;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }
    private void OnDrawGizmos()
    {
        // Draw the area where we're checking for colliders
        if (canDrag)
        {
            Gizmos.color = Color.green; // Color for the check area
            Gizmos.DrawWireSphere(transform.position, 0.5f); // Draw a wire sphere around the object
        }
    }
}

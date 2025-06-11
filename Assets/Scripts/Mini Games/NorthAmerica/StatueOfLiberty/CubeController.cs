using UnityEngine;

public class CubeController : MonoBehaviour
{
    private Vector2 initialPosition;
    private bool placedCorrectly = false;

    public Transform CircuitSlot;
    public bool CorrectCube; // Set this in the Inspector for the correct cube

    private Collider2D cubeCollider;

    void Start()
    {
        initialPosition = transform.position;
        cubeCollider = GetComponent<Collider2D>();
    }

    void OnMouseDrag()
    {
        if (StatueOfLibertyManager.Instance.isGameStart())
        {
            if (!cubeCollider.enabled)
            {

                
                return; // Exit early if collider is disabled
            }

            if (!placedCorrectly)
            {
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                transform.position = new Vector2(mousePosition.x, mousePosition.y);
            }
        }
       
    }

    void OnMouseUp()
    {
        if (StatueOfLibertyManager.Instance.isGameStart())
        {
            if (!cubeCollider.enabled)
            {
                return; // Exit early if collider is disabled
            }

            if (!placedCorrectly)
            {
                if (Vector2.Distance(transform.position, CircuitSlot.position) < 0.5f)
                {
                    if (CorrectCube)
                    {
                        transform.position = CircuitSlot.position;
                        placedCorrectly = true;
                        CircuitManager.Instance.CorrectPlacement();
                        StatueOfLibertyManager.Instance.Win();
                    }
                    else
                    {
                        transform.position = initialPosition; // Return to start
                    }
                }
                else
                {
                    transform.position = initialPosition; // Return to start
                }
            }
        }
        
    }

    public void SetColliderState(bool state)
    {
        cubeCollider.enabled = state;
    }

    public void DisableInteraction()
    {
        cubeCollider.enabled = false;
    }
}

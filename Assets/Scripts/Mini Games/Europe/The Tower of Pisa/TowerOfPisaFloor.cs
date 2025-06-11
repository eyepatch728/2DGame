using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TowerOfPisaFloor : MonoBehaviour
{
    [Header("Placement Settings")]
    public float allowedOffset = 0.5f;    // Smaller value for stricter placement

    private Rigidbody2D rb;
    private bool isPlaced = false;
    public BoxCollider2D boxColliderBottom;
    public BoxCollider2D boxColliderTop;
    public GameObject dustAnims;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;  // Start with gravity disabled while attached to rope
    }

    public bool TryPlace(Vector3 targetPosition)
    {
        if (isPlaced) return false;

        float offset = Mathf.Abs(transform.position.x - targetPosition.x);
        //Debug.Log($"Placement attempt - Offset: {offset}, Allowed: {allowedOffset}");

        //if (offset <= allowedOffset)
        //{
        //    // Successful placement
        //    transform.position = new Vector3(targetPosition.x, transform.position.y, transform.position.z);
        //    StartCoroutine(FinalizeSuccessfulPlacement());
        //    SpawnFloorNumber();
        //    boxColliderTop.enabled = true;
        //    TheTowerOfPisaManager.Instance.currentFloor++;
        //    StartCoroutine(TheTowerOfPisaManager.Instance.SpawnNextBlockAfterDelay(0.1f));
        //    TheTowerOfPisaManager.Instance.cameraFollow.IncrementYOffset();
        //    return true;
        //}
        //else
        {
            // Failed placement
            StartCoroutine(PlayDestroyAnimation(targetPosition));
            return false;
        }
    }

    private IEnumerator FinalizeSuccessfulPlacement()
    {

        // Enable gravity briefly to let the block settle
        rb.gravityScale = 1;

        // Wait a short moment for physics to settle
        yield return new WaitForSeconds(2f);

        // Disable the Rigidbody2D component
        rb.gravityScale = 0;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.isKinematic = true;  // Make it kinematic so it won't be affected by physics

        isPlaced = true;
    }


    private IEnumerator PlayDestroyAnimation(Vector3 targetPosition)
    {
        rb.gravityScale = 1;
        rb.AddTorque(10f);

        // Add random sideways force for more dynamic falling
        float randomForce = Random.Range(-2f, 2f);
        rb.AddForce(new Vector2(randomForce, 0), ForceMode2D.Impulse);

        yield return new WaitForSeconds(2f);
        if (isTriggering)
        {
            transform.position = new Vector3(0f, transform.position.y, transform.position.z);
            StartCoroutine(FinalizeSuccessfulPlacement());
            SpawnFloorNumber();
            boxColliderTop.enabled = true;
            dustAnims.gameObject.SetActive(true);

            //TheTowerOfPisaManager.Instance.currentFloor++;
            //TheTowerOfPisaManager.Instance.UpdateUI();
            //TheTowerOfPisaManager.Instance.rope.UpdateRopeHeight(TheTowerOfPisaManager.Instance.currentFloor);

            //StartCoroutine(TheTowerOfPisaManager.Instance.SpawnNextBlockAfterDelay(0.1f));
            //TheTowerOfPisaManager.Instance.cameraFollow.IncrementYOffset();
            TheTowerOfPisaManager.Instance.BoxSuccessfull();
            //print("Not Faulty 1");
        }
        else
        {
            StartCoroutine(TheTowerOfPisaManager.Instance.HandleUnsuccessfulPlacement());
            if (gameObject != null)
            {
                Destroy(gameObject,0.2f);
            }
        }

    }
    bool isTriggering;
    private void OnTriggerStay2D(Collider2D collision)
    {
        // Ensure the collision is happening with the correct object
        if (collision.CompareTag("towerCorrectTrigger"))
        {
            // Check if the collision is coming from the bottom collider only
            if (collision.IsTouching(boxColliderBottom))
            {
                isTriggering = true;
                Debug.Log("Trigger detected by bottom collider!");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("towerCorrectTrigger"))
        {
            isTriggering = false;
        }
    }

    private void SpawnFloorNumber()
    {
        //GameObject numberObj = new GameObject("FloorNumber");
        //TextMeshPro tmp = numberObj.AddComponent<TextMeshPro>();
        //tmp.text = TheTowerOfPisaManager.Instance.currentFloor.ToString();
        //tmp.transform.SetParent(transform);
        //tmp.transform.localPosition = new Vector3(0, 0.5f, 0);
        //tmp.alignment = TextAlignmentOptions.Center;
        //tmp.fontSize = 3;
        //tmp.color = Color.black;
    }
}

using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(LineRenderer))]
public class Player : MonoBehaviour
{
    private Vector2 dragStartPos;
    private Vector2 dragReleasePos;
    private Rigidbody2D rb;
    private bool isDragging = false;

    [SerializeField] private float shootForce = 10f;
    [SerializeField] private float maxDragDistance = 4f;
    private LineRenderer lineRenderer;
    public GameObject lastBlock;
    private bool isHanging = true;
    public SpriteRenderer armSprite;
    private Vector2 previousPosition;

    public GameObject popUp;
    public GameObject tutorial;
    public GameObject finalPopUp;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        DeActivatePhysicsToRigidBody();

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.5f;
        lineRenderer.endWidth = 0.5f;
        lineRenderer.enabled = true;

        if (armSprite != null)
        {
            armSprite.enabled = true;
        }
    }

    void Update()
    {
        if (!ArePopupsActive())
        {
            if (SceneManager.GetActiveScene().name == "Andes Mountain" && AndesMountainManager.Instance.isGameStart() ||
                SceneManager.GetActiveScene().name == "Everest Mountain" && EverestMountainManager.Instance.isGameStart())
            {
                if (!isHanging) return;

                if (Input.GetMouseButtonDown(0)) OnMouseDown();
                if (Input.GetMouseButton(0) && isDragging) OnMouseDrag();
                if (Input.GetMouseButtonUp(0)) OnMouseUp();

                UpdateLineRenderer();
            }
        }
    }

    void OnMouseDown()
    {

        if (ArePopupsActive() || !isHanging) return;

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider != null && hit.collider.gameObject == gameObject)
        {

            //SoundManager.instance.PlaySingleSound(2);
            isDragging = true;
            dragStartPos = (Vector2)lastBlock.transform.position;
            previousPosition = transform.position;
        }
    }

    void OnMouseDrag()
    {

        if (ArePopupsActive() || !isDragging) return;

        Vector2 currentMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 blockPos = lastBlock.transform.position;
        Vector2 dragVector = currentMousePos - blockPos;
        float dragDistance = dragVector.magnitude;

        if (dragDistance > maxDragDistance)
        {
            dragVector = dragVector.normalized * maxDragDistance;
        }

        Vector2 newPosition = blockPos + dragVector;
        if (newPosition.y > blockPos.y) newPosition.y = blockPos.y;

        transform.position = newPosition;
        previousPosition = transform.position;

        if (dragVector != Vector2.zero)
        {
            Vector2 direction = blockPos - (Vector2)transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        // **Stretch Hands Without Moving the Cat**
        if (armSprite != null)
        {
            armSprite.enabled = true;

            float stretchFactor = Mathf.Lerp(1f, 2.5f, dragDistance / maxDragDistance);

            // Stretching effect
            armSprite.transform.localScale = new Vector3(1f, stretchFactor, 1f);

            // Move the arm downward as it stretches
            armSprite.transform.position = new Vector3(armSprite.transform.position.x,
                transform.position.y - (stretchFactor - 1f) * 0.5f, armSprite.transform.position.z);
        }
    }





    void OnMouseUp()
    {

        if (ArePopupsActive() || !isHanging || !isDragging) return;

        isDragging = false;
        isHanging = false;
        dragReleasePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector2 forceDirection = dragStartPos - dragReleasePos;
        float forceMagnitude = Mathf.Clamp(forceDirection.magnitude, 0, maxDragDistance);
        forceDirection = forceDirection.normalized;

        Debug.DrawLine(dragStartPos, dragReleasePos, Color.red, 2f);

        ActivatePhysicsToRigidBody();

        SoundManager.instance.PlaySingleSound(2);
        rb.AddForce(forceDirection * forceMagnitude * shootForce, ForceMode2D.Impulse);

        lastBlock.GetComponent<BoxCollider2D>().enabled = false;
        Invoke("enableCollider", 0.2f);
        lineRenderer.enabled = false;

        // **Reset Hand Smoothly**
        if (armSprite != null)
        {
            armSprite.transform.localScale = Vector3.one;
            armSprite.transform.position = transform.position;  // Reset position
        }
    }


    void UpdateLineRenderer()
    {
        if (!lineRenderer.enabled || lastBlock == null) return;

        lineRenderer.sortingOrder = 1;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, lastBlock.transform.position);
    }

    public void enableCollider()
    {
        if (lastBlock != null)
        {
            lastBlock.GetComponent<BoxCollider2D>().enabled = true;
        }
    }

    public void ActivatePhysicsToRigidBody() => rb.isKinematic = false;
    public void DeActivatePhysicsToRigidBody() => rb.isKinematic = true;

    public void ResetSpring()
    {
        lineRenderer.enabled = true;
        rb.freezeRotation = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Block"))
        {
            if (lastBlock != other.gameObject && !isDragging)
            {
                rb.linearVelocity = Vector2.zero;
                lastBlock = other.gameObject;

                Vector3 targetPosition = (Vector2)other.gameObject.transform.position + Vector2.down * 1f;
                Vector3 topPositionOfBlock = (Vector2)other.gameObject.transform.position;

                if (armSprite != null) armSprite.enabled = true;

                transform.DOMove(topPositionOfBlock, 0.1f).OnComplete(() => {
                    transform.DOMove(targetPosition, 0.6f)
                        .SetEase(Ease.OutElastic)
                        .OnComplete(() => {
                            transform.position = targetPosition;
                            DeActivatePhysicsToRigidBody();
                            other.gameObject.GetComponent<Block>().CheckOnTop();
                        });
                });

                if (SceneManager.GetActiveScene().name == "Everest Mountain" &&
                    EverestMountainHeightTracker.Instance != null)
                {
                    EverestMountainHeightTracker.Instance.UpdateProgress(transform);
                }

                ResetSpring();
                isHanging = true;
                DeActivatePhysicsToRigidBody();
                transform.DORotate(Vector3.zero, 0.5f).SetEase(Ease.OutElastic);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            print("Collided with Ground");
            MoveToLastBlock();
        }
    }

    public void MoveToLastBlock()
    {
        if (lastBlock == null) return;

        rb.linearVelocity = Vector2.zero;

        Vector3 targetPosition = (Vector2)lastBlock.transform.position + Vector2.down * 1f;
        Vector3 topPositionOfBlock = (Vector2)lastBlock.transform.position;

        transform.DOMove(topPositionOfBlock, 0.1f).OnComplete(() => {
            transform.DOMove(targetPosition, 0.6f)
                .SetEase(Ease.OutElastic)
                .OnComplete(() => {
                    transform.position = targetPosition;
                    DeActivatePhysicsToRigidBody();
                    lastBlock.GetComponent<Block>().CheckOnTop();
                });
        });

        ResetSpring();
        isHanging = true;
        DeActivatePhysicsToRigidBody();
        transform.DORotate(Vector3.zero, 0.5f).SetEase(Ease.OutElastic);
    }

    public void Back()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private bool ArePopupsActive()
    {
        return (popUp != null && popUp.activeSelf) ||
               (tutorial != null && tutorial.activeSelf) ||
               (finalPopUp != null && finalPopUp.activeSelf);
    }
}

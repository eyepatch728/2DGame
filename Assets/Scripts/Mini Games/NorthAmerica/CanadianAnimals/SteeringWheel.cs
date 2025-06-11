using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class SteeringWheel : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public float maxSteeringAngle = 45f;
    public float returnSpeed = 300f;
    public float smoothingFactor = 10f;
    public float currentSteeringAngle { get; private set; }

    private bool isSteering = false;
    private float initialRotationZ = 0f;

    private void Start()
    {
        currentSteeringAngle = 0f;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isSteering = true;
        initialRotationZ = GetAngle(eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isSteering) return;

        float currentAngle = GetAngle(eventData.position);
        float deltaAngle = Mathf.DeltaAngle(initialRotationZ, currentAngle);

        currentSteeringAngle = Mathf.Clamp(currentSteeringAngle + deltaAngle, -maxSteeringAngle, maxSteeringAngle);
        transform.rotation = Quaternion.Euler(0, 0, currentSteeringAngle);

        initialRotationZ = currentAngle; // Keep updating for smooth rotation
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isSteering = false;
        StartCoroutine(ReturnToNeutral());
    }

    private IEnumerator ReturnToNeutral()
    {
        while (Mathf.Abs(currentSteeringAngle) > 0.1f)
        {
            currentSteeringAngle = Mathf.MoveTowards(currentSteeringAngle, 0f, returnSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0, 0, currentSteeringAngle);
            yield return null;
        }

        currentSteeringAngle = 0f;
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    private float GetAngle(Vector2 position)
    {
        Vector2 direction = position - (Vector2)transform.position;
        return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
    }
}

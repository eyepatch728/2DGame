using System.Collections.Generic;
using UnityEngine;

public class DragAndDrop2D : MonoBehaviour
{
    private Vector3 offset;
    private Camera mainCamera;
    public AlignParentOnCollision alignParentOnCollision;
    public AlignParentOnCollision alignParentOnCollision2;
    public List<Transform> connectedPieces = new List<Transform>(); // Tracks connected pieces

    void Start()
    {
        // Cache the main camera
        mainCamera = Camera.main;

        // Ensure the GameObject has a Collider2D
        if (GetComponent<Collider2D>() == null)
        {
            Debug.LogError("This script requires a Collider2D component.");
        }
    }

    void OnMouseDown()
    {
        // Calculate the offset between the mouse position and the object's position
        Vector3 mouseWorldPosition = GetMouseWorldPosition();
        offset = transform.position - mouseWorldPosition;
    }

    void OnMouseDrag()
    {
        FourPiecePuzzleGameManager.instance.isDragging = true;
        // Update the object's position to follow the mouse
        Vector3 mouseWorldPosition = GetMouseWorldPosition();
        Vector3 newPosition = mouseWorldPosition + offset;
        // Move the entire group
        Vector3 movementOffset = newPosition - transform.position;
        transform.position = newPosition;
        MoveGroup(movementOffset);
        //alignParentOnCollision2.MoveGroup(movementOffset);
        //alignParentOnCollision.transform.parent.GetComponent<DragAndDrop2D>().alignParentOnCollision.MoveGroup(movementOffset);
        //alignParentOnCollision2.transform.parent.GetComponent<DragAndDrop2D>().alignParentOnCollision.MoveGroup(movementOffset);
        //alignParentOnCollision.siblingCollider.MoveGroup(movementOffset);
        //alignParentOnCollision2.siblingCollider.MoveGroup(movementOffset);
    }

    void OnMouseUp()
    {
        FourPiecePuzzleGameManager.instance.isDragging = false;
        // Trigger collision check in AlignParentOnCollision
        if (alignParentOnCollision != null)
        {
            alignParentOnCollision.CheckCollision();
        }
        if (alignParentOnCollision2 != null)
        {
            alignParentOnCollision2.CheckCollision();
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        // Get the mouse position in screen space and convert it to world space
        Vector3 mouseScreenPosition = Input.mousePosition;
        mouseScreenPosition.z = Mathf.Abs(mainCamera.transform.position.z); // Set Z to the distance from the camera
        return mainCamera.ScreenToWorldPoint(mouseScreenPosition);
    }

    public void MoveGroup(Vector3 offset)
    {
        //print("Off Set: " + offset);
        // Move all connected pieces together
        print ("Group Count: " + connectedPieces.Count);
        foreach (var piece in connectedPieces)
        {
            if(piece!=this.transform)
                piece.position += offset;
        }
    }
}
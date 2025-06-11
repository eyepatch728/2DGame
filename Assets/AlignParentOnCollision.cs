using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlignParentOnCollision : MonoBehaviour
{
    public string targetTag = "Player"; // Tag to identify matching objects for collision private List<Transform> connectedPieces = new List<Transform>(); // Tracks connected pieces
    //public List<Transform> connectedPieces = new List<Transform>(); // Tracks connected pieces
    //public AlignParentOnCollision siblingCollider;
    public DragAndDrop2D parentDragAndDrop2D;
    public bool isJoined = false;
    public Collider2D myTargetCollider;
    //bool canCheckCollision = false;
    float lastCheckedTime = 0f;
    private void Start()
    {
        parentDragAndDrop2D = transform.parent.GetComponent<DragAndDrop2D>();
    }

    private void Update()
    {
        //if (lastCheckedTime <= 0)
        //{
        //    canCheckCollision = true;
        //}
        //else
        //{
        //    canCheckCollision = false;
        //    lastCheckedTime -= Time.deltaTime;
        //}
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!FourPiecePuzzleGameManager.instance.isDragging)
        {
            CheckCollision();
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!FourPiecePuzzleGameManager.instance.isDragging)
        {
            CheckCollision();
        }
    }
    public void CheckCollision()
    {
        // Find all colliders overlapping this object's collider
        Collider2D myCollider = GetComponent<Collider2D>();
        if (myCollider == null)
        {
            Debug.LogWarning("No Collider2D found on this object.");
            return;
        }

        Collider2D[] overlappingColliders = Physics2D.OverlapBoxAll(
            myCollider.bounds.center,
            myCollider.bounds.size,
            0f
        );

        foreach (var col in overlappingColliders)
        {
            // Ignore self and only process colliders with the target tag
            if (col.gameObject != gameObject && col.CompareTag(targetTag))
            {
                AlignParentPosition(col.transform);
                AddToGroup(col.transform.parent);
                col.transform.GetComponent<AlignParentOnCollision>().AddToGroup(transform.parent);
                transform.GetComponent<BoxCollider2D>().enabled = false;
                col.transform.GetComponent<BoxCollider2D>().enabled = false;

                // make sure that the Parent is aligned to the correct position/transform as of it's targeted : myTargetCollider
                break;
            }
        }
    }

    private void AlignParentPosition(Transform otherTransform)
    {
        // Get the parent object's transform
        Transform parentTransform = transform.parent;
        if (parentTransform != null)
        {
            // Calculate the position offset needed to align the colliders
            Vector3 offset = otherTransform.position - transform.position;
            // Adjust the parent object's position
            parentTransform.position += offset;
            Debug.Log("Parent position adjusted to align colliders.");
            isJoined = true;
            //Create a coroutine here to make sure it is called after the parent position is adjusted
            //StartCoroutine(UpdateConnectedPieces());
        }
        else
        {
            Debug.LogWarning("This object has no parent transform.");
        }
    }
    //IEnumerator UpdateConnectedPieces()
    //{
    //    yield return new WaitForEndOfFrame();
    //parentDragAndDrop2D.connectedPieces = connectedPieces;
    // }


    private void AddToGroup(Transform otherTransform)
    {
        //otherTransform.GetChild(0).GetComponent<BoxCollider2D>().enabled = false;
        //otherTransform.GetChild(1).GetComponent<BoxCollider2D>().enabled = false;
        AlignParentOnCollision otherComponent = otherTransform.GetComponent<AlignParentOnCollision>();

        if (otherComponent != null)
        {
            // Merge groups of connected pieces
            foreach (var piece in otherComponent.parentDragAndDrop2D.connectedPieces)
            {
                if (!parentDragAndDrop2D.connectedPieces.Contains(piece))
                {
                    parentDragAndDrop2D.connectedPieces.Add(piece);
                }
                if (!piece.GetComponent<DragAndDrop2D>().connectedPieces.Contains(this.transform.parent))
                    piece.GetComponent<DragAndDrop2D>().connectedPieces.Add(this.transform.parent);
                    //print(piece.gameObject.name);
            }

            foreach (var piece in parentDragAndDrop2D.connectedPieces)
            {
                if (!otherComponent.parentDragAndDrop2D.connectedPieces.Contains(piece))
                {
                    otherComponent.parentDragAndDrop2D.connectedPieces.Add(piece);
                }
                if (!piece.GetComponent<DragAndDrop2D>().connectedPieces.Contains(this.transform.parent))
                    piece.GetComponent<DragAndDrop2D>().connectedPieces.Add(this.transform.parent);
                    //print(piece.gameObject.name);
            }

            // Synchronize the groups between the two objects
            //otherComponent.parentDragAndDrop2D.connectedPieces = parentDragAndDrop2D.connectedPieces;
        }

        if (!parentDragAndDrop2D.connectedPieces.Contains(otherTransform))
        {
            parentDragAndDrop2D.connectedPieces.Add(otherTransform);
            FourPiecePuzzleGameManager.instance.piecesCorrect++;
        }


        foreach (var transform in otherTransform.GetComponent<DragAndDrop2D>().connectedPieces)
        {
            if (!parentDragAndDrop2D.connectedPieces.Contains(transform))
            {
                parentDragAndDrop2D.connectedPieces.Add(transform);
                FourPiecePuzzleGameManager.instance.piecesCorrect++;
            }
            if (!transform.GetComponent<DragAndDrop2D>().connectedPieces.Contains(this.transform.parent))
                transform.GetComponent<DragAndDrop2D>().connectedPieces.Add(this.transform.parent);
                //print(transform.gameObject.name);
        }

        if (otherComponent != null)
        {
            foreach (var piece in parentDragAndDrop2D.connectedPieces)
            {
                if (!otherComponent.parentDragAndDrop2D.connectedPieces.Contains(piece))
                {
                    otherComponent.parentDragAndDrop2D.connectedPieces.Add(piece);
                }
                if (!piece.GetComponent<DragAndDrop2D>().connectedPieces.Contains(this.transform.parent))
                    piece.GetComponent<DragAndDrop2D>().connectedPieces.Add(this.transform.parent);
                    print(piece.gameObject.name);
            }
        }
    }

    //public void MoveGroup(Vector3 offset)
    //{
    //    print("Off Set: " + offset);
    //    // Move all connected pieces together
    //    foreach (var piece in connectedPieces)
    //    {
    //        piece.position += offset;
    //    }
    //}

}

using UnityEngine;

public class TowerOfPisaFloorSpawner : MonoBehaviour
{
    public Transform rope; // Reference to the rope transform
    private GameObject currentFloor;

    //void Update()
    //{
    //    if (Input.GetMouseButtonDown(0) && currentFloor != null)
    //    {
    //        DropFloor();
    //    }
    //    else if (currentFloor == null)
    //    {
    //        SpawnFloor();
    //    }
    //}

    //void SpawnFloor()
    //{
    //    GameObject floorPrefab = TheTowerOfPisaManager.Instance.GetNextFloorPrefab();
    //    if (floorPrefab != null)
    //    {
    //        // Create a wrapper GameObject at the rope's position
    //        GameObject wrapper = new GameObject("FloorWrapper");
    //        wrapper.transform.position = rope.position;

    //        // Set the wrapper as a child of the rope to follow its movement
    //        wrapper.transform.SetParent(rope, true);

    //        // Instantiate the floor and parent it to the wrapper
    //        currentFloor = Instantiate(floorPrefab, wrapper.transform.position, Quaternion.identity, wrapper.transform);

    //        // Reset the scale of the floor to its original size
    //        currentFloor.transform.localScale = new Vector3(0.6f,0.6f,0.6f);

    //        // Disable gravity initially
    //        Rigidbody2D rb = currentFloor.GetComponent<Rigidbody2D>();
    //        if (rb != null)
    //        {
    //            rb.gravityScale = 0;
    //        }
    //    }
    //}
    //void DropFloor()
    //{
    //    if (currentFloor != null)
    //    {
    //        // Detach the floor from its parent (wrapper)
    //        Transform parentTransform = currentFloor.transform.parent;

    //        if (parentTransform != null)
    //        {
    //            currentFloor.transform.parent = null; // Detach the floor
    //            Destroy(parentTransform.gameObject); // Destroy the wrapper only
    //        }

    //        // Enable gravity to drop the floor
    //        Rigidbody2D rb = currentFloor.GetComponent<Rigidbody2D>();
    //        if (rb != null)
    //        {
    //            rb.gravityScale = 1; // Enable gravity for the floor
    //        }
    //        else
    //        {
    //            Debug.LogWarning("Rigidbody2D not found on the current floor!");
    //        }

    //        currentFloor = null;
    //    }
    //}


}


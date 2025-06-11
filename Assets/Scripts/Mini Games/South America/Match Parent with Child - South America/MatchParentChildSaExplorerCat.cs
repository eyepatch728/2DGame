using System.Collections;
using UnityEngine;

public class MatchParentChildSaExplorerCat : MonoBehaviour
{
    public Animator animator;
    private GameObject babyAnimal;
    [Header("Balloon Animation Settings")]
    public Transform dropPosition; // The position where the baby animal will be dropped
    public Transform upPosition;   // The position the balloon moves back to after dropping
    public static MatchParentChildSaExplorerCat instance;
    private void Awake()
    {
        instance = this;
        animator = GetComponent<Animator>();
    }
    public Coroutine deliveryCoroutine;
    public void DeliverBaby(GameObject baby, MatchParentChildSaManager manager)
    {
        babyAnimal = baby; // Assign the baby animal to be delivered
        deliveryCoroutine = StartCoroutine(HandleDelivery(manager));
    }

    private IEnumerator HandleDelivery(MatchParentChildSaManager manager)
    {
        // Step 1: Attach the baby animal to the balloon
        if (babyAnimal != null)
        {
            babyAnimal.transform.SetParent(transform.GetChild(3)); // Make the baby a child of the balloon
            babyAnimal.transform.localPosition = new Vector3(0, 2, 0); // Adjust position relative to the balloon
        }
        // Step 1: Play balloon descent animation
        animator.SetBool("isDown", true);
        yield return new WaitForSeconds(0.1f); // Assuming descent takes 1.5 seconds
        babyAnimal.transform.localScale = Vector3.one; // Adjust position relative to the balloon

        // Wait for the animation to complete (adjust based on your animation length)
        yield return new WaitForSeconds(1.5f); // Assuming descent takes 1.5 seconds

        // Step 2: Drop the baby at the drop position
        if (babyAnimal != null)
        {
            babyAnimal.transform.SetParent(null);
            babyAnimal.transform.position = dropPosition.position;

            // Enable dragging so the player can move the baby
            var babyScript = babyAnimal.GetComponent<AnimalBabyDraggable>();
            if (babyScript != null)
            {
                babyScript.EnableDragging(true);
            }
        }

        // Notify the manager that delivery is complete (if needed)
        if (manager != null)
        {
            //manager.OnCatDeliveryComplete();
        }

        // Step 3: Play balloon ascent animation
        yield return new WaitForSeconds(2f); // Brief delay before ascent
        animator.SetBool("isDown", false);

        // Wait for the ascent animation to complete
        yield return new WaitForSeconds(1.5f); // Assuming ascent takes 1.5 seconds

        // Step 4: Balloon has returned to its up position (ready for the next delivery)
        Debug.Log("Balloon has returned to its initial position.");
    }
}

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragDropHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Transform originalParent;
    private Vector3 originalPosition;
    private Canvas canvas;
    private RectTransform rectTransform;
    public int currGridLayer;
    public int currSiblingIndex;


    public Image[] correctPositions;

    void Start()
    {
        canvas = GetComponentInParent<Canvas>(); // Ensure proper UI dragging
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (AmazonRiverManager.Instance.isGameStart())
        {
            originalParent = transform.parent; // Store original parent (Grid)
            originalPosition = transform.position; // Store original position
            transform.SetParent(canvas.transform, true); // Temporarily move to canvas root
        }
       
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (AmazonRiverManager.Instance.isGameStart())
        {
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor; // Move with mouse

        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {



        SoundManager.instance.PlaySingleSound(0);
        if (AmazonRiverManager.Instance.isGameStart())
        {
            if (IsCorrectDropPosition())
            {
                rectTransform.SetParent(correctPositions[index].transform, false);
                rectTransform.position = correctPositions[index].transform.position;
                rectTransform.localScale = Vector3.one;
            }
            else
            {
                transform.SetParent(originalParent, false);
                rectTransform.anchoredPosition = originalPosition;
                rectTransform.localScale = Vector3.one;
            }

            // Check if dropped into a valid DropZone
            //DropZone dropZone = DropZone.currentDropZone;





            //////print("Dropped at: " + eventData.position);
            //////int newTargetGrid = AmazonRiverManager.Instance.GetRange(eventData.position);
            //////if (newTargetGrid != 5 && newTargetGrid != currGridLayer)
            //////{
            //////    transform.SetParent(AmazonRiverManager.Instance.transforms[newTargetGrid], false);
            //////    //transform.position = AmazonRiverManager.Instance.transforms[newTargetGrid].position;
            //////    currGridLayer = newTargetGrid;
            //////    currSiblingIndex = 0;
            //////    transform.SetSiblingIndex(currSiblingIndex);
            //////}
            //////else if (currGridLayer == newTargetGrid)
            //////{
            //////    transform.SetParent(AmazonRiverManager.Instance.transforms[currGridLayer], false);
            //////    transform.position = AmazonRiverManager.Instance.transforms[currGridLayer].position;
            //////    transform.SetSiblingIndex(currGridLayer);
            //////}
            //////else
            //////{
            //////    transform.SetParent(originalParent, false);
            //////    transform.position = originalPosition;
            //////}






            //if (dropZone != null)
            //{
            //    transform.SetParent(dropZone.transform, false); // Change parent to new grid
            //}
            //else
            //{
            //    // Return to original position if not dropped in a grid
            //    transform.SetParent(originalParent, false);
            //    transform.position = originalPosition;
            //}
        }

    }


    int index = 0;

    private bool IsCorrectDropPosition()
    {
        float minDistance = float.MaxValue;
        int closestIndex = -1;

        for (int i = 0; i < correctPositions.Length; i++)
        {
            float distance = Vector2.Distance(rectTransform.position, correctPositions[i].transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestIndex = i;
            }
        }

        if (minDistance < 50f)
        {
            index = closestIndex;
            return true;
        }

        return false;
    }

}

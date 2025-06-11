using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int ImageNumber; 
    private Vector2 initialPos; 
    public Image[] endingPosArray;

    public Image[] correctPositions;

    private RectTransform rectTransform;
    private bool isPlaced = false; 

    //private static int placedCount = 0; 
    private static int totalImages; 
    public GameObject cat; 

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        initialPos = rectTransform.anchoredPosition;

        //if (placedCount == 0)
            totalImages = AustraliaKoalaGameManager.Instance.currLevelTotalImageCount;
    }

    private void Update()
    {
        if (AustraliaKoalaGameManager.Instance.placedCount <= 1)
            totalImages = AustraliaKoalaGameManager.Instance.currLevelTotalImageCount;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {

        if (isPlaced) return;

    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isPlaced) return;

        rectTransform.position = eventData.position;
    }
    int index = 0;
    public void OnEndDrag(PointerEventData eventData)
    {
        if (isPlaced) return;

        if (IsCorrectDropPosition())
        {

            SoundManager.instance.PlaySingleSound(0);
            rectTransform.position = correctPositions[index].transform.position;
            correctPositions[index].GetComponent<GridBlocks>().isProhibitted = true;
            isPlaced = true;
            AustraliaKoalaGameManager.Instance.placedCount++;

            if (AustraliaKoalaGameManager.Instance.placedCount  == totalImages)
            {
                print("level end");

                SoundManager.instance.PlaySingleSound(4);
                MoveCat();
                AustraliaKoalaGameManager.Instance.placedCount = 0;
            }
        }
        else
        {
            rectTransform.anchoredPosition = initialPos;
        }
    }

    private bool IsCorrectDropPosition()
    {
        float distance = 0;
        for (int i = 0; i < correctPositions.Length; i++)
        {
            distance = Vector2.Distance(rectTransform.position, correctPositions[i].transform.position);
            if (distance < 50f)
            {
                if (correctPositions[i].GetComponent<GridBlocks>().isProhibitted)
                {
                    {
                        return false;
                    }
                }
                index = i;
                break;
            }
        }
        return distance < 50f;
    }

    private void MoveCat()
    {
        cat.GetComponent<Animator>().Play("CatWalking");
        if (cat == null)
        {
            Debug.LogError("Cat GameObject is not assigned in the Inspector!");
            return;
        }

        Vector3[] positions = new Vector3[endingPosArray.Length];
        for (int i = 0; i < endingPosArray.Length; i++)
        {
            positions[i] = endingPosArray[i].transform.position;
        }

        cat.transform.position = positions[0];

        cat.GetComponent<CatMovement>().MoveThroughPoints(positions);
    }

}

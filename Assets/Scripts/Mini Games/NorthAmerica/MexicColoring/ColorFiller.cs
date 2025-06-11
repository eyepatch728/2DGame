using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ColorFiller : MonoBehaviour, IPointerClickHandler
{
    static string FILLABLE_TAG = "Fillable";

    public Image coloredImage;
    private static Color? selectedColor = null;
    private static GameObject selectedColorObject = null;
    private static bool canAcceptInput = true;
    private void Awake()
    {
        if (coloredImage == null)
        {
            coloredImage = this.GetComponent<Image>();
        }
    }
    void Start()
    {
        
        // Register click handler for the entire canvas
        GameObject.Find("Canvas").AddComponent<GraphicRaycaster>();
        ResetAllColors();
        ResetSelectedColor();
    }

    public void OnPointerClick(PointerEventData eventData)
    {

        // This is a color selector - handle color selection
        selectedColor = coloredImage.color;

        // Highlight the selected color (optional visual feedback)
        if (selectedColorObject != null)
        {
            // Remove highlight from previously selected color
            selectedColorObject.transform.localScale = Vector3.one;
        }
        selectedColorObject = gameObject;
        transform.localScale = new Vector3(1.1f, 1.1f, 1.1f); // Scale up slightly to show selection
        SoundManager.instance.PlaySingleSound(1);
    }

    void Update()
    {
        if (!canAcceptInput) return;
        // Handle clicks when a color is selected
        if (selectedColor.HasValue && Input.GetMouseButtonDown(0))
        {
            // Raycast to check for fillable objects
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            foreach (RaycastResult result in results)
            {
                GameObject hitObject = result.gameObject;

                // Check if we hit a fillable object
                if (hitObject.CompareTag(FILLABLE_TAG))
                {
                    // Check if the clicked pixel is part of the visible area
                    if (IsPointOverVisiblePixel(hitObject, Input.mousePosition))
                    {
                        // Apply the selected color
                        hitObject.GetComponent<Image>().color = selectedColor.Value;
                        ColorGameManager.Instance.CheckAllFill();
                        break;
                    }
                }
            }
        }
    }

    private bool IsPointOverVisiblePixel(GameObject imageObject, Vector2 mousePosition)
    {
        RectTransform rectTransform = imageObject.GetComponent<RectTransform>();
        Image imageComponent = imageObject.GetComponent<Image>();
        Sprite sprite = imageComponent.sprite;

        // Convert mouse position to local position relative to the image's RectTransform
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, mousePosition, null, out Vector2 localPoint);

        // Convert local position to normalized position within the RectTransform
        Vector2 normalizedPoint = new Vector2(
            (localPoint.x + rectTransform.rect.width * rectTransform.pivot.x) / rectTransform.rect.width,
            (localPoint.y + rectTransform.rect.height * rectTransform.pivot.y) / rectTransform.rect.height
        );

        // Map normalized position to sprite's texture rectangle
        float texX = normalizedPoint.x * sprite.texture.width;
        float texY = normalizedPoint.y * sprite.texture.height;

        // Ensure texX and texY are within bounds
        texX = Mathf.Clamp(texX, 0, sprite.texture.width - 1);
        texY = Mathf.Clamp(texY, 0, sprite.texture.height - 1);

        // Sample the texture at the calculated coordinates
        Color pixelColor = sprite.texture.GetPixel((int)texX, (int)texY);
        return pixelColor.a > 0.1f; // Threshold for visible pixels
    }
    public void ResetSelectedColor()
    {
        selectedColor = null;
        if (selectedColorObject != null)
        {
            selectedColorObject.transform.localScale = Vector3.one;
            selectedColorObject = null;
        }
    }

    private void ResetAllColors()
    {
        GameObject[] fillableObjects = GameObject.FindGameObjectsWithTag(FILLABLE_TAG);

        foreach (GameObject obj in fillableObjects)
        {
            obj.GetComponent<Image>().color = Color.white; // Set to default color (white)
        }
    }
    public static void DisableInput() => canAcceptInput = false; // Disable input

    public static void EnableInput() => canAcceptInput = true; // Enable input
}

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems; 

public class SelectLayer : MonoBehaviour, IPointerDownHandler 
{
    public GameObject ContentPanel;
    private const int MaxChildCount = 4;
    public bool Iscoloring;

    // Do this when the mouse is clicked over the selectable object this script is attached to.
    public void OnPointerDown(PointerEventData eventData)
    {

        Debug.Log(this.gameObject.name + " Was Clicked.");
        // Check if ContentPanel already has the maximum number of children
        if (ContentPanel.transform.childCount == MaxChildCount&&SouthAfricaAmazonRiver.Instance.SelectedLayer!=null)
        {
            Debug.Log("ContentPanel has reached the maximum child limit. No further objects can be added.");
            SouthAfricaAmazonRiver.Instance.SelectedLayer = null;
            SouthAfricaAmazonRiver.Instance.SelectedBox = null;
            return;
        }

        if (ContentPanel.transform.childCount != 0)
        {

            // Check if no layer is selected
            if (SouthAfricaAmazonRiver.Instance.SelectedLayer == null)
            {
                // Assign the current layer as the selected layer
                SouthAfricaAmazonRiver.Instance.SelectedLayer = this.transform;
                SouthAfricaAmazonRiver.Instance.SelectedBox = this.transform.GetChild(0).GetChild(0);
            }
            else
            {
                // Check if the selected box is already inside the ContentPanel
                if (SouthAfricaAmazonRiver.Instance.SelectedBox.transform.parent == ContentPanel.transform)
                {
                    Debug.Log("The selected object is already inside the ContentPanel. No changes made.");
                    return;
                }

                // Get the child object of the selected layer
                GameObject gm = SouthAfricaAmazonRiver.Instance.SelectedBox.transform.gameObject;
                Debug.Log(gm.name, gm.gameObject);

                // Move the selected object to the ContentPanel
                gm.transform.parent = ContentPanel.transform;
                gm.transform.SetAsFirstSibling();

                // Adjust anchor to lower center
                SetAnchorToLowerCenter(ContentPanel.GetComponent<RectTransform>());

                // Clear selection
                SouthAfricaAmazonRiver.Instance.SelectedLayer = null;
                SouthAfricaAmazonRiver.Instance.SelectedBox = null;

            }
        }
        else if (SouthAfricaAmazonRiver.Instance.SelectedLayer != null && SouthAfricaAmazonRiver.Instance.SelectedBox != null)
        {
            // Get the child object of the selected layer
            GameObject gm = SouthAfricaAmazonRiver.Instance.SelectedBox.transform.gameObject;
            Debug.Log(gm.name, gm.gameObject);

            // Move the selected object to the ContentPanel
            gm.transform.parent = ContentPanel.transform;
            gm.transform.SetAsFirstSibling();

            // Adjust anchor to lower center
            SetAnchorToLowerCenter(ContentPanel.GetComponent<RectTransform>());

            // Clear selection
            SouthAfricaAmazonRiver.Instance.SelectedLayer = null;
            SouthAfricaAmazonRiver.Instance.SelectedBox = null;
        }
        if (ContentPanel.transform.childCount == MaxChildCount - 1)
        {
            string firstChildName = ContentPanel.transform.GetChild(0).name;
            bool allSameName = true;

            foreach (Transform child in ContentPanel.transform)
            {
                if (child.name != firstChildName)
                {
                    allSameName = false;
                    break;
                }
            }
            if (allSameName)
            {
                Debug.Log("All children have the same name: " + firstChildName);
                Iscoloring = true;

            }
            else
            {
                Debug.Log("Children have different names.");
            }
        }
        Debug.Log(ContentPanel.transform.childCount);

        SouthAfricaAmazonRiver.Instance.CheckState();

    }

    // Helper method to set the anchor to the lower center
    private void SetAnchorToLowerCenter(RectTransform rectTransform)
    {
        if (rectTransform == null) return;

        rectTransform.anchorMin = new Vector2(0.5f, 0); // Set the anchor to the middle-bottom
        rectTransform.anchorMax = new Vector2(0.5f, 0); // Set the anchor to the middle-bottom
        rectTransform.pivot = new Vector2(0.5f, 0);     // Adjust the pivot to match the anchor
        rectTransform.anchoredPosition = Vector2.zero;  // Ensure it sits at the lower center
    }
}

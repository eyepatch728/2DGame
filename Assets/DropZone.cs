//using UnityEngine;
//using UnityEngine.EventSystems;

//public class DropZone : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
//{
//    public static DropZone currentDropZone; // Stores the active drop zone

//    public void OnPointerEnter(PointerEventData eventData)
//    {
//        if (eventData.pointerDrag != null) // Ensure something is being dragged
//        {
//            currentDropZone = this; // Set this as the active drop zone
//        }
//    }

//    public void OnPointerExit(PointerEventData eventData)
//    {
//        if (currentDropZone == this)
//        {
//            currentDropZone = null; // Reset when leaving
//        }
//    }
//}

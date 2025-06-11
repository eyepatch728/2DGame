using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChinaFestivalUiManager : MonoBehaviour
{
    public static ChinaFestivalUiManager Instance;
    public GameObject popupPanel;
    public GameObject gameCompletePanel;
    public Image itemImage;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemDescriptionText;
    public float typewriterSpeed = 0.05f;  // Time between each character
    public float autoCloseDelay = 3f;
    private string currentDescription;
    private Coroutine typewriterCoroutine;
    private Coroutine autoCloseCoroutine;
    private bool isGameComplete = false;
    private void Awake()
    {
        Instance = this;
    }
    public void ShowItemPopup(ChinaFestivalsManager.FestivalItem item, bool isLastItem)
    {
        if (typewriterCoroutine != null)
            StopCoroutine(typewriterCoroutine);
        if (autoCloseCoroutine != null)
            StopCoroutine(autoCloseCoroutine);

        popupPanel.SetActive(true);
        itemImage.sprite = item.itemSprite;
        itemNameText.text = item.itemName;

        currentDescription = item.description;
        itemDescriptionText.text = "";
        typewriterCoroutine = StartCoroutine(TypewriterEffect(isLastItem));
    }
    private IEnumerator TypewriterEffect(bool isLastItem)
    {
        foreach (char letter in currentDescription)
        {
            itemDescriptionText.text += letter;
            yield return new WaitForSecondsRealtime(typewriterSpeed);
        }

        if (isLastItem)
        {
            yield return new WaitForSecondsRealtime(autoCloseDelay);
            ClosePopup();
            ShowGameComplete();
        }
        else
        {
            // For non-final items, proceed with normal auto-close
            autoCloseCoroutine = StartCoroutine(AutoClosePopup());
        }
    }

    private IEnumerator AutoClosePopup()
    {
        // Wait for specified delay
        yield return new WaitForSecondsRealtime(autoCloseDelay);
        ClosePopup();
    }
    public void ClosePopup()
    {
        if (typewriterCoroutine != null)
            StopCoroutine(typewriterCoroutine);
        if (autoCloseCoroutine != null)
            StopCoroutine(autoCloseCoroutine);
        popupPanel.SetActive(false);
        ChinaFestivalsManager.Instance.ResumeGame();
    }

    public void ShowGameComplete()
    {
        gameCompletePanel.SetActive(true);
        isGameComplete = true;
    }
}

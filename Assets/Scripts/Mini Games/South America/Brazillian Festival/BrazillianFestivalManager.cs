//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.SceneManagement;
//using TMPro;

//public class BrazillianFestivalManager : MonoBehaviour
//{
//    public static BrazillianFestivalManager Instance { get; private set; }

//    [System.Serializable]
//    public class FestivalItem
//    {
//        public string itemName;
//        public Sprite itemSprite;
//        [TextArea(3, 5)]
//        public string description;
//        public bool isCollected;
//        public AudioClip itemAudio;
//    }

//    public List<FestivalItem> festivalItems = new List<FestivalItem>();
//    public bool isGamePaused = false;
//    public GameObject itemsInfoPopUp;
//    //public GameObject gameCompletePanel;
//    public Image itemImage;
//    //public TMP_Text itemNameText;
//    public TMP_Text itemDescriptionText;
//    public float typewriterSpeed = 0.05f;  // Time between each character
//    public float autoCloseDelay = 3f;
//    private string currentDescription;
//    private Coroutine typewriterCoroutine;
//    private Coroutine autoCloseCoroutine;
//    private bool isGameComplete = false;
//    public bool gameActive = false;
//    [Header("Cat Animation")]
//    public Animator catAnimator; // Animator for the cat
//    public string isTalkingBool = "isTalking";
//    public AudioSource audioSource;
//    private void Awake()
//    {
//        if (Instance == null)
//        {
//            Instance = this;
//        }
//        else
//        {
//            Destroy(gameObject);
//        }
//    }
//    private void Start()
//    {
//        gameActive = false;
//    }
//    public void StartGame()
//    {
//        gameActive = true;

//    }
//    public void EndGame()
//    {
//        gameActive = false;
//        BrazilianPopups.Instance.ShowEndGamePopup();
//    }
//    public bool isGameStart()
//    {
//        return gameActive;
//    }
//    public void ShowItemPopup(BrazillianFestivalManager.FestivalItem item, bool isLastItem)
//    {
//        if (typewriterCoroutine != null)
//            StopCoroutine(typewriterCoroutine);
//        if (autoCloseCoroutine != null)
//            StopCoroutine(autoCloseCoroutine);

//        SoundManager.instance.PlaySingleSound(8);
//        itemsInfoPopUp.SetActive(true);
//        itemImage.sprite = item.itemSprite;
//        itemImage.SetNativeSize();
//        itemImage.preserveAspect = true;
//        //300,345
//        itemImage.rectTransform.sizeDelta = new Vector2(345f, 300f);
//        if (item.itemAudio != null)
//        {
//            //audioSource = GetComponent<AudioSource>(); // Get AudioSource attached to the GameManager (or assign it manually)
//            if (audioSource != null)
//            {
//                audioSource.clip = item.itemAudio;
//                audioSource.Play(); // Play the audio clip
//            }
//        }
//        //itemNameText.text = item.itemName;

//        currentDescription = item.description;
//        itemDescriptionText.text = "";
//        SetCatTalking(true);
//        typewriterCoroutine = StartCoroutine(TypewriterEffect(isLastItem));
//    }
//    private IEnumerator TypewriterEffect(bool isLastItem)
//    {
//        foreach (char letter in currentDescription)
//        {
//            itemDescriptionText.text += letter;
//            yield return new WaitForSecondsRealtime(typewriterSpeed);
//        }
//        SetCatTalking(false);
//        if (isLastItem)
//        {
//            yield return new WaitForSecondsRealtime(autoCloseDelay);
//            ClosePopup();
//            ShowGameComplete();
//        }
//        else
//        {
//            // For non-final items, proceed with normal auto-close
//            autoCloseCoroutine = StartCoroutine(AutoClosePopup());
//        }
//    }

//    private IEnumerator AutoClosePopup()
//    {
//        // Wait for specified delay
//        yield return new WaitForSecondsRealtime(autoCloseDelay);
//        if (audioSource != null && audioSource.isPlaying)
//        {
//            audioSource.Stop();
//        }
//        SetCatTalking(false);
//        ClosePopup();
//    }
//    public void ClosePopup()
//    {
//        if (typewriterCoroutine != null)
//            StopCoroutine(typewriterCoroutine);
//        if (autoCloseCoroutine != null)
//            StopCoroutine(autoCloseCoroutine);

//        SetCatTalking(false);
//        itemsInfoPopUp.SetActive(false);
//        ResumeGame();
//    }

//    public void ShowGameComplete()
//    {
//        //gameCompletePanel.SetActive(true);
//        isGameComplete = true;
//        EndGame();
//    }
//    public bool CheckGameComplete()
//    {
//        return festivalItems.TrueForAll(item => item.isCollected);
//    }

//    public void PauseGame()
//    {
//        isGamePaused = true;
//        //Time.timeScale = 0;
//    }

//    public void ResumeGame()
//    {
//        isGamePaused = false;
//        //Time.timeScale = 1;
//    }
//    public void BackToMainMenu()
//    {
//        SceneManager.LoadScene("MainMenu");
//    }
//    private void SetCatTalking(bool isTalking)
//    {
//        if (catAnimator)
//        {
//            catAnimator.SetBool(isTalkingBool, isTalking);
//        }
//    }
//}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class BrazillianFestivalManager : MonoBehaviour
{
    public static BrazillianFestivalManager Instance { get; private set; }

    [System.Serializable]
    public class FestivalItem
    {
        public string itemName;
        public Sprite itemSprite;
        [TextArea(3, 5)]
        public string description;
        public bool isCollected;
        public AudioClip itemAudio;
        public bool hasTriggeredPopup = false;
    }

    public List<FestivalItem> festivalItems = new List<FestivalItem>();
    public bool isGamePaused = false;
    public GameObject itemsInfoPopUp;
    public Image itemImage;
    public TMP_Text itemDescriptionText;
    public float typewriterSpeed = 0.05f;  // Time between each character
    public float autoCloseDelay = 3f;
    private string currentDescription;
    private Coroutine typewriterCoroutine;
    private Coroutine autoCloseCoroutine;
    private bool isGameComplete = false;
    public bool gameActive = false;

    [Header("Cat Animation")]
    public Animator catAnimator; // Animator for the cat
    public string isTalkingBool = "isTalking";
    public AudioSource audioSource;

    private bool isTyping = false;
    private bool canClose = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        gameActive = false;
    }

    public void StartGame()
    {
        gameActive = true;
    }

    public void EndGame()
    {
        gameActive = false;
        BrazilianPopups.Instance.ShowEndGamePopup();
    }

    public bool isGameStart()
    {
        return gameActive;
    }

    private void Update()
    {
        //itemImage.SetNativeSize();

        if (!itemsInfoPopUp.activeSelf) return;

        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            if (isTyping)
            {
                SkipTextAnimation();
            }
            else if (canClose)
            {
                ClosePopup();
                if (isLastItemm)
                {
                    ClosePopup();
                    ShowGameComplete();
                }
            }
        }
    }
    bool isLastItemm;
    public void ShowItemPopup(FestivalItem item, bool isLastItem)
    {
        if (item.hasTriggeredPopup) return; // NEW: Prevent re-triggering pop-up

        item.hasTriggeredPopup = true; // Mark as shown
        isLastItemm = isLastItem;

        if (typewriterCoroutine != null)
            StopCoroutine(typewriterCoroutine);
        if (autoCloseCoroutine != null)
            StopCoroutine(autoCloseCoroutine);

        SoundManager.instance.PlaySingleSound(8);
        itemsInfoPopUp.SetActive(true);
        itemImage.sprite = item.itemSprite;
        itemImage.SetNativeSize();
        if (item.itemName == "Guitar")
        {
            itemImage.transform.localScale *= 0.7f;
        }
        if (item.itemAudio != null && audioSource != null)
        {
            audioSource.clip = item.itemAudio;
            audioSource.Play();
        }

        currentDescription = item.description;
        itemDescriptionText.text = "";
        SetCatTalking(true);
        typewriterCoroutine = StartCoroutine(TypewriterEffect(isLastItem));
    }


    private IEnumerator TypewriterEffect(bool isLastItem)
    {
        isTyping = true;
        canClose = false;

        foreach (char letter in currentDescription)
        {
            itemDescriptionText.text += letter;
            yield return new WaitForSecondsRealtime(typewriterSpeed);
        }

        isTyping = false;
        canClose = true;
        SetCatTalking(false);

        if (isLastItem)
        {
            yield return new WaitForSecondsRealtime(autoCloseDelay);
            ClosePopup();
            ShowGameComplete();
        }
        else
        {
            autoCloseCoroutine = StartCoroutine(AutoClosePopup());
        }
    }

    private IEnumerator AutoClosePopup()
    {
        yield return new WaitForSecondsRealtime(autoCloseDelay);
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        SetCatTalking(false);
        ClosePopup();
    }

    public void ClosePopup()
    {
        if (typewriterCoroutine != null)
            StopCoroutine(typewriterCoroutine);
        if (autoCloseCoroutine != null)
            StopCoroutine(autoCloseCoroutine);

        SetCatTalking(false);
        itemsInfoPopUp.SetActive(false);
        ResumeGame();
    }

    public void ShowGameComplete()
    {
        isGameComplete = true;
        EndGame();
    }

    public bool CheckGameComplete()
    {
        return festivalItems.TrueForAll(item => item.isCollected);
    }

    public void PauseGame()
    {
        isGamePaused = true;
    }

    public void ResumeGame()
    {
        isGamePaused = false;
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void SetCatTalking(bool isTalking)
    {
        if (catAnimator)
        {
            catAnimator.SetBool(isTalkingBool, isTalking);
        }
    }

    private void SkipTextAnimation()
    {
        if (typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
        }

        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        itemDescriptionText.text = currentDescription;
        isTyping = false;
        canClose = true;
        SetCatTalking(false);
    }
}
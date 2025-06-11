using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AntarcticaPenguinPopups : MonoBehaviour
{
    public static AntarcticaPenguinPopups Instance;
    public GameObject popupPanel;
    public TMP_Text popupText;
    public GameObject startPopupImage;  // Start popup background image
    public GameObject endPopupImage;    // End popup background image
    public GameObject tutorialPanel;    // Tutorial panel to show how to play the game
    //public Animator tutorialAnimator;   // Animator component for the tutorial panel
    public string[] tutorialTexts;      // Text for the initial tutorial
    public string[] endGameTexts;       // Text for the post-game popup
    private int currentTextIndex = 0;
    public float typingSpeed = 0.07f;   // Speed of typing effect
    private System.Action onPopupComplete; // Callback action to be called after popup

    [Header("Cat Animation")]
    public Animator catAnimator; // Animator for the cat
    public string isTalkingBool = "isTalking";

    // Audio Clips
    public AudioClip startTextAudioClip;   // Audio clip to play at the start of text
    public AudioClip endTextAudioClip;     // Audio clip to play at the end of text
    public AudioSource audioSource;        // Audio source for playing clips
    private Coroutine typeTextCoroutine;
    private bool isTyping = false;
    private bool canClose = false;
    private bool isTutorialShown = false;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        ShowPopup(tutorialTexts, ShowTutorialPanel, true);
    }

    void Update()
    {
        if (!popupPanel.activeSelf || tutorialPanel.activeSelf) return;

        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            if (isTyping)
            {
                SkipTextAnimation();
            }
            else if (canClose)
            {
                ClosePopup();
            }
        }
    }

    public void ShowPopup(string[] texts, System.Action callback, bool isStartPopup)
    {
        if (isStartPopup && isTutorialShown) return;

        currentTextIndex = 0;
        popupPanel.SetActive(true);
        onPopupComplete = callback;

        startPopupImage.SetActive(isStartPopup);
        if (endPopupImage)
            endPopupImage.SetActive(!isStartPopup);

        isTyping = false;
        canClose = false;

        if (typeTextCoroutine != null)
            StopCoroutine(typeTextCoroutine);

        typeTextCoroutine = StartCoroutine(TypeText(texts, isStartPopup));
    }

    IEnumerator TypeText(string[] texts, bool isStartPopup)
    {
        if (audioSource != null)
        {
            if (isStartPopup && startTextAudioClip != null)
            {
                audioSource.clip = startTextAudioClip;
                audioSource.Play();
            }
            else if (!isStartPopup && endTextAudioClip != null)
            {
                audioSource.clip = endTextAudioClip;
                audioSource.Play();
            }
        }

        while (currentTextIndex < texts.Length)
        {
            popupText.text = "";
            SetCatTalking(true);
            isTyping = true;
            canClose = false;

            foreach (char letter in texts[currentTextIndex].ToCharArray())
            {
                popupText.text += letter;
                yield return new WaitForSeconds(typingSpeed);
            }

            isTyping = false;
            canClose = true;
            SetCatTalking(false);
            yield return new WaitForSeconds(1f);
            currentTextIndex++;
        }

        ClosePopup();
    }

    private void SkipTextAnimation()
    {
        if (typeTextCoroutine != null)
        {
            StopCoroutine(typeTextCoroutine);
        }

        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        if (currentTextIndex < (tutorialTexts.Length + endGameTexts.Length))
        {
            string[] currentTexts = tutorialTexts;
            if (currentTextIndex >= tutorialTexts.Length)
            {
                currentTexts = endGameTexts;
                currentTextIndex -= tutorialTexts.Length;
            }
            popupText.text = currentTexts[currentTextIndex];
        }

        isTyping = false;
        canClose = true;
        SetCatTalking(false);
    }

    private void ClosePopup()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        popupPanel.SetActive(false);
        if (onPopupComplete != null)
        {
            var callback = onPopupComplete;
            onPopupComplete = null;
            callback.Invoke();
        }
    }

    void ShowTutorialPanel()
    {
        isTutorialShown = true;
        tutorialPanel.SetActive(true);
        SoundManager.instance.PlaySingleSound(8);
        StartCoroutine(CloseTutorialAfterDelay(8f));
    }

    IEnumerator CloseTutorialAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (tutorialPanel.activeSelf)
        {
            tutorialPanel.SetActive(false);
            PenguinManager.Instance.StartGame();
        }
    }

    public void ShowEndGamePopup()
    {
        SoundManager.instance.PlaySingleSound(4);
        ShowPopup(endGameTexts, OnEndGamePopupComplete, false);
    }

    private void SetCatTalking(bool isTalking)
    {
        if (catAnimator)
        {
            catAnimator.SetBool(isTalkingBool, isTalking);
        }
    }

    void OnEndGamePopupComplete()
    {
        GlobalGameManager.instance.comingBackFromContinent = true;

        Debug.Log("End game popup finished!");
        SceneManager.LoadScene("MainMenu");
    }
}

using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TheAlpsPopUpsManager : MonoBehaviour
{
    public GameObject popupPanel;
    public TMP_Text popupText;
    public GameObject startPopupImage;
    public GameObject endPopupImage;
    public GameObject tutorialPanel;
    public string[] tutorialTexts;
    public string[] endGameTexts;
    private int currentTextIndex = 0;
    public float typingSpeed = 0.05f;
    private System.Action onPopupComplete;
    private bool isTyping = false;
    private bool firstTapSkipped = false;

    [Header("Cat Animation")]
    public Animator catAnimator;
    public string isTalkingBool = "isTalking";

    [Header("Audio Clips")]
    public AudioClip startTextAudioClip;
    public AudioClip[] endGameAudioClips;
    public AudioSource audioSource;

    [Header("End Game Images")]
    public GameObject[] endGameImages;

    [Header("Image Scaling Settings")]
    public Vector3 originalScale = new Vector3(1f, 1f, 1f);
    public Vector3 targetScale = new Vector3(1.2f, 1.2f, 1.2f);
    public float scaleDuration = 0.5f;

    void Start()
    {
        ShowPopup(tutorialTexts, ShowTutorialPanel, true);
    }

    private void Update()
    {
        if (popupPanel.activeSelf && Input.GetMouseButtonDown(0))
        {
            HandleSkipTap();
        }
    }

    public void ShowPopup(string[] texts, System.Action callback, bool isStartPopup)
    {
        currentTextIndex = 0;
        popupPanel.SetActive(true);
        onPopupComplete = callback;
        firstTapSkipped = false;

        startPopupImage.SetActive(isStartPopup);
        if (endPopupImage)
            endPopupImage.SetActive(!isStartPopup);

        StartCoroutine(TypeText(texts, isStartPopup));
    }

    IEnumerator TypeText(string[] texts, bool isStartPopup)
    {
        int lastIndex = texts.Length - 1;

        while (currentTextIndex < texts.Length)
        {
            popupText.text = "";
            SetCatTalking(true);
            isTyping = true;
            firstTapSkipped = false;

            if (!isStartPopup && currentTextIndex > 0 && currentTextIndex < lastIndex)
            {
                int imageIndex = (currentTextIndex == 1) ? 0 : currentTextIndex - 1;
                if (imageIndex < endGameImages.Length)
                {
                    StartCoroutine(ScaleImage(endGameImages[imageIndex]));
                }
            }

            if (!isStartPopup && endGameAudioClips.Length > currentTextIndex)
            {
                PlayAudioClip(endGameAudioClips[currentTextIndex]);
            }
            else if (isStartPopup && startTextAudioClip != null)
            {
                PlayAudioClip(startTextAudioClip);
            }

            foreach (char letter in texts[currentTextIndex].ToCharArray())
            {
                if (!isTyping)
                {
                    popupText.text = texts[currentTextIndex];
                    break;
                }

                popupText.text += letter;
                yield return new WaitForSeconds(typingSpeed);
            }

            isTyping = false;
            SetCatTalking(false);

            yield return new WaitForSeconds(1f);
            currentTextIndex++;
        }

        popupPanel.SetActive(false);
        onPopupComplete?.Invoke();
    }

    private void PlayAudioClip(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }

    private void HandleSkipTap()
    {
        if (isTyping)
        {
            StopAllCoroutines();
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
            popupText.text = currentTextIndex < tutorialTexts.Length ? tutorialTexts[currentTextIndex] : endGameTexts[currentTextIndex];
            isTyping = false;
            firstTapSkipped = true;
        }
        else if (firstTapSkipped)
        {
            firstTapSkipped = false;
            currentTextIndex++;
            StopAllCoroutines();
            popupPanel.SetActive(false);
            onPopupComplete?.Invoke();
        }
    }

    IEnumerator ScaleImage(GameObject imageObject)
    {
        if (imageObject == null) yield break;

        float timeElapsed = 0f;
        Transform imgTransform = imageObject.transform;

        imgTransform.localScale = originalScale;
        while (timeElapsed < scaleDuration)
        {
            imgTransform.localScale = Vector3.Lerp(originalScale, targetScale, timeElapsed / scaleDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        imgTransform.localScale = targetScale;

        yield return new WaitForSeconds(1f);

        timeElapsed = 0f;
        while (timeElapsed < scaleDuration)
        {
            imgTransform.localScale = Vector3.Lerp(targetScale, originalScale, timeElapsed / scaleDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        imgTransform.localScale = originalScale;
    }

    void ShowTutorialPanel()
    {
        tutorialPanel.SetActive(true);
        SoundManager.instance.PlaySingleSound(8);
        StartCoroutine(CloseTutorialAfterDelay(4f));
    }

    IEnumerator CloseTutorialAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        tutorialPanel.SetActive(false);
        TheAlpsMountainManager.Instance.StartGame();
    }

    private void SetCatTalking(bool isTalking)
    {
        if (catAnimator)
        {
            catAnimator.SetBool(isTalkingBool, isTalking);
        }
    }

    public void ShowEndGamePopup()
    {

        SoundManager.instance.PlaySingleSound(4);
        ShowPopup(endGameTexts, OnEndGamePopupComplete, false);
    }

    void OnEndGamePopupComplete()
    {
        GlobalGameManager.instance.comingBackFromContinent = true;

        Debug.Log("End game popup finished!");
        SceneManager.LoadScene("MainMenu");
    }
}

//using System.Collections;
//using TMPro;
//using UnityEngine;
//using UnityEngine.UI;

//public class MatchParentChildSaPopupController : MonoBehaviour
//{
//    public GameObject popupPanel;
//    public TMP_Text funFactText;
//    public Image animalSpriteImage;
//    [Header("Cat Animation")]
//    public Animator catAnimator; // Animator for the cat
//    public string isTalkingBool = "isTalking";
//    public AudioSource audioSource;
//    public AudioClip animalAudio;

//    [Header("Typewriter Effect")]
//    [SerializeField] private float typingSpeed = 0.05f;

//    private Coroutine typewriterCoroutine;
//    private void Start()
//    {
//        // Ensure panel is hidden at start
//        popupPanel.SetActive(false);
//    }

//    public void ShowPopup(string funFact, Sprite animalSprite, AudioClip audioClip)
//    {
//        if (MatchParentChildSaManager.Instance != null)
//        {
//            MatchParentChildSaManager.Instance.isGamePause = true;
//        }
//        if (typewriterCoroutine != null)
//        {
//            StopCoroutine(typewriterCoroutine);
//        }
//        funFactText.text = ""; // Clear text initially
//        animalSpriteImage.sprite = animalSprite;
//        animalAudio = audioClip;
//        animalSpriteImage.SetNativeSize();
//        RectTransform rectTransform = animalSpriteImage.rectTransform;
//        rectTransform.sizeDelta = new Vector2(350f, 350f);

//        popupPanel.SetActive(true);

//        typewriterCoroutine = StartCoroutine(TypewriterEffect(funFact));

//        // Play animal audio if available
//        if (audioSource != null && animalAudio != null)
//        {
//            audioSource.clip = animalAudio;
//            audioSource.Play();
//        }
//        if (catAnimator != null)
//        {
//            catAnimator.SetBool(isTalkingBool, true);
//        }
//        //Invoke("HidePopup", 3f);
//    }

//    private IEnumerator TypewriterEffect(string textToType)
//    {
//        // Create a temporary AudioSource for typing sounds if needed

//        foreach (char c in textToType)
//        {
//            funFactText.text += c;
//            yield return new WaitForSeconds(typingSpeed);
//        }
//        // Stop cat animation when typing is complete
//        if (catAnimator != null)
//        {
//            catAnimator.SetBool(isTalkingBool, false);
//        }

//        // Wait for the display duration after typing is complete
//        yield return new WaitForSeconds(3f);

//        HidePopup();
//        if (MatchParentChildSaManager.Instance != null)
//        {
//            MatchParentChildSaManager.Instance.isGamePause = false;
//            MatchParentChildSaManager.Instance.PrepareNextRound();
//        }
//    }

//    private void HidePopup()
//    {
//        // Stop any ongoing typewriter effect
//        if (typewriterCoroutine != null)
//        {
//            StopCoroutine(typewriterCoroutine);
//            typewriterCoroutine = null;
//        }

//        popupPanel.SetActive(false);
//    }

//}







using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MatchParentChildSaPopupController : MonoBehaviour
{
    public GameObject popupPanel;
    public TMP_Text funFactText;
    public Image animalSpriteImage;

    [Header("Cat Animation")]
    public Animator catAnimator;
    public string isTalkingBool = "isTalking";
    public AudioSource audioSource;
    public AudioClip animalAudio;

    [Header("Typewriter Effect")]
    [SerializeField] private float typingSpeed = 0.05f;

    private Coroutine typewriterCoroutine;
    private bool isTyping = false;
    private bool canClose = false;
    //private bool isPopupActive = false;
    private int lastTouchCount = 0;
    float lastTimeDone = 0f;
    private void Start()
    {
        popupPanel.SetActive(false);
    }

    private void Update()
    {
        if (!popupPanel.activeSelf)
        {
            lastTouchCount = 0;
            lastTimeDone = 0f;
            return;
        }

        int currentTouchCount = Input.touchCount;
        if (Input.GetMouseButtonDown(0) || (currentTouchCount > 0 && lastTouchCount == 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            if (isTyping)
            {
                SkipTextAnimation();
            }
            else if (canClose && lastTimeDone <= 0f)
            {
                lastTimeDone = 2f;
                HidePopup();
            }
        }
        lastTouchCount = currentTouchCount;
        lastTimeDone -= Time.deltaTime;
    }

    public void ShowPopup(string funFact, Sprite animalSprite, AudioClip audioClip)
    {
        if (MatchParentChildSaManager.Instance != null)
        {
            MatchParentChildSaManager.Instance.isGamePause = true;
        }

        if (typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
        }

        funFactText.text = "";
        animalSpriteImage.sprite = animalSprite;
        animalAudio = audioClip;
        animalSpriteImage.SetNativeSize();
        RectTransform rectTransform = animalSpriteImage.rectTransform;
        rectTransform.sizeDelta = new Vector2(350f, 350f);

        popupPanel.SetActive(true);
        //isPopupActive = true;
        isTyping = true;
        canClose = false;
        lastTouchCount = 0;

        typewriterCoroutine = StartCoroutine(TypewriterEffect(funFact));
    }

    private IEnumerator TypewriterEffect(string textToType)
    {
        currentFullText = textToType;
        if (audioSource != null && animalAudio != null)
        {
            audioSource.clip = animalAudio;
            audioSource.Play();
        }
        if (catAnimator != null)
        {
            catAnimator.SetBool(isTalkingBool, true);
        }

        foreach (char c in textToType)
        {
            funFactText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
        if (!popupPanel.activeSelf)
        {
            if (typewriterCoroutine != null)
            {
                StopCoroutine(typewriterCoroutine);
            }
        }

        isTyping = false;
        canClose = true;

        if (catAnimator != null)
        {
            catAnimator.SetBool(isTalkingBool, false);
        }
    }
    string currentFullText;
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

        // Instantly complete the current text
        funFactText.text = currentFullText;

        isTyping = false;
        canClose = true;

        if (catAnimator != null)
        {
            catAnimator.SetBool(isTalkingBool, false);
        }
    }


    private void HidePopup()
    {
        if (!popupPanel.activeSelf || lastTimeDone<=0f)
        {
            lastTouchCount = 0;
            lastTimeDone = 0f;
            return;
        }
        if (typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
            typewriterCoroutine = null;
        }
        popupPanel.SetActive(false);
        lastTouchCount = 0;
        if (MatchParentChildSaManager.Instance != null)
        {
            //if (MatchParentChildSaExplorerCat.instance.deliveryCoroutine != null)
            //{
            //StopCoroutine(MatchParentChildSaExplorerCat.instance.deliveryCoroutine);
            //StopAllCoroutines();
            //}
            //MatchParentChildSaExplorerCat.instance.animator.Play("default");
            //MatchParentChildSaExplorerCat.instance.animator.SetBool("isDown", false);
            MatchParentChildSaManager.Instance.explorerCatPrefab.transform.position = new Vector3(0, 8.3f, 0f);
            MatchParentChildSaManager.Instance.isGamePause = false;
            MatchParentChildSaManager.Instance.isTransitioning = false;
            //if(MatchParentChildSaManager.Instance.babyAnimalCoroutine != null)
            //    StopCoroutine(MatchParentChildSaManager.Instance.babyAnimalCoroutine);
            MatchParentChildSaManager.Instance.PrepareNextRound();
        }
    }
}

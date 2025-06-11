using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class AntarcticaAnimalPaintingManager : MonoBehaviour
{
    public static AntarcticaAnimalPaintingManager Instance { get; private set; }
    [SerializeField] private List<LevelSO> levels;
    public GameObject parentFillable;
    public GameObject fillableImage;
    public GameObject finalImage;
    public GameObject panelOnComplete;
    public Image panelOnCompleteImage;
    public Button continueButton;
    public TMP_Text descriptionText; // UI Text for displaying dialogue

    [Header("Cat Animation & Audio")]
    public Animator catAnimator; // Animator for the cat
    public string isTalkingBool = "isTalking";
    public AudioSource audioSource;

    public int currentIndex = 0;
    private List<GameObject> allFillable;
    public bool gameActive = false;
    private bool isTyping = false;
    private bool canClosePanel = false;
    private Coroutine typewriterCoroutine;
    private int currentDescriptionIndex = 0;
    private void Awake()
    {
        Instance = this;
        allFillable = new List<GameObject>();
    }

    private void Start()
    {
        gameActive = false;
        SetUpLevel();
    }
    private void Update()
    {
        if (panelOnComplete.activeSelf && Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                // Skip typewriter effect
                StopCoroutine(typewriterCoroutine);
                if (audioSource.isPlaying)
                {
                    audioSource.Stop();
                }
                CompleteAllText();
                isTyping = false;
                canClosePanel = true;
            }
            else if (canClosePanel)
            {
                // Close panel and proceed immediately
                panelOnComplete.SetActive(false);
                StartNewLevel(true);
            }
        }
    }
    public void StartGame()
    {
        gameActive = true;
        continueButton.onClick.AddListener(Continue);
    }

    public void EndGame()
    {
        gameActive = false;
        AntarcticaColoringPopups.instance.ShowEndGamePopup();
    }

    public bool isGameStart()
    {
        return gameActive;
    }
    private void CompleteAllText()
    {
        List<string> descriptions = levels[currentIndex].description;
        string fullText = string.Join("\n\n", descriptions);
        descriptionText.text = fullText;

        if (catAnimator != null)
        {
            catAnimator.SetBool(isTalkingBool, false);
        }
    }
    private void SetUpLevel()
    {
        allFillable.Clear();
        foreach (Transform child in parentFillable.transform)
        {
            Destroy(child.gameObject);
        }
        AntarcticaColorFiller colorFiller = FindObjectOfType<AntarcticaColorFiller>();
        if (colorFiller != null)
        {
            colorFiller.ResetSelectedColor();
        }
        int i = 0;
        foreach (var sprite in levels[currentIndex].fillableImages)
        {
            int k = i;
            GameObject imageObject = Instantiate(fillableImage);
            imageObject.tag = "Fillable";
            imageObject.transform.SetParent(parentFillable.transform);
            imageObject.GetComponent<FillableImage>().targetColor = levels[currentIndex].fillableImagesColors[k];

            RectTransform rectTransform = imageObject.GetComponent<RectTransform>();
            rectTransform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            rectTransform.anchoredPosition = Vector2.zero;

            if (sprite.rect.width > 700)
            {
                rectTransform.sizeDelta = GetReducedSize(sprite, 0.8f);
            }
            else
            {
                rectTransform.sizeDelta = GetReducedSize(sprite, 1);
            }

            Image imageComponent = imageObject.GetComponent<Image>();
            imageComponent.sprite = sprite;
            imageComponent.color = Color.white;

            allFillable.Add(imageObject);
            i++;
        }

        finalImage.GetComponent<Image>().sprite = levels[currentIndex].finalImage;
        continueButton.gameObject.SetActive(false);
        panelOnComplete.SetActive(false);
    }

    public Vector2 GetReducedSize(Sprite sprite, float scaleFactor)
    {
        float reducedWidth = (sprite.rect.width * scaleFactor) / 2;
        float reducedHeight = (sprite.rect.height * scaleFactor) / 2;
        return new Vector2(reducedWidth, reducedHeight);
    }

    public void CheckAllFill()
    {
        bool isAllFilled = true;
        int i = 0;

        foreach (var fillable in allFillable)
        {
            i++;
            if (fillable.GetComponent<Image>().color == Color.white || !fillable.GetComponent<FillableImage>().isFilled)
            {
                isAllFilled = false;
                break;
            }
        }

        if (currentIndex == 2 && i > levels[currentIndex].fillableImages.Count - 1)
        {
            Debug.Log("Hard Coded Check! ");
            isAllFilled = true;
        }

        if (isAllFilled)
        {
            continueButton.gameObject.SetActive(true);
            SoundManager.instance.PlaySingleSound(4);
            Debug.Log("All Images are filled");
        }
        else
        {
            Debug.Log("Images are left to be filled!");
        }
    }

    public void Continue()
    {
        AntarcticaColorFiller.DisableInput();
        panelOnComplete.SetActive(true);
        panelOnCompleteImage.sprite = levels[currentIndex].panelImage;
        currentDescriptionIndex = 0;
        StartCoroutine(PlayDescriptionWithAudio());
    }

    private IEnumerator PlayDescriptionWithAudio()
    {
        List<string> descriptions = levels[currentIndex].description;
        List<AudioClip> audioClips = levels[currentIndex].AudioClip;

        for (int i = 0; i < descriptions.Count; i++)
        {
            currentDescriptionIndex = i;
            isTyping = true;
            canClosePanel = false;

            if (catAnimator != null)
            {
                catAnimator.SetBool(isTalkingBool, true);
            }

            typewriterCoroutine = StartCoroutine(TypewriterEffect(descriptions[i],
                audioClips != null && i < audioClips.Count ? audioClips[i] : null));
            yield return typewriterCoroutine;

            if (catAnimator != null)
            {
                catAnimator.SetBool(isTalkingBool, false);
            }
        }

        isTyping = false;
        canClosePanel = true;

        // Wait for a brief moment before auto-proceeding
        yield return new WaitForSeconds(2.0f);

        // If the panel is still active (user hasn't clicked), proceed automatically
        if (panelOnComplete.activeSelf)
        {
            panelOnComplete.SetActive(false);
            StartNewLevel(false);
        }
    }

    private IEnumerator TypewriterEffect(string text, AudioClip audioClip)
    {
        descriptionText.text = "";
        float typingSpeed = 0.05f;

        if (audioClip != null)
        {
            audioSource.clip = audioClip;
            audioSource.Play();
        }

        foreach (char letter in text.ToCharArray())
        {
            descriptionText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        if (audioClip != null)
        {
            yield return new WaitForSeconds(audioClip.length - (text.Length * typingSpeed));
        }
        else
        {
            yield return new WaitForSeconds(2.0f);
        }
    }

    private void StartNewLevel(bool immediate = false)
    {
        currentIndex++;
        if (currentIndex < levels.Count)
        {
            if (immediate)
            {
                // Immediate transition
                SetUpLevel();
                AntarcticaColorFiller colorFiller = FindObjectOfType<AntarcticaColorFiller>();
                if (colorFiller != null)
                {
                    colorFiller.ResetSelectedColor();
                }
                AntarcticaColorFiller.EnableInput();
                if (LevelColors.instance != null)
                {
                    LevelColors.instance?.SetCurrentLevelColors(currentIndex);
                }
            }
            else
            {
                // Delayed transition
                SetUpLevel();
                AntarcticaColorFiller colorFiller = FindObjectOfType<AntarcticaColorFiller>();
                if (colorFiller != null)
                {
                    colorFiller.ResetSelectedColor();
                }
                AntarcticaColorFiller.EnableInput();
                if (LevelColors.instance != null)
                {
                    LevelColors.instance?.SetCurrentLevelColors(currentIndex);
                }
            }
        }
        else
        {
            if (immediate)
            {
                EndGame();
            }
            else
            {
                Invoke(nameof(EndGame), 2f);
            }
        }
    }
    public void Back()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

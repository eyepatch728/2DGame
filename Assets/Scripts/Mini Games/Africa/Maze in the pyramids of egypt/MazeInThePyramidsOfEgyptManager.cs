using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class MazeInThePyramidsOfEgyptManager : MonoBehaviour
{
    public static MazeInThePyramidsOfEgyptManager Instance { get; private set; }

    [Header("Game Configuration")]
    public int currentLevel = 1;
    public int totalLevels = 5;
    public int collectedBottles = 0;
    public int totalBottles = 5;

    [Header("UI References")]
    public Text levelText;
    public Text bottleCountText;
    public TextMeshProUGUI collectedBottlesText;
    public GameObject doorObject;
    public GameObject winPanel;
    public GameObject afterGameAnim;
    public GameObject[] levels;
    public GameObject[] levelGateLocked;
    public GameObject[] levelGateUnLocked;
    public bool moveLeft, moveRight, moveUp, moveDown;
    public List<string> facts = new List<string>();
    public List<AudioClip> factAudioClips = new List<AudioClip>();
    public List<Sprite> factImages = new List<Sprite>();
    private List<int> shownFacts = new List<int>();

    public GameObject InfoPopUp; // Reference to the Info Popup
    public TMP_Text facttext;
    public Image factimage;

    [Header("Cat Animation")]
    public Animator catAnimator; // Animator for the cat
    public string isTalkingBool = "isTalking";
    public AudioSource audioSource;
    public float typeSpeed = 0.05f;
    public bool gameActive = false;
    public Sprite spriteUp;
    public Sprite spriteDown;
    public Sprite spriteLeft;
    public Sprite spriteRight;

    private bool skipText = false; // Track if the player wants to skip the typing effect
    private bool isPopupActive = false; // Track if the popup is currently active

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        gameActive = false;
    }

    public void StartGame()
    {
        gameActive = true;
        InitializeGame();
    }

    public void EndGame()
    {
        gameActive = false;
        MazePyramidsPopups.Instance.ShowEndGamePopup();
    }

    public bool isGameStart()
    {
        return gameActive;
    }

    public void InitializeGame()
    {
        currentLevel = Random.Range(0, levels.Length);
        levels[currentLevel].SetActive(true);
        collectedBottles = 0;
        UpdateUI();
    }

    private void Update()
    {
        if (collectedBottlesText != null)
            collectedBottlesText.text = collectedBottles.ToString();

        if (moveDown)
        {
            MazeInThePyramidsOfEgyptPlayerController.instance.MoveDown();
        }
        if (moveUp)
        {
            MazeInThePyramidsOfEgyptPlayerController.instance.MoveUp();
        }
        if (moveLeft)
        {
            MazeInThePyramidsOfEgyptPlayerController.instance.MoveLeft();
        }
        if (moveRight)
        {
            MazeInThePyramidsOfEgyptPlayerController.instance.MoveRight();
        }
        if (!moveUp && !moveDown && !moveLeft && !moveRight && MazeInThePyramidsOfEgyptPlayerController.instance != null)
        {
            MazeInThePyramidsOfEgyptPlayerController.instance.StopUIMovement();
        }

        if (collectedBottles < 3)
        {
            levelGateLocked[currentLevel].SetActive(true);
            levelGateUnLocked[currentLevel].SetActive(false);
        }
        else
        {
            if (levelGateLocked[currentLevel] != null || levelGateUnLocked[currentLevel] != null)
            {
                levelGateLocked[currentLevel].SetActive(false);
                levelGateUnLocked[currentLevel].SetActive(true);
            }
        }

        // Handle skip text and close popup
        if (isGameStart() && isPopupActive && Input.GetMouseButtonDown(0))
        {
            if (!skipText)
            {
                skipText = true; // Skip the typing effect
            }
            else
            {
                ClosePopup(); // Close the popup
            }
        }
    }

    public void ShowFact()
    {
        InfoPopUp.SetActive(true);
        isPopupActive = true;
        skipText = false;

        if (facts.Count == 0 || facts.Count != factAudioClips.Count || facts.Count != factImages.Count)
        {
            Debug.LogWarning("Facts or audio clips are not properly set up!");
            return;
        }

        if (shownFacts.Count >= facts.Count)
        {
            // All facts have been shown; reset the list to allow repeats.
            shownFacts.Clear();
        }

        int newFactIndex;
        do
        {
            newFactIndex = Random.Range(0, facts.Count);
        } while (shownFacts.Contains(newFactIndex));

        shownFacts.Add(newFactIndex);

        // Display the image immediately
        if (factImages[newFactIndex] != null)
        {
            factimage.sprite = factImages[newFactIndex];
        }

        // Play the audio immediately
        if (audioSource != null && factAudioClips[newFactIndex] != null)
        {
            audioSource.clip = factAudioClips[newFactIndex];
            audioSource.Play();
        }

        // Start the typewriting effect with the selected fact
        StartCoroutine(TypeText(facts[newFactIndex]));
    }

    private IEnumerator TypeText(string text)
    {
        facttext.text = ""; // Clear the text before typing
        SetCatTalking(true);

        for (int i = 0; i < text.Length; i++)
        {
            if (skipText)
            {
                facttext.text = text; // Skip to the end of the text
                break;
            }
            facttext.text += text[i];
            yield return new WaitForSeconds(typeSpeed);
        }

        SetCatTalking(false);

        // Wait for the audio to finish playing before closing the popup
        while (audioSource != null && audioSource.isPlaying)
        {
            yield return null;
        }

        yield return new WaitForSeconds(1.5f); // Additional delay before closing
        ClosePopup();
    }

    private void ClosePopup()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        InfoPopUp.SetActive(false);
        isPopupActive = false;
    }

    private void SetCatTalking(bool isTalking)
    {
        if (catAnimator)
        {
            catAnimator.SetBool(isTalkingBool, isTalking);
        }
    }

    public void CollectBottle()
    {
        collectedBottles++;
        UpdateUI();

        if (collectedBottles >= totalBottles)
        {
            if (doorObject)
                doorObject.SetActive(true);
        }
    }

    public List<GameObject> btnslist = new List<GameObject>();
    public void CompleteLevel()
    {
        levels[currentLevel].SetActive(false);
        afterGameAnim.gameObject.SetActive(true);
        foreach (GameObject go in btnslist)
        {
            go.SetActive(false);
        }
        Invoke("EndGame", 1.5f);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void WinGame()
    {
        if (winPanel)
            winPanel.SetActive(true);
        Time.timeScale = 0; // Pause the game
    }

    public void UpdateUI()
    {
        if (levelText && bottleCountText)
        {
            levelText.text = $"Level: {currentLevel}";
            bottleCountText.text = $"Bottles: {collectedBottles}/{totalBottles}";
        }
    }

    public void MovePlayerVerticle(int upDown)
    {
        if (upDown == 0)
        {
            moveDown = false;
            moveUp = false;
        }

        if (upDown < 0)
        {
            moveDown = true;
        }
        else if (upDown > 0)
        {
            moveUp = true;
        }
    }

    public void MovePlayerHorizontal(int upDown)
    {
        if (upDown == 0)
        {
            moveLeft = false;
            moveRight = false;
        }

        if (upDown < 0)
        {
            moveLeft = true;
        }
        else if (upDown > 0)
        {
            moveRight = true;
        }
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
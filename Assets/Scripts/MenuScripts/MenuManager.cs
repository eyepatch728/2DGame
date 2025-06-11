using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour 
{
    [Header("Continent")]
    public List<ContinentData> continents;
    public List<GameObject> continentButtons = new List<GameObject>();
    private ContinentData selectedContinent;
    private List<string> visitedContinents = new List<string>();

    [Space]
    [Header("Mini Games")]
    public GameObject miniGamesPanel;
    public Transform miniGamesContainer;
    [Space]
    [Header("Other Panels")]
    public GameObject mainMenuPanel;
    [Space]
    [Header("Tutorial")]
    public Button tutorialButton;
    public TutorialManager tutorialManager;
    [Space]
    [Header("Back Button")]
    public Button backButton;

    public GameObject continentPopup;        // The popup panel
    public TMP_Text continentDescriptionText; // Text component for typewriter effect
    public AudioSource audioSource;          // Audio source for continent narration
    public float typewriterSpeed = 0.05f;    // Speed for the typewriter effect
    public Animator catAnimator;         // Reference to the player's Animator
    private readonly string isSpeakingBool = "isTalking"; // Name of the bool parameter
                                                          //public static bool returnToMiniGamesPanel = false;


    public GameObject popupPanel;
    [SerializeField] private TMP_Text textDescription;
    [Header("Cat Animation")]
    public Animator cat2Animator; // Animator for the cat
    public string isTalkingBool = "isTalking";
    [Header("Audio Settings")]
    public AudioSource audio2Source;
    public AudioClip[] audioClips;
    public string[] textSegments;
    public static bool isPopUpShown = false;


    public static MenuManager instance;
    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // Clear any previous state to ensure fresh start
        //PlayerPrefs.DeleteKey("SelectedContinentName");

        PlayerPrefs.GetInt("IsTutorialShown");
        if (PlayerPrefs.GetInt("IsTutorialShown") == 0)
        {
            tutorialManager.StartTutorial();
            PlayerPrefs.SetInt("IsTutorialShown", 1);
        }
        for (int i = 0; i < continents.Count; i++)
        {
            AssignContinentButton(continents[i], continentButtons[i]);
        }
        miniGamesPanel.SetActive(false);
        tutorialButton.onClick.AddListener(() => tutorialManager.StartTutorial());
        backButton.onClick.AddListener(BackToContinentSelection);
        bool comingBackFromMinigame = PlayerPrefs.GetInt("ComingBackFromMinigame", 0) == 1;
        // Check if we need to show the Mode Selection panel
        if (comingBackFromMinigame)
        {
            PlayerPrefs.SetInt("ComingBackFromMinigame", 0); // Reset after use
            PlayerPrefs.Save();

            this.selectedContinent = GetSelectedContinent(continents);

            if (this.selectedContinent != null)
            {
                ShowMiniGamesPanel(this.selectedContinent);
                string popupKey = "PopUpShown_" + this.selectedContinent.name;
                bool isPopupShownForContinent = PlayerPrefs.GetInt(popupKey, 0) == 1;

                Debug.Log("Popup Key: " + popupKey + ", Value: " + isPopupShownForContinent);

                if (!isPopupShownForContinent)
                {
                    popupPanel.SetActive(true);
                    StartCoroutine(PlayTypewriterEffect());

                    // Mark this continent's popup as shown
                    PlayerPrefs.SetInt(popupKey, 1);
                    PlayerPrefs.Save();
                }

                // Ensure the correct continent background music plays
                if (SoundManager.instance != null && selectedContinent.backgroundMusic != null)
                {
                    SoundManager.instance.ChangeMusic(selectedContinent.backgroundMusic);
                }
            }
            else
            {
                Debug.LogWarning("No continent selected");
            }
        }
        else
        {
            ShowMainMenuPanel(); // Show the normal Main Menu when reopening the app
        }
    }

    void ShowMiniGamesPanel(ContinentData continent)
    {
        miniGamesPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
        ShowMiniGames(continent); // Show the Mode Selection panel

    }
    private IEnumerator PlayTypewriterEffect()
    {
        Debug.Log($"Starting PlayTypewriterEffect with {textSegments.Length} segments and {audioClips.Length} audio clips");

        // Ensure audio source is available
        if (audio2Source == null)
        {
            audio2Source = SoundManager.instance.audioSource;
            Debug.Log("Retrieved audio source from SoundManager");
        }

        yield return new WaitForSeconds(0.1f); // Small initial delay to ensure everything is set up

        for (int i = 0; i < textSegments.Length; i++)
        {
            Debug.Log($"Processing segment {i}");
            textDescription.text = ""; // Clear text for the new segment

            // Handle audio first, before text
            if (i < audioClips.Length && audioClips[i] != null)
            {
                audio2Source.Stop();
                audio2Source.clip = audioClips[i];
                audio2Source.time = 0;
                audio2Source.volume = 1f;

                Debug.Log($"Playing audio clip {i}, Length: {audioClips[i].length}s");
                audio2Source.Play();
            }

            // Small delay to ensure audio starts properly
            yield return new WaitForSeconds(0.1f);

            // Start cat animation before text display
            cat2Animator.SetBool(isTalkingBool, true);

            // Display text
            foreach (char letter in textSegments[i])
            {
                textDescription.text += letter;
                yield return new WaitForSeconds(typewriterSpeed);
            }

            // Stop cat animation after text is fully displayed
            cat2Animator.SetBool(isTalkingBool, false);

            // Wait for current audio to finish
            if (audio2Source.isPlaying)
            {
                Debug.Log($"Waiting for audio clip {i} to finish");
                yield return new WaitForSeconds(audioClips[i].length);
            }
            else
            {
                Debug.Log($"No audio playing for segment {i}, waiting default time");
                //yield return new WaitForSeconds(1f);
            }

            // Add a pause between segments
            //yield return new WaitForSeconds(0.5f);
        }

        Debug.Log("Finished all segments");
        yield return new WaitForSeconds(1f);
        popupPanel.SetActive(false);
    }
    // Function to show the Main Menu Panel
    void ShowMainMenuPanel()
    {
        miniGamesPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
    public void SelectContinent(ContinentData continent)
    {

        selectedContinent = continent;
        PlayerPrefs.SetString("SelectedContinentName", selectedContinent.name);
        ShowMiniGames(selectedContinent);

    }
    public ContinentData GetSelectedContinent(List<ContinentData> allContinents)
    {
        string continentName = PlayerPrefs.GetString("SelectedContinentName", string.Empty);

        if (string.IsNullOrEmpty(continentName))
        {
            return null;  // No continent selected
        }

        // Find and return the continent with the matching name
        foreach (var continent in allContinents)
        {
            if (continent.name == continentName)
            {
                return continent;
            }
        }

        return null;  // Continent not found
    }

    void AssignContinentButton(ContinentData continent, GameObject continentBtn)
    {
        continentBtn.GetComponent<Button>().onClick.AddListener(() => SelectContinent(continent));
    }
    private void LoadBothGameScene(ContinentData continent)
    {
        string sceneName = "RunnerAndPuzzle";
        SceneManager.LoadSceneAsync(sceneName).completed += (asyncOperation) =>
        {
            // Ensure that the scene has loaded before trying to access components
            //RunnerGameManager runnerGameManager = FindFirstObjectByType<RunnerGameManager>();
            //if (runnerGameManager != null)
            //{
            //    runnerGameManager.SetLetters(continent.runnerGameLetters, continent.letterSprites);
            //}

            //// Similarly, get the PuzzleGameManager and set images
            //PuzzleGameManager puzzleGameManager = FindFirstObjectByType<PuzzleGameManager>();
            //if (puzzleGameManager != null)
            //{
            //    //puzzleGameManager.SetImages(continent.puzzleGameImages);
            //}
        };
    }

    private void ShowMiniGames(ContinentData continent)
    {
        // Deactivate all continent GameObjects in the container first
        foreach (Transform child in miniGamesContainer)
        {
            child.gameObject.SetActive(false); // Hide all continent panels
        }

        // Activate the selected continent's GameObject
        GameObject continentObject = miniGamesContainer.Find(continent.name)?.gameObject;

        if (continentObject == null)
        {
            //Debug.LogError($"Continent object '{continent.name}' not found in MiniGamesContainer.");
            return;
        }

        continentObject.SetActive(true);

        if (SoundManager.instance != null && continent.backgroundMusic != null)
        {
            SoundManager.instance.ChangeToContinentMusic(continent.backgroundMusic);
        }
        bool isFirstTime = !IsContinentOpened(continent.name);

        if (isFirstTime)
        {
            // First time opening the continent - perform specific actions here
            HandleFirstTimeContinentOpen(continent);
            MarkContinentAsOpened(continent.name); // Mark this continent as opened
        }
        else
        {
            // Assign listeners to the mini-game buttons within the continent GameObject
            print("Assigining Listeners to the mini games");
            foreach (Transform buttonTransform in continentObject.transform)
            {
                //Button button = buttonTransform.GetComponent<Button>();
                Button button = buttonTransform.GetComponentInChildren<Button>();
                if (button != null)
                {
                    MiniGameData miniGameData = button.GetComponent<MiniGameReference>().miniGameData;

                    if (miniGameData != null)
                    {
                        button.onClick.RemoveAllListeners(); // Remove any previous listeners
                        button.onClick.AddListener(() => StartMiniGame(miniGameData));
                    }
                    else
                    {
                        Debug.LogError("MiniGameData reference missing on button.");
                    }
                }
            }
        }

       

        // Show the mini-games panel and hide the main menu panel
        miniGamesPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
    }
    private void Update()
    {
        if(audioSource == null)
        {
            audioSource = SoundManager.instance?.audioSource;
        }
        if(audio2Source == null)
        {
            audio2Source = SoundManager.instance?.audioSource;
        }
    }
    private void HandleFirstTimeContinentOpen(ContinentData continent)
    {
        RunnerAndPuzzleManager.SelectedContinent = continent;
        // Specific functionality when a continent is opened for the first time.
        // For example, show a special message or animation.
        //Debug.Log($"Welcome to {continent.name}! This is your first time opening this continent.");
        string[] textSegments = {
        $"You've arrived on the continent of {continent.name}!",
        "Let's start discovering it.",
        "Collect all the letters and avoid the obstacles along the way!"

    };
        AudioClip[] audioClips = continent.audioClips; // Assuming this is an array of audio clips

        // Show the popup with typewriter text and multiple audio clips
        ShowContinentPopupWithAudio(textSegments, audioClips, "RunnerAndPuzzle");
        //SceneManager.LoadScene("RunnerAndPuzzle");
    }
    public void ShowContinentPopupWithAudio(string[] textSegments, AudioClip[] audioClips, string nextScene)
    {
        continentPopup.SetActive(true); // Show the popup

        // Ensure the number of text segments matches the number of audio clips
        if (textSegments.Length != audioClips.Length)
        {
            //Debug.LogError("Mismatch between text segments and audio clips!");
            return;
        }

        // Start the coroutine to play text and audio in sync
        StartCoroutine(PlayTextWithAudioAndLoadScene(textSegments, audioClips, nextScene));
    }

    private IEnumerator PlayTextWithAudioAndLoadScene(string[] textSegments, AudioClip[] audioClips, string nextScene)
    {
        //Debug.Log("Starting PlayTextWithAudioAndLoadScene coroutine");
        continentDescriptionText.text = "";

        for (int i = 0; i < textSegments.Length; i++)
        {
            //Debug.Log($"Starting segment {i}: {textSegments[i]}");
            string textSegment = textSegments[i];
            AudioClip audioClip = audioClips[i];

            // Set speaking to true before starting audio and text
            SetPlayerSpeaking(true);
            //Debug.Log($"Set speaking to TRUE for segment {i}");

            // Force update animator
            if (catAnimator != null)
            {
                catAnimator.Update(0f);
                //Debug.Log($"Current animation state - isTalking: {catAnimator.GetBool(isSpeakingBool)}");
            }

            // Play audio first
            if (audioSource != null && audioClip != null)
            {
                audioSource.clip = audioClip;
                audioSource.Play();
                //Debug.Log($"Playing audio clip {i}, length: {audioClip.length}s");
            }

            // Small delay to ensure animation and audio are synchronized
            yield return new WaitForSecondsRealtime(0.1f);

            // Clear text for new segment
            continentDescriptionText.text = "";

            // Typewriter effect
            foreach (char letter in textSegment)
            {
                continentDescriptionText.text += letter;
                // Double check animation state during typing
                if (catAnimator != null && !catAnimator.GetBool(isSpeakingBool))
                {
                    //Debug.LogWarning($"Animation turned off during typing in segment {i} - turning back on");
                    SetPlayerSpeaking(true);
                    catAnimator.Update(0f);
                }
                yield return new WaitForSecondsRealtime(typewriterSpeed);
            }

            // Wait for audio to complete
            if (audioSource != null && audioClip != null)
            {
                while (audioSource.isPlaying)
                {
                    // Keep checking animation state during audio playback
                    if (catAnimator != null && !catAnimator.GetBool(isSpeakingBool))
                    {
                        //Debug.LogWarning($"Animation turned off during audio in segment {i} - turning back on");
                        SetPlayerSpeaking(true);
                        catAnimator.Update(0f);
                    }
                    yield return null;
                }
            }

            //Debug.Log($"Finished segment {i}");

            // Set to idle after segment completes
            SetPlayerSpeaking(false);
            //Debug.Log($"Set speaking to FALSE after segment {i}");

            // Add pause between segments
            if (i < textSegments.Length - 1)
            {
                yield return new WaitForSecondsRealtime(0.5f);
            }
        }

        //Debug.Log("All segments complete, preparing to load scene");
        yield return new WaitForSecondsRealtime(0.5f);
        SceneManager.LoadScene(nextScene);
    }

    private void SetPlayerSpeaking(bool isSpeaking)
    {
        if (catAnimator != null)
        {
            //Debug.Log($"Setting isTalking to {isSpeaking}");
            catAnimator.SetBool(isSpeakingBool, isSpeaking);

            // Force immediate update after setting the bool
            catAnimator.Update(0f);

            // Verify the state was set correctly
            //bool currentState = catAnimator.GetBool(isSpeakingBool);
            //if (currentState != isSpeaking)
            //{
            //    Debug.LogError($"Failed to set animation state! Wanted: {isSpeaking}, Got: {currentState}");
            //}
        }
    }

    private bool IsContinentOpened(string continentName)
    {
        // Check if the continent has been marked as opened in PlayerPrefs
        return PlayerPrefs.GetInt(continentName, 0) == 1;
    }

    private void MarkContinentAsOpened(string continentName)
    {
        // Mark the continent as opened in PlayerPrefs
        PlayerPrefs.SetInt(continentName, 1);
        PlayerPrefs.Save(); // Ensure the data is saved immediately
    }
    public void BackToContinentSelection()
    {
        // Deactivate the currently active continent panel if one is selected
        if (selectedContinent != null)
        {
            GameObject continentObject = miniGamesContainer.Find(selectedContinent.name)?.gameObject;
            if (continentObject != null)
            {
                continentObject.SetActive(false);
            }
        }
        if (SoundManager.instance != null)
        {
            SoundManager.instance.ResetMiniGameMusic(); // Reset the pending mini-game music
            SoundManager.instance.ChangeMusic(SoundManager.instance.mainMenuMusic);
        }
        // Hide the mini-games panel and show the main menu panel (continent selection)
        miniGamesPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    //void CreateMiniGamesButton(MiniGameData miniGame)
    //{
    //    GameObject button = Instantiate(miniGamesButtonPrefab, miniGamesButtonContainer);
    //    button.GetComponentInChildren<TMP_Text>().text = miniGame.gameName;
    //    button.GetComponent<Button>().onClick.AddListener(() => StartMiniGame(miniGame));
    //    RectTransform rectTransform = button.GetComponent<RectTransform>();
    //    rectTransform.anchoredPosition = Vector2.zero; 
    //}
    private void StartMiniGame(MiniGameData miniGame)
    {
        Debug.Log($"Starting Mini-Game: {miniGame.gameName}");
        if (SoundManager.instance != null && miniGame.backgroundMusic != null)
        {
            SoundManager.instance.SetPendingMiniGameMusic(miniGame.backgroundMusic);
            // We don't need to do anything else since SetPendingMiniGameMusic now immediately plays the music
        }
        //returnToMiniGamesPanel = true;
        //PlayerPrefs.SetInt("LoadModeSelection", 1); // Save the state before switching scene
        PlayerPrefs.SetInt("ComingBackFromMinigame", 1);
        PlayerPrefs.Save();

        SceneManager.LoadSceneAsync(miniGame.sceneName);
    }
    public void BackToMainMenu() {
        SoundManager.instance.PlaySingleSound(1);
        SceneManager.LoadSceneAsync("MainMenu");
    }
}

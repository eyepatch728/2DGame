using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public GameObject explorerCat;
    public GameObject tutorialPanel;
    public TMP_Text tutorialText;
    public GameObject tutorialTextBg;
    public float overlayDuration = 2f;
    public GameObject[] continentObjects; // Array of continent GameObjects
    public Sprite[] numberSprites;
    public Image numberImage;
    public Button skipButton; // Button to skip the tutorial
    public float typingSpeed = 0.05f;
    public GameObject oceanOverlay; // Reference to the ocean overlay GameObject
    public float fadeDuration = 1f;
    public float scaleDuration = 0.5f; // Duration to smoothly scale up/down
    public Animator explorerCatAnimator;
    public AudioSource voiceOverSource;
    public AudioClip[] tutorialVoiceClips; // Array of voice clips matching text

    // List of continent names
    private List<string> continentNames = new List<string>
    {
        "North America", "South America", "Africa", "Europe", "Australia", "Asia", "Antarctica"
    };

    // List of counting numbers
    private List<string> countingNumbers = new List<string>
    {
        "One!", "Two!", "Three!", "Four!", "Five!", "Six!", "Seven!"
    };


    private Dictionary<GameObject, Vector3> originalPositions = new Dictionary<GameObject, Vector3>();
    private Dictionary<GameObject, Vector3> originalScales = new Dictionary<GameObject, Vector3>();
    private void Start()
    {
        if (PlayerPrefs.GetInt("IsTutorialShown") == 1)
        {
            //tutorialText.gameObject.SetActive(false);
        }
        StoreOriginalTransforms();
        numberImage.gameObject.SetActive(false);
        skipButton.onClick.AddListener(SkipTutorial);
        if (explorerCatAnimator != null)
        {
            explorerCatAnimator.SetBool("isTalking", false);
        }
    }


    private void StoreOriginalTransforms()
    {
        // Clear existing stored positions and scales
        originalPositions.Clear();
        originalScales.Clear();

        // Store the original position and scale of each continent
        foreach (GameObject continent in continentObjects)
        {
            originalPositions[continent] = continent.transform.localPosition;
            originalScales[continent] = continent.transform.localScale;
        }
    }

    private void RestoreOriginalTransforms()
    {
        // Restore each continent to its original position and scale
        foreach (GameObject continent in continentObjects)
        {
            if (originalPositions.ContainsKey(continent))
            {
                continent.transform.localPosition = originalPositions[continent];
                continent.transform.localScale = originalScales[continent];
            }
        }
    }

    private void Update()
    {
        if (voiceOverSource == null)
        {
            voiceOverSource = SoundManager.instance.sfxSource;
        }
    }
    public void StartTutorial()
    {
        RestoreOriginalTransforms();
        ResetTutorialState(); // Ensure everything starts fresh
        tutorialPanel.SetActive(true);
        tutorialTextBg.SetActive(true);
        MenuManager.instance.mainMenuPanel.SetActive(false);
        StartCoroutine(TutorialSequence());
    }

    private IEnumerator TutorialSequence()
    {
        explorerCat.SetActive(true);

        // Process overlays for each continent
        foreach (GameObject continent in continentObjects)
        {
            foreach (Transform child in continent.transform)
            {
                if (child.name == "overlay") // Check if the child is an overlay
                {
                    StartCoroutine(FadeOverlay(child.gameObject, true));
                }
            }
        }
        yield return new WaitForSeconds(0.2f);
        yield return DisplayText("Hello, little explorer! Did you know that most of our planet is covered by water? About 71% of Earth's surface is made up of oceans, rivers, and lakes!", 0);

        // Fade out overlays for each continent
        foreach (GameObject continent in continentObjects)
        {
            foreach (Transform child in continent.transform)
            {
                if (child.name == "overlay")
                {
                    StartCoroutine(FadeOverlay(child.gameObject, false));
                }
            }
        }

        yield return DisplayText("But there's also the land, where we live alongside many fascinating animals! This land is divided into seven continents, each with its own special and interesting things to discover.", 1);

        yield return DisplayText("Let's count the continents together!", 2);

        // Count the continents with a scale-up effect
        for (int i = 0; i < continentObjects.Length; i++)
        {
            yield return DisplayText(countingNumbers[i], i + 3); // Display the counting number

            // Special handling for Antarctica
            if (continentNames[i] == "Antarctica")
            {
                //yield return MoveContinentYAxis(continentObjects[i], 539.613892f); // Move Antarctica up
                yield return MoveContinentYAxis(continentObjects[i], 150f); // Move Antarctica up
            }

            yield return ScaleUpContinent(continentObjects[i]); // Apply scale-up effect

            // Move Antarctica back to its original position
            if (continentNames[i] == "Antarctica")
            {
                yield return MoveContinentYAxis(continentObjects[i], -400.114136f); // Move Antarctica back down
            }

            yield return new WaitForSeconds(overlayDuration); // Wait before the next continent
        }

        yield return DisplayText("Seven continents in total! They are:", 10);

        // Highlight each continent one by one
        for (int i = 0; i < continentObjects.Length; i++)
        {
            // Fade out the current continent overlay
            foreach (Transform child in continentObjects[i].transform)
            {
                if (child.name == "overlay")
                {
                    StartCoroutine(FadeOverlay(child.gameObject, false));
                }
            }

            // Fade in overlays of the remaining continents (exclude the current one)
            for (int j = 0; j < continentObjects.Length; j++)
            {
                if (j != i) // Skip the current continent
                {
                    foreach (Transform child in continentObjects[j].transform)
                    {
                        if (child.name == "overlay")
                        {
                            StartCoroutine(FadeOverlay(child.gameObject, true));
                        }
                    }
                    StartCoroutine(FadeOverlay(oceanOverlay, true));
                }
            }
            tutorialText.gameObject.SetActive(true);
            SoundManager.instance.PlaySFX(continentNames[i]);
            StartCoroutine(DisplayText(continentNames[i]));
            //yield return DisplayText(continentNames[i]);// Trigger smooth scale animation for continent
            //SoundManager.instance.PlaySFX(continentNames[i]);
            //yield return DisplayText(continentNames[i]);// Trigger smooth scale animation for continent

            DisplayNumber(i + 1); // Display the current number
            yield return AnimateContinent(continentObjects[i]);
            yield return new WaitForSeconds(overlayDuration);
        }

        tutorialTextBg.SetActive(true);
        yield return DisplayText("Each continent is unique! For example, Africa is known for its wild animals, and Antarctica is the coldest place on Earth. Doesn't that sound exciting?", 11);
        yield return DisplayText("Now it's your turn! Pick a continent you'd like to explore. All you have to do is tap on it. I'm curious to see what you'll choose!", 12);

        explorerCat.SetActive(false);
        tutorialPanel.gameObject.SetActive(false);
        MenuManager.instance.mainMenuPanel.SetActive(true);
    }

    private IEnumerator MoveContinentYAxis(GameObject continent, float targetY)
    {
        Vector3 startPosition = continent.transform.localPosition;
        Vector3 targetPosition = new Vector3(startPosition.x, targetY, startPosition.z);

        float elapsedTime = 0f;
        while (elapsedTime < scaleDuration)
        {
            continent.transform.localPosition = Vector3.Lerp(startPosition, targetPosition, elapsedTime / scaleDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        continent.transform.localPosition = targetPosition; // Ensure it reaches the final position
    }

    private IEnumerator ScaleUpContinent(GameObject continent)
    {
        Vector3 originalScale = continent.transform.localScale;
        Vector3 targetScale = originalScale * 1.2f; // Scale up by 20%

        float elapsedTime = 0f;
        while (elapsedTime < scaleDuration)
        {
            continent.transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / scaleDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Scale back to original size
        elapsedTime = 0f;
        while (elapsedTime < scaleDuration)
        {
            continent.transform.localScale = Vector3.Lerp(targetScale, originalScale, elapsedTime / scaleDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        continent.transform.localScale = originalScale; // Ensure it returns to the original scale
    }

    //private IEnumerator DisplayText(string message, int voiceIndex = -1)
    //{
    //    tutorialText.text = "";
    //    tutorialText.gameObject.SetActive(true);

    //    // Start talking animation
    //    if (explorerCatAnimator != null)
    //    {
    //        explorerCatAnimator.SetBool("isTalking", true);
    //    }

    //    // Stop any currently playing voice clip before playing a new one
    //    if (voiceOverSource.isPlaying)
    //    {
    //        voiceOverSource.Stop();
    //    }

    //    // Play corresponding voice clip if available
    //    if (voiceIndex >= 0 && voiceIndex < tutorialVoiceClips.Length)
    //    {
    //        voiceOverSource.clip = tutorialVoiceClips[voiceIndex];
    //        voiceOverSource.Play();
    //    }

    //    // Type text letter by letter
    //    foreach (char letter in message.ToCharArray())
    //    {
    //        tutorialText.text += letter;
    //        yield return new WaitForSeconds(typingSpeed);
    //    }

    //    // Wait until the voice-over finishes before proceeding
    //    while (voiceOverSource.isPlaying)
    //    {
    //        yield return null; // Wait until the clip finishes playing
    //    }

    //    // Add a short delay after voice-over finishes to prevent abrupt stopping
    //    yield return new WaitForSeconds(0.5f);

    //    // Stop talking animation AFTER text & voice-over are done
    //    if (explorerCatAnimator != null)
    //    {
    //        explorerCatAnimator.SetBool("isTalking", false);
    //    }

    //    // Hide number image after text is done
    //    numberImage.gameObject.SetActive(false);
    //}

    private IEnumerator DisplayText(string message, int voiceIndex = -1)
    {
        tutorialText.text = "";
        tutorialText.gameObject.SetActive(true);

        // Start talking animation BEFORE playing audio
        if (explorerCatAnimator != null)
        {
            explorerCatAnimator.SetBool("isTalking", true);
        }

        float delayPerCharacter = typingSpeed; // Default typing speed

        // Play corresponding voice clip if available
        if (voiceIndex >= 0 && voiceIndex < tutorialVoiceClips.Length)
        {
            voiceOverSource.clip = tutorialVoiceClips[voiceIndex];
            voiceOverSource.Play();

            // Dynamically adjust typing speed to match audio duration
            if (message.Length > 0 && voiceOverSource.clip.length > 0)
            {
                delayPerCharacter = voiceOverSource.clip.length / message.Length;
            }
        }

        // Type the text while audio plays
        for (int i = 0; i < message.Length; i++)
        {
            tutorialText.text += message[i];
            yield return new WaitForSeconds(delayPerCharacter);
        }

        // Ensure the talking animation stays active until the voice-over finishes
        while (voiceOverSource.isPlaying)
        {
            yield return null;
        }

        // Small delay for a natural transition before stopping talking animation
        yield return new WaitForSeconds(0.2f);

        if (explorerCatAnimator != null)
        {
            explorerCatAnimator.SetBool("isTalking", false);
        }

        // Hide number image after text is done
        numberImage.gameObject.SetActive(false);
    }




    private IEnumerator FadeOverlay(GameObject overlayObject, bool fadeIn)
    {
        CanvasGroup canvasGroup = overlayObject.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = overlayObject.AddComponent<CanvasGroup>();
        }

        float startAlpha = canvasGroup.alpha;
        float endAlpha = fadeIn ? 1f : 0f;
        float elapsedTime = 0f;

        overlayObject.SetActive(true);

        while (elapsedTime < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = endAlpha;

        if (!fadeIn)
        {
            overlayObject.SetActive(false);
        }
    }

    private IEnumerator AnimateContinent(GameObject continent)
    {
        Vector3 originalPosition = continent.transform.localPosition;
        int originalSiblingIndex = continent.transform.GetSiblingIndex();
        continent.transform.SetAsLastSibling();

        //// Store the original position of the continent
        //Vector3 originalPosition = continent.transform.localPosition;

        // Move the continent to the center of the canvas
        continent.transform.localPosition = Vector3.zero; // Set the position to the center of the canvas

        // Smoothly scale up the continent to 1.2 over the scaleDuration
        Vector3 initialScale = continent.transform.localScale;
        Vector3 targetScale = new Vector3(1.5f, 1.5f, 1.5f);
        float elapsedTime = 0f;

        while (elapsedTime < scaleDuration)
        {
            continent.transform.localScale = Vector3.Lerp(initialScale, targetScale, elapsedTime / scaleDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        continent.transform.localScale = targetScale; // Ensure it reaches the final scale

        // Wait for 1 or 2 seconds after scaling up
        yield return new WaitForSeconds(2f); // Adjust the duration as needed

        // Smoothly scale it back to 1
        elapsedTime = 0f;
        Vector3 targetScaleBack = new Vector3(1f, 1f, 1f);

        while (elapsedTime < scaleDuration)
        {
            continent.transform.localScale = Vector3.Lerp(targetScale, targetScaleBack, elapsedTime / scaleDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        continent.transform.localScale = targetScaleBack; // Ensure it reaches the final scale

        //// After scaling down, move the continent back to its original position
        //continent.transform.localPosition = originalPosition;
        continent.transform.localPosition = originalPosition;
        continent.transform.SetSiblingIndex(originalSiblingIndex);
        // Wait a little before introducing the next continent (if needed)
        yield return new WaitForSeconds(0.5f); // Optional wait time before the next continent
    }

    private void DisplayNumber(int number)
    {
        if (number > 0 && number <= numberSprites.Length)
        {
            numberImage.sprite = numberSprites[number - 1];
            numberImage.gameObject.SetActive(true);
        }
    }

    private void SkipTutorial()
    {
        StopAllCoroutines(); // Stop all running coroutines
        if (voiceOverSource.isPlaying)
        {
            voiceOverSource.Stop(); // Stop any playing audio
        }
        RestoreOriginalTransforms();

        tutorialPanel.SetActive(false);
        ResetTutorialState();
        MenuManager.instance.mainMenuPanel.SetActive(true);
    }

    private void ResetTutorialState()
    {
        // Reset the state of all continent overlays and the ocean overlay
        foreach (GameObject continent in continentObjects)
        {
            foreach (Transform child in continent.transform)
            {
                if (child.name == "overlay")
                {
                    child.gameObject.SetActive(false);
                    CanvasGroup canvasGroup = child.GetComponent<CanvasGroup>();
                    if (canvasGroup != null)
                    {
                        canvasGroup.alpha = 0f; // Ensure the overlay is fully transparent
                    }
                }
            }
        }

        // Reset ocean overlay
        oceanOverlay.SetActive(false);
        CanvasGroup oceanCanvasGroup = oceanOverlay.GetComponent<CanvasGroup>();
        if (oceanCanvasGroup != null)
        {
            oceanCanvasGroup.alpha = 0f; // Ensure the ocean overlay is fully transparent
        }

        // Reset UI elements
        tutorialText.text = "";
        tutorialText.gameObject.SetActive(false);
        numberImage.gameObject.SetActive(false);
        explorerCat.SetActive(false);
        if (explorerCatAnimator != null)
        {
            explorerCatAnimator.SetBool("isTalking", false);
        }
    }
}
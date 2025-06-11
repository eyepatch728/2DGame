using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FourPiecePuzzleGameManager : MonoBehaviour
{
    public static FourPiecePuzzleGameManager instance;
    public int piecesCorrect;
    private bool hasWon = false;
    public GameObject popupPanel;
    [SerializeField] private TMP_Text textDescription;
    [Header("Cat Animation")]
    public Animator catAnimator; // Animator for the cat
    public string isTalkingBool = "isTalking";
    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip[] audioClips;
    [SerializeField] private float typewriterSpeed = 0.05f;
    public string[] textSegments;
    public bool isDragging = false;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    void Update()
    {
        if (!hasWon && piecesCorrect >= 10)
        {
            hasWon = true; // Set flag to prevent multiple calls
            Debug.Log("You Win");

            if (PlayerPrefs.GetInt("ComingBackFromMinigame") != 1) // Prevent redundant writes
            {
                PlayerPrefs.SetInt("ComingBackFromMinigame", 1);
                PlayerPrefs.Save();
            }

            //StartCoroutine(LoadMainMenu());
            popupPanel.SetActive(true);
            StartCoroutine(PlayTypewriterEffect());
        }
    }
    public void Back()
    {
        SoundManager.instance.PlaySingleSound(1);
        SceneManager.LoadSceneAsync("MainMenu");
    }
    private IEnumerator PlayTypewriterEffect()
    {
        for (int i = 0; i < textSegments.Length; i++)
        {
            textDescription.text = ""; // Clear text for the new segment
            catAnimator.SetBool(isTalkingBool, true); // Start cat talking animation

            if (audioClips.Length > i && audioClips[i] != null)
            {
                audioSource.clip = audioClips[i];
                audioSource.Play();
            }

            foreach (char letter in textSegments[i])
            {
                textDescription.text += letter;
                yield return new WaitForSeconds(typewriterSpeed);
            }

            yield return new WaitForSeconds(1f); // Pause after each segment
        }

        catAnimator.SetBool(isTalkingBool, false); // Stop cat talking animation
        yield return new WaitForSeconds(1f); // Short delay before exiting
        SceneManager.LoadSceneAsync("MainMenu"); // Load Main Menu
    }
    private IEnumerator LoadMainMenu()
    {
        yield return new WaitForSeconds(1f); // Optional: Small delay for better UX
        SceneManager.LoadSceneAsync("MainMenu"); // Asynchronous loading for better performance
    }
}

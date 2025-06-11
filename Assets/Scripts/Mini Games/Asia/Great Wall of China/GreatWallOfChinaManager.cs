
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GreatWallOfChinaManager : MonoBehaviour
{

    public PuzzleSlot[] slots;
    public GameObject puzzleImage;
    public GameObject puzzleOutline;
    //public float imageScaleSpeed = 0.05f;
    public GameObject puzzleContainer;
    private bool isPuzzleComplete = false;
    public static GreatWallOfChinaManager Instance;
    public bool gameActive = false;
    //[Header("Cat Animation")]
    //public Animator catAnimator; // Animator for the cat
    //public string isTalkingBool = "isTalking";
    //public AudioSource audioSource;
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

    }
    public void EndGame()
    {

        gameActive = false;
        WallOfChinaPopups.Instance.ShowEndGamePopup();
    }
    public bool isGameStart()
    {
        return gameActive;
    }
    // Check if the puzzle is complete
    public void CheckPuzzleCompletion()
    {
        foreach (PuzzleSlot slot in slots)
        {
            if (!slot.IsPieceCorrectlyPlaced())
            {
                return; // Exit if any slot is incorrect
            }
        }

        CompletePuzzle();
    }

    // When puzzle is completed
    private void CompletePuzzle()
    {
        if (isPuzzleComplete) return;
        isPuzzleComplete = true;
        puzzleContainer.SetActive(false);
        puzzleOutline.SetActive(false);
        SoundManager.instance.PlaySingleSound(4);
        puzzleImage.SetActive(true);

        // Start enlarging the puzzle image
        StartCoroutine(ScaleImage());
    }

    // Gradually scale the image once puzzle is completed
    private IEnumerator ScaleImage()
    {
        Vector3 targetScale = new Vector3(0.6f, 0.6f, 0.6f);
        float scaleSpeed = 2f;
        float threshold = 0.01f; // To prevent infinite loops

        while (Vector3.Distance(puzzleImage.transform.localScale, targetScale) > threshold)
        {
            puzzleImage.transform.localScale = Vector3.Lerp(puzzleImage.transform.localScale, targetScale, scaleSpeed * Time.deltaTime);
            yield return null;
        }

        // Ensure the scale reaches exactly the target
        puzzleImage.transform.localScale = targetScale;

        yield return new WaitForSeconds(1f); // Optional delay

        // End the game after scaling completes
        EndGame();
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

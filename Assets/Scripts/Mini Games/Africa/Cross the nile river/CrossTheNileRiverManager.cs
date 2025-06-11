using UnityEngine;
using UnityEngine.SceneManagement;

public class CrossTheNileRiverManager : MonoBehaviour
{
    // Singleton instance
    public static CrossTheNileRiverManager Instance { get; private set; }
    public bool gameActive = false;
    [Header("Cat Animation")]
    public Animator catAnimator; // Animator for the cat
    public string isTalkingBool = "isTalking";
    public AudioSource audioSource;
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
        CorssTheNileRiverPopups.Instance.ShowEndGamePopup();
    }
    public bool isGameStart()
    {
        return gameActive;
    }
    public void GameOver()
    {
        // Logic for game over
        Debug.Log("Game Over!");
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        EndGame();
    }
    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

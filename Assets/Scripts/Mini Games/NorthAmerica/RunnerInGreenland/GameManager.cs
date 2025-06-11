using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public PopupManager PopupManager;
    public float gameDuration = 30f;  // 30 seconds game duration
    private bool gameActive = false;
    public GameObject player;
    public Animator playerAnimator;

    void Awake()
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
        playerAnimator = player.GetComponent<Animator>();
        playerAnimator.enabled = false;
    }
    public void StartGame()
    {
        gameActive = true;
        playerAnimator.enabled = true;
        Invoke("EndGame", gameDuration);
    }

    void EndGame()
    {
        gameActive = false;
        PopupManager.ShowEndGamePopup();
        // Logic for end game (e.g., transition to the next scene)
    }

    public bool IsGameActive()
    {
        return gameActive;
    }
    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

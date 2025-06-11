using UnityEngine;
using UnityEngine.SceneManagement;

public class TheAlpsMountainManager : MonoBehaviour
{
    public static TheAlpsMountainManager Instance;
    public TheAlpsPopUpsManager PopupManager;
    //public float gameDuration = 30f;  // 30 seconds game duration
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
        playerAnimator = player.GetComponent<Animator>();
        playerAnimator.enabled = false;
    }
    public void StartGame()
    {
        gameActive = true;
        playerAnimator.enabled = true;
        //Invoke("EndGame", gameDuration);
    }

    public void EndGame()
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

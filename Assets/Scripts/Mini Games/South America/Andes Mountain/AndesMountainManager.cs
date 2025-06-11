using UnityEngine;
using UnityEngine.SceneManagement;

public class AndesMountainManager : MonoBehaviour
{
    public static AndesMountainManager Instance;
    public bool gameActive = false;
    [Header("Cat Animation")]
    public Animator catAnimator; // Animator for the cat
    public string isTalkingBool = "isTalking";
    public AudioSource audioSource;
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
        AndesMountainPopups.Instance.ShowEndGamePopup();
    }
    public bool isGameStart()
    {
        return gameActive;
    }
    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

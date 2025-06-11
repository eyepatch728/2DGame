using UnityEngine;
using UnityEngine.SceneManagement;

public class EverestMountainManager : MonoBehaviour
{
    public static EverestMountainManager Instance;
    [SerializeField] private Transform startingBlock; // Reference to the first block
    [SerializeField] private Transform topBlock; // Reference to the highest block
    [SerializeField] private EverestMountainHeightTracker heightTracker;


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
        if (heightTracker != null)
        {
            heightTracker.SetStartAndEndBlocks(startingBlock, topBlock);
        }

    }
    public void EndGame()
    {
        gameActive = false;

        SoundManager.instance.PlaySingleSound(4);
        EverestMountainPopups.Instance.ShowEndGamePopup();
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

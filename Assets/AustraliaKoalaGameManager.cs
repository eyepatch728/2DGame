using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class AustraliaKoalaGameManager : MonoBehaviour
{

    public GameObject[] LevelArray;
    public int[] levelTotalImageCount;
    public int currLevelTotalImageCount;
    public int LevelValue =0;
    //public GameObject GameOver;
    public List<GridBlocks> GridBlocks;
    public static AustraliaKoalaGameManager Instance;
    public bool gameActive = false;
    public  int placedCount = 0;

    public bool canIdleAnim;
    //[Header("Cat Animation")]
    //public Animator catAnimator; // Animator for the cat
    //public string isTalkingBool = "isTalking";
    //public AudioSource audioSource;
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
        currLevelTotalImageCount = levelTotalImageCount[LevelValue];
    }
    public void StartGame()
    {
        gameActive = true;

    }
    public void EndGame()
    {
        gameActive = false;
        KoalaPopups.Instance.ShowEndGamePopup();
    }
    public bool isGameStart()
    {
        return gameActive;
    }
    // Update is called once per frame
    void Update()
    {
        if (isGameStart())
        {
            LevelCheck();

        }
        if (currLevelTotalImageCount < placedCount)
        {
            canIdleAnim = true; 
        }
    }

    public void NextLevel()
    {
        LevelValue++;
        foreach(GridBlocks gridBlock in GridBlocks)
        {
            gridBlock.isProhibitted = false;
        }
        currLevelTotalImageCount = levelTotalImageCount[LevelValue];
    }

    private void LevelCheck()
    {
        if (LevelValue == 0)
        {
            LevelArray[0].SetActive(true);
        }
        else if (LevelValue == 1)
        {

            LevelArray[0].SetActive(false);
            LevelArray[1].SetActive(true);
        }
        else if (LevelValue == 2)
        {
            LevelArray[1].SetActive(false);
            LevelArray[2].SetActive(true);
        }
        else if (LevelValue == 3)
        {
            LevelArray[2].SetActive(false);
            LevelArray[3].SetActive(true);
        }
        else if (LevelValue == 4)
        {
            LevelArray[3].SetActive(false);
            LevelArray[4].SetActive(true);
        }
        else if(LevelValue == 5)
        {
            EndGame();
            //GameEnd
        }
    }
    public void Back()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

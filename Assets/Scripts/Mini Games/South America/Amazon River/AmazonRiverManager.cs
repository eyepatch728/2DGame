using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AmazonRiverManager : MonoBehaviour
{
    public List<Transform> transforms = new List<Transform>();
    public List<Transform> transforms2 = new List<Transform>();

    public static AmazonRiverManager Instance;
    public GameObject blocker;
    public Animator playerAnimator;
    public bool gameActive = false;

    public GameObject multiColor;
    public GameObject greenColor;
    public GameObject levelEndImage;
    private Vector2 playerGameObjectPosition;
    public Vector2 currPlayerGameObjectPosition;

    public bool isLevel1Complete = false;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        playerGameObjectPosition = playerAnimator.gameObject.transform.position;
        gameActive = false;
    }
    public void StartGame()
    {
        gameActive = true;

    }
    public void EndGame()
    {
        gameActive = false;
        AmazonRiverPopups.Instance.ShowEndGamePopup();
    }
    public bool isGameStart()
    {
        return gameActive;
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }


    public bool AreAll3()
    {
        bool all3 = true;
        foreach (Transform t in transforms)
        {
            if (t.childCount != 3)
            {
                all3 = false;
                break;
            }
        }
        return all3;
    }

    public bool AreAll32()
    {
        bool all3 = true;
        foreach (Transform t in transforms2)
        {
            if (t.childCount != 3)
            {
                all3 = false;
                break;
            }
        }
        return all3;
    }

    private void Update()
    {
        currPlayerGameObjectPosition = playerAnimator.gameObject.GetComponent<RectTransform>().anchoredPosition;

        if (!isLevel1Complete)
        {
            if(AreAll3())
            {

                SoundManager.instance.PlaySingleSound(4);
                blocker.SetActive(true);
                StartCatWalking();
            }
            else
            {
                blocker.SetActive(false);
            }
            if (currPlayerGameObjectPosition.x > levelEndImage.GetComponent<RectTransform>().anchoredPosition.x - 100 && currPlayerGameObjectPosition.x < levelEndImage.GetComponent<RectTransform>().anchoredPosition.x + 100)
            {
                isLevel1Complete = true;
                startedWalking = false;
            }
        }
        else
        {
            if (AreAll32())
            {
                SoundManager.instance.PlaySingleSound(4);
                blocker.SetActive(true);
                StartCatWalking();
            }
            else
            {
                blocker.SetActive(false);
            }
        }

        

    }

    bool startedWalking = false;
    public void StartCatWalking()
    {
            

        if (startedWalking)
        {

            return;
        }
        if (isLevel1Complete)
        {
            Invoke("EndGame", 2f);
        }
        else
        {
            Invoke("CheckIdle",3.1f);
        }
        startedWalking = true;
        print("startedWalking " + startedWalking);
        playerAnimator.Play("RunAnimation");
        Invoke("MultiColorGamePlay", 3f);
        
    }

    //Give Current Postion of the object & Get it's Range: 880-1090, 1090- 1310, 1310-1530, 1530-1750, 1750-1970, 17150
    public int GetRange(Vector3 pos)
    {
        float position = pos.x;
        if (position >= 880 && position < 1090)
        {
            Debug.Log("Range: 880-1090");
            return 0;
        }
        else if (position >= 1090 && position < 1310)
        {
            Debug.Log("Range: 1090-1310");
            return 1;
        }
        else if (position >= 1310 && position < 1530)
        {
            Debug.Log("Range: 1310-1530");
            return 2;
        }
        else if (position >= 1530 && position < 1750)
        {
            Debug.Log("Range: 1530-1750");
            return 3;
        }
        else if (position >= 1750 && position < 1970)
        {
            Debug.Log("Range: 1750-1970");
            return 4;
        }
        else
        {
            Debug.Log("Out of Range: ");
            return 5;
        }
    }

    void MultiColorGamePlay()
    {
        multiColor.SetActive(true);
        greenColor.SetActive(false);
        blocker.SetActive(false);
        if (!isLevel1Complete)
        {

            
            
        }
    }
    void CheckIdle()
    {
        playerAnimator.Play("IdleAnimation");
        print("isLevel1Complete" + isLevel1Complete);
        playerAnimator.gameObject.transform.position = playerGameObjectPosition;
    }
    
}

[System.Serializable]
public class Ranges
{
    public int min;
    public int max;
    public Ranges(int min, int max)
    {
        this.min = min;
        this.max = max;
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StatueOfLibertyManager : MonoBehaviour
{
    public static StatueOfLibertyManager Instance;
    public CubeController[] Cubes; // Assign all CubeController scripts in the Inspector
    public float pulseDuration = 0.5f;
    public float maxScale = 1.2f;
    //public GameObject tutorialPanel;
    public GameObject animationArrow;
    //public GameObject disableTutorialPanel;
    private bool gameActive = false;
    public GameObject square;
    public GameObject cubes;
    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        gameActive = false;
        //tutorialPanel.SetActive(true);
        
    }

    private IEnumerator PulseCubes()
    {
        foreach (CubeController cubeController in Cubes)
        {
            cubeController.SetColliderState(false); // Disable dragging
            yield return StartCoroutine(PulseCube(cubeController.transform));
            cubeController.SetColliderState(true); // Enable dragging
        }
    }

    private IEnumerator PulseCube(Transform cube)
    {
        Vector3 originalScale = cube.localScale;
        Vector3 targetScale = originalScale * maxScale;

        float time = 0;

        while (time < pulseDuration)
        {
            cube.localScale = Vector3.Lerp(originalScale, targetScale, time / pulseDuration);
            time += Time.deltaTime;
            yield return null;
        }

        time = 0;
        while (time < pulseDuration)
        {
            cube.localScale = Vector3.Lerp(targetScale, originalScale, time / pulseDuration);
            time += Time.deltaTime;
            yield return null;
        }

        cube.localScale = originalScale;
    }
    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void StartGame()
    {
        gameActive = true;
        //yield return new WaitForSeconds(0.1f);
        StartCoroutine(PulseCubes());
        //yield return new WaitForSeconds(1.5f);
        //disableTutorialPanel.SetActive(true);
        //playerAnimator.enabled = true;
        //Invoke("EndGame", gameDuration);
    }

    public void EndGame()
    {
        gameActive = false;
        StartupPopupManager.instance.ShowEndGamePopup();
        // Logic for end game (e.g., transition to the next scene)
    }
    public bool isGameStart()
    {
        return gameActive;
    }
    public void Win()
    {
        square.SetActive(false);
        cubes.SetActive(false);
        animationArrow.SetActive(true);
    }
}

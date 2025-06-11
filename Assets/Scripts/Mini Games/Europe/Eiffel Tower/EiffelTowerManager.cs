using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EiffelTowerManager : MonoBehaviour
{
    public EiffelTowerCubeController[] Cubes; // Assign all CubeController scripts in the Inspector
    public float pulseDuration = 0.5f;
    public float maxScale = 1.2f;
    public static EiffelTowerManager Instance;
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
        StartCoroutine(PulseCubes());
    }
    public void EndGame()
    {
        gameActive = false;
        EiffelTowerPopups.Instance.ShowEndGamePopup();
    }
    public bool isGameStart()
    {
        return gameActive;
    }
    //IEnumerator Start()
    //{
    //    yield return new WaitForSeconds(0.1f);
    //    StartCoroutine(PulseCubes());
    //}

    private IEnumerator PulseCubes()
    {
        foreach (EiffelTowerCubeController cubeController in Cubes)
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
}

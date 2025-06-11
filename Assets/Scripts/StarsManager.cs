using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class StarsManager : MonoBehaviour
{
    public static StarsManager Instance { get; private set; }

    [SerializeField] private List<Image> stars;
    [SerializeField] private Sprite activeStarSprite;
    [SerializeField] private RectTransform starUIPanel; // The parent of the UI stars
    [SerializeField] private GameObject starAnimationPrefab; // Prefab of the star for animation
    private int currentInactiveStar = 0;

    [SerializeField] private GameObject winPopUp;
    [SerializeField] private GameObject gameFailPopup;

    void Awake()
    {
        Instance = this;
    }

  public void AnimateAndIncreaseStar(Vector3 worldPosition)
{
        Debug.Log("AnimationStart");
    // Instantiate a temporary star for animation
    GameObject tempStar = Instantiate(starAnimationPrefab, starUIPanel);

    // Convert the world position to screen space (2D coordinates)
    Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);

    // Convert the screen space to local position in the UI Canvas (this is where we need to adjust)
    RectTransformUtility.ScreenPointToLocalPointInRectangle(starUIPanel, screenPosition, null, out Vector2 localPosition);

    // Set the initial position of the temp star in local position inside the UI
    RectTransform tempStarRect = tempStar.GetComponent<RectTransform>();
    tempStarRect.anchoredPosition = localPosition;

    // Target UI position for the star (the position where it should move)
    RectTransform targetStarRect = stars[currentInactiveStar].GetComponent<RectTransform>();

    // Animate the star to the target position
    tempStarRect
        .DOMove(targetStarRect.position, 0.3f) // Move the temp star to the target UI star
        .SetEase(Ease.InOutQuad)
        .OnComplete(() =>
        {
            // Destroy the temporary star after the animation
            Destroy(tempStar);

            // Activate the UI star in the stars list and increment the index
            stars[currentInactiveStar].sprite = activeStarSprite;
            currentInactiveStar++;
        });
}


    public void OnGameWon()
    {
        if (SceneManager.GetActiveScene().name == "Andes Mountain")
        {
            AndesMountainManager.Instance.EndGame();

        }
        else if(SceneManager.GetActiveScene().name == "Everest Mountain")
        {
            EverestMountainManager.Instance.EndGame();

        }
        //winPopUp.SetActive(true);
        //Time.timeScale = 0;
    }

    public void OnGameFailed()
    {
        gameFailPopup.SetActive(true);
        Time.timeScale = 0;
    }
}

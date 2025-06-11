using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EverestMountainHeightTracker : MonoBehaviour
{
    public static EverestMountainHeightTracker Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private Image heightProgressBar;
    //[SerializeField] private TextMeshProUGUI heightText;

    [Header("Height Settings")]
    [SerializeField] private float maxHeight = 8850f; // Everest height in meters
    [SerializeField] private Transform startBlock; // First block in the level
    [SerializeField] private Transform finalBlock; // Topmost block in the level
    [Header("Animation Settings")]
    [SerializeField] private float fillDuration = 0.5f; // Duration of the fill animation
    [SerializeField] private Ease fillEase = Ease.OutQuad;
    private float startingY;
    private float totalDistance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        // Initialize if we have the references
        if (startBlock != null && finalBlock != null)
        {
            startingY = startBlock.position.y;
            totalDistance = finalBlock.position.y - startingY;
        }
    }

    public void UpdateProgress(Transform playerTransform)
    {
        if (totalDistance <= 0) return;

        // Calculate progress based on player's height relative to start and end points

        float currentProgress = Mathf.InverseLerp(startingY, startingY + totalDistance, playerTransform.position.y);
        // Update the progress bar

        heightProgressBar.DOFillAmount(currentProgress, fillDuration)
            .SetEase(fillEase);
        // Update the progress bar
        //heightProgressBar.fillAmount = currentProgress;

        // Calculate and display current height in meters
        float currentHeight = currentProgress * maxHeight;
        //heightText.text = $"{Mathf.Round(currentHeight)}m";
    }

    public void SetStartAndEndBlocks(Transform start, Transform end)
    {
        startBlock = start;
        finalBlock = end;
        startingY = start.position.y;
        totalDistance = end.position.y - startingY;
    }
}

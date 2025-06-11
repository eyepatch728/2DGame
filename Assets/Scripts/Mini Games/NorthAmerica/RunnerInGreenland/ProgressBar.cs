using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    public Image progressBar;
    public float progressSpeed = 1f;

    void Start()
    {
        // Ensure the progress bar starts full
        progressBar.fillAmount = 1f;
    }

    void Update()
    {
        if (GameManager.Instance.IsGameActive())
        {
            // Decrease fillAmount based on the progressSpeed and game duration
            progressBar.fillAmount -= (progressSpeed / GameManager.Instance.gameDuration) * Time.deltaTime;

            // Clamp the fillAmount to ensure it doesn't go below 0
            if (progressBar.fillAmount <= 0f)
            {
                progressBar.fillAmount = 0f;
            }
        }
    }
}

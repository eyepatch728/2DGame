using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Block : MonoBehaviour
{
    [SerializeField] private GameObject starGO; // Star on this platform
    public bool isOnTop = false;
    public int starIndex; // Index of the star in the UI

    private void Awake()
    {
        if (starGO != null)
        {
            starGO.SetActive(true);
        }
    }

    public void CollectStar()
    {
        if (starGO != null)
        {

            SoundManager.instance.PlaySingleSound(3);
            // Animate star collection to the UI
            StarsManager.Instance.AnimateAndIncreaseStar(starGO.transform.position);

            // Update UI star color
            UIManager.Instance.UpdateStarUI(starIndex);

            // Destroy the star in the game scene
            Destroy(starGO);
            starGO = null; // Prevent reuse
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Ensure the player is landing on the platform
        {
            CollectStar();
        }
    }

    public void CheckOnTop()
    {
        if (isOnTop)
        {

            SoundManager.instance.PlaySingleSound(4);
            if (SceneManager.GetActiveScene().name == "Everest Mountain")
            {
                EverestMountainManager.Instance.EndGame();
            }
            else if (SceneManager.GetActiveScene().name == "Andes Mountain")
            {
                AndesMountainManager.Instance.EndGame();

            }
            StarsManager.Instance.OnGameWon();
        }
    }
}

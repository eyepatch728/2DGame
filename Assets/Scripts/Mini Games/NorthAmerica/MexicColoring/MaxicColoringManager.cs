using UnityEngine;
using UnityEngine.SceneManagement;

public class MaxicColoringManager : MonoBehaviour
{
    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

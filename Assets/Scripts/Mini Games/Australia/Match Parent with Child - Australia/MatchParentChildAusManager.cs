using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchParentChildAusManager : MonoBehaviour
{
    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public Image[] starUIImages; // Assign in inspector

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateStarUI(int index)
    {
        if (index >= 0 && index < starUIImages.Length)
        {
            starUIImages[index].color = Color.yellow; // Change UI star to yellow
        }
    }
}

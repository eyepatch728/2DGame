using UnityEngine;
using TMPro;

public class MazeInThePyramidsOfEgyptBottleInfo : MonoBehaviour
{
    //[TextArea(3, 10)]
    public string pyramidFact = "";

    [Header("UI References")]
    public TMP_Text factText;
    public GameObject factPanel;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ShowPyramidFact();
        }
    }

    private void ShowPyramidFact()
    {
        factPanel.SetActive(true);
        factText.text = pyramidFact;
    }
}

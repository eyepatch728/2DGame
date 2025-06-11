using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SouthAfricaAmazonRiver : MonoBehaviour
{
    public Transform SelectedLayer;
    public Transform SelectedBox;
    public GameObject character;
    public List<SelectLayer> layers = new List<SelectLayer>();
    public static SouthAfricaAmazonRiver Instance;
    private void Awake()
    {
        Instance = this;
    }
    public void CheckState()
    {
        bool allAreColoring = true;

        for (int i = 0; i < layers.Count; i++)
        {
            if (!layers[i].Iscoloring)
            {
                allAreColoring = false;
                break; // Exit early if any layer is not coloring
            }
        }

        if (allAreColoring)
        {
            Debug.Log("All layers have Iscoloring set to true!");
            SouthAfricaAmazonRiver.Instance.character.GetComponent<Animator>().Play("RunAnimation");
            SouthAfricaAmazonRiver.Instance = null;
        }
        else
        {
            Debug.Log("Not all layers have Iscoloring set to true.");
        }
    }
    //private Vector2 mousePosition;
    //private bool isPicking;

    //public void OnMouseDown()
    //{
    //    if (transform.GetSiblingIndex() == 0)
    //    {
    //        topBlock = transform;

    //        GetComponent<Collider>().enabled = false;
    //    }
    //}
    public void Back()
    {
        SceneManager.LoadScene("MainMenu");
    }
}



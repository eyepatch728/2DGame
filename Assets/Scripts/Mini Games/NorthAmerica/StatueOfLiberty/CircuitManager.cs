using UnityEngine;

public class CircuitManager : MonoBehaviour
{
    public static CircuitManager Instance;
    public Transform CircuitSlot;
    public Transform Fire;
    public CubeController[] Cubes;
    private void Awake()
    {
        Instance = this;
    }

    public void CorrectPlacement()
    {
        Debug.Log("Correct cube placed. Disabling all other cubes.");
        SoundManager.instance.PlaySingleSound(0);
        Debug.Log("Hello here ");

        foreach (CubeController cube in Cubes)
        {
            cube.DisableInteraction();
        }
        Invoke(nameof(GameEnd),4f);
        ;
    }
    void GameEnd()
    {
        SoundManager.instance.PlaySingleSound(4);
        StatueOfLibertyManager.Instance.EndGame();
    }
    private void OutlineDisappears()
    {
        CircuitSlot.GetComponent<SpriteRenderer>().enabled = false;
    }

    private void PlayArrowAnimation()
    {
        //LeanTween.move(Arrow.gameObject, Fire.position, 2f).setEaseOutExpo();
    }
}

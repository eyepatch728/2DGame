using UnityEngine;

public class EiffelTowerCircuitManager : MonoBehaviour
{
    public static EiffelTowerCircuitManager Instance;
    public Transform CircuitSlot;
    public Transform Fire;
    public EiffelTowerCubeController[] Cubes;
    public GameObject shadowAnimation;
    public GameObject cubes;
    public GameObject eiffleTowerLightAnim;
    public GameObject circuit;
    private void Awake()
    {
        Instance = this;
    }

    public void CorrectPlacement()
    {
        Debug.Log("Correct cube placed. Disabling all other cubes.");

        SoundManager.instance.PlaySingleSound(0);
        foreach (EiffelTowerCubeController cube in Cubes)
        {
            cube.DisableInteraction();
        }
        CircuitSlot.gameObject.SetActive(false);
        cubes.gameObject.SetActive(false);
        shadowAnimation.GetComponent<Animator>().enabled=true;
        Invoke(nameof(EnableEiffleTowerLights),2f);
    }
    void EnableEiffleTowerLights()
    {
        circuit.SetActive(false);
        eiffleTowerLightAnim.SetActive(true);
        Invoke(nameof(GameEnd), 4f);

    }
    void GameEnd()
    {

        SoundManager.instance.PlaySingleSound(4);
        EiffelTowerManager.Instance.EndGame();
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

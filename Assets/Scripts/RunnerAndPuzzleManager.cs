using UnityEngine;

public class RunnerAndPuzzleManager : MonoBehaviour
{
    public static ContinentData SelectedContinent; // Static property to store the selected continent
    public GameObject NorthAmericaPrefab;
    public GameObject EuropePrefab;
    public GameObject AsiaPrefab;
    public GameObject SouthAmericaPrefab;
    public GameObject AfricaPrefab;
    public GameObject AustraliaPrefab;
    public GameObject AntarcticaPrefab;

    [Header("Puzzle Games")]
    public GameObject NorthAmericaPuzzle;
    public GameObject EuropePuzzle;
    public GameObject AsiaPuzzle;
    public GameObject SouthAmericaPuzzle;
    public GameObject AfricaPuzzle;
    public GameObject AustraliaPuzzle;
    public GameObject AntarcticaPuzzle;

    private GameObject currentRunnerGame;
    private GameObject currentPuzzleGame;
    public GameObject nameBG;

    public static RunnerAndPuzzleManager instance;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (SelectedContinent == null)
        {
            Debug.LogError("No continent selected. Please select a continent before loading the scene.");
            return;
        }

        // Instantiate the appropriate prefab based on the selected continent
        GameObject continentPrefab = GetPrefabForContinent(SelectedContinent.name);
        if (continentPrefab != null)
        {
            GameObject instantiatedPrefab = Instantiate(continentPrefab);
            instantiatedPrefab.SetActive(true);
            Debug.Log($"{SelectedContinent.name} Runner Game instantiated.");
        }
        else
        {
            Debug.LogError($"No prefab found for continent: {SelectedContinent.name}");
        }
        DisableAllPuzzleGames();
    }
    private void DisableAllPuzzleGames()
    {
        if (NorthAmericaPuzzle) NorthAmericaPuzzle.SetActive(false);
        if (EuropePuzzle) EuropePuzzle.SetActive(false);
        if (AsiaPuzzle) AsiaPuzzle.SetActive(false);
        if (SouthAmericaPuzzle) SouthAmericaPuzzle.SetActive(false);
        if (AfricaPuzzle) AfricaPuzzle.SetActive(false);
        if (AustraliaPuzzle) AustraliaPuzzle.SetActive(false);
        if (AntarcticaPuzzle) AntarcticaPuzzle.SetActive(false);
    }
    // Method to return the correct prefab based on continent name
    GameObject GetPrefabForContinent(string continentName)
    {
        switch (continentName)
        {
            case "NorthAmerica":
                return NorthAmericaPrefab;
            case "Europe":
                nameBG.transform.localScale = new Vector3(0.35f, 0.61f, 0.61f);
                return EuropePrefab;
            case "Asia":
                nameBG.transform.localScale = new Vector3(0.35f, 0.61f, 0.61f);
                return AsiaPrefab;
            case "SouthAmerica":
                return SouthAmericaPrefab;
            case "Africa":
                nameBG.transform.localScale = new Vector3(0.35f, 0.61f, 0.61f);
                return AfricaPrefab;
            case "Australia":
                nameBG.transform.localScale = new Vector3(0.45f, 0.61f, 0.61f);
                return AustraliaPrefab;
            case "Antarctica":
                nameBG.transform.localScale = new Vector3(0.50f, 0.61f, 0.61f);
                return AntarcticaPrefab;
            // Add other cases for different continents
            default:
                return null;
        }
    }
    public void ActivatePuzzleGame()
    {
        nameBG.SetActive(false);
        if (SelectedContinent == null)
        {
            Debug.LogError("No continent selected!");
            return;
        }

        // Disable current runner game if it exists
        if (currentRunnerGame != null)
        {
            currentRunnerGame.SetActive(false);
        }

        // Enable the appropriate puzzle game
        GameObject puzzleToActivate = GetPuzzleGameObject(SelectedContinent.name);
        if (puzzleToActivate != null)
        {
            currentPuzzleGame = puzzleToActivate;
            currentPuzzleGame.SetActive(true);
            Debug.Log($"{SelectedContinent.name} Puzzle Game activated.");
        }
        else
        {
            Debug.LogError($"No puzzle game found for continent: {SelectedContinent.name}");
        }
    }

    private GameObject GetPuzzleGameObject(string continentName)
    {
        switch (continentName)
        {
            case "NorthAmerica": return NorthAmericaPuzzle;
            case "Europe": return EuropePuzzle;
            case "Asia": return AsiaPuzzle;
            case "SouthAmerica": return SouthAmericaPuzzle;
            case "Africa": return AfricaPuzzle;
            case "Australia": return AustraliaPuzzle;
            case "Antarctica": return AntarcticaPuzzle;
            default: return null;
        }
    }
}


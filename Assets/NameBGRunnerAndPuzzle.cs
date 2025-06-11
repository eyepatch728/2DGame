using UnityEngine;

public class NameBGRunnerAndPuzzle : MonoBehaviour
{
    public string continentName;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        continentName = RunnerAndPuzzleManager.SelectedContinent.name;
        if (continentName.ToLower() != "NORTHAMERICA".ToLower() && continentName.ToLower() != "SOUTHAMERICA".ToLower())
        {
            this.transform.position = new Vector3(-0.25f, 4.6609f, 0.0f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData Instance  { get; private set; }

	public enum eGameType 
    { 
        Shapes = 0,
        Coloring = 1,
        Puzzle = 2,
        Environment = 3,
        Memory = 4,
        ShapePuzzle = 5,

        NumModules
    }

    public eGameType GameType;
    public int SelectedLevel;

	void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(this);
            Instance = this;
        }
    }
}

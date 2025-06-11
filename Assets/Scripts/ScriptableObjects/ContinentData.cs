using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewContinent", menuName = "Game/Continent")]
public class ContinentData : ScriptableObject
{
    public string continentName;
    public string description;
    public List<MiniGameData> miniGames;
    public AudioClip[] audioClips;
    public AudioClip backgroundMusic;
}

using UnityEngine;

[CreateAssetMenu(fileName = "NewMiniGame", menuName = "Game/MiniGame")]
public class MiniGameData : ScriptableObject
{
    public string gameName;
    public string sceneName;
    public string description;
    public AudioClip backgroundMusic;
}

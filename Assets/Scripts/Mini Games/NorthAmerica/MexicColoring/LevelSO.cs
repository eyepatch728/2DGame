
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Coloring Game Mexico/LevelSO")]
public class LevelSO : ScriptableObject
{
    public string name;
    public List<string> description;
    public List<AudioClip> AudioClip;
    public List<Sprite> fillableImages;
    public List<Color> fillableImagesColors;
    public Sprite finalImage;
    public Sprite panelImage;
    public int fillableLength;
}

using System.Collections.Generic;
using UnityEngine;

public class LevelColors : MonoBehaviour
{
    public static LevelColors instance;
    public List<GameObject> allColors;
    public List<GameObject> currColors;
    public List<GameObject> level1Colors;
    public List<GameObject> level2Colors;
    public List<GameObject> level3Colors;
    public List<GameObject> level4Colors;
    public List<GameObject> level5Colors;
    public List<GameObject> level6Colors;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        SetCurrentLevelColors(ColorGameManager.Instance.currentIndex);
    }

    public void SetCurrentLevelColors(int levelIndex)
    {
        //if (levelIndex == 0)
        //{
        //    currColors = level1Colors;
        //}
        //else if (levelIndex == 1)
        //{
        //    currColors = level2Colors;
        //}
        //else if (levelIndex == 2)
        //{
        //    currColors = level3Colors;
        //}
        //else if (levelIndex == 3)
        //{
        //    currColors = level4Colors;
        //}
        //else if (levelIndex == 4)
        //{
        //    currColors = level5Colors;
        //}
        //else if (levelIndex == 5)
        //{
        //    currColors = level6Colors;
        //}
        //foreach (GameObject colors in allColors)
        //{
        //    colors.SetActive(false);
        //}
        //foreach (GameObject colors in currColors)
        //{
        //    colors.SetActive(true);
        //}
        currColors = allColors;
    }

}

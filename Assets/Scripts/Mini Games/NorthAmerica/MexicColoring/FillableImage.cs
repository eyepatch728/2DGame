using UnityEngine;
using UnityEngine.UI;

public class FillableImage : MonoBehaviour
{
    public Color targetColor;
    public string targetColorCode;
    public bool isFilled = false;
    public Image currImage;
    bool checkOnce = false;
    private void Awake()
    {
        currImage = GetComponent<Image>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //print(ColorUtility.ToHtmlStringRGB(currImage.color) + " | " + targetColorCode.TrimStart('#').ToUpper());

        //if(ColorUtility.ToHtmlStringRGB(currImage.color) == ColorUtility.ToHtmlStringRGB(targetColor) || ColorUtility.ToHtmlStringRGB(currImage.color) == targetColorCode.TrimStart('#').ToUpper())
        if(currImage.color != Color.white)
        {
            isFilled = true;
            if (!checkOnce)
            {
                if(ColorGameManager.Instance!=null)
                    ColorGameManager.Instance.CheckAllFill();
                else
                    AntarcticaAnimalPaintingManager.Instance.CheckAllFill();
                checkOnce = true;
            }
        }
        else
        {
            isFilled = false;
            checkOnce = false;
        }
    }
}

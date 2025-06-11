using UnityEngine;

public class PosForIpad : MonoBehaviour
{
   
    [SerializeField] Vector2 PosForIphone;
    [SerializeField] Vector2 PosForIPad;

    public bool isIpad;
    public bool isIphone;
    public static PosForIpad Instance;
    public GameObject tutorialBG;
    public float aspectRatio;
    public bool isRightButton;
    public bool isleftButton;
    public bool isAnchoredPos;
    public bool isFestivalBg;
   
    Transform Transform;
    void Start()
    {
        CheckDeviceResolution();
    }

    private void Awake()
    {
        Instance = this;
    }
    private void Update()
    {
        CheckDeviceResolution();
    }

    void CheckDeviceResolution()
    {
        aspectRatio = (float)Screen.width / Screen.height;
       // Debug.Log("Screen Resolution: " + Screen.width + "x" + Screen.height);
       // Debug.Log("Aspect Ratio: " + aspectRatio);

        // Reset flags
        isIpad = false;
        isIphone = false;

        // Typical iPad aspect ratio is around 4:3
        //if (Mathf.Approximately(aspectRatio, 4f / 3f) || (Screen.width >= 768 && Screen.height >= 1024))
        //{
        //    isIpad = true;
        //    isIphone = false;
        //}
        // Typical iPhone aspect ratios are around 16:9 or 19.5:9 (newer models)
        if (aspectRatio >= 16f / 9f || Screen.width < 768)
        {
            isIpad = false;
            isIphone = true;
        }
        else
        {
            isIpad = true;
            isIphone = false;
        }

        if (isIpad)
        {
            Debug.Log("Detected an iPad resolution.");
            HandleIpadUI();
        }
        else if (isIphone)
        {
            Debug.Log("Detected an iPhone resolution.");
            HandleIphoneUI();
        }
        else
        {
            Debug.Log("Unknown device resolution.");
        }
    }

    void HandleIpadUI()
    {
        if (!isAnchoredPos)
        {
            Debug.Log("Applying iPad-specific settings...");
            Transform = this.GetComponent<Transform>();
            if (isRightButton)
            {
                Transform.localPosition = new Vector3(1214, -117, 0);

            }
            else if (isleftButton)
            {
                Transform.localPosition = new Vector3(1214, 217, 0);

            }
            else
            {
                Transform.localPosition = new Vector2(0, PosForIPad.y );

            }
        }
        else 
        {
            RectTransform rectTransform = this.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(PosForIPad.x, PosForIPad.y);


        }
    }

    void HandleIphoneUI()
    {
        if (!isAnchoredPos)
        {

            Debug.Log("Applying iPhone-specific settings...");
            Transform = this.GetComponent<Transform>();
            if (isRightButton)
            {
                Transform.localPosition = new Vector3(1130, -117, 0);

            }
            else if (isleftButton)
            {
                Transform.localPosition = new Vector3(1130, 217, 0);

            }
            else if(isFestivalBg)
            {
                Transform.localPosition = new Vector2(0,PosForIphone.y);
            }

        }
        else 
        { 
            RectTransform rectTransform = this.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(PosForIphone.x , PosForIphone.y);
        
        }

    }
}

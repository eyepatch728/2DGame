//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.SceneManagement;
//using TMPro;
//using System.Collections.Generic;
//using System.Collections;

//public class CoralsManager : MonoBehaviour
//{
//    public static CoralsManager Instance;

//    public GameObject player;          // Reference to the player (car)
//    public GameObject[] animals;       // Array of animal objects
//    public GameObject animalInfoPopUp; // Reference to the Animal Information Popup
//    private bool isPopupActive = false;

//    [SerializeField] private Image animalImage;
//    [SerializeField] private TMP_Text animalDescription;
//    [SerializeField] private TMP_Text visitedAnimalsText;

//    private List<GameObject> remainingAnimals = new List<GameObject>();  // Track remaining animals
//    private int animalsVisited = 0;       // Track the number of animals visited

//    public Button forwardButton;
//    public Button backwardButton;
//    public bool gameActive = false;

//    [Header("Cat Animation")]
//    public Animator catAnimator;
//    public string isTalkingBool = "isTalking";
//    public AudioSource audioSource;

//    private void Awake()
//    {
//        Instance = this;
//    }

//    private void Start()
//    {
//        gameActive = false;
//        remainingAnimals.AddRange(animals); // Add all animals to the list at start
//    }

//    public void StartGame()
//    {
//        gameActive = true;
//    }

//    public void EndGame()
//    {
//        gameActive = false;
//        CoralsPopups.Instance.ShowEndGamePopup();
//    }

//    public bool isGameStart()
//    {
//        return gameActive;
//    }

//    void Update()
//    {
//        if (isGameStart())
//        {
//            for (int i = remainingAnimals.Count - 1; i >= 0; i--)
//            {
//                GameObject animal = remainingAnimals[i];
//                if (animal == null) continue;

//                if (Vector2.Distance(player.transform.position, animal.transform.position) <= 1f && !isPopupActive)
//                {
//                    // Get the information first before destroying the object
//                    AnimalInfo animalInfo = animal.GetComponent<AnimalInfo>();
//                    if (animalInfo != null)
//                    {

//                        SoundManager.instance.PlaySingleSound(0);
//                        StartCoroutine(ShowAnimalInfo(animal, animalInfo));
//                    }
//                }
//            }

//            visitedAnimalsText.text = $"{animalsVisited}/{animals.Length}";

//            if (animalsVisited == animals.Length)
//            {
//                EndGame();
//            }
//        }
//    }

//    private IEnumerator ShowAnimalInfo(GameObject animal, AnimalInfo animalInfo)
//    {
//        isPopupActive = true;

//        // Store animal information before destruction
//        Sprite savedImage = animalInfo.animalImage;
//        string savedDescription = animalInfo.description;
//        AudioClip savedAudioClip = animalInfo.animalAudioClip;

//        // Remove from the list before destroying
//        remainingAnimals.Remove(animal);
//        Destroy(animal);

//        // Disable movement buttons
//        forwardButton.interactable = false;
//        backwardButton.interactable = false;

//        // Show popup
//        animalInfoPopUp.SetActive(true);
//        animalImage.sprite = savedImage;
//        animalDescription.text = "";
//        SetCatTalking(true);

//        if (savedAudioClip != null && audioSource != null)
//        {
//            audioSource.clip = savedAudioClip;
//            audioSource.Play();
//        }

//        yield return StartCoroutine(TypeWriterEffect(savedDescription));

//        SetCatTalking(false);
//        yield return new WaitForSeconds(5f);

//        if (audioSource != null && audioSource.isPlaying)
//        {
//            audioSource.Stop();
//        }

//        animalInfoPopUp.SetActive(false);
//        forwardButton.interactable = true;
//        backwardButton.interactable = true;

//        // Increment count
//        animalsVisited++;

//        isPopupActive = false;
//    }

//    private void SetCatTalking(bool isTalking)
//    {
//        if (catAnimator)
//        {
//            catAnimator.SetBool(isTalkingBool, isTalking);
//        }
//    }

//    private IEnumerator TypeWriterEffect(string textToType)
//    {
//        float typingSpeed = 0.07f;
//        for (int i = 0; i < textToType.Length; i++)
//        {
//            animalDescription.text += textToType[i];
//            yield return new WaitForSeconds(typingSpeed);
//        }
//    }

//    public void BackToMainMenu()
//    {
//        SceneManager.LoadScene("MainMenu");
//    }
//}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CoralsManager : MonoBehaviour
{
    public static CoralsManager Instance;
    public GameObject player;
    public GameObject[] animals;
    public GameObject animalInfoPopUp;
    private bool isPopupActive = false;
    [SerializeField] private Image animalImage;
    [SerializeField] private TMP_Text animalDescription;
    [SerializeField] private TMP_Text visitedAnimalsText;
    private HashSet<GameObject> animalsWithPopupShown = new HashSet<GameObject>();
    private int animalsVisited = 0;
    public Button forwardButton;
    public Button backwardButton;
    public bool gameActive = false;
    [Header("Cat Animation")]
    public Animator catAnimator;
    public string isTalkingBool = "isTalking";
    public AudioSource audioSource;
    private bool skipText = false;
    private GameObject currentAnimal;
    private bool hasTappedOnce = false;
    private bool autoCloseTriggered = false;

    // ADDED: New flag to track when to end the game after popup closes
    private bool shouldEndGameAfterPopup = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        gameActive = false;
    }

    public void StartGame()
    {
        gameActive = true;
    }

    public void EndGame()
    {
        gameActive = false;
        CoralsPopups.Instance.ShowEndGamePopup();
    }

    public bool isGameStart()
    {
        return gameActive;
    }

    void Update()
    {
        if (isGameStart() && isPopupActive && Input.GetMouseButtonDown(0))
        {
            if (!hasTappedOnce)
            {
                hasTappedOnce = true;
                skipText = true;
                if (audioSource != null && audioSource.isPlaying)
                {
                    audioSource.Stop();
                }
                animalDescription.text = currentAnimal.GetComponent<AnimalInfo>().description;
            }
            else
            {
                CloseAnimalPopup();

                // ADDED: Check if we should end the game after closing the popup
                if (shouldEndGameAfterPopup)
                {
                    EndGame();
                }
            }
        }

        if (isGameStart())
        {
            foreach (GameObject animal in animals)
            {
                if (Vector2.Distance(player.transform.position, animal.transform.position) < 4f && !isPopupActive && !animalsWithPopupShown.Contains(animal))
                {
                    SoundManager.instance.PlaySingleSound(0);
                    StartCoroutine(ShowAnimalInfo(animal));
                }
            }
            visitedAnimalsText.text = $"{animalsVisited}/{animals.Length}";

            // REMOVED: The original check that would end the game immediately
            // if (animalsVisited == animals.Length)
            // {
            //     EndGame();
            // }
        }
        //animalImage.SetNativeSize();
    }

    private IEnumerator ShowAnimalInfo(GameObject animal)
    {
        isPopupActive = true;
        skipText = false;
        hasTappedOnce = false;
        autoCloseTriggered = false;
        currentAnimal = animal;
        animalsWithPopupShown.Add(animal);
        animalsVisited++;

        // ADDED: Check if this is the last animal and set flag accordingly
        if (animalsVisited == animals.Length)
        {
            shouldEndGameAfterPopup = true;
        }

        AnimalInfo animalInfo = animal.GetComponent<AnimalInfo>();

        if (animalInfo != null)
        {
            forwardButton.interactable = false;
            backwardButton.interactable = false;
            animalImage.sprite = animalInfo.animalImage;
            animalInfoPopUp.SetActive(true);
            animalDescription.text = "";
            SetCatTalking(true);

            if (animalInfo.animalAudioClip != null && audioSource != null)
            {
                audioSource.clip = animalInfo.animalAudioClip;
                audioSource.Play();
            }

            yield return StartCoroutine(TypeWriterEffect(animalInfo.description));
            SetCatTalking(false);

            while (audioSource != null && audioSource.isPlaying)
            {
                yield return null;
            }

            if (!hasTappedOnce)
            {
                autoCloseTriggered = true;
                yield return new WaitForSeconds(2f);
                CloseAnimalPopup();

                // ADDED: Check if we should end the game after closing the popup automatically
                if (shouldEndGameAfterPopup)
                {
                    EndGame();
                }
            }
        }
    }

    private IEnumerator TypeWriterEffect(string textToType)
    {
        float typingSpeed = 0.04f;
        for (int i = 0; i < textToType.Length; i++)
        {
            if (skipText)
            {
                animalDescription.text = textToType;
                yield break;
            }
            animalDescription.text += textToType[i];
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    private void CloseAnimalPopup()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        animalInfoPopUp.SetActive(false);
        forwardButton.interactable = true;
        backwardButton.interactable = true;
        isPopupActive = false;
    }

    private void SetCatTalking(bool isTalking)
    {
        if (catAnimator)
        {
            catAnimator.SetBool(isTalkingBool, isTalking);
        }
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

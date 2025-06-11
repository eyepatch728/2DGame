//using System.Collections;
//using System.Collections.Generic;
//using TMPro;
//using UnityEngine;
//using UnityEngine.SceneManagement;
//using UnityEngine.UI;

//public class PenguinManager : MonoBehaviour
//{
//    public static PenguinManager Instance;
//    public GameObject player;          // Reference to the player (car)
//    public GameObject[] animals;      // Array of animal objects
//    public GameObject animalInfoPopUp; // Reference to the Animal Information Popup
//    //public GameObject winPopup; // Reference to the Win Popup
//    private bool isPopupActive = false;
//    [SerializeField] private Image animalImage;
//    [SerializeField] private TMP_Text animalDescription;
//    //[SerializeField] private TMP_Text animalName;
//    [SerializeField] private TMP_Text visitedAnimalsText;
//    private List<GameObject> animalsWithPopupShown = new List<GameObject>();  // Track animals whose popup has been shown
//    private int animalsVisited = 0;       // Track the number of animals visited
//    public Button forwardButton;        // Reference to the Forward Button
//    public Button backwardButton;       // Reference to the Backward Button
//    public bool gameActive = false;
//    [Header("Cat Animation")]
//    public Animator catAnimator; // Animator for the cat
//    public string isTalkingBool = "isTalking";
//    public AudioSource audioSource;
//    private void Awake()
//    {
//        Instance = this;
//    }
//    private void Start()
//    {
//        gameActive = false;
//    }
//    public void StartGame()
//    {
//        gameActive = true;

//    }
//    public void EndGame()
//    {
//        gameActive = false;
//        AntarcticaPenguinPopups.Instance.ShowEndGamePopup();
//    }
//    public bool isGameStart()
//    {
//        return gameActive;
//    }
//    void Update()
//    {
//        if (isGameStart())
//        {
//            // Loop through each animal and check if the player is close enough
//            foreach (GameObject animal in animals)
//            {
//                // Check if player is close to animal and the popup has not been shown for this animal
//                if (Vector2.Distance(player.transform.position, animal.transform.position) < 4f && !isPopupActive && !animalsWithPopupShown.Contains(animal))
//                {

//                    SoundManager.instance.PlaySingleSound(0);
//                    // Show the animal info popup if it hasn't been shown yet
//                    StartCoroutine(ShowAnimalInfo(animal));
//                }
//            }
//            visitedAnimalsText.text = $"{animalsVisited}/{animals.Length}";

//            // If all animals are visited, show the win popup
//            if (animalsVisited == animals.Length)
//            {
//                EndGame();
//            }
//        }
//        animalImage.SetNativeSize();
//    }

//    private IEnumerator ShowAnimalInfo(GameObject animal)
//    {
//        isPopupActive = true;  // Mark the popup as active

//        // Get the Animal's information from its properties
//        AnimalInfo animalInfo = animal.GetComponent<AnimalInfo>();

//        // Set the pop-up details if animalInfo is not null
//        if (animalInfo != null)
//        {
//            // Disable buttons to stop movement during popup
//            forwardButton.interactable = false;
//            backwardButton.interactable = false;

//            // Update the Image directly
//            animalImage.sprite = animalInfo.animalImage;

//            // Start typing effect for description text
//            animalInfoPopUp.SetActive(true); // Activate the pop-up
//            animalDescription.text = "";    // Clear any previous text
//            //animalName.text = "";
//            //animalName.text = animalInfo.animalName;
//            SetCatTalking(true);
//            if (animalInfo.animalAudioClip != null)
//            {
//                //audioSource = GetComponent<AudioSource>(); // Get AudioSource attached to the GameManager (or assign it manually)
//                if (audioSource != null)
//                {
//                    audioSource.clip = animalInfo.animalAudioClip;
//                    audioSource.Play(); // Play the audio clip
//                }
//            }
//            yield return StartCoroutine(TypeWriterEffect(animalInfo.description)); // Display description with typing effect
//            SetCatTalking(false);
//            // Wait for 5 seconds before hiding the popup
//            yield return new WaitForSeconds(5f);
//            if (audioSource != null && audioSource.isPlaying)
//            {
//                audioSource.Stop();
//            }
//            // Deactivate the pop-up
//            animalInfoPopUp.SetActive(false);

//            // Re-enable buttons after popup is closed
//            forwardButton.interactable = true;
//            backwardButton.interactable = true;

//            // Add the animal to the set to prevent the popup from showing again
//            animalsWithPopupShown.Add(animal);

//            // Increment the number of visited animals
//            animalsVisited++;
//        }

//        isPopupActive = false; // Allow the popup to be triggered again when the player leaves the animal's range
//    }
//    private void SetCatTalking(bool isTalking)
//    {
//        if (catAnimator)
//        {
//            catAnimator.SetBool(isTalkingBool, isTalking);
//        }
//    }
//    // Typing effect coroutine
//    private IEnumerator TypeWriterEffect(string textToType)
//    {
//        float typingSpeed = 0.07f; // Speed of typing effect (characters per second)
//        for (int i = 0; i < textToType.Length; i++)
//        {
//            animalDescription.text += textToType[i];
//            yield return new WaitForSeconds(typingSpeed);
//        }
//    }

//    // Show the win popup when all animals have been visited
//    //private IEnumerator ShowWinPopup()
//    //{
//    //    yield return new WaitForSeconds(1f);
//    //    winPopup.SetActive(true);  // Activate the win popup
//    //    yield return new WaitForSeconds(3f);  // Show the win popup for 3 seconds
//    //    winPopup.SetActive(false);  // Deactivate the win popup
//    //    yield return new WaitForSeconds(1f);  // Show the win popup for 3 seconds

//    //    BackToMainMenu();  // Go back to the main menu
//    //}

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

public class PenguinManager : MonoBehaviour
{
    public static PenguinManager Instance;
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
        AntarcticaPenguinPopups.Instance.ShowEndGamePopup();
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

            if (animalsVisited == animals.Length)
            {
                EndGame();
            }
        }
        animalImage.SetNativeSize();
    }

    private IEnumerator ShowAnimalInfo(GameObject animal)
    {
        isPopupActive = true;
        skipText = false;
        hasTappedOnce = false;
        currentAnimal = animal;
        animalsWithPopupShown.Add(animal);
        animalsVisited++;

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

            yield return new WaitForSeconds(1.5f);
            CloseAnimalPopup();
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

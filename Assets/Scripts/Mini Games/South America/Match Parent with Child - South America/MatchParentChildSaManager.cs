using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MatchParentChildSaManager : MonoBehaviour
{
    [System.Serializable]
    public class AnimalSet
    {
        public string animalName;
        public Sprite parentSprite;
        public Sprite babySprite;
        public string funFact;
        public bool hasBeenMatched = false;
        public AudioClip animalAudio;
        public bool shouldFlip;
    }

    public List<AnimalSet> allAnimals;
    public Transform leftPosition;
    public Transform rightPosition;
    public Transform centerPosition;  // This is where the rock is
    public Transform balloonStartPosition;

    public GameObject parentAnimalPrefab;
    public GameObject babyAnimalPrefab;
    public GameObject explorerCatPrefab;
    public MatchParentChildSaPopupController popupController;
    public Image animalImage;

    private List<AnimalSet> currentPairAnimals = new List<AnimalSet>();
    private AnimalSet currentLeftAnimal;
    private AnimalSet currentRightAnimal;
    private GameObject babyAnimal;
    private GameObject explorerCat;
    private GameObject leftParent;
    private GameObject rightParent;
    private List<GameObject> currentBabyAnimals = new List<GameObject>();
    private int matchedAnimalCount = 0;
    private int currentPairIndex = 0;
    private int completedPairCount = 0;
    private int currentBabyIndex = 0;
    public bool gameActive = false;
    public bool isGamePause = false;
    public bool isTransitioning = false;
    public static MatchParentChildSaManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        gameActive = false;
    }

    public void StartGame()
    {
        gameActive = true;
        InitializeGame();
    }

    public void EndGame()
    {
        gameActive = false;
        MatchParentChildSAPopups.Instance.ShowEndGamePopup();
    }

    public bool isGameStart()
    {
        return gameActive;
    }

    private void InitializeGame()
    {
        matchedAnimalCount = 0;
        currentPairIndex = 0;
        completedPairCount = 0;
        currentBabyIndex = 0;
        currentBabyAnimals.Clear();

        // Create pairs of animals (3 pairs total)
        GroupAnimalsIntoPairs();

        // Start with the first pair
        SpawnCurrentAnimalPair();
    }

    private void GroupAnimalsIntoPairs()
    {
        // Shuffle all animals first
        List<AnimalSet> shuffledAnimals = new List<AnimalSet>(allAnimals);
        for (int i = shuffledAnimals.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            AnimalSet temp = shuffledAnimals[i];
            shuffledAnimals[i] = shuffledAnimals[randomIndex];
            shuffledAnimals[randomIndex] = temp;
        }

        // Reset matched status
        foreach (var animal in shuffledAnimals)
        {
            animal.hasBeenMatched = false;
        }
    }

    private void SpawnCurrentAnimalPair()
    {
        if (isGamePause || isTransitioning)
        {
            return;
        }

        if (completedPairCount >= 3)
        {
            GameComplete();
            return;
        }

        int startIndex = currentPairIndex * 2;
        if (startIndex + 1 >= allAnimals.Count)
        {
            Debug.LogError("Not enough animals available to spawn a pair.");
            return;
        }

        currentLeftAnimal = allAnimals[startIndex];
        currentRightAnimal = allAnimals[startIndex + 1];

        currentLeftAnimal.hasBeenMatched = false;
        currentRightAnimal.hasBeenMatched = false;

        currentPairAnimals.Clear();
        currentPairAnimals.Add(currentLeftAnimal);
        currentPairAnimals.Add(currentRightAnimal);

        currentBabyIndex = 0;
        currentBabyAnimals.Clear();

        Vector3 offScreenLeft = leftPosition.position + new Vector3(-10f, 0, 0);
        Vector3 offScreenRight = rightPosition.position + new Vector3(10f, 0, 0);

        leftParent = Instantiate(parentAnimalPrefab, offScreenLeft, Quaternion.identity);
        rightParent = Instantiate(parentAnimalPrefab, offScreenRight, Quaternion.identity);

        leftParent.GetComponent<SpriteRenderer>().sprite = currentLeftAnimal.parentSprite;
        rightParent.GetComponent<SpriteRenderer>().sprite = currentRightAnimal.parentSprite;

        // 🛠️ **Apply flipping based on `shouldFlip`**
        leftParent.transform.localScale = currentLeftAnimal.shouldFlip
            ? new Vector3(-0.6f, 0.6f, 0.6f)  // If flip is needed, make it face left
            : new Vector3(0.6f, 0.6f, 0.6f);   // Default (face right)

        rightParent.transform.localScale = currentRightAnimal.shouldFlip
            ? new Vector3(0.6f, 0.6f, 0.6f)   // If flip is needed, make it face right
            : new Vector3(-0.6f, 0.6f, 0.6f); // Default (face left)

        UpdateParentCollider(leftParent, currentLeftAnimal);
        UpdateParentCollider(rightParent, currentRightAnimal);

        ParentAnimal leftParentScript = leftParent.GetComponent<ParentAnimal>();
        ParentAnimal rightParentScript = rightParent.GetComponent<ParentAnimal>();

        leftParentScript.animalData = currentLeftAnimal;
        rightParentScript.animalData = currentRightAnimal;

        isTransitioning = true;

        Sequence sequence = DOTween.Sequence();
        sequence.Join(leftParent.transform.DOMove(leftPosition.position, 0.8f).SetEase(Ease.OutBack))
               .Join(rightParent.transform.DOMove(rightPosition.position, 0.8f).SetEase(Ease.OutBack))
               .OnComplete(() => {
                   isTransitioning = false;
                   SpawnNextBaby();
               });
    }


    private void SpawnNextBaby()
    {
        if (isGamePause || isTransitioning)
        {
            return;
        }

        // If both babies have been matched in the current pair
        if (currentBabyIndex >= 2)
        {
            // Move to the next pair
            AnimateCurrentPairOffScreen();
            return;
        }

        // Choose which parent's baby to spawn
        AnimalSet chosenBaby = currentPairAnimals[currentBabyIndex];
        SpawnBabyAnimal(chosenBaby);
    }

    private void AnimateCurrentPairOffScreen()
    {
        isTransitioning = true;

        // Determine off-screen positions
        Vector3 offScreenLeft = leftPosition.position + new Vector3(-10f, 0, 0);
        Vector3 offScreenRight = rightPosition.position + new Vector3(10f, 0, 0);

        // Create a sequence for all animations
        Sequence sequence = DOTween.Sequence();

        // Animate parents sliding out
        if (leftParent != null)
        {
            sequence.Join(leftParent.transform.DOMove(offScreenLeft, 0.8f).SetEase(Ease.InBack));
        }

        if (rightParent != null)
        {
            sequence.Join(rightParent.transform.DOMove(offScreenRight, 0.8f).SetEase(Ease.InBack));
        }

        // Animate all babies sliding out to their respective parent's position
        foreach (GameObject baby in currentBabyAnimals)
        {
            if (baby != null)
            {
                // Determine which parent this baby belongs to
                AnimalBabyDraggable babyScript = baby.GetComponent<AnimalBabyDraggable>();
                if (babyScript != null && babyScript.animalData != null)
                {
                    // Move baby to left or right based on which parent it matches
                    Vector3 targetPosition = (babyScript.animalData == currentLeftAnimal) ? offScreenLeft : offScreenRight;
                    sequence.Join(baby.transform.DOMove(targetPosition, 0.8f).SetEase(Ease.InBack));
                }
            }
        }

        sequence.OnComplete(() => {
            isTransitioning = false;
            DestroyCurrentPair();
            completedPairCount++;
            currentPairIndex++;

            // Move to the next pair or end game
            if (completedPairCount < 3)
            {
                SpawnCurrentAnimalPair();
            }
            else
            {
                GameComplete();
            }
        });
    }

    private void DestroyCurrentPair()
    {
        if (leftParent != null)
        {
            Destroy(leftParent);
            leftParent = null;
        }
        if (rightParent != null)
        {
            Destroy(rightParent);
            rightParent = null;
        }

        // Destroy all baby animals from this pair
        foreach (GameObject baby in currentBabyAnimals)
        {
            if (baby != null)
            {
                Destroy(baby);
            }
        }
        currentBabyAnimals.Clear();

        // Also ensure the current baby is cleared
        if (babyAnimal != null)
        {
            Destroy(babyAnimal);
            babyAnimal = null;
        }
    }

    private void Update()
    {
        if (animalImage != null)
        {
            animalImage.SetNativeSize();
        }
    }

    void UpdateParentCollider(GameObject parentAnimal, AnimalSet animalSet)
    {
        SpriteRenderer spriteRenderer = parentAnimal.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer not found on parent animal!");
            return;
        }

        foreach (var collider in parentAnimal.GetComponents<Collider2D>())
        {
            Destroy(collider);
        }

        BoxCollider2D boxCollider = parentAnimal.AddComponent<BoxCollider2D>();
        boxCollider.isTrigger = true;
    }

    private void SpawnBabyAnimal(AnimalSet animalSet)
    {
        if (isGamePause || isTransitioning)
        {
            return;
        }

        if (explorerCat == null)
        {
            explorerCat = Instantiate(explorerCatPrefab, balloonStartPosition.position, Quaternion.identity);
        }

        babyAnimal = Instantiate(babyAnimalPrefab, balloonStartPosition.position, Quaternion.identity);
        currentBabyAnimals.Add(babyAnimal);  // Track this baby animal for the current pair

        AnimalBabyDraggable babyScript = babyAnimal.GetComponent<AnimalBabyDraggable>();
        babyScript.Initialize(animalSet, this);

        if (animalImage != null)
        {
            animalImage.SetNativeSize();
        }

        UpdateCollider(babyAnimal, animalSet);
        StartBalloonAnimation();
    }

    void UpdateCollider(GameObject babyAnimal, AnimalSet animalSet)
    {
        SpriteRenderer spriteRenderer = babyAnimal.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer not found on babyAnimal!");
            return;
        }

        foreach (var collider in babyAnimal.GetComponents<Collider2D>())
        {
            Destroy(collider);
        }

        BoxCollider2D boxCollider2D = babyAnimal.AddComponent<BoxCollider2D>();
        boxCollider2D.isTrigger = true;
    }

    private void StartBalloonAnimation()
    {
        if (explorerCat == null || babyAnimal == null)
        {
            Debug.LogError("ExplorerCat or BabyAnimal is not initialized.");
            return;
        }

        MatchParentChildSaExplorerCat catScript = explorerCat.GetComponent<MatchParentChildSaExplorerCat>();
        if (catScript != null)
        {
            catScript.dropPosition = centerPosition;
            catScript.DeliverBaby(babyAnimal, this);
        }
        else
        {
            Debug.LogError("ExplorerCat script is missing on the balloon object.");
        }
    }

    public Coroutine babyAnimalCoroutine;
    public void OnBabyPlaced(bool isCorrect, AnimalSet animal)
    {
        if (isCorrect)
        {
            SoundManager.instance.PlaySingleSound(0);
            Debug.Log("Correct Placement......................");
            animal.hasBeenMatched = true;
            matchedAnimalCount++;
            ShowFunFactPopup(animal);
            currentBabyIndex++; // Move to the next baby in the current pair
            parentAnimalPrefab.GetComponent<BoxCollider2D>().enabled = false;
        }
        else
        {
            Debug.Log("Incorrect Placement......................");
            babyAnimalCoroutine = StartCoroutine(MoveBabyAnimal(babyAnimal.transform, centerPosition, 0.3f));
        }
    }

    IEnumerator MoveBabyAnimal(Transform babyAnimal, Transform centerPosition, float duration)
    {
        if (babyAnimal == null) yield break;

        Vector3 startPos = babyAnimal.position;
        Vector3 endPos = centerPosition.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            babyAnimal.position = Vector3.Lerp(startPos, endPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure final position is exactly at the center
        babyAnimal.position = endPos;
    }

    public void PrepareNextRound()
    {
        if (isGamePause || isTransitioning)
        {
            return;
        }

        // After the popup is closed, spawn the next baby if needed
        SpawnNextBaby();
    }

    private void ShowFunFactPopup(AnimalSet animalSet)
    {
        popupController.ShowPopup(animalSet.funFact, animalSet.parentSprite, animalSet.animalAudio);
    }

    private void GameComplete()
    {
        DestroyAllObjects();
        EndGame();
    }

    private void DestroyAllObjects()
    {
        if (leftParent != null)
        {
            Destroy(leftParent);
            leftParent = null;
        }
        if (rightParent != null)
        {
            Destroy(rightParent);
            rightParent = null;
        }

        // Destroy all baby animals
        foreach (GameObject baby in currentBabyAnimals)
        {
            if (baby != null)
            {
                Destroy(baby);
            }
        }
        currentBabyAnimals.Clear();

        if (babyAnimal != null)
        {
            Destroy(babyAnimal);
            babyAnimal = null;
        }
        if (explorerCat != null)
        {
            Destroy(explorerCat);
            explorerCat = null;
        }
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
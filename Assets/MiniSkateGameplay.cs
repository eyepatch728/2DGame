using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
// using UnityEditor.Experimental.GraphView;
// using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MiniSkateGameplay : MonoBehaviour
{
	public Collider2D Player;
	public Animator PlayerAnimator;
	public int Health = 3;
	int LastHealth; // Used for animations
	public float MaxInvicibilityTimer = 1.0f;
	public float RunSpeed = 5.0f;
	public float JumpSpeed = 10.0f;
	public float HitSpeed = -10.0f;
	public float HitImpulse = 10.0f;
	public float RunRecoveryTimeFromHit = 0.2f; // Seconds
	public float RunRecoverTimeFromJump = 1.0f; // Seconds
	public float JumpImpulse = 100.0f;
	public float Gravity = 9.8f;

	public float LetterScaleMultiplier = 0.5f;

	public float ObstacleMinFrequency = 0.5f;
	public float ObstacleMaxFrequency = 1.0f;
	public float LetterMinFrequency = 3.0f;
	public float LetterMaxFrequency = 5.0f;

	public GameObject[] Obstacles;
	public GameObject[] Letters; // A-Z
	public GameObject[] Words;
	public float WordsScale = 0.75f;
	public string[] WordStrings;
	public Transform[] Grounds;
	public float MinGroundX = -2.0f;
	public Transform[] Backgrounds;
	public float MinBackgroundX = -30.0f;
	public float BackgroundParallax = 0.5f;
	public Sprite backgroundSprite;
	//public Image[] ActiveStars;
	//public Image[] InactiveStars;

	//public RawImage Fader;
	//public GameObject PopUp;
	//public GameObject WinPanel;
	//public GameObject LosePanel;

	[Header("Tutorial")]
	public RawImage TutorialBackdrop;
	public Image TutorialFocus;

	float obstacleNextDistance;
	float letterNextDistance;

	List<Collider2D> SpawnedObstacles = new List<Collider2D>();
	List<LetterDefinition> SpawnedLetters = new List<LetterDefinition>();
	string wantedWord;
	int wordPosition = 0;

	bool isJumping = false;
	Vector3 velocity = Vector3.zero;
	float scrollSpeed;
	bool jumpHold = false;
	float invicibilityTimer;
	//bool isDead = false; // Commented out to disable lose functionality
	bool isTutorial = false;
	bool tutorialDamaged = false;
	bool isPaused = false;
	float distance = 0f;

	const float FLOOR_HEIGHT = -2.69f;
	const float LETTER_HEIGHT = 2.0f;
	const float SPAWN_X = 12.0f;
	const float KILL_X = -12.0f;

	List<SpriteRenderer> WordLetters = new List<SpriteRenderer>();

	[Header("Transition")]
	public float TransitionDelay = 2.0f; // Time to wait before transitioning
	private RunnerAndPuzzleManager puzzleManager;
	private bool isTransitioning = false;
	private bool isGameComplete = false;

	public GameObject continentInfoPopUp; // Reference to the Animal Information Popup
	[SerializeField] private Image lettersImage;
	[SerializeField] private TMP_Text continentDescription;

	[Header("Cat Animation")]
	public Animator catAnimator; // Animator for the cat
	public string isTalkingBool = "isTalking";

	[Header("Audio Settings")]
	public AudioSource audioSource;
	public AudioClip[] audioClips; // Array of audio clips corresponding to text segments

	[Header("Typewriter Settings")]
	[SerializeField] private float typewriterSpeed = 0.05f;
	// Start is called before the first frame update
	[Header("Text Segments")]
	[TextArea(2, 5)] // Allows you to enter multiline text in the Inspector
	public string[] textSegments;
	public GameObject tutorialPanel;
	public string continent;
    [SerializeField] private GameObject transitionPanel; // The new panel
    [SerializeField] private TMP_Text transitionText; // Text inside the panel
    [SerializeField] private string[] nextTextSegments;
    [SerializeField] private AudioSource transitionAudioSource; // Audio source
    [SerializeField] private AudioClip[] transitionAudioClip; // Audio clip for this panel
    public Animator cat2Animator;
    void Start()
	{
		continent=PlayerPrefs.GetString("SelectedContinentName");
		isPaused = true;
		StartCoroutine(nameof(EnableTutorial));
		//SoundManager.Instance.CrossFadeMusic("SkateMiniGameBgSound", 1.0f);
		puzzleManager = FindFirstObjectByType<RunnerAndPuzzleManager>();
		if (puzzleManager == null)
		{
			Debug.LogError("RunnerAndPuzzleManager not found in scene!");
		}
		obstacleNextDistance = Random.Range(ObstacleMinFrequency, ObstacleMaxFrequency);
		letterNextDistance = Random.Range(LetterMinFrequency, LetterMaxFrequency);
		int wordIdx = Random.Range(0, WordStrings.Length);
		wantedWord = WordStrings[wordIdx];
		wordPosition = 0;
		scrollSpeed = RunSpeed;

		LastHealth = Health;
		//UpdateHealthGraphics();
		CreateWord(0);

		//isTutorial = !ProgressManager.Instance.IsTutorialShown(1);
		if (isTutorial)
		{
			StartCoroutine(TutorialCoroutine());
		}
	}

    // Update is called once per frame
    void Update()
    {
        //if (isDead)  // Commented out to disable lose functionality
        //	return;
        if (isPaused)
            return;
        if (isTransitioning) return; // Skip Update if transitioning or game is complete

        if (SpawnedObstacles == null || SpawnedLetters == null) return;
        // Check Distances
        if (distance > obstacleNextDistance && !isGameComplete)
        {
            SpawnObstacle();
            if (isTutorial)
            {
                if (distance < 40.0f)
                    obstacleNextDistance = 60.0f;
                else
                    obstacleNextDistance = 100.0f;
            }
            else
            {
                obstacleNextDistance = distance + Random.Range(ObstacleMinFrequency, ObstacleMaxFrequency);
            }

        }

        if (distance > letterNextDistance && !isGameComplete)
        {
            SpawnLetter();
            letterNextDistance = distance + Random.Range(LetterMinFrequency, LetterMaxFrequency);
        }

        // Update positions
        UpdatePlayer();

        // Update Ground
        foreach (var ground in Grounds)
        {
            ground.position += Vector3.left * (scrollSpeed * Time.deltaTime);
            if (ground.position.x < MinGroundX)
                ground.position += Vector3.right * (ground.GetComponent<SpriteRenderer>().size.x * ground.localScale.x * 2.0f);
        }

        distance += scrollSpeed * Time.deltaTime;

        // Update Backgrounds
        foreach (var background in Backgrounds)
        {
            background.position += Vector3.left * (scrollSpeed * Time.deltaTime * BackgroundParallax);
            if (background.position.x < MinBackgroundX)
                background.position += Vector3.right * (27.8f * 2.0f);
        }

        if (isTransitioning || isGameComplete) return; // Skip Update if transitioning or game is complete

        List<Collider2D> deleteObjects = new List<Collider2D>();
        foreach (var obstacle in SpawnedObstacles.ToList())
        {
            if (obstacle == null)
            {
                deleteObjects.Add(obstacle);
                continue;
            }

            obstacle.transform.position += Vector3.left * (scrollSpeed * Time.deltaTime);
            if (obstacle.transform.position.x < KILL_X)
            {
                deleteObjects.Add(obstacle);
                if (obstacle.gameObject != null)
                {
                    Destroy(obstacle.gameObject);
                }
            }
        }

        List<LetterDefinition> deleteLetters = new List<LetterDefinition>();
        foreach (var letter in SpawnedLetters.ToList())
        {
            if (letter == null || letter.gameObject == null)
            {
                deleteLetters.Add(letter);
                continue;
            }

            letter.transform.position += Vector3.left * (scrollSpeed * Time.deltaTime);
            if (letter.transform.position.x < KILL_X)
            {
                deleteLetters.Add(letter);
                if (letter.gameObject != null)
                {
                    Destroy(letter.gameObject);
                }
            }
        }



        // Check Collisions
        if (invicibilityTimer > 0)
        {
            invicibilityTimer -= Time.deltaTime;
        }
        else
        {
            foreach (var obstacle in SpawnedObstacles)
            {
                if (Player.IsTouching(obstacle))
                { // Hit!
                    invicibilityTimer = MaxInvicibilityTimer;
                    Health--;
                    tutorialDamaged = true;
                    //SoundManager.Instance.PlaySFX("HitSound");
                    //UpdateHealthGraphics();
                    Debug.Log("Hittt AIAIAIAIAIAIiiiiii");
                    scrollSpeed = HitSpeed;
                    velocity = Vector3.up * HitImpulse;
                    obstacle.enabled = false;

                    //if (Health == 0)
                    //{
                    //	ShowDeathUI();                      // Commented out to disable lose functionality

                    //	isDead = true;
                    //	PlayerAnimator.Play("Lose");
                    //	return;
                    //}

                    SpriteRenderer sr = Player.GetComponentInChildren<SpriteRenderer>();
                    //sr.DOFade(0f, MaxInvicibilityTimer).SetEase(Ease.Flash, 4.0f);
                    sr.DOColor(new Color(1.0f, 0f, 0f, 0.5f), MaxInvicibilityTimer).SetEase(Ease.Flash, 12.0f);
                    break;
                }
            }
        }

        foreach (var letter in SpawnedLetters)
        {
            if (Player.IsTouching(letter.Collider))
            { // Got Letter
              //Debug.Log("Ohh Mamaa");
              //SoundManager.Instance.PlaySFX("LetterMiniGameColect");

				SoundManager.instance.PlaySingleSound(3);
                var letterRef = letter; // Used for timed references
                int wordPosCopy = wordPosition;
                SpriteRenderer sr = letter.Collider.GetComponent<SpriteRenderer>();
                sr.sortingLayerName = "Foreground";
                sr.sortingOrder = 5;
                //SoundManager.Instance.PlaySFX(char.ToLower(letterRef.Letter).ToString());
                letter.transform.DOMove(WordLetters[wordPosition].transform.position, 1.0f);
                letter.transform.DOScale(WordLetters[wordPosition].transform.localScale * WordsScale, 1.0f).OnComplete(() =>
                {
                    Destroy(letterRef.gameObject);
                    WordLetters[wordPosCopy].color = Color.white;
                });
                wordPosition++;
                if (wordPosition < wantedWord.Length && wantedWord[wordPosition] != letter.Letter)
                {
                    // If we're touching one letter we need to remove all letters as we'll get new ones from the word
                    foreach (var let in SpawnedLetters)
                    {
                        if (let == letter)
                            continue;
                        Destroy(let.gameObject);
                    }
                    SpawnedLetters.Clear();
                    deleteLetters.Clear();
                }
                else
                {
                    //Destroy(letter.gameObject);
                    deleteLetters.Add(letter);
                }
                if (wordPosition >= wantedWord.Length)
                {
                    //isDead = true;   // Commented out to disable lose functionality
                    SoundManager.instance.PlaySingleSound(4);
                    ShowWinUI();
                    return;
                }

                break;
            }
        }

        // Delayed removal
        foreach (var del in deleteObjects)
        {
            if (SpawnedObstacles.Contains(del))
                SpawnedObstacles.Remove(del);
        }
        foreach (var del in deleteLetters)
        {
            if (SpawnedLetters.Contains(del))
                SpawnedLetters.Remove(del);
        }
        if (!isGameComplete && invicibilityTimer <= 0)
        {
            foreach (var obstacle in SpawnedObstacles.ToList())
            {
                if (obstacle != null && Player != null && Player.gameObject != null && obstacle.gameObject != null)
                {
                    if (Player.IsTouching(obstacle))
                    {
                        HandleObstacleCollision(obstacle);
                        break;
                    }
                }
            }
        }

        if (!isGameComplete)
        {
			for (int i = 0; i < SpawnedLetters.Count; i++)
			{
				if (SpawnedLetters[i] != null && SpawnedLetters[i].Collider != null && Player != null &&
					SpawnedLetters[i].gameObject != null && Player.gameObject != null)
				{
					if (Player.IsTouching(SpawnedLetters[i].Collider))
					{
      //                  if (wantedWord[wordPosition] == ' ')
						//{
      //                      HandleLetterCollection(SpawnedLetters[i-1]);
						//	print("MAHII Vayy: " + wantedWord[wordPosition]);
      //                  }
						//else
                            HandleLetterCollection(SpawnedLetters[i]);
						break;
					}
				}
			}
        }

        if (invicibilityTimer > 0)
            scrollSpeed = Mathf.MoveTowards(scrollSpeed, RunSpeed, Time.deltaTime / RunRecoveryTimeFromHit);
        else
            scrollSpeed = Mathf.MoveTowards(scrollSpeed, RunSpeed, Time.deltaTime / RunRecoverTimeFromJump);


        if (isJumping)
        {
            if (velocity.magnitude < 10.0f)
            {
                PlayerAnimator.Play("InAir");
            }
            else if (velocity.y > 0)
            {
                PlayerAnimator.Play("JumpUp");
            }
            else
            {
                PlayerAnimator.Play("JumpDown");
            }
        }
        else
        {
            if (invicibilityTimer > 0)
                PlayerAnimator.Play("Hit");
            else
                PlayerAnimator.Play("Run");
        }
    }

    IEnumerator EnableTutorial()
	{
		tutorialPanel.SetActive(true);
		yield return new WaitForSeconds(5f);
        tutorialPanel.SetActive(false);
		isPaused=false;

    }

    private void HandleObstacleCollision(Collider2D obstacle)
	{
		invicibilityTimer = MaxInvicibilityTimer;
		Health--;
		tutorialDamaged = true;
		//UpdateHealthGraphics();
		scrollSpeed = HitSpeed;
		velocity = Vector3.up * HitImpulse;
		if (obstacle != null)
		{
			obstacle.enabled = false;
		}

		if (Player != null)
		{
			SpriteRenderer sr = Player.GetComponentInChildren<SpriteRenderer>();
			if (sr != null)
			{
				sr.DOColor(new Color(1.0f, 0f, 0f, 0.5f), MaxInvicibilityTimer).SetEase(Ease.Flash, 12.0f);
			}
		}
	}

	private void HandleLetterCollection(LetterDefinition letter)
	{
		if (wordPosition >= WordLetters.Count) return;

		var letterRef = letter;
		int wordPosCopy = wordPosition;

		if (letter.Collider != null)
		{
			SpriteRenderer sr = letter.Collider.GetComponent<SpriteRenderer>();
			if (sr != null)
			{
				sr.sortingLayerName = "Foreground";
				sr.sortingOrder = 5;
			}
		}

		if (WordLetters[wordPosition] != null)
		{

			letter.transform.DOMove(WordLetters[wordPosition].transform.position, 1.0f);
			letter.transform.DOScale(WordLetters[wordPosition].transform.localScale * WordsScale, 1.0f)
				.OnComplete(() =>
				{
					if (letterRef != null && letterRef.gameObject != null)
					{
						Destroy(letterRef.gameObject);
					}
					if (wordPosCopy < WordLetters.Count && WordLetters[wordPosCopy] != null)
					{
						WordLetters[wordPosCopy].color = Color.white;
					}
				});

		}

		wordPosition++;

		if (wordPosition < wantedWord.Length && wantedWord[wordPosition] != letter.Letter)
		{
			foreach (var let in SpawnedLetters.ToList())
			{
				if (let != null && let != letter && let.gameObject != null)
				{
					Destroy(let.gameObject);
				}
			}
			SpawnedLetters.Clear();
		}

		if (wordPosition >= wantedWord.Length)
		{
			isGameComplete = true;
			ShowWinUI();
		}
	}
	void SpawnObstacle()
	{
		GameObject go = Instantiate(Obstacles[Random.Range(0, Obstacles.Length)]);
		go.SetActive(true);
		go.transform.position = new Vector3(SPAWN_X, FLOOR_HEIGHT, 0f);
		SpawnedObstacles.Add(go.GetComponent<Collider2D>());
	}

	void SpawnLetter()
	{
		if (wordPosition >= wantedWord.Length)
			return;

		char wantedChar = wantedWord[wordPosition];
        // Check if the character is a space
        //if (wantedChar == ' ')
        //{
        //    // Increment the position to skip the space
        //    wordPosition++;

        //    // Recursively call SpawnLetter to process the next character
        //    SpawnLetter();
        //    return; // Exit the current call to avoid spawning anything for the space
        //}
        int idx = wantedChar - 'A';
		GameObject go = Instantiate(Letters[idx]);
		go.SetActive(true);
		go.transform.position = new Vector3(SPAWN_X, LETTER_HEIGHT, 0f);
		go.transform.localScale *= LetterScaleMultiplier;
		LetterDefinition letterDef = go.GetComponent<LetterDefinition>();
		letterDef.Collider.gameObject.AddComponent<SetSpriteColorTintOffset>();
		SpawnedLetters.Add(letterDef);
	}

	void UpdatePlayer()
	{
		// Apply forces
		Player.transform.position += velocity * Time.deltaTime;
		velocity.y -= Gravity * Time.deltaTime;

		// Check ground collision
		if (Player.transform.position.y < FLOOR_HEIGHT)
		{
			//Player.transform.position = Player.transform.position.SetY(FLOOR_HEIGHT);
			velocity.y = 0f;
			isJumping = false;
		}

		if (!isTutorial && Input.touchCount > 0)
		{
			if (Input.GetTouch(0).phase == TouchPhase.Began)
			{
                SoundManager.instance.PlaySingleSound(7);
                jumpHold = true;
			}
			else if (Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(0).phase == TouchPhase.Canceled)
			{
				jumpHold = false;
			}
		}

		if (jumpHold && !isJumping)
		{
			// Apply impulse and Jump!
			velocity = Vector3.up * JumpImpulse;
			//Debug.Log("Jumping " + velocity.ToString() + Time.frameCount);
			isJumping = true;
			scrollSpeed = JumpSpeed;
			jumpHold = false;
			//SoundManager.Instance.PlaySFX("JumpCatMiniGame");
		}
	}

	//void UpdateHealthGraphics()
	//{
	//	Health = Mathf.Max(0, Health);
	//	for (int i = 0; i < Health; i++)
	//	{
	//		ActiveStars[i].gameObject.SetActive(true);
	//		InactiveStars[i].gameObject.SetActive(false);
	//	}
	//	for (int i = Health; i < ActiveStars.Length; i++)
	//	{
	//		InactiveStars[i].gameObject.SetActive(true);
	//		//ActiveStars[i].gameObject.SetActive(false);
	//	}

	//	if (LastHealth != Health)
	//	{
	//		for (int i = Health; i < LastHealth; i++)
	//		{
	//			int idx = i;
	//			Sequence s = DOTween.Sequence()
	//				.Append(ActiveStars[idx].transform.DOMove(ActiveStars[idx].transform.position + Vector3.left * 50.0f + Vector3.up * 50.0f, 0.5f).SetEase(Ease.Linear))
	//				.Append(ActiveStars[idx].transform.DOMove(ActiveStars[idx].transform.position + Vector3.left * 100.0f + Vector3.down * 2000.0f, 3.0f).SetEase(Ease.InSine))
	//				.Insert(0f, ActiveStars[idx].transform.DORotate(new Vector3(0f, 0f, -360.0f * 3.0f), 3.0f, RotateMode.FastBeyond360).SetEase(Ease.InSine))
	//				.OnComplete(() => ActiveStars[idx].gameObject.SetActive(false));
	//		}
	//	}

	//	LastHealth = Health;
	//}

	//public void Restart()
	//{
	//	//SoundManager.Instance.PlaySFX("ClickButton");
	//	print("Restart Button is Called");
	//	//SceneLoader.Instance.ReloadScene();
	//}

	//public void GoBack()
	//{
	//       //SoundManager.Instance.PlaySFX("ClickButton");
	//       print("Go Back Button is Called");
	//       //TransitionManager.Instance.ShowFade(2.0f, () => SceneLoader.Instance.LoadScene("MiniGames")); 
	//   }

	//    public void ShowDeathUI()
	//	{
	//#if UNITY_IOS
	//		if (!ProgressManager.Instance.IsReviewShown(1))
	//		{
	//			Debug.Log("Asking for review!");
	//			UnityEngine.iOS.Device.RequestStoreReview();
	//			ProgressManager.Instance.SetReviewShow(1);
	//		}
	//#endif

	//		string[] sfx = new string[] { "fine_try_again", "try_again", "come_play_again" };
	//		DOTween.Sequence()
	//			.AppendInterval(1.5f);
	//			//.AppendCallback(() => SoundManager.Instance.PlaySFX(sfx.GetRandomElement()));

	//		//SoundManager.Instance.PlaySFX("GameOverMiniGames");
	//		WinPanel.SetActive(false);
	//		LosePanel.SetActive(true);
	//		Fader.gameObject.SetActive(true);
	//		//DOTween.Sequence()
	//		//	.Append(Fader.DOFade(0.8f, 1.0f))
	//		//	.AppendCallback(() => PopUp.SetActive(true))
	//		//	.Append(PopUp.transform.DOScale(Vector3.zero, 0.4f).From().SetEase(Ease.OutBack));

	//        DOTween.Sequence()
	//            .Append(Fader.DOFade(0.8f, 1.0f))
	//            .AppendCallback(() => PopUp.SetActive(true))
	//            .Append(PopUp.transform.DOScale(Vector3.zero, 0.4f).From().SetEase(Ease.OutBack))
	//            .AppendInterval(TransitionDelay)
	//            .OnComplete(() => StartCoroutine(TransitionToPuzzleGame()));
	//    }

	public void ShowWinUI()
	{
#if UNITY_IOS
		if (!ProgressManager.Instance.IsReviewShown(1))
		{
			Debug.Log("Asking for review!");
			UnityEngine.iOS.Device.RequestStoreReview();
			ProgressManager.Instance.SetReviewShow(1);
		}
#endif
		print("You have Won the Game!!");
        for (int i = 0; i < SpawnedObstacles.Count; i++)
        {
            SpawnedObstacles[i].gameObject.SetActive(false);
        }
        //SoundManager.Instance.SetMusicVolume(1.0f, 0.1f);
        //SoundManager.Instance.AddSFXToQueue("FinishMiniGame_1");
        isGameComplete = true;
        PlayerAnimator.Play("Idle");
        StartCoroutine(DisplayTextWithAudio());
		//WinPanel.SetActive(true);
		//LosePanel.SetActive(false);
		//Fader.gameObject.SetActive(true);
		//DOTween.Sequence()
		//	.Append(Fader.DOFade(0.8f, 1.0f))
		//	.AppendCallback(() => PopUp.SetActive(true))
		//	.Append(PopUp.transform.DOScale(Vector3.zero, 0.4f).From().SetEase(Ease.OutBack));

		//DOTween.Sequence()
		//    .Append(Fader.DOFade(0.8f, 1.0f))
		//    .AppendCallback(() => PopUp.SetActive(true))
		//    .Append(PopUp.transform.DOScale(Vector3.zero, 0.4f).From().SetEase(Ease.OutBack))
		//    .AppendInterval(TransitionDelay)
		//    .OnComplete(() => StartCoroutine(TransitionToPuzzleGame()));

		//SoundManager.Instance.AddSFXToQueue(wantedWord.ToLower());
		//string[] sfx = new string[] { "so_fast", "professional_rider", "good_job", "doing_great", "feel_rhythm" };
		//SoundManager.Instance.AddSFXToQueue(sfx.GetRandomElement());
	}
	private IEnumerator DisplayTextWithAudio()
	{
		yield return new WaitForSeconds(1f);
        PlayerAnimator.Play("Idle");
        isPaused = true;
        continentInfoPopUp.SetActive(true);

        for (int i = 0; i < textSegments.Length; i++)
		{
			// Play corresponding audio clip
			if (i < audioClips.Length)
			{
				audioSource.clip = audioClips[i];
				audioSource.Play();
			}

			// Set the cat animation to talking
			catAnimator.SetBool(isTalkingBool, true);

			// Display text with typewriter effect
			yield return StartCoroutine(TypewriterEffect(textSegments[i]));

			// Wait for the audio clip to finish
			while (audioSource.isPlaying)
			{
				yield return null;
			}
		}

		// Set cat animation to idle after all text is displayed
		catAnimator.SetBool(isTalkingBool, false);
        StartCoroutine(ShowTransitionPanel());
	}

    private IEnumerator TypewriterEffect(string text)
	{
		continentDescription.text = ""; // Clear existing text

		foreach (char letter in text)
		{
			continentDescription.text += letter;
			yield return new WaitForSeconds(typewriterSpeed);
		}

		yield return new WaitForSeconds(0.5f); // Small pause after completing the text
	}


    private IEnumerator ShowTransitionPanel()
    {
        transitionPanel.SetActive(true);
		continentInfoPopUp.SetActive(false);
        transitionText.text = "";

        // Play transition panel content
        for (int i = 0; i < nextTextSegments.Length; i++)
        {
            // Play corresponding audio clip for transition
            if (i < transitionAudioClip.Length && transitionAudioClip[i] != null)
            {
                transitionAudioSource.clip = transitionAudioClip[i];
                transitionAudioSource.Play();
            }
            cat2Animator.SetBool(isTalkingBool, true);
            // Show text with typewriter effect
            yield return StartCoroutine(TypewriterEffect(nextTextSegments[i], transitionText));

            // Wait for the audio clip to finish before moving to next segment
            while (transitionAudioSource.isPlaying)
            {
                yield return null;
            }
            cat2Animator.SetBool(isTalkingBool, false);

            yield return new WaitForSeconds(0.5f); // Small pause before next segment
        }

        yield return new WaitForSeconds(0.5f); // Final pause

        transitionPanel.SetActive(false);

        // Now transition to puzzle game
        StartCoroutine(TransitionToPuzzleGame());
    }

    private IEnumerator TypewriterEffect(string text, TMP_Text textComponent = null)
    {
        // If no specific text component provided, use the continentDescription
        if (textComponent == null) textComponent = continentDescription;

        textComponent.text = ""; // Clear existing text

        foreach (char letter in text)
        {
            textComponent.text += letter;
            yield return new WaitForSeconds(typewriterSpeed);
        }

        yield return new WaitForSeconds(0.5f); // Small pause after completing the text
    }



    private IEnumerator TransitionToPuzzleGame()
	{
		isTransitioning = true; // Set transition flag
		isGameComplete = true;
		if (puzzleManager == null || RunnerAndPuzzleManager.SelectedContinent == null)
		{
			Debug.LogError("Cannot transition: Manager or SelectedContinent is null");
			yield break;
		}

		// Fade out current game
		//yield return Fader.DOFade(1f, 1.0f).WaitForCompletion();

		// Clean up all spawned objects
		CleanupGameObjects();

		// Activate puzzle game through the manager
		puzzleManager.ActivatePuzzleGame();

		// Destroy the runner game object
		DestroyRunnerGame();

		// Fade in puzzle game
		//yield return Fader.DOFade(0f, 1.0f).WaitForCompletion();
	}

	private void CleanupGameObjects()
	{
		// Clean up obstacles
		if (SpawnedObstacles != null)
		{
			foreach (var obstacle in SpawnedObstacles.ToList())
			{
				if (obstacle != null && obstacle.gameObject != null)
				{
					Destroy(obstacle.gameObject);
				}
			}
			SpawnedObstacles.Clear();
		}

		// Clean up letters
		if (SpawnedLetters != null)
		{
			foreach (var letter in SpawnedLetters.ToList())
			{
				if (letter != null && letter.gameObject != null)
				{
					Destroy(letter.gameObject);
				}
			}
			SpawnedLetters.Clear();
		}

		// Clean up word letters
		if (WordLetters != null)
		{
			foreach (var letter in WordLetters.ToList())
			{
				if (letter != null && letter.gameObject != null)
				{
					Destroy(letter.gameObject.transform.parent.gameObject);
				}
			}
			WordLetters.Clear();
		}
	}

	private void DestroyRunnerGame()
	{
		// Find the parent runner game object (assuming it's the parent of this script)
		Transform runnerParent = transform.parent ? transform.parent : transform;

		// Disable the game object first
		runnerParent.gameObject.SetActive(false);

		// Destroy the entire runner game object
		Destroy(runnerParent.gameObject);
	}

	private void OnDestroy()
	{
		// Final cleanup when the script is destroyed
		CleanupGameObjects();
	}
	
		public float paddingThreshold = 0.2f; // Extra padding on left & right (adjustable)

    void CreateWord(int idx)
    {
        GameObject word = Instantiate(Words[idx]);

        Transform letterRoot = word.transform.GetChild(0);

        float totalWidth = 0f;
        List<SpriteRenderer> letterRenderers = new List<SpriteRenderer>();

        // Calculate total width of the word
        for (int i = 0; i < letterRoot.childCount; i++)
        {
            SpriteRenderer sr = letterRoot.GetChild(i).GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                totalWidth += sr.bounds.size.x; // Sum up letter widths
                letterRenderers.Add(sr);
            }
        }

        // Get the screen width in world units
        float screenWidth = Camera.main.orthographicSize * 2f * Camera.main.aspect;

        // Calculate the maximum allowed width with padding
        float maxAllowedWidth = screenWidth * (1f - paddingThreshold * 2f);

        // Determine if scaling is needed
        float scaleFactor = 1f;
        float maxScaleFactor = 0.15f;
        if (totalWidth > maxAllowedWidth)
        {
            scaleFactor = maxAllowedWidth / totalWidth; // Scale factor to fit within screen
        }

        // Ensure scaleFactor does not exceed maxScaleFactor
        if (scaleFactor > maxScaleFactor)
        {
            scaleFactor = maxScaleFactor;
        }

        // 🔹 **Recalculate `totalWidth` after applying `scaleFactor`**
        totalWidth = 0f;
        foreach (var sr in letterRenderers)
        {
            totalWidth += sr.bounds.size.x * scaleFactor; // Adjust for scaled width
        }

        // Calculate centered positioning with updated width
        float startX = -totalWidth / 1.9f;
        float currentX = startX;
		int spaceIndex = -1;
        if (wantedWord == "NORTHAMERICA")
           spaceIndex = 5;
		else if (wantedWord == "SOUTHAMERICA")
            spaceIndex = 5;

        if (spaceIndex != -1)
        {
            Debug.Log("Space found at index: " + spaceIndex);
        }
        else
        {
            Debug.Log("No space found in the string.");
        }
        // Apply positioning and scaling
        for (int i = 0; i < letterRenderers.Count; i++)
        {
			if(spaceIndex == i)
			{
                currentX += (letterRenderers[i-1].bounds.size.x * scaleFactor + 0.6f)-0.2f; // Adjust position
            }
			if (spaceIndex+1 == i && spaceIndex!=0)
				currentX += 0.08f;
            letterRenderers[i].transform.localPosition = new Vector3(currentX, 0, 0);
            if (wantedWord == "NorthAmerica")
                letterRenderers[i].transform.localScale *= scaleFactor * 0.75f; // Apply scale
			else
				letterRenderers[i].transform.localScale *= scaleFactor*0.95f; // Apply scale
            currentX += Mathf.Clamp(letterRenderers[i].bounds.size.x * scaleFactor + letterRenderers[i].bounds.size.x, 0f,0.634858f); // Adjust position
            letterRenderers[i].color = new Color(0f, 0f, 0f, 0.5f);
            WordLetters.Add(letterRenderers[i]);
        }
        if (wantedWord == "NorthAmerica")
		{
            word.transform.position = new Vector3(-0.36f, 4.35f, 0f);
		}else
            word.transform.position = new Vector3(0f, 4.35f, 0f);
            // Set word's center position on screen
    }


	public void Back()
	{
        SoundManager.instance.PlaySingleSound(1);
        SceneManager.LoadSceneAsync("MainMenu");
	}


    IEnumerator TutorialCoroutine()
	{
		obstacleNextDistance = 20.0f;
		letterNextDistance = 40.0f;
		bool gotInput = false;

		//SoundManager.Instance.AddSFXToQueue("skateride_collect");

		// Show Obstacle Tutorial
		yield return new WaitUntil(() => distance > 32.0f);
		isPaused = true;
		TutorialFocus.gameObject.SetActive(true);
		TutorialFocus.transform.position = Camera.main.WorldToScreenPoint(SpawnedObstacles[0].transform.position + Vector3.up * 1.0f);
		TutorialBackdrop.gameObject.SetActive(true);
		float initialAlpha = TutorialBackdrop.color.a;
		TutorialBackdrop.color = Color.clear;
		SetSpriteColorTintOffset spriteOffset = SpawnedObstacles[0].GetComponent<SetSpriteColorTintOffset>();

		//SoundManager.Instance.AddSFXToQueue("avoid_rocks"); 

		Sequence fadeSequence = DOTween.Sequence()
			.Append(TutorialBackdrop.DOFade(initialAlpha, 2.0f))
			.Join(DOTween.To(spriteOffset.GetOffsetAlpha, spriteOffset.SetOffsetAlpha, 0.75f, 2.0f).SetEase(Ease.Flash, 8.0f))
			.AppendInterval(0.5f)
			.Append(TutorialBackdrop.DOFade(0f, 1.0f));
		yield return new WaitUntil(() => !fadeSequence.IsActive() || Input.touchCount > 0);

		if (fadeSequence.IsActive())
			fadeSequence.Kill(true);

		float hintCooldown = -1.0f;
		while (!gotInput)
		{
			hintCooldown -= Time.deltaTime;
			if (hintCooldown < 0)
			{
				FingerHintController.Instance.ShowTapHint(Vector3.zero);
				DOTween.Sequence()
					.AppendInterval(1.0f)
					.AppendCallback(() =>
					{
						//SoundManager.Instance.PlaySFX("jump");
					});
				hintCooldown = 5.0f;
			}
			//yield return new WaitForSeconds(2.0f);
			gotInput = (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);
			yield return null;
		}
		FingerHintController.Instance.Hide();

		isPaused = false;
		jumpHold = true;

		// Show Letter Tutorial
		yield return new WaitUntil(() => distance > 52.0f);
		isPaused = true;
		TutorialFocus.transform.position = Camera.main.WorldToScreenPoint(SpawnedLetters[0].transform.position + Vector3.up * 1.0f);
		TutorialBackdrop.color = Color.clear;
		spriteOffset = SpawnedLetters[0].Collider.GetComponent<SetSpriteColorTintOffset>();

		//SoundManager.Instance.AddSFXToQueue("collect_to_form");

		fadeSequence = DOTween.Sequence()
			.Append(TutorialBackdrop.DOFade(initialAlpha, 2.0f))
			.Join(DOTween.To(spriteOffset.GetOffsetAlpha, spriteOffset.SetOffsetAlpha, 0.75f, 2.0f).SetEase(Ease.Flash, 8.0f))
			.Append(TutorialBackdrop.DOFade(0f, 1.0f));
		yield return new WaitUntil(() => !fadeSequence.IsActive() || Input.touchCount > 0);

		if (fadeSequence.IsActive())
			fadeSequence.Kill(true);

		TutorialFocus.transform.position = Camera.main.WorldToScreenPoint(WordLetters[0].transform.position + Vector3.up * 0.75f);
		TutorialFocus.transform.localScale *= 0.5f;
		TutorialBackdrop.color = Color.clear;

		//SoundManager.Instance.AddSFXToQueue("jump_collect"); 

		fadeSequence = DOTween.Sequence()
			.Append(TutorialBackdrop.DOFade(initialAlpha, 2.0f))
			.Join(WordLetters[0].DOFade(0f, 2.0f).SetEase(Ease.Flash, 8.0f))
			.AppendInterval(0.5f)
			.Append(TutorialBackdrop.DOFade(0f, 1.0f));
		yield return new WaitUntil(() => !fadeSequence.IsActive() || Input.touchCount > 0);

		if (fadeSequence.IsActive())
			fadeSequence.Kill(true);

		gotInput = false;
		hintCooldown = -1.0f;
		while (!gotInput)
		{
			hintCooldown -= Time.deltaTime;
			if (hintCooldown < 0)
			{
				FingerHintController.Instance.ShowTapHint(Vector3.zero);
				DOTween.Sequence()
					.AppendInterval(1.0f)
					.AppendCallback(() =>
					{
						//SoundManager.Instance.AddSFXToQueue("jump");
					});
				hintCooldown = 5.0f;
			}
			//yield return new WaitForSeconds(2.0f);
			gotInput = (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);
			yield return null;
		}
		FingerHintController.Instance.Hide();
		isPaused = false;
		jumpHold = true;

		// Show Hit
		yield return new WaitUntil(() => tutorialDamaged);
		yield return new WaitForSeconds(0.5f);

		isPaused = true;
		//TutorialFocus.transform.position = ActiveStars[1].transform.position;
		TutorialFocus.transform.localScale *= 2.0f;
		TutorialBackdrop.color = Color.clear;

		//SoundManager.Instance.AddSFXToQueue("try_avoid_rocks"); 

		fadeSequence = DOTween.Sequence()
			.Append(TutorialBackdrop.DOFade(initialAlpha, 2.0f))
			.Append(TutorialBackdrop.DOFade(0f, 1.0f));
		yield return new WaitUntil(() => !fadeSequence.IsActive() || Input.touchCount > 0);

		if (fadeSequence.IsActive())
			fadeSequence.Kill(true);

		//isPaused = false;
		yield return new WaitForSeconds(3.0f);

		TutorialFocus.gameObject.SetActive(false);
		TutorialBackdrop.gameObject.SetActive(false);

		isTutorial = false;
		//ProgressManager.Instance.SetTutorialShown(1);

		//TransitionManager.Instance.ShowFade(2.0f, () => Restart());
	}
}

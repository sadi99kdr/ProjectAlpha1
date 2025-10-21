using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//-------------------------------------------------------------
public class GameManager : MonoBehaviour
{
    public Card cardPrefab;
    public Transform cardContainer;
    [SerializeField] Sprite[] sprites;

    private List<Sprite> spritePairs;

    Card firstSelected;
    Card secondSelected;
    public int matchCounts;
    public int turnsCount = 0;
    public int totalPoints;

    [Header("UI")]
    public Text winningText;
    public Text turnText;
    public Text pointText;
    [SerializeField] Text timerText;
    float elapsedTimer;

    private bool isGameWon = false;
    private bool isCheckingMatch = false;
    private bool isGameActive = false;
    private bool timerRunning = false;
    public float gameDuration = 60f;

    public Button nextButton;
    public Button retryButton;

    [Header("Sound Effects")]
    public AudioSource audioSource;
    public AudioClip matchSound;
    public AudioClip mismatchSound;
    public AudioClip gameWinSound;
    public AudioClip gameOverSound;


    private void Start()
    {
      
        ResetGameState();

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
            }
        }

        Debug.Log("Total Score: " + ScoreData.totalScore);
        pointText.text = "Total Points: " + ScoreData.totalScore;
        turnsCount = 0;

        PrepareSprites();
        CreateCards();

        StartCoroutine(startGameAfterPreview());

    }
    public void ResetGameState()
    {
        isGameActive = false;
        isCheckingMatch = false;
        firstSelected = null;
        secondSelected = null;
        matchCounts = 0;
        turnsCount = 0;
        totalPoints = 0;
        elapsedTimer = 0f;
        timerRunning = false;
        isGameWon = false;

        // UI
        winningText.gameObject.SetActive(false);
        nextButton.gameObject.SetActive(false);
        retryButton.gameObject.SetActive(false);
        UpdateUI();

        foreach (Transform child in cardContainer)
        {
            Card card = child.GetComponent<Card>();
            card.HideIcon();
            card.isSelected = false;
        }
    }
    private void PrepareSprites()
    {
        spritePairs = new List<Sprite>();
        for (int i = 0; i < sprites.Length; i++)
        {
            // adding 2 spites to make pair
            spritePairs.Add(sprites[i]);
            spritePairs.Add(sprites[i]);
        }

        //shuffle
        ShuffleSprites(spritePairs);
    }

    //Instantiate the cards
    void CreateCards()
    {

        foreach (Transform child in cardContainer)
        {
            Destroy(child.gameObject); // clear old cards
        }

        for (int i = 0; i < spritePairs.Count; i++)
        {
            Card card = Instantiate(cardPrefab, cardContainer);
            card.SetIconsSprite(spritePairs[i]);
            card.gameManager = this;
        }
    }

    // Show the cards at 3 seconds
    IEnumerator startGameAfterPreview()
    {
        //Display
        foreach (Transform child in cardContainer)
        {
            Card card = child.GetComponent<Card>();
            card.ShowIcon();
            card.isSelected = true;
        }
        yield return new WaitForSeconds(3f);

        //Hide
        foreach (Transform child in cardContainer)
        {
            Card card = child.GetComponent<Card>();
            card.HideIcon();
            card.isSelected = false;
        }
        isGameActive = true;
        timerRunning = true;
        Debug.Log("Game Active! Start Playing");

    }

    public void SetSelected(Card card)
    {
        if (!isGameActive || isCheckingMatch)
        {
            return;
        }
        if (card == firstSelected) return;

        if (card.isSelected == false)
        {
            card.ShowIcon();

            if (firstSelected == null)
            {
                firstSelected = card;
                return;
            }

            if (secondSelected == null)
            {
                secondSelected = card;

                turnsCount++;
                UpdateUI();

                StartCoroutine(CheckMatching(firstSelected, secondSelected));
            }
        }

    }

    IEnumerator CheckMatching(Card a, Card b)
    {
        isCheckingMatch = true;
        yield return new WaitForSeconds(1f);

        if (a.iconSprite == b.iconSprite)
        {
            float matchSoundLength = 0f;
            if (matchSound != null)
            {
                audioSource.PlayOneShot(matchSound);
                matchSoundLength = matchSound.length;
            }
            matchCounts++;
            totalPoints += 2;

            if (matchCounts >= spritePairs.Count / 2)
            {
                isGameWon = true;
                isGameActive = false;
                yield return new WaitForSeconds(matchSoundLength + 0.1f);


                // level1 game completed animations
                //if (gameWinSound != null)
                //{
                //    audioSource.PlayOneShot(gameWinSound);
                //}
               
                winningText.gameObject.SetActive(true);
                nextButton.gameObject.SetActive(true);
                //.....
                
                yield return new WaitForSeconds(2f);
                //load to next level
               
            }
            // Debug.Log("Card Matched");
        }
        else
        {
            if (mismatchSound != null)
            {
                audioSource.PlayOneShot(mismatchSound);
            }

            totalPoints = Mathf.Max(0, totalPoints - 1);
            a.HideIcon();
            b.HideIcon();
        }

        UpdateUI();
        firstSelected = null;
        secondSelected = null;
        isCheckingMatch = false;
    }

    void ShuffleSprites(List<Sprite> spriteList)
    {
        for (int i = spriteList.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);

            //swap

            Sprite temp = spriteList[i];
            spriteList[i] = spriteList[randomIndex];
            spriteList[randomIndex] = temp;
        }
    }
    void UpdateUI()
    {
        if (turnText != null)
        {
            turnText.text = "Turns: " + turnsCount;
        }

        if (pointText != null)
        {
            pointText.text = "Points: " + totalPoints;
        }
    }

    void GameOver()
    {
        winningText.text = "Game Over!...";
        winningText.gameObject.SetActive(true);

        if (gameOverSound != null)
        {
            audioSource.PlayOneShot(gameOverSound);
        }


        retryButton.gameObject.SetActive(true);

        foreach (Transform child in cardContainer)
        {
            child.GetComponent<Button>().interactable = false;
        }
        isGameActive = false;
        timerRunning = false;
    }

    public void LoadNextLevel()
    {
        if (gameWinSound != null)
        {
            audioSource.PlayOneShot(gameWinSound);
        }
        //save current level points
        ScoreData.totalScore += totalPoints;
        totalPoints = 0;

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("Game Complete!");
            SceneManager.LoadScene(0);
        }

    }

    public void RestartLevel()
    {
        ResetGameState();

        PrepareSprites();
        CreateCards();

        StartCoroutine(startGameAfterPreview());
    }

    public void OnclickRetryButton()
    {
        retryButton.gameObject.SetActive(false);
        RestartLevel();
    }

    private void Update()
    {
        if (isGameWon || !timerRunning)
            return;

        // Countdown timer logic
        elapsedTimer += Time.deltaTime;
        float timeLeft = gameDuration - elapsedTimer;

        if (timeLeft > 0)
        {
            timerText.text = Mathf.CeilToInt(timeLeft).ToString();
        }
        else
        {
            GameOver();
            timerRunning = false;
            timerText.text = "0";
        }

    }

    public void OnclickMenu()
    {
        SceneManager.LoadScene(0);
    }


}
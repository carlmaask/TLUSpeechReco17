using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;

public class GameEngine : MonoBehaviour
{
    private List<string> wordHolder = new List<string>();
    private string[] words;
  
    public Text results;
    public Image target;
    public Text Score;
    public Text timeLeft;
    public Text endScore;

    public GameObject pauseMenu;
    public GameObject endScreen;
    public GameObject unableSpeechReco;

    private ConfidenceLevel confidence = ConfidenceLevel.Medium;
    private KeywordRecognizer recognizer;

    protected string word;
    protected string currentWord;
    protected int counter;

    protected float score;
    protected float timer;
    protected float displayTimer;

    protected bool started = true;
    protected bool paused;
    protected bool firstClick = true;
    protected bool timeOver = false;
    private void Start()
    {
        Object[] sprites = Resources.LoadAll<Sprite>("");

        foreach (Object sprite in sprites)
        {
            wordHolder.Add(sprite.name);
        }

        words = wordHolder.ToArray();

        if (words != null)
        {
            started = true;
            paused = false;
            Time.timeScale = 1.0f;
            currentWord = words[0];
            changeImage(currentWord);
            counter = 0;
            score = 0;
            timer = 30;
            displayTimer = 5;
            Score.text = score.ToString();
            timeLeft.text = timer.ToString();
            speechRecoEnabled();
        }
    }

    private void NewRecognizer()
    {
        recognizer = new KeywordRecognizer(words, confidence);
    }

    private void Recognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
            if (args.text == currentWord)
            {
                if (words.Length != counter)
                {
                    rightWord(currentWord);
                } else {
                    rightWord(currentWord);
                    endGame(score);
                }
            }
    }

    private void Update()
    {
        if (Input.GetKeyDown("escape"))
        {
            pauseGame();
        }

        if (started == true) {
            if (timeOver == false)
            {
                timer -= Time.deltaTime;
                if (timer < 0) {
                    timeOver = true;
                    if (words.Length != counter) { 
                        wrongWord(currentWord, 1);
                    } else if (counter > words.Length || counter == words.Length) {
                        wrongWord(currentWord, 1);
                        endGame(score);
                    }
                }
            }
        }
    }

    private void FixedUpdate()
    {
        updateTimerScore();
    }

    private void updateTimerScore()
    {
        Score.text = score.ToString();
        timeLeft.text = Mathf.Round(timer).ToString();
    }

    private void nextWord()
    {
        if (words.Length != counter) {
            timeOver = false;
            counter++;
            timer = 30.0f;
            currentWord = words[Random.Range(0, words.Length)];
            changeImage(currentWord);
            firstClick = true;
        } else {
            endGame(score);
        }
    }

    private void rightWord(string word)
    {
        displayResultText(true, word);
        score += Mathf.Round(timer) + 1;
    }

    private void wrongWord(string word, int type)
    {
        displayResultText(false, word, type);
    }

    public void requestNextWord()
    {
        if (firstClick)
        {
            displayHelpText(currentWord);
            firstClick = false;
        } else {
            wrongWord(currentWord, 2);
        }
    }

    private void displayResultText(bool correct, string word, int type = 1)
    {
        if (correct)
        {
            results.color = Color.green;
            results.text = "Õige! Sõna oli: <b> " + word + "</b>";
            StartCoroutine(dissapearText());
        }
        else
        {
            results.color = Color.red;
            StartCoroutine(dissapearText());
            switch(type) {
                case 1:
                    results.text = "Aeg sai otsa! Sõna oli: <b> " + word + "</b>";
                    break;
                case 2:
                    results.text = "Sõna oli: <b> " + word + "</b>";
                    break;
            }
        }
        
    }

    private void displayHelpText(string word)
    {
        results.color = Color.yellow;
        results.text = "Käesolev sõna on " + word.Length + " tähemärki pikk.";
    }

    IEnumerator dissapearText()
    {
        yield return new WaitForSeconds(displayTimer);
        results.text = "";
        nextWord();
    }

    private void displayPauseMenu()
    {
        if (pauseMenu.activeInHierarchy == false)
        {
            pauseMenu.SetActive(true);
        } else {
            pauseMenu.SetActive(false);
        }
    }

    private void changeImage(string img) {

        Image targetImg;

        targetImg = target.GetComponent<Image>();

        Sprite newSprite = Resources.Load<Sprite>(img);

        if (newSprite)
        {
            targetImg.sprite = newSprite;
        } else {
            Debug.Log("no image found with: " + img);
        }
    }

    public void pauseGame()
    {
        if (paused)
        {
            displayPauseMenu();
            Time.timeScale = 1.0f;
            paused = false;
            recognizer.Stop();
        }
        else
        {
            displayPauseMenu();
            Time.timeScale = 0.0f;
            paused = true;
            recognizer.Start();
        }
    }

    public void endGame(float score)
    {
        endScreen.SetActive(true);
        Time.timeScale = 0.0f;
        paused = true;
        started = false;
        endScore.text = score.ToString();
        recognizer.Stop();
    }

    public void quitGame()
    {
        Application.Quit();
    }

    private void OnApplicationQuit()
    {
        if (recognizer != null && recognizer.IsRunning)
        {
            recognizer.OnPhraseRecognized -= Recognizer_OnPhraseRecognized;
            recognizer.Stop();
        }
    }

    public void speechRecoEnabled()
    {
        if (PhraseRecognitionSystem.isSupported) {
            NewRecognizer();
            recognizer.OnPhraseRecognized += Recognizer_OnPhraseRecognized;
            recognizer.Start();
        } else {
            unableSpeechReco.SetActive(true);
            Time.timeScale = 0.0f;
            paused = true;
            started = false;
        }
    }
}

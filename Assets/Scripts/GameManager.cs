using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public enum LevelType
    {
        Easy,
        Medium,
        Hard
    }
    [SerializeField] Camera mainCamera;
    [SerializeField] Board board;
    [SerializeField] Highscore highscore;

    // UI
    [SerializeField] GameObject startPanel;
    [SerializeField] GameObject endPanel;
    [SerializeField] GameObject gamePanel;
    [SerializeField] Text scoreText;
    [SerializeField] Text highscoreText;
    [SerializeField] Text timerText;
    [SerializeField] Text flagsText;
    LevelType difficulty;

    // Sound effects
    public AudioClip loseSound;
    public AudioClip winSound;
    public AudioClip defaultMusic;



    bool inGame = false;
    float timer;

    // Start is called before the first frame update
    void Start()
    {
        SoundManager.Instance.PlayMusic(defaultMusic);
    }

    // Update is called once per frame
    void Update()
    {
        if (inGame)
        {
            timer += Time.deltaTime;
            timerText.text = timer.ToString("000");
            flagsText.text = board.flagCount().ToString("00");
        }
    }

    public void startGame(int diff)
    {
        board.resetBoard();
        gamePanel.SetActive(true);
        switch ((LevelType)diff)
        {
            case LevelType.Easy:
                difficulty = LevelType.Easy;
                board.setup(10, 10, 10);
                mainCamera.orthographicSize = 6;
                break;
            case LevelType.Medium:
                difficulty = LevelType.Medium;
                board.setup(16, 16, 40);
                mainCamera.orthographicSize = 9;
                break;
            case LevelType.Hard:
                difficulty = LevelType.Hard;
                board.setup(30, 16, 99);
                mainCamera.orthographicSize = 10.8f;
                break;
        }
        inGame = true;
        board.gameObject.SetActive(true);
        startPanel.SetActive(false);
    }

    public void endGame(bool won)
    {
        inGame = false;
        if (won)
        {
            if (timer < highscore.getScore(difficulty) || highscore.getScore(difficulty) == 0)
            {
                highscore.setScore(difficulty, timer);
            }
            scoreText.text = timer.ToString("000");
            SoundManager.Instance.Play(winSound);
        }
        else {
            scoreText.text = "-";
            SoundManager.Instance.Play(loseSound);
        }

        if (highscore.getScore(difficulty) != 0)
        {
            highscoreText.text = highscore.getScore(difficulty).ToString("000");
        }
        else
        {
            highscoreText.text = "-";
        }
        endPanel.SetActive(true);
    }
    public void restartGame()
    {
        timer = 0f;
        endPanel.SetActive(false);
        gamePanel.SetActive(false);
        startPanel.SetActive(true);
    }

    public void resetgame()
    {
        restartGame();
        startGame((int)difficulty);
    }
}

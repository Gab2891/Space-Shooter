using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text _scoreText;
    [SerializeField]
    private Sprite[] _liveSprites;
    [SerializeField]
    private Image _livesImg;
    [SerializeField]
    private Text _gameOverText;
    [SerializeField]
    private Text _restartText;
    private GameManager _gameManager;
    private int _bestScore;
    [SerializeField]
    private Text _bestScoreText;

    // Start is called before the first frame update
    void Start()
    {
        _bestScore = PlayerPrefs.GetInt("BestScore", 0); ;
        _bestScoreText.text = "Best: " + _bestScore;
        _scoreText.text = "Score: " + 0;
        _gameOverText.gameObject.SetActive(false);
        _restartText.gameObject.SetActive(false);
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (_gameManager == null)
            Debug.LogError("Game Manager is null");
    }
    
    public void ChangeScore(int score)
    {
        _scoreText.text = "Score: " + score;
        CheckForBestScore(score);
    }

    public void UpdateLives(int currentLives)
    {
        if (currentLives < 0)
            currentLives = 0;
        _livesImg.sprite = _liveSprites[currentLives];
        if (currentLives < 1)
            GameOverSequence();
    }

    void GameOverSequence()
    {
        PlayerPrefs.SetInt("BestScore", _bestScore);
        _gameManager.GameOver();
        _restartText.gameObject.SetActive(true);
        _gameOverText.gameObject.SetActive(true);
        StartCoroutine(GameOverFlickerRoutine());
    }

    IEnumerator GameOverFlickerRoutine()
    {
        while (true)
        {
            _gameOverText.text = "Game Over";
            yield return new WaitForSeconds(0.5f);
            _gameOverText.text = "";
            yield return new WaitForSeconds(0.5f);
        }
        
    }

    void CheckForBestScore(int currentScore)
    {
        if (currentScore > _bestScore)
        {
            _bestScore = currentScore;
            _bestScoreText.text = "Best: " + _bestScore;
        }
    }
}
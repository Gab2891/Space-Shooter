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
    [SerializeField]
    GameObject[] _shieldImgs;
    [SerializeField]
    private Text _ammoText;
    [SerializeField]
    private Image _thrusterImg;
    [SerializeField]
    private Text _thrusterText;
    [SerializeField]
    private Color _emptyThrusterColor = new Color(0.4f, 0.4f, 0.4f, 1.0f);
    private float _colorRedRange = 0.0f, _colorGreenRange = 0.0f, _colorBlueRange = 0.0f;
    [SerializeField]
    private Text _waveText;
    [SerializeField]
    private Text _remainingEnemiesText;

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

        _colorRedRange = 1.0f - _emptyThrusterColor.r;
        _colorGreenRange = 1.0f - _emptyThrusterColor.g;
        _colorBlueRange = 1.0f - _emptyThrusterColor.b;
    }
    
    public void WaveInfoChange(int actualWave, int totalWave)
    {
        _waveText.text = "Wave: " + actualWave + "/" + totalWave;
    }

    public void RemainingEnemiesChange(int enemies)
    {
        _remainingEnemiesText.text = "Enemies: " + enemies;
    }

    public void ChangeScore(int score)
    {
        _scoreText.text = "Score: " + score;
        CheckForBestScore(score);
    }

    public void ChangeAmmo(int ammo, int maxAmmo)
    {
        _ammoText.text = "x "+ammo+"/"+maxAmmo;
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

    public void UpdateShield(int shieldResistance)
    {
        if (shieldResistance < 0)
            shieldResistance = 0;
        switch (shieldResistance)
        {
            case 0:
                for (int i = 0; i < _shieldImgs.Length; i++)
                    _shieldImgs[i].SetActive(false);
                break;
            case 1:
                _shieldImgs[0].SetActive(true);
                for (int i = 1; i < _shieldImgs.Length; i++)
                    _shieldImgs[i].SetActive(false);
                break;
            case 2:
                for (int i = 0; i < shieldResistance; i++)
                    _shieldImgs[i].SetActive(true);
                _shieldImgs[shieldResistance].SetActive(false);
                break;
            default:
                for (int i = 0; i < _shieldImgs.Length; i++)
                    _shieldImgs[i].SetActive(true);
                break;
        }
    }

    public void UpdateThruster(float percentage)
    {
        _thrusterText.text = Mathf.RoundToInt(percentage) + "%";
        float normalizeColorRed = ((percentage * _colorRedRange) / 100) + _emptyThrusterColor.r;
        float normalizeColorGreen = ((percentage - _colorGreenRange) / 100) + _emptyThrusterColor.g;
        float normalizeColorBlue = ((percentage - _colorBlueRange) / 100) + _emptyThrusterColor.b;
        _thrusterImg.color = new Color(normalizeColorRed, normalizeColorGreen, normalizeColorBlue, 1.0f);
    }
}
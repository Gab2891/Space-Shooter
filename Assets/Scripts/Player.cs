using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4.5f;
    [SerializeField]
    private float _speedMultiplier = 2.0f;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _tripleShotPrefab;
    [SerializeField]
    private float _fireRate = 0.25f;
    private float _canFire = -1.0f;
    [SerializeField]
    private int _lives = 3;
    private SpawnManager _spawnManager;
    [SerializeField]
    private bool _isTripleShotActive = false;
    [SerializeField]
    private bool _isSpeedUpActive = false;
    [SerializeField]
    private bool _isShieldActive = false;
    [SerializeField]
    private GameObject _shieldVisualizer;
    [SerializeField]
    private int _score;
    private UIManager _uiManager;
    [SerializeField]
    private GameObject _leftEngine, _rightEngine;
    [SerializeField]
    private AudioClip _laserSoundClip;
    [SerializeField]
    private AudioClip _explosionSoundClip;
    private AudioSource _audioSource;
    private GameManager _gameManager;
    [SerializeField]
    private bool _isPlayerOne = true;
    private PlayerAnimation _playerAnimation;

    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (_gameManager == null)
            Debug.LogError("The Game Manager is NULL");

        if(!_gameManager.IsCoopMode())
            transform.position = new Vector3(0, 0, 0);

        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        if(_spawnManager == null)
            Debug.LogError("The Spawn Manager is NULL");

        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        if (_uiManager == null)
            Debug.LogError("The UI Manager is NULL");

        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
            Debug.LogError("Audio Source in player is null");
        else
            _audioSource.clip = _laserSoundClip;

        _playerAnimation = GetComponent<PlayerAnimation>();
        if (_playerAnimation == null)
            Debug.LogError("PlayerAnimation is null");
    }

    // Update is called once per frame
    void Update()
    {
        if (_isPlayerOne)
        {
            CalculateMovement();

            #if UNITY_ANDROID || UNITY_IOS
            if ((Input.GetKeyDown(KeyCode.Space) || CrossPlatformInputManager.GetButtonDown("Shot")) && Time.time > _canFire)
                FireLaser();
            #else
            if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && Time.time > _canFire)
                FireLaser();
            #endif
        }
        else
        {
            PlayerTwoMovement();
            if ((Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetMouseButtonDown(1)) && Time.time > _canFire)
                FireLaser();
        }

    }

    void FireLaser()
    {
        _canFire = Time.time + _fireRate;

        if (_isTripleShotActive)
            Instantiate(_tripleShotPrefab, transform.position + new Vector3(0, 0, 0), Quaternion.identity);
        else
            Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.0f, 0), Quaternion.identity);
        
        _audioSource.Play();
    }

    void CalculateMovement()
    {
        float horizontalInput;
        float verticalInput;

        #if UNITY_ANDROID || UNITY_IOS
        horizontalInput = CrossPlatformInputManager.GetAxis("Horizontal");
        verticalInput = CrossPlatformInputManager.GetAxis("Vertical");
        #else
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        #endif

        if (horizontalInput > 0)
        {
            _playerAnimation.LeftMovOff();
            _playerAnimation.RightMovOn();
        }
        else if (horizontalInput < 0)
        {
            _playerAnimation.RightMovOff();
            _playerAnimation.LeftMovOn();
        }
        else
        {
            _playerAnimation.RightMovOff();
            _playerAnimation.LeftMovOff();
        }

        float actualSpeed = _speed;
        if (_isSpeedUpActive)
            actualSpeed *= _speedMultiplier;

        transform.Translate(new Vector3(horizontalInput, verticalInput, 0) * actualSpeed * Time.deltaTime);

        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -3.8f, 0), 0);

        if (transform.position.x > 11.3f)
        {
            transform.position = new Vector3(-11.3f, transform.position.y, 0);
        }
        else if (transform.position.x < -11.3f)
        {
            transform.position = new Vector3(11.3f, transform.position.y, 0);
        }
    }

    void PlayerTwoMovement()
    {
        float actualSpeed = _speed;
        if (_isSpeedUpActive)
            actualSpeed *= _speedMultiplier;

        if (Input.GetKey(KeyCode.Keypad8))
            transform.Translate(Vector3.up * actualSpeed * Time.deltaTime);
        if (Input.GetKey(KeyCode.Keypad2))
            transform.Translate(Vector3.down * actualSpeed * Time.deltaTime);
        if (Input.GetKey(KeyCode.Keypad6))
        {
            _playerAnimation.LeftMovOff();
            _playerAnimation.RightMovOn();
            transform.Translate(Vector3.right * actualSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.Keypad4))
        {   
            _playerAnimation.RightMovOff();
            _playerAnimation.LeftMovOn();
            transform.Translate(Vector3.left * actualSpeed * Time.deltaTime);
        }
        else
        {
            _playerAnimation.RightMovOff();
            _playerAnimation.LeftMovOff();
        }
            
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -3.8f, 0), 0);

        if (transform.position.x > 11.3f)
        {
            transform.position = new Vector3(-11.3f, transform.position.y, 0);
        }
        else if (transform.position.x < -11.3f)
        {
            transform.position = new Vector3(11.3f, transform.position.y, 0);
        }
    }

    public void Damage()
    {
        if(_isShieldActive)
        {
            _isShieldActive = false;
            _shieldVisualizer.SetActive(_isShieldActive);
            return;
        }
        _lives--;

        if (_lives == 2)
            _leftEngine.SetActive(true);
        else if (_lives == 1)
            _rightEngine.SetActive(true);

        _uiManager.UpdateLives(_lives);

        if (_lives < 1)
        {
            _audioSource.clip = _explosionSoundClip;
            _audioSource.Play();
            _spawnManager.OnPlayerDeath();
            Destroy(this.gameObject);
        }
    }

    public void TripleShotActive()
    {
        _isTripleShotActive = true;
        StartCoroutine(TripleShotPowerDownRoutine());
    }

    IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isTripleShotActive = false;
    }

    public void SpeedUpActive()
    {
        _isSpeedUpActive = true;
        StartCoroutine(SpeedDownRoutine());
    }

    IEnumerator SpeedDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isSpeedUpActive = false;
    }

    public void ShieldActive()
    {
        _isShieldActive = true;
        _shieldVisualizer.SetActive(_isShieldActive);
    }

    public void AddScore(int points)
    {
        _score += points;
        _uiManager.ChangeScore(_score);
    }
}
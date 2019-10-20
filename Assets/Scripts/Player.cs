using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour
{
    [SerializeField]
    private int _maxAmmo = 15;
    [SerializeField]
    private float _speed = 4.5f;
    [SerializeField]
    private float _speedThrusters = 1.5f;
    [SerializeField]
    private float _speedMultiplier = 2.0f;
    [SerializeField]
    private Thruster _thruster;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _tripleShotPrefab;
    [SerializeField]
    private GameObject _missilesPrefab;
    [SerializeField]
    private float _fireRate = 0.25f;
    private float _canFire = -1.0f;
    [SerializeField]
    private int _lives = 3;
    private SpawnManager _spawnManager;
    [SerializeField]
    private bool _isTripleShotActive = false;
    [SerializeField]
    private bool _isMissilesActive = false;
    [SerializeField]
    private bool _isSpeedUpActive = false;
    [SerializeField]
    private bool _isShieldActive = false;
    [SerializeField]
    private Shield _shieldVisualizer;
    [SerializeField]
    private int _score;
    private UIManager _uiManager;
    [SerializeField]
    private GameObject _leftEngine, _rightEngine;
    [SerializeField]
    private AudioClip _laserSoundClip;
    [SerializeField]
    private AudioClip _errorSoundClip;
    [SerializeField]
    private AudioClip _explosionSoundClip;
    private AudioSource _audioSource;
    private GameManager _gameManager;
    private CameraShake _cameraShake;
    [SerializeField]
    private bool _isPlayerOne = true;
    private PlayerAnimation _playerAnimation;
    private int _actualAmmo;
    [SerializeField]
    private float _shakeDuration = 0.3f, _shakeMagnitud = 0.1f;
    private float _thrusterPercentage = 100.0f;
    [SerializeField]
    private float _thrusterLenght = 5.0f;
    [SerializeField]
    private float _thrusterRecoveryTime = 10.0f;
    private float _thrusterIncreaser = 0.0f, _thrusterDecreaser = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (_gameManager == null)
            Debug.LogError("The Game Manager is NULL");

        if(!_gameManager.IsCoopMode())
            transform.position = new Vector3(0, 0, 0);

        _cameraShake = GameObject.Find("CameraContainer/Main Camera").GetComponent<CameraShake>();
        if (_cameraShake == null)
            Debug.LogError("Camera Shake is null");

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

        Reload();
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
        if(_actualAmmo > 0)
        {
            --_actualAmmo;
            _canFire = Time.time + _fireRate;

            if (_isTripleShotActive)
                Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
            else if (_isMissilesActive)
                Instantiate(_missilesPrefab, transform.position, Quaternion.identity);
            else
                Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.0f, 0), Quaternion.identity);
            
            _uiManager.ChangeAmmo(_actualAmmo);

            _audioSource.clip = _laserSoundClip;
        }
        else
        {
            _audioSource.clip = _errorSoundClip;
        }
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
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if(_thrusterPercentage > 0.0f)
            {
                _thrusterDecreaser = (Time.deltaTime * 100) / _thrusterLenght;
                _thrusterPercentage -= _thrusterDecreaser;
                if (_thrusterPercentage < 0.0f)
                    _thrusterPercentage = 0.0f;
                actualSpeed += _speedThrusters;
                if (!_thruster.IsBossted())
                    _thruster.Boost();
                _uiManager.UpdateThruster(_thrusterPercentage);
            }
        }
        else
        {
            if(_thrusterPercentage < 100.0f)
            {
                _thrusterIncreaser = (Time.deltaTime * 100) / _thrusterRecoveryTime;
                _thrusterPercentage += _thrusterIncreaser;
                if (_thrusterPercentage > 100.0f)
                    _thrusterPercentage = 100.0f;
                _uiManager.UpdateThruster(_thrusterPercentage);
            }
            if (_thruster.IsBossted())
                _thruster.Normal();
        }
        
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
        if (Input.GetKey(KeyCode.RightShift))
        {
            actualSpeed += _speedThrusters;
            if (!_thruster.IsBossted())
                _thruster.Boost();
        }
        else if (_thruster.IsBossted())
            _thruster.Normal();
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
        StartCoroutine(_cameraShake.ShakeStart(_shakeDuration, _shakeMagnitud));

        if (_isShieldActive)
        {
            _isShieldActive = _shieldVisualizer.ReceiveDamage();
            _uiManager.UpdateShield(_shieldVisualizer.DamageResistance());
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

    public void RecoverHealth()
    {
        ++_lives;
        if (_lives > 3)
            _lives = 3;

        if (_lives == 3)
            _leftEngine.SetActive(false);
        else if (_lives == 2)
            _rightEngine.SetActive(false);

        _uiManager.UpdateLives(_lives);
    }

    public void Reload()
    {
        _actualAmmo = _maxAmmo;
        _uiManager.ChangeAmmo(_actualAmmo);
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

    public void MissilesActive()
    {
        _isMissilesActive = true;
        StartCoroutine(MissilesPowerDownRoutine());
    }

    IEnumerator MissilesPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isMissilesActive = false;
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
        _shieldVisualizer.TurnOn();
        _uiManager.UpdateShield(_shieldVisualizer.DamageResistance());
    }

    public void AddScore(int points)
    {
        _score += points;
        _uiManager.ChangeScore(_score);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public enum WaveDirections {DOWN, RIGHT, LEFT, DOWN_RIGHT, DOWN_LEFT, RIGHT_LEFT, DOWN_RIGHT_LEFT};
    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private GameObject[] _powerUps;
    [SerializeField]
    private GameObject _powerUpContainer;
    [SerializeField]
    private GameObject[] _collectables;
    [SerializeField]
    private GameObject _collectablesContainer;
    private bool _stopSpawning = false;
    [SerializeField]
    private int _rarePowerUpIndex;
    private bool _rarePowerUpAppearsRandom = false;
    [SerializeField]
    private int[] _enemiesPerWave;
    [SerializeField]
    private WaveDirections[] _waveDirections;
    private int _actualWave = 0;
    private int _actualEnemies = 0;
    private int _enemiesCount = 0;
    private UIManager _uiManager;

    void Start()
    {
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        if (_uiManager == null)
            Debug.LogError("The UI Manager is NULL");
        else
        {
            _uiManager.WaveInfoChange(_actualWave, _enemiesPerWave.Length);
            _uiManager.RemainingEnemiesChange(_enemiesCount);
        }
    }

    public void StartSpawning()
    {
        _stopSpawning = false;
        _enemiesCount = _enemiesPerWave[_actualWave];
        _actualEnemies = _enemiesCount;
        ++_actualWave;
        _uiManager.WaveInfoChange(_actualWave, _enemiesPerWave.Length);
        _uiManager.RemainingEnemiesChange(_enemiesCount);
        StartCoroutine(SpawnRoutine());
        StartCoroutine(SpawnPowerUpRoutine());
        StartCoroutine(SpawnCollectablesRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (!_stopSpawning)
        {
            yield return new WaitForSeconds(Random.Range(3.0f, 4.0f));
            WaveDirections dir = EnemyDirection();
            GameObject newEnemy;
            Enemy newEnemyScript;
            switch (dir)
            {
                case WaveDirections.DOWN:
                    newEnemy = Instantiate(_enemyPrefab, new Vector3(Random.Range(-10.0f, 10.0f), 7, 0), Quaternion.identity);
                    newEnemyScript = newEnemy.GetComponent<Enemy>();
                    if (newEnemyScript != null)
                        newEnemyScript.SetMovDirection(Enemy.MoveDirection.DOWN);
                    break;
                case WaveDirections.RIGHT:
                    newEnemy = Instantiate(_enemyPrefab, new Vector3(-13.0f, Random.Range(0.0f, 6.0f), 0), Quaternion.identity);
                    newEnemyScript = newEnemy.GetComponent<Enemy>();
                    if (newEnemyScript != null)
                        newEnemyScript.SetMovDirection(Enemy.MoveDirection.RIGHT);
                    break;
                case WaveDirections.LEFT:
                    newEnemy = Instantiate(_enemyPrefab, new Vector3(13.0f, Random.Range(0.0f, 6.0f), 0), Quaternion.identity);
                    newEnemyScript = newEnemy.GetComponent<Enemy>();
                    if (newEnemyScript != null)
                        newEnemyScript.SetMovDirection(Enemy.MoveDirection.LEFT);
                    break;
                default:
                    newEnemy = Instantiate(_enemyPrefab, new Vector3(Random.Range(-10.0f, 10.0f), 7, 0), Quaternion.identity);
                    newEnemyScript = newEnemy.GetComponent<Enemy>();
                    if (newEnemyScript != null)
                        newEnemyScript.SetMovDirection(Enemy.MoveDirection.DOWN);
                    break;
            }
            newEnemy.transform.parent = _enemyContainer.transform;
            --_enemiesCount;
            if (_enemiesCount <= 0)
                _stopSpawning = true;
        }
    }

    IEnumerator SpawnPowerUpRoutine()
    {
        while (!_stopSpawning)
        {
            yield return new WaitForSeconds(Random.Range(3.0f, 7.0f));
            int randomPowerUp = Random.Range(0, _powerUps.Length);
            GameObject newPowerUp = Instantiate(_powerUps[randomPowerUp], new Vector3(Random.Range(-10.0f, 10.0f), 7, 0), Quaternion.identity);
            newPowerUp.transform.parent = _powerUpContainer.transform;
        }
    }

    IEnumerator SpawnCollectablesRoutine()
    {
        while (!_stopSpawning)
        {
            yield return new WaitForSeconds(Random.Range(7.0f, 9.0f));
            int randomCollectable = Random.Range(0, _collectables.Length);
            if (randomCollectable == _rarePowerUpIndex)
            {
                if(!_rarePowerUpAppearsRandom)
                    randomCollectable = Random.Range(0, _collectables.Length);
                _rarePowerUpAppearsRandom = !_rarePowerUpAppearsRandom;
            }
            GameObject newCollectable = Instantiate(_collectables[randomCollectable], new Vector3(Random.Range(-10.0f, 10.0f), 7, 0), Quaternion.identity);
            newCollectable.transform.parent = _powerUpContainer.transform;
        }
    }

    IEnumerator NextWave()
    {
        yield return new WaitForSeconds(Random.Range(6.0f, 8.0f));
        StartSpawning();
    }

    public WaveDirections EnemyDirection()
    {
        if (_waveDirections[_actualWave - 1] == WaveDirections.DOWN_RIGHT_LEFT)
            return (WaveDirections) Random.Range(0, 3);
        else if (_waveDirections[_actualWave - 1] == WaveDirections.RIGHT_LEFT)
            return (WaveDirections)Random.Range(1, 3);
        else if (_waveDirections[_actualWave - 1] == WaveDirections.DOWN_LEFT)
        {
            int dir = Random.Range(0, 2);
            if (dir == 1)
                dir = 2;
            return (WaveDirections) dir;
        }
        else if (_waveDirections[_actualWave - 1] == WaveDirections.DOWN_RIGHT)
            return (WaveDirections)Random.Range(0, 2);
        else
            return _waveDirections[_actualWave - 1];
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }

    public int GetActualWave()
    {
        return _actualWave;
    }

    public int GetActualEnemies()
    {
        return _actualEnemies;
    }

    public void KillEnemy()
    {
        --_actualEnemies;
        if (_actualEnemies <= 0)
        {
            _actualEnemies = 0;
            if (_stopSpawning)
                StartCoroutine(NextWave());
        }
        _uiManager.RemainingEnemiesChange(_actualEnemies);
    }
}
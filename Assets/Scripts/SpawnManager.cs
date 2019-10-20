using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
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

    public void StartSpawning()
    {
        StartCoroutine(SpawnRoutine());
        StartCoroutine(SpawnPowerUpRoutine());
        StartCoroutine(SpawnCollectablesRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (!_stopSpawning)
        {
            yield return new WaitForSeconds(Random.Range(3.0f, 4.0f));
            GameObject newEnemy = Instantiate(_enemyPrefab, new Vector3(Random.Range(-10.0f, 10.0f), 7, 0), Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
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

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }
}
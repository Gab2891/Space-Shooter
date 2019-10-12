using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private bool _isGameOver = false;
    [SerializeField]
    private bool _isCoopMode = false;
    [SerializeField]
    private GameObject _pausePanel;
    private Animator _animator;

    public void Start()
    {
        _animator = _pausePanel.GetComponent<Animator>();
        if (_animator == null)
            Debug.LogError("Animator is null in Game Manager");
        else
            _animator.updateMode = AnimatorUpdateMode.UnscaledTime;
    }

    public void GameOver()
    {
        _isGameOver = true;
    }

    // Update is called once per frame
    private void Update()
    {
        if (_isGameOver && Input.GetKeyDown(KeyCode.R))
            if(!_isCoopMode)
                SceneManager.LoadScene(1); //Game Scene
            else
                SceneManager.LoadScene(2); //Coop Game Scene

        if (_isGameOver && Input.GetKeyDown(KeyCode.Backspace))
            SceneManager.LoadScene(0); //Main Menu scene

        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

        if (Input.GetKeyDown(KeyCode.P))
            Pause();
    }

    public bool IsCoopMode()
    {
        return _isCoopMode;
    }

    public void Pause()
    {
        _pausePanel.SetActive(true);
        _animator.SetBool("isPaused", true);
        Time.timeScale = 0;
    }

    public void Resume()
    {
        _animator.SetBool("isPaused", false);
        _pausePanel.SetActive(false);
        Time.timeScale = 1;
    }

    public void BackMainMenu()
    {
        SceneManager.LoadScene(0); //Main menu scene 
        Time.timeScale = 1;
    }
}

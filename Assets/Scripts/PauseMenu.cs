using System;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [NonSerialized] public bool canPause = true;
    private bool isPaused = false;

    private CursorLockMode previousLockMode;
    private bool previousCursorVisibility;

    private void Update()
    {
        if (!canPause && !isPaused)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

        if (isPaused)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Resume();
            }
        }
    }

    private void Pause()
    {
        SoundManager.Instance.PlayBeep(0);
        pauseMenu.SetActive(true);
        isPaused = true;

        previousLockMode = Cursor.lockState;
        previousCursorVisibility = Cursor.visible;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0;
    }

    public void Resume()
    {
        SoundManager.Instance.PlayBeep(0);
        pauseMenu.SetActive(false);
        isPaused = false;

        Cursor.lockState = previousLockMode;
        Cursor.visible = previousCursorVisibility;

        Time.timeScale = 1;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUIController : MonoBehaviour
{
    bool pauseMenuActive;
    Transform _pauseMenuCanvas;
    PauseUIHandler _pauseMenu;

    private void Awake()
    {
        _pauseMenuCanvas = transform.Find("Canvas_PauseMenu");
        _pauseMenu = FindObjectOfType<PauseUIHandler>();
        _pauseMenuCanvas.gameObject.SetActive(false);
        pauseMenuActive = false;

        ControllerInput.Instance.Pause.AddListener(TogglePauseMenu);
    }

    private void OnEnable()
    {
        
    }

    void TogglePauseMenu()
    {
        if (pauseMenuActive)
        {
            _pauseMenu.SetInactive();
            _pauseMenuCanvas.gameObject.SetActive(false);
            pauseMenuActive = false;

            Time.timeScale = 1;
        }
        else
        {
            _pauseMenuCanvas.gameObject.SetActive(true);
            _pauseMenu.SetActive();
            pauseMenuActive = true;

            Time.timeScale = 0;
        }
    }
}

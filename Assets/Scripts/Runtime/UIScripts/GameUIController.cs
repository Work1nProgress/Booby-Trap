using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Audio;

public class GameUIController : MonoBehaviour
{
    bool pauseMenuActive;
    Transform _pauseMenuCanvas;
    PauseUIHandler _pauseMenu;

    UnityAction<float> _resumeButtonHandler;

    [SerializeField] AudioMixer _masterAudioMixer;

    private void Awake()
    {
        _pauseMenuCanvas = transform.Find("Canvas_PauseMenu");
        _pauseMenu = FindObjectOfType<PauseUIHandler>();
        _pauseMenuCanvas.gameObject.SetActive(false);
        pauseMenuActive = false;

        _resumeButtonHandler = new UnityAction<float>((float value) => { TogglePauseMenu(); });

        ControllerInput.Instance.Pause.AddListener(TogglePauseMenu);
    }

    private void OnEnable()
    {
        SetupAudioSliders();
        SetupButtons();
    }

    void TogglePauseMenu()
    {
        if (pauseMenuActive)
        {
            _pauseMenu.MenuElement(0).Event().RemoveListener(_resumeButtonHandler);

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

            _pauseMenu.MenuElement(0).Event().AddListener(_resumeButtonHandler);

            Time.timeScale = 0;
        }
    }

    void SetupAudioSliders()
    {
        _pauseMenu.MenuElement(1).Event().AddListener((float value) => { _masterAudioMixer.SetFloat("MasterVolume", (value * 100) - 80); });
        _pauseMenu.MenuElement(2).Event().AddListener((float value) => { _masterAudioMixer.SetFloat("MusicVolume", (value * 100) - 80); });
        _pauseMenu.MenuElement(3).Event().AddListener((float value) => { _masterAudioMixer.SetFloat("EffectsVolume", (value * 100) - 80); });
    }

    void SetupButtons()
    {
        _pauseMenu.MenuElement(3).Event().AddListener((float value) => { Application.Quit(); });
    }
}

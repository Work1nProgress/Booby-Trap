using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class IG_PauseMenuHandler : GenericMenuHandler
{
    [SerializeField] AudioMixer _audioMixer;

    protected override void Start()
    {
        base.Start();
    }

    public override void OnOpen()
    {
        base.OnOpen();
        Time.timeScale = 0;
    }

    public override void OnClose()
    {
        base.OnClose();
        Time.timeScale = 1;
    }

    public void OnResume()
    {
        InGameUIController.CloseMenu("Pause");
    }

    public void ChangeMasterVolume(float volume)
    {
        if (_audioMixer != null)
            _audioMixer.SetFloat("MasterVolume", (volume * 100) - 80);
        else
            Debug.LogWarning("Warning: No AudioMixer to adjust!");
    }

    public void ChangeMusicVolume(float volume)
    {
        if (_audioMixer != null)
            _audioMixer.SetFloat("MusicVolume", (volume * 100) - 80);
        else
            Debug.LogWarning("Warning: No AudioMixer to adjust!");
    }

    public void ChangeEffectsVolume(float volume)
    {
        if (_audioMixer != null)
            _audioMixer.SetFloat("EffectsVolume", (volume * 100) - 80);
        else
            Debug.LogWarning("Warning: No AudioMixer to adjust!");
    }

    public void OnQuit()
    {
        Application.Quit();
    }
}

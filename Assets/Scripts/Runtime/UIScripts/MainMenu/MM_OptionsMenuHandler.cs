using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MM_OptionsMenuHandler : GenericMenuHandler
{
    [SerializeField] AudioMixer _audioMixer;

    public void OpenPrimaryMenu()
    {
        MainMenuUIController.OpenMenu("Primary");
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
}

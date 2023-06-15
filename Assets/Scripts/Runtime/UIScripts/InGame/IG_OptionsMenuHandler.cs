using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class IG_OptionsMenuHandler : GenericMenuHandler
{
    [SerializeField] AudioMixer _audioMixer;

    [SerializeField] Slider _masterSlider, _musicSlider, _effectsSlider;

    public override void OnOpen()
    {
        base.OnOpen();
        Time.timeScale = 0;

        if (_masterSlider != null)
        {
            _audioMixer.SetFloat(
            "MasterVolume",
            Mathf.Log10(_masterSlider.value) * 20);
        }
        else
            Debug.LogWarning("Warning: Master volume slider not set!");

        if (_musicSlider != null)
        {
            _audioMixer.SetFloat(
            "MusicVolume",
            Mathf.Log10(_musicSlider.value) * 20);
        }
        else
            Debug.LogWarning("Warning: Music volume slider not set!");

        if (_effectsSlider != null)
        {
            _audioMixer.SetFloat(
            "EffectsVolume",
            Mathf.Log10(_effectsSlider.value) * 20);
        }
        else
            Debug.LogWarning("Warning: Effects volume slider not set!");
    }

    public override void OnClose()
    {
        base.OnClose();
        Time.timeScale = 1;
    }

    public void OnChangeMasterVolume(float volume)
    {
        _audioMixer.SetFloat(
            "MasterVolume",
            Mathf.Log10(volume) * 20);
    }

    public void OnChangeMusicVolume(float volume)
    {
        _audioMixer.SetFloat(
            "MusicVolume",
            Mathf.Log10(volume) * 20);
    }

    public void OnChangeEffectsVolume(float volume)
    {
        _audioMixer.SetFloat(
            "EffectsVolume",
            Mathf.Log10(volume) * 20);
    }

    public void OnOpenPauseMenu()
    {
        InGameUIController.ToggleMenu("Pause");
    }
}

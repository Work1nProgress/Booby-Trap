using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MM_OptionsMenuHandler : GenericMenuHandler
{
    [SerializeField] AudioMixer _audioMixer;

    [SerializeField] Slider _masterSlider, _musicSlider, _effectsSlider;

    public override void OnOpen()
    {
        base.OnOpen();

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

    public void OpenPrimaryMenu()
    {
        MainMenuUIController.OpenMenu("Primary");
    }

    public void ChangeMasterVolume(float volume)
    {
        if (_audioMixer != null)
            _audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
        else
            Debug.LogWarning("Warning: No AudioMixer to adjust!");
    }

    public void ChangeMusicVolume(float volume)
    {
        if (_audioMixer != null)
            _audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
        else
            Debug.LogWarning("Warning: No AudioMixer to adjust!");
    }

    public void ChangeEffectsVolume(float volume)
    {
        if (_audioMixer != null)
            _audioMixer.SetFloat("EffectsVolume", Mathf.Log10(volume) * 20);
        else
            Debug.LogWarning("Warning: No AudioMixer to adjust!");
    }
}

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

        float masterVolume, musicVolume, effectsVolume;
        _audioMixer.GetFloat("MasterVolume", out masterVolume);
        _audioMixer.GetFloat("MusicVolume", out musicVolume);
        _audioMixer.GetFloat("EffectsVolume", out effectsVolume);

        if (_masterSlider != null)
        {
            masterVolume = (masterVolume + 80) * 0.01f;
            _masterSlider.value = masterVolume;
        }
        else
            Debug.LogWarning("Warning: Master volume slider not set!");

        if (_musicSlider != null)
        {
            musicVolume = (musicVolume + 80) * 0.01f;
            _musicSlider.value = musicVolume;
        }
        else
            Debug.LogWarning("Warning: Music volume slider not set!");

        if (_effectsSlider != null)
        {
            effectsVolume = (effectsVolume + 80) * 0.01f;
            _effectsSlider.value = effectsVolume;
        }
        else
            Debug.LogWarning("Warning: Effects volume slider not set!");
    }

    public override void OnClose()
    {
        base.OnClose();
        Time.timeScale = 1;
    }

    public void OnChangeMasterVolume(float value)
    {
        _audioMixer.SetFloat(
            "MasterVolume",
            value * 100f - 80f);
    }

    public void OnChangeMusicVolume(float value)
    {
        _audioMixer.SetFloat(
            "MusicVolume",
            value * 100f - 80f);
    }

    public void OnChangeEffectsVolume(float value)
    {
        _audioMixer.SetFloat(
            "EffectsVolume",
            value * 100f - 80f);
    }

    public void OnOpenPauseMenu()
    {
        InGameUIController.ToggleMenu("Pause");
    }
}

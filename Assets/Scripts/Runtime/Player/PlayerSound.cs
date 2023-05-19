using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerSound : MonoBehaviour
{
    public AudioClip jumpSound;
    public AudioClip landSound;
    public AudioClip fireSound;
    public AudioClip damagedSound;
    public AudioClip collectAmmoSound;
    public AudioClip outOfAmmoSound;
    public AudioClip enterPotalSound;

    private new AudioSource audio;

    void Start()
    {
        audio = GetComponent<AudioSource>();
    }

    public void PlaySound(AudioClip sound, float volumeScale = 1)
    {
        if (sound != null)
            audio.PlayOneShot(sound, volumeScale);
        else
            Debug.LogWarning("Sound effect has not been assigned to PlayerSound.");
    }

}

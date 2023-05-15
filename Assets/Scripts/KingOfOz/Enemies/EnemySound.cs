using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EnemySound : MonoBehaviour
{
    // sound effects
    public AudioClip fireSound;
    public AudioClip damagedSound;
    public AudioClip dieSound;

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
            Debug.LogWarning("Sound effect has not been assigned to EnemySound.");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicItem : MonoBehaviour
{
    AudioSource m_AudioSource;
    public bool IsPlaying;
    public float Length => m_AudioSource.clip.length;
    public float Time => m_AudioSource.time;
    public float TimeSamples => m_AudioSource.timeSamples;

    public MusicItem Init(AudioSource audioSource)
    {
        m_AudioSource = audioSource;
        return this;
    }


    public void Play()
    {

        m_AudioSource.Play();
    }


    public void Set(AudioClip clip, float volume, float time = 0)
    {
        m_AudioSource.clip = clip;
        m_AudioSource.volume = volume;
        m_AudioSource.time = time;
    }

    public virtual void SetVolume(float volume)
    {

        m_AudioSource.volume = volume;
    }

    public void SetAndPlay(AudioClip clip, float volume, float time = 0)
    {
        Set(clip, volume, time);
        m_AudioSource.Play();
    }

    public void Stop()
    {
        m_AudioSource.Stop();

    }



    public void Despawn()
    {
        IsPlaying = false;
    }

    public void Spawn()
    {
        transform.parent = null;
        IsPlaying = true;
    }
}

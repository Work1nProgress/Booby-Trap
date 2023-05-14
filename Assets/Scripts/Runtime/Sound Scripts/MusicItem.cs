using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicItem : PoolObject
{
    [SerializeField]
    AudioSource m_AudioSource;
    public float Length => m_AudioSource.clip.length;
    public float Time => m_AudioSource.time;
    public int TimeSamples => m_AudioSource.timeSamples;
    public bool IsPlaying => m_AudioSource.isPlaying;

   


    public void Play()
    {

        m_AudioSource.Play();
    }


    public virtual void Set(MusicTrack track, float volume)
    {
        m_AudioSource.clip = track.Music;
        m_AudioSource.volume = volume;
    }

    public virtual void SetVolume(float volume)
    {
        m_AudioSource.volume = volume;
    }

    public void SetTime(float time)
    {
        m_AudioSource.time = time;
        
    }

    public void Stop()
    {
        m_AudioSource.Stop();

    }
}

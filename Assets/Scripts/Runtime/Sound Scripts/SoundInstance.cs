using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundInstance : PoolObject
{
    [SerializeField]
    AudioSource m_AudioSource;

    Transform m_Target;
    bool followTarget;

    public string SoundName => m_AudioSource.clip.name;
    public float Length{

        get
        {
            if (m_AudioSource.pitch == 0)
            {
                return m_AudioSource.clip.length;
            }
            else
            {
                return m_AudioSource.clip.length / Mathf.Abs(m_AudioSource.pitch);
            }
        }
    }


    public void Set(SoundItem item, Transform target){


        m_AudioSource.clip = item.AudioClip;
        m_AudioSource.pitch = item.PitchShift;
        m_AudioSource.pitch += Random.value * item.RandomPitch;
        m_AudioSource.volume = 0.5f + item.Volume;
        followTarget = target != null;
        m_Target = target;

    }
    private void Update()
    {
        if (!followTarget)
        {
            return;
        }
        if (m_AudioSource.isPlaying)
        {
            transform.position = m_Target.position;
        }
    }

    public float Play()
    {
        m_AudioSource.Play();
        return Length;
    }
}

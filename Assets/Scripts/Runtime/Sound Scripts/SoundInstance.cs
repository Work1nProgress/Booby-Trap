using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundInstance : PoolObject
{
    [SerializeField]
    AudioSource m_AudioSource;

    Transform m_Target;
    bool followTarget;


    string m_SoundName;
    public string SoundName => m_SoundName;
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


    public void Set(SoundItem item, Sound sound, Transform target, float minDistance, float maxDistance, bool spatialBlend)
    {
        m_SoundName = sound.SoundName;

        m_AudioSource.clip = item.AudioClip;
        m_AudioSource.pitch = 1+sound.PitchShift;
        m_AudioSource.pitch += Random.value * sound.RandomPitch;

        
     

        if (sound.UseCustomMinMax)
        {

            m_AudioSource.minDistance = sound.MinDistance;
            m_AudioSource.maxDistance = Mathf.Max(sound.MaxDistance, m_AudioSource.minDistance + 0.1f);
        }
        else
        {
            m_AudioSource.minDistance = minDistance;
            m_AudioSource.maxDistance = Mathf.Max(maxDistance, m_AudioSource.minDistance + 0.1f);
        }

       

        m_AudioSource.rolloffMode = AudioRolloffMode.Custom;
       
        m_AudioSource.dopplerLevel = sound.DopplerLevel;





        followTarget = target != null;
        m_Target = target;

       var t = 0f;
        var blend = 1f;
        if (followTarget) {
            t = Vector3.Distance(ControllerGame.Instance.player.transform.position, target.position)/m_AudioSource.maxDistance;
            blend = m_AudioSource.GetCustomCurve(AudioSourceCurveType.CustomRolloff).Evaluate(t);
           }

        m_AudioSource.volume = (0.5f + sound.Volume);


        if (sound.OverrideCustomSpatialBlend)
        {
            m_AudioSource.volume *= sound.UseCustomSpatialBlend ? blend : 1f;
        }
        else
        {
            m_AudioSource.volume *= spatialBlend ? blend : 1f;
        }
        
       



    }
    private void Update()
    {
        if (!followTarget || m_Target == null)
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

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


    public void Set(SoundItem item, Sound sound, Transform target, Dictionary<AudioSourceCurveType, AnimationCurve> curves, float minDistance, float maxDistance)
    {
        m_SoundName = sound.SoundName;

        m_AudioSource.clip = item.AudioClip;
        m_AudioSource.pitch = 1+sound.PitchShift;
        m_AudioSource.pitch += Random.value * sound.RandomPitch;
        m_AudioSource.volume = 0.5f + sound.Volume;



        if (sound.UseCustomRolloff || sound.UseCustomReverbZoneMix || sound.UseCustomSpatialBlend || sound.UseCustomSpread)
        {
            m_AudioSource.minDistance = sound.MinDistance;
            m_AudioSource.maxDistance = sound.MaxDistance;
        }
        else
        {
            m_AudioSource.minDistance = minDistance;
            m_AudioSource.maxDistance = maxDistance;
        }

        m_AudioSource.dopplerLevel = sound.DopplerLevel;

        //curves
        AnimationCurve tmp = sound.UseCustomRolloff ? sound.CustomRolloff : curves[AudioSourceCurveType.CustomRolloff];
        m_AudioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, tmp);

        tmp = sound.UseCustomReverbZoneMix ? sound.ReverbZoneMix : curves[AudioSourceCurveType.ReverbZoneMix];
        m_AudioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, tmp);

        tmp = sound.UseCustomSpread ? sound.Spread : curves[AudioSourceCurveType.Spread];
        m_AudioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, tmp);

        tmp = sound.UseCustomSpatialBlend ? sound.SpatialBlend : curves[AudioSourceCurveType.SpatialBlend];
        m_AudioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, tmp);





        followTarget = target != null;
        m_Target = target;


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

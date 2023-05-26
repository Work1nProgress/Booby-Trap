using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{

    public string SoundName;

    public PlaySubitemType PlaySubitemType;
    public SoundItem[] soundItems = new SoundItem[0];


    [Range(-0.5f, 0.5f)]
    public float Volume = 0;
    public float PitchShift;
    public float RandomPitch;
    public float DopplerLevel;
    public float MinimumTimeBetweenSameSound = 0.1f;



    public int notSameTwiceOffset = 1;

    public float ChanceToPlay = 100f;

    public float MinDistance;
    public float MaxDistance;


    public bool UseCustomMinMax;
    public bool OverrideCustomSpatialBlend;
    public bool UseCustomSpatialBlend;



    public SoundItem SoundItem(List<SoundItem> prev)
    {


        if (prev != null && prev.Count > 0)
        {
            switch (PlaySubitemType)
            {
                case PlaySubitemType.RandomNotSameTwice:
                    var si = System.Array.FindAll(soundItems, x => !prev.Contains(x));
                    return si[Random.Range(0, si.Length)];

            }
        }
            return soundItems[Random.Range(0, soundItems.Length)];




    }

    public void AddSoundItem(AudioClip clip)
    {
        if (soundItems == null)
        {
            soundItems = new SoundItem[0];
        }

        Utils.AddArrayElement(ref soundItems, new SoundItem(clip));
    }
}

    [System.Serializable]
    public class SoundItem{
    public AudioClip AudioClip;

    //[Range(-0.5f, 0.5f)]
    //public float Volume = 0.5f;
    //public float PitchShift;
    //public float RandomPitch;
    //public float DopplerLevel;
    //public bool UseCustomSpatialBlend;


    //public float MinDistance;
    //public float MaxDistance;

    //public bool UseCustomSpread;
    //public bool UseCustomRolloff;
    //public bool UseCustomReverbZoneMix;

    //public AnimationCurve SpatialBlend;
    //public AnimationCurve Spread;
    //public AnimationCurve CustomRolloff;
    //public AnimationCurve ReverbZoneMix;


    public SoundItem(AudioClip audioClip)
    {
        AudioClip = audioClip;
    }

}

public enum PlaySubitemType
{
    Random,
    RandomNotSameTwice
}

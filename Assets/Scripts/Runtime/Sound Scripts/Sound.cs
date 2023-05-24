using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{

    public string SoundName;

    public PlaySubitemType PlaySubitemType;
    public  SoundItem[] soundItems;


    [Range(-0.5f, 0.5f)]
    public float Volume = 0;
    public float PitchShift;
    public float RandomPitch;
    public float DopplerLevel;
    public float MinimumTimeBetweenSameSound = 0.1f;
    public bool UseCustomSpatialBlend;


    public float MinDistance;
    public float MaxDistance;

    public bool UseCustomSpread;
    public bool UseCustomRolloff;
    public bool UseCustomReverbZoneMix;

    public AnimationCurve SpatialBlend = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 0));
    public AnimationCurve Spread = new AnimationCurve(new Keyframe(0, 1), new Keyframe(360, 0));
    public AnimationCurve CustomRolloff = new AnimationCurve(new Keyframe(0, 1), new Keyframe(0.2f, 0.8f), new Keyframe(0.4f, 0.5f), new Keyframe(0.7f, 0.2f), new Keyframe(1, 0f));
    public AnimationCurve ReverbZoneMix = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 0));

    public SoundItem SoundItem(SoundItem prev)
    {


        if (prev != null)
        {
            switch (PlaySubitemType)
            {
                case PlaySubitemType.RandomNotSameTwice:
                    var si = System.Array.FindAll(soundItems, x => x != prev);
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

        Debug.Log(soundItems.Length);
        Utils.AddArrayElement(ref soundItems, new SoundItem(clip));
        Debug.Log(soundItems.Length);
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

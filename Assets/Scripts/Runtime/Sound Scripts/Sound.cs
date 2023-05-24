using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
    public string SoundName;

    public PlaySubitemType PlaySubitemType;
    [SerializeField]
    private SoundItem[] soundItems;

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
}

    [System.Serializable]
    public class SoundItem{
    public AudioClip AudioClip;

    [Range(-0.5f, 0.5f)]
    public float Volume = 0.5f;
    public float PitchShift;
    public float RandomPitch;
    public bool UseCustomSpatialBlend;
    public float DopplerLevel;




}

public enum PlaySubitemType
{
    Random,
    RandomNotSameTwice
}

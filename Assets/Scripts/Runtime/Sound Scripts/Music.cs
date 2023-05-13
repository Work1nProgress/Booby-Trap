using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

[System.Serializable]
public class MusicTrack
{
    [Range(0, 1f)]
    public float Volume = 1f;

    [SerializeField]
    AudioClip m_AudioClip;

    public AudioClip Music => m_AudioClip;

}

[System.Serializable]
public class LayeredTrack
{
    public string Name;
    [Range(0, 1f)]
    public float Volume = 1f;

    [SerializeField]
    MusicTrack[] Layers;

    public MusicTrack[] Tracks => Layers;

    public int LayerCount =>Layers.Length;

    public AudioClip[] GetLayers(List<int> layers)
    {
        var musicLayers = new AudioClip[layers.Count];
        for (int i = 0; i < layers.Count; i++)
        {
            musicLayers[i] = Layers[i].Music;
        }
        return musicLayers;
    }

    [System.NonSerialized]
    float m_Length;
    public float Length
    {
        get
        {
            if (m_Length > 0)
            {
                return m_Length;
            }
            else
            {
                for (int i = 0; i < Layers.Length; i++)
                {
                    if (Layers[i].Music.length > m_Length)
                    {
                        m_Length = Layers[i].Music.length;
                    }
                }
                return m_Length;
            }

        }

    }

}



[System.Serializable]
public class MusicPlaylist
{
    public string Name;
    public bool Crossfade;
    public float CrossfadeTime;
    public bool Randomize;
    public string[] MusicTracks;

    public int GetTrack(int currentIndex = -1)
    {

        if (Randomize)
        {
            int remove = 1;
            if (currentIndex == -1)
            {
                remove = 0;
            }
            int[] candidates = new int[MusicTracks.Length - remove];
            for (int i = 0; i < candidates.Length; i++)
            {
                if (i < currentIndex)
                {
                    candidates[i] = i;
                }
                else if (i > currentIndex)
                {
                    candidates[i - remove] = i;
                }
            }

            var trackIndex = Random.Range(0, candidates.Length);
            return trackIndex;

        }
        else
        {
            currentIndex++;
            if (currentIndex > MusicTracks.Length - 1)
            {
                currentIndex = 0;
            }
            return currentIndex;

        }
    }


}




using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class MusicTrack
{
    public string Name;
    [Range(0, 1f)]
    public float Volume = 1f;

    [SerializeField]
    AudioClip m_AudioClip;

    public AudioClip Music => m_AudioClip;

}

[System.Serializable]
public class LayeredMusic
{
    public string Name;
    [Range(0, 1f)]
    public float Volume = 1f;

    [SerializeField]
    MusicTrack[] Layers;

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
}



[System.Serializable]
public class MusicPlaylist
{
    public string Name;
    public bool Crossfade;
    public float CrossfadeTime;
    public bool Randomize;
    public string[] MusicTracks;
}




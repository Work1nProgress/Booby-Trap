using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

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

    public (string name, int trackIndex) GetTrack(int currentIndex = -1)
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
            return (MusicTracks[candidates[trackIndex]], trackIndex);

        }
        else
        {
            currentIndex++;
            if (currentIndex > MusicTracks.Length - 1)
            {
                currentIndex = 0;
            }
            return (MusicTracks[currentIndex], currentIndex);

        }
    }


}




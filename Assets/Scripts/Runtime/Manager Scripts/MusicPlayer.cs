using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;
using Random = UnityEngine.Random;

public class MusicPlayer : GenericSingleton<MusicPlayer>
{

    [SerializeField]
    AudioMixerGroup Mixer;

    [SerializeField]
    MusicPlaylist[] Playlists;

    [SerializeField]
    LayeredMusic[] LayeredTracks;

    [SerializeField]
    MusicTrack[] Tracks;


    int m_ClipIndex;


    MusicPlaylist m_CurrentPlaylist;
    MusicTrack m_CurrentTrack;





    MusicItem[] m_ClipPool;
    Dictionary<string, MusicItem> m_Playing;




    void Start()
    {
        int maxLayers = 1;
        foreach (var layered in LayeredTracks)
        {
            if (layered.LayerCount > maxLayers)
            {
                maxLayers = layered.LayerCount;
            }
        }
        m_ClipPool = new MusicItem[maxLayers];
        for (int i = 0; i < maxLayers; i++)
        {
            var audioSource = new GameObject().AddComponent<AudioSource>();
            audioSource.spatialBlend = 0;
            m_ClipPool[i] = audioSource.gameObject.AddComponent<MusicItem>().Init(audioSource);
            m_ClipPool[i].transform.SetParent(transform,false);
            m_ClipPool[i].gameObject.SetActive(false);
        }
    }

    

    public void PlayMusic(string musicName)
    {
    }

    public void PlayPlaylist(string playlist)
    {
        if (m_CurrentPlaylist.Name == playlist)
        {

            Debug.Log($"Playlist {playlist} already playing");
            return;
        }
        if (m_CurrentTrack != null)
        {
            m_Playing[m_CurrentTrack.Name].Stop();

        }

        Array.Find(Playlists, x => x.Name == playlist);
    }

    private int GetNextIndex()
    {
        m_ClipIndex++;
        if (m_ClipIndex >= m_CurrentPlaylist.MusicTracks.Length)
            m_ClipIndex = 0;

        return m_ClipIndex;
    }

    private int GetRandomIndex()
    {
        return Random.Range(0, m_CurrentPlaylist.MusicTracks.Length);
    }

    private void ChangeMusic(int index)
    {
        index = Mathf.Clamp(index, 0, m_CurrentPlaylist.MusicTracks.Length - 1);

        //_source.Stop();
        //_source.clip = _musicClips[index];
        //_source.Play();
        //Debug.Log("Play new music");
    }
}

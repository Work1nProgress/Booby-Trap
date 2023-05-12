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
    Queue<MusicItem> PoolQueue;
    Dictionary<string, MusicItem> m_Playing;

    IEnumerator CurrentTracker, NextTracker;




    protected override void Awake()
    {
        base.Awake();

        int maxLayers = 1;
        foreach (var layered in LayeredTracks)
        {
            if (layered.LayerCount > maxLayers)
            {
                maxLayers = layered.LayerCount;
            }
        }

        foreach (var playlist in Playlists)
        {
            if (playlist.Crossfade)
            {
                maxLayers = Mathf.Max(maxLayers, 2);
                break;
            }
        }


        m_ClipPool = new MusicItem[maxLayers];
        PoolQueue = new Queue<MusicItem>();
        m_Playing = new Dictionary<string, MusicItem>();

        for (int i = 0; i < maxLayers; i++)
        {
            var audioSource = new GameObject().AddComponent<AudioSource>();
            audioSource.spatialBlend = 0;
            audioSource.gameObject.name = $"MusicItem_{i}";
            audioSource.outputAudioMixerGroup = Mixer;
            m_ClipPool[i] = audioSource.gameObject.AddComponent<MusicItem>().Init(audioSource);
            m_ClipPool[i].transform.SetParent(transform,false);
            m_ClipPool[i].gameObject.SetActive(false);
            PoolQueue.Enqueue(m_ClipPool[i]);
        }

      
    }



    public void PlayMusic(string musicName)
    {

        if (m_CurrentPlaylist?.Name == musicName)
        {

            Debug.Log($"Music {musicName} already playing");
            return;
        }

        if (m_CurrentTrack != null)
        {
            m_Playing[m_CurrentTrack.Name].Stop();
            var item = m_Playing[m_CurrentTrack.Name];
            m_Playing.Remove(m_CurrentTrack.Name);
            item.Stop();
            item.Despawn();
            item.gameObject.name = $"MusicItem_{Array.FindIndex(m_ClipPool, x => x == item)}";
            PoolQueue.Enqueue(item);
            item.transform.SetParent(transform);
        }

        var itemNew = PoolQueue.Dequeue();
        itemNew.Spawn();
        itemNew.gameObject.SetActive(true);
      
        var track = GetTrack(musicName);

        if (track == null)
        {
            Debug.Log($"Track {musicName} not found.");
            return;
        }
        m_CurrentTrack = track;
        m_Playing.Add(m_CurrentTrack.Name, itemNew);
        itemNew.gameObject.name = $"Music_{musicName}";
        itemNew.SetAndPlay(track.Music, track.Volume);
        StartCoroutine(CurrentTracker = Tracker(track.Music.length));
    }

    public void PlayPlaylist(string playlist)
    {
        if (m_CurrentPlaylist?.Name == playlist)
        {

            Debug.Log($"Playlist {playlist} already playing");
            return;
        }
        Array.Find(Playlists, x => x.Name == playlist);
        var nextPlaylist = Array.Find(Playlists, x => x.Name == playlist);
        m_CurrentPlaylist = nextPlaylist;
        m_ClipIndex = -1;
        NextTrack();

    }

    private MusicTrack GetTrack(string name) {
        return Array.Find(Tracks, x => x.Name == name);
        
    }

    public void NextTrack()
    {
        var trackInfo = m_CurrentPlaylist.GetTrack(m_ClipIndex);
        m_CurrentPlaylist.GetTrack(m_ClipIndex);
        m_ClipIndex = trackInfo.trackIndex;
        PlayMusic(trackInfo.name);
    }



    IEnumerator Tracker(float length)
    {
        yield return new WaitForSeconds(length);
        NextTrack();
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

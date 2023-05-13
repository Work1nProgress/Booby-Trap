using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;
using Random = UnityEngine.Random;

public class MusicPlayer : GenericSingleton<MusicPlayer>
{

    [SerializeField]
    public AudioMixerGroup Mixer;

    [SerializeField]
    MusicPlaylist[] Playlists;


    [SerializeField]
    LayeredTrack[] Tracks;


    int m_ClipIndex;


    MusicPlaylist m_CurrentPlaylist;
    LayeredTrack m_CurrentTrack, m_NextTrack;





    MusicItemLayered[] m_ClipPool;
    Queue<MusicItemLayered> PoolQueue;
    Dictionary<string, MusicItemLayered> m_Playing;

    IEnumerator CurrentTracker;

    [SerializeField]
    float DeltaTime;



    protected override void Awake()
    {
        base.Awake();

        int maxLayers = 1;
        foreach (var layered in Tracks)
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
                maxLayers *= 2;
                break;
            }
        }


        m_ClipPool = new MusicItemLayered[maxLayers];
        PoolQueue = new Queue<MusicItemLayered>();
        m_Playing = new Dictionary<string, MusicItemLayered>();

        for (int i = 0; i < maxLayers; i++)
        {
            m_ClipPool[i] = new GameObject().AddComponent<MusicItemLayered>().Init();
            m_ClipPool[i].transform.SetParent(transform,false);
            m_ClipPool[i].gameObject.SetActive(false);
            PoolQueue.Enqueue(m_ClipPool[i]);
        }

      
    }



    public void PlayMusic(string musicName)
    {

        if (m_CurrentPlaylist?.Name == musicName)
        {

            Debug.LogWarning($"Music {musicName} already playing");
            return;
        }

        if (m_CurrentTrack != null)
        {
            RemoveCurrentTrack();
        }
        var itemNew = CreateNewItem(musicName);
        m_CurrentTrack = itemNew.track;
        itemNew.item.Play(1);

        RemoveCoroutine(CurrentTracker);
        StartCoroutine(CurrentTracker = Tracker(m_CurrentTrack.Length, m_CurrentPlaylist.CrossfadeTime));
    }

    void PlayNextCrossfaded()
    {

        var trackInfo = m_CurrentPlaylist.GetTrack(m_ClipIndex);
        m_ClipIndex = trackInfo.trackIndex;
        var itemNew = CreateNewItem(trackInfo.name);
        m_NextTrack = itemNew.track;
        itemNew.item.SetVolumePercent(0);
        itemNew.item.Play();
       
    }


    (MusicItemLayered item, LayeredTrack track) CreateNewItem(string musicName)
    {
        var itemNew = PoolQueue.Dequeue();
        itemNew.gameObject.SetActive(true);

        var track = GetTrack(musicName);

        if (track == null)
        {
            Debug.LogWarning($"Track {musicName} not found.");
            return default;
        }

        m_Playing.Add(musicName, itemNew);
        itemNew.gameObject.name = $"Music_{musicName}";
        itemNew.Set(track);
        return (itemNew, track);
    }

    

    public void PlayPlaylist(string playlist)
    {
        if (m_CurrentPlaylist?.Name == playlist)
        {
            Debug.LogWarning($"Playlist {playlist} already playing");
            return;
        }
        Array.Find(Playlists, x => x.Name == playlist);
        var nextPlaylist = Array.Find(Playlists, x => x.Name == playlist);
        m_CurrentPlaylist = nextPlaylist;
        m_ClipIndex = -1;
        NextTrack();

    }

    private LayeredTrack GetTrack(string name) {
        return Array.Find(Tracks, x => x.Name == name);      
    }


    void RemoveCurrentTrack()
    {

        m_Playing[m_CurrentTrack.Name].Stop();
        var item = m_Playing[m_CurrentTrack.Name];
        m_Playing.Remove(m_CurrentTrack.Name);
        item.StopAndShutDown();
        item.gameObject.name = $"MusicItem_{Array.FindIndex(m_ClipPool, x => x == item)}";
        PoolQueue.Enqueue(item);
        item.transform.SetParent(transform);
    }



    public void NextTrack()
    {
        var trackInfo = m_CurrentPlaylist.GetTrack(m_ClipIndex);
        m_ClipIndex = trackInfo.trackIndex;
        PlayMusic(trackInfo.name);
    }



    IEnumerator Tracker(float length, float fadeTime)
    {

        if (fadeTime > 0)
        {

            yield return new WaitForSecondsRealtime(length - fadeTime / 2f);
      

            bool isCrossfading = true;

            float timer = 0;

          
            PlayNextCrossfaded();
         
            while (isCrossfading)
            {
                

                if (timer <= fadeTime)
                {
                    float t = timer / fadeTime;
                    m_Playing[m_CurrentTrack.Name].SetVolumePercent(1-t);
                    m_Playing[m_NextTrack.Name].SetVolumePercent(t);

                    yield return new WaitForSeconds(DeltaTime);
                }
                else
                {
                    m_Playing[m_CurrentTrack.Name].SetVolumePercent(0);
                    m_Playing[m_NextTrack.Name].SetVolumePercent(1);
                    RemoveCurrentTrack();
                    m_CurrentTrack = m_NextTrack;
                    isCrossfading = false;
                    RemoveCoroutine(CurrentTracker);
                    StartCoroutine(CurrentTracker = Tracker(m_CurrentTrack.Length - timer, m_CurrentPlaylist.CrossfadeTime));
                }
                timer += DeltaTime;
            }
        }
        else
        {
            yield return new WaitForSeconds(length);
            NextTrack();
        }
    }

  

    void RemoveCoroutine(IEnumerator enumerator)
    {
        if (enumerator != null)
        {
            StopCoroutine(enumerator);
            enumerator = null;
        }
    }



    

   


}

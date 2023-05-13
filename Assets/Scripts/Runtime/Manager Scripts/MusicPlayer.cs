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
    (LayeredTrack track, MusicItemLayered item) m_Current, m_Next;

    Queue<MusicItemLayered> m_ItemQueue = new Queue<MusicItemLayered>();

    IEnumerator CurrentTracker;

    [SerializeField]
    float DeltaTime;



    protected override void Awake()
    {
        base.Awake();

        for (int i = 0; i < 2; i++)
        {
            m_ItemQueue.Enqueue(CreateLayeredMusicItem());
        }

    }

    MusicItemLayered CreateLayeredMusicItem()
    {
       var item = new GameObject().AddComponent<MusicItemLayered>().Init();
       item.transform.SetParent(transform, false);
       item.gameObject.SetActive(false);
       return item;
    }

    (LayeredTrack track, MusicItemLayered item) InitializeNextItem(string musicName)
    {

        Debug.Log(musicName);
        var item = m_ItemQueue.Dequeue();
        item.gameObject.SetActive(true);

        var track = GetTrack(musicName);

        if (track == null)
        {
            Debug.LogWarning($"Track {musicName} not found.");
            return default;
        }

        item.gameObject.name = $"Music_{musicName}";
        item.Set(track);
        return (track, item);
    }



    public void PlayMusic(string musicName, params int[] layers)
    {
       
        int[] defaultLayers = new int[] { 0 };
        if (layers.Length > 0)
        {
            defaultLayers = layers;
        }

        Debug.Log($"play music {musicName}");
        var layersFromName = GetLayersFromName(musicName);
        if (layersFromName != null)
        {
            defaultLayers = layersFromName;
        }

        if (m_CurrentPlaylist?.Name == musicName)
        {

            Debug.LogWarning($"Music {musicName} already playing");
            return;
        }
        float crossfade = 0;
        if (m_CurrentPlaylist != null)
        {
            crossfade = m_CurrentPlaylist.CrossfadeTime;
        }

        if (m_Current.track != null)
        {
            RemoveTrack(ref m_Current);
        }
        m_Current= InitializeNextItem(musicName);

        m_Current.item.Play(defaultLayers);

        RemoveCoroutine(CurrentTracker);
        Debug.Log($"start tracker for music {musicName}");
        StartCoroutine(CurrentTracker = Tracker(m_Current.track.Length, crossfade));
    }

    public void AddLayers(string playlistName, params int[] layers)
    {
        if (m_CurrentPlaylist != null && playlistName != m_CurrentPlaylist.Name)
        {
            return;
        }
        if (m_Current.item != null)
        {
            m_Current.item.Play(layers);
        }
        if (m_Next.item != null)
        {
            m_Next.item.Play(layers);
        }
    }

    public void RemoveLayers(string playlistName, params int[] layers)
    {
        if (m_CurrentPlaylist != null && playlistName != m_CurrentPlaylist.Name)
        {
            return;
        }
        if (m_CurrentPlaylist != null && playlistName != m_CurrentPlaylist.Name)
        {
            return;
        }
        if (m_Current.item != null)
        {
            m_Current.item.Stop(layers);
        }
        if (m_Next.item != null)
        {
            m_Next.item.Stop(layers);
        }
    }

    public void PlayPlaylist(string playlist, params int[] layers)
    {
        if (m_CurrentPlaylist?.Name == playlist)
        {
            Debug.LogWarning($"Playlist {playlist} already playing");
            return;
        }
        var nextPlaylist = Array.Find(Playlists, x => x.Name == playlist);

        if (nextPlaylist == null)
        {
            Debug.LogWarning($"Playlist {playlist} not found");
            return;
        }

        if(m_CurrentPlaylist != null)
        {
            StopPlaying();
           
        }
        m_CurrentPlaylist = nextPlaylist;
        m_ClipIndex = -1;
        NextTrack(layers);

    }

    void PlayNextCrossfaded()
    {
        m_ClipIndex = m_CurrentPlaylist.GetTrack(m_ClipIndex);
        m_Next = InitializeNextItem(m_CurrentPlaylist.MusicTracks[m_ClipIndex]);
        if (m_Next.item == null)
        {
            return;
        }
        m_Next.item.SetVolumePercent(0);

        var layersFromName = GetLayersFromName(m_CurrentPlaylist.MusicTracks[m_ClipIndex]);
        if (layersFromName != null)
        {
            m_Next.item.Play(layersFromName);
        }
        else
        {
            m_Next.item.Play(m_Current.item.CurrentLayers);
        }

       
       
    }
    const string mainSeparator = ";";
    const string layerSeparator = "_";
    int[] GetLayersFromName(string _name)
    {
        if (_name.Contains(mainSeparator))
        {
            var split = _name.Split(mainSeparator);
           
            var layerString = split[1].Split(layerSeparator);
            int[] layers = new int[layerString.Length];
            for (int i = 0; i < layerString.Length; i++)
            {
                if (int.TryParse(layerString[i], out var layer))
                {
                    layers[i] = layer;
                }
                else
                {
                    Debug.LogWarning($"Music {_name} has invalid layers");
                }

            }
            return layers;
        }
        return null;
    }

    public void NextTrack(params int [] layers)
    {
        m_ClipIndex = m_CurrentPlaylist.GetTrack(m_ClipIndex);
        PlayMusic(m_CurrentPlaylist.MusicTracks[m_ClipIndex], layers);
    }

    void RemoveTrack(ref (LayeredTrack track, MusicItemLayered item) trackData)
    {
        trackData.item.StopAndShutDown();
        trackData.item.gameObject.name = $"MusicItem";
        trackData.item.transform.SetParent(transform);
        m_ItemQueue.Enqueue(trackData.item);
        trackData = default;
    }


    private LayeredTrack GetTrack(string name) {
        return Array.Find(Tracks, x => x.Name == name.Split(mainSeparator)[0]);      
    }

    public void StopPlaying()
    {

        RemoveCoroutine(CurrentTracker);
        if (m_Current.track != null)
        {
            RemoveTrack(ref m_Current);
        }
        if (m_Next.track != null)
        {
            RemoveTrack(ref m_Next);
        }
        m_CurrentPlaylist = default;
       
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
                    m_Current.item.SetVolumePercent(1-t);
                    m_Next.item.SetVolumePercent(t);

                    yield return new WaitForSeconds(DeltaTime);
                }
                else
                {
                    m_Current.item.SetVolumePercent(0);
                    m_Next.item.SetVolumePercent(1);
                    RemoveTrack(ref m_Current);
                    m_Current = m_Next;
                    m_Next = default;
                    isCrossfading = false;
                    RemoveCoroutine(CurrentTracker);
                    StartCoroutine(CurrentTracker = Tracker(m_Current.track.Length - timer, m_CurrentPlaylist.CrossfadeTime));
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

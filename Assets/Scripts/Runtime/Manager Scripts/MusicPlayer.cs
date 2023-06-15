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


    PlayState m_State;


    string m_QueuedTrack;
    int[] m_QueuedLayers;

    int[] m_PlaylistCurrentLayers;

    bool m_LoopSingle;
    float m_CrossfadeTimeSingle;
    float m_CurrentFadePercent;
    float m_PreviousFadePercent = 1;

    float GetPlaylistCrossfade
    {
        get
        {
            if (m_CurrentPlaylist == null)
            {
                return 0f;
            }
            else
            {
                return m_CurrentPlaylist.Crossfade ? m_CurrentPlaylist.CrossfadeTime : 0f;
            }
        }

    }




    #region Initialization
    protected override void Awake()
    {
        base.Awake();

        for (int i = 0; i < 2; i++)
        {
            m_ItemQueue.Enqueue(CreateLayeredMusicItem());
        }
        m_State = PlayState.None;

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

    #endregion



    #region Public Methods


    public void PlayMusic(string musicName, params int[] layers)
    {
        PlayMusic(musicName, 0, false, 0, layers);
    }

    public void PlayMusic(string musicName, float fadeIn = 0, bool loop = false, float crossFade= 0, params int[] layers)
    {
        m_QueuedTrack = musicName;
        m_QueuedLayers = layers;
        m_LoopSingle = loop;
        m_CrossfadeTimeSingle = crossFade;
        NextSong(fadeIn);

    }

    public void PlayPlaylist(string playlist, float fadeIn = 0, params int[] layers)
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
        m_QueuedLayers = layers;
        m_CurrentPlaylist = nextPlaylist;

        m_ClipIndex = m_CurrentPlaylist.GetTrack(-1);
        m_QueuedTrack = m_CurrentPlaylist.MusicTracks[m_ClipIndex];

        NextSong(fadeIn);
    }

    public void StopPlaying(float fadeOut = 0)
    {



        m_CurrentPlaylist = null;
        if (m_State == PlayState.None)
        {
            return;
        }

        RemoveCoroutine(CurrentTracker);
        if (fadeOut > 0)
        {
            StartCoroutine(CurrentTracker = FadeOutCoroutine(fadeOut));
        }
        else
        {

            RemoveTrack(ref m_Current);

            if (m_Next.item != null)
            {
                RemoveTrack(ref m_Next);
            }
            m_State = PlayState.None;
        }

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
        for (int i = 0; i < layers.Length; i++) {
            if (!Array.Exists(m_QueuedLayers,x =>x == layers[i])){
                Utils.AddArrayElement(ref m_QueuedLayers, layers[i]);
            }
        }
    }

    public void RemoveLayers(string playlistName, params int[] layers)
    {
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
        for (int i = 0; i < layers.Length; i++)
        {
            var idx = Array.FindIndex(m_QueuedLayers, x => x == layers[i]);
            if(idx != -1)
            {
                Utils.DeleteArrayElement(ref m_QueuedLayers, idx);
            }
        }
    }



    #endregion

    void PlayTrack(string trackName, ref (LayeredTrack track, MusicItemLayered item) trackData, float volume = 1f)
    {

        if (m_CurrentPlaylist?.Name == trackName)
        {

            Debug.LogWarning($"Music {trackName} already playing");
            return;
        }

        trackData = InitializeNextItem(trackName);
        trackData.item.SetVolumePercent(volume);
        if (m_CurrentPlaylist == null)
        {
            trackData.item.Play(m_QueuedLayers);
        }
        else
        {
            if (m_QueuedLayers != null)
            {
                trackData.item.Play(m_QueuedLayers);
            }
            else
            {
                var layersFromName = GetLayersFromName(m_QueuedTrack);
                if (layersFromName != null)
                {
                    trackData.item.Play(layersFromName);
                }
                else
                {
                    trackData.item.Play(m_PlaylistCurrentLayers);
                }

            }
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


    void NextSong(float fadeTime)
    {

        switch (m_State)
        {
            case PlayState.None:

                m_PreviousFadePercent = 1;
                if (fadeTime > 0)
                {
                    PlayTrack(m_QueuedTrack, ref m_Current, 0);
                    StartCoroutine(CurrentTracker = FadeInCoroutine(fadeTime));
                }
                else
                {
                    PlayTrack(m_QueuedTrack, ref m_Current);
                    m_State = PlayState.Playing;
                    if (m_CurrentPlaylist == null)
                    {
                        StartCoroutine(CurrentTracker = PlayingCoroutine(m_CrossfadeTimeSingle));
                    }
                    else
                    {
                        StartCoroutine(CurrentTracker = PlayingCoroutine(GetPlaylistCrossfade));
                    }
                }
                break;

            case PlayState.FadeIn:
            case PlayState.CrossFade:
            case PlayState.Playing:
            case PlayState.FadeOut:
                RemoveCoroutine(CurrentTracker);
                if (m_State == PlayState.CrossFade)
                {
                    if (m_Next.item != null)
                    {
                        CleanUpAfterCrossfade();
                    }
                }
                if (fadeTime > 0)
                {
                    PlayTrack(m_QueuedTrack, ref m_Next, 0);
                    StartCoroutine(CurrentTracker = CrossFadeCoroutine(fadeTime));
                }
                else
                {

                    if (m_State != PlayState.FadeOut)
                    {
                        m_PlaylistCurrentLayers = m_Current.item.CurrentLayers;
                    }

                    PlayTrack(m_QueuedTrack, ref m_Next);
                    CleanUpAfterCrossfade();
                    if (m_CurrentPlaylist == null)
                    {
                        StartCoroutine(CurrentTracker = PlayingCoroutine(m_CrossfadeTimeSingle));
                    }
                    else
                    {
                        StartCoroutine(CurrentTracker = PlayingCoroutine(GetPlaylistCrossfade));
                    }
                }

                
                break;
        }

    }

    void OnCoroutineEnd(PlayState state)
    {
        switch (state)
        {
            case PlayState.None:
                break;

            case PlayState.FadeInComplete:
            case PlayState.CrossFadeComplete:
                RemoveCoroutine(CurrentTracker);
                if (m_CurrentPlaylist == null)
                {
                    StartCoroutine(CurrentTracker = PlayingCoroutine(m_CrossfadeTimeSingle));
                }
                else
                {
                    StartCoroutine(CurrentTracker = PlayingCoroutine(GetPlaylistCrossfade));
                }

                break;
            case PlayState.PlayingComplete:
                RemoveCoroutine(CurrentTracker);
                if (m_CurrentPlaylist == null)
                {
                    if (m_LoopSingle)
                    {
                        PlayTrack(m_QueuedTrack, ref m_Next);
                      
                        StartCoroutine(CurrentTracker = CrossFadeCoroutine(m_CrossfadeTimeSingle));
                    }

                }
                else
                {

                    m_ClipIndex = m_CurrentPlaylist.GetTrack(m_ClipIndex);
                    m_QueuedTrack = m_CurrentPlaylist.MusicTracks[m_ClipIndex];

                    if (m_CurrentPlaylist.Crossfade)
                    {

                        PlayTrack(m_QueuedTrack, ref m_Next, 0);
                        StartCoroutine(CurrentTracker = CrossFadeCoroutine(m_CurrentPlaylist.CrossfadeTime));
                    }
                    else
                    {
                        RemoveTrack(ref m_Current);
                        PlayTrack(m_QueuedTrack, ref m_Current);
                        StartCoroutine(CurrentTracker = PlayingCoroutine(0));
                    }
                }
                break;
        }
    }


    void OnEnd()
    {
        OnCoroutineEnd(PlayState.PlayingComplete);
    }

    IEnumerator PlayingCoroutine(float shortenByTime)
    {

        Invoke(nameof(OnEnd), m_Current.track.Length - m_Current.item.GetCurrentTime - shortenByTime);
        yield return null;
        //yield return new WaitForSeconds(m_Current.track.Length - m_Current.item.GetCurrentTime - shortenByTime);
       
    }

    IEnumerator FadeInCoroutine(float fadeInTime)
    {
        m_State = PlayState.FadeIn;
        float timer = 0;
        while (m_State == PlayState.FadeIn)
        {
            if (timer <= fadeInTime)
            {
                m_CurrentFadePercent = timer / fadeInTime;
                m_Current.item.SetVolumePercent(m_CurrentFadePercent * m_PreviousFadePercent);

                yield return new WaitForSeconds(DeltaTime);
            }
            else
            {
                m_CurrentFadePercent = 1;
                m_Current.item.SetVolumePercent(1);
                m_State = PlayState.Playing;
            }
            timer += DeltaTime;
        }
        OnCoroutineEnd(PlayState.FadeInComplete);
    }

    IEnumerator CrossFadeCoroutine(float crossFadeTime)
    {
        m_State = PlayState.CrossFade;
        float timer = 0;
        while (m_State == PlayState.CrossFade)
        {
            if (timer <= crossFadeTime)
            {
                m_CurrentFadePercent = timer / crossFadeTime;
                m_Current.item.SetVolumePercent((1 - m_CurrentFadePercent) * m_PreviousFadePercent);
                m_Next.item.SetVolumePercent(m_CurrentFadePercent);

                yield return new WaitForSeconds(DeltaTime);
            }
            else
            {
                m_CurrentFadePercent = 1;
                m_Current.item.SetVolumePercent(0);
                m_Next.item.SetVolumePercent(1);
                CleanUpAfterCrossfade();
                m_State = PlayState.Playing;
            }
            timer += DeltaTime;
        }
        OnCoroutineEnd(PlayState.CrossFadeComplete);
    }

    IEnumerator FadeOutCoroutine(float fadeOutTime)
    {
        m_State = PlayState.FadeOut;
        float timer = 0;
        while (m_State == PlayState.FadeOut)
        {
            if (timer <= fadeOutTime)
            {
                m_CurrentFadePercent = timer / fadeOutTime;
                if (m_Current.item != null)
                {
                    m_Current.item.SetVolumePercent((1 - m_CurrentFadePercent) * m_PreviousFadePercent);
                }
                if (m_Next.item != null)
                {
                    m_Next.item.SetVolumePercent((1 - m_CurrentFadePercent) * m_PreviousFadePercent);
                }

                yield return new WaitForSeconds(DeltaTime);
            }
            else
            {
                m_CurrentFadePercent = 1;
                m_Current.item.SetVolumePercent(0);
                
                RemoveTrack(ref m_Current);

                if (m_Next.item!=  null)
                {
                    RemoveTrack(ref m_Next);
                }
                m_State = PlayState.None;
            }
            timer += DeltaTime;
        }
        RemoveCoroutine(CurrentTracker);
    }

    void CleanUpAfterCrossfade()
    {
        RemoveTrack(ref m_Current);
        m_Current = m_Next;
        m_Next = default;
    }

  

    void RemoveCoroutine(IEnumerator enumerator)
    {
        if (enumerator != null)
        {
            m_PreviousFadePercent = m_CurrentFadePercent;
            StopCoroutine(enumerator);
        }
    }



    

   


}

public enum PlayState
{
    None,
    FadeIn,
    FadeInComplete,
    Playing,
    PlayingComplete,
    CrossFade,
    CrossFadeComplete,
    FadeOut
}

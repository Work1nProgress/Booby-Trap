using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicItemLayered : MonoBehaviour
{

    MusicItem[] m_SubItems;


    LayeredTrack m_Track;



    int m_CurrentLayers;
    public int CurrentLayers => m_CurrentLayers;



    float GetCurrentTime
    {
        get
        {
            for (int i = 0; i < m_SubItems.Length; i++)
            {
                if (m_SubItems[i].IsPlaying)
                {
                    return m_SubItems[i].Time;
                }
            }
            return 0f;
        }
    }




    public MusicItemLayered Init()
    {

        return this;
    }

    public void StopAndShutDown()
    {
        for (int i = m_SubItems.Length - 1; i >= 0; i--)
        {
            m_SubItems[i].Stop();
            PoolManager.Despawn(m_SubItems[i]);
        }
    }


    public void Set(LayeredTrack track)
    {

        m_CurrentLayers = 0;
        m_Track = track;
        m_SubItems = new MusicItem[m_Track.LayerCount];

            for (int i = 0; i < m_Track.LayerCount; i++)
            {
                m_SubItems[i] = PoolManager.Spawn<MusicItem>("MusicItem", transform);
                m_SubItems[i].gameObject.SetActive(false);
            }
        
 
    }

    public void SetVolumePercent(float percent)
    {
        for (int i = 0; i < m_SubItems[i].Length; i++)
        {
            m_SubItems[i].SetVolume(m_Track.Tracks[i].Volume * percent);

        }
    }


    public void Play(params int[] layers)
    {

        

        int m_flags = Utils.AddFlags(m_CurrentLayers, layers);
        if (m_CurrentLayers == m_flags)
        {
            Debug.Log($"already playing same layers");
            return;
        }
        m_CurrentLayers = m_flags;
        var flagList = Utils.FlagsToList(m_CurrentLayers);
        foreach (var layer in flagList)
        {
            if (!IsLayerValid(layer))
            {
                continue;
            }
            if (!m_SubItems[layer].IsPlaying)
            {
                m_SubItems[layer].SetTime(GetCurrentTime);
                m_SubItems[layer].Play();
            }
        }
    }

    public void Stop(params int[] layers)
    {
        m_CurrentLayers = Utils.RemoveFlags(m_CurrentLayers, layers);
        foreach (var layer in layers)
        {
            if (!IsLayerValid(layer))
            {
                continue;
            }
            if (m_SubItems[layer].IsPlaying)
            {
                m_SubItems[layer].Stop();
            }
        }
      
    }

    bool IsLayerValid(int layer)
    {
        return layer >= 0 && layer < m_SubItems.Length;
    }






}

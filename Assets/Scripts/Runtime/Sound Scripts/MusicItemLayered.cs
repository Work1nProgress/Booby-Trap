using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class MusicItemLayered : MonoBehaviour
{

    MusicItem[] m_SubItems;


    LayeredTrack m_Track;



    bool[] m_CurrentLayers;
    public int[] CurrentLayers
    {
        get
        {
            var trueFlags = System.Array.FindAll(m_CurrentLayers, x => x);
            var layers = new int[trueFlags.Length];
            int currentIndex = 0;
            for (int i = 0; i < m_CurrentLayers.Length; i++)
            {
                if (m_CurrentLayers[i])
                {
                    layers[currentIndex] = i;
                    currentIndex++;
                }
            }
            return layers;

        }

    }


    public float GetCurrentTime
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
            m_CurrentLayers[i] = false;
            PoolManager.Despawn(m_SubItems[i]);
        }
    }


    public void Set(LayeredTrack track)
    {

        if (m_CurrentLayers != null && m_CurrentLayers.Length == track.LayerCount)
        {
            System.Array.Fill(m_CurrentLayers, false);
        }
        else
        {
            m_CurrentLayers = new bool[track.LayerCount];
        }
        m_Track = track;
        m_SubItems = new MusicItem[m_Track.LayerCount];

            for (int i = 0; i < m_Track.LayerCount; i++)
            {
                m_SubItems[i] = PoolManager.Spawn<MusicItem>("MusicItem", transform);
                m_SubItems[i].Set(m_Track.Tracks[i], m_Track.Volume * m_Track.Tracks[i].Volume);
                m_SubItems[i].gameObject.SetActive(false);
            }
        
 
    }

    public void SetVolumePercent(float percent)
    {
        for (int i = 0; i < m_SubItems.Length; i++)
        {
            m_SubItems[i].SetVolume(m_Track.Volume * m_Track.Tracks[i].Volume * percent);

        }
    }

   


    public void Play(params int[] layers)
    {
        if (layers == null || layers.Length == 0)
        {
            layers = new int[]{0};
        }
        foreach (var layer in layers)
        {
            if (!IsLayerValid(layer))
            {
                continue;
            }
            if (!m_SubItems[layer].IsPlaying)
            {
                m_SubItems[layer].gameObject.SetActive(true);
                m_SubItems[layer].SetTime(GetCurrentTime);
                m_SubItems[layer].Play();
                m_CurrentLayers[layer] = true;
            }
        }     
    }

    public void Stop(params int[] layers)
    {
        foreach (var layer in layers)
        {
            if (!IsLayerValid(layer))
            {
                continue;
            }

            if (m_SubItems[layer].IsPlaying)
            {
                m_SubItems[layer].Stop();
                m_SubItems[layer].gameObject.SetActive(false);
                m_CurrentLayers[layer] = false;
            }
        }   
    }

    bool IsLayerValid(int layer)
    {
        return layer >= 0 && layer < m_SubItems.Length;
    }
}

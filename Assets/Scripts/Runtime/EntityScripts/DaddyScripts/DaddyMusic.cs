using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using DG.Tweening;
public class DaddyMusic : MonoBehaviour
{
    HealthMusicLayer currentLayers;
    [SerializeField]
    AudioMixer _MusicVolume;

    float MaxHealth;
    DaddyController daddyController;

    [SerializeField]
    List<HealthMusicLayer> musicLayers;

    float currentHealthPercent;
    int currentLayerIndex;
    string playlistName = "daddyPlaylist";

    public float previousMusicVolume;

    private void Start()
    {
        foreach (var layers in musicLayers)
        {
            layers.Init();
        }
        daddyController = GetComponent<DaddyController>();
        MaxHealth = daddyController.MaxHealth;
        daddyController.OnChangeHealth.AddListener(OnHealthChanged);
        daddyController.OnDeath.AddListener(OnDeath);
        musicLayers = musicLayers.OrderByDescending(x => x.HealthPercent).ToList();
        currentLayers = musicLayers[0];
        currentHealthPercent = 1f;
       // MusicPlayer.Instance.PlayPlaylist(playlistName, 7f, currentLayers.LayersList.ToArray());
    }

    public void ResetMusic()
    {
        MusicPlayer.Instance.StopPlaying();
        currentLayers = musicLayers[0];
        currentHealthPercent = 1f;
        
        MusicPlayer.Instance.PlayPlaylist(playlistName, 6f, currentLayers.LayersList.ToArray());
    }

    public void ResetVolume(float delay)
    {

        _MusicVolume.DOSetFloat("MusicVolume", previousMusicVolume, 0.5f).SetDelay(delay);
    }


    void OnHealthChanged(int change) {

        currentHealthPercent = (float)daddyController.Health / MaxHealth;
        if (currentHealthPercent <= 0 || currentLayerIndex + 1 > musicLayers.Count-1)
        {
            return;
        }
        if (currentHealthPercent < musicLayers[currentLayerIndex + 1].HealthPercent)
        {
            ChangeTo(musicLayers[currentLayerIndex + 1]);

             _MusicVolume.GetFloat("MusicVolume", out previousMusicVolume);
            Debug.Log(previousMusicVolume);
            _MusicVolume.DOSetFloat("MusicVolume", -50f, 0.5f);


        }
    }

    void ChangeTo(HealthMusicLayer healthMusicLayer)
    {
        List<int> LayersToRemove = new List<int>();

        for (int i = 0; i < currentLayers.LayersList.Count; i++)
        {

            if (!healthMusicLayer.LayersList.Contains(currentLayers.LayersList[i]))
            {

                LayersToRemove.Add(currentLayers.LayersList[i]);
            }
        }
        MusicPlayer.Instance.RemoveLayers(playlistName, LayersToRemove.ToArray());
        MusicPlayer.Instance.AddLayers(playlistName, healthMusicLayer.LayersList.ToArray());
        currentLayerIndex = musicLayers.IndexOf(healthMusicLayer);
        currentLayers = healthMusicLayer;
    }

    void OnDeath()
    {
        MusicPlayer.Instance.StopPlaying(4);
    }







}

[System.Serializable]
public class HealthMusicLayer
{

    [System.NonSerialized]
    public List<int> LayersList;
   public float HealthPercent;
    public string Layers;

    public void Init()
    {
        LayersList = new List<int>();
        var split = Layers.Split(",");
        foreach (var s in split)
        {
            LayersList.Add(int.Parse(s));
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DaddyMusic : MonoBehaviour
{
    HealthMusicLayer currentLayers;

    float MaxHealth;
    DaddyController daddyController;

    [SerializeField]
    List<HealthMusicLayer> musicLayers;

    float currentHealthPercent;
    int currentLayerIndex;
    string playlistName = "daddyPlaylist";

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

        MusicPlayer.Instance.PlayPlaylist(playlistName, 0.5f, currentLayers.LayersList.ToArray());
    }



    void OnHealthChanged(int change) {

        currentHealthPercent = (float)daddyController.Health / MaxHealth;
        if (currentHealthPercent <= 0)
        {
            return;
        }
        if (currentHealthPercent < musicLayers[currentLayerIndex + 1].HealthPercent)
        {
            List<int> LayersToRemove = new List<int>();

            for (int i = 0; i < currentLayers.LayersList.Count; i++)
            {
                Debug.Log(currentLayers.LayersList[i]);
                if (!musicLayers[currentLayerIndex + 1].LayersList.Contains(currentLayers.LayersList[i]))
                {

                    Debug.Log($"remove layer {currentLayers.LayersList[i]}");
                    LayersToRemove.Add(currentLayers.LayersList[i]);
                }
            }
            MusicPlayer.Instance.RemoveLayers(playlistName, LayersToRemove.ToArray());
            MusicPlayer.Instance.AddLayers(playlistName, musicLayers[currentLayerIndex+1].LayersList.ToArray());
            currentLayerIndex++;
            currentLayers = musicLayers[currentLayerIndex];

        }
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

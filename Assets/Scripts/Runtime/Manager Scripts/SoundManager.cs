using System;
using System.Collections.Generic;

using UnityEngine.Audio;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SoundManager : GenericSingleton<SoundManager>
{

 
    public AudioMixerGroup Mixer;

    [SerializeField] private List<Sound> sounds;


    public List<Sound> Sounds => sounds;

    List<SoundInstance> PlayingSounds = new List<SoundInstance>();
    List<SoundInstance> RecentSounds = new List<SoundInstance>();
    List<LastPlayedHelper> LastPlayed = new List<LastPlayedHelper>();


    string SoundInstanceName = "SoundInstance";

    public float MinDistance;
    public float MaxDistance;
    public float SpatialBlend;


    public float Spread;

    public float ReverbZoneMix;



    public void Play(string name, Transform target = default)
    {

        var idx = sounds.FindIndex(x => x.SoundName == name);
        if (idx == -1)
        {
            Debug.LogError($"Sound {name} not found");
            return;
        }
        else
        {
            if (RecentSounds.FindIndex(x => x.SoundName == name) != -1)
            {
                Debug.Log($"already playing {name}");
                return;
            }

            var a = UnityEngine.Random.value;
            Debug.Log($"{name} {a} {sounds[idx].ChanceToPlay} {sounds[idx].ChanceToPlay / 100f}");
            if (a >= (sounds[idx].ChanceToPlay / 100f))
            {
                return;
            }
            Vector3 targetPosition = target == default ? Camera.main.transform.position : target.position;
            var soundInstance = PoolManager.Spawn<SoundInstance>(SoundInstanceName, null, targetPosition);
            List<SoundItem> previous = new List<SoundItem>();
            if (sounds[idx].PlaySubitemType == PlaySubitemType.RandomNotSameTwice)
            {
                var allPrevious = LastPlayed.FindAll(x => x.SoundName == name);
                for (int i = allPrevious.Count - 1; i >= 0; i--)
                {
                    allPrevious[i].SkipsLeft--;
                    if (allPrevious[i].SkipsLeft <= 0)
                    {
                        LastPlayed.Remove(allPrevious[i]);
                    }
                    else
                    {
                        previous.Add(allPrevious[i].SoundItem);
                    }
                }
            }

          

            SceneManager.MoveGameObjectToScene(soundInstance.gameObject, SceneManager.GetActiveScene());
            var next = sounds[idx].SoundItem(previous);
            soundInstance.name = $"{sounds[idx].SoundName}_{idx}";
            soundInstance.Set(next, sounds[idx], target, MinDistance, MaxDistance, Spread, ReverbZoneMix, SpatialBlend);


            var length = soundInstance.Play();


            PlayingSounds.Add(soundInstance);
            if (sounds[idx].MinimumTimeBetweenSameSound > 0)
            {
                RecentSounds.Add(soundInstance);
                StartCoroutine(RemoveFromList(Mathf.Min(sounds[idx].MinimumTimeBetweenSameSound, soundInstance.Length), RecentSounds, soundInstance));
            }

            StartCoroutine(DespawnAfter(soundInstance.Length, PlayingSounds, soundInstance));
            soundInstance.Play();

            if (sounds[idx].PlaySubitemType == PlaySubitemType.RandomNotSameTwice)
            {

                if (LastPlayed.Find(x => x.SoundItem == next) == null)
                {
                    LastPlayed.Add(new LastPlayedHelper
                    {
                        SoundName = name,
                        SoundItem = next,
                        SkipsLeft = sounds[idx].notSameTwiceOffset
                    });
                }
                
            }
        }
    }

    IEnumerator RemoveFromList(float time, List<SoundInstance> list, SoundInstance instance)
    {
        if (time > 0)
        {
            yield return new WaitForSeconds(time);
        }
        list.Remove(instance);
    }

    IEnumerator DespawnAfter(float time, List<SoundInstance> list, SoundInstance instance)
    {
        if (time > 0)
        {
            yield return new WaitForSeconds(time);
        }
        list.Remove(instance);
        PoolManager.Despawn(instance);
    }

    public class LastPlayedHelper
    {

        public string SoundName;
        public SoundItem SoundItem;
        public int SkipsLeft;

    }
}

   

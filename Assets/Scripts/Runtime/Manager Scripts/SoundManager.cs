using System;
using System.Collections.Generic;

using UnityEngine.Audio;
using UnityEngine;
using System.Collections;

public class SoundManager : GenericSingleton<SoundManager>
{

    [SerializeField]
    AudioMixerGroup Mixer;

    [SerializeField] private List<Sound> sounds;


    public List<Sound> Sounds => sounds;

    List<SoundInstance> PlayingSounds = new List<SoundInstance>();
    List<SoundInstance> RecentSounds = new List<SoundInstance>();
    Dictionary<string, SoundItem> LastPlayed;

    public float MinimumTimeBetweenSameSound;
    string SoundInstanceName = "SoundInstance";



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
                //Debug.Log($"already playing that sound");
                return;
            }
            Vector3 targetPosition = target == default ? Camera.main.transform.position : target.position;
            var soundInstance = PoolManager.Spawn<SoundInstance>(SoundInstanceName, null, targetPosition);
            SoundItem previous = null;
            if (sounds[idx].PlaySubitemType == PlaySubitemType.RandomNotSameTwice)
            {
                LastPlayed.TryGetValue(name, out previous);
            }
            var next = sounds[idx].SoundItem(previous);
            soundInstance.Set(next, target);
            var length = soundInstance.Play();

            PlayingSounds.Add(soundInstance);
            if (MinimumTimeBetweenSameSound > 0)
            {
                RecentSounds.Add(soundInstance);
            }





            if (sounds[idx].PlaySubitemType == PlaySubitemType.RandomNotSameTwice)
            {
                if (LastPlayed.ContainsKey(name)){
                    LastPlayed[name] = next;
                }
                else
                {
                    LastPlayed.Add(name, next);
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
}

   

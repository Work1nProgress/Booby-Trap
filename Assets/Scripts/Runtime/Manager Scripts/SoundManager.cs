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

    List<LoopHelper> LoopingSounds = new List<LoopHelper>();



    string SoundInstanceName = "SoundInstance";

    public float MinDistance;
    public float MaxDistance;
    public bool UseSpatialSound;





    public void PlayLooped(string name, GameObject source = null, Transform target = default)
    {

        if (LoopingSounds.Exists(x => x.SoundName == name && x.Source == source))
        {
            return;
        }

        var idx = CanPlay(name);
        if (idx == -1)
        {
            return;
        }


        var soundInstance = PlayInstance(idx, name, target);

        LoopingSounds.Add(new LoopHelper
        {
            SoundInstance = soundInstance,
            Source = source,
            SoundName = name

        });;
        soundInstance.SetLoops(true);
        soundInstance.Play();
    }


    public void CancelLoop(GameObject source)
    {

        var loopingSounds = LoopingSounds.FindAll(x => x.Source == source);
        foreach (var loopingSound in loopingSounds)
        {
            if (loopingSound != null)
            {
                RemoveFromLooping(loopingSound);
            }
        }

    }

    public void CancelLoop(string soundName, GameObject source)
    {
        var loopingSound = LoopingSounds.Find(x => x.SoundName == soundName && x.Source == source);
        if(loopingSound != null){

            RemoveFromLooping(loopingSound);
        }
    }

    void RemoveFromLooping(LoopHelper loopingSound)
    {
        LoopingSounds.Remove(loopingSound);
        PlayingSounds.Remove(loopingSound.SoundInstance);
        PoolManager.Despawn(loopingSound.SoundInstance);
    }

    public void Play(string soundName, Transform target = default)
    {
        var idx = CanPlay(soundName);
        if (idx == -1)
        {
            return;
        }
        PlayOneShot(idx, soundName, target);
    }

    public void PlayDelayed(string soundName, float delay, Transform target = default)
    {
        StartCoroutine(WaitforDelaye(soundName, delay, transform));
    }

    IEnumerator WaitforDelaye(string soundName, float delay, Transform target) {


        yield return new WaitForSeconds(delay);
        Play(soundName, transform);
    }

    public void PlayOneShot(int idx, string soundName, Transform target)
    {


        var soundInstance = PlayInstance(idx, soundName, target);
        soundInstance.SetLoops(false);
        soundInstance.Play();
        StartCoroutine(DespawnAfter(soundInstance.Length, PlayingSounds, soundInstance));
        
    }


    private SoundInstance PlayInstance(int idx, string soundName, Transform target)
    {
        Vector3 targetPosition = target == default ? Camera.main.transform.position : target.position;
        var soundInstance = PoolManager.Spawn<SoundInstance>(SoundInstanceName, null, targetPosition);
        List<SoundItem> previous = new List<SoundItem>();
        if (sounds[idx].PlaySubitemType == PlaySubitemType.RandomNotSameTwice)
        {
            var allPrevious = LastPlayed.FindAll(x => x.SoundName == soundName);
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
        soundInstance.Set(next, sounds[idx], target, MinDistance, MaxDistance, UseSpatialSound);


        var length = soundInstance.Play();


        PlayingSounds.Add(soundInstance);
        if (sounds[idx].MinimumTimeBetweenSameSound > 0)
        {
            RecentSounds.Add(soundInstance);
            StartCoroutine(RemoveFromList(Mathf.Min(sounds[idx].MinimumTimeBetweenSameSound, soundInstance.Length), RecentSounds, soundInstance));
        }
        


       

        if (sounds[idx].PlaySubitemType == PlaySubitemType.RandomNotSameTwice)
        {

            if (LastPlayed.Find(x => x.SoundItem == next) == null)
            {
                LastPlayed.Add(new LastPlayedHelper
                {
                    SoundName = soundName,
                    SoundItem = next,
                    SkipsLeft = sounds[idx].notSameTwiceOffset
                });
            }

        }
        return soundInstance;
    }


    int CanPlay(string soundName)
    {
        var idx = sounds.FindIndex(x => x.SoundName == soundName);
        if (idx == -1)
        {
            Debug.LogWarning($"Sound {soundName} not found");
            return -1;
        }
        else
        {
            if (RecentSounds.FindIndex(x => x.SoundName == soundName) != -1)
            {
                //    Debug.Log($"already playing {name}");
                return -1;
            }


            if (UnityEngine.Random.value >= (sounds[idx].ChanceToPlay / 100f))
            {
                return -1;
            }
        }
        return idx;
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

    public class LoopHelper
    {

        public string SoundName;
        public GameObject Source;
        public SoundInstance SoundInstance;

    }
}

   

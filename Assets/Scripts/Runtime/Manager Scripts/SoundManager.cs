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
    Dictionary<string, SoundItem> LastPlayed;


    string SoundInstanceName = "SoundInstance";

    public float MinDistance;
    public float MaxDistance;

    public AnimationCurve SpatialBlend = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 0));
    public AnimationCurve Spread = new AnimationCurve(new Keyframe(0, 1), new Keyframe(360, 0));
    public AnimationCurve CustomRolloff = new AnimationCurve(new Keyframe(0, 1), new Keyframe(0.2f,0.8f), new Keyframe(0.4f, 0.5f), new Keyframe(0.7f, 0.2f), new Keyframe(1, 0f));
    public AnimationCurve ReverbZoneMix = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 0));

    Dictionary<AudioSourceCurveType, AnimationCurve> curves = new Dictionary<AudioSourceCurveType, AnimationCurve>();

    protected override void Awake()
    {
        base.Awake();
        curves.Add(AudioSourceCurveType.CustomRolloff, CustomRolloff);
        curves.Add(AudioSourceCurveType.ReverbZoneMix, ReverbZoneMix);
        curves.Add(AudioSourceCurveType.Spread, Spread);
        curves.Add(AudioSourceCurveType.SpatialBlend, SpatialBlend);
    }

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
            Vector3 targetPosition = target == default ? Camera.main.transform.position : target.position;
            var soundInstance = PoolManager.Spawn<SoundInstance>(SoundInstanceName, null, targetPosition);
            SoundItem previous = null;
            if (sounds[idx].PlaySubitemType == PlaySubitemType.RandomNotSameTwice)
            {
                LastPlayed.TryGetValue(name, out previous);
            }

            SceneManager.MoveGameObjectToScene(soundInstance.gameObject, SceneManager.GetActiveScene());
            var next = sounds[idx].SoundItem(previous);
            soundInstance.name = $"{sounds[idx].SoundName}_{idx}";
            soundInstance.Set(next, sounds[idx], target, curves, MinDistance, MaxDistance);
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

   

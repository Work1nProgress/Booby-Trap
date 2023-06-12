using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(SoundManager))]
public class SoundManagerEditor : Editor
{

    SoundManager data;


    bool m_DefaultSettingsFoldout, m_CurrentSoundFoldout, m_CurrentSubitemFoldout;
    int currentSoundIndex;
    int currentSubitemIndex = 0;

    public void OnEnable()
    {
        data = target as SoundManager;

        //remove empty subitems
        foreach (var sound in data.Sounds)
        {
            for (int i = sound.soundItems.Length - 1; i >= 0; i--)
            {
                if (sound.soundItems[i].AudioClip == null)
                {
                    Utils.DeleteArrayElement(ref sound.soundItems, i);
                }
            }

        }

        currentSoundIndex = EditorPrefs.GetInt("soundIndex", 0);
        m_DefaultSettingsFoldout = EditorPrefs.GetBool("defaultSettingsFoldout", true);
        m_CurrentSoundFoldout = EditorPrefs.GetBool("currentSoundFoldout", true);
        m_CurrentSubitemFoldout = EditorPrefs.GetBool("currentSubItemFoldout", true);
    }

    public void OnDisable()
    {
        //remove empty subitems
        foreach (var sound in data.Sounds)
        {
            for (int i = sound.soundItems.Length - 1; i >= 0; i--)
            {
                if (sound.soundItems[i].AudioClip == null)
                {
                    Utils.DeleteArrayElement(ref sound.soundItems, i);
                }
            }

        }
        Save();
    }
    public override void OnInspectorGUI()
    {

        EditorGUI.BeginChangeCheck();

        m_DefaultSettingsFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(m_DefaultSettingsFoldout, "Default Settings", EditorStyles.foldoutHeader);
        if (m_DefaultSettingsFoldout)
        {

            GUILayout.BeginVertical(EditorStyles.helpBox);
            data.Mixer = (UnityEngine.Audio.AudioMixerGroup)EditorGUILayout.ObjectField("Mixer", data.Mixer, typeof(UnityEngine.Audio.AudioMixerGroup), false);
            data.MinDistance = EditorGUILayout.FloatField(new GUIContent("Min Distance", "Closest distance that custom curves change"), data.MinDistance, EditorStyles.numberField);
            data.MaxDistance = EditorGUILayout.FloatField(new GUIContent("Max Distance", "Furthest distance that custom curves change"), data.MaxDistance, EditorStyles.numberField);


            
            data.UseSpatialSound = EditorGUILayout.Toggle(new GUIContent("Spatial blend", "Sets if 3D engine has an effect on the audio source"), data.UseSpatialSound,EditorStyles.toggle);

            GUILayout.EndVertical();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
       


        GUILayout.BeginHorizontal();

        if (GUILayout.Button(new GUIContent("Add selected audio clips", "Adds all the currently selected audio clips to a new Sound. Hint:use inspector lock to select files."), EditorStyles.miniButton, GUILayout.Width(200)))
        {
            AudioClip[] audioClips = GetSelectedAudioclips();
            if (audioClips.Length > 0)
            {
                CreateNewSound(audioClips);
            }
        }
        if (GUILayout.Button("+", GUILayout.Width(30)))
        {
            CreateNewSound();
        }
        EditorGUI.BeginDisabledGroup(data.Sounds.Count <= 0);
            if (GUILayout.Button("-", GUILayout.Width(30))) {

                data.Sounds.RemoveAt(currentSoundIndex);
                currentSoundIndex--;
                currentSoundIndex = Mathf.Clamp(currentSoundIndex, 0, data.Sounds.Count - 1);
            }

        EditorGUI.EndDisabledGroup();

        GUILayout.EndHorizontal();
        if (data.Sounds == null || data.Sounds.Count == 0)
        {
            GUILayout.Label("No sounds added, create a new sound!");
            return;
        }




        
        m_CurrentSoundFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(m_CurrentSoundFoldout, "Current Sound Settings", EditorStyles.foldoutHeader);
        if (m_CurrentSoundFoldout)
        {
            var soundsNames = data.Sounds.Select(x => x.SoundName).ToArray();
            int currentSound = currentSoundIndex;
            currentSoundIndex = EditorGUILayout.Popup("Current sound", currentSoundIndex, soundsNames);
            if (currentSound != currentSoundIndex)
            {
                currentSubitemIndex = 0;
            }

            if (currentSoundIndex < 0)
            {
                currentSoundIndex = 0;
            }
            if (currentSoundIndex >= data.Sounds.Count)
            {
                currentSoundIndex = data.Sounds.Count - 1;
            }
            var sound = data.Sounds[currentSoundIndex];
            GUILayout.BeginVertical(EditorStyles.helpBox);
            sound.SoundName = EditorGUILayout.TextField("Sound Name", sound.SoundName, EditorStyles.textField);

            sound.Volume = EditorGUILayout.Slider(new GUIContent("Modify Volume", "0 is default value"), sound.Volume, - 0.5f, 0.5f);
            sound.PitchShift = EditorGUILayout.FloatField(new GUIContent("Pitch shift", "0 is default value"), sound.PitchShift, EditorStyles.numberField);
            sound.RandomPitch = EditorGUILayout.FloatField(new GUIContent("Random Pitch ±"), sound.RandomPitch, EditorStyles.numberField);
            sound.DopplerLevel = EditorGUILayout.Slider(new GUIContent("Doppler Level"), sound.DopplerLevel, 0, 1);
            sound.MinimumTimeBetweenSameSound = EditorGUILayout.FloatField(new GUIContent("Min Time Between Play", "If attempted to play before, it will skip the command."), sound.MinimumTimeBetweenSameSound, EditorStyles.numberField);
            sound.ChanceToPlay = EditorGUILayout.FloatField(new GUIContent("Chance to play", "If less than 100%, there is a chance this sound will not play."), sound.ChanceToPlay, EditorStyles.numberField);

            sound.PlaySubitemType = (PlaySubitemType)EditorGUILayout.EnumPopup("Play Mode", sound.PlaySubitemType, EditorStyles.popup);
            if (sound.PlaySubitemType == PlaySubitemType.RandomNotSameTwice)
            {
                sound.notSameTwiceOffset = EditorGUILayout.IntSlider("Skip last:", sound.notSameTwiceOffset, 0, sound.soundItems.Length - 1);
            }
            sound.OverrideCustomSpatialBlend = EditorGUILayout.Toggle(new GUIContent("Override Spatial Blend"), sound.OverrideCustomSpatialBlend, EditorStyles.toggle);
            EditorGUI.BeginDisabledGroup(!sound.OverrideCustomSpatialBlend);
            
            sound.UseCustomSpatialBlend = EditorGUILayout.Toggle(new GUIContent("Spatial blend", "Sets how much if 3D engine has an effect on the audio source."), sound.UseCustomSpatialBlend, EditorStyles.toggle);
            EditorGUI.EndDisabledGroup();

          

         
            sound.UseCustomMinMax = EditorGUILayout.Toggle(new GUIContent("Custom min max"), sound.UseCustomMinMax, EditorStyles.toggle);
            if (sound.UseCustomMinMax)
            {
                sound.MinDistance = EditorGUILayout.FloatField(new GUIContent("Min Distance", "Closest distance that custom curves change"), sound.MinDistance, EditorStyles.numberField);
                sound.MaxDistance = EditorGUILayout.FloatField(new GUIContent("Max Distance", "Furthest distance that custom curves change"), sound.MaxDistance, EditorStyles.numberField);
            }


            GUILayout.EndVertical();

        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        m_CurrentSubitemFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(m_CurrentSubitemFoldout, "Current Sound SubItem", EditorStyles.foldoutHeader);

        if (m_CurrentSubitemFoldout)
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);



            GUILayout.BeginHorizontal();

            if (GUILayout.Button(new GUIContent("Add selected audio clips as subitems", "Adds all the currently selected audio filed to the current Sound as subItems. Hint:use inspector lock to select files."), EditorStyles.miniButton, GUILayout.Width(300)))
            {
                AudioClip[] audioClips = GetSelectedAudioclips();
                if (audioClips.Length > 0)
                {
                    foreach (var a in audioClips) {
                        data.Sounds[currentSoundIndex].AddSoundItem(a);
                    }
                }
            }
            if (GUILayout.Button("+", GUILayout.Width(30)))
            {
                Debug.Log("add subitem");
                data.Sounds[currentSoundIndex].AddSoundItem(null);
            }
            EditorGUI.BeginDisabledGroup(data.Sounds.Count <= 0);
            if (GUILayout.Button("-", GUILayout.Width(30)))
            {

                Utils.DeleteArrayElement(ref data.Sounds[currentSoundIndex].soundItems, currentSubitemIndex);
                currentSubitemIndex--;
                currentSubitemIndex = Mathf.Clamp(currentSubitemIndex, 0, data.Sounds[currentSoundIndex].soundItems.Length - 1);
            }

            EditorGUI.EndDisabledGroup();

            GUILayout.EndHorizontal();




            if (data.Sounds[currentSoundIndex].soundItems.Length == 0)
            {
                GUILayout.Label("No subitem added, create a new subitem!");
                GUILayout.EndVertical();
                return;
            }
            var subitemNames = new string[data.Sounds[currentSoundIndex].soundItems.Length];
            for (int i = 0; i < subitemNames.Length; i++)
            {
                if (data.Sounds[currentSoundIndex] != null)
                {
                    subitemNames[i] = data.Sounds[currentSoundIndex].SoundName + $"_{i}";
                }
                else
                {
                    subitemNames[i] = $"empty_{i}";
                }
            }
            currentSubitemIndex = EditorGUILayout.Popup("Subitem", currentSubitemIndex, subitemNames);
            data.Sounds[currentSoundIndex].soundItems[currentSubitemIndex].AudioClip = (AudioClip)EditorGUILayout.ObjectField(
                "Audio Clip",
                data.Sounds[currentSoundIndex].soundItems[currentSubitemIndex].AudioClip,
                typeof(AudioClip),
                false);

            GUILayout.EndVertical();

        }
        EditorGUILayout.EndFoldoutHeaderGroup();



        if (EditorGUI.EndChangeCheck())
        {
            Save();
        }
        GUILayout.Label("Debug");
        showDefaultInspector = EditorGUILayout.Toggle("show default inspector", showDefaultInspector);
        if (showDefaultInspector)
        {
            base.OnInspectorGUI();
        }
    }
    bool showDefaultInspector;

    void Save()
    {
        EditorPrefs.SetInt("soundIndex", currentSoundIndex);
        EditorPrefs.SetBool("defaultSettingsFoldout", m_DefaultSettingsFoldout);
        EditorPrefs.SetBool("currentSoundFoldout", m_CurrentSoundFoldout);
        EditorPrefs.SetBool("currentSubItemFoldout", m_CurrentSubitemFoldout);

       
        if (target != null)
        {
            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssetIfDirty(target);
        }
    }

    string FindName()
    {
        while (true)
        {
            int counter = 0;
            if (!data.Sounds.Any(x => x.SoundName == $"new_sound_{counter}"))
            {
               return $"new_sound_{counter}";
            }
            counter++;
        }

    }

    void CreateNewSound(params AudioClip[] audioClips) {

        var sound = new Sound
        {
            PitchShift = -0.5f,
            RandomPitch = 0.5f,
            notSameTwiceOffset = 2,
            PlaySubitemType = PlaySubitemType.RandomNotSameTwice

        };

        for (int i = 0; i < audioClips.Length; i++)
        {
            sound.AddSoundItem(audioClips[i]);
        }
        sound.SoundName = FindName();

        data.Sounds.Add(sound);


        currentSoundIndex = data.Sounds.IndexOf(sound);
    }

    static AudioClip[] GetSelectedAudioclips()
    {
        var objList = Selection.GetFiltered(typeof(AudioClip), SelectionMode.DeepAssets);
        var clipList = new AudioClip[objList.Length];

        for (int i = 0; i < objList.Length; i++)
        {
            clipList[i] = (AudioClip)objList[i];
        }

        return clipList;
    }
}

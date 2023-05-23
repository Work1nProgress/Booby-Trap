using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SoundManager))]
public class SoundManagerEditor : Editor
{

    SoundManager data;


    int currentSoundIndex;

    public void OnEnable()
    {
        data = target as SoundManager;
        currentSoundIndex = EditorPrefs.GetInt("soundIndex", 0);
    }

    public void OnDisable()
    {
        Save();
    }
    public override void OnInspectorGUI()
    {

        EditorGUI.BeginChangeCheck();


        GUILayout.Label("Settings", EditorStyles.boldLabel);



        GUILayout.Label("Sound", EditorStyles.boldLabel);
        if (data.Sounds == null || data.Sounds.Count == 0)
        {
            GUILayout.Label("No sounds added, create a new sound!");
        }
        var sound = data.Sounds[currentSoundIndex];

      
        if (EditorGUI.EndChangeCheck())
        {
            Save();
        }

        base.OnInspectorGUI();
    }


    void Save()
    {
        EditorPrefs.SetInt("soundIndex", currentSoundIndex);
        if (target != null)
        {
            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssetIfDirty(target);
        }
    }
}

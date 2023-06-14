using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerSaveLoad : GenericSingleton<ControllerSaveLoad>
{
    public const string SAVE_STRING = "echo_save_file";
    public SaveData _saveData;
    public static SaveData GetSaveData => Instance._saveData;
    protected override void Awake()
    {
        base.Awake();
       var saveString = PlayerPrefs.GetString(SAVE_STRING);
        if (!string.IsNullOrEmpty(saveString))
        {

            _saveData = SaveData.GetSaveData(saveString);
        }
        else
        {
            _saveData = new SaveData();
        }
    }

    public static void ClearSave()
    {
        Instance._saveData = new SaveData();
        Save();
    }

    public static void Save()
    {
        PlayerPrefs.SetString(SAVE_STRING, Instance._saveData.GetJson());
//        Debug.Log(Instance._saveData.GetJson());
        PlayerPrefs.Save();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class SaveData
{
    public List<string> Keycards = new List<string>();
    public List<string> HealthPickups = new List<string>();
    public bool HasSpear = false;
    public Vector3 SavePosition = default;
    public int SaveRoom = 1;


    public string GetJson()
    {
       return JsonUtility.ToJson(this, true);
    }

    public static SaveData GetSaveData(string json)
    {
        return JsonUtility.FromJson<SaveData>(json);

    }
}

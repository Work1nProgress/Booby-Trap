using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ContinueGameButtonToggle : MonoBehaviour
{

    [SerializeField]
    TMP_Text text;
    private void Start()
    {
        text.SetText(ControllerSaveLoad.GetSaveData.Patricide ? "Continue" : "Start Game");
       
    }
}

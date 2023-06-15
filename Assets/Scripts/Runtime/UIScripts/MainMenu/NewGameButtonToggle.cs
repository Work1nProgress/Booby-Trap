using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NewGameButtonToggle : MonoBehaviour
{

    private void Start()
    {
        if (!ControllerSaveLoad.GetSaveData.Patricide)
        {
            gameObject.SetActive(false);
        }
    }




}

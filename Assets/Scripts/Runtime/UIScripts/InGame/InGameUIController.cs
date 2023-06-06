using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameUIController : UIControllerSingleton<InGameUIController>
{
    private void Start()
    {
        ControllerInput.Instance.Pause.AddListener(() => { ToggleMenu("Pause"); });
    }
}

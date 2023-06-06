using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUIController : UIControllerSingleton<MainMenuUIController>
{
    private void Start()
    {
        
    }

    protected override void OnActivate()
    {
        base.OnActivate();

        OpenMenu("Primary");
    }
}

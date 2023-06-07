using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MM_OptionsMenuHandler : GenericMenuHandler
{
    public void OpenMainMenu()
    {
        MainMenuUIController.OpenMenu("Primary");
    }
}

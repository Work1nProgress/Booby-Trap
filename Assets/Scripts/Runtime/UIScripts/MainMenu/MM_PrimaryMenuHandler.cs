using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MM_PrimaryMenuHandler : GenericMenuHandler
{

    public void StartGame()
    {
        Debug.Log("Pull the lever, Kronk!");
    }

    public void OtherThing()
    {
        Debug.Log("Oh shit! Double rainbow!!! (not really)");
    }

    public void OpenOptionsMenu()
    {
        MainMenuUIController.OpenMenu("Options");
    }

    public void OpenMainMenu()
    {
        MainMenuUIController.OpenMenu("Primary");
    }
}

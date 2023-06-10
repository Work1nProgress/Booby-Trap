using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MM_PrimaryMenuHandler : GenericMenuHandler
{

    public void StartGame()
    {
        ControllerGameFlow.Instance.LoadNewScene("BossScene");
        MainMenuUIController.CloseMenu();
    }

    public void OtherThing()
    {
        Debug.Log("Oh shit! Double rainbow!!! (not really)");
    }

    public void OpenOptionsMenu()
    {
        MainMenuUIController.OpenMenu("Options");
    }

}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MM_PrimaryMenuHandler : GenericMenuHandler
{

    public void StartGame()
    {
        MainMenuUIController.SetActiveUIDomain(UIDomain.INGAME);
        ControllerGameFlow.Instance.LoadNewScene("GardenofDestiny");
        MainMenuUIController.CloseMenu();
    }

    public void NewGame()
    {
        ControllerSaveLoad.ClearSave();
        MainMenuUIController.SetActiveUIDomain(UIDomain.INGAME);
        ControllerGameFlow.Instance.LoadNewScene("GardenofDestiny");
        MainMenuUIController.CloseMenu();
    }

    public void OtherThing()
    {
        
    }

    public void OpenOptionsMenu()
    {
        MainMenuUIController.OpenMenu("Options");
    }

}

using UnityEngine;

public class ControllerLoadingScene : MonoBehaviour
{


    //string mainMenuScene = "MainMenuScene";
    string mainMenuScene = "RoomTestScene";

    void Start()
    {

        //TODO load save file

        ControllerGameFlow.Instance.LoadNewScene(mainMenuScene);
    }
}

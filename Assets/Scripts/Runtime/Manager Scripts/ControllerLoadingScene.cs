using UnityEngine;

public class ControllerLoadingScene : MonoBehaviour
{


    string mainMenuScene = "MainMenuScene";
    //string mainMenuScene = "RoomTestScene";

    void Start()
    {

        //TODO load save file
        MusicPlayer.Instance.PlayPlaylist("spearmaidenPlaylist");
        ControllerGameFlow.Instance.LoadNewScene(mainMenuScene);
    }
}

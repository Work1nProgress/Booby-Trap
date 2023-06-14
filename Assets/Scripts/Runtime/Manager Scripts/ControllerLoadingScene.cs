using UnityEngine;
using UnityEngine.SceneManagement;

public class ControllerLoadingScene : MonoBehaviour
{


    //string mainMenuScene = "MainMenuScene";
    //string mainMenuScene = "RoomTestScene";

    [SerializeField]
    SoundManager SoundManager;

    void Start()
    {

        Instantiate(SoundManager, null);

        //TODO load save file
     //   MusicPlayer.Instance.PlayPlaylist("spearmaidenPlaylist");
        ControllerGameFlow.Instance.LoadNewScene("GardenofDestiny");
    }
}

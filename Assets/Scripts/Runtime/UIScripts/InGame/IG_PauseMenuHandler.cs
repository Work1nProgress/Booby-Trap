using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class IG_PauseMenuHandler : GenericMenuHandler
{
    [SerializeField] AudioMixer _audioMixer;

    public override void OnOpen()
    {
        base.OnOpen();
        Time.timeScale = 0;
    }

    public override void OnClose()
    {
        base.OnClose();
        Time.timeScale = 1;
    }

    public void OnResume()
    {
        InGameUIController.ToggleMenu("Pause");
    }

    public void OnOpenOptionsMenu()
    {
        InGameUIController.ToggleMenu("Options");
    }

    public void OnQuit()
    {
        Application.Quit();
    }
}

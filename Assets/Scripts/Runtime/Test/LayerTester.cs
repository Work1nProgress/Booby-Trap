using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LayerTester : MonoBehaviour
{

    [SerializeField]
    List<Button> buttons;

    [SerializeField]
    List<int> layers = new List<int> { 0, 1, 2, 3 };

    public void PlaylistAuto()
    {
        MusicPlayer.Instance.PlayPlaylist("testPlaylistAuto", 1);
    }

    public void SongNormal()
    {
        MusicPlayer.Instance.PlayMusic("Test Song", layers:new int[]{ 1,2,3});
    }

    public void SongLoop()
    {
        MusicPlayer.Instance.PlayMusic("Test Song",3, true, 5, 1,2,3);
    }

  

    public void PlaylistManual()
    {
        List<int> startLayers = new List<int>();
        foreach (var b in buttons)
        {
            if (b.targetGraphic.color == Color.green)
            {
                startLayers.Add(buttons.IndexOf(b));
            }
        }

        if (startLayers.Count > 0)
        {
            MusicPlayer.Instance.PlayPlaylist("testPlaylistManual", layers:startLayers.ToArray());
        }
        else
        {
            MusicPlayer.Instance.PlayPlaylist("testPlaylistManual");

        }
        int count = 0;
        Button onlyOne = null;
        foreach (var b in buttons)
        {
            if (b.targetGraphic.color == Color.green)
            {
                count++;
                onlyOne = b;
            }
            if (count > 1)
            {
                break;
            }

        }
        if (count == 1)
        {
            onlyOne.interactable = false;
        }
        else
        {
            foreach (var b in buttons)
            {
                b.interactable = true;
            }
        }

    }


    public void OnLayerToggle(Button button)
    {
        var index = buttons.IndexOf(button);



     
        if (button.targetGraphic.color == Color.green)
        {

            MusicPlayer.Instance.RemoveLayers("testPlaylistManual", index);
            button.targetGraphic.color = Color.red;
        }
        else if (button.targetGraphic.color == Color.red)
        {
            MusicPlayer.Instance.AddLayers("testPlaylistManual", index);
            button.targetGraphic.color = Color.green;

        }
      

        int count = 0;
        Button onlyOne = null;
        foreach (var b in buttons)
        {
            if (b.targetGraphic.color == Color.green)
            {
                count++;
                onlyOne = b;
            }
            if (count > 1)
            {
                break;
            }

        }
        if (count == 1)
        {
            onlyOne.interactable = false;
        }
        else
        {
            foreach (var b in buttons)
            {
                b.interactable = true;
            }
        }
    }

    


    public void Stop()
    {
        MusicPlayer.Instance.StopPlaying();
      
    }
}

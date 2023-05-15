using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    //public UnityEvent onEnter;
    public int sceneBuildIndex;

    public bool useSceneName;
    public string sceneName;

    public void EnterPortal()
    {
        if(useSceneName)
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        else
            SceneManager.LoadScene(sceneBuildIndex, LoadSceneMode.Single);
    }
}

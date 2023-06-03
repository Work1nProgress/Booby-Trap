using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControllerGameFlow : GenericSingleton<ControllerGameFlow>
{
    private string _currentScene;

    public delegate void OnSceneLoadedSignature();
    public event OnSceneLoadedSignature OnSceneLoaded;

    protected override void Awake()
    {
        base.Awake();
        _currentScene = SceneManager.GetActiveScene().name;
    }

    public void LoadNewScene(string sceneName)
    {
        SceneManager.sceneLoaded += OnAfterSceneLoaded;
        SceneManager.LoadScene(sceneName);
    }

    public void LoadNewScene(int sceneIndex)
    {
        SceneManager.sceneLoaded += OnAfterSceneLoaded;
        SceneManager.LoadScene(sceneIndex);
    }

    public void ResetCurrentScene()
    {
        LoadNewScene(_currentScene);
    }

    void OnAfterSceneLoaded(Scene s, LoadSceneMode loadSceneMode)
    {
        SceneManager.sceneLoaded -= OnAfterSceneLoaded;
        _currentScene = SceneManager.GetActiveScene().name;
        var controllerLocal = FindFirstObjectByType<ControllerLocal>();
        if (controllerLocal != null)
        {
            controllerLocal.Init();
        }

        OnSceneLoaded?.Invoke();
    }
}

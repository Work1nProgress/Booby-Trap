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


    private void Start()
    {
        int a = 0;

        a = Utils.AddFlags(a, 1, 2, 3, 4, 5);
        Debug.Log(string.Join("_", Utils.FlagsToList(a)));

        int b = 2;
        Debug.Log(string.Join("_", Utils.FlagsToList(b)));
        a = Utils.RemoveFlags(a, b);
        Debug.Log(string.Join("_", Utils.FlagsToList(a)));

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

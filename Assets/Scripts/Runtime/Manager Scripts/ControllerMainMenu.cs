using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerMainMenu : ControllerLocal
{

    [SerializeField]
    bool AutoLoad;

    int mainScene = 2;

    public void OnLoadMainScene()
    {
        ControllerGameFlow.Instance.LoadNewScene(mainScene);
    }

    public override void Init()
    {
        base.Init();
        if (AutoLoad)
        {
            OnLoadMainScene();
        }
    }
}

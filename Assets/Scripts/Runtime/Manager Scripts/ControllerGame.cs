using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerGame : ControllerLocal
{

    static ControllerGame m_Instance;
    public static ControllerGame Instance => m_Instance;

    // Use this method to initialize everyhing you need at the begging of the scene
    public override void Init()
    {
        base.Init();
        m_Instance = this;
        PoolManager.Instance.Init();

        
    }
}




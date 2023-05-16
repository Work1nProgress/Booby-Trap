using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityController : StateHandler
{

}

[Serializable]
public abstract class EntityBehavior
{
    [SerializeField] private ActionType _type;
    public ActionType Type => _type;

    public abstract void Execute();

    public enum ActionType
    {
        ENTER,
        EXIT,
        UPDATE,
        FIXED
    }
}

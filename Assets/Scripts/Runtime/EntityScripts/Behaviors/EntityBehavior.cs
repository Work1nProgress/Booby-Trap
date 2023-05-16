using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityBehavior : ScriptableObject
{
    public virtual void Execute(EntityController controller) { }
    public virtual void Execute(EntityController controller, float deltaTime) { }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityBehavior : ScriptableObject
{
    public virtual void Execute(EntityState state, EntityController controller) { }
    public virtual void Execute(EntityState state, EntityController controller, float deltaTime) { }
}

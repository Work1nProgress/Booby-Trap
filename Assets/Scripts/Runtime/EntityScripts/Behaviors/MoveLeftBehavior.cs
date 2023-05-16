using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Move Left", menuName = "Entities/Behavior/Move Left")]
public class MoveLeftBehavior : EntityBehavior
{
    public override void Execute(EntityController controller, float deltaTime)
    {
        controller.Rigidbody.AddForce(
            Vector2.left * controller.MovementSpeed,
            ForceMode2D.Impulse);
    }
}

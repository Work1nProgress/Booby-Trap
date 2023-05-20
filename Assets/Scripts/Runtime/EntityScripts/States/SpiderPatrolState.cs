using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpiderPatrolState", menuName = "Entities/Spider Patrol")]
public class SpiderPatrolState : EntityState
{
    private bool _movingRight = false;
    public override void EnterState()
    {
        base.EnterState();

        _movingRight = false;
    }

    public override void ExitState()
    {
        _controller.Jump(20);

        base.ExitState();
    }

    public override void FixedUpdateState(float deltaTime)
    {
        base.FixedUpdateState(deltaTime);

        _controller.Rigidbody.MovePosition(
            _movingRight ?
            _controller.Rigidbody.position + Vector2.right * _controller.MovementSpeed * deltaTime
            : _controller.Rigidbody.position - Vector2.right * _controller.MovementSpeed * deltaTime);
    }
}

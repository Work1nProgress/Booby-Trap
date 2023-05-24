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
        base.ExitState();
    }

    public override void FixedUpdateState(float deltaTime)
    {
        base.FixedUpdateState(deltaTime);

        RaycastHit2D cliffHit = Physics2D.Raycast(_movingRight ?
            _controller.Rigidbody.position + new Vector2(0.55f, 0) :
            _controller.Rigidbody.position - new Vector2(0.55f, 0),
            Vector2.down,
            0.55f,
            LayerMask.GetMask("Ground"));
        RaycastHit2D wallHit = Physics2D.Raycast(_movingRight ?
            _controller.Rigidbody.position + new Vector2(0.55f, 0) :
            _controller.Rigidbody.position - new Vector2(0.55f, 0),
            _movingRight ?
            Vector2.right:
            Vector2.right * -1,
            0.1f,
            LayerMask.GetMask("Ground"));

        if (wallHit.collider != null)
            if (_movingRight) _controller.RotateCounterClockwise();
            else _controller.RotateClockwise();

        _controller.Move(_controller.MovementSpeed, _movingRight);
    }

    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CyclopsPatrolState", menuName = "Entities/Cyclops Patrol")]
public class CyclopsPatrolState : EntityState
{
    private bool _movingRight = false;

    public override void EnterState()
    {
        base.EnterState();
    }

    public override void FixedUpdateState(float deltaTime)
    {
        base.FixedUpdateState(deltaTime);
        if (DetectElevationChange()) _movingRight = !_movingRight;
        _controller.Move(_controller.MovementSpeed, _movingRight);

        Transform playerCheck = CheckForPlayer();
        if (playerCheck != null)
        {
            _controller.PlayerDetected(playerCheck);
            ToNextState();
        }
            
    }

    private bool DetectElevationChange()
    {
        RaycastHit2D cliffHit = Physics2D.Raycast(_movingRight ?
            _controller.Rigidbody.position + new Vector2(0.56f, 0) :
            _controller.Rigidbody.position - new Vector2(0.56f, 0),
            Vector3.down,
            0.55f,
            LayerMask.GetMask("Ground"));

        RaycastHit2D wallHit = Physics2D.Raycast(_controller.Rigidbody.position,
            _movingRight ? Vector3.right : Vector3.left,
            0.55f,
            LayerMask.GetMask("Ground"));

        return !(cliffHit.collider != null && wallHit.collider == null);
    }

    private Transform CheckForPlayer()
    {
        RaycastHit2D playerHit = Physics2D.Raycast(_controller.Rigidbody.position,
            _movingRight ? Vector3.right : Vector3.left,
            12,
            LayerMask.GetMask("Player"));

        if (playerHit.collider != null)
            return playerHit.collider.transform;

        return null;
    }
}
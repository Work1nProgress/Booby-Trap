using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PatrolState", menuName = "Entities/States/Patrol State")]
public class PatrolState : EntityState
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

        _controller.Sprite.flipX = _movingRight;

        bool playerCheck = CheckForPlayer();

         _controller.PlayerDetected(playerCheck);
        if (playerCheck)
        {
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

    private bool CheckForPlayer()
    {
        var distance = Vector3.Distance(_controller.Rigidbody.position, ControllerGame.Instance.player.transform.position);

        if (distance <= _controller.Stats.DetectionDistance)
        {
            if (Physics2D.Raycast(_controller.Rigidbody.position,

           ControllerGame.Instance.player.RigidBody.position - _controller.Rigidbody.position,
           distance,
           LayerMask.GetMask("Ground"))){
                return false;
            }

            if (Vector2.Angle(_movingRight ? Vector3.right : Vector3.left,
                ControllerGame.Instance.player.RigidBody.position - _controller.Rigidbody.position) > _controller.Stats.DetectionAngle)
            {
                return false;
            }


            return true;

        }
        return false;

      
    }
}
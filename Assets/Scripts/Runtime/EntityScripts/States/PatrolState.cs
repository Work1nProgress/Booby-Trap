using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PatrolState", menuName = "Entities/States/Patrol State")]
public class PatrolState : EntityState
{
    private bool _movingRight = false;

    [SerializeField]
    bool IsIdle;

    public override void EnterState()
    {
        base.EnterState();
        if (!_controller.isAggressive)
        {
            SoundManager.Instance.PlayLooped(_controller.Sound.PassiveLoop, _controller.gameObject, _controller.transform);
        }
        if (IsIdle)
        {
            _movingRight = !_movingRight;
        }
    }

    public override void ExitState()
    {
        SoundManager.Instance?.CancelLoop(_controller.Sound.PassiveLoop, _controller.gameObject);
        base.ExitState();
    }

    public override void FixedUpdateState(float deltaTime)
    {
        base.FixedUpdateState(deltaTime);
        if (DetectElevationChange()) _movingRight = !_movingRight;
        _controller.Move(_controller.MovementSpeed, _movingRight);

        _controller.Sprite.flipX = _movingRight;

        bool playerCheck = CheckForPlayer(_movingRight) && !IsIdle;

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
            Utils.GroundLayer);

        RaycastHit2D wallHit = Physics2D.Raycast(_controller.Rigidbody.position,
            _movingRight ? Vector3.right : Vector3.left,
            0.55f,
            Utils.GroundLayer);

        return !(cliffHit.collider != null && wallHit.collider == null);
    }

  
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ChaseState", menuName = "Entities/ChaseState")]
public class ChaseState : EntityState
{
    private bool _movingRight = false;

    [SerializeField]
    float OffsetX;

    Vector2 RaycastVector;


    public override void EnterState()
    {

        ControllerGame.Instance.AddAgressiveEnemy(_controller);
        base.EnterState();
        RaycastVector = new Vector2(OffsetX, 0);
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FixedUpdateState(float deltaTime)
    {
        base.FixedUpdateState(deltaTime);

        if (_controller.HasTarget)
        {
            _movingRight = _controller.transform.position.x < ControllerGame.Instance.player.RigidBody.position.x;
            _controller.Sprite.flipX = _movingRight;

            if (!DetectWall()) _controller.Move(_controller.Stats.MovementSpeedChase, _movingRight);

            if (CloseToPlayer()) ChangeState(nextState);
        }
        else ChangeState(altState);
    }

    private bool DetectWall()
    {
        RaycastHit2D wallHit = Physics2D.Raycast(_movingRight ?
            _controller.Rigidbody.position + RaycastVector :
            _controller.Rigidbody.position - RaycastVector,
            _movingRight ?
            Vector2.right :
            Vector2.right * -1,
            0.1f,
            Utils.GroundLayerMask);

        return wallHit.collider != null;
    }

    private bool CloseToPlayer()
    {
        RaycastHit2D playerHit = Physics2D.Raycast(_movingRight ?
            _controller.Rigidbody.position + RaycastVector :
            _controller.Rigidbody.position - RaycastVector,
            _movingRight ?
            Vector2.right :
            Vector2.right * -1,
            0.3f,
            Utils.GroundLayerMask);

        return playerHit.collider != null;
    }
}

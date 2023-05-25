using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CyclopsChaseState", menuName = "Entities/Cyclops Chase")]
public class CyclopsChaseState : EntityState
{
    private bool _movingRight = false;

    public override void FixedUpdateState(float deltaTime)
    {
        base.FixedUpdateState(deltaTime);

        if (_controller.Target != null)
        {
            _movingRight = _controller.transform.position.x < _controller.Target.position.x ?
                true : false;

            if (!DetectWall()) _controller.Move(_controller.MovementSpeed * 1.1f, _movingRight);

            if (CloseToPlayer()) ChangeState(nextState);
        }
        else ChangeState(altState);
    }

    private bool DetectWall()
    {
        RaycastHit2D wallHit = Physics2D.Raycast(_movingRight ?
            _controller.Rigidbody.position + new Vector2(0.55f, 0) :
            _controller.Rigidbody.position - new Vector2(0.55f, 0),
            _movingRight ?
            Vector2.right :
            Vector2.right * -1,
            0.1f,
            LayerMask.GetMask("Ground"));

        return wallHit.collider != null;
    }

    private bool CloseToPlayer()
    {
        RaycastHit2D playerHit = Physics2D.Raycast(_movingRight ?
            _controller.Rigidbody.position + new Vector2(0.55f, 0) :
            _controller.Rigidbody.position - new Vector2(0.55f, 0),
            _movingRight ?
            Vector2.right :
            Vector2.right * -1,
            0.3f,
            LayerMask.GetMask("Player"));

        return playerHit.collider != null;
    }
}

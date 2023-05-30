using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "StunnedState", menuName = "Entities/States/Stunned State")]
public class StunnedState : EntityState
{



    float StunTime;

    bool colliderState;
    bool wasApplyingGravity;
    [SerializeField]
    bool RotateDirection;

    [SerializeField]
    bool ApplyGravity = true;

    public void SetTime(float time)
    {
        StunTime = time;
    }

    public override void EnterState()
    {

        if (RotateDirection)
        {
            _controller.Rigidbody.isKinematic = false;
        }
        wasApplyingGravity = _controller.DoApplyGravity;
        _controller.DoApplyGravity = ApplyGravity;
        colliderState = _controller.Collider.isTrigger;
        _controller.Collider.isTrigger = false; 
        base.EnterState();

    }

    public override void ExitState()
    {
        if (RotateDirection)
        {
            _controller.Rigidbody.isKinematic = true;
        }
        _controller.DoApplyGravity = wasApplyingGravity;
        _controller.Collider.isTrigger = colliderState;
        base.ExitState();
    }

    public override void UpdateState(float deltaTime)
    {
        base.UpdateState(deltaTime);
        StunTime -= deltaTime;
        if (StunTime <= 0)
        {
            ToNextState();
        }
    }

    public override void FixedUpdateState(float deltaTime)
    {
        base.FixedUpdateState(deltaTime);
        if (RotateDirection)
        {
            _controller.Rigidbody.MoveRotation(0);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "SlashAttack", menuName = "Entities/Daddy/Slash Attack")]
public class DaddySlashPhase : DaddyAttackPhase
{
    [SerializeField]
    int SlashDistance;

    [SerializeField]
    Vector2 SlashSize;

    [SerializeField]
    Vector2 SlashPosition;


    Vector2 _SlashEndPosition;

    Vector2 startPos;

    public override void BeginPhase()
    {
      
        base.BeginPhase();
    }

    protected override void StartTelegraph()
    {
        base.StartTelegraph();
        //TODO face Echo instead of towards center
        _controller.FaceTowards(_controller.GetTilePosition() < _controller.GetRoomPosition.x + _controller.GetRoomSize.x / 2 ? 1 : -1);
        startPos = _controller.Rigidbody.position;
        _SlashEndPosition = startPos + new Vector2(SlashDistance * _controller.facingDirection, 0);
    }



    protected override void StartActive()
    {
        base.StartActive();
        var slash = PoolManager.Spawn<PoolObjectTimed>("SlashDaddy", _controller.transform, new Vector3(SlashPosition.x * _controller.facingDirection, SlashPosition.y, 0));
        slash.StartTicking(m_ActiveTime);
    }


    public override void UpdatePhase(float deltaTime)
    {


        base.UpdatePhase(deltaTime);

        if (_State == DaddyPhaseState.Active)
        {
            _controller.Rigidbody.MovePosition(Vector2.Lerp(startPos, _SlashEndPosition, _currentTime / m_ActiveTime));
        }

    }
}

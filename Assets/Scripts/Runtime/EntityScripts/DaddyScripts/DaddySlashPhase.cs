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
        _controller.FaceTowardsEcho();
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
            var target = Vector2.Lerp(startPos, _SlashEndPosition, _currentTime / m_ActiveTime);
            //dont let dada run out of the room
            var clampedTarget = new Vector2(
                Mathf.Clamp(target.x,
                _controller.GetRoomPosition.x+0.5f,
                _controller.GetRoomPosition.x + _controller.GetRoomSize.x-0.5f)
                , target.y);
            _controller.Rigidbody.MovePosition(clampedTarget);

            var hit = Physics2D.OverlapBox(_controller.Rigidbody.position+SlashPosition, SlashSize, 0, Utils.PlayerLayer);
//            Debug.Log(hit);
            if (hit)
            {
                ControllerGame.Instance.player.Damage(DamageToPlayer);
            }
        }
    }

    public override void DrawHitboxes()
    {
        base.DrawHitboxes();
        if (_State == DaddyPhaseState.Active) {
            Gizmos.DrawWireCube(_controller.Rigidbody.position + SlashPosition*_controller.facingDirection, SlashSize);
        }
    }
}

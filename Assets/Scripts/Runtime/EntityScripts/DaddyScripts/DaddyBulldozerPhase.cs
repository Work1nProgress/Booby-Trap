using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BulldozerPhase", menuName = "Entities/Daddy/Bulldozer Attack")]
public class DaddyBulldozerPhase : DaddyAttackPhase
{


    [SerializeField]
    Vector2 BulldozeSize;

    [SerializeField]
    Vector2 BulldozePosition;


    Vector2 _BulldozeEndPosition;

    Vector2 startPos;

    protected override void StartTelegraph()
    {
        base.StartTelegraph();
     
    }


    public override void UpdatePhase(float deltaTime)
    {

        base.UpdatePhase(deltaTime);

        if (_State == DaddyPhaseState.Active)
        {
            _controller.Rigidbody.MovePosition(Vector2.Lerp(startPos, _BulldozeEndPosition, _currentTime / m_ActiveTime));
            var hit = Physics2D.OverlapBox(_controller.Rigidbody.position + BulldozePosition, BulldozeSize, 0, Utils.PlayerLayer);
            if (hit)
            {
                ControllerGame.Instance.player.Damage(DamageToPlayer);
            }
        }

    }

    protected override void OnTeleport()
    {
        _controller.FaceTowards(TeleportPosition.x < _controller.GetRoomPosition.x + _controller.GetRoomSize.x / 2 ? 1 : -1);
        var slash = PoolManager.Spawn<PoolObjectTimed>("Bulldoze", _controller.transform, new Vector3(BulldozePosition.x * _controller.facingDirection, BulldozePosition.y, 0));
        slash.transform.localScale = BulldozeSize;
        slash.StartTicking(m_TelegraphTime - _currentTime + m_ActiveTime);
        base.OnTeleport();
    }

    protected override void OnEndTelegraph()
    {
        startPos = _controller.Rigidbody.position;

   
        _BulldozeEndPosition = startPos + new Vector2((_controller.GetRoomSize.x - 2) * _controller.facingDirection, 0);

      
        base.OnEndTelegraph();
    }

    public override void DrawHitboxes()
    {
        base.DrawHitboxes();
        if (_State == DaddyPhaseState.Active)
        {
            Gizmos.DrawWireCube(_controller.Rigidbody.position + BulldozePosition * _controller.facingDirection, BulldozeSize);
        }
    }
}

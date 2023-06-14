using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


[CreateAssetMenu(fileName = "SlashAttack", menuName = "Entities/Daddy/Slash Attack")]
public class DaddySlashAttack : DaddyAttack
{
    [SerializeField]
    int SlashDistance;

    [SerializeField]
    Vector2 SlashSize;

    [SerializeField]
    Vector2 SlashPosition;


    Vector2 _SlashEndPosition;

    Vector2 startPos;

    [SerializeField]
    Ease MovementEase = Ease.Linear;

 

    protected override void StartTelegraph()
    {
        base.StartTelegraph();
        _controller.FaceTowardsEcho();
        startPos = _controller.Rigidbody.position;
        _SlashEndPosition = startPos + new Vector2(SlashDistance * _controller.facingDirection, 0);
        SoundManager.Instance.Play(_controller.Sound.SlashCharge, _controller.transform);
    }



    protected override void StartActive()
    {
        base.StartActive();
        SoundManager.Instance.Play(_controller.Sound.SlashAttack, _controller.transform);
    }


    public override void UpdateAttack(float deltaTime)
    {


        base.UpdateAttack(deltaTime);

        if (_State == DaddyAttackState.Active)
        {
            var target = Vector2.Lerp(startPos, _SlashEndPosition, DOVirtual.EasedValue(0, 1, _currentTime / m_ActiveTime, MovementEase));
            //dont let dada run out of the room
            var clampedTarget = new Vector2(
                Mathf.Clamp(target.x,
                _controller.GetRoomPosition.x+0.5f,
                _controller.GetRoomPosition.x + _controller.GetRoomSize.x-0.5f)
                , target.y);
            _controller.Rigidbody.MovePosition(clampedTarget);

            var hit = Physics2D.OverlapBox(_controller.Rigidbody.position+SlashPosition, SlashSize, 0, Utils.PlayerLayerMask);
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
        if (_State == DaddyAttackState.Active) {
            Gizmos.DrawWireCube(_controller.Rigidbody.position + SlashPosition*_controller.facingDirection, SlashSize);
        }
    }
}

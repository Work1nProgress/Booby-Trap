using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CyclopsAttackState", menuName = "Entities/Cyclops Attack")]
public class CyclopsAttackState : EntityState
{
    [SerializeField] float _windupTime = 1.2f;
    [SerializeField] float _coolOffTime = 0.5f;

    [SerializeField] int _attackPower = 2;

    CountdownTimer attackWindup;
    CountdownTimer attackCoolOff;

    bool _targetOnRight;

    public override void EnterState()
    {
        base.EnterState();

        _targetOnRight = _controller.Rigidbody.position.x < ControllerGame.Instance.player.RigidBody.position.x;

        attackWindup = new CountdownTimer(_windupTime, false, false, () => { Attack(_attackPower); attackCoolOff.Resume(); });
        attackCoolOff = new CountdownTimer(_coolOffTime, true, false, () => { ToNextState(); });
    }

    public override void ClearEvents()
    {
        attackWindup.Dispose();
        attackCoolOff.Dispose();
        base.ClearEvents();
    }

    public void Attack(int power)
    {

        SoundManager.Instance.Play(_controller.Sound.Attack, _controller.transform);
        Vector2 p2 = _controller.Rigidbody.position;
        p2.y -= 0.55f;
        p2.x += _targetOnRight ? 0.85f : -0.85f;

        Collider2D playerCollider = Physics2D.OverlapArea(_controller.Rigidbody.position,
            p2, Utils.PlayerLayerMask);

        if (playerCollider)
        {
            ControllerGame.Instance.player.Damage(power);
        }

       
    }
}

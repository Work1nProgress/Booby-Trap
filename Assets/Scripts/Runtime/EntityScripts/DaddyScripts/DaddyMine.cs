using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DaddyMine : EntityBase
{

    MineData _data;

    [SerializeField]
    string Explode, Destroyed;

    public void Init(MineData data)
    {
        _data = data;

    }

    

    private void FixedUpdate()
    {
        if (Physics2D.OverlapBox(_data.MinePosition, _data.MineHitbox,0, Utils.PlayerLayerMask)) {
            PoolManager.Spawn<PoolObjectTimed>("MineExplosion", null, transform.position);
            _data.MineCallback.Invoke(_data.MinePosition);
            SoundManager.Instance.Play(Explode, transform);
            PoolManager.Despawn(this);
            ControllerGame.Instance.player.Damage(_data.Damage);
        }
    }

    protected override void OnKill()
    {
        SoundManager.Instance.Play(Destroyed, transform);
        _data.MineCallback.Invoke(_data.MinePosition);
        PoolManager.Despawn(this);
    }
}


public class MineData {
    public float MineTelegraphDuration;
    public Vector2 MinePosition;
    public Vector2 MineHitbox;
    public UnityAction<Vector2> MineCallback;
    public int Damage;

}

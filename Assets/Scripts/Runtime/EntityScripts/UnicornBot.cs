using UnityEngine;

public class UnicornBot : EnemyBase
{
    public override void Init(EntityStats stats)
    {
        base.Init(stats);
        Rigidbody.gravityScale = 0;
    }

}
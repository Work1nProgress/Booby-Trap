using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyclopsBot : EnemyBase
{
    public override void Init(EntityStats stats)
    {
        base.Init(stats);
        Rigidbody.gravityScale = 0;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollapsingWall : PoolObjectTimed
{
    public override void Reuse()
    {
        base.Reuse();
        GetComponent<SpriteAnimator>().Reset();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class MineTelegraph : PoolObject
{

    [SerializeField]
    SpriteRenderer SpriteRenderer;

    MineData _data;

    public override void Reuse()
    {
        SpriteRenderer.DOFade(0, 0);
        base.Reuse();
    }
    public void Init(MineData data)
    {
        _data = data;
        SpriteRenderer.DOFade(1, _data.MineTelegraphDuration).OnComplete(() => SpawnMine());

    }

    void SpawnMine()
    {
        PoolManager.Spawn<DaddyMine>("DaddyMine", null, transform.position).Init(_data);
        PoolManager.Despawn(this);
    }
}

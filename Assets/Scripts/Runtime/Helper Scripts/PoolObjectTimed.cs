using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolObjectTimed : PoolObject
{

    [SerializeField]
    bool autoKill;
    [SerializeField]
    protected float duration;
    CountdownTimer timer;
    public override void Reuse()
    {
        if (autoKill)
        {
            StartTicking();
        }
    }

    public void StartTicking()
    {
        timer = new CountdownTimer(duration, false, false);
        timer.RegisterEvent(SelfDestruct);
        base.Reuse();
    }


    void SelfDestruct()
    {
        timer.UnregisterEvent(SelfDestruct);
        PoolManager.Despawn(this);
    }

    private void OnDestroy()
    {
        timer.Dispose();
    }
}

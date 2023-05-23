using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Spear : PoolObject
{

    protected float m_Lifetime;
    protected UnityAction m_OnDespawnedCallback;
    protected int m_Direction;
    public static Vector3 StuckOffset = new Vector3(0.23225f, 0, 0);

    [SerializeField]
    protected SpriteRenderer m_SpriteRenderer;


    public void Init(float lifeTime,int direction ,UnityAction OnDespawnedCallback)
    {
        m_Lifetime = lifeTime;
        m_OnDespawnedCallback = OnDespawnedCallback;
        m_Direction = direction;
        m_SpriteRenderer.flipY = direction > 0;
    }

    protected void Remove()
    {
       
        PoolManager.Despawn(this);
    }

    protected void RemoveAndNotify()
    {
        m_OnDespawnedCallback.Invoke();
        ControllerGame.Instance.RemoveSpear(this);
    }
}

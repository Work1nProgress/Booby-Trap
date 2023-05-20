using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FlyingSpear : Spear
{

    float m_Range;
    Vector3 m_StartPositon;


    [Header("Ground Checks")]
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.49f, 0.03f);
    private Vector3 groundCheckPoint = new Vector3(0.216f, 0, 0);

    [SerializeField]
    LayerMask groundLayer;

    [SerializeField]
    Rigidbody2D m_RigidBody;

    Vector3 stuckOffset = new Vector3(0.23225f, 0, 0);



    public void Init(float speed, float lifeTime, float range, float echoSpeed, int direction, float inheritSpeed, UnityAction OnDespawnedCallback)
    {
       
        m_RigidBody.velocity = new Vector2(speed* direction + echoSpeed * inheritSpeed, 0);
        m_Range = range;
        m_StartPositon = transform.position;
        Init(lifeTime, direction, OnDespawnedCallback);
    }


    bool CheckHitWall()
    {
        return Physics2D.OverlapBox(transform.position + m_Direction * groundCheckPoint, groundCheckSize, 0, groundLayer);
    }

    private void FixedUpdate()
    {
        if (m_Range > 0)
        {
            if (Vector3.Distance(transform.position, m_StartPositon) > m_Range)
            {
                RemoveAndNotify();
                return;
            }
        }
       
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {


        var spear = PoolManager.Spawn<StuckSpear>("StuckSpear", null, transform.position + m_Direction * stuckOffset, Quaternion.Euler(0, 0, 90));
        spear.Init(m_Lifetime,
           m_Direction,
            m_OnDespawnedCallback);

        var index = ControllerGame.Instance.GetSpearIndex(this);

        ControllerGame.Instance.Spears[index] = spear;
        Remove();
    }

   



    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;

        Gizmos.DrawWireCube(transform.position + m_Direction * groundCheckPoint, groundCheckSize);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StuckSpear : Spear
{


    [SerializeField]
    Collider2D Collider;

    bool down;
    bool jump;


    float m_TriggerDuration = 0.7f;
    float m_TriggerCountdown = 0;

    private void Update()
    {
        m_Lifetime -= Time.deltaTime;
        m_TriggerCountdown -= Time.deltaTime;

        if (m_Lifetime <= 0)
        {
            RemoveAndNotify(); 
        }
        if (m_TriggerCountdown > 0 && !Collider.isTrigger)
        {
            Collider.isTrigger = true;
        } else if(m_TriggerCountdown <= 0 && Collider.isTrigger)
        {
            Collider.isTrigger = false;
        }
        down = Input.GetAxisRaw("Vertical") < 0;
        jump = Input.GetButton("Jump");
    }

    public void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.gameObject.layer == 6 && down && jump)
        {
            m_TriggerCountdown = m_TriggerDuration;
        }
    }
}

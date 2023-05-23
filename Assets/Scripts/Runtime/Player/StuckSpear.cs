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
    bool playerLeft;

    private void OnEnable()
    {
        ControllerInput.Instance?.Vertical.AddListener(OnVertical);
        ControllerInput.Instance?.Jump.AddListener(OnJump);
    }

    private void OnDisable()
    {
        ControllerInput.Instance?.Vertical.RemoveListener(OnVertical);
        ControllerInput.Instance?.Jump.AddListener(OnJump);
    }

    private void Update()
    {
        m_Lifetime -= Time.deltaTime;

        if (m_Lifetime <= 0)
        {
            Collider.isTrigger = false;
            RemoveAndNotify(); 
        }
        
        if(Collider.isTrigger && playerLeft)
        {
            Collider.isTrigger = false;
        }
       
    }

    public void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.gameObject.layer == 6 && down && jump)
        {
            Collider.isTrigger = true;
            jump = false;
            playerLeft = false;
           
        }
       
      
    }

    public void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.layer == 6)
        {
            playerLeft = true;
        }
    }


    //public void OnTriggerExit2D(Collision2D collision)

    void OnVertical(float value)
    {
        down = value < 0;
    }

    void OnJump(bool value)
    {
        jump = value;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : EntityController
{
    [Header("EnemyBase")]

    [SerializeField]
    int DamageToDeal;

    [SerializeField]
    float Range;

    [SerializeField]
    float AttackForce;
     
    protected float StunTimer;

    protected float _stepsTimer;


   





    protected override void Update()
    {
        UpdateSteps();
        base.Update();

    }



    protected virtual void UpdateSteps()
    {
        //_stepsTimer -= Time.deltaTime * Mathf.Abs(Rigidbody.velocity.x) * stepVelocityFactor;
        //if (Mathf.Abs(Rigidbody.velocity.x) > 0.1f && _stepsTimer < 0)
        //{
        //    SoundManager.Instance.Play(SoundSteps, transform);
        //    _stepsTimer = stepDelay;
        //}
    }


    protected override void FixedUpdate()
    {
        if (Vector3.Distance(ControllerGame.Instance.player.transform.position, transform.position) < Range)
        {

            ControllerGame.Instance.player.AttackForce(transform.position, AttackForce);

            ControllerGame.Instance.player.Damage(DamageToDeal);
            
        }
        for (int i = ControllerGame.Instance.Spears.Count - 1; i >= 0; i--) 
        {
            if (ControllerGame.Instance.Spears[i] is StuckSpear)
            {
                if(Vector3.Distance(transform.position, ControllerGame.Instance.Spears[i].transform.position) < Range){
                    ControllerGame.Instance.RemoveSpear(ControllerGame.Instance.Spears[i]);
                }

            }
        }
        base.FixedUpdate();
    }

    public void KnockBackAndStun(Vector2 Force, float stunDuration)
    {
        StunTimer = stunDuration;
    }


    protected override void OnKill()
    {
       
        base.OnKill();
    }


    protected virtual void OnDrawGizmosSelected()
    {
#if UNITY_EDITOR
        UnityEditor.Handles.color = Color.green;

        UnityEditor.Handles.DrawWireDisc(transform.position, transform.forward, Range);

    #endif

    }


}

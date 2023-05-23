using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : EntityBase
{
    [Header("EnemyBase")]

    [SerializeField]
    int DamageToDeal;

    [SerializeField]
    float Range;

    [SerializeField]
    float AttackForce;








    protected virtual void FixedUpdate()
    {
       
        if (Vector3.Distance(ControllerGame.Instance.player.transform.position, transform.position) < Range)
        {

            ControllerGame.Instance.player.AttackForce(transform.position, AttackForce);

            ControllerGame.Instance.player.Damage(DamageToDeal);
            
        }
    }


    protected virtual void OnDrawGizmosSelected()
    {
#if UNITY_EDITOR
        UnityEditor.Handles.color = Color.green;

        UnityEditor.Handles.DrawWireDisc(transform.position, transform.up, Range);

    #endif

    }


}

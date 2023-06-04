using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemySound", menuName = "Entities/EnemySound")]
public class EnemySound : ScriptableObject
{
    public string PassiveLoop;

    public string AgressiveLoop;

    public string StepsPassive;
    public string StepsAggresive;

    public string Death;
    public string Hurt;

    public string NoticePlayer;

    public string Attack;


    public float stepDelay, stepVelocityFactor;
    public float AggresionEndDistance;
}

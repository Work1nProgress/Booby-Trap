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


    [SerializeField]
    StateData StunnedStateData;


    protected float StunTimer;

    protected float _stepsTimer;

    bool isStunned;

    



    public override void Init(EnemyStats Stats)
    {
        base.Init(Stats);
        AddState(InitializeState(StunnedStateData, this));
    }

    protected override void Update()
    {
        UpdateSteps();
        base.Update();

    }



    protected virtual void UpdateSteps()
    {
        if (string.IsNullOrEmpty(Sound.StepsPassive))
        {
            return;
        }
        _stepsTimer -= Time.deltaTime * Mathf.Abs(Rigidbody.velocity.x) * Sound.stepVelocityFactor;
        if (Mathf.Abs(Rigidbody.velocity.x) > 0.1f && _stepsTimer < 0)
        {
            SoundManager.Instance.Play(Sound.StepsPassive, transform);
            _stepsTimer = Sound.stepDelay;
        }
    }


    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (isStunned) return;

        if (Vector3.Distance(ControllerGame.Instance.player.transform.position, transform.position) < Range)
        {

            ControllerGame.Instance.player.AttackForce(transform.position, AttackForce);

            ControllerGame.Instance.player.Damage(DamageToDeal);
            if (CurrentState is FlyingAttackState)
            {
                CurrentState.ToAltState();
            }


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

    }

    public override void Damage(int ammount)
    {
        
        base.Damage(ammount);
        if (Health > 0)
        {
            SoundManager.Instance.Play(Sound.Hurt, transform);
        }
        else
        {
            SoundManager.Instance.CancelLoop(Sound.PassiveLoop, gameObject);
            SoundManager.Instance.CancelLoop(Sound.AgressiveLoop, gameObject);
            SoundManager.Instance.Play(Sound.Death, transform);
        }
        
    }

    public void KnockBackAndStun(Vector2 Force, float stunDuration)
    {
        StunTimer = stunDuration;
        ChangeState(StunnedStateData.stateName);
        Rigidbody.AddForce(Force, ForceMode2D.Impulse);
    }

    protected override void BeforeInitState(EntityState state)
    {
        if (state is StunnedState)
        {
            (state as StunnedState).SetTime(StunTimer);
            state.OnStateChanged.AddListener(CheckStun);
            isStunned = true;
        }
        base.BeforeInitState(state);
    }

    void CheckStun(bool entering, EntityState state)
    {

        if (!entering)
        {
            isStunned = false;
            state.OnStateChanged.RemoveListener(CheckStun);
        }
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

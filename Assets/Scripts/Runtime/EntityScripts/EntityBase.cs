using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EntityBase : PoolObject
{
    //temporary for prefab initialization
    [SerializeField] EntityStats _tempStats;

    protected int _maxHealth;
    protected int _health;
    public int Health => _health;
    public int MaxHealth => _maxHealth;

    public UnityEvent OnDeath;
    public UnityEvent<int> OnChangeHealth = new UnityEvent<int>();

   

    public virtual void Init(EntityStats stats)
    {
        OnDeath = new UnityEvent();
        _maxHealth = Mathf.Abs(stats.MaxHealth);
        _health = _maxHealth;
    }

    public virtual void Damage(int ammount) => ChangeHealth(Mathf.Abs(ammount) * -1);
    public virtual void Heal(int ammount) => ChangeHealth(Mathf.Abs(ammount));

    private void ChangeHealth(int ammount)
    {

      
        int newHealth = Mathf.Clamp(_health + ammount, 0, _maxHealth);
        _health = newHealth;
        if (ammount != 0)
        {
            OnChangeHealth.Invoke(ammount);
        }

        if (newHealth <= 0)
        {
            OnDeath.Invoke();
            OnKill();
        }
       
    }

    protected virtual void OnKill()
    {
        Destroy(this.gameObject);
    }
}

[Serializable]
public class EntityStats
{
    public int MaxHealth;
}

[Serializable]
public class EnemyStats : EntityStats
{
    public float DetectionAngle;
    public float DetectionDistance;
    public float MovementSpeed;
    public float MovementSpeedChase;
}
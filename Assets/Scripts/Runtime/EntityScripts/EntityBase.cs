using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EntityBase : MonoBehaviour
{
    //temporary for prefab initialization
    [SerializeField] EntityStats _tempStats;

    protected int _maxHealth;
    protected int _health;
    public int Health => _health;

    public UnityEvent OnDeath;

    private void Awake()
    {
        Init(_tempStats);
    }

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
        if (newHealth <= 0)
            OnDeath.Invoke();
    }
}

[Serializable]
public struct EntityStats
{
    public int MaxHealth;
}
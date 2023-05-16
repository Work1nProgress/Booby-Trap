using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New State", menuName = "Entities/State")]
public class EntityState : ScriptableObject
{
    private string _name;
    public string Name => _name;

    public delegate void ChangeStateSignature(string state);
    public event ChangeStateSignature OnChangeStateRequest;

    [SerializeField] private EntityBehavior[] _EnterStateBehaviors;
    [SerializeField] private EntityBehavior[] _UpdateStateBehaviors;
    [SerializeField] private EntityBehavior[] _FixedStateBehaviors;
    [SerializeField] private EntityBehavior[] _ExitStateBehaviors;

    EntityController _controller;

    public void Initialize(EntityController controller, string name)
    {
        _controller = controller;
        _name = name;
    }

    public virtual void EnterState()
    {
        foreach (EntityBehavior behavior in _EnterStateBehaviors)
            behavior.Execute(_controller);
    }
    public virtual void ExitState()
    {
        foreach (EntityBehavior behavior in _ExitStateBehaviors)
            behavior.Execute(_controller);
    }
    public virtual void UpdateState(float deltaTime)
    {
        foreach (EntityBehavior behavior in _UpdateStateBehaviors)
            behavior.Execute(_controller, Time.deltaTime);
    }
    public virtual void FixedUpdateState(float deltaTime)
    {
        foreach (EntityBehavior behavior in _FixedStateBehaviors)
            behavior.Execute(_controller, Time.fixedDeltaTime);
    }
    public virtual void ChangeState(string state)
    {
        if (OnChangeStateRequest != null)
            OnChangeStateRequest.Invoke(state);
    }

    public virtual void ClearEvents()
    {
        OnChangeStateRequest = null;
    }
}



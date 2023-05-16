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

    public virtual void EnterState() { }
    public virtual void ExitState() { }
    public virtual void UpdateState(float deltaTime) { }
    public virtual void FixedUpdateState(float deltaTime) { }
    public virtual void ChangeState(string state)
    {
        if (OnChangeStateRequest != null)
            OnChangeStateRequest.Invoke(state);
    }
}



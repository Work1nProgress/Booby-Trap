using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StateHandler : MonoBehaviour
{
    [SerializeField] private string _entryState;
    [SerializeField] StatePair[] _statePairs;

    private Dictionary<string, EntityState> _states;
    private EntityState _currentState;

    public void Initialize(EntityController controller)
    {
        _states = new Dictionary<string, EntityState>();
        
        foreach (StatePair pair in _statePairs)
        {
            EntityState state = Instantiate(pair.state);
            state.Initialize(controller, pair.name);
            AddState(state);
        }

        ChangeState(_entryState);
    }
    protected bool AddState(EntityState state) => _states.TryAdd(state.Name, state);
    protected bool RemoveState(string stateName) => _states.Remove(stateName);
    protected void ChangeState(string stateName)
    {
        EntityState newState = GetState(stateName);
        if (newState != null)
        {
            if (_currentState != null)
            {
                _currentState.ExitState();
                _currentState.OnChangeStateRequest -= ChangeState;
            }

            _currentState = newState;

            _currentState.OnChangeStateRequest += ChangeState;
            _currentState.EnterState();
        }
        else
            Debug.LogWarning($"{gameObject.name}: '{stateName}' is not a valid state.");
    }
    private EntityState GetState(string key)
    {
        EntityState state;
        if (!_states.TryGetValue(key, out state))
            state = null;
        return state;
    }


    protected virtual void Update()
    {
        if (_currentState != null)
            _currentState.UpdateState(Time.deltaTime);
    }

    protected virtual void FixedUpdate()
    {
        if (_currentState != null)
            _currentState.FixedUpdateState(Time.fixedDeltaTime);
    }
}

[Serializable]
public struct StatePair
{
    public string name;
    public EntityState state;
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class StateHandler : EntityBase
{
    [Header("State Handling")]
    [SerializeField] private string _initialState;
    [SerializeField] StateData[] _entityStates;

    private Dictionary<string, EntityState> _states;
    private EntityState _currentState;
    public EntityState CurrentState => _currentState;

    public void InitStateHandler(EntityController controller)
    {
        _states = new Dictionary<string, EntityState>();
        
        foreach (StateData data in _entityStates)
        {
            AddState(InitializeState(data, controller));
        }

        ChangeState(_initialState);
    }

    protected EntityState InitializeState(StateData data, EntityController controller)
    {
        EntityState state = Instantiate(data.state);
     
        state.InitState(controller, data);
        return state;
    }

    protected virtual void BeforeInitState(EntityState state) { } 
    protected bool AddState(EntityState state) => _states.TryAdd(state.stateName, state);
    protected bool RemoveState(string stateName) => _states.Remove(stateName);
    protected virtual void ChangeState(string stateName)
    {
        EntityState newState = GetState(stateName);
        if (newState != null)
        {
            if (_currentState != null)
            {
                CleanUpCurrentState();

            }

            BeforeInitState(newState);
            _currentState = newState;

            _currentState.OnChangeStateRequest += ChangeState;
            _currentState.EnterState();
        }
        else
            Debug.LogWarning($"{gameObject.name}: '{stateName}' is not a valid state.");
    }
    protected EntityState GetState(string key)
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

    private void OnDestroy()
    {
        if (_currentState != null)
        {
            CleanUpCurrentState();
        }
    }

    protected void CleanUpCurrentState()
    {
        _currentState.OnChangeStateRequest -= ChangeState;
        _currentState.ExitState();
        _currentState.ClearEvents();

    }
    
}

[Serializable]
public struct StateData
{
    public string stateName;
    public string nextState;
    public string altState;
    public EntityState state;
    public bool timedState;
    public bool timerAltState;
    public float stateTime;
}

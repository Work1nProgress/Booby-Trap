using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New State", menuName = "Entities/State")]
public class EntityState : ScriptableObject
{
    private string _name;
    public string stateName => _name;

    public delegate void ChangeStateSignature(string state);
    public event ChangeStateSignature OnChangeStateRequest;

    private string _nextState;
    public string nextState => _nextState;

    private string _altState;
    public string altState => _altState;

    protected EntityController _controller;

    private Dictionary<string, bool> _stateBools;
    public Dictionary<string, bool> StateBools => _stateBools;

    private bool _timedState = false;
    private float _stateTime;
    CountdownTimer _stateTimer;

    public void InitState(EntityController controller, StateData data)
    {
        _nextState = data.nextState;
        _altState = data.altState;
        _timedState = data.timedState;
        _stateTime = _timedState ? data.stateTime : 0;

        _stateTimer = _timedState ? 
            new CountdownTimer(
                _stateTime, false, false,
                data.timerAltState ?
                () => ToAltState()
                : () => ToNextState())
            : null;

        _controller = controller;
        _name = data.stateName;

        _stateBools = new Dictionary<string, bool>();
    }

    #region State Executors
    public virtual void EnterState()
    {
        if (_stateTimer != null)
            _stateTimer.Resume();
    }
    public virtual void ExitState()
    {

        if(_stateTimer != null)
        {
            _stateTimer.Reset();
            _stateTimer.Pause();
        }
    }
    public virtual void UpdateState(float deltaTime) { }
    public virtual void FixedUpdateState(float deltaTime) { }
    #endregion

    #region State Transitioners
    protected void ChangeState(string state)
    {
        if (OnChangeStateRequest != null && state != null)
            OnChangeStateRequest.Invoke(state);
    }
    public void ToNextState() => ChangeState(_nextState);
    public void ToAltState() => ChangeState(_altState);
    #endregion

    public virtual void ClearEvents()
    {
        OnChangeStateRequest = null;
    }
}



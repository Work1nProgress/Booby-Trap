using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

    public UnityEvent<bool, EntityState> OnStateChanged = new UnityEvent<bool, EntityState>();

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
        OnStateChanged.Invoke(true,this);
        if (_stateTimer != null)
            _stateTimer.Resume();
    }
    public virtual void ExitState()
    {
        OnStateChanged.Invoke(false, this);
        if (_stateTimer != null)
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


    protected bool CheckForPlayer(bool movingRight)
    {
        var distance = Vector3.Distance(_controller.Rigidbody.position, ControllerGame.Instance.player.transform.position);

        if (distance <= _controller.Stats.DetectionDistance)
        {
            if (Physics2D.Raycast(_controller.Rigidbody.position,

           ControllerGame.Instance.player.RigidBody.position - _controller.Rigidbody.position,
           distance,
           LayerMask.GetMask("Ground")))
            {
                return false;
            }

            if (Vector2.Angle(movingRight ? Vector3.right : Vector3.left,
                ControllerGame.Instance.player.RigidBody.position - _controller.Rigidbody.position) > _controller.Stats.DetectionAngle)
            {
                return false;
            }


            return true;

        }
        return false;


    }

    protected void MoveArial(Vector2 direction, float speed)
    {
        Vector2 force = direction * (speed * Time.deltaTime);
        _controller.Rigidbody.AddForce(force);
        _controller.transform.localScale = _controller.Rigidbody.velocity.x >= 0.01f ? new Vector3(-1f, 1f, 1f) : new Vector3(1f, 1f, 1f);
    }
}



using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    [SerializeField] Transform _pointA;
    [SerializeField] Transform _pointB;
    [SerializeField] Transform _carriage;

    bool _moveToPointB;
    bool _moving;
    bool _canMove;

    [SerializeField] ElevatorPoint _startPoint;
    [SerializeField] float _tripTime;
    float _currentTime;

    [SerializeField] float _resetTime = 1f;
    CountdownTimer _movementResetTimer;

    [SerializeField]
    string _keystring;

    private void Awake()
    {
        _movementResetTimer = new CountdownTimer(_resetTime, true, false);
        _movementResetTimer.RegisterEvent(() => { StopReset(); });

        if (_startPoint == ElevatorPoint.A)
        {
            _carriage.position = _pointA.position;
            _moveToPointB = true;
        }
        else
        {
            _carriage.position = _pointB.position;
            _moveToPointB = false;
        }

        _canMove = true;
        _moving = false;
    }

    private void FixedUpdate()
    {
        if (_moving && _canMove)
            Step();
    }

    private void Step()
    {
        if (_moveToPointB)
        {
            _carriage.position = Vector3.Lerp(_pointA.position, _pointB.position, _currentTime/_tripTime);
        }
        else
            _carriage.position = Vector3.Lerp(_pointB.position, _pointA.position, _currentTime / _tripTime);

        if (_currentTime < _tripTime)
            _currentTime += Time.fixedDeltaTime;
        
        if (_currentTime > _tripTime)
        {
            _currentTime = _tripTime;
        }

        if(_currentTime == _tripTime)
        {
            StopElevator();
            StartReset();
        }
    }

    public void StartElevator(Transform interactor)
    {
        if(!_moving && _canMove)
        _moving = true;
    }

    private void StopElevator()
    {
        _moving = false;
        _canMove = false;
        _moveToPointB = !_moveToPointB;
        _currentTime = 0;
    }

    private void StartReset()
    {
        _movementResetTimer.Resume();
    }

    private void StopReset()
    {
        _movementResetTimer.Reset();
        _movementResetTimer.Pause();
        _canMove = true;
    }

    private enum ElevatorPoint
    {
        A,
        B
    }
}
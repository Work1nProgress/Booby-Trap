using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    [SerializeField] Transform _pointA;
    [SerializeField] Transform _pointB;
    [SerializeField] Transform _carriage;
    [SerializeField] Transform _interactCanvas;

    bool _moveToPointB, _moving, _canMove, _interactorNear;

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

        if(_interactCanvas != null)
            _interactCanvas.gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (_moving && _canMove)
            Step();

        if(_interactorNear && _canMove && !_moving)
            _interactCanvas.gameObject.SetActive(true);
        else
            _interactCanvas.gameObject.SetActive(false);

        _interactorNear = false;
    }

    private bool VerifyKeycard()
    {
        return ControllerSaveLoad.GetSaveData.Keycards.Contains(_keystring);
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
        if(VerifyKeycard())
            if (!_moving && _canMove)
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

    public void InteractorNear()
    {
        if(VerifyKeycard())
            _interactorNear = true;
    }

    private enum ElevatorPoint
    {
        A,
        B
    }
}
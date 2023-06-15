using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Elevator : MonoBehaviour
{
    [SerializeField] Transform _pointA;
    [SerializeField] Transform _pointB;
    [SerializeField] Transform _carriage;
    [SerializeField] Transform _interactCanvas;

    bool _moveToPointB, _moving, _canMove, _interactorNear, _isPaused;

    [SerializeField] ElevatorPoint _startPoint;
    [SerializeField] float _tripTime;
    float _currentTime;

    [SerializeField] float _resetTime = 1f;
    CountdownTimer _movementResetTimer;

    [SerializeField]
    string _keystring;

    [SerializeField]
    Rigidbody2D rb;

    Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
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
        if (_moving && _canMove && !_isPaused)
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
            var pos = Vector3.Lerp(_pointA.position, _pointB.position, _currentTime / _tripTime);
            rb.MovePosition(pos);
        }
        else
        {
            var pos = Vector3.Lerp(_pointB.position, _pointA.position, _currentTime / _tripTime);
            rb.MovePosition(pos);
        }

        if (_currentTime < _tripTime)
            _currentTime += Time.fixedDeltaTime;
        
        if (_currentTime > _tripTime)
        {
            _currentTime = _tripTime;
        }

        if(_currentTime == _tripTime)
        {
            _animator.SetBool("Moving", false);
            SoundManager.Instance.Play("Elevator_End", transform);
            DOVirtual.DelayedCall(0.2f, () => SoundManager.Instance.CancelLoop(gameObject));

            StopElevator();
            StartReset();
        }
    }

    public void StartElevator()
    {
        if (VerifyKeycard())
        {
            if (!_moving && _canMove)
            {
                _animator.SetBool("Moving", true);
                _moving = true;
                SoundManager.Instance.Play("Elevator_Start", transform);
                DOVirtual.DelayedCall(1.8f, () => SoundManager.Instance.PlayLooped("Elevator_Loop", gameObject, transform));
                
            }
        }
        else
        {
            SoundManager.Instance.Play("Error", ControllerGame.Instance.player.transform);
        }
    }

    private void StopElevator()
    {
        _moving = false;
        _canMove = false;
        _moveToPointB = !_moveToPointB;
        _currentTime = 0;
    }

    public void Pause(bool isPaused)
    {
        _isPaused = isPaused;

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
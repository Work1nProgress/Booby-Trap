using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
[CreateAssetMenu(fileName = "PatrolWaypointsState", menuName = "Entities/States/Patrol Waypoints State")]
public class PatrolWaypointsState : EntityState
{
    private int _currentWaypoint;
    private float waypointMarginOfError = 2f;

    [SerializeField]
    bool IsIdle;

    private Transform[] _waypoints;


    public override void EnterState()
    {
        SoundManager.Instance.PlayLooped(_controller.Sound.PassiveLoop, _controller.gameObject, _controller.transform);
        base.EnterState();
    }

    public override void ExitState()
    {
        SoundManager.Instance.CancelLoop(_controller.Sound.PassiveLoop, _controller.gameObject);
        base.ExitState();
    }

    public void AssignWaypoints(Transform[] waypoints, Vector3 position)
    {
        _waypoints = waypoints;
        var distance= 9999999999;
        for (int i=0 ; i < _waypoints.Length;i++) {
            if (Vector3.Distance(_waypoints[i].position, position) < distance)
            {
                _currentWaypoint = i;
            }

        }
    }


    public override void FixedUpdateState(float deltaTime)
    {
        base.FixedUpdateState(deltaTime);

        if (IsIdle)
        {
            MoveToNextWaypoint();
            return;
        }
        bool playerCheck = CheckForPlayer(true);

        _controller.PlayerDetected(playerCheck);
        if (playerCheck)
        {
            ToNextState();
        }
        else
        {
            MoveToNextWaypoint();
            
        }

    }

    private void MoveToNextWaypoint()
    {
        if (_waypoints.Length < 2) return;

        var distanceToCurrentWaypoint = Vector2.Distance(_waypoints[_currentWaypoint].position, _controller.Rigidbody.position);
        Vector2 direction;
        if (distanceToCurrentWaypoint < waypointMarginOfError)
        {
            _currentWaypoint++;
            if (_currentWaypoint >= _waypoints.Length)
            {
                _currentWaypoint = 0;
            }
        }

        direction = ((Vector2)_waypoints[_currentWaypoint].position - _controller.Rigidbody.position).normalized;
        MoveArial(direction, _controller.Stats.MovementSpeed);
    }

    

}

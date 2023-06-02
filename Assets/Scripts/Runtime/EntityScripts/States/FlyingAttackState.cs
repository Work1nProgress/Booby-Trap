using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;


[CreateAssetMenu(fileName = "FlyingAttackState", menuName = "Entities/States/Flying Attack State")]
public class FlyingAttackState : EntityState
{
    private Seeker _seeker;
    private Path _path;

    private float nextPathpointDistance = 1f;


    private int _currentPathpoint = 0;
    private bool _reachedEndOfPath = false;

    float UpdatePathTimer;


    public override void EnterState()
    {

        SoundManager.Instance.PlayLooped(_controller.Sound.AgressiveLoop, _controller.gameObject, _controller.transform);
        base.EnterState();
    }

    public override void ExitState()
    {
        SoundManager.Instance?.CancelLoop(_controller.Sound.AgressiveLoop, _controller.gameObject);
        base.ExitState();
    }

    public void AssignPathFinding(Seeker seeker)
    {
        _seeker = seeker;
    }

    private void UpdatePath()
    {


        if (_seeker.IsDone())
        {
            _seeker.StartPath(_controller.Rigidbody.position, ControllerGame.Instance.player.RigidBody.position, OnPathComplete);
        }
    }
    private void OnPathComplete(Path p)
    {
        if (p.error) return;

        _path = p;
        _currentPathpoint = 0;
    }

    public override void FixedUpdateState(float deltaTime)
    {
        base.FixedUpdateState(deltaTime);
        UpdatePathTimer -= deltaTime;
        if (UpdatePathTimer <= 0)
        {
            UpdatePath();
            UpdatePathTimer = 0.5f;
        }
        ChaseThePlayer();
    }

    private void ChaseThePlayer()
    {
        if (_path == null) return;

        _reachedEndOfPath = _currentPathpoint >= _path.vectorPath.Count;

        // handle this that after one attack of the player there is a timeout where the harpy loses aggro
        try
        {
            Vector2 direction = ((Vector2)_path.vectorPath[_currentPathpoint] - _controller.Rigidbody.position).normalized;
            MoveArial(direction, _controller.Stats.MovementSpeedChase);
        }
        catch
        {
            var playerCheck = CheckForPlayer(true);
            if (!playerCheck)
            {
                ToAltState();
            }
            return;
        }




        float distance = Vector2.Distance(_controller.Rigidbody.position, _path.vectorPath[_currentPathpoint]);
        if (distance < nextPathpointDistance)
        {
            _currentPathpoint++;
        }

 
    }
}

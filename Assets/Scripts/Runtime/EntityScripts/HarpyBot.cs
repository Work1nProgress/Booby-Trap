using Pathfinding;
using UnityEngine;

public class HarpyBot : EnemyBase
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float nextPathpointDistance = 3f;
    [SerializeField] private GameObject spriteObject;
    [SerializeField] private float lookRadius = 7;
    [SerializeField] private float losePlayerRadius = 12;
    [SerializeField] private float waypointMarginOfError = 2f;

    private int _currentWaypoint;
    private bool _playerDetected = false;

    private Path _path;
    private Seeker _seeker;
    private int _currentPathpoint = 0;
    private bool _reachedEndOfPath = false;

    [SerializeField] private Transform _player;

    public void Awake()
    {
       
        if (waypoints.Length > 2)
        {
            _currentWaypoint = 0;
        }
        
        _seeker = GetComponent<Seeker>();

       
    }

    private void OnPathComplete(Path p)
    {
        if (p.error) return;
        
        _path = p;
        _currentPathpoint = 0;
    }

    protected override void FixedUpdate()
    {
        LookForThePlayer();
        
        if (!_playerDetected)
        {
            MoveToNextWaypoint();
        }
        else
        {
            ChaseThePlayer();
        }
        base.FixedUpdate();

    }

    private void UpdatePath()
    {
        if (_seeker.IsDone())
        {
            _seeker.StartPath(Rigidbody.position, _player.position, OnPathComplete);
        }
    }

    private void LookForThePlayer()
    {
        var distanceToPlayer = Vector3.Distance(_player.position, transform.position);
        if (distanceToPlayer < lookRadius  && !_playerDetected)
        {

            _playerDetected = true;
            InvokeRepeating(nameof(UpdatePath), 0f, .5f);
        } else if (distanceToPlayer > losePlayerRadius)
        {
            _playerDetected = false;
            CancelInvoke(nameof(UpdatePath));
        }
    }

    private void ChaseThePlayer()
    {
        if (_path == null) return;

        _reachedEndOfPath = _currentPathpoint >= _path.vectorPath.Count;

        // handle this that after one attack of the player there is a timeout where the harpy loses aggro
        try
        {
            Vector2 direction = ((Vector2)_path.vectorPath[_currentPathpoint] - Rigidbody.position).normalized;
            Move(direction);
        }
        catch
        {
            _playerDetected = false;
            return;
        }
        
        
        

        float distance = Vector2.Distance(Rigidbody.position, _path.vectorPath[_currentPathpoint]);
        if (distance < nextPathpointDistance)
        {
            _currentPathpoint++;
        }
        
        spriteObject.transform.localScale = Rigidbody.velocity.x >= 0.01f ? new Vector3(-1f, 1f, 1f) : new Vector3(1f, 1f, 1f);
    }

    private void Move(Vector2 direction)
    {
        Vector2 force = direction * (moveSpeed * Time.deltaTime);
        Rigidbody.AddForce(force);
    }

    private void MoveToNextWaypoint()
    {
        if(waypoints.Length < 2) return;

        var distanceToCurrentWaypoint = Vector2.Distance(waypoints[_currentWaypoint].position, Rigidbody.position);
        Vector2 direction;
        if (distanceToCurrentWaypoint < waypointMarginOfError)
        {
            _currentWaypoint++;
            if (_currentWaypoint >= waypoints.Length)
            {
                _currentWaypoint = 0;
            }
        }
        
        direction = ((Vector2)waypoints[_currentWaypoint].position - Rigidbody.position).normalized;
        
        Move(direction);
    }
}

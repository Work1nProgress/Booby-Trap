using Pathfinding;
using UnityEngine;

public class HarpyBot : EnemyBase
{
    [SerializeField]
    private Seeker _seeker;

    [SerializeField]
    private Transform[] _waypoints;


    public override void Init(EntityStats stats)
    {
        base.Init(stats);
        Rigidbody.gravityScale = 0;
    }


    protected override void BeforeInitState(EntityState state)
    {
        switch (state)
        {
            case FlyingAttackState attackState:
                attackState.AssignPathFinding(_seeker);
                break;

            case PatrolWaypointsState patrolWaypointsState:
                patrolWaypointsState.AssignWaypoints(_waypoints, transform.position);
                break;
        }
    }



}

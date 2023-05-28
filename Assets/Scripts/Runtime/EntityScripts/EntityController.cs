using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Rigidbody2D))]
public class EntityController : StateHandler
{
    private Rigidbody2D _rigidbody;
    public Rigidbody2D Rigidbody => _rigidbody;

    EnemyStats _enemyStats;
    public EnemyStats Stats => _enemyStats;

    public float MovementSpeed => Stats.MovementSpeed;

    protected bool _isGrounded;
    public bool Grounded => _isGrounded;

    private bool _isJumping;
    public bool Jumping => _isJumping;

    private CountdownTimer _jumpResetTimer;

    private const float Gravity = 9.81f;
    private Vector2 _velocity = Vector2.zero;

    private EntityRotationDirection _upwardDirection = EntityRotationDirection.UP;

    private bool _HasTarget;
    public bool HasTarget => _HasTarget;
    protected int GroundLayer;

    [SerializeField]
    SpriteRenderer _sprite;
    public SpriteRenderer Sprite => _sprite;


    public virtual void Init(EnemyStats Stats)
    {
        _enemyStats = Stats;
        Init(Stats as EntityStats);
    }


    public override void Init(EntityStats stats)
    {

        base.Init(stats);

        _jumpResetTimer = new CountdownTimer(0.35f, true, false, ResetJump);

       
        _rigidbody = GetComponent<Rigidbody2D>();
     
        GroundLayer = LayerMask.GetMask("Ground");

        InitStateHandler(this);
    }

    protected override void OnKill()
    {
        base.OnKill();
        _jumpResetTimer.Dispose();
    }

    protected override void FixedUpdate()
    {

      //  _isGrounded = CheckForGround();
       // ApplyGravity(Time.fixedDeltaTime);
       // ApplyVelocity(Time.fixedDeltaTime, _upwardDirection);

        base.FixedUpdate();
    }

    private GameObject OtherObject(RaycastHit2D hit) => hit.collider != null ? hit.collider.gameObject : null;
    private bool CheckForGround()
    {
        Debug.DrawRay(transform.position, transform.up * -1 * 0.55f, Color.red);

        if (!_isJumping)
        {
            RaycastHit2D hitleft = Physics2D.Raycast(transform.position - (Vector3.right * 0.5f), transform.up * -1, 0.55f, GroundLayer);
            RaycastHit2D hitright = Physics2D.Raycast(transform.position + (Vector3.right * 0.5f), transform.up * -1, 0.55f, GroundLayer);
            if (OtherObject(hitleft) != null || OtherObject(hitright) != null)
                return true;
        }

        return false;
    }

    private void ApplyGravity(float deltaTime)
    {
        if(!_isJumping)
            _velocity.y = !_isGrounded ?
            _velocity.y - Gravity * deltaTime :
            0;
    }


    private void ApplyVelocity(float deltaTime, EntityRotationDirection upDirection)
    {
        _rigidbody.position = _rigidbody.position +
            _velocity * 
            deltaTime;
    }

    

    public virtual void Move(float speed, bool right)
    {
        Vector2 direction = new Vector2(
            right ? speed * Time.fixedDeltaTime : speed * Time.fixedDeltaTime * -1, 0);

        _rigidbody.MovePosition(_rigidbody.position + direction);
    }

    public virtual void Jump(float power)
    {
        if (_isGrounded)
        {
            _isJumping = true;
            _jumpResetTimer.Resume();
            _velocity.y = power;
        }
    }

    public override void Destroy()
    {
        _jumpResetTimer.Dispose();
        base.Destroy();
    }

    private void ResetJump()
    {
        _isJumping = false;
        _jumpResetTimer.Reset();
        _jumpResetTimer.Pause();
    }

    public void PlayerDetected(bool hasTarget)
    {
        _HasTarget = hasTarget;
    }
}

public enum EntityRotationDirection
{
    UP,
    RIGHT,
    DOWN,
    LEFT
}
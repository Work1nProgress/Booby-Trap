using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EntityController : StateHandler
{
    private Rigidbody2D _rigidbody;
    public Rigidbody2D Rigidbody => _rigidbody;

    [Header("Entity Variables")]
    [SerializeField] private float _movementSpeed;
    public float MovementSpeed => _movementSpeed;

    protected bool _isGrounded;
    public bool Grounded => _isGrounded;

    private bool _isJumping;
    public bool Jumping => _isJumping;

    private CountdownTimer _jumpResetTimer;

    private const float Gravity = 9.81f;
    private Vector2 _velocity = Vector2.zero;

    private EntityRotationDirection _upwardDirection = EntityRotationDirection.UP;

    public override void Init(EntityStats stats)
    {
        base.Init(stats);

        _jumpResetTimer = new CountdownTimer(0.35f, true, false, ResetJump);

        _rigidbody = GetComponent<Rigidbody2D>();
        _rigidbody.gravityScale = 0;

        InitStateHandler(this);
    }

    protected override void FixedUpdate()
    {
        HandleRotation(_upwardDirection);

        _isGrounded = CheckForGround();
        ApplyGravity(Time.fixedDeltaTime);
        ApplyVelocity(Time.fixedDeltaTime, _upwardDirection);

        base.FixedUpdate();
    }

    private GameObject OtherObject(RaycastHit2D hit) => hit.collider.gameObject;
    private bool CheckForGround()
    {
        Debug.DrawRay(transform.position, transform.up * -1 * 0.55f, Color.red);

        if (!_isJumping)
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, transform.up * -1, 0.55f);
            foreach (RaycastHit2D hit in hits)
            {
                if (OtherObject(hit) != gameObject &&
                    OtherObject(hit) != null)
                    return true;
            }
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
            RotatedVelocity(upDirection) * 
            deltaTime;
    }

    private void HandleRotation(EntityRotationDirection direction)
    {
        _rigidbody.rotation = RotationAngle(direction);
    }

    private float RotationAngle(EntityRotationDirection upDirection)
    {
        switch (upDirection)
        {
            case EntityRotationDirection.UP: return 0;
            case EntityRotationDirection.RIGHT: return -90;
            case EntityRotationDirection.DOWN: return -180;
            case EntityRotationDirection.LEFT: return 90;
        }
        return 0;
    }

    private Vector2 RotatedVelocity(EntityRotationDirection upDirection)
    {
        Vector2 velocity = Vector2.zero;

        switch (upDirection)
        {
            case EntityRotationDirection.UP:
                velocity = _velocity;
                return velocity;
            case EntityRotationDirection.RIGHT:
                velocity.x = _velocity.y;
                velocity.y = _velocity.x;
                return velocity;
            case EntityRotationDirection.DOWN:
                velocity.y = _velocity.y * -1;
                velocity.x = _velocity.x * -1;
                return velocity;
            case EntityRotationDirection.LEFT:
                velocity.x = _velocity.y * -1;
                velocity.y = _velocity.x * -1;
                return velocity;
        }

        return velocity;
    }

    public void Jump(float power)
    {
        if (_isGrounded)
        {
            _isJumping = true;
            _jumpResetTimer.Resume();
            _velocity.y = power;
        }
    }

    private void ResetJump()
    {
        _isJumping = false;
        _jumpResetTimer.Reset();
        _jumpResetTimer.Pause();
    }
}

public enum EntityRotationDirection
{
    UP,
    RIGHT,
    DOWN,
    LEFT
}
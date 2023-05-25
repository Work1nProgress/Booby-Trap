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

    private Transform _target;
    public Transform Target => _target;

    public override void Init(EntityStats stats)
    {
        base.Init(stats);

        _jumpResetTimer = new CountdownTimer(0.35f, true, false, ResetJump);

        _rigidbody = GetComponent<Rigidbody2D>();
        _rigidbody.gravityScale = 0;

        GameObject playerObj = GameObject.Find("Echo");
        _target = playerObj != null ? playerObj.transform : null;

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

    private GameObject OtherObject(RaycastHit2D hit) => hit.collider != null ? hit.collider.gameObject : null;
    private bool CheckForGround()
    {
        Debug.DrawRay(transform.position, transform.up * -1 * 0.55f, Color.red);

        if (!_isJumping)
        {
            RaycastHit2D hitleft = Physics2D.Raycast(transform.position - (Vector3.right * 0.5f), transform.up * -1, 0.55f, LayerMask.GetMask("Ground"));
            RaycastHit2D hitright = Physics2D.Raycast(transform.position + (Vector3.right * 0.5f), transform.up * -1, 0.55f, LayerMask.GetMask("Ground"));
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
            RotatedVelocity(upDirection) * 
            deltaTime;
    }

    public void RotateClockwise()
    {
        switch (_upwardDirection)
        {
            case EntityRotationDirection.UP: _upwardDirection = EntityRotationDirection.RIGHT; return;
            case EntityRotationDirection.RIGHT: _upwardDirection = EntityRotationDirection.DOWN; return;
            case EntityRotationDirection.DOWN: _upwardDirection = EntityRotationDirection.LEFT; return;
            case EntityRotationDirection.LEFT: _upwardDirection = EntityRotationDirection.UP; return;
        }
    }

    public void RotateCounterClockwise()
    {
        switch (_upwardDirection)
        {
            case EntityRotationDirection.UP: _upwardDirection = EntityRotationDirection.LEFT; return;
            case EntityRotationDirection.RIGHT: _upwardDirection = EntityRotationDirection.UP; return;
            case EntityRotationDirection.DOWN: _upwardDirection = EntityRotationDirection.RIGHT; return;
            case EntityRotationDirection.LEFT: _upwardDirection = EntityRotationDirection.DOWN; return;
        }
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

    private Vector2 RotatedVector(Vector2 vector)
    {
        Vector2 holderVector = Vector2.zero;

        switch (_upwardDirection)
        {
            case EntityRotationDirection.UP:
                holderVector = vector;
                return holderVector;
            case EntityRotationDirection.RIGHT:
                holderVector.x = vector.y;
                holderVector.y = vector.x;
                return holderVector;
            case EntityRotationDirection.DOWN:
                holderVector.y = vector.y * -1;
                holderVector.x = vector.x * -1;
                return holderVector;
            case EntityRotationDirection.LEFT:
                holderVector.x = vector.y * -1;
                holderVector.y = vector.x * -1;
                return holderVector;
        }

        return holderVector;
    }

    public void SetRotation(EntityRotationDirection upwardDriecttion)
    {
        _upwardDirection = upwardDriecttion;
    }

    public virtual void Move(float speed, bool right)
    {
        Vector2 direction = RotatedVector(new Vector2(
            right ? speed * Time.fixedDeltaTime : speed * Time.fixedDeltaTime * -1, 0));

        _rigidbody.MovePosition(_rigidbody.position + RotatedVector(direction));
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

    private void ResetJump()
    {
        _isJumping = false;
        _jumpResetTimer.Reset();
        _jumpResetTimer.Pause();
    }

    public void PlayerDetected(Transform playerTransform)
    {
        _target = playerTransform;
    }
}

public enum EntityRotationDirection
{
    UP,
    RIGHT,
    DOWN,
    LEFT
}
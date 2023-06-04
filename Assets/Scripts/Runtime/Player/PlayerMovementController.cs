using UnityEngine;


public class PlayerMovementController : MonoBehaviour
{
    [Header("Running")]
    [SerializeField]
    [Tooltip("Top speed")]
    float m_RunSpeed = 10;
    [SerializeField]
    [Tooltip("Horizontal movement acceleration")]
    float runAcceleration = 5;
    [SerializeField]
    [Tooltip("Horizontal movement decceleration")]
    float runDecceleration = 5;

    [Header("Jumping")]
    [Tooltip("Jumping force happens once")]
    [SerializeField]
    float jumpForce = 10;

    [SerializeField]
    [Tooltip("Multiplier for horizontal movement acceleration in air")]
    float jumpAcceleration = 1;
    [SerializeField]
    [Tooltip("Multiplier for horizontal movement decceleration in air")]
    float jumpDecceleration = 1;

    [Header("Gravity")]
    [Tooltip("Overall gravity multiplier")]
    [SerializeField]
    float gravityMultiplier = 1;
    [SerializeField]
    [Tooltip("Gravity multiplier while jumping and holding jump")]
    float jumpGravityJumpHeld = 1.7f;
    [SerializeField]
    [Tooltip("Gravity multiplier while jumping and not holding jump")]
    float jumpGravity = 3f;
    [SerializeField]
    [Tooltip("Gravity multiplier while falling and holding jump")]

    float fallingGravityJumpHeld = 2f;
    [SerializeField]
    [Tooltip("Gravity multiplier while falling and not holding jump")]
    float fallingGravity = 4f;
    [SerializeField]
    [Tooltip("Gravity multiplier while falling after dashing")]
    float fallingGravityAfterDash = 1.7f;

    [SerializeField]

    [Tooltip("Gravity multiplier while falling after dashing")]
    float maximumFallSpeed = 50f;

    [Header("Hang Times")]
    [SerializeField]
    [Tooltip("Vertical velocity threshold at apex of jump to toggle better manouverability")]
    float hangTimeThreshold = 0.1f;
    [SerializeField]
    [Tooltip("Extra acceleration at apex")]
    float hangTimeAccelerationMult = 1.1f;
    [SerializeField]
    [Tooltip("Extra top speed at apex")]
    float hangTimeSpeedMult = 1.1f;

    [SerializeField]
    [Tooltip("Meep Meep")]
    float coyoteTime = 0.7f;

    [SerializeField]
    Rigidbody2D m_RigidBody;

    public Rigidbody2D RigidBody => m_RigidBody;

    [SerializeField]
    SpriteRenderer m_PlayerSprite;


    [Header("Sound")]
    [SerializeField]
    string JumpSound;
    [SerializeField]
    string LandSound;
    [SerializeField]
    string StepsSound;


    [SerializeField]
    float stepDelay = 0.5f;
    [SerializeField]
    float stepVelocityFactor = 0.1f;


    public Vector2 lastGroundedPosition;


    bool allowDashing = false;
    float dashSpeed = 500;
    float dashAcceleration = 10;
    float dashCooldown = 3.0f;
    float dashDuration = 0.1f;
    private Vector2 dashCheckSize = new Vector2(0.49f, 0.03f);
    private Vector3 dashCheckPointLeft;
    private Vector3 dashCheckPointRight;


    [Header("Layers")]
    public LayerMask groundLayer;

    [Header("Ground Checks")]
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.49f, 0.03f);
    [SerializeField] private Vector3 groundCheckPoint;



    private bool jumping = false;
    private bool falling = false;
    private bool fallingFromDash = false;
    private bool standingOnSpear = false;


    private float inputX, inputY;
    private bool jumpHeld = false;
    private bool dashPressed = false;

    float LastOnGroundTime;
    float LastDashTime;
    float LastDashDurationTime;

    float StepsTimer;

    float dashDirection;
    float canDashDirection;


    bool feetTouchingGround;
    public bool OnGround => LastOnGroundTime > 0;
    public bool Dashing => LastDashDurationTime > 0;

    public bool FacingRight => !m_PlayerSprite.flipX;



    private void OnEnable()
    {
        ControllerInput.Instance.Horizontal.AddListener(OnHorizontal);
        ControllerInput.Instance.Vertical.AddListener(OnVertical);
        ControllerInput.Instance.Jump.AddListener(OnJump);
    }

    private void OnDisable()
    {
        ControllerInput.Instance.Horizontal.RemoveListener(OnHorizontal);
        ControllerInput.Instance.Vertical.RemoveListener(OnVertical);
        ControllerInput.Instance.Jump.RemoveListener(OnJump);
    }


    // Update is called once per frame
    void Update()
    {
        LastOnGroundTime -= Time.deltaTime;
        LastDashTime -= Time.deltaTime;
        LastDashDurationTime -= Time.deltaTime;
        StepsTimer -= Time.deltaTime * Mathf.Abs(m_RigidBody.velocity.x) * stepVelocityFactor;
        if (allowDashing)
        {
            dashPressed = Input.GetButtonDown("Fire2") || Input.GetKeyDown(KeyCode.K);
        }

        GroundCheck();

        if (feetTouchingGround && Mathf.Abs(m_RigidBody.velocity.x) > 0.1f && StepsTimer < 0)
        {
            SoundManager.Instance.Play(StepsSound, transform);
            StepsTimer = stepDelay;
        }
    }

    private void FixedUpdate()
    {

        UpdateVerticalMovement();

        //UPDATE HORIZONTAL MOVEMENT
        float targetSpeed;

        //Calculate the direction we want to move in and our desired velocity
        if (Dashing)
        {
            targetSpeed = dashDirection * dashSpeed;
        }
        else
        {
            {
                targetSpeed = inputX * m_RunSpeed;
            }
        }
        //We can reduce are control using Lerp() this smooths changes to are direction and speed
        targetSpeed = Mathf.Lerp(m_RigidBody.velocity.x, targetSpeed, 1);

        float accelerationRate;

        if (Dashing)
        {
            accelerationRate = dashAcceleration;
        }
        else if (OnGround)
        {

            accelerationRate = (Mathf.Abs(targetSpeed) > 0.01f) ? runAcceleration : runDecceleration;

        }
        else
        {
            accelerationRate = (Mathf.Abs(targetSpeed) > 0.01f) ? runAcceleration * jumpAcceleration : runDecceleration * jumpDecceleration;
        }

        if (!Dashing && !OnGround && Mathf.Abs(m_RigidBody.velocity.y) < hangTimeThreshold)
        {
            accelerationRate *= hangTimeAccelerationMult;
            targetSpeed *= hangTimeSpeedMult;
        }


        //Calculate difference between current velocity and desired velocity
        float speedDif = targetSpeed - m_RigidBody.velocity.x;
        //Calculate force along x-axis to apply to thr player

        float movement = speedDif * accelerationRate;

        //Convert this to a vector and apply to rigidbody
        m_RigidBody.AddForce(movement * Vector2.right, ForceMode2D.Force);
        if (Mathf.Abs(m_RigidBody.velocity.x) > 0.1f)
        {
            m_PlayerSprite.flipX = m_RigidBody.velocity.x < 0;
        }
    }

    private void UpdateVerticalMovement()
    {



        //not possible if dashing not allowed
        if (Dashing)
        {
            m_RigidBody.gravityScale = 0;
            return;
        }
       



       //not sure this does anything
        if (OnGround && jumping && m_RigidBody.velocity.y <= 0)
        {
            jumping = false;
          
        }
        else
        {

            
            //we are falling
            if (m_RigidBody.velocity.y <= 0)
            {
                falling = true;
                jumping = false;
                // apply correct gravity scale
                if (jumpHeld)
                {
                    m_RigidBody.gravityScale = gravityMultiplier * fallingGravityJumpHeld;

                }
                else if (fallingFromDash)
                {
                    m_RigidBody.gravityScale = gravityMultiplier * fallingGravityAfterDash;
                }
                else
                {
                    m_RigidBody.gravityScale = gravityMultiplier * fallingGravity;
                }
                m_RigidBody.velocity = new Vector2(m_RigidBody.velocity.x, Mathf.Clamp(m_RigidBody.velocity.y, -maximumFallSpeed, 0));
            }
            // apply correct gravity scale for jump
            else if(!jumpHeld)
            {
                m_RigidBody.gravityScale = gravityMultiplier * jumpGravity;
            }

        }


    }




    bool wasDashing;
    private void UpdateDash()
    {
        //dash cooldown ended and dash input pressed
        if (LastDashTime <= 0 && dashPressed && !fallingFromDash && allowDashing)
        {
            //if player is holding a direction
            if (Mathf.Abs(inputX) > 0)
            {
                dashDirection = inputX;
            }
            //use the current facing direction
            else
            {
                dashDirection = FacingRight ? 1 : -1;
            }
            //check if player is already colliding with a wall
            if (canDashDirection == 0 || dashDirection != canDashDirection)
            {
                LastDashDurationTime = dashDuration;
                LastDashTime = dashCooldown;

                m_RigidBody.velocity = new Vector2(m_RigidBody.velocity.x, 0);
                jumping = false;
            }
            else
            {
                dashDirection = 0;

            }
        }

        if (wasDashing && !Dashing)
        {
            EndDash();
        }
        wasDashing = Dashing;
    }

    public void TeleportToLastGround()
    {

        m_RigidBody.velocity = default;
        //TODO fix this so we teleport to the center of the tile
        // var centerPosition = new Vector3(Utils.WorldPositionToTile(lastGroundedPosition.x)+0.5f, lastGroundedPosition.y, 0);
        transform.position = lastGroundedPosition;//centerPosition;
    }

    private void GroundCheck()
    {
        if (allowDashing)
        {
            canDashDirection = 0;
            var dashCanceledLeft = Physics2D.OverlapBox(transform.position + dashCheckPointLeft, dashCheckSize, 0, groundLayer);
            if (dashCanceledLeft)
            {
                if (Dashing && dashDirection < 0)
                {
                    EndDash();
                }
                canDashDirection -= 1;
            }

            var dashCanceledRight = Physics2D.OverlapBox(transform.position + dashCheckPointRight, dashCheckSize, 0, groundLayer);
            if (dashCanceledRight)
            {
                if (Dashing && dashDirection > 0)
                {
                    EndDash();
                }
                canDashDirection += 1;
            }
        }

        var collider = Physics2D.OverlapBox(transform.position + groundCheckPoint, groundCheckSize, 0, groundLayer);
        if (collider != null && !jumping)
        {

            if (!feetTouchingGround && falling)
            {

                PoolManager.Spawn<PoolObjectTimed>("dustparticles", null, transform.position + groundCheckPoint).StartTicking();
                SoundManager.Instance.Play(LandSound, transform);
                
            }
            feetTouchingGround = true;
            lastGroundedPosition = m_RigidBody.position;
   
            standingOnSpear = collider.gameObject.CompareTag("Spear");
            LastOnGroundTime = coyoteTime;
            fallingFromDash = false;

            falling = false;
            m_RigidBody.gravityScale = gravityMultiplier * fallingGravity;
        }
        else
        {
            feetTouchingGround = false;
            standingOnSpear = false;
        }

    }

    void EndDash()
    {
        LastDashDurationTime = 0;
        LastDashTime = dashCooldown;
        dashDirection = 0;
        LastOnGroundTime = 0;
        wasDashing = false;
        if (!OnGround)
        {
            fallingFromDash = true;
            m_RigidBody.gravityScale = gravityMultiplier * fallingGravityAfterDash;
        }
    }


    #region input
    void OnJump(bool value)
    {

        jumpHeld = value;
        if (value && OnGround && !jumping)
        {
            if (inputY < 0 && standingOnSpear)
            {
                return;
            }
            SoundManager.Instance.Play(JumpSound, transform);
            jumping = true;
            PoolManager.Spawn<PoolObjectTimed>("dustparticles", null, transform.position + groundCheckPoint).StartTicking();

            //no more coyote time
            LastOnGroundTime = 0;

            m_RigidBody.gravityScale = gravityMultiplier * jumpGravityJumpHeld;


            //more force applied if we are falling down (Wil. E. Coyote)
            m_RigidBody.AddForce(Vector2.up * (jumpForce - m_RigidBody.velocity.y), ForceMode2D.Impulse);
        }
    }

    

    void OnHorizontal(float value)
    {
        inputX = value;
    }

    void OnVertical(float value)
    {
        inputY = value;
    }

    #endregion



    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;

        Gizmos.DrawWireCube(transform.position + groundCheckPoint, groundCheckSize);
        if (allowDashing)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(transform.position + dashCheckPointLeft, dashCheckSize);
            Gizmos.DrawWireCube(transform.position + dashCheckPointRight, dashCheckSize);
        }
    }
}
